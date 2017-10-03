namespace N64Emu.PI
{
    public partial class PeripheralInterface
    {
        #region Properties
        public StatusRegister Status { get; } = new StatusRegister();

        public byte[] BootROM { get; set; }

        public byte[] RAM { get; } = new byte[64];

        public Domain[] Domains { get; } = new[]
        {
            new Domain(),
            new Domain()
        };
        #endregion
    }
}
