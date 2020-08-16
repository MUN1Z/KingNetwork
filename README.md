
<p align="center">
  <img src="https://github.com/Mun1z/KingNetwork/blob/master/resources/logo.png">
</p>

# Examples

**Cubes Unity Example**<br/>

<p align="center">
  <img src="https://github.com/Mun1z/KingNetwork/blob/master/resources/CubesExample.gif">
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
// create and start the server
var server = new KingServer(port: 7171);
server.MessageReceivedHandler = OnMessageReceived;
server.Start();

// implements the callback for MessageReceivedHandler
private void OnMessageReceived(IClient client, KingBufferReader kingBuffer)
{
    Console.WriteLine($"Received data from client {client.Id}, data length {kingBuffer.Length()}");
}

// send a message to all clients
using(var kingBuffer = KingBufferWriter.Create())
{
    kingBuffer.Write("Test message!");
    server.SendMessageToAll(kingBuffer);
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
private void OnMessageReceived(KingBufferReader kingBuffer)
{
    Console.WriteLine($"Received data from server, data length {kingBuffer.Length()}");
}

/// send a message to server
using(var kingBuffer = KingBufferWriter.Create()
{
    kingBuffer.Write("Test message!");
    client.SendMessage(kingBuffer);
}

// disconnect from the server when we are done
client.Disconnect();
```

# Using the WebSocket connection on KingServer
```C#
// create and start the server
var server = new KingServer(port: 7171);
server.MessageReceivedHandler = OnMessageReceived;
server.Start(NetworkListenerType.WSText); // Or NetworkListenerType.WSBinary

// implements the callback for MessageReceivedHandler
private void OnMessageReceived(IClient client, KingBufferReader kingBuffer)
{
    Console.WriteLine($"Received data from client {client.Id}, data length {kingBuffer.Length()}");
}

// send a message to all clients
using(var kingBuffer = KingBufferWriter.Create())
{
    kingBuffer.Write("Test message!");
    server.SendMessageToAll(kingBuffer);
}

// stop the server when you don't need it anymore
server.Stop();
```

# Using the TCP connection on KingClient
```C#
// create and connect the client
var client = new KingClient();
client.MessageReceivedHandler = OnMessageReceived;
client.Connect("127.0.0.1", 7171, NetworkListenerType.WSText); // Or NetworkListenerType.WSBinary

// implements the callback for MessageReceivedHandler
private void OnMessageReceived(KingBufferReader kingBuffer)
{
    Console.WriteLine($"Received data from server, data length {kingBuffer.Length()}");
}

/// send a message to server
using(var kingBuffer = KingBufferWriter.Create()
{
    kingBuffer.Write("Test message!");
    client.SendMessage(kingBuffer);
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
