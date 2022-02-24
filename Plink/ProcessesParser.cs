using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Plink
{
    internal class ProcessesParser
    {
        static ProcessesParser()
        {
            SettingsDir = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Plink"));
            SettingsFile = new FileInfo(Path.Combine(SettingsDir.FullName, "rules.json"));
        }

        public ProcessesParser()
        {
            RunningRules = new();
            Rules = new();
        }

        private static DirectoryInfo SettingsDir { get; }

        private static FileInfo SettingsFile { get; }

        private List<Rule> Rules { get; }

        private HashSet<Guid> RunningRules { get; }

        public void Load()
        {
            try
            {
                SettingsFile.Refresh();
                if (SettingsFile.Exists)
                {
                    var reader = new StreamReader(new FileStream(SettingsFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                    var rules = JsonSerializer.Deserialize<Rule[]>(reader.ReadToEnd());
                    reader.Close();
                    Rules.Clear();
                    if (rules is not null)
                        Rules.AddRange(rules);
                }
                else
                {
                    Rules.Clear();
                }
            }
            catch (Exception)
            {
            }
        }

        public void ParseProcesses()
        {
            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    var name = process.MainModule?.ModuleName;
                    if (name is not null)
                    {
                        var rules = Rules.Where(rule => rule.ProcessName == name);
                        foreach (var rule in rules)
                        {
                            if (!RunningRules.Contains(rule.Id))
                            {
                                var ruleAction = new Process
                                {
                                    StartInfo = new ProcessStartInfo(rule.ActionName, rule.ActionArguments)
                                    {
                                        UseShellExecute = true,
                                        WorkingDirectory = rule.ActionDirectory
                                    }
                                };
                                _ = process.WaitForExitAsync().ContinueWith(_ =>
                                {
                                    RunningRules.Remove(rule.Id);
                                    if (rule.AutoClose)
                                        try
                                        {
                                            ruleAction.Kill();
                                        }
                                        catch (Exception) { }
                                });
                                RunningRules.Add(rule.Id);
                                ruleAction.Start();
                            }
                        }
                    }
                }
                catch (Exception) { }
            }
        }

        public async Task Run()
        {
            Load();
            ParseProcesses();
            var clock = new System.Timers.Timer(1000)
            {
                AutoReset = true
            };
            clock.Elapsed += Clock_Elapsed;
            clock.Enabled = true;
            await Task.Delay(-1);
        }

        private void Clock_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            Load();
            ParseProcesses();
        }

        private class Rule
        {
            [JsonPropertyName("action-arguments")]
            public string ActionArguments { get; init; } = "";

            [JsonPropertyName("action-working-directory")]
            public string ActionDirectory { get; init; } = "";

            [JsonPropertyName("action-filename")]
            public string ActionName { get; init; } = "";

            [JsonPropertyName("auto-close")]
            public bool AutoClose { get; init; } = false;

            [JsonPropertyName("id")]
            public Guid Id { get; init; } = Guid.Empty;

            [JsonPropertyName("process-filename")]
            public string ProcessName { get; init; } = "";
        }
    }
}