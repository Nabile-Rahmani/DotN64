namespace N64Emu.PI
{
    public partial class PeripheralInterface
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
