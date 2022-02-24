using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plink_Editor
{
    internal class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel()
        {
            Rules = new ObservableCollection<RuleViewModel>();
            Rules.Add(new RuleViewModel(Guid.NewGuid(), "aa.exe", "arg", "dir", "path", true));
        }

        public ObservableCollection<RuleViewModel> Rules { get; }
    }
}