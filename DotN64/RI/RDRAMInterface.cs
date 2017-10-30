using System.Collections.Generic;

namespace DotN64.RI
{
    using Extensions;

    public partial class RDRAMInterface
    {
        #region Fields
        private readonly IReadOnlyList<MappingEntry> memoryMaps;
        #endregion

        #region Properties
        public byte Select { get; set; }

        public ConfigRegister Config { get; set; }

        public ModeRegister Mode { get; set; }
        #endregion

        #region Constructors
        public RDRAMInterface()
        {
            memoryMaps = new[]
            {
                new MappingEntry(0x0470000C, 0x0470000F) // RI select.
                {
                    Read = o => Select,
                    Write = (o, v) => Select = (byte)(v & (1 << 3) - 1)
                },
                new MappingEntry(0x04700004, 0x04700007) // RI config.
                {
                    Write = (o, v) => Config = v
                },
                new MappingEntry(0x04700008, 0x0470000B) // RI current load.
                {
                    Write = (o, v) => { /* TODO: Any write updates current control register. */ }
                },
                new MappingEntry(0x04700000, 0x04700003) // RI mode.
                {
                    Write = (o, v) => Mode = v
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
