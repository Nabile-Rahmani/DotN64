using System;
using System.Collections.Specialized;

namespace DotN64.CPU
{
    public partial class VR4300
    {
        public partial class FloatingPointUnit
        {
            /// <summary>
            /// See: datasheet#7.2.4.
            /// </summary>
            public class ControlStatusRegister : Register
            {
                #region Fields
                private static readonly BitVector32.Section rmSection = BitVector32.CreateSection((1 << 2) - 1),
                                                            flagsSection = BitVector32.CreateSection((1 << 5) - 1, rmSection),
                                                            enablesSection = BitVector32.CreateSection((1 << 5) - 1, flagsSection),
                                                            causeSection = BitVector32.CreateSection((1 << 6) - 1, enablesSection),
                                                            constant1 = BitVector32.CreateSection((1 << 5) - 1, causeSection),
                                                            cSection = BitVector32.CreateSection(1, constant1),
                                                            fsSection = BitVector32.CreateSection(1, cSection);
                #endregion

                #region Properties
                protected override uint Data
                {
                    get => fpu.cpu.FCR31;
                    set => fpu.cpu.FCR31 = value;
                }

                /// <summary>
                /// Bits 1 and 0 in the FCR31 register constitute the Rounding Mode (RM) bits.
                /// These bits specify the rounding mode that FPU uses for all floating-point operations.
                /// </summary>
                public RoundingMode RM
                {
                    get => (RoundingMode)this[rmSection];
                    set => this[rmSection] = (byte)value;
                }

                /// <summary>
                /// The Flag bits are cumulative and indicate the exceptions that were raised after reset.
                /// Flag bits are set to 1 if an IEEE754 exception is raised but the occurrence of the exception is prohibited.
                /// Otherwise, they remain unchanged.
                /// The Flag bits are never cleared as a side effect of floating-point operations; however, they can be set or cleared by writing a new value into the FCR31, using a CTC1 instruction.
                /// </summary>
                public ExceptionFlags Flags
                {
                    get => (ExceptionFlags)this[flagsSection];
                    set => this[flagsSection] = (byte)value;
                }

                /// <summary>
                /// A floating-point exception is generated any time a Cause bit and the corresponding Enable bit are set.
                /// As soon as the Cause bit enabled through the Floating-point operation, an exception occurs.
                /// When both Cause and Enable bits are set by the CTC1 instruction, an exception also occurs.
                /// </summary>
                public ExceptionFlags Enables
                {
                    get => (ExceptionFlags)this[enablesSection];
                    set => this[enablesSection] = (byte)value;
                }

                /// <summary>
                /// Bits 17:12 in the FCR31 contain Cause bits which reflect the results of the most recently executed floating-point instruction.
                /// The Cause bits are a logical extension of the CP0 Cause register; they identify the exceptions raised by the last floating-point operation; and generate exceptions if the corresponding Enable bit is set.
                /// If more than one exception occurs on a single instruction, each appropriate bit is set.
                /// </summary>
                public ExceptionFlags Cause
                {
                    get => (ExceptionFlags)this[causeSection];
                    set => this[causeSection] = (byte)value;
                }

                /// <summary>
                /// When a floating-point Compare operation takes place, the result is stored at bit 23, the Condition bit.
                /// The C bit is set to 1 if the condition is true; the bit is cleared to 0 if the condition is false.
                /// Bit 23 is affected only by compare and CTC1 instructions.
                /// </summary>
                public bool C
                {
                    get => Convert.ToBoolean(this[cSection]);
                    set => this[cSection] = Convert.ToInt32(value);
                }

                /// <summary>
                /// The FS bit enables a value that cannot be normalized (denormalized number) to be flashed.
                /// When the FS bit is set and the enable bit is not set for the underflow exception and illegal exception, the result of the denormalized number does not cause the unimplemented operation exception, but is flushed.
                /// Whether the flushed result is 0 or the minimum normalized value is determined depending on the rounding mode (refer to Table 7-2).
                /// If the result is flushed, the Flag and Cause bits are set for the underflow and illegal exceptions.
                /// </summary>
                public bool FS
                {
                    get => Convert.ToBoolean(this[fsSection]);
                    set => this[fsSection] = Convert.ToInt32(value);
                }
                #endregion

                #region Constructors
                public ControlStatusRegister(FloatingPointUnit fpu)
                    : base(fpu) { }
                #endregion

                #region Enumerations
                public enum RoundingMode : byte
                {
                    /// <summary>Round result to nearest representable value; round to value with least-significant bit 0 when the two nearest representable values are equally near.</summary>
                    RN = 0b00,
                    /// <summary>Round toward 0: round to value closest to and not greater in magnitude than the infinitely precise result.</summary>
                    RZ = 0b01,
                    /// <summary>Round toward + ∞: round to value closest to and not less than the infinitely precise result.</summary>
                    RP = 0b10,
                    /// <summary>Round toward – ∞: round to value closest to and not greater than the infinitely precise result.</summary>
                    RM = 0b11
                }

                [Flags]
                public enum ExceptionFlags : byte
                {
                    /// <summary>Inexact Operation.</summary>
                    I = 1 << 0,
                    /// <summary>Underflow.</summary>
                    U = 1 << 1,
                    /// <summary>Overflow.</summary>
                    O = 1 << 2,
                    /// <summary>Division by Zero.</summary>
                    Z = 1 << 3,
                    /// <summary>Invalid Operation.</summary>
                    V = 1 << 4,
                    /// <summary>Unimplemented Operation.</summary>
                    E = 1 << 5
                }
                #endregion
            }
        }
    }
}
