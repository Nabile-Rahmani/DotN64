namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public partial class ParallelInterface
        {
            public class Domain
            {
                #region Properties
                /// <summary>
                /// Device latency.
                /// </summary>
                public byte Latency { get; set; }

                /// <summary>
                /// Device R/W strobe pulse width.
                /// </summary>
                public byte PulseWidth { get; set; }

                private byte pageSize;
                /// <summary>
                /// Device page size.
                /// </summary>
                public byte PageSize
                {
                    get => pageSize;
                    set => pageSize = (byte)(value & ((1 << 4) - 1));
                }

                private byte release;
                /// <summary>
                /// Device R/W release duration.
                /// </summary>
                public byte Release
                {
                    get => release;
                    set => release = (byte)(value & ((1 << 2) - 1));
                }
                #endregion
            }
        }
    }
}
