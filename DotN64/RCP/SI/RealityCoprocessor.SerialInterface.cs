namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public partial class SerialInterface : Interface
        {
            #region Properties
            public Statuses Status { get; set; }
            #endregion

            #region Constructors
            public SerialInterface(RealityCoprocessor rcp)
                : base(rcp)
            {
                MemoryMaps = new[]
                {
                    new MappingEntry(0x04800018, 0x0480001B) // SI status.
                    {
                        Read = o => (uint)Status,
                        Write = (o, v) => Status &= ~Statuses.Interrupt
                    }
                };
            }
            #endregion
        }
    }
}
