using PreferenceManagerLibrary.Example.ViewModels;
using System.Windows;

namespace PreferenceManagerLibrary.Example.Views
{
    public partial class TreeConfigView : Window
    {
        private bool isClosing = false;
        public TreeConfigView(ConfigViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            Closing += (_, _) => isClosing = true;
            viewModel.OnCloseRequest += (_, _) => { if (!isClosing) Close(); };
        }
    }
}
