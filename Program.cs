
using Dz4;
//Структурируйте код клиента и сервера чата, используя знания о шаблонах.


internal class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            await Server.Instance.AcceptMsg();
        }
    }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                await Client.SendMsg(args[0]);
            }
        }
    }
}
