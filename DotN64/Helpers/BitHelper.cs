using System.Runtime.CompilerServices;

namespace DotN64.Helpers
{
    public static class BitHelper
    {
        #region Methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Get(uint data, int shift, uint size) => data >> shift & size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Get(ulong data, int shift, ulong size) => data >> shift & size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set(ref uint data, int shift, uint size, uint value)
        {
            data &= ~(size << shift);
            data |= (value & size) << shift;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set(ref ulong data, int shift, ulong size, ulong value)
        {
            data &= ~(size << shift);
            data |= (value & size) << shift;
        }

        public static unsafe void Write(byte[] data, int index, uint value)
        {
            fixed (byte* pointer = &data[index])
            {
                *(uint*)pointer = value;
            }
        }
        #endregion
    }
}
