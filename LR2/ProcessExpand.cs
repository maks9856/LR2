using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR2
{
    public static class ProcessExpand
    {
        public static IEnumerable<Process> StartMultipleProcesses(this Process processTemplate, ProcessStartInfo processStartInfo, int numberOfProcesses)
        {
            List<Process> processList = new List<Process>();
            for (int i = 0; i < numberOfProcesses; i++)
            {
                Process process = new Process();
                process.StartInfo = processStartInfo;
                processList.Add(process);
                process.Start();
                
            }
            return processList;
        }
        public static IEnumerable<ProcessPriorityClass> GetProcessPriorities(this Process processTemplate)
        {
            return Enum.GetValues(typeof(ProcessPriorityClass)).Cast<ProcessPriorityClass>();
        }
    }
}
