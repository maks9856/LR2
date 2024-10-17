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

            var processManager = new Process(); 
            foreach (var processId in processManager.GetProcessIds())
            {
                comboBoxRemove.Items.Add(processId);
                comboBoxStoped.Items.Add(processId);
                comboBoxChangePrioretary.Items.Add(processId);
            }
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            var process = new Process();
            process.CreateProcesses((int)NumberUpDown.Value);
            LoadProcesses();
        }

        private void ButtonKill_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxRemove.SelectedItem is int selectedProcessId)
            {
                var process = new Process();
                process.DeleteProcess(selectedProcessId);
                LoadProcesses();
            }
            else
            {
              
            }
        }

        private void ButtonKillAll_Click(object sender, RoutedEventArgs e)
        {
            var process = new Process();
            process.DeleteAllProcesses();
            LoadProcesses();
        }

        private void ButtonChangePrioretary_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxChangePrioretary.SelectedItem is int selectedProcessId &&
                comboBoxPrioretary.SelectedItem is ProcessPriorityClass newPriority)
            {
               var process=new Process();
               process.ChangeProcessPriority(selectedProcessId, newPriority);
            }
            else
            {
               
            }
        }

        private void ButtonStoped_Click(object sender, RoutedEventArgs e)
        {
            
            Process process = new Process();
            process.CompleteProcess((int)comboBoxStoped.SelectedItem);
        }
    }
}
