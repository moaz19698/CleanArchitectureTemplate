namespace CleanArchitecture.Application.Common.Exceptions
{
    public class NotFoundAssmblyException : Exception
    {
        #region Constructor
        public NotFoundAssmblyException(string assemblyName)
          : base($"Not found {{ {nameof(Application)} }} assembly or namespace in current app domain")
        {
        }
        #endregion
    }
}
