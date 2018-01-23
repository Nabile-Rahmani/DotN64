using System.Collections.Specialized;

namespace DotN64.CPU
{
    public partial class VR4300
    {
        public partial class FloatingPointUnit
        {
            public abstract class Register
            {
                #region Fields
                protected readonly FloatingPointUnit fpu;
                #endregion

                #region Properties
                private BitVector32 Bits => new BitVector32((int)Data);

                protected abstract uint Data { get; set; }
                #endregion

                #region Indexers
                protected int this[BitVector32.Section section]
                {
                    get => Bits[section];
                    set
                    {
                        var bits = Bits;
                        bits[section] = value;
                        Data = (uint)bits.Data;
                    }
                }

                protected bool this[int mask]
                {
                    get => Bits[mask];
                    set
                    {
                        var bits = Bits;
                        bits[mask] = value;
                        Data = (uint)bits.Data;
                    }
                }
                #endregion

                #region Constructors
                protected Register(FloatingPointUnit fpu)
                {
                    this.fpu = fpu;
                }
                #endregion
            }
        }
    }
}
