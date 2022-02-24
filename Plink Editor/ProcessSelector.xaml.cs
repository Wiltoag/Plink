using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Plink_Editor
{
    /// <summary>
    /// Interaction logic for ProcessSelector.xaml
    /// </summary>
    public partial class ProcessSelector : Window
    {
        public ProcessSelector()
        {
            InitializeComponent();
        }

        public string? Result { get; private set; }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var source = (FrameworkElement)sender;
            var process = (ProcessViewModel)source.DataContext;
            Result = process.Name;
            DialogResult = true;
        }

        private void OpenLocationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var source = (FrameworkElement)sender;
            var process = (ProcessViewModel)source.DataContext;
            try
            {
                new Process
                {
                    StartInfo = new ProcessStartInfo("explorer.exe", $"/select,\"{process.Reference.MainModule?.FileName}\"")
                    {
                        UseShellExecute = true
                    }
                }.Start();
            }
            catch (Exception) { }
        }
    }
}