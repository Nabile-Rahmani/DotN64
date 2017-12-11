using System.Collections.Generic;

namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public abstract class Interface
        {
            #region Fields
            protected readonly RealityCoprocessor rcp;
            #endregion

            #region Properties
            public IReadOnlyList<MappingEntry> MemoryMaps { get; protected set; }
            #endregion

            #region Constructors
            protected Interface(RealityCoprocessor rcp)
            {
                this.rcp = rcp;
            }
            #endregion
        }
    }
}
