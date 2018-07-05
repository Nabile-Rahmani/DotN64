using System;
using System.IO;
using System.Text;

namespace DotN64
{
    using Helpers;

    public partial class Cartridge
    {
        #region Fields
        public const ushort HeaderSize = 0x40, BootstrapSize = 0x1000 - HeaderSize;
        #endregion

        #region Properties
        public byte[] ROM { get; }

        public uint ClockRate => BitHelper.FromBigEndian(BitConverter.ToUInt32(ROM, 0x04));

        public uint BootAddressOffset => BitHelper.FromBigEndian(BitConverter.ToUInt32(ROM, 0x08));

        public uint ReleaseOffset => BitHelper.FromBigEndian(BitConverter.ToUInt32(ROM, 0x0C));

        public uint[] CRC => new[]
        {
            BitHelper.FromBigEndian(BitConverter.ToUInt32(ROM, 0x10)),
            BitHelper.FromBigEndian(BitConverter.ToUInt32(ROM, 0x14))
        };

        public string ImageName => Encoding.ASCII.GetString(ROM, 0x20, 0x34 - 0x20);

        public MediaFormat Format => (MediaFormat)BitHelper.FromBigEndian(BitConverter.ToUInt32(ROM, 0x38));

        public ushort ID => BitHelper.FromBigEndian(BitConverter.ToUInt16(ROM, 0x3C));

        public CountryCode Country => (CountryCode)ROM[0x3E];

        public byte Version => ROM[0x3F];
        #endregion

        #region Constructors
        public Cartridge(byte[] rom)
        {
            ROM = rom;
        }

        public Cartridge(FileInfo file)
            : this(File.ReadAllBytes(file.FullName)) { }
        #endregion
    }
}
