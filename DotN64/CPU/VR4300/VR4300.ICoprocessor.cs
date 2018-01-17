namespace DotN64.CPU
{
    public partial class VR4300
    {
        public interface ICoprocessor
        {
            #region Methods
            void Run(Instruction instruction);
            #endregion
        }
    }
}
