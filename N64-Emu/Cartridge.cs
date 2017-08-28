using System.IO;

namespace N64Emu
{
    public class Cartridge
    {
        #region Properties
        public byte[] Data { get; set; }
        #endregion

        #region Methods
        public static Cartridge FromFile(FileInfo file)
        {
            using (var reader = new BinaryReader(file.OpenRead()))
            {
                return new Cartridge
                {
                    Data = reader.ReadBytes((int)reader.BaseStream.Length)
                };
            }
        }
        #endregion
    }
}
