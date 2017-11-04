using System;
using System.Collections.Generic;
using System.Net;

namespace DotN64.RCP
{
    using Extensions;

    public partial class RealityCoprocessor
    {
        public partial class PeripheralInterface : Interface
        {
            #region Fields
            private readonly IReadOnlyList<MappingEntry> memoryMaps;

            private const byte CICStatusOffset = 60;
            private const byte ResetControllerStatus = 1 << 0, ClearInterruptStatus = 1 << 1;
            #endregion

            #region Properties
            protected override IReadOnlyList<MappingEntry> MemoryMaps => memoryMaps;

            public StatusRegister Status { get; set; }

            public byte[] BootROM { get; set; }

            public byte[] RAM { get; } = new byte[64];

            public Domain[] Domains { get; } = new[]
            {
                new Domain(),
                new Domain()
            };

            /// <summary>
            /// Starting RDRAM address.
            /// </summary>
            public uint DRAMAddress { get; set; }

            /// <summary>
            /// Starting AD16 address.
            /// </summary>
            public uint PBusAddress { get; set; }

            /// <summary>
            /// Write data length.
            /// </summary>
            public uint WriteLength { get; set; }
            #endregion

            #region Constructors
            public PeripheralInterface(RealityCoprocessor rcp)
                : base(rcp)
            {
                memoryMaps = new[]
                {
                    new MappingEntry(0x1FC00000, 0x1FC007BF) // PIF Boot ROM.
                    {
                        Read = o => (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(BootROM, (int)o))
                    },
                    new MappingEntry(0x1FC007C0, 0x1FC007FF) // PIF (JoyChannel) RAM.
                    {
                        Read = o => BitConverter.ToUInt32(RAM, (int)o),
                        Write = (o, v) =>
                        {
                            unsafe
                            {
                                fixed (byte* data = &RAM[(int)o])
                                {
                                    *(uint*)data = v;
                                }
                            }

                            if (o == CICStatusOffset && RAM[o] == (byte)CICStatus.Waiting) // The boot ROM waits for the PIF's CIC check to be OK.
                                RAM[o] = (byte)CICStatus.OK; // We tell it it's OK by having the loaded word that gets ANDI'd match the immediate value 128, storing non-zero which allows us to exit the BEQL loop.
                        }
                    },
                    new MappingEntry(0x04600010, 0x04600013) // PI status.
                    {
                        Read = o => (uint)Status,
                        Write = (o, v) =>
                        {
                            if ((v & ResetControllerStatus) != 0)
                                ResetController();

                            if ((v & ClearInterruptStatus) != 0)
                                ClearInterrupt();
                        }
                    },
                    new MappingEntry(0x04600014, 0x04600017) // PI dom1 latency.
                    {
                        Write = (o, v) => Domains[0].Latency = (byte)v
                    },
                    new MappingEntry(0x04600018, 0x0460001B) // PI dom1 pulse width.
                    {
                        Write = (o, v) => Domains[0].PulseWidth = (byte)v
                    },
                    new MappingEntry(0x0460001C, 0x0460001F) // PI dom1 page size.
                    {
                        Write = (o, v) => Domains[0].PageSize = (byte)v
                    },
                    new MappingEntry(0x04600020, 0x04600023) // PI dom1 release.
                    {
                        Write = (o, v) => Domains[0].Release = (byte)v
                    },
                    new MappingEntry(0x04600000, 0x04600003) // PI DRAM address.
                    {
                        Write = (o, v) => DRAMAddress = v & ((1 << 24) - 1)
                    },
                    new MappingEntry(0x04600004, 0x04600007) // PI pbus (cartridge) address.
                    {
                        Write = (o, v) => PBusAddress = v
                    },
                    new MappingEntry(0x0460000C, 0x0460000F) // PI write length.
                    {
                        Write = (o, v) => WriteLength = v & ((1 << 24) - 1)
                    },
                    new MappingEntry(0x10000000, 0x1FBFFFFF) // Cartridge Domain 1 Address 2.
                    {
                        Read = o => (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(rcp.Nintendo64.Cartridge.ROM, (int)o))
                    }
                };
            }
            #endregion

            #region Methods
            private void ClearInterrupt() { /* TODO: Implement. */ }

            private void ResetController() { /* TODO: Implement. */ }

            public void EmulateBootROM()
            {
                // Replicating the memory writes to properly initialise the subsystems.
                var writes = new uint[,]
                {
                    { 0x4040010, 0xA },
                    { 0x4600010, 0x3 },
                    { 0x440000C, 0x3FF },
                    { 0x4400024, 0x0 },
                    { 0x4400010, 0x0 },
                    { 0x4500000, 0x0 },
                    { 0x4500004, 0x0 },
                    { 0x4600014, 0x40 }, // These four are likely cartridge-specific (PI domain 1 values).
                    { 0x4600018, 0xFF803712 },
                    { 0x460001C, 0xFFFF8037 },
                    { 0x4600020, 0xFFFFF803 },
                    // Omitted the CIC result.
                    { 0x1FC007FC, 0xC0 }
                };

                for (int i = 0; i < writes.GetLength(0); i++)
                {
                    var address = (ulong)writes[i, 0];

                    rcp.Nintendo64.MemoryMaps.GetEntry(address).WriteWord(address, writes[i, 1]);
                }

                for (int i = 0x40; i < 0x1000; i += sizeof(uint)) // Copying the bootstrap code from the cartridge to the RSP's DMEM.
                {
                    var address = (ulong)(0x04000000 + i);

                    rcp.Nintendo64.MemoryMaps.GetEntry(address).WriteWord(address, (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(rcp.Nintendo64.Cartridge.ROM, i)));
                }

                // Restoring CPU state.
                rcp.Nintendo64.CPU.CP0.Registers[12] = 0x34000000;
                rcp.Nintendo64.CPU.CP0.Registers[16] = 0x6E463;
                rcp.Nintendo64.CPU.GPR[1] = 0x1;
                rcp.Nintendo64.CPU.GPR[2] = 0x6459969A;
                rcp.Nintendo64.CPU.GPR[3] = 0x6459969A;
                // Omitted the CIC result.
                rcp.Nintendo64.CPU.GPR[6] = 0xFFFFFFFFA4001F0C;
                rcp.Nintendo64.CPU.GPR[7] = 0xFFFFFFFFA4001F08;
                rcp.Nintendo64.CPU.GPR[8] = 0xC0;
                rcp.Nintendo64.CPU.GPR[10] = 0x40;
                rcp.Nintendo64.CPU.GPR[11] = 0xFFFFFFFFA4000040;
                rcp.Nintendo64.CPU.GPR[12] = 0xFFFFFFFFD19AE574;
                rcp.Nintendo64.CPU.GPR[13] = 0x4A459BAE;
                rcp.Nintendo64.CPU.GPR[14] = 0xFFFFFFFFE8EAD626;
                rcp.Nintendo64.CPU.GPR[15] = 0x6459969A;
                rcp.Nintendo64.CPU.GPR[20] = 0x1;
                rcp.Nintendo64.CPU.GPR[25] = 0x453CA37B;
                rcp.Nintendo64.CPU.GPR[29] = 0xFFFFFFFFA4001FF0;
                rcp.Nintendo64.CPU.GPR[31] = 0xFFFFFFFFA4001550;
                rcp.Nintendo64.CPU.HI = 0x6459969A;
                rcp.Nintendo64.CPU.LO = 0x6459969A;
                rcp.Nintendo64.CPU.PC = 0xFFFFFFFFA4000040;
            }
            #endregion
        }
    }
}
