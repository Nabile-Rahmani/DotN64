namespace DotN64
{
    using static RCP.RealityCoprocessor;

    public interface IVideoOutput
    {
        #region Methods
        void Draw(VideoFrame frame, VideoInterface vi, RDRAM ram);
        #endregion
    }
}
