using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using DotNetEnv;
using Microsoft.Maui.ApplicationModel;


namespace MessageLisko.Networking;

/*
 * CLIENT SIDE USER CONNECTION HANDLER
 * 
 * CONNECTS TO SERVER, AUTHENTICATES USER
 * 
 */


public class ClientConnect
{

    // -- Fields -------------------------------------
    private readonly TcpClient _client = new ();
    

    private readonly IPAddress _address;
    private readonly int _port;
    private readonly int _timeout;
    private string _userName;

    

    private NetworkStream? _stream;
    private StreamWriter? _writer;
    private StreamReader? _reader;



    // -- Properties ---------------------------------

    public event Action<string>? MessageReceived;
    public string UserName
    {
        get => _userName;
        set
        {
            if (_userName != value)
            {
                _userName = value;

            }
        }
    }
    
    // -- Constructor --------------------------------

    public ClientConnect()
    {
        string root = "C:\\Programming\\MessageLisko\\MessageLisko\\MessageLisko\\.env";
        Env.Load(@root);
        if (string.IsNullOrWhiteSpace(Env.GetString("HOST")))
            throw new InvalidOperationException("HOST not set in .env");

        if (string.IsNullOrWhiteSpace(Env.GetString("PORT")))
            throw new InvalidOperationException("PORT not set in .env");
        _address = IPAddress.Parse(Env.GetString("HOST")!);
        _port = int.Parse(Env.GetString("PORT"));
    }

    // -- Methods -------------------------------------


    public async Task Connect()
    {
        try
        {

            _client.Connect(_address, _port);
            Debug.WriteLine("Connected");
            _stream = _client.GetStream();
            _writer = new StreamWriter(_stream, Encoding.UTF8) { AutoFlush = true };
            _reader = new StreamReader(_stream, Encoding.UTF8);




            _ = Task.Run(async () =>
            {
                while (true)
                {
                    string msg = await _reader.ReadLineAsync();
                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        Debug.WriteLine("Message received");
                        MessageReceived?.Invoke(msg);
                    }
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task sendProtocolMessage(string msg)
    {
        if (_client.Connected && _writer != null)
        {
            await _writer.WriteLineAsync(msg);
        }
    }

    public void closeConnection()
    {
        if (_client.Connected)
        {
            _client.Close();
        }
    }

    public bool isConnected()
    {
        return _client.Connected;
    }

}
