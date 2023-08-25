using PreferenceManagerLibrary.Example.ViewModels;
using System.Windows;

namespace PreferenceManagerLibrary.Example.Views
{
    public partial class TabConfigView : Window
    {
        private bool isClosing = false;
        public TabConfigView(ConfigViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            Closing += (_, _) => isClosing = true;
            viewModel.OnCloseRequest += (_, _) => { if (!isClosing) Close(); };
        }
    }
}
