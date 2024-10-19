using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace LR2
{
    public class ProcessManager
    {
        public delegate void Message(string message);
        public static event Message? error;
        public static event Message? success;

        private static readonly List<Process> _processes = new List<Process>();
        private static readonly Dictionary<int, bool> _processStatus = new Dictionary<int, bool>();

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

        public void CreateProcesses(int numberOfProcesses)
        {
            try
            {
                Process process = new Process();
                var StartInfo = new ProcessStartInfo
                {
                    FileName = "C:\\Users\\Macsh\\source\\repos\\ConsoleApp5\\ConsoleApp5\\bin\\Debug\\ConsoleApp5.exe",
                    Arguments = $"{-0.99} {0.99} {100000}",
                    CreateNoWindow = false,
                    UseShellExecute = false,
                };

                foreach (var pr in process.StartMultipleProcesses(StartInfo, numberOfProcesses))
                {
                    pr.EnableRaisingEvents = true;
                    pr.Exited += (sender, e) =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            success?.Invoke($"Процес з ID {pr.Id} завершився.");
                        });
                        _processes.Remove(pr);
                        _processStatus.Remove(pr.Id);
                    };
                    _processes.Add(pr);
                    _processStatus[pr.Id] = true; 

                    success?.Invoke($"Створено процес з ID {pr.Id}");
                }
            }
            catch (Exception ex)
            {
                error?.Invoke($"Помилка: {ex}");
            }
        }

        public static IEnumerable<int> GetProcessIds()
        {
            return _processes.Select(p => p.Id);
        }

        public void DeleteProcess(int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                process.Kill();
                _processes.RemoveAll(p => p.Id == processId);
                _processStatus.Remove(processId); 
                success?.Invoke($"Вбито процес з ID {processId}");
            }
            catch (ArgumentException ex)
            {
                error?.Invoke($"Помилка: не знайдено ID:{processId}   {ex}");
            }
            catch (Exception ex)
            {
                error?.Invoke($"Помилка: {ex}");
            }
        }

        public void DeleteAllProcesses()
        {
            var processesToDelete = new List<Process>(_processes);
            foreach (var process in processesToDelete)
            {
                try
                {
                    process.Kill();
                    _processStatus.Remove(process.Id); 
                    success?.Invoke($"Вбито процес з ID {process.Id}");
                }
                catch (Exception ex)
                {
                    error?.Invoke($"Помилка: {ex}");
                }
            }

            _processes.Clear();
        }

        public void ChangeProcessPriority(int processId, ProcessPriorityClass newPriority)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                process.PriorityClass = newPriority;
                success?.Invoke($"Успішно поміняний пріорітет у проекеса з ID {processId} на {newPriority}");
            }
            catch (ArgumentException)
            {
                error?.Invoke($"Процес з {processId} не знайдено");
            }
            catch (Exception ex)
            {
                error?.Invoke($"Помилка: {ex}");
            }
        }

        public void SuspendProcess(int processId)
        {
            try
            {
                if (_processStatus.ContainsKey(processId) && _processStatus[processId] == false)
                {
                    success?.Invoke($"Процес з ID {processId} вже був зупинений.");
                    return;
                }

                var process = Process.GetProcessById(processId);
                foreach (ProcessThread thread in process.Threads)
                {
                    IntPtr hThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                    if (hThread != IntPtr.Zero)
                    {
                        SuspendThread(hThread);
                    }
                }
                _processStatus[processId] = false; 
                success?.Invoke($"Процес з ID {processId} призупинено.");
            }
            catch (Exception ex)
            {
                error?.Invoke($"Помилка при призупиненні процесу {processId}: {ex}");
            }
        }

        public void SuspendAllProcess()
        {
            var processes = new List<Process>(_processes);
            try
            {
                foreach (var process in processes)
                {
                    if (_processStatus[process.Id] == false)
                    {
                        success?.Invoke($"Процес з ID {process.Id} вже був зупинений.");
                        continue;
                    }

                    foreach (ProcessThread thread in process.Threads)
                    {
                        IntPtr hThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                        if (hThread != IntPtr.Zero)
                        {
                            SuspendThread(hThread);
                        }
                    }
                    _processStatus[process.Id] = false; 
                    success?.Invoke($"Процес з ID {process.Id} призупинено.");
                }
            }
            catch (Exception ex)
            {
                error?.Invoke($"Помилка: {ex}");
            }
        }

        public void ResumeProcess(int processId)
        {
            try
            {
                if (_processStatus.ContainsKey(processId) && _processStatus[processId] == true)
                {
                    success?.Invoke($"Процес з ID {processId} вже був запущений.");
                    return;
                }

                var process = Process.GetProcessById(processId);
                foreach (ProcessThread thread in process.Threads)
                {
                    IntPtr hThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                    if (hThread != IntPtr.Zero)
                    {
                        ResumeThread(hThread);
                    }
                }
                _processStatus[processId] = true; 
                success?.Invoke($"Процес {processId} відновлено.");
            }
            catch (Exception ex)
            {
                error?.Invoke($"Помилка при відновленні процесу {processId}: {ex}");
            }
        }

        public void ResumeAllProcess()
        {
            try
            {
                foreach (var process in _processes)
                {
                    if (_processStatus[process.Id] == true)
                    {
                        success?.Invoke($"Процес з ID {process.Id} вже був запущений.");
                        continue;
                    }

                    foreach (ProcessThread thread in process.Threads)
                    {
                        IntPtr hThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                        if (hThread != IntPtr.Zero)
                        {
                            ResumeThread(hThread);
                        }
                    }
                    _processStatus[process.Id] = true;
                    success?.Invoke($"Процес {process.Id} відновлено.");
                }
            }
            catch (Exception ex)
            {
                error?.Invoke($"Помилка: {ex}");
            }
        }

      
        public Dictionary<int, bool> GetProcessStatus()
        {
            return new Dictionary<int, bool>(_processStatus);
        }
    }
}
