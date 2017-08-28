namespace N64Emu
{
    public partial class VR4300
    {
        public partial class CP0
        {
            #region Properties
            public ulong[] Registers { get; } = new ulong[32];

            public ConfigRegister Config { get; }
            #endregion

            public CP0()
            {
                Config = new ConfigRegister(Registers);
            }

            #region Methods
            public void PowerOnReset()
            {
                Config.ConfigEP = ConfigRegister.EP.D;
                Config.ConfigBE = ConfigRegister.BE.BigEndian;
            }
            #endregion
        }
    }
}
