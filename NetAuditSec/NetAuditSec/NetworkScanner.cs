using System.Net.NetworkInformation;
using System.Net;
using Microsoft.AspNetCore.HttpOverrides;

namespace NetAuditSec
{
    internal class NetworkScanner
    {
        private readonly string Name = "Network scanner";
        private readonly string[] Tables = { "network_scans", "active_devices" };
        private readonly string ColumnName = "ip";
        private readonly string ScanFor = "Active devices";
        protected DB Db = null;

        public NetworkScanner() { }

        public void NetworkScannerMenu()
        {
            int choice;

        NETWORK_SCANNER_MENU:
            Console.Clear();
            Console.WriteLine($"{this.Name}:");
            Console.WriteLine("[1] Scan the network");
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
                    this.ScanTheNetwork();
                    goto NETWORK_SCANNER_MENU;

                case 2:
                    if (this.Db == null) this.Db = new DB();
                    this.Db.GetMySqlConnection();
                    Functions.SeePreviousScans(this.Db, this.Name, this.Tables, this.ColumnName, this.ScanFor);
                    goto NETWORK_SCANNER_MENU;

                case 3:
                    return;

                default:
                    Console.WriteLine("Wrong option...");
                    Thread.Sleep(500);
                    goto NETWORK_SCANNER_MENU;
            }
        }

        private void ScanTheNetwork()
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            int choice;

        GET_NETWORK_INTERFACE:
            List<Array> interfaces = GetNetworkInterfaces(networkInterfaces);

            Console.WriteLine($"[{interfaces.Count + 1}] Back");

            Console.Write($"\n{this.Name}> ");
            try
            {
                choice = Int32.Parse(Console.ReadLine());
            }
            catch
            {
                choice = interfaces.Count + 2;
            }

            if (choice == interfaces.Count + 1)
            {
                return;
            }
            else if (choice <= 0 || choice > interfaces.Count + 1)
            {
                Console.WriteLine("Wrong option...");
                Thread.Sleep(500);
                goto GET_NETWORK_INTERFACE;
            }
            else
            {
                // 0 - ip address, 1 - netmask, 2 - netmask prefix
                string[] selected_interface = (string[])interfaces[choice - 1];
                int netmask_prefix_int = Int32.Parse(selected_interface[2]);

                if (netmask_prefix_int != 24)
                {
                    Console.WriteLine("Only for netmask /24...");
                    Thread.Sleep(1500);
                    goto GET_NETWORK_INTERFACE;
                }

                string network_address = CalculateNetworkAddress(selected_interface[0], selected_interface[1]);
                IPAddress network_address_parsed = IPAddress.Parse(network_address);
                IPNetwork network_to_scan = new IPNetwork(network_address_parsed, netmask_prefix_int);

                string[] network_address_array_string_octets = network_address.Split(".");

                int[] network_address_array_int_octets = {
                    Int32.Parse(network_address_array_string_octets[0]),
                    Int32.Parse(network_address_array_string_octets[1]),
                    Int32.Parse(network_address_array_string_octets[2]),
                    Int32.Parse(network_address_array_string_octets[3])
                };

                if (network_address_array_int_octets[3] == 0) network_address_array_int_octets[3]++;

                string address_to_check;
                List<string> active_devices = new List<string>();

                while (network_address_array_int_octets[3] < 255)
                {
                    address_to_check = network_address_array_string_octets[0] + "." + network_address_array_string_octets[1] + "." + network_address_array_string_octets[2] + "." + network_address_array_int_octets[3].ToString();

                    if (network_to_scan.Contains(IPAddress.Parse(address_to_check)))
                    {
                        if (Net.SendPing(address_to_check))
                        {
                            Console.WriteLine(address_to_check + " *** [Host is up]");
                            active_devices.Add(address_to_check);
                        }
                        else
                        {
                            Console.WriteLine(address_to_check);
                        }
                    }
                    network_address_array_int_octets[3]++;
                }

                Console.Write("\nNetwork scanning ended...\nPress Enter to continue...");
                Console.ReadLine();
                Console.Clear();
                Console.WriteLine("Active devices:");
                foreach (string d in active_devices) { Console.WriteLine(d); }

                if(!active_devices.Any())
                {
                    Console.Write("\nThere is no active devices... \nPress Enter to continue...");
                    Console.ReadLine();
                    return;
                }

                string save;
            SAVE_SCAN:
                Console.WriteLine("\nSave this scan? y/n");
                Console.Write($"\n{this.Name}> ");
                save = Console.ReadLine();

                if (save == "y")
                {
                    if(this.Db == null) this.Db = new DB();

                    this.Db.GetMySqlConnection();
                    this.Db.InsertScan(active_devices, this.Tables);
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

        private List<Array> GetNetworkInterfaces(NetworkInterface[] networkInterfaces)
        {
            string local_ip_address = "";
            string netmask = "";
            string netmask_prefix = "";

            List<Array> interfaces = new List<Array>();

            Console.Clear();
            Console.WriteLine("Select network interface:");

            for (int i = 0; i < networkInterfaces.Length; i++)
            {
                if (networkInterfaces[i].NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                    networkInterfaces[i].OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }

                Console.Write($"[{i + 1}] {networkInterfaces[i].Name}: ");

                IPInterfaceProperties ipProperties = networkInterfaces[i].GetIPProperties();
                UnicastIPAddressInformationCollection unicastAddresses = ipProperties.UnicastAddresses;

                foreach (UnicastIPAddressInformation unicastAddress in unicastAddresses)
                {
                    if (unicastAddress.Address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork ||
                        unicastAddress.Address.IsIPv6LinkLocal)
                    {
                        continue;
                    }

                    local_ip_address = unicastAddress.Address.ToString();
                    netmask = unicastAddress.IPv4Mask.ToString();
                    netmask_prefix = unicastAddress.PrefixLength.ToString();
                }

                Console.WriteLine(local_ip_address);
                string[] data = { local_ip_address, netmask, netmask_prefix };
                interfaces.Add(data);
            }

            return interfaces;
        }

        private string CalculateNetworkAddress(string ipAddress, string subnetMask)
        {
            IPAddress ip = IPAddress.Parse(ipAddress);
            IPAddress mask = IPAddress.Parse(subnetMask);

            byte[] ipBytes = ip.GetAddressBytes();
            byte[] maskBytes = mask.GetAddressBytes();

            byte[] networkBytes = new byte[ipBytes.Length];

            for (int i = 0; i < ipBytes.Length; i++)
            {
                networkBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
            }

            return new IPAddress(networkBytes).ToString();
        }
    }
}
