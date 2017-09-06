namespace N64Emu.Interfaces.Video
{
    public partial class VideoInterface
    {
        #region Properties
        private ushort verticalInterrupt;
        /// <summary>
        /// Interrupt when current half-line = V_INTR.
        /// </summary>
        public ushort VerticalInterrupt
        {
            get => verticalInterrupt;
            set => verticalInterrupt = (ushort)(value & ((1 << 10) - 1));
        }

        public HorizontalVideoRegister HorizontalVideo { get; set; }

        private ushort currentVerticalLine;
        /// <summary>
        /// Current half line, sampled once per line (the lsb of V_CURRENT is constant within a field, and in interlaced modes gives the field number - which is constant for non-interlaced modes).
        /// </summary>
        public ushort CurrentVerticalLine
        {
            get => currentVerticalLine;
            set
            {
                currentVerticalLine = (ushort)(value & ((1 << 10) - 1));

                // TODO: Clear interrupt line.
            }
        }
        #endregion
    }
}
