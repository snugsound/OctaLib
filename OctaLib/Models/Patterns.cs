using Avalonia.Media.Imaging;
using BogaNet.Helper;
using System;
using System.Collections.ObjectModel;

namespace OctaLibAvalonia.Models;

public class Pattern
{
    public int Number { get; set; }
    public int PartNumber { get; set; }
    public string PartName { get; set; }

    public bool Status { get; set; }

    public string CompositePart
    {
        get
        {
            return $"{PartNumber}: {PartName}";
        }
    }

    // FIXME: Not very clean
    public Bitmap? ImageSrc
    {
        get
        {
            if (Number % 4 == 1)
            {
                if (Status)
                {
                    return ImageHelper.LoadFromResource("avares://OctaLib/Assets/ot-button-illum-outline.png");
                }
                else
                {
                    return ImageHelper.LoadFromResource("avares://OctaLib/Assets/ot-button-outline.png");
                }

            }
            else
            {
                if (Status)
                {
                    return ImageHelper.LoadFromResource("avares://OctaLib/Assets/ot-button-illum.png");
                }
                else
                {
                    return ImageHelper.LoadFromResource("avares://OctaLib/Assets/ot-button.png");
                }

            }
        }
    }
}

public class Patterns : ObservableCollection<Pattern>
{
    public Patterns()
    {

        // This just populates some dummy patterns for the initial view
        for (int i = 1; i < 17; i++)
        {
            var p = new Pattern();
            p.Number = i;
            p.PartNumber = 1;
            p.PartName = "ONE";
            p.Status = false;
            Add(p);
        }
    }

}
