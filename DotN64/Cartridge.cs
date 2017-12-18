using System;
using System.IO;
using System.Net;
using System.Text;

namespace DotN64
{
    using Helpers;

    public class Cartridge
    {
        #region Properties
        public byte[] ROM { get; set; }

        public uint ClockRate => BitHelper.FromBigEndian(BitConverter.ToUInt32(ROM, 0x04));

        public uint BootAddressOffset => BitHelper.FromBigEndian(BitConverter.ToUInt32(ROM, 0x08));

        public uint ReleaseOffset => BitHelper.FromBigEndian(BitConverter.ToUInt32(ROM, 0x0C));

        public uint[] CRC => new[]
        {
            BitHelper.FromBigEndian(BitConverter.ToUInt32(ROM, 0x10)),
            BitHelper.FromBigEndian(BitConverter.ToUInt32(ROM, 0x14))
        };

        public string ImageName => Encoding.ASCII.GetString(ROM, 0x20, 0x34 - 0x20);

        public byte ManufacturerID => ROM[0x3B];

        public ushort ID => (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(ROM, 0x3C));

        public byte CountryCode => ROM[0x3E];
        #endregion

        #region Methods
        public static Cartridge FromFile(FileInfo file)
        {
            using (var reader = new BinaryReader(file.OpenRead()))
            {
                return new Cartridge
                {
                    ROM = reader.ReadBytes((int)reader.BaseStream.Length)
                };
            }
        }
        #endregion
    }
}
