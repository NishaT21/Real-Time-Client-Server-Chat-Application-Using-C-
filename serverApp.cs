using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    class Program
    {
        private static readonly object consoleLock = new object();
        private static readonly StringBuilder typedText = new StringBuilder();

        private static bool isTyping = false;
        private static string currentPrompt = "";

        static async Task Main(string[] args)
        {
            try
            {
                using TcpClient client = new TcpClient();

                Console.WriteLine("Connecting to the server...");

                await client.ConnectAsync(
                    "127.0.0.1",
                    8001);

                Console.WriteLine("Connected successfully.");
                Console.WriteLine("You can start chatting.");
                Console.WriteLine("Type exit to close the chat.");

                using NetworkStream stream = client.GetStream();

                using StreamReader reader =
                    new StreamReader(stream);

                using StreamWriter writer =
                    new StreamWriter(stream)
                    {
                        AutoFlush = true
                    };

                Task receiveTask = ReceiveMessages(reader);

                Task sendTask = Task.Run(() =>
                {
                    SendMessages(writer);
                });

                await Task.WhenAny(receiveTask, sendTask);
            }
            catch (Exception e)
            {
                ShowMessage("Error: " + e.Message);
            }

            Console.WriteLine();
            Console.WriteLine("Connection closed.");
            Console.WriteLine("Press Enter to close the client.");
            Console.ReadLine();
        }

        static async Task ReceiveMessages(StreamReader reader)
        {
            while (true)
            {
                string? message = await reader.ReadLineAsync();

                if (message == null)
                {
                    ShowMessage("Server disconnected.");
                    break;
                }

                ShowMessage("Server: " + message);

                if (message.Equals(
                    "exit",
                    StringComparison.OrdinalIgnoreCase))
                {
                    ShowMessage("Server ended the chat.");
                    break;
                }
            }
        }

        static void SendMessages(StreamWriter writer)
        {
            while (true)
            {
                string message = ReadMessage("Client: ");

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
            lock (consoleLock)
            {
                currentPrompt = prompt;
                typedText.Clear();
                isTyping = true;

                RedrawTypingLine();
            }

            while (true)
            {
                ConsoleKeyInfo key =
                    Console.ReadKey(intercept: true);

                lock (consoleLock)
                {
                    if (key.Key == ConsoleKey.Enter)
                    {
                        string message = typedText.ToString();

                        ClearCurrentLine();
                        Console.WriteLine(currentPrompt + message);

                        typedText.Clear();
                        currentPrompt = "";
                        isTyping = false;

                        return message;
                    }

                    if (key.Key == ConsoleKey.Backspace)
                    {
                        if (typedText.Length > 0)
                        {
                            typedText.Remove(
                                typedText.Length - 1,
                                1);
                        }
                    }
                    else if (!char.IsControl(key.KeyChar))
                    {
                        typedText.Append(key.KeyChar);
                    }

                    RedrawTypingLine();
                }
            }
        }

        static void ShowMessage(string message)
        {
            lock (consoleLock)
            {
                ClearCurrentLine();

                Console.WriteLine(message);

                if (isTyping)
                {
                    RedrawTypingLine();
                }
            }
        }

        static void RedrawTypingLine()
        {
            ClearCurrentLine();

            Console.Write(currentPrompt + typedText);
        }

        static void ClearCurrentLine()
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