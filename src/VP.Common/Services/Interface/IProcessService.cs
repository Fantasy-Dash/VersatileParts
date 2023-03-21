using System.Diagnostics;

namespace VP.Common.Services.Interface
{
    public interface IProcessService
    {//todo 注释

        public bool CommandLineContains(Process process, string filter);

        public string GetCommandLine(Process process);

        public List<Process> GetFiltedByCommandLine(string filter);
    }
}
