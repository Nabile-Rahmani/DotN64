namespace N64Emu.SI
{
    public partial class SerialInterface
    {
        #region Properties
        public StatusRegister Status { get; } = new StatusRegister();
        #endregion
    }
}
