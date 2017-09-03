using System;

namespace N64Emu.CPU
{
    public partial class VR4300
    {
        public partial class SystemControlUnit
        {
            public class StatusRegister : Register
            {
                #region Fields
                private const int IEShift = 0, IESize = (1 << 1) - 1;
                private const int EXLShift = 1, EXLSize = (1 << 1) - 1;
                private const int ERLShift = 2, ERLSize = (1 << 1) - 1;
                private const int KSUShift = 3, KSUSize = (1 << 2) - 1;
                private const int UXShift = 5, UXSize = (1 << 1) - 1;
                private const int SXShift = 6, SXSize = (1 << 1) - 1;
                private const int KXShift = 7, KXSize = (1 << 1) - 1;
                private const int IMShift = 8, IMSize = (1 << 8) - 1;
                private const int DSShift = 16, DSSize = (1 << 9) - 1;
                private const int REShift = 25, RESize = (1 << 1) - 1;
                private const int FRShift = 26, FRSize = (1 << 1) - 1;
                private const int RPShift = 27, RPSize = (1 << 1) - 1;
                private const int CUShift = 28, CUSize = (1 << 4) - 1;
                #endregion

                #region Properties
                protected override RegisterIndex Index => RegisterIndex.Status;

                /// <summary>
                /// Specifies and indicates global interrupt enable.
                /// </summary>
                public bool IE
                {
                    get => GetBoolean(IEShift, IESize);
                    set => SetValue(IEShift, IESize, value);
                }

                /// <summary>
                /// Specifies and indicates exception level.
                /// </summary>
                public bool EXL
                {
                    get => GetBoolean(EXLShift, EXLSize);
                    set => SetValue(EXLShift, EXLSize, value);
                }

                /// <summary>
                /// Specifies and indicates error level.
                /// </summary>
                public bool ERL
                {
                    get => GetBoolean(ERLShift, ERLSize);
                    set => SetValue(ERLShift, ERLSize, value);
                }

                /// <summary>
                /// Specifies and indicates mode bits.
                /// </summary>
                public Mode KSU
                {
                    get => (Mode)GetValue(KSUShift, KSUSize);
                    set => SetValue(KSUShift, KSUSize, (ulong)value);
                }

                /// <summary>
                /// Enables 64-bit addressing and operations in User mode.
                /// When this bit is set, XTLB miss exception is generated on TLB misses in User mode addresses space.
                /// </summary>
                public bool UX
                {
                    get => GetBoolean(UXShift, UXSize);
                    set => SetValue(UXShift, UXSize, value);
                }

                /// <summary>
                /// Enables 64-bit addressing and operations in Supervisor mode.
                /// When this bit is set, XTLB miss exception is generated on TLB misses in Supervisor mode addresses space.
                /// </summary>
                public bool SX
                {
                    get => GetBoolean(SXShift, SXSize);
                    set => SetValue(SXShift, SXSize, value);
                }

                /// <summary>
                /// Enables 64-bit addressing in Kernel mode.
                /// When this bit is set, XTLB miss exception is generated on TLB misses in Kernel mode addresses space.
                /// </summary>
                public bool KX
                {
                    get => GetBoolean(KXShift, KXSize);
                    set => SetValue(KXShift, KXSize, value);
                }

                /// <summary>
                /// Interrupt Mask field, enables external, internal, coprocessors or software interrupts.
                /// </summary>
                public byte IM
                {
                    get => (byte)GetValue(IMShift, IMSize);
                    set => SetValue(IMShift, IMSize, value);
                }

                /// <summary>
                /// Diagnostic Status field.
                /// </summary>
                public DiagnosticStatus DS
                {
                    get => (DiagnosticStatus)GetValue(DSShift, DSSize);
                    set => SetValue(DSShift, DSSize, value);
                }

                /// <summary>
                /// Reverse-Endian bit, enables reverse of system endianness in User mode.
                /// </summary>
                public bool RE
                {
                    get => GetBoolean(REShift, RESize);
                    set => SetValue(REShift, RESize, value);
                }

                /// <summary>
                /// Enables additional floating-point registers.
                /// </summary>
                public bool FR
                {
                    get => GetBoolean(FRShift, FRSize);
                    set => SetValue(FRShift, FRSize, value);
                }

                /// <summary>
                /// Enables low-power operation by reducing the internal clock frequency and the system interface clock frequency to one-quarter speed.
                /// </summary>
                public bool RP
                {
                    get => GetBoolean(RPShift, RPSize);
                    set => SetValue(RPShift, RPSize, value);
                }

                /// <summary>
                /// Controls the usability of each of the four coprocessor unit numbers.
                /// </summary>
                public CoprocessorUsabilities CU
                {
                    get => (CoprocessorUsabilities)GetValue(CUShift, CUSize);
                    set => SetValue(CUShift, CUSize, (ulong)value);
                }
                #endregion

                #region Constructors
                public StatusRegister(SystemControlUnit cp0)
                    : base(cp0) { }
                #endregion

                #region Structures
                public struct DiagnosticStatus
                {
                    #region Fields
                    private ushort data;
                    #endregion

                    #region Properties
                    /// <summary>
                    /// These bits are defined to maintain compatibility with the VR4200, and is not used by thehardware of the VR4300.
                    /// </summary>
                    public bool DE
                    {
                        get => Get(0);
                        set => Set(0, value);
                    }

                    /// <summary>
                    /// These bits are defined to maintain compatibility with the VR4200, and is not used by thehardware of the VR4300.
                    /// </summary>
                    public bool CE
                    {
                        get => Get(1);
                        set => Set(1, value);
                    }

                    /// <summary>
                    /// CP0 condition bit.
                    /// </summary>
                    public bool CH
                    {
                        get => Get(2);
                        set => Set(2, value);
                    }

                    /// <summary>
                    /// Indicates a Soft Reset or NMI has occurred.
                    /// </summary>
                    public bool SR
                    {
                        get => Get(4);
                        set => Set(4, value);
                    }

                    /// <summary>
                    /// Indicates TLB shutdown has occurred (read-only); used to avoid damage to the TLB if more than one TLB entry matches a single virtual address.
                    /// </summary>
                    public bool TS
                    {
                        get => Get(5);
                        set => Set(5, value);
                    }

                    /// <summary>
                    /// Controls the location of TLB miss and general purpose exception vectors.
                    /// </summary>
                    /// <value>Boostrap if true, otherwise Normal.</value>
                    public bool BEV
                    {
                        get => Get(6);
                        set => Set(6, value);
                    }

                    /// <summary>
                    /// Enables Instruction Trace Support.
                    /// </summary>
                    public bool ITS
                    {
                        get => Get(8);
                        set => Set(8, value);
                    }
                    #endregion

                    #region Methods
                    private bool Get(int shift) => (data >> shift & 1) != 0;

                    private void Set(int shift, bool value)
                    {
                        data &= (ushort)~(1 << shift);
                        data |= (ushort)((value ? 1 : 0) << shift);
                    }
                    #endregion

                    #region Operators
                    public static implicit operator DiagnosticStatus(ushort value) => new DiagnosticStatus { data = value };

                    public static implicit operator ushort(DiagnosticStatus status) => status.data;
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
                    None = 0,
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
