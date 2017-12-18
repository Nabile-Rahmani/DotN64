using System;
using System.Collections.Generic;

namespace DotN64
{
    using Helpers;

    public partial class RDRAM
    {
        #region Properties
        public IReadOnlyList<MappingEntry> MemoryMaps { get; }

        public byte[] Memory { get; }

        public ConfigRegister[] Configs { get; } = new ConfigRegister[Enum.GetNames(typeof(ConfigIndex)).Length];
        #endregion

        #region Constructors
        public RDRAM(byte[] memory)
        {
            Memory = memory;
            MemoryMaps = new[]
            {
                new MappingEntry(0x00000000, 0x03EFFFFF) // RDRAM memory.
                {
                    Read = o => BitConverter.ToUInt32(Memory, (int)o),
                    Write = (o, v) => BitHelper.Write(Memory, (int)o, v)
                },
                new MappingEntry(0x03F00000, 0x03FFFFFF) // RDRAM registers.
                {
                    Read = o =>
                    {
                        if (!GetRegisterInfo((uint)o, out var register, out var index))
                            return 0;

                        unsafe
                        {
                            fixed (void* pointer = &Configs[index.Value])
                            {
                                return *((uint*)pointer + register);
                            }
                        }
                    },
                    Write = (o, v) =>
                    {
                        if (!GetRegisterInfo((uint)o, out var register, out var index))
                            return;

                        unsafe
                        {
                            fixed (void* pointer = &Configs[index.Value])
                            {
                                *((uint*)pointer + register) = v;
                            }
                        }
                    }
                }
            };
        }
        #endregion

        #region Methods
        private bool GetRegisterInfo(uint offset, out uint register, out int? index)
        {
            register = (offset & ((1 << 8) - 1)) / sizeof(uint);

            switch (offset >> 8)
            {
                case 0x0000:
                    index = (int)ConfigIndex.Zero;
                    break;
                case 0x0004:
                    index = (int)ConfigIndex.One;
                    break;
                case 0x0800:
                    index = (int)ConfigIndex.Global;
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
