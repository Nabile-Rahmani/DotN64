using System.Collections.Generic;

namespace DotN64.AI
{
    using Extensions;

    public class AudioInterface
    {
        #region Fields
        private readonly IReadOnlyList<MappingEntry> memoryMaps;
        #endregion

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

        #region Constructors
        public AudioInterface()
        {
            memoryMaps = new[]
            {
                new MappingEntry(0x04500000, 0x04500003) // AI DRAM address.
                {
                    Write = (o, v) => DRAMAddress = v
                },
                new MappingEntry(0x04500004, 0x04500007) // AI length.
                {
                    Write = (o, v) => TransferLength = v
                }
            };
        }
        #endregion

        #region Methods
        public uint ReadWord(ulong address) => memoryMaps.GetEntry(address).ReadWord(address);

        public void WriteWord(ulong address, uint value) => memoryMaps.GetEntry(address).WriteWord(address, value);
        #endregion
    }
}
