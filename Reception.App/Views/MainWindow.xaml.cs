using Avalonia;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using Reception.App.ViewModels;

namespace Reception.App.Views
{
    public class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}