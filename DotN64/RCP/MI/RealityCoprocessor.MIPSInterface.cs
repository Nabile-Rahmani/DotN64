using System.Collections.Specialized;

namespace DotN64.RCP
{
    using CPU;

    public partial class RealityCoprocessor
    {
        public partial class MIPSInterface : Interface
        {
            #region Properties
            private VR4300 CPU => rcp.nintendo64.CPU;

            public InitModeRegister InitMode { get; set; }

            public VersionRegister Version { get; set; } = VersionRegister.Version2;

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
                        Write = (o, d) =>
                        {
                            var bits = new BitVector32((int)d);
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
                        Read = o => Version
                    },
                    new MappingEntry(0x04300008, 0x0430000B) // MI interrupt.
                    {
                        Read = o => (uint)Interrupt
                    },
                    new MappingEntry(0x0430000C, 0x0430000F) // MI interrupt mask.
                    {
                        Read = o => (uint)InterruptMask,
                        Write = (o, d) =>
                        {
                            var mask = (InterruptMaskWrites)d;

                            void Clear(InterruptMaskWrites clearMask, Interrupts interrupt)
                            {
                                if ((mask & clearMask) != 0)
                                    InterruptMask &= ~interrupt;
                            }

                            void Set(InterruptMaskWrites setMask, Interrupts interrupt)
                            {
                                if ((mask & setMask) != 0)
                                    InterruptMask |= interrupt;
                            }

                            Clear(InterruptMaskWrites.ClearSP, Interrupts.SP);
                            Set(InterruptMaskWrites.SetSP, Interrupts.SP);

                            Clear(InterruptMaskWrites.ClearSI, Interrupts.SI);
                            Set(InterruptMaskWrites.SetSI, Interrupts.SI);

                            Clear(InterruptMaskWrites.ClearAI, Interrupts.AI);
                            Set(InterruptMaskWrites.SetAI, Interrupts.AI);

                            Clear(InterruptMaskWrites.ClearVI, Interrupts.VI);
                            Set(InterruptMaskWrites.SetVI, Interrupts.VI);

                            Clear(InterruptMaskWrites.ClearPI, Interrupts.PI);
                            Set(InterruptMaskWrites.SetPI, Interrupts.PI);

                            Clear(InterruptMaskWrites.ClearDP, Interrupts.DP);
                            Set(InterruptMaskWrites.SetDP, Interrupts.DP);

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
                    CPU.Int |= (byte)InterruptPins.RCP;
                else
                    CPU.Int &= (byte)~InterruptPins.RCP;
            }
            #endregion
        }
    }
}
