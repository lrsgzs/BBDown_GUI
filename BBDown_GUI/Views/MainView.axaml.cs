using Avalonia.Controls;
using Avalonia.Interactivity;
using BBDown_GUI.ViewModels;

namespace BBDown_GUI.Views;

public partial class MainView : UserControl
{
    public MainViewModel ViewModel { get; } = new();
    
    public MainView()
    {
        DataContext = this;
        InitializeComponent();
    }
    
    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        DataContext = null;
    }
}