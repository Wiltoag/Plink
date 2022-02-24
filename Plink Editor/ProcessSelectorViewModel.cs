using DynamicData;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Plink_Editor
{
    internal class ProcessSelectorViewModel : ReactiveObject
    {
        private ReadOnlyObservableCollection<ProcessViewModel> consoleProcesses;
        private ReadOnlyObservableCollection<ProcessViewModel> windowProcesses;

        public ProcessSelectorViewModel()
        {
            ProcessesSource = new SourceCache<ProcessViewModel, int>(p => p.GetHashCode());
            var connect = ProcessesSource.Connect();
            var comparer = Comparer<ProcessViewModel>.Create((left, right) => left.PreferredName.CompareTo(right.PreferredName));
            connect
                .Filter(p => p.WindowTitle is not null)
                .Sort(comparer)
                .Bind(out windowProcesses)
                .Subscribe();
            connect
                .Filter(p => p.WindowTitle is null)
                .Sort(comparer)
                .Bind(out consoleProcesses)
                .Subscribe();

            ProcessesSource.AddOrUpdate(Process.GetProcesses()
                .Select(p =>
                {
                    try
                    {
                        return new ProcessViewModel(p);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                })
                .OfType<ProcessViewModel>());
        }

        public ReadOnlyObservableCollection<ProcessViewModel> ConsoleProcesses => consoleProcesses;
        public ReadOnlyObservableCollection<ProcessViewModel> WindowProcesses => windowProcesses;
        private SourceCache<ProcessViewModel, int> ProcessesSource { get; }
    }

    internal class ProcessViewModel : ReactiveObject, IEquatable<ProcessViewModel?>
    {
        private static Dictionary<string, BitmapSource> iconsCache;

        static ProcessViewModel()
        {
            iconsCache = new();
        }

        public ProcessViewModel(Process source)
        {
            Reference = source;
            Name = source.MainModule?.ModuleName ?? throw new InvalidOperationException();
            WindowTitle = source.MainWindowHandle == IntPtr.Zero ? null : string.IsNullOrEmpty(source.MainWindowTitle) ? null : source.MainWindowTitle;
            var productName = source.MainModule?.FileVersionInfo.ProductName;
            if (!string.IsNullOrEmpty(productName))
            {
                PreferredName = productName;
            }
            else if (!string.IsNullOrEmpty(WindowTitle))
            {
                PreferredName = WindowTitle;
            }
            else
            {
                PreferredName = source.ProcessName;
            }
            Icon = null;
            var exeFile = source.MainModule?.FileName;
            if (exeFile is not null)
            {
                if (iconsCache.ContainsKey(exeFile))
                    Icon = iconsCache[exeFile];
                else
                    try
                    {
                        var icon = System.Drawing.Icon.ExtractAssociatedIcon(exeFile);
                        if (icon is not null)
                        {
                            var bitmap = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(16, 16));
                            Icon = bitmap;
                            iconsCache.Add(exeFile, bitmap);
                        }
                    }
                    catch (Exception) { }
            }
        }

        public ImageSource? Icon { get; }
        public string Name { get; }
        public string PreferredName { get; }
        public Process Reference { get; }
        public string? WindowTitle { get; }

        public static bool operator !=(ProcessViewModel? left, ProcessViewModel? right)
        {
            return !(left == right);
        }

        public static bool operator ==(ProcessViewModel? left, ProcessViewModel? right)
        {
            return EqualityComparer<ProcessViewModel>.Default.Equals(left, right);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ProcessViewModel);
        }

        public bool Equals(ProcessViewModel? other)
        {
            return other != null &&
                   Name == other.Name &&
                   WindowTitle == other.WindowTitle;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, WindowTitle);
        }
    }
}