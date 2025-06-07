using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class TcpClientApp
{
    static async Task Main()
    {
        using var client = new TcpClient();

        try
        {
            await client.ConnectAsync("127.0.0.1", 5000);
            Console.WriteLine("Connected to server.");

            using var stream = client.GetStream();

            while (true)
            {
                Console.Write("Enter message (or 'exit' to quit): ");
                string message = Console.ReadLine();

                if (string.IsNullOrEmpty(message)) continue;
                if (message.ToLower() == "exit") break;

                byte[] data = Encoding.UTF8.GetBytes(message);
                await stream.WriteAsync(data, 0, data.Length);

                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                Console.WriteLine("Received from server: " + response);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
        }

        Console.WriteLine("Disconnected.");
    }
}
