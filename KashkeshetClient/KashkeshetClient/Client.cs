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

        public void StartSession()
        {

            Console.Write("Please enter user name: ");
            request.Name = Console.ReadLine();

            int port = 11000;
            client = new TcpClient("10.1.0.20", port);

            Console.WriteLine("Connected To Server, For Exit Enter \"exit\" ");
            SendObject(request, client);
            SendData(client);


        }
        public void SendObject(Request request1, TcpClient client)
        {
            IFormatter formatter = new BinaryFormatter();
            NetworkStream strm = client.GetStream();
            formatter.Serialize(strm, request1);
            Console.WriteLine("send Messege");
        }

        public void SendData(TcpClient client)
        {
            Console.WriteLine("get to SEND DATA");
            Stream strm = client.GetStream();



            //start reciving data
            Thread thread = new Thread(o => ReceiveData((TcpClient)o));
            thread.Start(client);
            //thread.Join();

            while (UserInput()!= "exit")
            {

                SendObject(request, client);


            }

            client.Client.Shutdown(SocketShutdown.Send);
            thread.Join();
            strm.Close();
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
                Console.Write("\n"+Encoding.ASCII.GetString(receivedBytes, 0, byte_count));
            }


        }


    }
}
