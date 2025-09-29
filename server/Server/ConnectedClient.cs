using System.IO;
using System.Net.Sockets;
using System.Text;

namespace MessageLiskoServer.Server;

public class ConnectedClient
{
    public TcpClient Client { get; }
    public NetworkStream stream { get; }
    public StreamWriter Writer { get; }
    public StreamReader Reader { get; }
    public string endPoint { get; }

    public ConnectedClient(TcpClient client)
    {
        
        Client = client;
        stream = client.GetStream();
        Writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
        Reader = new StreamReader(stream);
        endPoint = client.Client.RemoteEndPoint.ToString();
    }
}
