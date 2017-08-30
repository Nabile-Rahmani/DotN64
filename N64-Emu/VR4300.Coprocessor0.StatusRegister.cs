using System;

namespace N64Emu
{
    public partial class VR4300
    {
        public partial class Coprocessor0
        {
            public class StatusRegister
            {
                #region Fields
                private readonly Coprocessor0 cp0;

                private const int IESize = 1, IEShift = 0;
                private const int EXLSize = 1, EXLShift = 1;
                private const int ERLSize = 1, ERLShift = 2;
                private const int KSUSize = 2, KSUShift = 3;
                private const int UXSize = 1, UXShift = 5;
                private const int SXSize = 1, SXShift = 6;
                private const int KXSize = 1, KXShift = 7;
                private const int IMSize = 8, IMShift = 8;
                private const int DSSize = 9, DSShift = 16;
                private const int RESize = 1, REShift = 25;
                private const int FRSize = 1, FRShift = 26;
                private const int RPSize = 1, RPShift = 27;
                private const int CUSize = 4, CUShift = 28;
                #endregion

                #region Properties
                /// <summary>
                /// Specifies and indicates global interrupt enable.
                /// </summary>
                public bool IE
                {
                    get => (cp0.Registers[(int)Register.Status] >> IEShift & IESize) != 0;
                    set
                    {
                        cp0.Registers[(int)Register.Status] &= ~(ulong)(IESize << IEShift);
                        cp0.Registers[(int)Register.Status] |= ((ulong)(value ? IESize : 0) & IESize) << IEShift;
                    }
                }

                /// <summary>
                /// Specifies and indicates exception level.
                /// </summary>
                public bool EXL
                {
                    get => (cp0.Registers[(int)Register.Status] >> EXLShift & EXLSize) != 0;
                    set
                    {
                        cp0.Registers[(int)Register.Status] &= ~(ulong)(EXLSize << EXLShift);
                        cp0.Registers[(int)Register.Status] |= ((ulong)(value ? EXLSize : 0) & EXLSize) << EXLShift;
                    }
                }

                /// <summary>
                /// Specifies and indicates error level.
                /// </summary>
                public bool ERL
                {
                    get => (cp0.Registers[(int)Register.Status] >> ERLShift & ERLSize) != 0;
                    set
                    {
                        cp0.Registers[(int)Register.Status] &= ~(ulong)(ERLSize << ERLShift);
                        cp0.Registers[(int)Register.Status] |= ((ulong)(value ? ERLSize : 0) & ERLSize) << ERLShift;
                    }
                }

                /// <summary>
                /// Specifies and indicates mode bits.
                /// </summary>
                public KSU ConfigKSU
                {
                    get => (KSU)(cp0.Registers[(int)Register.Config] >> KSUShift & KSUSize);
                    set
                    {
                        cp0.Registers[(int)Register.Config] &= ~(ulong)(KSUSize << KSUShift);
                        cp0.Registers[(int)Register.Config] |= ((ulong)value & KSUSize) << KSUShift;
                    }
                }

                /// <summary>
                /// Enables 64-bit addressing and operations in User mode.
                /// When this bit is set, XTLB miss exception is generated on TLB misses in User mode addresses space.
                /// </summary>
                public bool UX
                {
                    get => (cp0.Registers[(int)Register.Status] >> UXShift & UXSize) != 0;
                    set
                    {
                        cp0.Registers[(int)Register.Status] &= ~(ulong)(UXSize << UXShift);
                        cp0.Registers[(int)Register.Status] |= ((ulong)(value ? UXSize : 0) & UXSize) << UXShift;
                    }
                }

                /// <summary>
                /// Enables 64-bit addressing and operations in Supervisor mode.
                /// When this bit is set, XTLB miss exception is generated on TLB misses in Supervisor mode addresses space.
                /// </summary>
                public bool SX
                {
                    get => (cp0.Registers[(int)Register.Status] >> SXShift & SXSize) != 0;
                    set
                    {
                        cp0.Registers[(int)Register.Status] &= ~(ulong)(SXSize << SXShift);
                        cp0.Registers[(int)Register.Status] |= ((ulong)(value ? SXSize : 0) & SXSize) << SXShift;
                    }
                }

