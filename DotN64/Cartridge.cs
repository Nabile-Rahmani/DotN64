using System;
using System.IO;
using System.Net;
using System.Text;

namespace DotN64
{
    public class Cartridge
    {
        #region Properties
        public byte[] ROM { get; set; }

        public uint ClockRate => (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(ROM, 0x04));

        public uint BootAddressOffset => (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(ROM, 0x08));

        public uint ReleaseOffset => (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(ROM, 0x0C));

        public uint[] CRC => new[]
        {
            (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(ROM, 0x10)),
            (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(ROM, 0x14))
        };

        public string ImageName => Encoding.ASCII.GetString(ROM, 0x20, 0x33 - 0x20);

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
