using OctaLib;

internal class Program
{
    private const int ARG_PATH = 0;
    private const int ARG_COMMAND = 1;
    private const int ARG_1 = 2;
    private const int ARG_2 = 3;

    private static void IntegrityCheck(string path)
    {
        if (!File.Exists(path + "\\project.strd"))
        {
            Console.WriteLine("This does not appear to be a valid Octatrack project directory");
            return;
        }

        if (ProjectUtils.GetVersion(path) != 19)
        {
            Console.WriteLine("Unrecognized project version");
            return;
        }
    }

    private static void Info(string path)
    {
        string[] partnames = new string[4];


        for (int bankNum = 1; bankNum < 17; bankNum++)
        {

            var bankNumStr = bankNum.ToString("00");
            var b = File.ReadAllBytes(path + $"\\bank{bankNumStr}.strd");

            if (!BankUtils.ValidateHeader(b))
            {
                Console.WriteLine($"Bank {bankNum + 1} appears to be invalid");
                return;
            }

            //Console.WriteLine($"bank{bankNumStr} has {b.Length} bytes");

            string[] partNames = new string[4];
            for (int p = 0; p < 4; p++)
            {
                partNames[p] = BankUtils.ReadPartName(b, p).PadRight(6);
            }

            Console.WriteLine($"bank{bankNumStr} part names: {partNames[0]} {partNames[1]} {partNames[2]} {partNames[3]}");

            Console.Write("Pattern states: ");
            for (int pat = 0; pat < 16; pat++)
            {
                Console.Write(BankUtils.ReadPatternActiveState(b, pat) ? "1" : "0");
            }
            Console.WriteLine();
        }
    }

    private static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: OctaLib <project folder> <command> <params>");
            Console.WriteLine("");
            Console.WriteLine("Commands:");
            Console.WriteLine("info         Print info about the project");
            Console.WriteLine("swap-banks   Swap banks");
            return;
        }

        IntegrityCheck(args[ARG_PATH]);

        switch (args[ARG_COMMAND].ToLower())
        {
            case "info":
                Info(args[ARG_PATH]);
                break;

            case "swap-banks":
                if (args.Length < 4)
                {
                    Console.WriteLine("swap-banks requires 2 additional arguments (bank # 1 and 2)");
                    return;
                }
                var b1 = Int32.Parse(args[ARG_1]);
                var b2 = Int32.Parse(args[ARG_2]);
                if (b1 < 1 || b1 > 16)
                {
                    Console.WriteLine("Invalid bank number (expected: 1-16)");
                    return;
                }
                if (b2 < 1 || b2 > 16)
                {
                    Console.WriteLine("Invalid bank number (expected: 1-16)");
                    return;
                }
                if (b1 == b2)
                {
                    Console.WriteLine("Bank numbers must be different");
                    return;
                }
                BankUtils.SwapBanks(args[ARG_PATH], b1, b2);
                break;
        }
    }
}