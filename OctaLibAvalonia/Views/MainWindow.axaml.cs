using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace OctaLibAvalonia.Views;

public partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

}
