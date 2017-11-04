using System.Collections.Specialized;

namespace DotN64.CPU
{
    public partial class VR4300
    {
        public partial class SystemControlUnit
        {
            public abstract class Register
            {
                #region Fields
                private readonly SystemControlUnit cp0;
                #endregion

                #region Properties
                private BitVector32 Bits => new BitVector32((int)Data);

                protected ulong Data
                {
                    get => cp0.Registers[(int)Index];
                    set => cp0.Registers[(int)Index] = value;
                }

                protected abstract RegisterIndex Index { get; }
                #endregion

                #region Indexers
                protected int this[BitVector32.Section section]
                {
                    get => Bits[section];
                    set
                    {
                        var bits = Bits;
                        bits[section] = value;
                        Data = (ulong)bits.Data;
                    }
                }

                protected bool this[int mask]
                {
                    get => Bits[mask];
                    set
                    {
                        var bits = Bits;
                        bits[mask] = value;
                        Data = (ulong)bits.Data;
                    }
                }
                #endregion

                #region Constructors
                protected Register(SystemControlUnit cp0)
                {
                    this.cp0 = cp0;
                }
                #endregion
            }
        }
    }
}
