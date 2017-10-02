namespace N64Emu.Interfaces.Serial
{
    public partial class SerialInterface
    {
        #region Properties
        public StatusRegister Status { get; } = new StatusRegister();
        #endregion
    }
}
