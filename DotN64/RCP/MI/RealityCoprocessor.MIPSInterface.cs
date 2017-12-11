using System;
using System.Collections.Specialized;

namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public partial class MIPSInterface : Interface
        {
            #region Properties
            public InitModeRegister InitMode { get; set; }
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
                                throw new NotImplementedException("Clear DP interrupt.");

                            mode.RDRAMRegMode &= bits[InitModeRegister.ClearRDRAMRegSection] == 0;
                            mode.RDRAMRegMode |= bits[InitModeRegister.SetRDRAMRegModeSection] != 0;

                            InitMode = mode;
                        }
                    },
                    new MappingEntry(0x04300004, 0x04300007) // MI version.
                    {
                        Read = o => 0 // TODO.
                    }
                };
            }
            #endregion
        }
    }
}
