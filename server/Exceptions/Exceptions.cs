// Exceptions class

using System;

namespace MessageLiskoServer.Exceptions;

public class ProtocolException : Exception
{
    public ProtocolException(string message) : base("Invalid Message operation") { }
}

public class ConnectionException : Exception
{
    public ConnectionException(string message) : base("Client's connection to the server has failed") { }
}

public class InvalidOperation : Exception
{
    public InvalidOperation(string message) : base("Invalid operation") { }
}