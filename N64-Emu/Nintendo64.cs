using System;

namespace N64Emu
{
    public class Nintendo64
    {
        #region Properties
        public VR4300 CPU { get; }

        public byte[] RAM { get; } = new byte[4 * 1024 * 1024]; // 4 MB of base memory (excludes the expansion pack).

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
            CPU.GPRegisters[(int)VR4300.GPRegister.s4] = 0x1;
            CPU.GPRegisters[(int)VR4300.GPRegister.s6] = 0x3F;
            CPU.GPRegisters[(int)VR4300.GPRegister.sp] = 0xA4001FF0;
            CPU.CP0.Registers[(int)VR4300.Coprocessor0.Register.Random] = 0x0000001F;
            CPU.CP0.Registers[(int)VR4300.Coprocessor0.Register.Status] = 0x70400004;
            CPU.CP0.Registers[(int)VR4300.Coprocessor0.Register.PRId] = 0x00000B00;
            CPU.CP0.Registers[(int)VR4300.Coprocessor0.Register.Config] = 0x0006E463;

            Array.Copy(BitConverter.GetBytes(0x01010101), 0, RAM, (uint)CPU.MapMemory(new UIntPtr(0x04300004)), sizeof(int));
            Array.Copy(Cartridge.ROM, 0, RAM, (uint)CPU.MapMemory(new UIntPtr(0xA4000000)), 0x1000);

            CPU.ProgramCounter = (ulong)CPU.MapMemory(new UIntPtr(0xA4000040));
        }

        public void Initialise()
        {
            CPU.PowerOnReset();
            RunPIF();
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
