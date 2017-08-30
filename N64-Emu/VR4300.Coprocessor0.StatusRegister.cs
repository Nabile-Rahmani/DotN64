using System;

namespace N64Emu
{
    public partial class VR4300
    {
        public partial class Coprocessor0
        {
            public class StatusRegister : Register
            {
                #region Fields
                private const int IESize = 1 << 1, IEShift = 0;
                private const int EXLSize = 1 << 1, EXLShift = 1;
                private const int ERLSize = 1 << 1, ERLShift = 2;
                private const int KSUSize = 1 << 2, KSUShift = 3;
                private const int UXSize = 1 << 1, UXShift = 5;
                private const int SXSize = 1 << 1, SXShift = 6;
                private const int KXSize = 1 << 1, KXShift = 7;
                private const int IMSize = 1 << 8, IMShift = 8;
                private const int DSSize = 1 << 9, DSShift = 16;
                private const int RESize = 1 << 1, REShift = 25;
                private const int FRSize = 1 << 1, FRShift = 26;
                private const int RPSize = 1 << 1, RPShift = 27;
                private const int CUSize = 1 << 4, CUShift = 28;
                #endregion

                #region Properties
                protected override RegisterIndex Index => RegisterIndex.Status;

                /// <summary>
                /// Specifies and indicates global interrupt enable.
                /// </summary>
                public bool IE
                {
                    get => GetBoolean(IEShift, IESize);
                    set => SetRegister(IEShift, IESize, value);
                }

                /// <summary>
                /// Specifies and indicates exception level.
                /// </summary>
                public bool EXL
                {
                    get => GetBoolean(EXLShift, EXLSize);
                    set => SetRegister(EXLShift, EXLSize, value);
                }

                /// <summary>
                /// Specifies and indicates error level.
                /// </summary>
                public bool ERL
                {
                    get => GetBoolean(ERLShift, ERLSize);
                    set => SetRegister(ERLShift, ERLSize, value);
                }

                /// <summary>
                /// Specifies and indicates mode bits.
                /// </summary>
                public KSU ConfigKSU
                {
                    get => (KSU)GetRegister(KSUShift, KSUSize);
                    set => SetRegister(KSUShift, KSUSize, (ulong)value);
                }

                /// <summary>
                /// Enables 64-bit addressing and operations in User mode.
                /// When this bit is set, XTLB miss exception is generated on TLB misses in User mode addresses space.
                /// </summary>
                public bool UX
                {
                    get => GetBoolean(UXShift, UXSize);
                    set => SetRegister(UXShift, UXSize, value);
                }

                /// <summary>
                /// Enables 64-bit addressing and operations in Supervisor mode.
                /// When this bit is set, XTLB miss exception is generated on TLB misses in Supervisor mode addresses space.
                /// </summary>
                public bool SX
                {
                    get => GetBoolean(SXShift, SXSize);
                    set => SetRegister(SXShift, SXSize, value);
                }

                /// <summary>
                /// Enables 64-bit addressing in Kernel mode.
                /// When this bit is set, XTLB miss exception is generated on TLB misses in Kernel mode addresses space.
                /// </summary>
                public bool KX
                {
                    get => GetBoolean(KXShift, KXSize);
                    set => SetRegister(KXShift, KXSize, value);
                }

                /// <summary>
                /// Interrupt Mask field, enables external, internal, coprocessors or software interrupts.
                /// </summary>
                public byte IM
                {
                    get => (byte)GetRegister(IMShift, IMSize);
                    set => SetRegister(IMShift, IMSize, value);
                }

                /// <summary>
                /// Diagnostic Status field.
                /// </summary>
                public ushort DS
                {
                    get => (ushort)GetRegister(DSShift, DSSize);
                    set => SetRegister(DSShift, DSSize, value);
                }

                /// <summary>
                /// Reverse-Endian bit, enables reverse of system endianness in User mode.
                /// </summary>
                public bool RE
                {
                    get => GetBoolean(REShift, RESize);
                    set => SetRegister(REShift, RESize, value);
                }

                /// <summary>
                /// Enables additional floating-point registers.
                /// </summary>
                public bool FR
                {
                    get => GetBoolean(FRShift, FRSize);
                    set => SetRegister(FRShift, FRSize, value);
                }

                /// <summary>
                /// Enables low-power operation by reducing the internal clock frequency and the system interface clock frequency to one-quarter speed.
                /// </summary>
                public bool RP
                {
                    get => GetBoolean(RPShift, RPSize);
                    set => SetRegister(RPShift, RPSize, value);
                }

                /// <summary>
                /// Controls the usability of each of the four coprocessor unit numbers.
                /// </summary>
                public CU ConfigCU
                {
                    get => (CU)GetRegister(CUShift, CUSize);
                    set => SetRegister(CUShift, CUSize, (ulong)value);
                }
                #endregion

                #region Constructors
                public StatusRegister(Coprocessor0 cp0)
                    : base(cp0) { }
                #endregion

                #region Enumerations
                public enum KSU : byte
                {
                    Kernel = 0b00,
                    Supervisor = 0b01,
                    User = 0b10
                }

                [Flags]
                public enum CU : byte
                {
                    None = 0,
                    CP0,
                    CP1,
                    CP2,
                    CP3
                }
                #endregion
            }
        }
    }
}
