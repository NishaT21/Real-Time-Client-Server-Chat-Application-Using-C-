# Real-Time-Client-Server-Chat-Application-Using-C#
A TCP socket based console chat system that enables two-way asynchronous communication between a server and a client.

<h2>📡 Chat Application</h2>

<p>
This system consists of two separate applications:
</p>

<ul>
    <li><strong>Server Application</strong></li>
    <li><strong>Client Application</strong></li>
</ul>

<p>
Once the connection is established, both users can send and receive messages independently without following a fixed client-server reply sequence.
</p>

<h3>🚀 Features</h3>

<ul>
    <li>TCP-based communication between a client and a server</li>
    <li>Real-time two-way message exchange</li>
    <li>Independent message sending from either side</li>
    <li>Asynchronous message receiving</li>
    <li>Custom console input handling</li>
    <li>Preservation of partially typed messages</li>
    <li>Graceful connection termination using the <code>exit</code> command</li>
    <li>Local testing through the loopback address <code>127.0.0.1</code></li>
</ul>

<h3>⚡ Asynchronous Communication</h3>

<p>
The application uses asynchronous programming to continuously listen for incoming messages while allowing users to type and send new messages simultaneously.
</p>

<p>
A custom console handling mechanism has been implemented to prevent incoming messages from interrupting or overwriting partially typed text, ensuring a smooth chat experience.
</p>

<h3>🛠 Technologies Used</h3>

<ul>
    <li>C# and .NET</li>
    <li>TCP/IP Sockets</li>
    <li>TcpListener & TcpClient</li>
    <li>NetworkStream</li>
    <li>StreamReader & StreamWriter</li>
    <li>Async/Await Programming</li>
</ul>


<img src="flowchart.png" alt="Flowchart" width="300">


## 📡 Server Application Execution Flow

```text
Program Class Loaded
│
├── Class Fields Initialized
│     ├── consoleLock
│     ├── typedText
│     ├── isTyping
│     └── currentPrompt
│
└── Main() Starts
      │
      ├── Create TcpListener
      ├── Start Listening on 127.0.0.1:8001
      ├── Wait for Client Connection
      ├── Client Connected
      ├── Create NetworkStream
      ├── Create StreamReader and StreamWriter
      │
      ├── Start Receive Task
      │     └── ReceiveMessages(reader)
      │           └── Continuously reads incoming client messages
      │
      ├── Start Send Task
      │     └── SendMessages(writer)
      │           └── Allows server user to type and send messages
      │
      └── Wait until one task finishes
            └── Usually when "exit" is typed or connection is closed
```

## 💻 Client Application Execution Flow

```text
Program Class Loaded
│
├── Class Variables Created
│     ├── consoleLock
│     ├── typedText
│     ├── isTyping
│     └── currentPrompt
│
└── Main() Starts
      │
      ├── Create TcpClient
      ├── Try to Connect to Server
      │     └── IP Address: 127.0.0.1
      │     └── Port: 8001
      │
      ├── Connection Established
      ├── Create NetworkStream
      ├── Create StreamReader and StreamWriter
      │
      ├── Start Receive Task
      │     └── ReceiveMessages(reader)
      │           └── Continuously reads incoming server messages
      │
      ├── Start Send Task
      │     └── SendMessages(writer)
      │           └── Allows client user to type and send messages
      │
      └── Wait until one task finishes
            └── Usually when "exit" is typed or connection is closed
```
