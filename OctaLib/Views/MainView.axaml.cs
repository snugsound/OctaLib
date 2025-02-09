using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using DialogHostAvalonia;
using OctaLib;
using OctaLibAvalonia.Models;
using OctaLibAvalonia.ViewModels;
using ReactiveUI;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace OctaLibAvalonia.Views;

public partial class MainView : ReactiveUserControl<MainViewModel>
{

    public MainView()
    {
        InitializeComponent();

        this.WhenActivated(action =>
            action(ViewModel!.ShowDialog.RegisterHandler(DoShowDialogAsync)));

        MainViewModel.OnReload += ReloadProject;
    }

    // This code is only valid in newer ReactiveUI which is shipped since avalonia 11.2.0 
    private async Task DoShowDialogAsync(IInteractionContext<BankSwapViewModel, BankSwapViewModel> interaction)
    {
        var dialog = new BankSwapWindow();
        dialog.DataContext = interaction.Input;

        var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;
        var result = await dialog.ShowDialog<BankSwapViewModel>(mainWindow);
        interaction.SetOutput(result);
    }

    private async void OnOpenProject(object sender, RoutedEventArgs e)
    {
        var options = new FolderPickerOpenOptions();
        options.Title = "Select project folder";

        var topLevel = TopLevel.GetTopLevel(this);
         var result = await topLevel.StorageProvider.OpenFolderPickerAsync(options);

        if (result == null || result.Count == 0)
        {
            return;
        }
        Trace.WriteLine($"Selected folder {result[0].Path.LocalPath}");

        int version = -1;
        var folder = result[0].Path.LocalPath;
        try
        {
            version = ProjectUtils.GetVersion(folder);
        }
        catch (FileNotFoundException)
        {

        }

        if (version != 19)
        {
            await DialogHost.Show(topLevel.Resources["ProjectErrorDialog"], "MainDialogHost");
            return;
        }

        TopText.Text = $"Loaded project: {folder}";
        LoadProject(folder);

        MainViewModel.LoadedProject = folder;
        SwapBanksMenuItem.IsEnabled = true;
    }

    private void LoadProject(string folder)
    {
        var banks = new Banks();

        for (int bankNum = 1; bankNum < 17; bankNum++)
        {
            var patterns = new Patterns();
            patterns.Clear();

            var bankNumStr = bankNum.ToString("00");
            var b = File.ReadAllBytes(Path.Combine(folder, $"bank{bankNumStr}.strd"));

            var partNames = new string[4];
            for (int partNum = 0; partNum < 4; partNum++)
            {
                partNames[partNum] = BankUtils.ReadPartName(b, partNum);
            }

            for (int patternNum = 0; patternNum < 16; patternNum++)
            {
                var p = new Pattern();
                p.Status = BankUtils.ReadPatternActiveState(b, patternNum);
                p.Number = patternNum + 1;
                p.PartNumber = BankUtils.ReadPatternPart(b, patternNum) + 1;
                p.PartName = partNames[p.PartNumber - 1];
                patterns.Add(p);
            }

            banks[bankNum - 1].Patterns = patterns;

        }

        BankItems.ItemsSource = banks;
    }

    private async void OnAbout(object sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        await DialogHost.Show(topLevel.Resources["AboutDialog"], "MainDialogHost");
    }

    public void ReloadProject()
    {
        LoadProject(MainViewModel.LoadedProject);
    }

}
