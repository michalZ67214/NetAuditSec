using System.Text.RegularExpressions;

namespace NetAuditSec
{
    internal class Functions
    {
        public static void SeePreviousScans(DB db_handler, string tool_name, string[] table_names, string column_name, string purpose_of_scan, bool display_ip = false)
        {
            List<string> dates = new List<string>();
            List<string[]> scans = new List<string[]>();
            List<string> elements = new List<string>();
            int choice;
            string selected_date;
            string selected_date_format_mysql;
            string id_of_scan;
            string time_of_scan;
            string choice_of_delete;

        SELECT_DATE:
            dates.Clear();
            dates = db_handler.GetDatesOfScans(table_names[0]);

            if (!dates.Any())
            {
                Console.Clear();
                Console.WriteLine("There is no scans in database...");
                db_handler.CloseMySqlConnection();
                Thread.Sleep(1500);
                return;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Select date:");
                for (int i = 0; i < dates.Count; i++)
                {
                    Console.WriteLine($"[{i + 1}] {dates[i]}");
                }
                Console.WriteLine($"[{dates.Count + 1}] Back");

                Console.Write($"\n{tool_name}> ");
                try
                {
                    choice = Int32.Parse(Console.ReadLine());
                }
                catch
                {
                    choice = dates.Count + 2;
                }

                if (choice == dates.Count + 1)
                {
                    db_handler.CloseMySqlConnection();
                    return;
                }
                else if (choice <= 0 || choice > dates.Count + 1)
                {
                    Console.WriteLine("Wrong option...");
                    Thread.Sleep(500);
                    goto SELECT_DATE;
                }
                else
                {
                    selected_date = dates[choice - 1];
                    selected_date_format_mysql = DateTime.Parse(selected_date).ToString("yyyy-MM-dd");

                SELECT_TIME:
                    scans.Clear();

                    // 0 - id, 1 - datetime, 2 - ip(only for port scan)
                    scans = db_handler.GetScansOfDay(table_names[0], selected_date_format_mysql, display_ip);

                    if (!scans.Any())
                    {
                        Console.Clear();
                        Console.WriteLine($"There is no scans from {selected_date}...");
                        Thread.Sleep(1500);
                        goto SELECT_DATE;
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine($"Select time from {selected_date}");
                        for (int i = 0; i < scans.Count; i++)
                        {
                            Console.WriteLine($"[{i + 1}] {scans[i][1]}{((scans[i][2] == null) ? "" : $" - {scans[i][2]}")}");
                        }
                        Console.WriteLine($"[{scans.Count + 1}] Delete all scans from {selected_date}");
                        Console.WriteLine($"[{scans.Count + 2}] Back");

                        Console.Write($"\n{tool_name}> ");
                        try
                        {
                            choice = Int32.Parse(Console.ReadLine());
                        }
                        catch
                        {
                            choice = scans.Count + 3;
                        }

                        if (choice == scans.Count + 2)
                        {
                            goto SELECT_DATE;
                        }
                        else if (choice == scans.Count + 1)
                        {
                        DELETE_SCANS:
                            Console.WriteLine($"\nConfirm deletion of all scans from {selected_date}? y/n");
                            Console.Write($"\n{tool_name}> ");
                            choice_of_delete = Console.ReadLine();
                            if (choice_of_delete == "y")
                            {
                                db_handler.DeleteAllScansFromDay(table_names[0], selected_date_format_mysql);
                                Console.Write($"\nAll scans from {selected_date} deleted successfully. \nPress Enter to continue...");
                                Console.ReadLine();
                                goto SELECT_DATE;
                            }
                            else if (choice_of_delete == "n")
                            {
                                goto SELECT_TIME;
                            }
                            else
                            {
                                goto DELETE_SCANS;
                            }
                        }
                        else if (choice <= 0 || choice > scans.Count + 1)
                        {
                            Console.WriteLine("Wrong option...");
                            Thread.Sleep(500);
                            goto SELECT_TIME;
                        }
                        else
                        {
                            id_of_scan = scans[choice - 1][0];
                            time_of_scan = scans[choice - 1][1];

                        DISPLAY_ELEMENTS:
                            elements.Clear();
                            elements = db_handler.GetElementsFromScan(column_name, table_names[1], id_of_scan);

                            if (!elements.Any())
                            {
                                Console.WriteLine($"Entries in database not found at {time_of_scan}...");
                                Thread.Sleep(1500);
                                goto SELECT_TIME;
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine($"{purpose_of_scan}{((scans[choice - 1][2] == null) ? "" : $" {scans[choice - 1][2]}")} at {time_of_scan}:");
                                foreach (string el in elements)
                                {
                                    Console.WriteLine(el);
                                }

                                Console.WriteLine("\nSelect from the menu:");
                                Console.WriteLine("[1] Delete this scan from database");
                                Console.WriteLine("[2] Back");
                                Console.WriteLine($"[3] Back to {tool_name} menu");
                                Console.Write($"\n{tool_name}> ");
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
                                    DELETE_SCAN:
                                        Console.WriteLine($"\nConfirm deletion of scan from {time_of_scan}? y/n");
                                        Console.Write($"\n{tool_name}> ");
                                        choice_of_delete = Console.ReadLine();
                                        if (choice_of_delete == "y")
                                        {
                                            db_handler.DeleteScanById(table_names[0], id_of_scan);
                                            Console.Write($"\nScan from {time_of_scan} deleted successfully. \nPress Enter to continue...");
                                            Console.ReadLine();
                                            goto SELECT_TIME;
                                        }
                                        else if (choice_of_delete == "n")
                                        {
                                            goto DISPLAY_ELEMENTS;
                                        }
                                        else
                                        {
                                            goto DELETE_SCAN;
                                        }
                                    case 2:
                                        goto SELECT_TIME;
                                    case 3:
                                        db_handler.CloseMySqlConnection();
                                        return;
                                    default:
                                        Console.WriteLine("Wrong option...");
                                        Thread.Sleep(500);
                                        goto DISPLAY_ELEMENTS;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static string GetIpAddressOfTarget(string tool_name)
        {
            string ip;
            Match ip_validation;
            string[] ip_validation_octets;
            bool is_ip_octet_valid;

        GET_IP_ADDRESS_OF_TARGET:
            Console.Clear();
            Console.Write("Enter IP address of target");
            Console.Write($"\n{tool_name}> ");
            ip = Console.ReadLine();

            ip_validation = Regex.Match(ip, @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$");

            if (ip_validation.Success)
            {
                ip_validation_octets = ip.Split(".");
                is_ip_octet_valid = true;
                foreach (string octet in ip_validation_octets)
                {
                    if (Int32.Parse(octet) > 255) is_ip_octet_valid = false;
                }

                if (!is_ip_octet_valid)
                {
                    Console.WriteLine("Incorrect ip address...");
                    Thread.Sleep(1500);
                    goto GET_IP_ADDRESS_OF_TARGET;
                }
                else
                {
                    if (!Net.SendPing(ip))
                    {
                        Console.WriteLine($"{ip} is not active...");
                        Thread.Sleep(1500);
                        goto GET_IP_ADDRESS_OF_TARGET;
                    }
                }
            }
            else
            {
                Console.WriteLine("Incorrect ip address...");
                Thread.Sleep(1500);
                goto GET_IP_ADDRESS_OF_TARGET;
            }

            return ip;
        }
    }
}
