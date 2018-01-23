using System.Collections.Specialized;

namespace DotN64.CPU
{
    public partial class VR4300
    {
        public partial class FloatingPointUnit
        {
            /// <summary>
            /// See: datasheet#7.2.5.
            /// </summary>
            public class ImplementationRevisionRegister : Register
            {
                #region Fields
                private static readonly BitVector32.Section revSection = BitVector32.CreateSection((1 << 8) - 1),
                                                            impSection = BitVector32.CreateSection((1 << 8) - 1, revSection);
                #endregion

                #region Properties
                protected override uint Data
                {
                    get => fpu.cpu.FCR0;
                    set => fpu.cpu.FCR0 = value;
                }

                /// <summary>
                /// Revision number in the form of y.x.
                /// </summary>
                public byte Rev
                {
                    get => (byte)this[revSection];
                    set => this[revSection] = value;
                }

                /// <summary>
                /// Implementation number (0x0B).
                /// </summary>
                public byte Imp
                {
                    get => (byte)this[impSection];
                    set => this[impSection] = value;
                }
                #endregion

                #region Constructors
                public ImplementationRevisionRegister(FloatingPointUnit fpu)
                    : base(fpu)
                {
                    Imp = 0x0B;
                }
                #endregion
            }
        }
    }
}
