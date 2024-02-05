using NetAuditSec;

class MainMenu
{
    private static NetworkScanner NetworkScanner = null;
    private static PortScanner PortScanner = null;
    private static DosSlowHttp DosSlowHttp = null;

    public static void Main()
    {
        int choice;

    MAIN_MENU:
        Console.Clear();
        Console.WriteLine("Select from the menu:");
        Console.WriteLine("[1] Network scanner");
        Console.WriteLine("[2] Port scanner");
        Console.WriteLine("[3] DoS - Slow HTTP");
        Console.WriteLine("[4] Exit");

        Console.Write("\nMenu> ");
        try
        {
            choice = Int32.Parse(Console.ReadLine());
        }
        catch
        {
            choice = 5;
        }

        switch (choice)
        {
            case 1:
                if (MainMenu.NetworkScanner == null)
                {
                    MainMenu.NetworkScanner = new NetworkScanner();
                    NetworkScanner.NetworkScannerMenu();
                    goto MAIN_MENU;
                }
                else
                {
                    MainMenu.NetworkScanner.NetworkScannerMenu();
                    goto MAIN_MENU;
                }

            case 2:
                if (MainMenu.PortScanner == null)
                {
                    MainMenu.PortScanner = new PortScanner();
                    PortScanner.PortScannerMenu();
                    goto MAIN_MENU;
                }
                else
                {
                    MainMenu.PortScanner.PortScannerMenu();
                    goto MAIN_MENU;
                }

            case 3:
                if (MainMenu.DosSlowHttp == null)
                {
                    MainMenu.DosSlowHttp = new DosSlowHttp();
                    DosSlowHttp.DosSlowHttpMenu();
                    goto MAIN_MENU;
                }
                else
                {
                    MainMenu.DosSlowHttp.DosSlowHttpMenu();
                    goto MAIN_MENU;
                }

            case 4:
                Console.WriteLine("Exit...");
                System.Environment.Exit(0);
                break;

            default:
                Console.WriteLine("Wrong option...");
                Thread.Sleep(500);
                goto MAIN_MENU;
        }
    }
}