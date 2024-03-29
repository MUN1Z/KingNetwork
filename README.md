[![MIT License](https://img.shields.io/apm/l/atomic-design-ui.svg?)](https://github.com/tterb/atomic-design-ui/blob/master/LICENSEs)
[![Discord](https://img.shields.io/discord/507641974421979145?label=Discord)](https://discord.gg/N8gyF7)
[![GitHub stars](https://img.shields.io/github/stars/mun1z/kingnetwork?label=stargazers&logoColor=yellow&style=social)](https://github.com/mun1z/kingnetwork/stargazers)

<p align="center">
  <img src="https://github.com/Mun1z/KingNetwork/blob/main/resources/logo.png">
</p>

# Examples

**Cubes Unity Example**<br/>

<p align="center">
  <img src="https://github.com/Mun1z/KingNetwork/blob/main/resources/CubesExample.gif">
</p>

<br/>

# Nuget Packages

**King Server**<br/>
<a href="https://www.nuget.org/packages/KingNetwork.Server/">https://www.nuget.org/packages/KingNetwork.Server/</a><br/>

**King Client**<br/>
<a href="https://www.nuget.org/packages/KingNetwork.Client/">https://www.nuget.org/packages/KingNetwork.Client/</a><br/>

**King Shared**<br/>
<a href="https://www.nuget.org/packages/KingNetwork.Shared/">https://www.nuget.org/packages/KingNetwork.Shared/</a><br/><br/>

# Using the TCP connection on KingServer
```C#
// create and start the async server
var server = new KingServer(port: 7171);
server.MessageReceivedHandler = OnMessageReceived;

//ASync execution
await server.StartAsync(); //You can pass a out var cancellationToken in parameter
//Sync execution
server.Start();

// implements the callback for MessageReceivedHandler
private void OnMessageReceived(IClient client, IKingBufferReader reader)
{
    Console.WriteLine($"Received data from client {client.Id}, data length {reader.Length()}");
}

// send a message to all clients
using(var writer = KingBufferWriter.Create())
{
    writer.Write("Test message!");
    server.SendMessageToAll(writer);
}

// stop the server when you don't need it anymore
server.Stop();
```

# Using the TCP connection on KingClient
```C#
// create and connect the client
var client = new KingClient();
client.MessageReceivedHandler = OnMessageReceived;
client.Connect("127.0.0.1", 7171);

// implements the callback for MessageReceivedHandler
private void OnMessageReceived(IKingBufferReader reader)
{
    Console.WriteLine($"Received data from server, data length {reader.Length()}");
}

/// send a message to server
using(var writer = KingBufferWriter.Create()
{
    writer.Write("Test message!");
    client.SendMessage(writer);
}

// disconnect from the server when we are done
client.Disconnect();
```

# Using the UDP connection on KingServer
```C#
// create and start the server
var server = new KingServer(listenerType: NetworkListenerType.UDP, port: 7171);
server.MessageReceivedHandler = OnMessageReceived;

//ASync execution
await server.StartAsync(); //You can pass a out var cancellationToken in parameter
//Sync execution
server.Start();

// implements the callback for MessageReceivedHandler
private void OnMessageReceived(IClient client, IKingBufferReader reader)
{
    Console.WriteLine($"Received data from client {client.Id}, data length {reader.Length()}");
}

// send a message to all clients
using(var writer = KingBufferWriter.Create())
{
    writer.Write("Test message!");
    server.SendMessageToAll(writer);
}

// stop the server when you don't need it anymore
server.Stop();
```

# Using the UDP connection on KingClient
```C#
// create and connect the client
var client = new KingClient(listenerType: NetworkListenerType.UDP);
client.MessageReceivedHandler = OnMessageReceived;
client.Connect("127.0.0.1", 7171);

// implements the callback for MessageReceivedHandler
private void OnMessageReceived(IKingBufferReader reader)
{
    Console.WriteLine($"Received data from server, data length {reader.Length()}");
}

/// send a message to server
using(var writer = KingBufferWriter.Create()
{
    writer.Write("Test message!");
    client.SendMessage(writer);
}

// disconnect from the server when we are done
client.Disconnect();
```

# Using the RUDP connection on KingServer
```C#
// create and start the server
var server = new KingServer(listenerType: NetworkListenerType.RUDP, port: 7171);
server.MessageReceivedHandler = OnMessageReceived;

//ASync execution
await server.StartAsync(); //You can pass a out var cancellationToken in parameter
//Sync execution
server.Start();

// implements the callback for MessageReceivedHandler
private void OnMessageReceived(IClient client, IKingBufferReader reader)
{
    Console.WriteLine($"Received data from client {client.Id}, data length {reader.Length()}");
}

// send a message to all clients
using(var writer = KingBufferWriter.Create())
{
    writer.Write("Test message!");
    //You can use RudpMessageType.Reliable to send Reliable messages and RudpMessageType.Unreliable to send Unreliable messages
    server.SendMessageToAll(writer, RudpMessageType.Reliable);
}

// stop the server when you don't need it anymore
server.Stop();
```

# Using the RUDP connection on KingClient
```C#
// create and connect the client
var client = new KingClient(listenerType: NetworkListenerType.RUDP);
client.MessageReceivedHandler = OnMessageReceived;
client.Connect("127.0.0.1", 7171);

// implements the callback for MessageReceivedHandler
private void OnMessageReceived(IKingBufferReader reader)
{
    Console.WriteLine($"Received data from server, data length {reader.Length()}");
}

/// send a message to server
using(var writer = KingBufferWriter.Create()
{
    writer.Write("Test message!");
    //You can use RudpMessageType.Reliable to send Reliable messages and RudpMessageType.Unreliable to send Unreliable messages
    client.SendMessage(writer, RudpMessageType.Reliable);
}

// disconnect from the server when we are done
client.Disconnect();
```

# Using the WebSocket connection on KingServer
```C#
// create and start the server
var server = new KingServer(listenerType: NetworkListenerType.WSText, port: 7171); // Or NetworkListenerType.WSBinary
server.MessageReceivedHandler = OnMessageReceived;

//ASync execution
await server.StartAsync(); //You can pass a out var cancellationToken in parameter
//Sync execution
server.Start();

// implements the callback for MessageReceivedHandler
private void OnMessageReceived(IClient client, IKingBufferReader reader)
{
    Console.WriteLine($"Received data from client {client.Id}, data length {reader.Length()}");
}

// send a message to all clients
using(var writer = KingBufferWriter.Create())
{
    writer.Write("Test message!");
    server.SendMessageToAll(writer);
}

// stop the server when you don't need it anymore
server.Stop();
```

# Using the WebSocket connection on KingClient
```C#
// create and connect the client
var client = new KingClient(listenerType: NetworkListenerType.WSText); // Or NetworkListenerType.WSBinary
client.MessageReceivedHandler = OnMessageReceived;
client.Connect("127.0.0.1", 7171);

// implements the callback for MessageReceivedHandler
private void OnMessageReceived(IKingBufferReader reader)
{
    Console.WriteLine($"Received data from server, data length {reader.Length()}");
}

/// send a message to server
using(var writer = KingBufferWriter.Create()
{
    writer.Write("Test message!");
    client.SendMessage(writer);
}

// disconnect from the server when we are done
client.Disconnect();
```

# TCP Benchmarks

**Connections Test**<br/>
We also test only the raw KingNetwork library by spawing 1 server and 1000 clients, each client sending 100 bytes 14 times per second and the server echoing the same message back to each client.
Test Computer: Acer F 15 with a 2,9 GHz Intel Core i7 7gen processor and 32 GB ram DDR4.<br/>
Test Results:<br/>

| Clients | CPU Usage | Ram Usage | Bandwidth Client+Server  | Result |
| ------- | ----------| --------- | ------------------------ | ------ |
|  64     |      0.5% |      9 MB |         0.3 MB/s         | Passed |
|  128    |        1% |     10 MB |         0.7 MB/s         | Passed |
|  500    |       18% |     18 MB |         2~3 MB/s         | Passed |
|  1000   |       32% |     26 MB |         5~6 MB/s         | Passed |
