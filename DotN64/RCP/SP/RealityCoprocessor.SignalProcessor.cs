using System;
using System.Collections.Generic;

namespace DotN64.RCP
{
    using Extensions;

    public partial class RealityCoprocessor
    {
        public class SignalProcessor
        {
            #region Fields
            private readonly IReadOnlyList<MappingEntry> memoryMaps;
            #endregion

            #region Properties
            public uint Status { get; set; } = 1;

            public uint DMABusy { get; set; }

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
                        Write = (o, v) =>
                        {
                            unsafe
                            {
                                fixed (byte* data = &IMEM[(int)o])
                                {
                                    *(uint*)data = v;
                                }
                            }
                        }
                    },
                    new MappingEntry(0x04040010, 0x04040013) // SP status.
                    {
                        Read = o => Status,
                        Write = (o, v) => Status = v
                    },
                    new MappingEntry(0x04040018, 0x0404001B) // SP DMA busy.
                    {
                        Read = o => DMABusy
                    },
                    new MappingEntry(0x04000000, 0x04000FFF) // SP_DMEM read/write.
                    {
                        Read = o => BitConverter.ToUInt32(DMEM, (int)o),
                        Write = (o, v) =>
                        {
                            unsafe
                            {
                                fixed (byte* data = &DMEM[(int)o])
                                {
                                    *(uint*)data = v;
                                }
                            }
                        }
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
}
