using DynamicData;
using Microsoft.Win32;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Plink_Editor
{
    internal class MainWindowViewModel : ReactiveObject
    {
        static MainWindowViewModel()
        {
            SettingsDir = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Plink"));
            SettingsFile = new FileInfo(Path.Combine(SettingsDir.FullName, "rules.json"));
            RegKeyStartup = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true) ?? throw new InvalidOperationException();
            EngineFileName = "Plink.exe";
            RegValueNameStartup = "Plink";
        }

        public MainWindowViewModel()
        {
            Rules = new ObservableCollection<RuleViewModel>();
            Load();
            RunOnStartup = RegKeyStartup.GetValue(RegValueNameStartup) is not null;
            this
                .WhenAnyValue(o => o.RunOnStartup)
                .Subscribe(value =>
                {
                    if (value)
                    {
                        RegKeyStartup.SetValue(RegValueNameStartup, Path.Combine(Environment.CurrentDirectory, EngineFileName), RegistryValueKind.String);
                    }
                    else
                    {
                        try
                        {
                            RegKeyStartup.DeleteValue(RegValueNameStartup, false);
                        }
                        catch (Exception) { }
                    }
                });
        }

        public ObservableCollection<RuleViewModel> Rules { get; }

        [Reactive]
        public bool RunOnStartup { get; set; }

        private static string EngineFileName { get; }
        private static RegistryKey RegKeyStartup { get; }

        private static string RegValueNameStartup { get; }
        private static DirectoryInfo SettingsDir { get; }
        private static FileInfo SettingsFile { get; }

        public void Load()
        {
            SettingsFile.Refresh();
            if (SettingsFile.Exists)
            {
                Rules.Clear();
                var rules = JsonSerializer.Deserialize<RuleModel[]>(File.ReadAllText(SettingsFile.FullName));
                if (rules is not null)
                    Rules.AddRange(rules.Select(r => new RuleViewModel(r)));
            }
        }

        public void Save()
        {
            SettingsDir.Create();
            File.WriteAllText(SettingsFile.FullName, JsonSerializer.Serialize(Rules.Select(r => r.Model), new JsonSerializerOptions
            {
                WriteIndented = true
            }));
        }
    }
}