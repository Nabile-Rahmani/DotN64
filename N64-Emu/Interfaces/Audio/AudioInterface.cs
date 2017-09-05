namespace N64Emu.Interfaces.Audio
{
    public class AudioInterface
    {
        #region Properties
        private uint dramAddress;
        /// <summary>
        /// Starting RDRAM address (8B-aligned).
        /// </summary>
        public uint DRAMAddress
        {
            get => dramAddress;
            set => dramAddress = value & ((1 << 23) - 1);
        }

        private uint transferLength;
        public uint TransferLength
        {
            get => transferLength;
            set => transferLength = (uint)(value & ((1 << 17) - 1) & ~((1 << 3) - 1)); // "v2.0".
        }
        #endregion
    }
}
