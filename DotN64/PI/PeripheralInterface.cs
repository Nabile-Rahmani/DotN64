using System;
using System.Collections.Generic;
using System.Net;

namespace DotN64.PI
{
    using Extensions;

    public partial class PeripheralInterface
    {
        #region Fields
        private readonly IReadOnlyList<MappingEntry> memoryMaps;

        private const byte CICStatusOffset = 60;
        private const byte ResetControllerStatus = 1 << 0, ClearInterruptStatus = 1 << 1;
        #endregion

        #region Properties
        public StatusRegister Status { get; set; }

        public byte[] BootROM { get; set; }

        public byte[] RAM { get; } = new byte[64];

        public Domain[] Domains { get; } = new[]
        {
            new Domain(),
            new Domain()
        };
        #endregion

        #region Constructors
        public PeripheralInterface()
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
                }
            };
        }
        #endregion

        #region Methods
        private void ClearInterrupt() { /* TODO: Implement. */ }

        private void ResetController() { /* TODO: Implement. */ }

        public uint ReadWord(ulong address) => memoryMaps.GetEntry(address).ReadWord(address);

        public void WriteWord(ulong address, uint value) => memoryMaps.GetEntry(address).WriteWord(address, value);
        #endregion
    }
}
