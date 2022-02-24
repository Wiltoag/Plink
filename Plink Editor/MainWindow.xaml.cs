using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Plink_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NativeWindow win32Parent;

        public MainWindow()
        {
            InitializeComponent();
            win32Parent = new NativeWindow();
            win32Parent.AssignHandle(new WindowInteropHelper(this).Handle);
        }

        private void PickExecutableMenuItem_Click(object sender, RoutedEventArgs e)
        {
        }

        private void SetActionExecutableMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var source = (FrameworkElement)sender;
            var rule = (RuleViewModel)source.DataContext;
            var dialog = new OpenFileDialog
            {
                Title = "Open action executable",
                Multiselect = false,
                Filter = "Executables|*.exe|All files|*.*"
            };
            if (dialog.ShowDialog(win32Parent) == System.Windows.Forms.DialogResult.OK)
            {
                rule.TriggerName = Path.GetFileName(dialog.FileName);
                rule.TriggerDirectory = Path.GetDirectoryName(dialog.FileName) ?? throw new InvalidOperationException();
            }
        }

        private void SetExecutableMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var source = (FrameworkElement)sender;
            var rule = (RuleViewModel)source.DataContext;
            var dialog = new OpenFileDialog
            {
                Title = "Open process executable",
                Multiselect = false,
                Filter = "Executables|*.exe|All files|*.*"
            };
            if (dialog.ShowDialog(win32Parent) == System.Windows.Forms.DialogResult.OK)
            {
                rule.ProcessName = Path.GetFileName(dialog.FileName);
            }
        }
    }
}