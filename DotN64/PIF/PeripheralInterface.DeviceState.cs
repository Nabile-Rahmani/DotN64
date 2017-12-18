using System.Collections.Specialized;

namespace DotN64
{
    public partial class PeripheralInterface
    {
        private struct DeviceState
        {
            #region Fields
            private BitVector32 bits;

            private static readonly BitVector32.Section ipl2SeedSection = BitVector32.CreateSection(0xFF),
                                                        ipl3SeedSection = BitVector32.CreateSection(0xFF, ipl2SeedSection),
                                                        resetSection = BitVector32.CreateSection(0x02, ipl3SeedSection),
                                                        versionSection = BitVector32.CreateSection(0x02, resetSection),
                                                        romSection = BitVector32.CreateSection(0x04, versionSection);
            #endregion

            #region Properties
            public byte IPL2Seed
            {
                get => (byte)bits[ipl2SeedSection];
                set => bits[ipl2SeedSection] = value;
            }

            public byte IPL3Seed
            {
                get => (byte)bits[ipl3SeedSection];
                set => bits[ipl3SeedSection] = value;
            }

            public ResetType Reset
            {
                get => (ResetType)bits[resetSection];
                set => bits[resetSection] = (byte)value;
            }

            public byte Version
            {
                get => (byte)bits[versionSection];
                set => bits[versionSection] = value;
            }

            public ROMType ROM
            {
                get => (ROMType)bits[romSection];
                set => bits[romSection] = (byte)value;
            }
            #endregion

            #region Operators
            public static implicit operator DeviceState(uint data) => new DeviceState { bits = new BitVector32((int)data) };

            public static implicit operator uint(DeviceState state) => (uint)state.bits.Data;
            #endregion

            #region Enumerations
            public enum ResetType : byte
            {
                ColdReset = 0,
                NMI = 1
            }

            public enum ROMType : byte
            {
                Cartridge = 0,
                DiskDrive = 1
            }
            #endregion
        }
    }
}
