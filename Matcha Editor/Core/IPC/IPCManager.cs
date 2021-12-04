using System;
using System.Net.Sockets;
using System.Configuration;
using System.Text;
using Matcha_Editor.Core.IPC.Command;
using System.Timers;
using System.Diagnostics;
using System.Windows.Input;

namespace Matcha_Editor.Core.IPC
{
    class IPCManager
    {
        private static IPCManager m_Instance;

        public static IPCManager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new IPCManager();

                return m_Instance;
            }
        }

        private TcpClient m_TCPClient;
        private NetworkStream m_NetworkStream;

        public bool HasActiveConnection { get; private set; }

        private Timer m_ConnectionAliveChecker;

        private IPCManager()
        {
            m_ConnectionAliveChecker = new Timer();
            m_ConnectionAliveChecker.Interval = 10000;
            m_ConnectionAliveChecker.Elapsed += (object s, ElapsedEventArgs e) => VerifyConnectionAlive();
            m_ConnectionAliveChecker.Start();
        }

        private void VerifyConnectionAlive()
        {
            try
            {
                if (m_TCPClient == null || m_TCPClient.Client == null || !m_TCPClient.Client.Connected)
                {
                    HasActiveConnection = false;
                }
                else
                {
                    // Detect if client disconnected
                    if (m_TCPClient.Client.Poll(0, SelectMode.SelectRead))
                    {
                        byte[] buff = new byte[1];
                        HasActiveConnection = m_TCPClient.Client.Receive(buff, SocketFlags.Peek) != 0;
                    }
                    else
                    {
                        HasActiveConnection = true;
                    }
                }
            }
            catch
            {
                HasActiveConnection = false;
            }

            CommandManager.InvalidateRequerySuggested();
        }

        public void Initialize()
        {
            Console.WriteLine("Initializing IPC Manager");
            string hostname = ConfigurationManager.AppSettings.Get("EngineHostname");
            int port = int.Parse(ConfigurationManager.AppSettings.Get("EnginePort"));

            m_TCPClient = new TcpClient();
            m_TCPClient.Connect(hostname, port);
            m_NetworkStream = m_TCPClient.GetStream();
            Console.WriteLine("IPC connection with engine established");
            HasActiveConnection = true;
        }

        public void Post(CommandBase command)
        {
            if (!HasActiveConnection)
                return;

            Send(command.ToBytes());
            Console.WriteLine($"Command Sent { command.ToJson() }");
        }

        public string Get(CommandBase command)
        {
            if (!HasActiveConnection)
                return "";

            Post(command);
            return WaitForReply();
        }

        private void Send(byte[] data)
        {
            try
            {
                m_NetworkStream.Write(data, 0, data.Length);
                m_NetworkStream.Write(new byte[] { 0x0 }, 0, 1); // Delimiter (maybe move into command.tobytes?
            }
            catch (Exception)
            {
                VerifyConnectionAlive();
            }
        }

        private string WaitForReply()
        {
            StringBuilder sb = new StringBuilder();
            byte[] data = new byte[2048];
            while (true)
            {
                try
                {
                    int bytesRead = m_NetworkStream.Read(data, 0, data.Length);
                    Console.WriteLine($"Received TCP packet of size {bytesRead}, with data {Encoding.ASCII.GetString(data, 0, bytesRead)}");
                    if (bytesRead == 1 && data[0] == 0)
                        break;
                    sb.Append(Encoding.ASCII.GetString(data, 0, bytesRead));
                }
                catch (Exception)
                {
                    VerifyConnectionAlive();
                }
            }

            return sb.ToString();
        }
    }
}
