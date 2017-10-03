namespace N64Emu.AI
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
            set => dramAddress = value & ((1 << 24) - 1);
        }

        private uint transferLength;
        public uint TransferLength
        {
            get => transferLength;
            set => transferLength = (uint)(value & ((1 << 18) - 1) & ~((1 << 3) - 1)); // "v2.0".
        }
        #endregion
    }
}
