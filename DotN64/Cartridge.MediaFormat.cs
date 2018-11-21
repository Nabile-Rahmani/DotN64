namespace DotN64
{
    public partial class Cartridge
    {
        public enum MediaFormat : byte
        {
            ExpandableCartridge = (byte)'C',
            Disk = (byte)'D',
            ExpansionDisk = (byte)'E',
            Cartridge = (byte)'N'
        }
    }
}
