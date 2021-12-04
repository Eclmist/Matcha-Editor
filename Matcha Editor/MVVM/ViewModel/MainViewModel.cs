using System.Collections.ObjectModel;
using Matcha_Editor.MVVM.Model;
using Matcha_Editor.MVVM.ViewModel.Commands;

namespace Matcha_Editor.MVVM.ViewModel
{
    class MainViewModel
    {
        public ImportAssetCommand ImportAssetCommand { get; private set; }
        public ConnectToEngineCommand ConnectToEngineCommand { get; private set; }

        public MainViewModel()
        {
            ImportAssetCommand = new ImportAssetCommand();
            ConnectToEngineCommand = new ConnectToEngineCommand();
        }
    }
}
