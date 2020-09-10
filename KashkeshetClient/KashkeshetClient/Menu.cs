using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace KashkeshetClient
{
    class Menu
    {
        public Client client { get; set; }
        public Request request = new Request();
        public Menu()
        {
            client = new Client(request);


        }


        public void MainMenu()
        {
            TcpClient tcpClient = client.StartSession();
            while (true)
            {
                Console.WriteLine("1. Broadcast Chat.");
                Console.WriteLine("2. Private Chat.");
                Console.WriteLine("3. Exit");
                int choice = UserInput();

                switch (choice)
                {
                    case 1:
                        request.Type = "message";
                        client.SendObject(tcpClient);
                        client.SendData(tcpClient);

                        break;
                    case 2:
                        request.Type = "showClients";
                        client.SendData(tcpClient);
                        break;

                    case 3:
                        return;
                    default:
                        Console.WriteLine("You don't have this option");
                        MainMenu();
                        break;
                }
            }

        }


        public int UserInput()
        {
            int result;
            bool parsedSuccessfully = int.TryParse(Console.ReadLine(), out result);
            while (!parsedSuccessfully)
            {
                Console.WriteLine("Numbers Only");
                parsedSuccessfully = int.TryParse(Console.ReadLine(), out result);
            }
            return result;
        }


    }
}