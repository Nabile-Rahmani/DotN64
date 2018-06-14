namespace DotN64
{
    using static Helpers.BitHelper;

    public partial class PeripheralInterface
    {
        private struct DeviceState
        {
            #region Fields
            private uint data;

            private const byte IPL2SeedShift = 0, IPL2SeedSize = 0xFF;
            private const byte IPL3SeedShift = 8, IPL3SeedSize = 0xFF;
            private const byte ResetShift = 17, ResetSize = 1;
            private const byte VersionShift = 18, VersionSize = 1;
            private const byte ROMShift = 19, ROMSize = 1;
            #endregion

            #region Properties
            public byte IPL2Seed
            {
                get => (byte)Get(data, IPL2SeedShift, IPL2SeedSize);
                set => Set(ref data, IPL2SeedShift, IPL2SeedSize, value);
            }

            public byte IPL3Seed
            {
                get => (byte)Get(data, IPL3SeedShift, IPL3SeedSize);
                set => Set(ref data, IPL3SeedShift, IPL3SeedSize, value);
            }

            public ResetType Reset
            {
                get => (ResetType)Get(data, ResetShift, ResetSize);
                set => Set(ref data, ResetShift, ResetSize, (byte)value);
            }

            public byte Version
            {
                get => (byte)Get(data, VersionShift, VersionSize);
                set => Set(ref data, VersionShift, VersionSize, value);
            }

            public ROMType ROM
            {
                get => (ROMType)Get(data, ROMShift, ROMSize);
                set => Set(ref data, ROMShift, ROMSize, (byte)value);
            }
            #endregion

            #region Operators
            public static implicit operator DeviceState(uint data) => new DeviceState { data = data };

            public static implicit operator uint(DeviceState state) => state.data;
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
