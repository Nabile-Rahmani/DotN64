using System;
using System.Collections.Specialized;

namespace DotN64.CPU
{
    public partial class VR4300
    {
        public partial class SystemControlUnit
        {
            /// <summary>
            /// See: datasheet#6.3.5.
            /// </summary>
            public class StatusRegister : Register
            {
                #region Fields
                private static readonly BitVector32.Section ie = BitVector32.CreateSection(1),
                exl = BitVector32.CreateSection(1, ie),
                erl = BitVector32.CreateSection(1, exl),
                ksu = BitVector32.CreateSection((1 << 2) - 1, erl),
                ux = BitVector32.CreateSection(1, ksu),
                sx = BitVector32.CreateSection(1, ux),
                kx = BitVector32.CreateSection(1, sx),
                im = BitVector32.CreateSection((1 << 8) - 1, kx),
                ds = BitVector32.CreateSection((1 << 9) - 1, im),
                re = BitVector32.CreateSection(1, ds),
                fr = BitVector32.CreateSection(1, re),
                rp = BitVector32.CreateSection(1, fr),
                cu = BitVector32.CreateSection((1 << 4) - 1, rp);
                #endregion

                #region Properties
                /// <summary>
                /// Specifies and indicates global interrupt enable.
                /// </summary>
                public bool IE
                {
                    get => Convert.ToBoolean(this[ie]);
                    set => this[ie] = Convert.ToInt32(value);
                }

                /// <summary>
                /// Specifies and indicates exception level.
                /// </summary>
                public bool EXL
                {
                    get => Convert.ToBoolean(this[exl]);
                    set => this[exl] = Convert.ToInt32(value);
                }

                /// <summary>
                /// Specifies and indicates error level.
                /// </summary>
                public bool ERL
                {
                    get => Convert.ToBoolean(this[erl]);
                    set => this[erl] = Convert.ToInt32(value);
                }

                /// <summary>
                /// Specifies and indicates mode bits.
                /// </summary>
                public Mode KSU
                {
                    get => (EXL | ERL) ? Mode.Kernel : (Mode)this[ksu];
                    set => this[ksu] = (int)value;
                }

                /// <summary>
                /// Enables 64-bit addressing and operations in User mode.
                /// When this bit is set, XTLB miss exception is generated on TLB misses in User mode addresses space.
                /// </summary>
                public bool UX
                {
                    get => Convert.ToBoolean(this[ux]);
                    set => this[ux] = Convert.ToInt32(value);
                }

                /// <summary>
                /// Enables 64-bit addressing and operations in Supervisor mode.
                /// When this bit is set, XTLB miss exception is generated on TLB misses in Supervisor mode addresses space.
                /// </summary>
                public bool SX
                {
                    get => Convert.ToBoolean(this[sx]);
                    set => this[sx] = Convert.ToInt32(value);
                }

                /// <summary>
                /// Enables 64-bit addressing in Kernel mode.
                /// When this bit is set, XTLB miss exception is generated on TLB misses in Kernel mode addresses space.
                /// </summary>
                public bool KX
                {
                    get => Convert.ToBoolean(this[kx]);
                    set => this[kx] = Convert.ToInt32(value);
                }

                /// <summary>
                /// Interrupt Mask field, enables external, internal, coprocessors or software interrupts.
                /// </summary>
                public byte IM
                {
                    get => (byte)this[im];
                    set => this[im] = value;
                }

                /// <summary>
                /// Diagnostic Status field.
                /// </summary>
                public DiagnosticStatus DS
                {
                    get => (DiagnosticStatus)this[ds];
                    set => this[ds] = value;
                }

                /// <summary>
                /// Reverse-Endian bit, enables reverse of system endianness in User mode.
                /// </summary>
                public bool RE
                {
                    get => Convert.ToBoolean(this[re]);
                    set => this[re] = Convert.ToInt32(value);
                }

