using System.Runtime.InteropServices;

namespace DotN64
{
    public partial class RDRAM
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct ConfigRegister
        {
            #region Fields
            public uint DeviceType;
            public uint DeviceID;
            public uint Delay;
            public uint Mode;
            public uint RefInterval;
            public uint RefRow;
            public uint RasInterval;
            public uint MinInterval;
            public uint AddressSelect;
            public uint DeviceManufacturer;
            #endregion
        }
    }
}
