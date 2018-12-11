namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public partial class MIPSInterface
        {
            [System.Flags]
            public enum InterruptPins : byte
            {
                /// <summary>RCP interrupt asserted.</summary>
                RCP = 1 << 0,
                /// <summary>A peripherial has generated an interrupt.</summary>
                Cartridge = 1 << 1,
                /// <summary>User has pushed reset button on console.</summary>
                PreNMI = 1 << 2,
                /// <summary>Indy has read the value in the RDB port.</summary>
                RDBRead = 1 << 3,
                /// <summary>Indy has written a value to the RDB port.</summary>
                RDBWrite = 1 << 4
            }
        }
    }
}
