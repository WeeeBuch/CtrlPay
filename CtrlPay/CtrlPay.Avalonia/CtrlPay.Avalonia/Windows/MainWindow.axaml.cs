using Avalonia.Controls;
using CtrlPay.Avalonia.ViewModels;

namespace CtrlPay.Avalonia.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        internal void ChangeView(ViewModelBase view)
        {
            this.Content = view;
        }
    }
}