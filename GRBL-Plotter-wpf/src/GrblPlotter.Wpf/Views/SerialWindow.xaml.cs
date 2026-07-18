using System.Windows;
using System.Windows.Input;
using GrblPlotter.Wpf.ViewModels;

namespace GrblPlotter.Wpf.Views;

public partial class SerialWindow : Window
{
    public SerialWindow()
    {
        InitializeComponent();
    }

    private MainViewModel? Vm => DataContext as MainViewModel;

    private void Clear_Click(object sender, RoutedEventArgs e) => Vm?.LogLines.Clear();

    private void Send_Click(object sender, RoutedEventArgs e) => SendCmd();

    private void CmdBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            SendCmd();
            e.Handled = true;
        }
    }

    private void SendCmd()
    {
        if (Vm == null) return;
        var text = CmdBox.Text;
        if (string.IsNullOrWhiteSpace(text)) return;
        if (Vm.SendManualCommand.CanExecute(text))
            Vm.SendManualCommand.Execute(text);
        CmdBox.Clear();
    }
}
