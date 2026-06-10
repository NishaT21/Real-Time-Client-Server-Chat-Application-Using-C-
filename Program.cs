using System;
using System.IO;  //Reading and writing data streams
using System.Net;  //IP address handling
using System.Net.Sockets; //TCP communication
using System.Text; //Storing and modifying typed text
using System.Threading.Tasks; //Running send and receive tasks together

namespace ServerApp
{
    class Program
    {
        private static readonly object consoleLock = new object();  //prevent two tasks from writting to the console at the same time
        private static readonly StringBuilder typedText = new StringBuilder(); //store the message while the user is typing it
         
        private static bool isTyping = false; //check whether thr s user is typing it
        private static string currentPrompt = ""; // store the current promp

        static async Task Main(string[] args)
        {
            TcpListener listener =
                new TcpListener(IPAddress.Loopback, 8001);

            try
            {
                listener.Start(); //server to begin listening for incoming client connections.

                Console.WriteLine("Server started.");
                Console.WriteLine("Waiting for a client...");

                using TcpClient client =
                    await listener.AcceptTcpClientAsync(); //await-program waits without freezing the application unnecessarily.

                Console.WriteLine("Client connected.");
                Console.WriteLine("You can start chatting.");
                Console.WriteLine("Type exit to close the chat.");

                using NetworkStream stream = client.GetStream();  //Creating the Communication Stream

                //Reading and Writing Messages
                using StreamReader reader =
                    new StreamReader(stream);

                using StreamWriter writer =
                    new StreamWriter(stream)
                    {
                        AutoFlush = true //each message sent immediately without stored temporarily 
                    };


                // Task: Two operations run at the same time
                //ReceiveMessages() → continuously listens for messages
                //SendMessages()    → allows the user to type messages
                Task receiveTask = ReceiveMessages(reader);

                Task sendTask = Task.Run(() =>
                {
                    SendMessages(writer);
                });

                await Task.WhenAny(receiveTask, sendTask);
            }
            catch (Exception e)
            {
                ShowMessage("Error: " + e.Message);  //show earrors like Error: ....
            }
            finally
            {
                listener.Stop();
            }

            Console.WriteLine();
            Console.WriteLine("Connection closed.");
            Console.WriteLine("Press Enter to close the server.");
            Console.ReadLine();
        }


        static async Task ReceiveMessages(StreamReader reader)
        {
            while (true)  //this loop waits for new incoming messages
            {
                string? message = await reader.ReadLineAsync();

                if (message == null) //if null message , client disconnected
                {
                    ShowMessage("Client disconnected.");
                    break;
                }

                ShowMessage("Client: " + message);

                //if client sent exit, disconnected 
                if (message.Equals(
                    "exit",
                    StringComparison.OrdinalIgnoreCase))
                {
                    ShowMessage("Client ended the chat.");
                    break;
                }
            }
        }

        static void SendMessages(StreamWriter writer)
        {
            while (true)  //continously allow to type an message by user
            {
                string message = ReadMessage("Server: "); 

                writer.WriteLine(message);

                if (message.Equals(
                    "exit",
                    StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }
        }

        static string ReadMessage(string prompt)
        {
            lock (consoleLock) //set the prompt and parepare to capture the text
            {
                currentPrompt = prompt;
                typedText.Clear();
                isTyping = true;

                RedrawTypingLine();
            }

            while (true) //Read one key at a time
            {
                ConsoleKeyInfo key =
                    Console.ReadKey(intercept: true);

                lock (consoleLock)
                {
                    if (key.Key == ConsoleKey.Enter) //when enter is pressed , finish the message
                    {
                        string message = typedText.ToString();

                        ClearCurrentLine();
                        Console.WriteLine(currentPrompt + message);

                        typedText.Clear(); //define the typing status
                        currentPrompt = "";
                        isTyping = false;

                        return message;
                    }

                    if (key.Key == ConsoleKey.Backspace) //handle the backspace manually
                    {
                        if (typedText.Length > 0)
                        {
                            typedText.Remove(
                                typedText.Length - 1,
                                1);
                        }
                    }
                    else if (!char.IsControl(key.KeyChar)) //Add normal characters to the typed message
                    {
                        typedText.Append(key.KeyChar);
                    }

                    RedrawTypingLine();  //refresh the typing line after every key press
                }
            }
        }

        static void ShowMessage(string message)  //clear the line where maybe user be typing
        {
            lock (consoleLock)
            {
                ClearCurrentLine();

                Console.WriteLine(message); //show the message

                if (isTyping) //if user was typing, show their unfinished message again
                {
                    RedrawTypingLine();
                }
            }
        }

        static void RedrawTypingLine()  //clear and redraw the current typing line 
        {
            ClearCurrentLine();

            Console.Write(currentPrompt + typedText);
        }

        static void ClearCurrentLine() //clear the current console line safety 
        {
            int width = Math.Max(
                Console.WindowWidth - 1,
                1);

            Console.Write(
                "\r" +
                new string(' ', width) +
                "\r");
        }
    }
}