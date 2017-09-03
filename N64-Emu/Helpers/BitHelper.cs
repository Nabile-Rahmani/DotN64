using System.Runtime.CompilerServices;

namespace N64Emu.Helpers
{
    public static class BitHelper
    {
        #region Methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SignExtend(ushort value) => (uint)(short)value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SignExtend(uint value) => (ulong)(int)value;
        #endregion
    }
}
