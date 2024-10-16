using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LR2
{
    public static class ProcessManager
    {
        private static readonly List<Process> _processes = new List<Process>();

        public static void CreateProcesses(this Process processTemplate, int numberOfProcesses)
        {
            if (numberOfProcesses <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfProcesses), "The number of processes must be greater than zero.");
            }

            for (int i = 0; i < numberOfProcesses; i++)
            {
                try
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "C:\\Users\\Macsh\\source\\repos\\ConsoleApp5\\ConsoleApp5\\bin\\Debug\\ConsoleApp5.exe",
                            Arguments = $"{0} {0.5} {10}",
                            UseShellExecute = true,
                            CreateNoWindow = true
                        }
                    };
                    process.Start();
                    _processes.Add(process);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error starting process: {ex.Message}");
                }
            }
        }

        public static IEnumerable<int> GetProcessIds(this Process processTemplate)
        {
            return _processes.Select(p => p.Id);
        }

        public static void DeleteProcess(this Process processTemplate, int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                process.Kill();
                _processes.RemoveAll(p => p.Id == processId);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Process with ID {processId} not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete process: {ex.Message}");
            }
        }

        public static void DeleteAllProcesses(this Process processTemplate)
        {
            var processesToTerminate = new List<Process>(_processes);
            foreach (var process in processesToTerminate)
            {
                try
                {
                    process.Kill();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error terminating process ID {process.Id}: {ex.Message}");
                }
            }

            _processes.Clear();
        }

        public static IEnumerable<ProcessPriorityClass> GetProcessPriorities(this Process processTemplate)
        {
            return Enum.GetValues(typeof(ProcessPriorityClass)).Cast<ProcessPriorityClass>();
        }

        public static void ChangeProcessPriority(this Process processTemplate, int processId, ProcessPriorityClass newPriority)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                process.PriorityClass = newPriority;
                Console.WriteLine($"Successfully changed priority of process ID {processId} to {newPriority}.");
            }
            catch (ArgumentException)
            {
                Console.WriteLine($"Process with ID {processId} not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error changing process priority: {ex.Message}");
            }
        }
    }
}
