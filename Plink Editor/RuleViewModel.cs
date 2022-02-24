using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Plink_Editor
{
    [Serializable]
    internal record RuleModel
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; } = Guid.Empty;
        [JsonPropertyName("auto-close")]
        public bool AutoClose { get; init; } = false;
        [JsonPropertyName("process-filename")]
        public string ProcessName { get; init; } = "";
        [JsonPropertyName("action-arguments")]
        public string TriggerArguments { get; init; } = "";
        [JsonPropertyName("action-working-directory")]
        public string TriggerDirectory { get; init; } = "";
        [JsonPropertyName("action-filename")]
        public string TriggerName { get; init; } = "";
    }

    internal class RuleViewModel : ReactiveObject
    {
        public RuleViewModel()
        {
            Id = Guid.NewGuid();
            AutoClose = false;
            ProcessName = "";
            TriggerArguments = "";
            TriggerDirectory = "";
            TriggerName = "";
        }

        public RuleViewModel(RuleModel copy) : this()
        {
            Id = copy.Id;
            AutoClose = copy.AutoClose;
            ProcessName = copy.ProcessName;
            TriggerArguments = copy.TriggerArguments;
            TriggerDirectory = copy.TriggerDirectory;
            TriggerName = copy.TriggerName;
        }

        public RuleViewModel(string processName, string triggerArguments, string triggerDirectory, string triggerPath, bool autoClose) : this()
        {
            ProcessName = processName;
            TriggerArguments = triggerArguments;
            TriggerDirectory = triggerDirectory;
            TriggerName = triggerPath;
            AutoClose = autoClose;
        }

        [Reactive]
        public bool AutoClose { get; set; }

        public Guid Id { get; }

        public RuleModel Model => new()
        {
            Id = Id,
            AutoClose = AutoClose,
            ProcessName = ProcessName,
            TriggerArguments = TriggerArguments,
            TriggerDirectory = TriggerDirectory,
            TriggerName = TriggerName
        };

        [Reactive]
        public string ProcessName { get; set; }

        [Reactive]
        public string TriggerArguments { get; set; }

        [Reactive]
        public string TriggerDirectory { get; set; }

        [Reactive]
        public string TriggerName { get; set; }
    }
}