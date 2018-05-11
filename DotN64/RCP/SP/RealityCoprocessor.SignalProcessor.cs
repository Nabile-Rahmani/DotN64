using System;
using System.Collections.Generic;

namespace DotN64.RCP
{
    using Helpers;

    public partial class RealityCoprocessor
    {
        public partial class SignalProcessor
        {
            #region Fields
            private readonly RealityCoprocessor rcp;
            #endregion

            #region Properties
            public IReadOnlyList<MappingEntry> MemoryMaps { get; }

            public Statuses Status { get; set; } = Statuses.Halt;

            public bool DMABusy { get; set; }

            /// <summary>
            /// Instruction memory.
            /// </summary>
            public byte[] IMEM { get; } = new byte[0x1000];

            /// <summary>
            /// Data memory.
            /// </summary>
            public byte[] DMEM { get; } = new byte[0x1000];

            /// <summary>
            /// Program counter.
            /// </summary>
            public ushort PC { get; set; }
            #endregion

            #region Constructors
            public SignalProcessor(RealityCoprocessor rcp)
            {
                this.rcp = rcp;
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
                            var status = (StatusWrites)v;

                            void Clear(StatusWrites clearMask, Statuses value)
                            {
                                if ((status & clearMask) != 0)
                                    Status &= ~value;
                            }

                            void Set(StatusWrites setMask, Statuses value)
                            {
                                if ((status & setMask) != 0)
                                    Status |= value;
                            }

                            Clear(StatusWrites.ClearHalt, Statuses.Halt);
                            Set(StatusWrites.SetHalt, Statuses.Halt);

                            Clear(StatusWrites.ClearBroke, Statuses.Broke);

                            if ((status & StatusWrites.ClearInterrupt) != 0)
                                rcp.MI.Interrupt &= ~MIPSInterface.Interrupts.SP;

                            if ((status & StatusWrites.SetInterrupt) != 0)
                                rcp.MI.Interrupt |= MIPSInterface.Interrupts.SP;

                            Clear(StatusWrites.ClearSingleStep, Statuses.SingleStep);
                            Set(StatusWrites.SetSingleStep, Statuses.SingleStep);

                            Clear(StatusWrites.ClearInterruptOnBreak, Statuses.InterruptOnBreak);
                            Set(StatusWrites.SetInterruptOnBreak, Statuses.InterruptOnBreak);

                            Clear(StatusWrites.ClearSignal0, Statuses.Signal0);
                            Set(StatusWrites.SetSignal0, Statuses.Signal0);

                            Clear(StatusWrites.ClearSignal1, Statuses.Signal1);
                            Set(StatusWrites.SetSignal1, Statuses.Signal1);

                            Clear(StatusWrites.ClearSignal2, Statuses.Signal2);
                            Set(StatusWrites.SetSignal2, Statuses.Signal2);

                            Clear(StatusWrites.ClearSignal3, Statuses.Signal3);
                            Set(StatusWrites.SetSignal3, Statuses.Signal3);

                            Clear(StatusWrites.ClearSignal4, Statuses.Signal4);
                            Set(StatusWrites.SetSignal4, Statuses.Signal4);

                            Clear(StatusWrites.ClearSignal5, Statuses.Signal5);
                            Set(StatusWrites.SetSignal5, Statuses.Signal5);

                            Clear(StatusWrites.ClearSignal6, Statuses.Signal6);
                            Set(StatusWrites.SetSignal6, Statuses.Signal6);

                            Clear(StatusWrites.ClearSignal7, Statuses.Signal7);
                            Set(StatusWrites.SetSignal7, Statuses.Signal7);
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
                    },
                    new MappingEntry(0x04080000, 0x04080003) // SP PC.
                    {
                        Read = o => PC
                    }
                };
            }
            #endregion
        }
    }
}
