namespace N64Emu.Interfaces.Peripheral
{
    public partial class PeripheralInterface
    {
        #region Properties
        public StatusRegister Status { get; } = new StatusRegister();

        public byte[] BootROM { get; set; }

        public byte[] RAM { get; } = new byte[64];
        #endregion
    }
}
