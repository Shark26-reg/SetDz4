using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Dz4;

namespace Dz4
{                      //шаблон  Singleton
    internal class Server
    {
        private static Server _instance;
        private static readonly object _lock = new object();
        private static bool _exitRequested = false;
        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private UdpClient _udpClient;

        
        private Server()
        {
            _udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 16874)); 
        }

        public static Server Instance
        {
            get
            {
                lock (_lock) 
                {
                    if (_instance == null)
                    {
                        _instance = new Server();
                    }
                    return _instance;
                }
            }
        }

        public async Task AcceptMsg()
        {
            Console.WriteLine("Сервер ожидает сообщения. Для завершения нажмите клавишу exit...");

            Task exitTask = Task.Run(() =>
            {
                Console.ReadKey();
                _exitRequested = true;
                _cancellationTokenSource.Cancel();
            });

            while (!_exitRequested)
            {
                try
                {
                    UdpReceiveResult data = await _udpClient.ReceiveAsync();

                    CancellationToken token = _cancellationTokenSource.Token;
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    await ProcessMessage(data, token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
            await exitTask;
        }
        private async Task ProcessMessage(UdpReceiveResult data, CancellationToken token)
        {
            string dataAsString = Encoding.UTF8.GetString(data.Buffer);
            Message msg = Message.FromJson(dataAsString);

            Console.WriteLine(msg.ToString());
            Message responseMsg = new Message("Server", "Message accepted on server!");
            string responseMsgJson = responseMsg.ToJson();
            byte[] responseData = Encoding.UTF8.GetBytes(responseMsgJson);

            await _udpClient.SendAsync(responseData, responseData.Length, data.RemoteEndPoint);
        }
    }

}
