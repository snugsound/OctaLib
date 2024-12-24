using OctaLibAvalonia.Models;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows.Input;

namespace OctaLibAvalonia.ViewModels;

public class MainViewModel : ViewModelBase
{
    public Banks Banks {  get; set; }

    public static string? LoadedProject {  get; set; }

    public string CurrentVersion
    {
        get
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
    public ICommand BankSwapCommand { get; }

    public Interaction<BankSwapViewModel, BankSwapViewModel> ShowDialog { get; }

    public MainViewModel()
    {
        Banks = new Banks();

        ShowDialog = new Interaction<BankSwapViewModel, BankSwapViewModel>();

        BankSwapCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var model = new BankSwapViewModel();

            var result = await ShowDialog.Handle(model);
        });
    }

}
