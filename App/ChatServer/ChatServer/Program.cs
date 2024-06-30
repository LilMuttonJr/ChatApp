using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static TcpListener tcpListener;
    static List<ClientHandler> clients = new List<ClientHandler>();

    static void Main(string[] args)
    {
        Console.Title = "Server";
        StartServer();
    }

    static void StartServer()
    {
        tcpListener = new TcpListener(IPAddress.Any, 12345);
        tcpListener.Start();
        Console.WriteLine("Server started. Waiting for clients to connect...");

        while (true)
        {
            TcpClient tcpClient = tcpListener.AcceptTcpClient();
            ClientHandler clientHandler = new ClientHandler(tcpClient);
            clients.Add(clientHandler);
            Console.WriteLine($"Client connected: {tcpClient.Client.RemoteEndPoint}");

            Thread clientThread = new Thread(clientHandler.HandleClient);
            clientThread.Start();
        }
    }

    class ClientHandler
    {
        TcpClient tcpClient;
        NetworkStream stream;
        string clientUsername;

        public ClientHandler(TcpClient client)
        {
            tcpClient = client;
        }

        public void HandleClient()
        {
            try
            {
                stream = tcpClient.GetStream();
                SendMessageToAll($"{tcpClient.Client.RemoteEndPoint} has joined the chat.");

                while (true)
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        if (clientUsername == null)
                        {
                            clientUsername = message;
                            Console.WriteLine($"{tcpClient.Client.RemoteEndPoint} set username to: {clientUsername}");
                        }
                        else
                        {
                            Console.WriteLine($"{clientUsername}: {message}");
                            SendMessageToAll($"{clientUsername}: {message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client {tcpClient.Client.RemoteEndPoint} disconnected: {ex.Message}");
            }
            finally
            {
                if (clientUsername != null)
                {
                    SendMessageToAll($"{clientUsername} has left the chat.");
                }
                stream.Close();
                tcpClient.Close();
                clients.Remove(this);
            }
        }

        void SendMessageToAll(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            foreach (ClientHandler client in clients)
            {
                if (client != this)
                {
                    client.stream.Write(data, 0, data.Length);
                    client.stream.Flush();
                }
            }
        }
    }
}
