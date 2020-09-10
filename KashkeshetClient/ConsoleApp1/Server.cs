using KashkeshetClient;
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


namespace KashkeshetServer
{
    class Server
    {
        static readonly object _lock = new object();

        static readonly Dictionary<int, TcpClient> TcpClients = new Dictionary<int, TcpClient>();
        static readonly Dictionary<TcpClient, Request> Requests = new Dictionary<TcpClient, Request>();

        //public TcpClient client { get; set; }
        public Request request { get; set; }

        public Stream SenderStrm { get; set; }

        public Stream RevicerStrm { get; set; }
        public int count { get; set; }
        //public int id { get; set; }
        public Server()
        {
            count = 1;
            //id = 1;
        }
        public void StartServer()
        {
            TcpListener ServerSocket = new TcpListener(IPAddress.Any, 11000);
            ServerSocket.Start();


            while (true)
            {

                TcpClient client = ServerSocket.AcceptTcpClient();
                Console.WriteLine("clinet connected");

                //recive the request

                if (!TcpClients.ContainsKey(count))
                {
                    TcpClients.Add(count, client);
                    //Thread threadobject = new Thread(o => reciveObject((TcpClient)o));
                    //threadobject.Start(client);
                    ////threadobject.Join();
                }

                Thread t = new Thread(handle_clients);
                t.Start(count);
                count++;

            }

        }
        //public void reciveObject(TcpClient client)
        //{

            

        //}
        public void handle_clients(object o)
        {
            int id = (int)o;
            TcpClient client2;

            lock (_lock) client2 = TcpClients[id];

            while (true)
            {
                RevicerStrm = client2.GetStream();
                IFormatter formatter = new BinaryFormatter();
                request = (Request)formatter.Deserialize(RevicerStrm);
                if (!Requests.ContainsKey(client2))
                {
                    lock (_lock) Requests.Add(client2, request);
                    Requests[client2].Text = ("join the Chat\n");
                    //Console.WriteLine("sending joining message massege");
                    //SendMessage(client2);

                }
                Console.WriteLine("recived obj " + Requests[client2].Name);

                if (Requests[client2].Text == "exit")
                {
                    break;
                }
                Console.WriteLine("sending message");
                SendMessage(client2);
            }
            Requests[client2].Text = ("Leave the Chat\n");
            SendMessage(client2);
            lock (_lock) TcpClients.Remove(id);
            client2.Client.Shutdown(SocketShutdown.Both);
            client2.Close();

        }

        public void SendMessage(TcpClient client1)
        {
            TcpClient client3;
            lock (_lock) client3 = client1;

            lock (_lock)
            {

                if (request.Type == "message")
                {
                    foreach (TcpClient c in TcpClients.Values)
                    {
                        Console.WriteLine("Enter the loop");
                        if (client3 != c)
                        {
                            //IFormatter formatter = new BinaryFormatter();
                            SenderStrm = c.GetStream();
                            //formatter.Serialize(SenderStrm, request);
                            //Console.WriteLine("send");
                            byte[] buffer = Encoding.ASCII.GetBytes(request.Name +": "+ request.Text);
                            SenderStrm.Write(buffer, 0, buffer.Length);

                        }

                    }
                    Console.WriteLine("out of the loop");

                }

            }


        }

    }
}
