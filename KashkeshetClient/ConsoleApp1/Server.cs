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

                //recive the request

                if (!TcpClients.ContainsKey(count))
                {
                    TcpClients.Add(count, client);
                }

                Thread t = new Thread(handle_clients);
                t.Start(count);
                count++;

            }

        }

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
                    if (request.Type == "message")
                    {
                        Requests[client2].Text = (request.Name + " join the Chat\n");
                    }


                }
                lock (_lock) Requests[client2] = request;

                if (Requests[client2].Text == "exit")
                {
                    Requests[client2].Text = (request.Name + " Leave the Chat\n");
                    break;
                }
                SendMessage(client2);
            }

            SendMessage(client2);
            lock (_lock) TcpClients.Remove(id);
            client2.Client.Shutdown(SocketShutdown.Both);
            client2.Close();

        }

        public void SendMessage(TcpClient client1)
        {
            TcpClient client3;
            TcpClient client4;
            lock (_lock) client3 = client1;
            lock (_lock) client4 = client1;

            lock (_lock)
            {

                if (request.Type == "message")
                {
                    foreach (TcpClient c in TcpClients.Values)
                    {


                        if (client3 != c)
                        {
                            SenderStrm = c.GetStream();
                            byte[] buffer = Encoding.ASCII.GetBytes(request.Text);
                            SenderStrm.Write(buffer, 0, buffer.Length);

                        }

                    }


                }

                else if (request.Type == "showClients")
                {
                    string ConnectedClients = "Connected Clients:\n";
                    foreach (Request item in Requests.Values)
                    {

                        ConnectedClients += (item.Name + "\n");

                    }
                    SenderStrm = client3.GetStream();
                    byte[] buffer = Encoding.ASCII.GetBytes(ConnectedClients);
                    SenderStrm.Write(buffer, 0, buffer.Length);

                }
                else if (request.Type == "privateChat")
                {
                    foreach (KeyValuePair<TcpClient, Request> item in Requests)
                    {

                        if (item.Value.Name == Requests[client3].Dst)
                        {
                            lock (_lock) client4 = item.Key;
                        }
                        //להוסיף מה קורה אם אין לקוח כזה
                    }
                    if (client4 == client3)
                    {
                        request.Text = "This client is not connected";
                    }
                    SenderStrm = client4.GetStream();
                    byte[] buffer = Encoding.ASCII.GetBytes(request.Name + ": " + request.Text);
                    SenderStrm.Write(buffer, 0, buffer.Length);
                }

            }


        }

    }
}