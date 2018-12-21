using System.Collections.Specialized;

namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public partial class MIPSInterface
        {
            public struct VersionRegister
            {
                #region Fields
                private BitVector32 bits;

                private static readonly BitVector32.Section io = BitVector32.CreateSection((1 << 8) - 1),
                                                            rac = BitVector32.CreateSection((1 << 8) - 1, io),
                                                            rdp = BitVector32.CreateSection((1 << 8) - 1, rac),
                                                            rsp = BitVector32.CreateSection((1 << 8) - 1, rdp);
                #endregion

                #region Properties
                public byte IO
                {
                    get => (byte)bits[io];
                    set => bits[io] = value;
                }

                public byte RAC
                {
                    get => (byte)bits[rac];
                    set => bits[rac] = value;
                }

                public byte RDP
                {
                    get => (byte)bits[rdp];
                    set => bits[rdp] = value;
                }

                public byte RSP
                {
                    get => (byte)bits[rsp];
                    set => bits[rsp] = value;
                }

                /// <summary>
                /// The prototype RCP chip.
                /// </summary>
                public static VersionRegister Version1 => new VersionRegister
                {
                    IO = 1,
                    RAC = 1,
                    RDP = 1,
                    RSP = 1
                };

                /// <summary>
                /// The retail RCP chip.
                /// </summary>
                public static VersionRegister Version2 => new VersionRegister
                {
                    IO = 2,
                    RAC = 1,
                    RDP = 2,
                    RSP = 2
                };
                #endregion

                #region Operators
                public static implicit operator VersionRegister(uint data) => new VersionRegister { bits = new BitVector32((int)data) };

                public static implicit operator uint(VersionRegister version) => (uint)version.bits.Data;
                #endregion
            }
        }
    }
}
