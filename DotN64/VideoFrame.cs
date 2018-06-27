namespace DotN64
{
    using static RCP.RealityCoprocessor;

    public struct VideoFrame : System.IEquatable<VideoFrame>
    {
        #region Properties
        public ushort Width { get; set; }

        public ushort Height { get; set; }

        public VideoInterface.ControlRegister.PixelSize Size { get; set; }
        #endregion

        #region Constructors
        public VideoFrame(VideoInterface vi)
        {
            Width = (ushort)((vi.HorizontalVideo.ActiveVideoEnd - vi.HorizontalVideo.ActiveVideoStart) * (float)vi.HorizontalScale.ScaleUpFactor / (1 << 10));
            Height = (ushort)(((vi.VerticalVideo.ActiveVideoEnd - vi.VerticalVideo.ActiveVideoStart) >> 1) * (float)vi.VerticalScale.ScaleUpFactor / (1 << 10));
            Size = vi.Control.Type;
        }
        #endregion

        #region Methods
        public bool Equals(VideoFrame other) => other.Width == Width && other.Height == Height && other.Size == Size;

        public override bool Equals(object obj) => obj is VideoFrame && Equals((VideoFrame)obj);

        public override int GetHashCode() => Width ^ Height ^ (byte)Size;
        #endregion

        #region Operators
        public static bool operator ==(VideoFrame left, VideoFrame right) => left.Equals(right);

        public static bool operator !=(VideoFrame left, VideoFrame right) => !left.Equals(right);
        #endregion
    }
}
