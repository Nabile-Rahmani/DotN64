﻿using System.Runtime.CompilerServices;

namespace DotN64.CPU
{
    public partial class VR4300
    {
        /// <summary>
        /// See: datasheet#6.4.
        /// </summary>
        private static class ExceptionProcessing
        {
            #region Fields
            private const ulong ResetVector = 0xFFFFFFFFBFC00000,
                                GeneralVector = 0xFFFFFFFF80000000,
                                GeneralVectorBEV = 0xFFFFFFFFBFC00200;
            private const ushort GeneralVectorOffset = 0x0180;
            #endregion

            #region Methods
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static void HandleReset(VR4300 cpu)
            {
                cpu.CP0.Registers[(int)SystemControlUnit.RegisterIndex.ErrorEPC] = cpu.PC;
                cpu.PC = ResetVector;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static void HandleGeneral(VR4300 cpu, SystemControlUnit.CauseRegister.ExceptionCode excCode, byte? ce = null)
            {
                cpu.CP0.Cause.ExcCode = excCode;

                if (ce.HasValue)
                    cpu.CP0.Cause.CE = ce.Value;

                cpu.CP0.Registers[(int)SystemControlUnit.RegisterIndex.BadVAddr] = cpu.PC;

                if (!cpu.CP0.Status.EXL)
                {
                    if ((cpu.CP0.Cause.BD = cpu.branchDelay))
                        cpu.CP0.Registers[(int)SystemControlUnit.RegisterIndex.EPC] = cpu.PC - Instruction.Size;
                    else
                        cpu.CP0.Registers[(int)SystemControlUnit.RegisterIndex.EPC] = cpu.PC;
                }

                cpu.CP0.Status.EXL = true;
                cpu.PC = (cpu.CP0.Status.DS.BEV ? GeneralVectorBEV : GeneralVector) + GeneralVectorOffset;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void ColdReset(VR4300 cpu)
            {
                var ds = cpu.CP0.Status.DS;

                ds.TS = ds.SR = cpu.CP0.Status.RP = false;
                cpu.CP0.Config.EP = 0;

                cpu.CP0.Status.ERL = ds.BEV = true;
                cpu.CP0.Config.BE = (SystemControlUnit.ConfigRegister.Endianness)1;

                cpu.CP0.Registers[(int)SystemControlUnit.RegisterIndex.Random] = 31;

                cpu.CP0.Config.EC = cpu.DivMode;

                cpu.CP0.Status.DS = ds;

                HandleReset(cpu);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Interrupt(VR4300 cpu) => HandleGeneral(cpu, SystemControlUnit.CauseRegister.ExceptionCode.Int);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void ReservedInstruction(VR4300 cpu, Instruction instruction)
            {
                HandleGeneral(cpu, SystemControlUnit.CauseRegister.ExceptionCode.RI);

                throw new UnimplementedOperationException(instruction); // TODO: Remove this and the parameter once every instruction gets implemented.
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void CoprocessorUnusable(VR4300 cpu, byte unit) => HandleGeneral(cpu, SystemControlUnit.CauseRegister.ExceptionCode.CpU, unit);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void FloatingPoint(VR4300 cpu) => HandleGeneral(cpu, SystemControlUnit.CauseRegister.ExceptionCode.FPE);
            #endregion
        }
    }
}
