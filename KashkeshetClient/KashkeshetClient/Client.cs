using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KashkeshetClient
{
    public class Client
    {
        public Request request { get; set; }
        public Request resiveRequest { get; set; }

        public TcpClient client { get; set; }
        public bool isFirstTime { get; set; }

        public Client(Request request)
        {
            this.request = request;
            isFirstTime = true;
        }

        public TcpClient StartSession()
        {

            Console.Write("Please enter user name: ");
            request.Name = Console.ReadLine();

            int port = 11000;
            client = new TcpClient("10.1.0.20", port);

            Console.WriteLine("Connected To Server, For Exit Enter \"exit\" ");

            return client;


        }
        public void SendObject(TcpClient client)
        {
            IFormatter formatter = new BinaryFormatter();
            NetworkStream strm = client.GetStream();
            formatter.Serialize(strm, request);

        }

        public void SendData(TcpClient client)
        {
            Stream strm = client.GetStream();



            //start reciving data
            Thread thread = new Thread(o => ReceiveData((TcpClient)o));
            thread.Start(client);


            while (true)
            {

                if (request.Type == "message")
                {
                    Console.Write("you: ");
                    request.Text = (request.Name+": "+ Console.ReadLine());
                    SendObject(client);
                    if (request.Text == "exit")
                        break;
                }
                else if (request.Type == "showClients")
                {

                    SendObject(client);
                    Console.Write("Choose client name to chat with: ");
                    request.Dst = Console.ReadLine();
                    request.Text = (request.Name + " want to private chat with you");
                    request.Type = "privateChat";
                }
                else if (request.Type == "privateChat")
                {
                    Console.Write("you: ");
                    request.Text = (request.Name + ": " + Console.ReadLine());
                    SendObject(client);
                }



            }
            client.Client.Shutdown(SocketShutdown.Both);
            client.Close();



        }

        public string UserInput()
        {
            string userInput;
            Console.Write("Enter Message: ");
            userInput = Console.ReadLine();
            request.Text = userInput;
            return userInput;

        }

        public void ReceiveData(TcpClient client2)
        {


            Stream recivestrm = client2.GetStream();
            byte[] receivedBytes = new byte[1024];
            int byte_count;
            while ((byte_count = recivestrm.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
            {
                string RecivedText = Encoding.ASCII.GetString(receivedBytes, 0, byte_count);

                Console.Write("\n" + RecivedText  +"\nyou:");
            }


        }


    }
}