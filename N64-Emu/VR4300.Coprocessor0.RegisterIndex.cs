namespace N64Emu
{
    public partial class VR4300
    {
        public partial class Coprocessor0
        {
            public enum RegisterIndex : byte
            {
                /// <summary>
                /// Programmable pointer into TLB array.
                /// </summary>
                Index = 0,
                /// <summary>
                /// Pseudorandom pointer into TLB array (read only).
                /// </summary>
                Random = 1,
                /// <summary>
                /// Low half of TLB entry for even virtual address (VPN).
                /// </summary>
                EntryLo0 = 2,
                /// <summary>
                /// Low half of TLB entry for odd virtual address (VPN).
                /// </summary>
                EntryLo1 = 3,
                /// <summary>
                /// Pointer to kernel virtual page table entry (PTE) in 32-bit mode.
                /// </summary>
                Context = 4,
                /// <summary>
                /// Page size specification.
                /// </summary>
                PageMask = 5,
                /// <summary>
                /// Number of wired TLB entries.
                /// </summary>
                Wired = 6,
                /// <summary>
                /// Display of virtual address that occurred an error last.
                /// </summary>
                BadVAddr = 8,
                /// <summary>
                /// Timer Count.
                /// </summary>
                Count = 9,
                /// <summary>
                /// High half of TLB entry (including ASID).
                /// </summary>
                EntryHi = 10,
                /// <summary>
                /// Timer Compare Value.
                /// </summary>
                Compare = 11,
                /// <summary>
                /// Operation status setting.
                /// </summary>
                Status = 12,
                /// <summary>
                /// Display of cause of last exception.
                /// </summary>
                Cause = 13,
                /// <summary>
                /// Exception Program Counter.
                /// </summary>
                EPC = 14,
                /// <summary>
                /// Processor Revision Identifier.
                /// </summary>
                PRId = 15,
                /// <summary>
                /// Memory system mode setting.
                /// </summary>
                Config = 16,
                /// <summary>
                /// Load Linked instruction address display.
                /// </summary>
                LLAddr = 17,
                /// <summary>
                /// Memory reference trap address low bits.
                /// </summary>
                WatchLo = 18,
                /// <summary>
                /// Memory reference trap address high bits.
                /// </summary>
                WatchHi = 19,
                /// <summary>
                /// Pointer to Kernel virtual PTE table in 64-bit mode.
                /// </summary>
                XContext = 20,
                /// <summary>
                /// Cache parity bits.
                /// </summary>
                ParityError = 26,
                /// <summary>
                /// Cache Error and Status register.
                /// </summary>
                CacheError = 27,
                /// <summary>
                /// Cache Tag register low.
                /// </summary>
                TagLo = 28,
                /// <summary>
                /// Cache Tag register high.
                /// </summary>
                TagHi = 29,
                /// <summary>
                /// Error Exception Program Counter.
                /// </summary>
                ErrorEPC = 30
            }
        }
    }
}
