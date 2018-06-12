using System;

namespace DotN64.Desktop
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyReleaseStreamAttribute : Attribute
    {
        #region Properties
        public string Stream { get; set; }
        #endregion

        #region Constructors
        public AssemblyReleaseStreamAttribute(string stream)
        {
            Stream = stream;
        }
        #endregion
    }
}
