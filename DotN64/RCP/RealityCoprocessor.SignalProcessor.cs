using System;
using System.Collections.Generic;
using System.Linq;

namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public class SignalProcessor
        {
            #region Fields
            private readonly IReadOnlyList<MappingEntry> memoryMaps;
            #endregion

            #region Properties
            public uint StatusRegister { get; set; } = 1;

            public uint DMABusyRegister { get; set; }

            public byte[] IMEM { get; } = new byte[0x1000];

            public byte[] DMEM { get; } = new byte[0x1000];
            #endregion

            #region Constructors
            public SignalProcessor()
            {
                memoryMaps = new[]
                {
                    new MappingEntry(0x04001000, 0x04001FFF) // SP_IMEM read/write.
                    {
                        Read = o => BitConverter.ToUInt32(IMEM, (int)o),
                        Write = (o, v) => Array.Copy(BitConverter.GetBytes(v), 0, IMEM, (int)o, sizeof(uint))
                    },
                    new MappingEntry(0x04040010, 0x04040013) // SP status.
                    {
                        Read = o => StatusRegister,
                        Write = (o, v) => StatusRegister = v
                    },
                    new MappingEntry(0x04040018, 0x0404001B) // SP DMA busy.
                    {
                        Read = o => DMABusyRegister
                    },
                    new MappingEntry(0x04000000, 0x04000FFF) // SP_DMEM read/write.
                    {
                        Read = o => BitConverter.ToUInt32(DMEM, (int)o),
                        Write = (o, v) => Array.Copy(BitConverter.GetBytes(v), 0, DMEM, (int)o, sizeof(uint))
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
}
