using System.Runtime.InteropServices;

namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public partial class RDRAMInterface
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RDRAMConfigRegister
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
}
