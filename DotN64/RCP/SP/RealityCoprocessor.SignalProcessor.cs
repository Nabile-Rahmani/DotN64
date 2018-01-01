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

                            if ((status & StatusWrites.ClearHalt) != 0)
                                Status &= ~Statuses.Halt;

                            if ((status & StatusWrites.SetHalt) != 0)
                                Status |= Statuses.Halt;

                            if ((status & StatusWrites.ClearBroke) != 0)
                                Status &= ~Statuses.Broke;

                            if ((status & StatusWrites.ClearInterrupt) != 0)
                                rcp.MI.Interrupt &= ~MIPSInterface.Interrupts.SP;

                            if ((status & StatusWrites.SetInterrupt) != 0)
                                rcp.MI.Interrupt |= MIPSInterface.Interrupts.SP;

                            if ((status & StatusWrites.ClearSingleStep) != 0)
                                Status &= ~Statuses.SingleStep;

                            if ((status & StatusWrites.SetSingleStep) != 0)
                                Status |= Statuses.SingleStep;

                            if ((status & StatusWrites.ClearInterruptOnBreak) != 0)
                                Status &= ~Statuses.InterruptOnBreak;

                            if ((status & StatusWrites.SetInterruptOnBreak) != 0)
                                Status |= Statuses.InterruptOnBreak;

                            if ((status & StatusWrites.ClearSignal0) != 0)
                                Status &= ~Statuses.Signal0;

                            if ((status & StatusWrites.SetSignal0) != 0)
                                Status |= Statuses.Signal0;

                            if ((status & StatusWrites.ClearSignal1) != 0)
                                Status &= ~Statuses.Signal1;

                            if ((status & StatusWrites.SetSignal1) != 0)
                                Status |= Statuses.Signal1;

                            if ((status & StatusWrites.ClearSignal2) != 0)
                                Status &= ~Statuses.Signal2;

                            if ((status & StatusWrites.SetSignal2) != 0)
                                Status |= Statuses.Signal2;

                            if ((status & StatusWrites.ClearSignal3) != 0)
                                Status &= ~Statuses.Signal3;

                            if ((status & StatusWrites.SetSignal3) != 0)
                                Status |= Statuses.Signal3;

                            if ((status & StatusWrites.ClearSignal4) != 0)
                                Status &= ~Statuses.Signal4;

                            if ((status & StatusWrites.SetSignal4) != 0)
                                Status |= Statuses.Signal4;

                            if ((status & StatusWrites.ClearSignal5) != 0)
                                Status &= ~Statuses.Signal5;

                            if ((status & StatusWrites.SetSignal5) != 0)
                                Status |= Statuses.Signal5;

                            if ((status & StatusWrites.ClearSignal6) != 0)
                                Status &= ~Statuses.Signal6;

                            if ((status & StatusWrites.SetSignal6) != 0)
                                Status |= Statuses.Signal6;

                            if ((status & StatusWrites.ClearSignal7) != 0)
                                Status &= ~Statuses.Signal7;

                            if ((status & StatusWrites.SetSignal7) != 0)
                                Status |= Statuses.Signal7;
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
