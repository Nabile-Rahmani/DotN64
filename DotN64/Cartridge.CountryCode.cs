namespace DotN64
{
    public partial class Cartridge
    {
        /// <summary>
        /// See: https://archive.org/13/items/SNESDevManual/book1.pdf #1-2-20.
        /// </summary>
        public enum CountryCode : byte
        {
            Common = (byte)'A',
            Brazil = (byte)'B',
            China = (byte)'C',
            Germany = (byte)'D',
            NorthAmerica = (byte)'E',
            France = (byte)'F',
            Netherlands = (byte)'H',
            Italy = (byte)'I',
            Japan = (byte)'J',
            Korea = (byte)'K',
            Canada = (byte)'N',
            Europe = (byte)'P',
            Spain = (byte)'S',
            Australia = (byte)'U',
            Scandinavia = (byte)'W',
        }
    }
}
