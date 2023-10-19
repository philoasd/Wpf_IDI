using System.Text;
using System.Net.Sockets;
using System.Net;

namespace IOControl
{
    public class TCPSockerServer
    {
        TcpListener server = null;
        Thread listenerThread;
        private volatile bool isRunning = true; // 新增一个标志用来指示服务器是否应该停止

        public TCPSockerServer(string ip, int port)
        {
            // 创建TcpListener对象
            server = new TcpListener(IPAddress.Parse(ip), port);
            listenerThread = new Thread(new ThreadStart(ListenForClients));
            // 开始监听客户端请求
            listenerThread.Start();
        }

        private void ListenForClients()
        {
            server.Start();

            while (isRunning)
            {
                // 这里会阻塞，直到有客户端连接
                TcpClient clinet = null;
                try
                {
                    clinet = server.AcceptTcpClient();
                }
                catch (SocketException ex)
                {
                    // 捕获异常并检查是否由于服务器停止引起的
                    if (!isRunning)
                    {
                        // 服务器已经停止，可以安全退出循环
                        break;
                    }
                    else
                    {
                        // 其他的SocketException，应该处理或记录错误
                        Console.WriteLine("SocketException: " + ex.Message);
                        break;
                    }
                }

                // 客户端连接后，创建一个线程来处理客户端请求
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(clinet);
            }

            //server.Stop();
        }

        private void HandleClientComm(Object clientObj)
        {
            TcpClient tcpClient = (TcpClient)clientObj;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;

            while (true)
            {
                bytesRead = 0;

                try
                {
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch
                {
                    break;
                }

                if (bytesRead == 0)
                {
                    break;
                }

                // 客户端发送的数据存储在message数组中，可以在此处处理数据
                string data = Encoding.UTF8.GetString(message, 0, bytesRead);
                Console.Write("get message :" + data + '\n');

                // 处理完数据后，可以在此处构建响应消息
                string responseMessage = "received message: " + data;

                // 将响应消息转换为字节数组
                byte[] responseBytes = Encoding.ASCII.GetBytes(responseMessage);

                // 发送响应消息给客户端
                clientStream.Write(responseBytes, 0, responseBytes.Length);
                clientStream.Flush();
            }

            tcpClient.Close();
        }

        public void Stop()
        {
            isRunning = false;
        }
    }
}
