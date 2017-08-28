using System;

namespace N64Emu
{
    public class Nintendo64
    {
        #region Properties
        public VR4300 CPU { get; } = new VR4300();

        public byte[] RAM { get; } = new byte[8 * 1024 * 1024]; // 8 MB of memory (includes expansion pack).

        public Cartridge Cartridge { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Runs the PIF ROM.
        /// See: http://www.emulation64.com/ultra64/bootn64.html
        /// </summary>
        private void RunPIF()
        {
            // Set default register values.
            CPU.GPRegisters[(int)VR4300.GPRegister.s4] = 0x1;
            CPU.GPRegisters[(int)VR4300.GPRegister.s6] = 0x3F;
            CPU.GPRegisters[(int)VR4300.GPRegister.sp] = 0xA4001FF0;
            CPU.Coprocessor0.Registers[(int)VR4300.CP0.Register.Random] = 0x0000001F;
            CPU.Coprocessor0.Registers[(int)VR4300.CP0.Register.Status] = 0x70400004;
            CPU.Coprocessor0.Registers[(int)VR4300.CP0.Register.PRId] = 0x00000B00;
            CPU.Coprocessor0.Registers[(int)VR4300.CP0.Register.Config] = 0x0006E463;

            for (int i = 0; i < sizeof(int); i++)
            {
                RAM[MapMemory(new IntPtr(0x04300004 + i))] = 0x01;
            }

            for (int i = 0; i < 0x1000; i++) // Set the first 0x1000 bytes of the cartridge to this memory location.
            {
                RAM[MapMemory(new IntPtr(0xA4000000 + i))] = Cartridge.Data[i];
            }

            CPU.ProgramCounter = (ulong)MapMemory(new IntPtr(0xA4000040)); // Move the program counter to this address.
        }

        private int MapMemory(IntPtr address) => address.ToInt32() - new IntPtr(0x04000000).ToInt32(); // Need to emulate a MMU (RAM, cartridge space, etc.).

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
