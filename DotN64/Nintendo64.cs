using System;
using System.Net;

namespace DotN64
{
    using AI;
    using CPU;
    using MI;
    using PI;
    using RCP;
    using SI;
    using VI;

    public class Nintendo64
    {
        #region Properties
        public VR4300 CPU { get; }

        public RealityCoprocessor RCP { get; } = new RealityCoprocessor();

        public PeripheralInterface PI { get; } = new PeripheralInterface();

        public SerialInterface SI { get; } = new SerialInterface();

        public AudioInterface AI { get; } = new AudioInterface();

        public VideoInterface VI { get; } = new VideoInterface();

        public MIPSInterface MI { get; } = new MIPSInterface();

        public byte[] RAM { get; } = new byte[4 * 1024 * 1024]; // 4 MB of base memory (excludes the expansion pack).

        public Cartridge Cartridge { get; set; }
        #endregion

        #region Constructors
        public Nintendo64()
        {
            var memoryMaps = new[]
            {
                new MappingEntry(0x1FC00000, 0x1FC007BF, false) // PIF Boot ROM.
                {
                    Read = PI.ReadWord,
                    Write = PI.WriteWord
                },
                new MappingEntry(0x1FC007C0, 0x1FC007FF, false) // PIF (JoyChannel) RAM.
                {
                    Read = PI.ReadWord,
                    Write = PI.WriteWord
                },
                new MappingEntry(0x04600000, 0x046FFFFF, false) // Peripheral interface (PI) registers.
                {
                    Read = PI.ReadWord,
                    Write = PI.WriteWord
                },
                new MappingEntry(0x04000000, 0x040FFFFF, false) // SP registers.
                {
                    Read = RCP.SP.ReadWord,
                    Write = RCP.SP.WriteWord
                },
                new MappingEntry(0x04400000, 0x044FFFFF, false) // Video interface (VI) registers.
                {
                    Read = VI.ReadWord,
                    Write = VI.WriteWord
                },
                new MappingEntry(0x04500000, 0x045FFFFF, false) // Audio interface (AI) registers.
                {
                    Read = AI.ReadWord,
                    Write = AI.WriteWord
                },
                new MappingEntry(0x04300000, 0x043FFFFF, false) // MIPS interface (MI) registers.
                {
                    Read = MI.ReadWord,
                    Write = MI.WriteWord
                },
                new MappingEntry(0x04800000, 0x048FFFFF, false) // Serial interface (SI) registers.
                {
                    Read = SI.ReadWord,
                    Write = SI.WriteWord
                },
                new MappingEntry(0x10000000, 0x1FBFFFFF) // Cartridge Domain 1 Address 2.
                {
                    Read = o => (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(Cartridge.ROM, (int)o))
                },
                new MappingEntry(0x04100000, 0x041FFFFF, false) // DP command registers.
                {
                    Read = RCP.DP.ReadWord,
                    Write = RCP.DP.WriteWord
                }
            };
            CPU = new VR4300(memoryMaps);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Emulates the PIF ROM.
        /// </summary>
        private void RunPIF()
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
                // Omitted the CIC result.
                { 0x1FC007FC, 0xC0 }
            };

            for (int i = 0; i < writes.GetLength(0); i++)
            {
                var address = writes[i, 0] + 0xFFFFFFFFA0000000; // The constant converts them back to virtual addresses.

                CPU.CP0.Map(ref address).WriteWord(address, writes[i, 1]);
            }

            for (int i = 0; i < 0x1000; i += sizeof(uint)) // Copying the bootstrap code from the cartridge to the RSP's DMEM.
            {
                var dmemAddress = (ulong)(0xA4000000 + i);

                CPU.CP0.Map(ref dmemAddress).WriteWord(dmemAddress, (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(Cartridge.ROM, i)));
            }

            // Restoring CPU state.
            CPU.CP0.Registers[12] = 0x34000000;
            CPU.CP0.Registers[16] = 0x6E463;
            CPU.GPR[1] = 0x1;
            CPU.GPR[2] = 0x6459969A;
            CPU.GPR[3] = 0x6459969A;
            // Omitted the CIC result.
            CPU.GPR[6] = 0xFFFFFFFFA4001F0C;
            CPU.GPR[7] = 0xFFFFFFFFA4001F08;
            CPU.GPR[8] = 0xC0;
            CPU.GPR[10] = 0x40;
            CPU.GPR[11] = 0xFFFFFFFFA4000040;
            CPU.GPR[12] = 0xFFFFFFFFD19AE574;
            CPU.GPR[13] = 0x4A459BAE;
            CPU.GPR[14] = 0xFFFFFFFFE8EAD626;
            CPU.GPR[15] = 0x6459969A;
            CPU.GPR[20] = 0x1;
            CPU.GPR[25] = 0x453CA37B;
            CPU.GPR[29] = 0xFFFFFFFFA4001FF0;
            CPU.GPR[31] = 0xFFFFFFFFA4001550;
            CPU.HI = 0x6459969A;
            CPU.LO = 0x6459969A;
            CPU.PC = 0xFFFFFFFFA4000040;
        }

        public void PowerOn()
        {
            CPU.PowerOnReset();

            if (PI.BootROM == null)
                RunPIF();

            while (true)
            {
                CPU.Step();
            }
        }
        #endregion
    }
}
