using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class TcpServer
{
    private static readonly byte[] SendBuffer = new byte[512];

    static async Task Main()
    {
        Random.Shared.NextBytes(SendBuffer);

        var listener = new TcpListener(IPAddress.Any, 5000);
        listener.Server.NoDelay = true;
        listener.Start();
        Console.WriteLine("TCP Server started on port 5000.");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            Console.WriteLine("Client connected.");
            _ = HandleClient(client);
        }
    }

    static async Task HandleClient(TcpClient client)
    {
        using var stream = client.GetStream();
        client.NoDelay = true;

        try
        {
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
            {
                await stream.WriteAsync(SendBuffer, 0, SendBuffer.Length);
            }
            await stream.FlushAsync();

            var responseBuffer = new byte[16];
            int bytesRead = 0;
            while (bytesRead == 0)
            {
                bytesRead = await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length);
            }
            sw.Stop();

            var resultBytes = Encoding.UTF8.GetBytes(sw.Elapsed.TotalMilliseconds.ToString());
            await stream.WriteAsync(resultBytes, 0, resultBytes.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }

        client.Close();
        Console.WriteLine("Client disconnected.");
    }
}
