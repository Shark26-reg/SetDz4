﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dz4
{
    internal class Client
    {
        public static async Task SendMsg(string name)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 16874);
            UdpClient udpClient = new UdpClient();

            while (true)
            {
                Console.Write("Введите сообщение (или 'Exit' для завершения): ");
                string text = Console.ReadLine();
                                
                Message msg = new Message(name, text);
                string responseMsgJs = msg.ToJson();
                byte[] responseData = Encoding.UTF8.GetBytes(responseMsgJs);
                await udpClient.SendAsync(responseData, responseData.Length, ep);

                byte[] answerData = udpClient.Receive(ref ep);
                string answerMsgJs = Encoding.UTF8.GetString(answerData);
                Message answerMsg = Message.FromJson(answerMsgJs);
                Console.WriteLine(answerMsg.ToString());
            }
        }
    }
}
