using System.Net;
using System.Net.Sockets;
using System.Text;

namespace VP.Common.Helpers
{
    //todo 注释
    public static class SocketHelper
    {
        public static int FindFreePort()
        {
            int result = 0;
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                var localEP = new IPEndPoint(IPAddress.Any, 0);
                socket.Bind(localEP);
                localEP = (IPEndPoint)socket.LocalEndPoint!;
                result = localEP!.Port;
            }
            finally
            {
                socket.Close();
            }

            return result;
        }
        public static Socket CreateServer(IPEndPoint iPEndPoint)
        {
            Socket socketServer = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketServer.Bind(iPEndPoint);
            socketServer.Listen();
            return socketServer;
        }

        public static Socket CreateClient(IPEndPoint iPEndPoint)
        {
            Socket sender = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sender.Connect(iPEndPoint);
            return sender;
        }

        public static async Task<string> RequestAndReceiveAsync(Socket socket, string message)
        {
            await RequestAsync(socket, message);
            var ret = await ReceiveAsync(socket);
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            return ret;
        }

        public static async Task<int> RequestAsync(Socket socket, string message)
        {
            return await socket.SendAsync(Encoding.UTF8.GetBytes(message), SocketFlags.None);
        }

        public static async Task<string> ReceiveAsync(Socket socket)
        {
            int tryReadCount = 0;
            while (!socket.Poll(new TimeSpan(0, 0, 3), SelectMode.SelectRead))
            {
                tryReadCount++;
                if (tryReadCount>10)
                    throw new TimeoutException();
            }
            byte[] buffer = new byte[4096];
            byte[] receivedData = Array.Empty<byte>();
            int count;
            do
            {
                Array.Resize(ref receivedData, receivedData.Length + buffer.Length);
                count =await socket.ReceiveAsync(buffer, SocketFlags.None);
                Array.Copy(buffer, 0, receivedData, receivedData.Length - buffer.Length, count);
            } while (count == buffer.Length);
            receivedData = receivedData.Take(receivedData.Length-buffer.Length+count).ToArray();
            return Encoding.UTF8.GetString(receivedData, 0, receivedData.Length);
        }

        public static bool IsServerConnectable(IPEndPoint iPEndPoint, TimeSpan timeout)
        {
            try
            {
                using var client = new TcpClient();
                var result = client.BeginConnect(iPEndPoint.Address, iPEndPoint.Port, null, null);
                var success = result.AsyncWaitHandle.WaitOne(timeout);
                if (!success)
                    return false;
                client.EndConnect(result);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
