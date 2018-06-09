using System;
using System.Runtime.CompilerServices;

namespace DotN64.CPU
{
    using static Helpers.BitHelper;

    public partial class VR4300
    {
        /// <summary>
        /// See: datasheet#3.1.
        /// </summary>
        public struct Instruction : IEquatable<Instruction>
        {
            #region Fields
            private uint data;

            private const int OPShift = 26, OPSize = (1 << 6) - 1;
            private const int RSShift = 21, RSSize = (1 << 5) - 1;
            private const int RTShift = 16, RTSize = (1 << 5) - 1;
            private const int ImmediateShift = 0, ImmediateSize = (1 << 16) - 1;
            private const int TargetShift = 0, TargetSize = (1 << 26) - 1;
            private const int RDShift = 11, RDSize = (1 << 5) - 1;
            private const int SAShift = 6, SASize = (1 << 5) - 1;
            private const int FunctShift = 0, FunctSize = (1 << 6) - 1;
            private const byte COPzSize = (1 << 2) - 1;
            public const int Size = sizeof(uint);
            #endregion

            #region Properties
            /// <summary>
            /// 6-bit operation code.
            /// </summary>
            public OpCode OP
            {
                get => (OpCode)Get(data, OPShift, OPSize);
                set => Set(ref data, OPShift, OPSize, (uint)value);
            }

            /// <summary>
            /// 5-bit source register number.
            /// </summary>
            public byte RS
            {
                get => (byte)Get(data, RSShift, RSSize);
                set => Set(ref data, RSShift, RSSize, value);
            }

            /// <summary>
            /// 5-bit target (source/destination) register number or branch condition.
            /// </summary>
            public byte RT
            {
                get => (byte)Get(data, RTShift, RTSize);
                set => Set(ref data, RTShift, RTSize, value);
            }

            /// <summary>
            /// 16-bit immediate value, branch displacement or address displacement.
            /// </summary>
            public ushort Immediate
            {
                get => (ushort)Get(data, ImmediateShift, ImmediateSize);
                set => Set(ref data, ImmediateShift, ImmediateSize, value);
            }

            /// <summary>
            /// 26-bit unconditional branch target address.
            /// </summary>
            public uint Target
            {
                get => Get(data, TargetShift, TargetSize);
                set => Set(ref data, TargetShift, TargetSize, value);
            }

            /// <summary>
            /// 5-bit destination register number.
            /// </summary>
            public byte RD
            {
                get => (byte)Get(data, RDShift, RDSize);
                set => Set(ref data, RDShift, RDSize, value);
            }

            /// <summary>
            /// 5-bit shift amount.
            /// </summary>
            public byte SA
            {
                get => (byte)Get(data, SAShift, SASize);
                set => Set(ref data, SAShift, SASize, value);
            }

            /// <summary>
            /// 6-bit function field.
            /// </summary>
            public byte Funct
            {
                get => (byte)Get(data, FunctShift, FunctSize);
                set => Set(ref data, FunctShift, FunctSize, value);
            }

            public SpecialOpCode? Special
            {
                get => OP == OpCode.SPECIAL ? (SpecialOpCode?)Funct : null;
                set
                {
                    OP = OpCode.SPECIAL;
                    Funct = (byte)value;
                }
            }

            public RegImmOpCode? RegImm
            {
                get => OP == OpCode.REGIMM ? (RegImmOpCode?)RT : null;
                set
                {
                    OP = OpCode.REGIMM;
                    RT = (byte)value;
                }
            }

            /// <summary>
            /// Gets the coprocessor unit index.
            /// </summary>
            public byte? COPz => (OpCode)((byte)OP & ~COPzSize) == OpCode.COP0 ? (byte?)((byte)OP & COPzSize) : null;
            #endregion

            #region Methods
            public static Instruction From(OpCode op) => new Instruction { OP = op };

            public static Instruction From(SpecialOpCode op) => new Instruction { Special = op };

            public static Instruction From(RegImmOpCode op) => new Instruction { RegImm = op };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Instruction ToOpCode()
            {
                switch (OP)
                {
                    case OpCode.SPECIAL:
                        return new Instruction
                        {
                            OP = OP,
                            Funct = Funct
                        };
                    case OpCode.REGIMM:
                        return new Instruction
                        {
                            OP = OP,
                            RT = RT
                        };
                    default:
                        return new Instruction { OP = OP };
                }
            }

            public bool Equals(Instruction other) => other.data == data;

            public override bool Equals(object obj) => obj is Instruction && ((Instruction)obj).data == data;

            public override int GetHashCode() => (int)data;

            public override string ToString()
            {
                switch (OP)
                {
                    case OpCode.SPECIAL:
                        return Special.ToString();
                    case OpCode.REGIMM:
                        return RegImm.ToString();
                    default:
                        return OP.ToString();
                }
            }
            #endregion

            #region Operators
            public static implicit operator Instruction(uint value) => new Instruction { data = value };

            public static implicit operator uint(Instruction instruction) => instruction.data;
            #endregion
        }
    }
}
