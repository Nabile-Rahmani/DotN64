namespace DotN64.PI
{
    public partial class PeripheralInterface
    {
        private enum CICStatus : byte
        {
            /// <summary>
            /// The boot ROM copies the cartridge's bootstrap code [0x40, 0x1000[ into the RSP's DMEM while computing the CIC algorithm on the data.
            /// When done, it stores the result in the PIF RAM at [48, 56[.
            /// </summary>
            Computing = 16,
            /// <summary>
            /// The CPU waits for the PIF CPU to validate that the cartridge's CIC computed the same result stored in the PIF RAM.
            /// </summary>
            Waiting = 48,
            /// <summary>
            /// The CIC check matched, continue booting.
            /// </summary>
            OK = 128
        }
    }
}
