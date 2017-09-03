using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace N64Emu
{
    using static Helpers.BitHelper;

    // Reference: http://datasheets.chipdb.org/NEC/Vr-Series/Vr43xx/U10504EJ7V0UMJ1.pdf
    public partial class VR4300
    {
        #region Fields
        private readonly Nintendo64 nintendo64;
        private readonly IReadOnlyDictionary<OpCode, Action<Instruction>> operations;
        #endregion

        #region Properties
        /// <summary>
        /// General purpose registers.
        /// </summary>
        public ulong[] GPRegisters { get; } = new ulong[32];

        /// <summary>
        /// Floating-point operation registers.
        /// </summary>
        public double[] FPRegisters { get; } = new double[32];

        public ulong ProgramCounter { get; set; }

        /// <summary>
        /// Integer multiply and divide high-order double word result.
        /// </summary>
        public ulong HIRegister { get; set; }

        /// <summary>
        /// Integer multiply and divide low-order double word result.
        /// </summary>
        public ulong LORegister { get; set; }

        /// <summary>
        /// Load/Link 1-bit register.
        /// </summary>
        public bool LLBitRegister { get; set; }

        /// <summary>
        /// Implementation/Revision register.
        /// </summary>
        /// <value>The FCR.</value>
        public float FCR0 { get; set; }

        /// <summary>
        /// Control/Status register.
        /// </summary>
        public float FCR31 { get; set; }

        public Coprocessor0 CP0 { get; } = new Coprocessor0();
        #endregion

        #region Constructors
        public VR4300(Nintendo64 nintendo64)
        {
            this.nintendo64 = nintendo64;
            operations = new Dictionary<OpCode, Action<Instruction>>
            {
                [OpCode.LUI] = i => GPRegisters[i.RT] = (ulong)(i.Immediate << 16),
                [OpCode.MTC0] = i => CP0.Registers[i.RD] = GPRegisters[i.RT],
                [OpCode.ORI] = i => GPRegisters[i.RT] = GPRegisters[i.RS] | i.Immediate,
                [OpCode.LW] = i => // 'offset' is Immediate, 'base' is RS.
                {
                    var vAddr = SignExtend(i.Immediate) + GPRegisters[i.RS];
                    GPRegisters[i.RT] = SignExtend(ReadWord(new UIntPtr(vAddr)));
                },
                [OpCode.ANDI] = i => GPRegisters[i.RT] = (ulong)(i.Immediate & (ushort)GPRegisters[i.RS]),
                [OpCode.BEQL] = i =>
                {
                    var delaySlotInstruction = ReadWord(new UIntPtr(ProgramCounter));

                    if (GPRegisters[i.RS] == GPRegisters[i.RT])
                    {
                        ProgramCounter += SignExtend((ushort)(i.Immediate << 2));

                        Run(delaySlotInstruction);
                    }
                    else
                        ProgramCounter += sizeof(uint);
                },
                [OpCode.ADDIU] = i => GPRegisters[i.RT] = GPRegisters[i.RS] + SignExtend(i.Immediate),
                [OpCode.SW] = i => WriteWord(MapMemory(new UIntPtr(SignExtend(i.Immediate) + GPRegisters[i.RS])), (uint)GPRegisters[i.RT]),
                [OpCode.BNEL] = i =>
                {
                    var delaySlotInstruction = ReadWord(new UIntPtr(ProgramCounter));

                    if (GPRegisters[i.RS] != GPRegisters[i.RT])
                    {
                        ProgramCounter += SignExtend((ushort)(i.Immediate << 2));

                        Run(delaySlotInstruction);
                    }
                    else
                        ProgramCounter += sizeof(uint);
                },
                [OpCode.BNE] = i =>
                {
                    if (GPRegisters[i.RS] != GPRegisters[i.RT])
                        ProgramCounter += SignExtend(i.Immediate);
                }
            };
        }
        #endregion

        #region Methods
        public void PowerOnReset()
        {
            ProgramCounter = 0xFFFFFFFFBFC00000;

            CP0.PowerOnReset();
        }

        public void Run(Instruction instruction)
        {
            if (operations.TryGetValue(instruction.OP, out var operation))
                operation(instruction);
            else
                throw new Exception($"Unknown opcode (0b{Convert.ToString((byte)instruction.OP, 2)}) from instruction 0x{(uint)instruction:X}.");
        }

        public void Step()
        {
            var instruction = ReadWord(new UIntPtr(ProgramCounter));
            ProgramCounter += sizeof(uint);

            Run(instruction);
        }

        private uint ReadWord(UIntPtr virtualAddress)
        {
            var physicalAddress = MapMemory(virtualAddress);
            var entry = Nintendo64.MemoryMaps.FirstOrDefault(e => (uint)physicalAddress >= e.StartAddress && (uint)physicalAddress <= e.EndAddress);

            switch (entry.EntryName)
            {
                case Nintendo64.MappingEntry.Name.None:
                    switch ((uint)physicalAddress)
                    {
                        case Nintendo64.SPStatusRegisterAddress:
                            return nintendo64.RCP.RSP.StatusRegister;
                        case Nintendo64.SPDMABusyRegisterAddress:
                            return nintendo64.RCP.RSP.DMABusyRegister;
                    }
                    break;
                case Nintendo64.MappingEntry.Name.PIFBootROM:
                    return (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(nintendo64.PIFROM, (int)((uint)physicalAddress - entry.StartAddress)));
            }

            throw new Exception($"Unknown physical address: 0x{(uint)physicalAddress:X}.");
        }

        private void WriteWord(UIntPtr physicalAddress, uint word)
        {
            var entry = Nintendo64.MemoryMaps.FirstOrDefault(e => (uint)physicalAddress >= e.StartAddress && (uint)physicalAddress <= e.EndAddress);

            switch (entry.EntryName)
            {
                case Nintendo64.MappingEntry.Name.None:
                    switch ((uint)physicalAddress)
                    {
                        case Nintendo64.SPStatusRegisterAddress:
                            nintendo64.RCP.RSP.StatusRegister = word;
                            return;
                    }
                    break;
            }
        }

        /// <summary>
        /// Translates a virtual address into a physical address.
        /// See: datasheet#5.2.4 Table 5-3.
        /// </summary>
        /// <param name="address">The virtual address.</param>
        /// <returns>The physical address.</returns>
        public UIntPtr MapMemory(UIntPtr address) // TODO: move to an MMU, and CP0 relations ?
        {
            switch ((ulong)address >> 29 & 0b111)
            {
                case 0b100: // kseg0.
                    return new UIntPtr((ulong)address - 0xFFFFFFFF80000000);
                case 0b101: // kseg1.
                    return new UIntPtr((ulong)address - 0xFFFFFFFFA0000000);
                default:
                    throw new Exception($"Unknown memory map segment for location 0x{(ulong)address:X}.");
            }
        }
        #endregion
    }
}