                /// <summary>
                /// Enables 64-bit addressing in Kernel mode.
                /// When this bit is set, XTLB miss exception is generated on TLB misses in Kernel mode addresses space.
                /// </summary>
                public bool KX
                {
                    get => (cp0.Registers[(int)Register.Status] >> KXShift & KXSize) != 0;
                    set
                    {
                        cp0.Registers[(int)Register.Status] &= ~(ulong)(KXSize << KXShift);
                        cp0.Registers[(int)Register.Status] |= ((ulong)(value ? KXSize : 0) & KXSize) << KXShift;
                    }
                }

                /// <summary>
                /// Interrupt Mask field, enables external, internal, coprocessors or software interrupts.
                /// </summary>
                public byte IM
                {
                    get => (byte)(cp0.Registers[(int)Register.Config] >> IMShift & IMSize);
                    set
                    {
                        cp0.Registers[(int)Register.Config] &= ~(ulong)(IMSize << IMShift);
                        cp0.Registers[(int)Register.Config] |= ((ulong)value & IMSize) << IMShift;
                    }
                }

                /// <summary>
                /// Diagnostic Status field.
                /// </summary>
                public ushort DS
                {
                    get => (ushort)(cp0.Registers[(int)Register.Config] >> DSShift & DSSize);
                    set
                    {
                        cp0.Registers[(int)Register.Config] &= ~(ulong)(DSSize << DSShift);
                        cp0.Registers[(int)Register.Config] |= ((ulong)value & DSSize) << DSShift;
                    }
                }

                /// <summary>
                /// Reverse-Endian bit, enables reverse of system endianness in User mode.
                /// </summary>
                public bool RE
                {
                    get => (cp0.Registers[(int)Register.Status] >> REShift & RESize) != 0;
                    set
                    {
                        cp0.Registers[(int)Register.Status] &= ~(ulong)(RESize << REShift);
                        cp0.Registers[(int)Register.Status] |= ((ulong)(value ? RESize : 0) & RESize) << REShift;
                    }
                }

                /// <summary>
                /// Enables additional floating-point registers.
                /// </summary>
                public bool FR
                {
                    get => (cp0.Registers[(int)Register.Status] >> FRShift & FRSize) != 0;
                    set
                    {
                        cp0.Registers[(int)Register.Status] &= ~(ulong)(FRSize << FRShift);
                        cp0.Registers[(int)Register.Status] |= ((ulong)(value ? FRSize : 0) & FRSize) << FRShift;
                    }
                }

                /// <summary>
                /// Enables low-power operation by reducing the internal clock frequency and the system interface clock frequency to one-quarter speed.
                /// </summary>
                public bool RP
                {
                    get => (cp0.Registers[(int)Register.Status] >> RPShift & RPSize) != 0;
                    set
                    {
                        cp0.Registers[(int)Register.Status] &= ~(ulong)(RPSize << RPShift);
                        cp0.Registers[(int)Register.Status] |= ((ulong)(value ? RPSize : 0) & RPSize) << RPShift;
                    }
                }

                /// <summary>
                /// Controls the usability of each of the four coprocessor unit numbers.
                /// </summary>
                public CU ConfigCU
                {
                    get => (CU)(cp0.Registers[(int)Register.Config] >> CUShift & CUSize);
                    set
                    {
                        cp0.Registers[(int)Register.Config] &= ~(ulong)(CUSize << CUShift);
                        cp0.Registers[(int)Register.Config] |= ((ulong)value & CUSize) << CUShift;
                    }
                }
                #endregion

                #region Constructors
                public StatusRegister(Coprocessor0 cp0)
                {
                    this.cp0 = cp0;
                }
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
