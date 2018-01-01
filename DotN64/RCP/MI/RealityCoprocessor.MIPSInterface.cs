using System.Collections.Specialized;

namespace DotN64.RCP
{
    using CPU;

    public partial class RealityCoprocessor
    {
        public partial class MIPSInterface : Interface
        {
            #region Fields
            private static readonly byte interruptPin = 1 << 0;
            #endregion

            #region Properties
            private VR4300 CPU => rcp.nintendo64.CPU;

            public InitModeRegister InitMode { get; set; }

            public Interrupts InterruptMask { get; set; }

            private Interrupts interrupt;
            public Interrupts Interrupt
            {
                get => interrupt;
                set
                {
                    interrupt = value;

                    UpdateInterrupt();
                }
            }
            #endregion

            #region Constructors
            public MIPSInterface(RealityCoprocessor rcp)
                : base(rcp)
            {
                MemoryMaps = new[]
                {
                    new MappingEntry(0x04300000, 0x04300003) // MI init mode.
                    {
                        Write = (o, v) =>
                        {
                            var bits = new BitVector32((int)v);
                            var mode = InitMode;

                            mode.InitLength = (byte)bits[InitModeRegister.InitLengthSection];

                            mode.InitMode &= bits[InitModeRegister.ClearInitModeSection] == 0;
                            mode.InitMode |= bits[InitModeRegister.SetInitModeSection] != 0;

                            mode.EBusTestMode &= bits[InitModeRegister.ClearEBusTestModeSection] == 0;
                            mode.EBusTestMode |= bits[InitModeRegister.SetEBusTestModeSection] != 0;

                            if (bits[InitModeRegister.ClearDPInterruptSection] != 0)
                                Interrupt &= ~Interrupts.DP;

                            mode.RDRAMRegMode &= bits[InitModeRegister.ClearRDRAMRegSection] == 0;
                            mode.RDRAMRegMode |= bits[InitModeRegister.SetRDRAMRegModeSection] != 0;

                            InitMode = mode;
                        }
                    },
                    new MappingEntry(0x04300004, 0x04300007) // MI version.
                    {
                        Read = o => 0 // TODO.
                    },
                    new MappingEntry(0x0430000C, 0x0430000F) // MI interrupt mask.
                    {
                        Write = (o, v) =>
                        {
                            var mask = (InterruptMaskWrites)v;

                            if ((mask & InterruptMaskWrites.ClearSP) != 0)
                                InterruptMask &= ~Interrupts.SP;

                            if ((mask & InterruptMaskWrites.SetSP) != 0)
                                InterruptMask |= Interrupts.SP;

                            if ((mask & InterruptMaskWrites.ClearSI) != 0)
                                InterruptMask &= ~Interrupts.SI;

                            if ((mask & InterruptMaskWrites.SetSI) != 0)
                                InterruptMask |= Interrupts.SI;

                            if ((mask & InterruptMaskWrites.ClearAI) != 0)
                                InterruptMask &= ~Interrupts.AI;

                            if ((mask & InterruptMaskWrites.SetAI) != 0)
                                InterruptMask |= Interrupts.AI;

                            if ((mask & InterruptMaskWrites.ClearVI) != 0)
                                InterruptMask &= ~Interrupts.VI;

                            if ((mask & InterruptMaskWrites.SetVI) != 0)
                                InterruptMask |= Interrupts.VI;

                            if ((mask & InterruptMaskWrites.ClearPI) != 0)
                                InterruptMask &= ~Interrupts.PI;

                            if ((mask & InterruptMaskWrites.SetPI) != 0)
                                InterruptMask |= Interrupts.PI;

                            if ((mask & InterruptMaskWrites.ClearDP) != 0)
                                InterruptMask &= ~Interrupts.DP;

                            if ((mask & InterruptMaskWrites.SetDP) != 0)
                                InterruptMask |= Interrupts.DP;

                            UpdateInterrupt();
                        }
                    }
                };
            }
            #endregion

            #region Methods
            private void UpdateInterrupt()
            {
                if ((Interrupt & InterruptMask) != 0)
                    CPU.Int |= interruptPin;
                else
                    CPU.Int &= (byte)~interruptPin;
            }
            #endregion
        }
    }
}
