using System.Net;
using System.Net.Sockets;
using System.Text;
namespace UDPClassWorkServer
{
    internal class Program
    {
        static int RemotePort = 9001;
        static IPAddress RemoteIPAddr = IPAddress.Loopback;
        static int LocalPort = 9000;

        static UdpClient _serverSocket;
        static void Main(string[] args)
        {
            Console.Title = "Chat";

            Console.WriteLine("Server started on port 9000");
            Console.ResetColor();

            _serverSocket = new UdpClient(LocalPort);

            Thread recieveThread = new Thread(RecieveData);
            recieveThread.IsBackground = true;
            recieveThread.Start();


            Console.ForegroundColor = ConsoleColor.Blue;
            while (true) {
                string msg = Console.ReadLine();
                if(!string.IsNullOrEmpty(msg))
                SendData(Console.ReadLine() ?? "");
            }

        }

        private static void RecieveData()
        {
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Any, 9001);
            try
            {
                while (true)
                {
                    byte[] buffer = _serverSocket.Receive(ref ipEnd);
                    string data = Encoding.UTF8.GetString(buffer);

                    // format: username|color|message
                    string[] parts = data.Split('|');
                    if (parts.Length < 3) continue;

                    string username = parts[0];
                    string color = parts[1];
                    string message = parts[2];

                    ConsoleColor oldColor = Console.ForegroundColor;
                    if (Enum.TryParse(color, out ConsoleColor userColor))
                        Console.ForegroundColor = userColor;

                    Console.WriteLine($"[{username}]: {message}");
                    Console.ForegroundColor = oldColor;

                    string echo = $"[{username}]: {message}";
                    byte[] response = Encoding.UTF8.GetBytes(echo);
                    _serverSocket.Send(response, response.Length, ipEnd);

                    
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Error socket: {ex}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex);

            }
        }

        private static void SendData(string data)
        {
            if(string.IsNullOrEmpty(data))
            {
                return;
            }

            UdpClient uClient = new UdpClient();

            IPEndPoint ipEnd = new IPEndPoint(RemoteIPAddr, RemotePort);
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            uClient.Send(bytes, bytes.Length, ipEnd);
            uClient.Close();
        }
    }         
}
