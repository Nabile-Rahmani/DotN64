namespace N64Emu
{
    public partial class Nintendo64
    {
        public struct MappingEntry
        {
            #region Properties
            public uint StartAddress { get; set; }

            public uint EndAddress { get; set; }

            public Name EntryName { get; set; }
            #endregion

            #region Enumerations
            public enum Name
            {
                None,
                PIFBootROM
            }
            #endregion
        }
    }
}
