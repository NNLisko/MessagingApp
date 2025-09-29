/* 
 * Network protocol thread class, containing the client connection,
 * with methods to validate and process the messages from a client
 * and call database updates
 * 
 * protocol messages from the clients should come in the form below
 * (with some messagetypes having additional fields)
 * 
 *  | MSGTYPE: x | USERNAME: x | RECEIVINGUSERNAME: x | MSG: x |
 *  
 *  MSGTYPE:            [chatMessage, userLogin]
 *  USERNAME:            [String]
 *  RECEIVINGUSERNAME:  [STRING]
 *  MSG:                [String]
 *  
 *  NOTES:
 *  
 *  - with "|" as the divisor  
 *  - example 
 *      | MSGTYPE: chatMessage | USERNAME: Lisko | RECEIVINGUSERNAME: Bob | MSG: Hello Bob! |
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MessageLiskoServer.Exceptions;

namespace MessageLiskoServer.Server;

class ClientHandler
{
    private readonly ConnectedClient? _client;

    string? data;
    private string? _userName;

    public ClientHandler(TcpClient client)
    {
        _client = new ConnectedClient(client);
    }

    public async Task ProcessAsync()
    {

        try
        {

            //stream = _client.GetStream();
            //writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            //reader = new StreamReader(stream, Encoding.UTF8);

            while ((data = await _client.Reader.ReadLineAsync()) != null)
            {
                //await updateClients.broadCastMessage(_userName, data);
                try 
                {
                    ValidateMessageFormat(data);
                    HandleMessage(data);
                }
                catch (ProtocolException p)
                {
                    CloseConnection(_client);
                }
                
            }
        }
        catch (Exception e)
        {
            CloseConnection(_client);
        }
        finally
        {
            if (!string.IsNullOrEmpty(_userName))
            {
                updateClients.removeClient(_userName);
            }

            CloseConnection(_client);
        }
    }

    // validates messageformat by turning it into a map and validating key-value pairs
    // for specific messagetypes

    private void ValidateMessageFormat(String data)
    {
        if (data == null || data.Length == 0) 
            throw new ProtocolException("Message contained nothing");

        var MessageObject = makeDictionary(data);

        // checks the message dictionary against mandatory field switch case below

        if (!MessageObject.TryGetValue("MSGTYPE", out string msgType))
        {
            throw new ProtocolException("Message missing MSGTYPE");
        }

        var requiredFields = msgType switch
        {
            "chatMessage" => new[] { "USERNAME", "MSG" },
            "userLogin" => new[] { "USERNAME" },
            //"userLogout" => new[] { "USERNAME" },
            //"friendRequest" => new[] { "USERNAME", "RECEIVINGUSERNAME" },
            //"friendRequestResponse" => new[] { "USERNAME", "RECEIVINGUSERNAME", "RESPONSE" },
            _ => Array.Empty<string>()
        };

        foreach (var field in requiredFields)
        {
            if (!MessageObject.ContainsKey(field)) 
                throw new ProtocolException("Message doesn't contain the required fields");
        }
    }


    // processes messagetype, so the first field and sends it to the correct method
    private void HandleMessage(String data)
    {

        Console.WriteLine($"RECEIVED: {data}");
        var messageObject = makeDictionary(data);

        messageObject.TryGetValue("MSGTYPE", out string msgtype);

        if (msgtype.Equals("chatMessage"))
        {
            ProcessChatMessage(messageObject);
            
        } else if (msgtype.Equals("userLogin"))
        {
            ProcessUserLogin(messageObject);
        }
    }

    // processes messagetypes of chatMessage
    private void ProcessChatMessage(Dictionary<string, string> MessageObject)
    {
        MessageObject.TryGetValue("MSG", out string message);
        MessageObject.TryGetValue("USERNAME", out string user);

        string protocolMessage = composeProtocolMessage(_userName, "chatMessage", message);
        Console.WriteLine($"SENT ALL: {protocolMessage}");
        updateClients.broadCastMessage(protocolMessage);
    }


    // Handles messages type userLogin : client opens app
    private async Task ProcessUserLogin(Dictionary<string, string> MessageObject)
    {
        MessageObject.TryGetValue("USERNAME", out string user);
        _userName = user;

        updateClients.addClient(_userName, _client);
        string protocolMessage = composeProtocolMessage(_userName, "userLogin");
        Console.WriteLine($"SENT ALL: {protocolMessage}");
        await updateClients.broadCastMessage(protocolMessage);
    }


    // Makes a dictionary of the message
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

    private string composeProtocolMessage(string user, string type, string message = null)
    {
        if (type == "userLogin")
        {
            return $"| MSGTYPE: userLogin | USERNAME: {user} |";
        } else if (type == "chatMessage")
        {
            return $"| MSGTYPE: chatMessage | USERNAME: {user} | MSG: {message} |";
        }
        return string.Empty;
    }

    private void CloseConnection(ConnectedClient _client)
    {

    }
}