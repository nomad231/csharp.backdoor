using System;
using System.Net.Sockets;
using System.Text;

class Client
{
    static void Main()
    {
        // Sunucunun IP adresi ve port numarası
        // Server's IP adress and port number here
       
        string serverIp = "127.0.0.1";
        int port = 8888;

        TcpClient client = new TcpClient();

        try
        {
            client.Connect(serverIp, port);

            Console.WriteLine("Sunucuya bağlandı.");

            using (NetworkStream stream = client.GetStream())
            {
                while (true)
                {
                    Console.Write("Komut girin ('exit' ile çıkış): ");
                    string command = Console.ReadLine();

                    byte[] commandBytes = Encoding.ASCII.GetBytes(command);
                    stream.Write(commandBytes, 0, commandBytes.Length);

                    if (command == "exit")
                    {
                        Console.WriteLine("Çıkış isteği gönderildi.");
                        break;
                    }

                    StringBuilder responseBuilder = new StringBuilder();
                    byte[] buffer = new byte[1024];

                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        string responsePart = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        responseBuilder.Append(responsePart);
                        if (responsePart.EndsWith("\n")) 
                        {
                            break;
                        }
                    }

                    string result = responseBuilder.ToString().TrimEnd('\n');
                    Console.WriteLine("Sonuç: " + result);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Bağlantı hatası: " + ex.Message);
        }
        finally
        {
            client.Close();
        }
    }
}
