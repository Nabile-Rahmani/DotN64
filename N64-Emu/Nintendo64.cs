using System;

namespace N64Emu
{
    public partial class Nintendo64
    {
        #region Fields
        public static readonly MappingEntry[] MemoryMaps =
        {
            new MappingEntry
            {
                StartAddress = 0x1FC00000,
                EndAddress = 0x1FC007BF,
                EntryName = MappingEntry.Name.PIFBootROM
            }
        };

        public const uint SPStatusRegisterAddress = 0x04040010;
        #endregion

        #region Properties
        public VR4300 CPU { get; }

        public RealityCoprocessor RCP { get; } = new RealityCoprocessor();

        public byte[] RAM { get; } = new byte[4 * 1024 * 1024]; // 4 MB of base memory (excludes the expansion pack).

        public byte[] PIFROM { get; set; }

        public Cartridge Cartridge { get; private set; }
        #endregion

        #region Constructors
        public Nintendo64()
        {
            CPU = new VR4300(this);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Runs the PIF ROM.
        /// See: http://www.emulation64.com/ultra64/bootn64.html
        /// </summary>
        private void RunPIF()
        {
            CPU.GPRegisters[(int)VR4300.GPRegisterIndex.s4] = 0x1;
            CPU.GPRegisters[(int)VR4300.GPRegisterIndex.s6] = 0x3F;
            CPU.GPRegisters[(int)VR4300.GPRegisterIndex.sp] = 0xA4001FF0;
            CPU.CP0.Registers[(int)VR4300.Coprocessor0.RegisterIndex.Random] = 0x0000001F;
            CPU.CP0.Registers[(int)VR4300.Coprocessor0.RegisterIndex.Status] = 0x70400004;
            CPU.CP0.Registers[(int)VR4300.Coprocessor0.RegisterIndex.PRId] = 0x00000B00;
            CPU.CP0.Registers[(int)VR4300.Coprocessor0.RegisterIndex.Config] = 0x0006E463;

            Array.Copy(BitConverter.GetBytes(0x01010101), 0, RAM, (uint)CPU.MapMemory(new UIntPtr(0x04300004)), sizeof(int));
            Array.Copy(Cartridge.ROM, 0, RAM, (uint)CPU.MapMemory(new UIntPtr(0xA4000000)), 0x1000);

            CPU.ProgramCounter = (ulong)CPU.MapMemory(new UIntPtr(0xA4000040));
        }

        public void Initialise()
        {
            CPU.PowerOnReset();

            if (PIFROM == null)
                RunPIF();

            while (true)
            {
                CPU.Step();
            }
        }

        public void Insert(Cartridge cartridge)
        {
            if (Cartridge != null)
                throw new Exception("Existing game could be running.");

            Cartridge = cartridge;
        }
        #endregion
    }
}
