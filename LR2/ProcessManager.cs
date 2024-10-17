using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace LR2
{
    public static class ProcessManager
    {
        public  delegate void  Message(string message);
        public static event Message? error;
        public static event Message? success;
        private static readonly List<Process> _processes = new List<Process>();
        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);

        public static void CreateProcesses(this Process processTemplate, int numberOfProcesses)
        {
            for (int i = 0; i < numberOfProcesses; i++)
            {
                try
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "C:\\Users\\Macsh\\source\\repos\\ConsoleApp5\\ConsoleApp5\\bin\\Debug\\ConsoleApp5.exe",
                            Arguments = $"{-0.99} {0.99} {100000}",
                            UseShellExecute = true,
                            CreateNoWindow = true
                        }
                    };
                    process.Start();
                   
                    _processes.Add(process);
                }
                catch (Exception ex)
                {
                    error?.Invoke($"Помилка:{ex}");
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
                error?.Invoke($"Помилка: не знайдено ID:{processId}   {ex}");
            }
            catch (Exception ex)
            {
                error?.Invoke($"Помилка:{ex}");
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
                    error?.Invoke($"Помилка:{ex}");
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
                success?.Invoke($"Успішно поміняний пріорітет у{processId} на{newPriority}");
            }
            catch (ArgumentException)
            {
               error?.Invoke($"Процес з {processId} не знайдено");
            }
            catch (Exception ex)
            {
                error?.Invoke($"Помилка:{ex}");
            }
        }

        public static void SuspendProcess(this Process processTemplate, int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                foreach (ProcessThread thread in process.Threads)
                {
                    IntPtr hThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                    if (hThread != IntPtr.Zero)
                    {
                        SuspendThread(hThread);
                    }
                }
                success?.Invoke($"Процес {processId} призупинено.");
            }
            catch (Exception ex)
            {
                error?.Invoke($"Помилка при призупиненні процесу {processId}: {ex}");
            }
        }

        public static void ResumeProcess(this Process processTemplate, int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                foreach (ProcessThread thread in process.Threads)
                {
                    IntPtr hThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                    if (hThread != IntPtr.Zero)
                    {
                        ResumeThread(hThread);
                    }
                }
                success?.Invoke($"Процес {processId} відновлено.");
            }
            catch (Exception ex)
            {
                error?.Invoke($"Помилка при відновленні процесу {processId}: {ex}");
            }
        }
    }
}
