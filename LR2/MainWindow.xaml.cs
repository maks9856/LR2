using System.Diagnostics;
using System.Windows;

namespace LR2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadProcessPriorities();
            ProcessManager.success += MessageToList;
            ProcessManager.error += Error;
        }

        private void LoadProcessPriorities()
        {
            var process = new Process();
            comboBoxPrioretary.Items.Clear();

            foreach (var priority in process.GetProcessPriorities())
            {
                comboBoxPrioretary.Items.Add(priority);
            }
        }

        private void LoadProcesses()
        {
            comboBoxRemove.Items.Clear();
            comboBoxStoped.Items.Clear();
            comboBoxChangePrioretary.Items.Clear();
            comboBoxRestore.Items.Clear();
            foreach (var processId in ProcessManager.GetProcessIds())
            {
                comboBoxRemove.Items.Add(processId);
                comboBoxStoped.Items.Add(processId);
                comboBoxChangePrioretary.Items.Add(processId);
                comboBoxRestore.Items.Add(processId);
            }
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            if (NumberUpDown.Value is int NumberOfProcess)
            {
                var processManager = new ProcessManager();
                processManager.CreateProcesses(NumberOfProcess);
                LoadProcesses();
            }
        }

        private void ButtonKill_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxRemove.SelectedItem is int selectedProcessId)
            {
                var processManager = new ProcessManager();
                processManager.DeleteProcess(selectedProcessId);
                LoadProcesses();
            }
            else
            {
              
            }
        }

        private void ButtonKillAll_Click(object sender, RoutedEventArgs e)
        {
            var processManager = new ProcessManager();
            processManager.DeleteAllProcesses();
            LoadProcesses();
        }

        private void ButtonChangePrioretary_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxChangePrioretary.SelectedItem is int selectedProcessId &&
                comboBoxPrioretary.SelectedItem is ProcessPriorityClass newPriority)
            {
               var process=new ProcessManager();
               process.ChangeProcessPriority(selectedProcessId, newPriority);
            }
            else
            {
               
            }
        }

        private void ButtonStoped_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxStoped.SelectedItem is int selectedProcessId)
            {
                var process = new ProcessManager();
                process.SuspendProcess(selectedProcessId);
            }
        }

        private void ButtonStopedAll_Click(object sender, RoutedEventArgs e)
        {
            var process = new ProcessManager();
            process.SuspendAllProcess();
        }

        private void ButtonRestore_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxRestore.SelectedItem is int selectedProcessId)
            {
                var process = new ProcessManager();
                process.ResumeProcess(selectedProcessId);
            }
        }

        private void ButtonResroreAll_Click(object sender, RoutedEventArgs e)
        {
            var process = new ProcessManager();
            process.ResumeAllProcess();
        }

        public void MessageToList(string message)
        {
            listOfEvent.Items.Add(message);
        }
        public void Error(string message)
        {
            MessageBox.Show(message);
        }
    }
}
