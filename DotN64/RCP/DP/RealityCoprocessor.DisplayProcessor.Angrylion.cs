using System.Runtime.InteropServices;

namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public partial class DisplayProcessor
        {
            public static class Angrylion
            {
                #region Fields
                private const string LibraryName = "n64video.dll";
                #endregion

                #region Methods
                [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
                public static extern void angrylion_rdp_init();

                [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
                public static extern unsafe uint rdp_process_list(uint* dp_start, uint* dp_current, uint* dp_end, uint* dp_status, uint* rsp_dmem, uint* rdram, out uint crashed);
                #endregion
            }
        }
    }
}
