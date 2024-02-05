namespace NetAuditSec
{
    internal class PortScanner : NetworkScanner
    {
        private readonly string Name = "Port scanner";
        private readonly string[] Tables = { "port_scans", "open_ports" };
        private readonly string ColumnName = "port";
        private readonly string ScanFor = "Open ports on";
        private string IpOfTarget;
        private int PortToScan;
        private string RangeOfPortsToScan;
        private string[] RangeOfPortsToScan_arr;
        private int FirstPort;
        private int LastPort;
        private int[] PopularPorts = { 20, 21, 22, 23, 25, 53, 67, 68, 69, 70, 79, 80, 110, 119, 143, 161, 220, 389, 443, 514, 636, 873, 995, 3306, 3389, 5432, 5222, 6000, 6001, 6002, 6003, 6004, 6005, 6006, 6007, 6661, 6662, 6663, 6664, 6665, 6666, 6667, 6668, 8080, 25565 };
        private List<string> OpenPorts = new List<string>();
        public PortScanner() { }

        public void PortScannerMenu()
        {
            int choice;

        PORT_SCANNER_MENU:
            Console.Clear();
            Console.WriteLine($"{this.Name}:");
            Console.WriteLine("[1] Scan");
            Console.WriteLine("[2] See previous scans");
            Console.WriteLine("[3] Back to menu");

            Console.Write($"\n{this.Name}> ");
            try
            {
                choice = Int32.Parse(Console.ReadLine());
            }
            catch
            {
                choice = 4;
            }

            switch (choice)
            {
                case 1:
                    this.Scan();
                    goto PORT_SCANNER_MENU;

                case 2:
                    if (this.Db == null) this.Db = new DB();
                    this.Db.GetMySqlConnection();
                    Functions.SeePreviousScans(this.Db, this.Name, this.Tables, this.ColumnName, this.ScanFor, true);
                    goto PORT_SCANNER_MENU;

                case 3:
                    return;

                default:
                    Console.WriteLine("Wrong option...");
                    Thread.Sleep(500);
                    goto PORT_SCANNER_MENU;
            }
        }

        private void Scan()
        {
            int choice;

        SCAN:
            this.IpOfTarget = Functions.GetIpAddressOfTarget(this.Name);

        SCAN_PORTS:
            Console.Clear();
            Console.WriteLine($"IP of target: {this.IpOfTarget}");
            Console.WriteLine("[1] Scan using one port");
            Console.WriteLine("[2] Scan using range of ports");
            Console.WriteLine("[3] Scan using set of popular ports (45 ports)");
            Console.WriteLine("[4] Back to enter IP address of target");
            Console.WriteLine($"[5] Back to {this.Name} menu");

            Console.Write($"\n{this.Name}> ");
            try
            {
                choice = Int32.Parse(Console.ReadLine());
            }
            catch
            {
                choice = 6;
            }

            switch (choice)
            {
                case 1:
                    this.GetPortToScan();
                    this.ScanOnePort();
                    break;

                case 2:
                    this.GetRangeOfPortsToScan();
                    this.ScanRangeOfPorts();
                    break;

                case 3:
                    this.ScanPopularPorts();
                    break;

                case 4:
                    goto SCAN;

                case 5:
                    return;

                default:
                    Console.WriteLine("Wrong option...");
                    Thread.Sleep(500);
                    goto SCAN_PORTS;
            }
        }

        private void GetPortToScan()
        {
        GET_PORT_TO_SCAN:
            Console.Clear();
            Console.WriteLine($"Enter port number to scan on {this.IpOfTarget}");
            Console.Write($"{this.Name}> ");
            try
            {
                this.PortToScan = Int32.Parse(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Incorrect port number...");
                Thread.Sleep(1500);
                goto GET_PORT_TO_SCAN;
            }
        }

        private void ScanOnePort()
        {
            this.OpenPorts.Clear();
            if (Net.IsPortOpen(this.IpOfTarget, this.PortToScan))
            {
                Console.WriteLine($"{this.IpOfTarget}:{this.PortToScan} is open ***");

                this.OpenPorts.Add(this.PortToScan.ToString());

                this.SavePortScan();
            }
            else
            {
                Console.WriteLine($"{this.IpOfTarget}:{this.PortToScan} is closed");
                Console.Write("\nPress Enter to continue...");
                Console.ReadLine();
            }
        }

        private void GetRangeOfPortsToScan()
        {
        GET_RANGE_OF_PORTS_TO_SCAN:
            Console.Clear();
            Console.WriteLine($"Enter range of ports numbers to scan on {this.IpOfTarget}");
            Console.WriteLine($"In format <port>-<port>, e.g. 1-5");
            Console.Write($"{this.Name}> ");
  
            this.RangeOfPortsToScan = Console.ReadLine();

            try
            {
                this.RangeOfPortsToScan_arr = this.RangeOfPortsToScan.Split("-");

                this.FirstPort = Int32.Parse(this.RangeOfPortsToScan_arr[0]);
                this.LastPort = Int32.Parse(this.RangeOfPortsToScan_arr[1]);

                if (this.FirstPort >= this.LastPort || this.FirstPort == 0 || this.LastPort == 0) throw new Exception();
            }
            catch
            {
                Console.WriteLine("Incorrect format of ports numbers...");
                Thread.Sleep(1500);
                goto GET_RANGE_OF_PORTS_TO_SCAN;
            }
        }

        private void ScanRangeOfPorts()
        {
            this.OpenPorts.Clear();
            for (int p = this.FirstPort; p <= this.LastPort; p++)
            {
                if (Net.IsPortOpen(this.IpOfTarget, p))
                {
                    Console.WriteLine($"{this.IpOfTarget}:{p} is open ***");

                    this.OpenPorts.Add(p.ToString());
                }
                else
                {
                    Console.WriteLine($"{this.IpOfTarget}:{p} is closed");
                }
            }

            Console.Write("\nPort scanning ended...\nPress Enter to continue...");
            Console.ReadLine();
            Console.Clear();

            if (!this.OpenPorts.Any())
            {
                Console.Write($"There is no open ports on {this.IpOfTarget} from given range({this.FirstPort}-{this.LastPort})... \nPress Enter to continue...");
                Console.ReadLine();
                return;
            }
            else
            {
                Console.WriteLine($"Open ports on {this.IpOfTarget} from given range({this.FirstPort}-{this.LastPort}):");
                foreach (string p in this.OpenPorts) { Console.WriteLine(p); }

                this.SavePortScan();
            }
        }

        private void ScanPopularPorts()
        {
            this.OpenPorts.Clear();
            foreach (int p in this.PopularPorts)
            {
                if (Net.IsPortOpen(this.IpOfTarget, p))
                {
                    Console.WriteLine($"{this.IpOfTarget}:{p} is open ***");

                    this.OpenPorts.Add(p.ToString());
                }
                else
                {
                    Console.WriteLine($"{this.IpOfTarget}:{p} is closed");
                }
            }

            Console.Write("\nPort scanning ended...\nPress Enter to continue...");
            Console.ReadLine();
            Console.Clear();

            if (!this.OpenPorts.Any())
            {
                Console.Write($"There is no open popular ports on {this.IpOfTarget}... \nPress Enter to continue...");
                Console.ReadLine();
                return;
            }
            else
            {
                Console.WriteLine($"Open popular ports on {this.IpOfTarget}:");
                foreach (string p in this.OpenPorts) { Console.WriteLine(p); }

                this.SavePortScan();
            }
        }

        private void SavePortScan()
        {
            string save;

        SAVE_SCAN:
            Console.WriteLine("\nSave this scan? y/n");
            Console.Write($"\n{this.Name}> ");
            save = Console.ReadLine();

            if (save == "y")
            {
                if (this.Db == null) this.Db = new DB();

                this.Db.GetMySqlConnection();
                this.Db.InsertScan(this.OpenPorts, this.Tables, this.IpOfTarget);
                this.Db.CloseMySqlConnection();

                Console.Write("\nScan saved successfully. \nPress Enter to continue...");
                Console.ReadLine();
            }
            else if (save == "n")
            {
                return;
            }
            else
            {
                goto SAVE_SCAN;
            }
        }
    }
}
