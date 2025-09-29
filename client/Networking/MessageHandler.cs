using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Diagnostics.Eventing.Reader;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Devices.HumanInterfaceDevice;
using System.Diagnostics;


namespace MessageLisko.Networking;

// MESSAGE CLASS TO HANDLE THE RECEIVED MESSAGES AND SENDING THEM TO THE SERVER

public class MessageHandler : INotifyPropertyChanged
{

    // -- Fields -------------------------------------

    private readonly ClientConnect _client;
    private bool _connected;

    private string _userName;

    // -- Properties ---------------------------------

    public ObservableCollection<string> Messages { get; } = new ();
    
    public string UserName
    {
        get => _userName;
        set
        {
            if (_userName != value)
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }
    }

    public bool Connected
    {
        get => _connected;
        set
        {
            if (_connected != value)
            {
                _connected = value;
                OnPropertyChanged(nameof(Connected));
                OnPropertyChanged(nameof(ConnectionStatusText));
            }
        }
    } 

    // -- IPropertyChanged things -------------------- 

    public string ConnectionStatusText => _connected ? "Server: Online" : "Server: Offline";

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    // -- Constructor --------------------------------
    public MessageHandler(ClientConnect client)
    {
        _client = client;

        _client.MessageReceived += msg =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ParseProtocolMessage(msg);
            });
        };
    }

    // -- Methods ------------------------------------

    public async Task HandleClientLogin(string name)
    {
        UserName = name;
        _client.UserName = name;
        await _client.Connect();

        if (_client.isConnected())
        {
            Connected = true;
        }
        
        string protocolMessage = $"| MSGTYPE: userLogin | USERNAME: {UserName} |";
        await _client.sendProtocolMessage(protocolMessage);
    }

    public void sendMessage(string message)
    {
        string protocolMessage = $"| MSGTYPE: chatMessage | USERNAME: {UserName} | MSG: {message} |";
        _client.sendProtocolMessage(protocolMessage);
    }
    

    public void ParseProtocolMessage(string msg) {
        Debug.WriteLine("Parsing");
        var messageObject = makeDictionary(msg);

        messageObject.TryGetValue("MSGTYPE", out string msgtype);
        messageObject.TryGetValue("USERNAME", out string user);

        if (msgtype == "userLogin")
        {
            AddMessage($"{user} joined!");
        } else if (msgtype == "chatMessage")
        {
            messageObject.TryGetValue("MSG", out string message);
            AddMessage($"{user}: {message}");
        }
    }

    public void AddMessage(string msg)
    {
        Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() =>
        {
            Messages.Add(msg);
        });
    }


    private Dictionary<string, string> makeDictionary(String data)
    {
        String[] fields = data.Split("|", StringSplitOptions.RemoveEmptyEntries);
        var MessageDict = new Dictionary<String, String>();

        foreach (String field in fields)
        {
            String[] parts = field.Split(':', 2);
            if (parts.Length == 2)
            {
                String key = parts[0].Trim();
                String value = parts[1].Trim();
                MessageDict[key] = value;
            }
        }
        return MessageDict;
    }
}
