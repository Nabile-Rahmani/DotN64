using System.Collections.Generic;

namespace DotN64.SI
{
    using Extensions;

    public partial class SerialInterface
    {
        #region Fields
        private readonly IReadOnlyList<MappingEntry> memoryMaps;
        #endregion

        #region Properties
        public StatusRegister Status { get; } = new StatusRegister();
        #endregion

        #region Constructors
        public SerialInterface()
        {
            memoryMaps = new[]
            {
                new MappingEntry(0x04800018, 0x0480001B) // SI status.
                {
                    Read = o => (uint)Status.Bits.Data,
                    Write = (o, v) => Status.Interrupt = false
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
