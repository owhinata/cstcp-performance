using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class TcpClientApp
{
    static async Task Main(string[] args)
    {
        using var client = new TcpClient();
        client.NoDelay = true;

        try
        {
            var serverIp = args.Length > 0 ? args[0] : "127.0.0.1";
            await client.ConnectAsync(serverIp, 5000);
            Console.WriteLine("Connected to server.");

            using var stream = client.GetStream();

            var receiveBuffer = new byte[512];
            int totalReceived = 0;
            while (totalReceived < 512 * 100000)
            {
                int bytesRead = await stream.ReadAsync(receiveBuffer, 0, receiveBuffer.Length);
                if (bytesRead == 0)
                {
                    return;
                }
                totalReceived += bytesRead;
            }

            await stream.WriteAsync(new byte[] { 1 }, 0, 1);
            await stream.FlushAsync();

            var resultBuffer = new byte[64];
            int len = await stream.ReadAsync(resultBuffer, 0, resultBuffer.Length);
            var result = Encoding.UTF8.GetString(resultBuffer, 0, len);
            Console.WriteLine($"Elapsed: {result} ms");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }

        Console.WriteLine("Disconnected.");
    }
}
