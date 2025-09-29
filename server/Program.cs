using System.Threading.Tasks;
using System;
using MessageLiskoServer.Server;
using MessageLiskoServer.Database;
using DotNetEnv;
using System.Net;

namespace MessageLiskoServer;

class Program
{

    public static async Task Main(string[] args)
    {
        string root = "C:\\Programming\\MessageLisko\\MessageServer\\";
        Env.Load(root + ".env");
        string? host = Environment.GetEnvironmentVariable("HOST");
        int port = int.Parse(Environment.GetEnvironmentVariable("PORT"));

        string databaseConnString = Environment.GetEnvironmentVariable("CONNSTR");

        var pool = ConnectionPool.CreatePool()
        
        MessageTcpListener server = new MessageTcpListener(host, port);
        await server.StartServer();

        
    }
}