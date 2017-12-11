using System;
using System.Collections.Generic;

namespace DotN64.RCP
{
    using Helpers;

    public partial class RealityCoprocessor
    {
        public partial class SignalProcessor
        {
            #region Properties
            public IReadOnlyList<MappingEntry> MemoryMaps { get; }

            public StatusRegister Status { get; set; } = StatusRegister.Halt;

            public bool DMABusy { get; set; }

            /// <summary>
            /// Instruction memory.
            /// </summary>
            public byte[] IMEM { get; } = new byte[0x1000];

            /// <summary>
            /// Data memory.
            /// </summary>
            public byte[] DMEM { get; } = new byte[0x1000];
            #endregion

            #region Constructors
            public SignalProcessor()
            {
                MemoryMaps = new[]
                {
                    new MappingEntry(0x04001000, 0x04001FFF) // SP_IMEM read/write.
                    {
                        Read = o => BitConverter.ToUInt32(IMEM, (int)o),
                        Write = (o, v) => BitHelper.Write(IMEM, (int)o, v)
                    },
                    new MappingEntry(0x04040010, 0x04040013) // SP status.
                    {
                        Read = o => (uint)Status,
                        Write = (o, v) =>
                        {
                            var status = (WriteStatusRegister)v;

                            if (status.HasFlag(WriteStatusRegister.ClearHalt))
                                Status &= ~StatusRegister.Halt;

                            if (status.HasFlag(WriteStatusRegister.SetHalt))
                                Status |= StatusRegister.Halt;

                            if (status.HasFlag(WriteStatusRegister.ClearBroke))
                                Status &= ~StatusRegister.Broke;

                            if (status.HasFlag(WriteStatusRegister.ClearInterrupt)) { /* TODO. */ }

                            if (status.HasFlag(WriteStatusRegister.SetInterrupt)) { /* TODO. */ }

                            if (status.HasFlag(WriteStatusRegister.ClearSingleStep))
                                Status &= ~StatusRegister.SingleStep;

                            if (status.HasFlag(WriteStatusRegister.SetSingleStep))
                                Status |= StatusRegister.SingleStep;

                            if (status.HasFlag(WriteStatusRegister.ClearInterruptOnBreak))
                                Status &= ~StatusRegister.InterruptOnBreak;

                            if (status.HasFlag(WriteStatusRegister.SetInterruptOnBreak))
                                Status |= StatusRegister.InterruptOnBreak;

                            if (status.HasFlag(WriteStatusRegister.ClearSignal0))
                                Status &= ~StatusRegister.Signal0;

                            if (status.HasFlag(WriteStatusRegister.SetSignal0))
                                Status |= StatusRegister.Signal0;

                            if (status.HasFlag(WriteStatusRegister.ClearSignal1))
                                Status &= ~StatusRegister.Signal1;

                            if (status.HasFlag(WriteStatusRegister.SetSignal1))
                                Status |= StatusRegister.Signal1;

                            if (status.HasFlag(WriteStatusRegister.ClearSignal2))
                                Status &= ~StatusRegister.Signal2;

                            if (status.HasFlag(WriteStatusRegister.SetSignal2))
                                Status |= StatusRegister.Signal2;

                            if (status.HasFlag(WriteStatusRegister.ClearSignal3))
                                Status &= ~StatusRegister.Signal3;

                            if (status.HasFlag(WriteStatusRegister.SetSignal3))
                                Status |= StatusRegister.Signal3;

                            if (status.HasFlag(WriteStatusRegister.ClearSignal4))
                                Status &= ~StatusRegister.Signal4;

                            if (status.HasFlag(WriteStatusRegister.SetSignal4))
                                Status |= StatusRegister.Signal4;

                            if (status.HasFlag(WriteStatusRegister.ClearSignal5))
                                Status &= ~StatusRegister.Signal5;

                            if (status.HasFlag(WriteStatusRegister.SetSignal5))
                                Status |= StatusRegister.Signal5;

                            if (status.HasFlag(WriteStatusRegister.ClearSignal6))
                                Status &= ~StatusRegister.Signal6;

                            if (status.HasFlag(WriteStatusRegister.SetSignal6))
                                Status |= StatusRegister.Signal6;

                            if (status.HasFlag(WriteStatusRegister.ClearSignal7))
                                Status &= ~StatusRegister.Signal7;

                            if (status.HasFlag(WriteStatusRegister.SetSignal7))
                                Status |= StatusRegister.Signal7;
                        }
                    },
                    new MappingEntry(0x04040018, 0x0404001B) // SP DMA busy.
                    {
                        Read = o => Convert.ToUInt32(DMABusy)
                    },
                    new MappingEntry(0x04000000, 0x04000FFF) // SP_DMEM read/write.
                    {
                        Read = o => BitConverter.ToUInt32(DMEM, (int)o),
                        Write = (o, v) => BitHelper.Write(DMEM, (int)o, v)
                    }
                };
            }
            #endregion
        }
    }
}
