using System;

namespace DotN64.CPU
{
    public partial class VR4300
    {
        public partial class SystemControlUnit
        {
            #region Properties
            public ulong[] Registers { get; } = new ulong[32];

            public ConfigRegister Config { get; }

            public StatusRegister Status { get; }
            #endregion

            #region Constructors
            public SystemControlUnit()
            {
                Config = new ConfigRegister(this);
                Status = new StatusRegister(this);
            }
            #endregion

            #region Methods
            /// <summary>
            /// Translates a virtual address into a physical address.
            /// See: datasheet#5.2.4 Table 5-3.
            /// </summary>
            public ulong Map(ulong address)
            {
                switch (address >> 29 & 0b111)
                {
                    case 0b100: // kseg0.
                        return address - 0xFFFFFFFF80000000;
                    case 0b101: // kseg1.
                        return address - 0xFFFFFFFFA0000000;
                    default:
                        throw new Exception($"Unknown memory map segment for location 0x{address:X16}.");
                }
            }
            #endregion
        }
    }
}
