namespace DotN64.CPU
{
    public partial class VR4300
    {
        public enum GPRIndex : byte
        {
            /// <summary>Hardwired to zero.</summary>
            zero,
            /// <summary>Assembler temporary (used for pseudo-ops).</summary>
            at,
            /// <summary>Function return values.</summary>
            v0,
            v1,
            /// <summary>Function arguments.</summary>
            a0,
            a1,
            a2,
            a3,
            /// <summary>Temporaries.</summary>
            t0,
            t1,
            t2,
            t3,
            t4,
            t5,
            t6,
            t7,
            /// <summary>Saved temporaries.</summary>
            s0,
            s1,
            s2,
            s3,
            s4,
            s5,
            s6,
            s7,
            /// <summary>Temporaries.</summary>
            t8,
            t9,
            /// <summary>Reserved for kernel.</summary>
            k0,
            k1,
            /// <summary>Global pointer.</summary>
            gp,
            /// <summary>Stack pointer.</summary>
            sp,
            /// <summary>Frame pointer or saved temporary.</summary>
            fp,
            /// <summary>Return address.</summary>
            ra
        }
    }
}
