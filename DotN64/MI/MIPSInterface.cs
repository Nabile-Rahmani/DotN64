using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace DotN64.MI
{
    using Extensions;

    public partial class MIPSInterface
    {
        #region Fields
        private readonly IReadOnlyList<MappingEntry> memoryMaps;
        #endregion

        #region Properties
        public InitModeRegister InitMode { get; set; }
        #endregion

        #region Constructors
        public MIPSInterface()
        {
            memoryMaps = new[]
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

                        mode.EbusTestMode &= bits[InitModeRegister.ClearEbusTestModeSection] == 0;
                        mode.EbusTestMode |= bits[InitModeRegister.SetEbusTestModeSection] != 0;

                        if (bits[InitModeRegister.ClearDPInterruptSection] != 0)
                            throw new NotImplementedException("Clear DP interrupt.");

                        mode.RDRAMRegMode &= bits[InitModeRegister.ClearRDRAMRegSection] == 0;
                        mode.RDRAMRegMode |= bits[InitModeRegister.SetRDRAMRegModeSection] != 0;

                        InitMode = mode;
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
