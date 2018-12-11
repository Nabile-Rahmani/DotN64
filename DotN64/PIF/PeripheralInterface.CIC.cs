namespace DotN64
{
    public partial class PeripheralInterface
    {
        private static class CIC
        {
            #region Methods
            private static uint ComputeCRC32(byte[] data, int offset, int length)
            {
                var table = new uint[256];
                uint c;

                for (uint i = 0; i < table.Length; i++)
                {
                    c = i;

                    for (uint j = 0; j < 8; j++)
                    {
                        if ((c & 1) != 0)
                            c = 0xEDB88320 ^ (c >> 1);
                        else
                            c >>= 1;
                    }

                    table[i] = c;
                }

                c = 0 ^ 0xFFFFFFFF;

                for (int i = 0; i < length; i++)
                {
                    c = table[(c ^ (data[offset + i])) & 0xFF] ^ (c >> 8);
                }

                return c ^ 0xFFFFFFFF;
            }

            public static DeviceState GetDeviceState(byte[] data, int offset = Cartridge.HeaderSize, int length = Cartridge.BootstrapSize)
            {
                switch (ComputeCRC32(data, offset, length))
                {
                    case 0x6170A4A1: // NUS-6101.
                    case 0x009E9EA3: // NUS-7102.
                        return new DeviceState
                        {
                            IPL2Seed = 0x3F,
                            IPL3Seed = 0x3F,
                            Version = 1
                        };
                    case 0x90BB6CB5: // NUS-6102.
                        return new DeviceState
                        {
                            IPL2Seed = 0x3F,
                            IPL3Seed = 0x3F
                        };
                    case 0x0B050EE0: // NUS-6103.
                        return new DeviceState
                        {
                            IPL2Seed = 0x3F,
                            IPL3Seed = 0x78
                        };
                    case 0x98BC2C86: // NUS-6105.
                        return new DeviceState
                        {
                            IPL2Seed = 0x3F,
                            IPL3Seed = 0x91
                        };
                    case 0xACC8580A: // NUS-6106.
                        return new DeviceState
                        {
                            IPL2Seed = 0x3F,
                            IPL3Seed = 0x85
                        };
                    case 0x0E018159: // NUS-8303.
                        return new DeviceState { IPL3Seed = 0xDD };
                    default:
                        return 0;
                }
            }
            #endregion
        }
    }
}
