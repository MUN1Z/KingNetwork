# Using the KingServer
```C#
// create and start the server
var server = new KingServer(port: 7171);
server.MessageReceivedHandler = OnMessageReceived;
server.Start();

// implements the callback for MessageReceivedHandler
private void OnMessageReceived(IClient client, KingBuffer kingBuffer)
{
    Console.WriteLine($"Received data from client {client.Id}, data length {kingBuffer.Length()}");
}

// send a message to all clients
var kingBuffer = new KingBuffer();
kingBuffer.WriteString("Test message!");
server.SendMessageToAll(kingBuffer);

// stop the server when you don't need it anymore
server.Stop();
```

# Using the KingClient
```C#
// create and connect the client
var client = new KingClient();
client.MessageReceivedHandler = OnMessageReceived;
client.Connect("127.0.0.1", 7171);

// implements the callback for MessageReceivedHandler
private void OnMessageReceived(KingBuffer kingBuffer)
{
    Console.WriteLine($"Received data from server, data length {kingBuffer.Length()}");
}

/// send a message to server
var kingBuffer = new KingBuffer();
kingBuffer.WriteString("Test message!");
client.SendMessage(kingBuffer);

// disconnect from the server when we are done
client.Disconnect();
```

# Unity Integration
TODO 
```
# Benchmarks

**Connections Test**<br/>
We also test only the raw KingNetwork library by spawing 1 server and 1000 clients, each client sending 100 bytes 14 times per second and the server echoing the same message back to each client.
Test Computer: Acer F 15 with a 2,9 GHz Intel Core i7 7gen processor and 32 GB ram DDR4.<br/>
Test Results:<br/>

| Clients | CPU Usage | Ram Usage | Bandwidth Client+Server  | Result |
| ------- | ----------| --------- | ------------------------ | ------ |
|  1000   |       32% |     26 MB |         5-6 MB/s         | Passed |
