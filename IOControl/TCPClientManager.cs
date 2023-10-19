using System.Net.Sockets;
using System.Text;

namespace IOControl
{
    public class TCPClientManager
    {
        private TcpClient _TcpClient;
        private NetworkStream _NetworkStream;

        /// <summary>
        /// 连接tcp
        /// </summary>
        /// <param name="serverIp"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public async Task ConnectedAsync(string serverIp, int port)
        {
            _TcpClient = new TcpClient();
            await _TcpClient.ConnectAsync(serverIp, port);
            _NetworkStream = _TcpClient.GetStream();
        }

        /// <summary>
        /// 断开tcp连接
        /// </summary>
        public void CloseConnected()
        {
            if (_NetworkStream != null)
            {
                _NetworkStream.Close();
            }
            if (_TcpClient != null)
            {
                _TcpClient.Close();
            }
        }

        /// <summary>
        /// 发送消息并等待返回结果
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<string> SendAndReceiveAsync(string data)
        {
            try
            {
                await SendDataAsync(data + '\r');
                return await ReceiveDataAsync();
            }
            catch (Exception ex)
            {
                LoggerManager.LogError($"Cannot send/received tcp data, because {ex}");
                return "";
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task SendDataAsync(string data)
        {
            byte[] sendData = Encoding.ASCII.GetBytes(data);
            await _NetworkStream.WriteAsync(sendData, 0, sendData.Length);
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <returns></returns>
        private async Task<string> ReceiveDataAsync()
        {
            byte[] receiveBuffer = new byte[1024];
            int bytesRead = await _NetworkStream.ReadAsync(receiveBuffer, 0, receiveBuffer.Length);
            string receivedData = Encoding.ASCII.GetString(receiveBuffer, 0, bytesRead);
            return receivedData;
        }
    }
}
