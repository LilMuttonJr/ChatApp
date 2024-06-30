using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static TcpClient client;
    static NetworkStream stream;
    static string username;

    static void Main(string[] args)
    {
        Console.Title = "Client";

        ConnectToServer();
        HandleChat();
    }

    static void ConnectToServer()
    {
        client = new TcpClient();

        try
        {
            client.Connect("localhost", 12345);
            stream = client.GetStream();
            Console.WriteLine("Connected to server. Enter your username:");
            username = Console.ReadLine();
            SendMessage($"{username} has joined the chat.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to connect to server: {ex.Message}");
            return;
        }
    }

    static void HandleChat()
    {
        Thread receiveThread = new Thread(new ThreadStart(ReceiveMessages));
        receiveThread.Start();

        while (true)
        {
            string message = Console.ReadLine();
            if (message.ToLower() == "/exit")
            {
                SendMessage($"{username} has left the chat.");
                break;
            }
            SendMessage($"{username}: {message}");
        }

        receiveThread.Join(); // Wait for receive thread to finish
        Disconnect();
    }

    static void ReceiveMessages()
    {
        try
        {
            while (true)
            {
                byte[] receivedBuffer = new byte[4096];
                int bytesRead = stream.Read(receivedBuffer, 0, receivedBuffer.Length);
                if (bytesRead > 0)
                {
                    string receivedMessage = Encoding.UTF8.GetString(receivedBuffer, 0, bytesRead);
                    Console.WriteLine(receivedMessage);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving message: {ex.Message}");
        }
    }

    static void SendMessage(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }

    static void Disconnect()
    {
        try
        {
            stream.Close();
            client.Close();
            Console.WriteLine("Disconnected from server.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error disconnecting: {ex.Message}");
        }
    }
}
