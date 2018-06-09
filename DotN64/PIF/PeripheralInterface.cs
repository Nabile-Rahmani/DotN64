using System;
using System.Collections.Generic;

namespace DotN64
{
    using CPU;
    using Extensions;
    using Helpers;

    public partial class PeripheralInterface
    {
        #region Fields
        private readonly Nintendo64 nintendo64;

        private const byte CICStatusOffset = 0x3C, DeviceStateOffset = 0x24;
        #endregion

        #region Properties
        public IReadOnlyList<MappingEntry> MemoryMaps { get; }

        public byte[] BootROM { get; set; }

        public byte[] RAM { get; } = new byte[64];

        private DeviceState DeviceStateFlags
        {
            get => BitConverter.ToUInt32(RAM, DeviceStateOffset);
            set => BitHelper.Write(RAM, DeviceStateOffset, value);
        }
        #endregion

        #region Constructors
        public PeripheralInterface(Nintendo64 nintendo64)
        {
            this.nintendo64 = nintendo64;
            MemoryMaps = new[]
            {
                new MappingEntry(0x1FC00000, 0x1FC007BF) // PIF Boot ROM.
                {
                    Read = o => BitHelper.FromBigEndian(BitConverter.ToUInt32(BootROM, (int)o))
                },
                new MappingEntry(0x1FC007C0, 0x1FC007FF) // PIF (JoyChannel) RAM.
                {
                    Read = o => BitConverter.ToUInt32(RAM, (int)o),
                    Write = (o, d) =>
                    {
                        BitHelper.Write(RAM, (int)o, d);
                        OnRAMWritten((int)o);
                    }
                }
            };
        }
        #endregion

        #region Methods
        /// <summary>
        /// Called when a word is written at a specified index in the RAM.
        /// Handles actions sent through memory writes.
        /// </summary>
        private void OnRAMWritten(int index)
        {
            switch (index)
            {
                case CICStatusOffset:
                    if (RAM[index] == (byte)CICStatus.Waiting) // The boot ROM waits for the PIF's CIC check to be OK.
                        RAM[index] = (byte)CICStatus.OK; // We tell it it's OK by having the loaded word that gets ANDI'd match the immediate value 128, storing non-zero which allows us to exit the BEQL loop.
                    break;
            }
        }

        private void EmulateBootROM()
        {
            // Replicating the memory writes to properly initialise the subsystems.
            var writes = new uint[,]
            {
                { 0x4040010, 0xA },
                { 0x4600010, 0x3 },
                { 0x440000C, 0x3FF },
                { 0x4400024, 0x0 },
                { 0x4400010, 0x0 },
                { 0x4500000, 0x0 },
                { 0x4500004, 0x0 },
                { 0x4600014, 0x40 }, // These four are likely cartridge-specific (PI domain 1 values).
                { 0x4600018, 0xFF803712 },
                { 0x460001C, 0xFFFF8037 },
                { 0x4600020, 0xFFFFF803 },
                // Omitted the CIC results.
                { 0x1FC007FC, 0xC0 }
            };

            for (int i = 0; i < writes.GetLength(0); i++)
            {
                nintendo64.RCP.MemoryMaps.WriteWord(writes[i, 0], writes[i, 1]);
            }

            if (DeviceStateFlags.ROM == DeviceState.ROMType.Cartridge && nintendo64.Cartridge != null)
            {
                for (int i = Cartridge.HeaderSize; i < Cartridge.HeaderSize + Cartridge.BootstrapSize; i += sizeof(uint)) // Copying the bootstrap code from the cartridge to the RSP's DMEM.
                {
                    BitHelper.Write(nintendo64.RCP.SP.DMEM, i, BitHelper.FromBigEndian(BitConverter.ToUInt32(nintendo64.Cartridge.ROM, i)));
                }
            }

            nintendo64.CPU.GPR[(int)VR4300.GPRIndex.S3] = (ulong)DeviceStateFlags.ROM;
            nintendo64.CPU.GPR[(int)VR4300.GPRIndex.S4] = (ulong)GetRegion();
            nintendo64.CPU.GPR[(int)VR4300.GPRIndex.S5] = (ulong)DeviceStateFlags.Reset;
            nintendo64.CPU.GPR[(int)VR4300.GPRIndex.S6] = DeviceStateFlags.IPL3Seed;
            nintendo64.CPU.GPR[(int)VR4300.GPRIndex.S7] = DeviceStateFlags.Version;

            // Restoring CPU state.
            nintendo64.CPU.CP0.Registers[12] = 0x34000000;
            nintendo64.CPU.CP0.Registers[16] = 0x6E463;
            nintendo64.CPU.GPR[6] = 0xFFFFFFFFA4001F0C;
            nintendo64.CPU.GPR[7] = 0xFFFFFFFFA4001F08;
            nintendo64.CPU.GPR[8] = 0xC0;
            nintendo64.CPU.GPR[10] = 0x40;
            nintendo64.CPU.GPR[11] = 0xFFFFFFFFA4000040;
            nintendo64.CPU.GPR[29] = 0xFFFFFFFFA4001FF0;
            nintendo64.CPU.GPR[31] = 0xFFFFFFFFA4001550;
            nintendo64.CPU.PC = 0xFFFFFFFFA4000040;
        }

        /// <summary>
        /// Determines the console region from the loaded cartridge's header.
        /// </summary>
        private TVType GetRegion()
        {
            switch (nintendo64.Cartridge?.Country)
            {
                case Cartridge.CountryCode.Europe:
                case Cartridge.CountryCode.Germany:
                case Cartridge.CountryCode.Australia:
                    return TVType.PAL;
                case Cartridge.CountryCode.Japan:
                case Cartridge.CountryCode.USA:
                    return TVType.NTSC;
                default:
                    return 0;
            }
        }

        public void Reset()
        {
            if (nintendo64.Cartridge?.ROM.Length >= Cartridge.HeaderSize + Cartridge.BootstrapSize)
                DeviceStateFlags = CIC.GetSeed(nintendo64.Cartridge.ROM);

            if (BootROM == null)
                EmulateBootROM();
        }
        #endregion
    }
}
