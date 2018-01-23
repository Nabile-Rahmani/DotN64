using System.Net;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(byte[] data, int index, uint value)
        {
            fixed (byte* pointer = &data[index])
            {
                *(uint*)pointer = value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort FromBigEndian(ushort value) => (ushort)IPAddress.NetworkToHostOrder((short)value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint FromBigEndian(uint value) => (uint)IPAddress.NetworkToHostOrder((int)value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ToBigEndian(ushort value) => (ushort)IPAddress.HostToNetworkOrder((short)value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ToBigEndian(uint value) => (uint)IPAddress.HostToNetworkOrder((int)value);
        #endregion
    }
}
