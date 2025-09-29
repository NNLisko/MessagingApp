/*
 * A STATIC CLASS TO KEEP AN ACCESSIBLE LIST OF CLIENT CONNECTIONS
 * 
 * 
 */

using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using MessageLiskoServer.Exceptions;
using System.Collections.Concurrent;
using System;

namespace MessageLiskoServer.Server;

public static class updateClients
{
    public static ConcurrentDictionary<string, ConnectedClient> clientList { get; set; } =
        new ConcurrentDictionary<string, ConnectedClient>();

    public static bool addClient(string userName, ConnectedClient client)
    {
        // DEBUG Console.Write("client added");
        return clientList.TryAdd(userName, client);
    }

    public static void removeClient(string userName)
    {
        clientList.TryRemove(userName, out _);
    }

    public static async Task broadCastMessage(string msg)
    {
        Console.WriteLine("broadcasting");
        foreach (var client in clientList)
        {
            await client.Value.Writer.WriteLineAsync(msg);
        }
    }
}