                /// <summary>
                /// Enables additional floating-point registers.
                /// </summary>
                public bool FR
                {
                    get => Convert.ToBoolean(this[fr]);
                    set => this[fr] = Convert.ToInt32(value);
                }

                /// <summary>
                /// Enables low-power operation by reducing the internal clock frequency and the system interface clock frequency to one-quarter speed.
                /// </summary>
                public bool RP
                {
                    get => Convert.ToBoolean(this[rp]);
                    set => this[rp] = Convert.ToInt32(value);
                }

                /// <summary>
                /// Controls the usability of each of the four coprocessor unit numbers.
                /// </summary>
                public CoprocessorUsabilities CU
                {
                    get => (CoprocessorUsabilities)this[cu];
                    set => this[cu] = (int)value;
                }
                #endregion

                #region Constructors
                public StatusRegister(SystemControlUnit cp0)
                    : base(cp0, RegisterIndex.Status) { }
                #endregion

                #region Structures
                public struct DiagnosticStatus
                {
                    #region Fields
                    private BitVector32 bits;

                    private static readonly int de = BitVector32.CreateMask(),
                    ce = BitVector32.CreateMask(de),
                    ch = BitVector32.CreateMask(ce),
                    constant1 = BitVector32.CreateMask(ch),
                    sr = BitVector32.CreateMask(constant1),
                    ts = BitVector32.CreateMask(sr),
                    bev = BitVector32.CreateMask(ts),
                    constant2 = BitVector32.CreateMask(bev),
                    its = BitVector32.CreateMask(constant2);
                    #endregion

                    #region Properties
                    /// <summary>
                    /// These bits are defined to maintain compatibility with the VR4200, and is not used by the hardware of the VR4300.
                    /// </summary>
                    public bool DE
                    {
                        get => bits[de];
                        set => bits[de] = value;
                    }

                    /// <summary>
                    /// These bits are defined to maintain compatibility with the VR4200, and is not used by the hardware of the VR4300.
                    /// </summary>
                    public bool CE
                    {
                        get => bits[ce];
                        set => bits[ce] = value;
                    }

                    /// <summary>
                    /// CP0 condition bit.
                    /// </summary>
                    public bool CH
                    {
                        get => bits[ch];
                        set => bits[ch] = value;
                    }

                    /// <summary>
                    /// Indicates a Soft Reset or NMI has occurred.
                    /// </summary>
                    public bool SR
                    {
                        get => bits[sr];
                        set => bits[sr] = value;
                    }

                    /// <summary>
                    /// Indicates TLB shutdown has occurred (read-only); used to avoid damage to the TLB if more than one TLB entry matches a single virtual address.
                    /// </summary>
                    public bool TS
                    {
                        get => bits[ts];
                        set => bits[ts] = value;
                    }

                    /// <summary>
                    /// Controls the location of TLB miss and general purpose exception vectors.
                    /// </summary>
                    /// <value>Boostrap if true, otherwise Normal.</value>
                    public bool BEV
                    {
                        get => bits[bev];
                        set => bits[bev] = value;
                    }

                    /// <summary>
                    /// Enables Instruction Trace Support.
                    /// </summary>
                    public bool ITS
                    {
                        get => bits[its];
                        set => bits[its] = value;
                    }
                    #endregion

                    #region Operators
                    public static implicit operator DiagnosticStatus(ushort data) => new DiagnosticStatus { bits = new BitVector32(data) };

                    public static implicit operator ushort(DiagnosticStatus status) => (ushort)status.bits.Data;
                    #endregion
                }
                #endregion

                #region Enumerations
                public enum Mode : byte
                {
                    Kernel = 0b00,
                    Supervisor = 0b01,
                    User = 0b10
                }

                [Flags]
                public enum CoprocessorUsabilities : byte
                {
                    CP0 = 1 << 0,
                    CP1 = 1 << 1,
                    CP2 = 1 << 2,
                    CP3 = 1 << 3
                }
                #endregion
            }
        }
    }
}
