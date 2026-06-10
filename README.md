# Real-Time-Client-Server-Chat-Application-Using-C-
A TCP socket based console chat system that enables two-way asynchronous communication between a server and a client.

<h2> The system consists of two separate applications: </h2>
     ● a server application
     ● a client application. 
  Once the connection is established, both users can send and receive messages independently without following a fixed client - server reply sequence.

► Use asynchronous programming to continuously listen for incoming messages while allowing the user to type and send new messages at the same time.
► A custom console handling mechanism was also implemented to prevent incoming messages from interrupting or overwriting partially typed text.

Key Features
TCP-based communication between a client and a server
Real-time two-way message exchange
Independent message sending from either side
Asynchronous message receiving
Custom console input handling
Preservation of partially typed messages
Graceful connection termination using the exit command
Local testing through the loopback address 127.0.0.1
