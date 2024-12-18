using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Code
{
    public interface IExampleCircuits
    {
        string[] GetExamples();
        Task<CircuitFiles> GetExampleAsync(string exampleName);
    }
}
