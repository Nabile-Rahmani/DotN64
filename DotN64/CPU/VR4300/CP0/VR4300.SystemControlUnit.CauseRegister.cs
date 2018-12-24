using System;
using System.Collections.Specialized;

namespace DotN64.CPU
{
    public partial class VR4300
    {
        public partial class SystemControlUnit
        {
            /// <summary>
            /// See: datasheet#6.3.6.
            /// </summary>
            public class CauseRegister : Register
            {
                #region Fields
                private static readonly BitVector32.Section constant0 = BitVector32.CreateSection((1 << 2) - 1),
                                                            excCode = BitVector32.CreateSection((1 << 5) - 1, constant0),
                                                            constant1 = BitVector32.CreateSection(1, excCode),
                                                            ip = BitVector32.CreateSection((1 << 8) - 1, constant1),
                                                            constant2 = BitVector32.CreateSection((1 << 12) - 1, ip),
                                                            ce = BitVector32.CreateSection((1 << 2) - 1, constant2),
                                                            constant3 = BitVector32.CreateSection(1, ce),
                                                            bd = BitVector32.CreateSection(1, constant3);
                public static readonly ulong WriteMask = (ulong)(InterruptPending.WriteMask << ip.Offset);
                #endregion

                #region Properties
                /// <summary>
                /// Exception code field (refer to Table 6-2 for details.)
                /// </summary>
                public ExceptionCode ExcCode
                {
                    get => (ExceptionCode)this[excCode];
                    set => this[excCode] = (byte)value;
                }

                /// <summary>
                /// Indicates an interrupt is pending.
                /// 1 → interrupt pending
                /// 0 → no interrupt
                /// IP(7)   :  Timer interrupt
                /// IP(6:2) :  External normal interrupts. Controlled by Int[4:0], or external write requests
                /// IP(1:0) :  Software interrupts. Only these bits can cause interrupt exception when they are set to 1 by software.
                /// </summary>
                public InterruptPending IP
                {
                    get => (InterruptPending)this[ip];
                    set => this[ip] = value;
                }

                /// <summary>
                /// Coprocessor unit number referenced when a Coprocessor Unusable exception has occurred.
                /// If this exception does not occur, undefined.
                /// </summary>
                public byte CE
                {
                    get => (byte)this[ce];
                    set => this[ce] = value;
                }

                /// <summary>
                /// Indicates whether the last exception occurred has been executed in a branch delay slot.
                /// 1 → delay slot
                /// 0 → normal
                /// </summary>
                public bool BD
                {
                    get => Convert.ToBoolean(this[bd]);
                    set => this[bd] = Convert.ToInt32(value);
                }
                #endregion

                #region Constructors
                public CauseRegister(SystemControlUnit cp0)
                    : base(cp0, RegisterIndex.Cause) { }
                #endregion

                #region Structures
                public struct InterruptPending
                {
                    #region Fields
                    private BitVector32 bits;

                    private static readonly BitVector32.Section softwareInterrupts = BitVector32.CreateSection((1 << 2) - 1),
                    externalNormalInterrupts = BitVector32.CreateSection((1 << 5) - 1, softwareInterrupts),
                    timerInterrupt = BitVector32.CreateSection(1, externalNormalInterrupts);
                    public static readonly byte WriteMask = (byte)(softwareInterrupts.Mask << softwareInterrupts.Offset);
                    #endregion

                    #region Properties
                    public byte SoftwareInterrupts
                    {
                        get => (byte)bits[softwareInterrupts];
                        set => bits[softwareInterrupts] = value;
                    }

                    public byte ExternalNormalInterrupts
                    {
                        get => (byte)bits[externalNormalInterrupts];
                        set => bits[externalNormalInterrupts] = value;
                    }

                    public bool TimerInterrupt
                    {
                        get => Convert.ToBoolean(bits[timerInterrupt]);
                        set => bits[timerInterrupt] = Convert.ToInt32(value);
                    }
                    #endregion

                    #region Operators
                    public static implicit operator InterruptPending(byte data) => new InterruptPending { bits = new BitVector32(data) };

                    public static implicit operator byte(InterruptPending interrupt) => (byte)interrupt.bits.Data;
                    #endregion
                }
                #endregion

                #region Enumerations
                public enum ExceptionCode : byte
                {
                    /// <summary>Interrupt.</summary>
                    Int = 0,
                    /// <summary>TLB Modification exception.</summary>
                    Mod = 1,
                    /// <summary>TLB Miss exception (load or instruction fetch).</summary>
                    TLBL = 2,
                    /// <summary>TLB Miss exception (store).</summary>
                    TLBS = 3,
                    /// <summary>Address Error exception (load or instruction fetch).</summary>
                    AdEL = 4,
                    /// <summary>Address Error exception (store).</summary>
                    AdES = 5,
                    /// <summary>Bus Error exception (instruction fetch).</summary>
                    IBE = 6,
                    /// <summary>Bus Error exception (data reference: load or store).</summary>
                    DBE = 7,
                    /// <summary>Syscall exception.</summary>
                    Sys = 8,
                    /// <summary>Breakpoint exception.</summary>
                    Bp = 9,
                    /// <summary>Reserved Instruction exception.</summary>
                    RI = 10,
                    /// <summary>Coprocessor Unusable exception.</summary>
                    CpU = 11,
                    /// <summary>Arithmetic Overflow exception.</summary>
                    Ov = 12,
                    /// <summary>Trap exception.</summary>
                    Tr = 13,
                    /// <summary>Floating-Point exception.</summary>
                    FPE = 15,
                    /// <summary>Watch exception.</summary>
                    WATCH = 23
                }
                #endregion
            }
        }
    }
}
