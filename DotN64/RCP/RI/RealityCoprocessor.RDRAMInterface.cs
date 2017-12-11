using System;

namespace DotN64.RCP
{
    using Helpers;

    public partial class RealityCoprocessor
    {
        public partial class RDRAMInterface : Interface
        {
            #region Properties
            public byte Select { get; set; }

            public ConfigRegister Config { get; set; }

            public ModeRegister Mode { get; set; }

            public RefreshRegister Refresh { get; set; }

            public RDRAMConfigRegister[] RDRAMConfigs { get; } = new RDRAMConfigRegister[Enum.GetNames(typeof(RDRAMConfigIndex)).Length];
            #endregion

            #region Constructors
            public RDRAMInterface(RealityCoprocessor rcp)
                : base(rcp)
            {
                MemoryMaps = new[]
                {
                    new MappingEntry(0x0470000C, 0x0470000F) // RI select.
                    {
                        Read = o => Select,
                        Write = (o, v) => Select = (byte)(v & (1 << 3) - 1)
                    },
                    new MappingEntry(0x04700004, 0x04700007) // RI config.
                    {
                        Write = (o, v) => Config = v
                    },
                    new MappingEntry(0x04700008, 0x0470000B) // RI current load.
                    {
                        Write = (o, v) => { /* TODO: Any write updates current control register. */ }
                    },
                    new MappingEntry(0x04700000, 0x04700003) // RI mode.
                    {
                        Write = (o, v) => Mode = v
                    },
                    new MappingEntry(0x04700010, 0x04700013) // RI refresh.
                    {
                        Read = a => Refresh,
                        Write = (a, v) => Refresh = v
                    },
                    new MappingEntry(0x00000000, 0x03EFFFFF) // RDRAM memory.
                    {
                        Read = o => BitConverter.ToUInt32(rcp.Nintendo64.RAM, (int)o),
                        Write = (o, v) => BitHelper.Write(rcp.Nintendo64.RAM, (int)o, v)
                    },
                    new MappingEntry(0x03F00000, 0x03FFFFFF) // RDRAM registers.
                    {
                        Read = o =>
                        {
                            if (!GetRDRAMRegisterInfo((uint)o, out var register, out var index))
                                return 0;

                            unsafe
                            {
                                fixed (void* pointer = &RDRAMConfigs[index.Value])
                                {
                                    return *((uint*)pointer + register / sizeof(uint));
                                }
                            }
                        },
                        Write = (o, v) =>
                        {
                            if (!GetRDRAMRegisterInfo((uint)o, out var register, out var index))
                                return;

                            unsafe
                            {
                                fixed (void* pointer = &RDRAMConfigs[index.Value])
                                {
                                    *((uint*)pointer + register / sizeof(uint)) = v;
                                }
                            }
                        }
                    }
                };
            }
            #endregion

            #region Methods
            private bool GetRDRAMRegisterInfo(uint offset, out uint register, out int? index)
            {
                register = offset & ((1 << 8) - 1);

                switch (offset >> 8)
                {
                    case 0x0000:
                        index = (int)RDRAMConfigIndex.Zero;
                        break;
                    case 0x0004:
                        index = (int)RDRAMConfigIndex.One;
                        break;
                    case 0x0800:
                        index = (int)RDRAMConfigIndex.Global;
                        break;
                    default:
                        index = null;
                        return false;
                }

                return true;
            }
            #endregion
        }
    }
}
