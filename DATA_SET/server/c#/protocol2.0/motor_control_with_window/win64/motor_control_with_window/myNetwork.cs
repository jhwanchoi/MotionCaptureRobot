using System;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using System.Runtime.InteropServices;

namespace myNetworks
{
    class myNetwork
    {
        static TcpListener listener;
        // currently we are accepting only one client
        //const int LIMIT = 5; //5 concurrent clients
        static TcpClient client;
        static NetworkStream s;
        static StreamReader sr;
        static StreamWriter sw;
        static BinaryReader bsr;
        static BinaryWriter bsw;

        [Serializable]
        struct Skeleton
        {
            public double No1;
            public double No2;
            public double No3;


            public double No4;
            public double No5;
            public double No6;
            public double No7;
            public double No8;
            public double No9;
            public double No10;
            public double No11;
            public double No12;
        }

        public static void StartServer(int portNum)
        {
            // Socket Server
            //listener = new TcpListener(54321);
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), portNum);
            listener.Start();
            //Console.WriteLine("Server mounted, listening to port 55555");
            Thread t = new Thread(new ThreadStart(Service));
            t.IsBackground = true;
            t.Start();
        }
        public static bool StartClient(string hostIP, int portNum)
        {
            try
            {
                client = new TcpClient(hostIP, portNum);
            }
            catch (Exception e)
            {
                return false;
            }
            if (!client.Connected) return false;
            s = client.GetStream();
            sr = new StreamReader(s);
            sw = new StreamWriter(s);
            bsr = new BinaryReader(s);
            bsw = new BinaryWriter(s);
            sw.AutoFlush = true;
            return true;
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static void CloseClient()
        {
            try
            {
                if (!client.Connected) return;
                sr.Close();
                sw.Close();
                bsr.Close();
                bsw.Close();
                s.Close();
                client.Close();
            }
            catch (Exception e)
            { }
        }

        public static bool SendData(string data)
        {
            //if (!client.Connected)
            //{
            //    System.Console.WriteLine("Server connection interrupted.");
            //return false;
            //}
            try
            {
                sw.WriteLine(data); // no need to add a code for flush because we set the autoflush parameter by true
                return true;
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Server connection interrupted.");
                return false;
            }
            //return true;
        }

        public static void SendData(char[] buffer) // note that char is 2Byte
        {
            if (!client.Connected) return;
            sw.Write(buffer);
            sr.Read();
        }

        public static bool SendData(byte[] buffer) // for 1Byte array sending
        {
            if (!client.Connected) return false;
            try
            {
                bsw.Write(buffer);
                bsw.Flush();
                //bsr.Read();
                return true;
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Server connection interrupted.");
                return false;
            }
        }
        

        public static void Deserialize(byte[] data)
        {
            Skeleton tempInst = new Skeleton();
            int size_skeleton = Marshal.SizeOf(tempInst);
            byte[] skeletonData = new byte[size_skeleton];
            Array.Copy(data, 1, skeletonData, 0, size_skeleton);
            GCHandle handle = GCHandle.Alloc(skeletonData, GCHandleType.Pinned);
            IntPtr buffer = handle.AddrOfPinnedObject();
            tempInst = (Skeleton)Marshal.PtrToStructure(buffer, typeof(Skeleton));
            /*
            textBoxPP1.Text = tempInst.No1.ToString();
            textBoxPP2.Text = tempInst.No2.ToString();
            textBoxPP3.Text = tempInst.No3.ToString();
            textBoxPP4.Text = tempInst.No4.ToString();
            textBoxPP5.Text = tempInst.No5.ToString();
            textBoxPP6.Text = tempInst.No6.ToString();
            textBoxPP7.Text = tempInst.No7.ToString();
            textBoxPP8.Text = tempInst.No8.ToString();
            textBoxPP9.Text = tempInst.No9.ToString();
            textBoxPP10.Text = tempInst.No10.ToString();
            textBoxPP11.Text = tempInst.No11.ToString();
            textBoxPP12.Text = tempInst.No12.ToString();

            trackBar1.Value = (int)tempInst.No1;
            trackBar2.Value = (int)tempInst.No2;
            trackBar3.Value = (int)tempInst.No3;
            trackBar4.Value = (int)tempInst.No4;
            trackBar5.Value = (int)tempInst.No5;
            trackBar6.Value = (int)tempInst.No6;
            trackBar7.Value = (int)tempInst.No7;
            trackBar8.Value = (int)tempInst.No8;
            trackBar9.Value = (int)tempInst.No9;
            trackBar10.Value = (int)tempInst.No10;
            trackBar11.Value = (int)tempInst.No11;
            trackBar12.Value = (int)tempInst.No12;
            */
        }

        public static void Service()
        {
            while (true)
            {
                Socket soc = listener.AcceptSocket();
                soc.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000);
                Console.WriteLine("Connected: {0}", soc.RemoteEndPoint);
                try
                {
                    Stream ss = new NetworkStream(soc);
                    StreamWriter ssw = new StreamWriter(ss);
                    StreamReader ssr = new StreamReader(ss);
                    BinaryReader bssr = new BinaryReader(ss);
                    BinaryWriter bssw = new BinaryWriter(ss);

                    while (true)
                    {
                        string name = ssr.ReadLine();
                        System.Console.WriteLine(name);
                        byte[] buffer = new byte[512];
                        //buffer = bssr.ReadBytes(512);

                        int iReadLength = bssr.Read(buffer, 0, 512);
                        if (iReadLength > 0)
                        {
                            if (iReadLength == (int)buffer[0])
                                Deserialize(buffer);
                            //FaceController.ControlDlg.AffectivaReceived(buffer, iReadLength);
                        }
                        bssw.Write(buffer, 0, iReadLength);
                        bssw.Flush();
                        sw.WriteLine(name);
                        Console.WriteLine("Received: " + name);
                        if (name == "" || name == null) break;
                    }
                    ss.Close();
                    break;
                }
                catch (Exception e)
                {
#if LOG
                        Console.WriteLine(e.Message);
#endif
                }
#if LOG
                        Console.WriteLine("Disconnected: {0}", soc.RemoteEndPoint);
#endif
                soc.Close();
            }
        }
    }
}