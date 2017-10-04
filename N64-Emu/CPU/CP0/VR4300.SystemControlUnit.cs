using System;
using System.Collections.Generic;
using System.Linq;

namespace N64Emu.CPU
{
    public partial class VR4300
    {
        public partial class SystemControlUnit
        {
            #region Fields
            private readonly IReadOnlyList<MappingEntry> memoryMaps;
            #endregion

            #region Properties
            public ulong[] Registers { get; } = new ulong[32];

            public ConfigRegister Config { get; }

            public StatusRegister Status { get; }
            #endregion

            #region Constructors
            public SystemControlUnit(IReadOnlyList<MappingEntry> memoryMaps)
            {
                this.memoryMaps = memoryMaps;
                Config = new ConfigRegister(this);
                Status = new StatusRegister(this);
            }
            #endregion

            #region Methods
            public void PowerOnReset()
            {
                Config.EP = ConfigRegister.TransferDataPattern.D;
                Config.BE = ConfigRegister.Endianness.BigEndian;
            }

            /// <summary>
            /// Translates a virtual address into a physical address.
            /// See: datasheet#5.2.4 Table 5-3.
            /// </summary>
            /// <param name="address">The virtual address to translate into a physical address.</param>
            /// <returns>The memory map entry associated with the address.</returns>
            public MappingEntry Map(ref ulong address)
            {
                switch (address >> 29 & 0b111)
                {
                    case 0b100: // kseg0.
                        address -= 0xFFFFFFFF80000000;
                        break;
                    case 0b101: // kseg1.
                        address -= 0xFFFFFFFFA0000000;
                        break;
                    default:
                        throw new Exception($"Unknown memory map segment for location 0x{address:X}.");
                }

                try
                {
                    var value = address;

                    return memoryMaps.First(e => e.Contains(value));
                }
                catch (InvalidOperationException e)
                {
                    throw new Exception($"Unknown physical address: 0x{address:X}.", e);
                }
            }
            #endregion
        }
    }
}
