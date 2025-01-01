using System.Collections.ObjectModel;

namespace OctaLibAvalonia.Models;

public class Bank
{
    public int Number { get; set; }
    public Patterns? Patterns {  get; set; }
}

public class Banks : ObservableCollection<Bank>
{
    public Banks()
    {

        // This just populates some dummy banks for the initial view
        for (int i = 1; i < 17; i++)
        {
            var b = new Bank();
            b.Number = i;
            b.Patterns = new Patterns();
            Add(b);
        }
    }
}
