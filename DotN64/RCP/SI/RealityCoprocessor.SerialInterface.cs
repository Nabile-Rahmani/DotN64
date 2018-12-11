using System;

namespace DotN64.RCP
{
    using Helpers;

    public partial class RealityCoprocessor
    {
        public partial class SerialInterface : Interface
        {
            #region Properties
            private PeripheralInterface PIF => rcp.nintendo64.PIF;

            public Statuses Status { get; set; }
            #endregion

            #region Constructors
            public SerialInterface(RealityCoprocessor rcp)
                : base(rcp)
            {
                MemoryMaps = new[]
                {
                    new MappingEntry(0x04800018, 0x0480001B) // SI status.
                    {
                        Read = o => (uint)Status,
                        Write = (o, d) =>
                        {
                            Status &= ~Statuses.Interrupt;
                            rcp.MI.Interrupt &= ~MIPSInterface.Interrupts.SI;
                        }
                    },
                    new MappingEntry(0x1FC00000, 0x1FC007BF) // PIF Boot ROM.
                    {
                        Read = o => BitHelper.FromBigEndian(BitConverter.ToUInt32(PIF.BootROM, (int)o))
                    },
                    new MappingEntry(0x1FC007C0, 0x1FC007FF) // PIF (JoyChannel) RAM.
                    {
                        Read = o => BitConverter.ToUInt32(PIF.RAM, (int)o),
                        Write = (o, d) =>
                        {
                            BitHelper.Write(PIF.RAM, (int)o, d);
                            PIF.OnRAMWritten((int)o);
                            Interrupt();
                        }
                    }
                };
            }
            #endregion

            #region Methods
            private void Interrupt()
            {
                Status |= Statuses.Interrupt;
                rcp.MI.Interrupt |= MIPSInterface.Interrupts.SI;
            }
            #endregion
        }
    }
}
