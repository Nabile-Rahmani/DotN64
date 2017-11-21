using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DotN64.Extensions
{
    public static class MappingEntryExtensions
    {
        #region Methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MappingEntry GetEntry(this IReadOnlyList<MappingEntry> memoryMaps, ulong address)
        {
            for (int i = 0; i < memoryMaps.Count; i++)
            {
                var entry = memoryMaps[i];

                if (entry.Contains(address))
                    return entry;
            }

            throw new Exception($"Unknown physical address: 0x{address:X16}.");
        }
        #endregion
    }
}
