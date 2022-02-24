using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plink_Editor
{
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

        public RuleViewModel(Guid id, string processName, string triggerArguments, string triggerDirectory, string triggerPath, bool autoClose) : this()
        {
            Id = id;
            ProcessName = processName;
            TriggerArguments = triggerArguments;
            TriggerDirectory = triggerDirectory;
            TriggerName = triggerPath;
            AutoClose = autoClose;
        }

        [Reactive]
        public bool AutoClose { get; set; }

        public Guid Id { get; }

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