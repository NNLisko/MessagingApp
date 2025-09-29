using System;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Collections.Concurrent;
using MessageLiskoServer.Exceptions;
using DotNetEnv;

namespace MessageLiskoServer.Server;

class MessageTcpListener {

    private TcpListener? _server;
    private readonly string _serverAddress;
    private readonly int _port;


    public MessageTcpListener(string serverAddress, int port)
    {
        _serverAddress = serverAddress;
        _port = port;
    }

    public void StopServer()
    {
        _server?.Stop();
        _server = null;
    }

    public async Task StartServer()
    {
        if (_server != null)
        {
            throw new InvalidOperationException("Server already running");
        }
        
        try
        {
            IPAddress ipAddress = IPAddress.Parse(_serverAddress);
            
            _server = new TcpListener(ipAddress, _port);
            _server.Start();
            

            while (true)
            {
                Console.Write("Waiting for connection... ");

                TcpClient client = await _server.AcceptTcpClientAsync();
                Console.WriteLine($"Connected from: {client.Client.RemoteEndPoint}");

                _ = Task.Run(() => new ClientHandler(client).ProcessAsync());
            }
        }
        catch (ConnectionException e)
        {
            Console.WriteLine("ConnectionException: {0}", e);
        }
        finally {
            StopServer();
        }
        Console.WriteLine("\nHit enter to continue...");
        Console.Read();
    }
}