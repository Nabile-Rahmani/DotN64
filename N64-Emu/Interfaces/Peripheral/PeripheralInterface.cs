namespace N64Emu.Interfaces.Peripheral
{
    public partial class PeripheralInterface
    {
        #region Properties
        public StatusRegister Status { get; } = new StatusRegister();
        #endregion
    }
}
