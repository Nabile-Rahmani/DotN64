namespace DotN64.CPU
{
    public partial class VR4300
    {
        public enum GPRIndex : byte
        {
            /// <summary>Hardwired to zero.</summary>
            Zero,
            /// <summary>Assembler temporary (used for pseudo-ops).</summary>
            AT,
            /// <summary>Function return values.</summary>
            V0,
            V1,
            /// <summary>Function arguments.</summary>
            A0,
            A1,
            A2,
            A3,
            /// <summary>Temporaries.</summary>
            T0,
            T1,
            T2,
            T3,
            T4,
            T5,
            T6,
            T7,
            /// <summary>Saved temporaries.</summary>
            S0,
            S1,
            S2,
            S3,
            S4,
            S5,
            S6,
            S7,
            /// <summary>Temporaries.</summary>
            T8,
            T9,
            /// <summary>Reserved for kernel.</summary>
            K0,
            K1,
            /// <summary>Global pointer.</summary>
            GP,
            /// <summary>Stack pointer.</summary>
            SP,
            /// <summary>Frame pointer or saved temporary.</summary>
            FP,
            /// <summary>Return address.</summary>
            RA
        }
    }
}
