using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Communications
{
    public class NetSocket : ICommunication
    {
        const char ETX = (char)'\n';
        //start response
        const char STX = (char)'!';

        #region varables
        private readonly int _portNumber;
        private readonly string _hostIp;
        private static Socket? _client;
        public bool IsConnected => _client is null ? false : _client.Connected;

        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        public event ICommunication.MessageReceivedHandler? MessageReceived;

        private string recivedMsg = "";
        #endregion

        private Stopwatch stopwatch;

        public NetSocket()
        {
            _portNumber = 8080;
            _hostIp = "192.168.16.254";
            Connect();
            stopwatch = new Stopwatch();
        }

        public NetSocket(string hostIp, int portNumber)
        {
            _portNumber = portNumber;
            _hostIp = hostIp;
            Connect();
        }

        public void ChangeSetting(ICommunicationSetting setting)
        {
            throw new NotImplementedException();
        }

        public bool Connect()
        {
            // Connect to a remote device.  
            try
            {
                IPAddress ipAddress = IPAddress.Parse(_hostIp);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, _portNumber);

                // Create a TCP/IP socket.  
                _client = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                _client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), _client);
                connectDone.WaitOne(2500);

                //// Create the state object.  
                //StateObject state = new StateObject();
                //state.workSocket = _client;

                //// Begin receiving the data from the remote device.  
                //_client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                //    new AsyncCallback(ReceiveCallback), state);

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return false;
            }
        }

        public string Read()
        {
            return recivedMsg;
        }

        public void Write(string data)
        {
            stopwatch.Restart();
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            try
            {
                if (_client is null) return;
                // Begin sending the data to the remote device.  
                _ = _client.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), _client);

                sendDone.WaitOne(2000);

                Receive(_client);
                receiveDone.WaitOne();
            }
            catch
            {
                Debug.WriteLine("Send data failed.");
            }

        }

        #region private methods
        private void Receive(Socket? client)
        {
            Debug.WriteLine(string.Format("T : {0:0.0} Receive Started. ", stopwatch.ElapsedMilliseconds));

            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = client;

                if (client is null) return;
                // Begin receiving the data from the remote device.  
                _ = client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Debug.WriteLine(string.Format("T : {0:0.0} Receive End. ", stopwatch.ElapsedMilliseconds));

        }
        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket? client = (Socket?)ar.AsyncState;

                if (client is null) return;
                // Complete the connection.  
                client.EndConnect(ar);

                if (client.RemoteEndPoint != null)
                    Debug.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            Debug.WriteLine(string.Format("T : {0:0.0} Receive callback started. ", stopwatch.ElapsedMilliseconds));

            try
            {
                // Retrieve the state object and the client socket
                // from the asynchronous state object.  
                StateObject? state = (StateObject?)ar.AsyncState;

                if (state is null) return;

                Socket? client = state.workSocket;

                if (client is null) return;
                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    string content = state.sb.ToString();
                    if ((content.IndexOf("\r") > -1) || (content.IndexOf("\n") > -1) || (content.IndexOf("\r\n") > -1))
                    {
                        // Signal that all bytes have been received.  
                        state.sb.Clear();
                        receiveDone.Set();
                        RaiseMessageReceived(content);
                    }

                    Debug.WriteLine(string.Format("T : {0:0.0} Receive callback ended. ", stopwatch.ElapsedMilliseconds));

                    // Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                      new AsyncCallback(ReceiveCallback), state);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

        }

        protected virtual void RaiseMessageReceived(string message)
        {
            recivedMsg = message;
            //MessageReceived?.Invoke(this, message);
            ProcessBuffer(Encoding.ASCII.GetBytes(recivedMsg), recivedMsg.Length);
        }

        private void ProcessBuffer(byte[] buffer, int length)
        {

            List<byte>? message = null;
            for (int i = 0; i < length; i++)
            {
                if (buffer[i] == ETX)
                {
                    if (message != null)
                    {
                        message.Add(buffer[i]);
                        MessageReceived?.Invoke(this, Encoding.ASCII.GetString(message.ToArray()));
                        message = null;
                    }
                }
                else if (buffer[i] == STX)
                {
                    message = new List<byte>();
                    message.Add(buffer[i]);
                }
                else if (message != null)
                    message.Add(buffer[i]);
            }
        }
        private void SendCallback(IAsyncResult ar)
        {

            try
            {
                // Retrieve the socket from the state object.  
                Socket? client = (Socket?)ar.AsyncState;

                if (client is null) return;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);

                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


        }
        #endregion
    }
}
