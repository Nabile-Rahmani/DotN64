using System.Collections.Generic;
using System.Linq;

namespace N64Emu.MI
{
    public class MIPSInterface
    {
        #region Fields
        private readonly IReadOnlyList<MappingEntry> memoryMaps;
        #endregion

        #region Constructors
        public MIPSInterface()
        {
            memoryMaps = new[]
            {
                new MappingEntry(0x04300004, 0x04300007) // MI version.
                {
                    Write = (o, v) => { }
                }
            };
        }
        #endregion

        #region Methods
        public uint ReadWord(ulong address) => memoryMaps.First(e => e.Contains(address)).ReadWord(address);

        public void WriteWord(ulong address, uint value) => memoryMaps.First(e => e.Contains(address)).WriteWord(address, value);
        #endregion
    }
}
