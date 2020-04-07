using System;
using System.Net.Sockets;

// в проекте Client добавить ссылку на проект Server
// назначить запускаемые проекты в решении и несколько проектов запуск

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            // (сервер должен быть запущен и слушать порт)

            Socket clientSocket = null;// переменная для цикла сообщений

            byte[] buf = new byte[256]; // буфер для обмена
            string str = "";//перевод буфера в строку

            // Начинаем писать сообщения
            string msg = String.Empty;
            
            Console.WriteLine("Вводите сообщения. Пустое - выход.");
          
            // запрашивает сообщение в консоли
            Console.Write("message>  ");
            msg = Console.ReadLine();

            while (msg != String.Empty)
            {
                // переводим сообщение в байты 
                buf = Config.Ini.communicationEncoding.GetBytes(msg);
                try
                {// подключаемся к пункту назначения - код как у сервера
                    clientSocket = new Socket(AddressFamily.InterNetwork,
                        SocketType.Stream, ProtocolType.Tcp);
                    // соединяемся...             
                    clientSocket.Connect(Config.Ini.endPoint);
                    // отправляем сообщение
                    clientSocket.Send(buf);
                    // получаем ответ и переводим в строку
                    str = "";
                    do
                    {
                        clientSocket.Receive(buf);
                        str += Config.Ini.communicationEncoding.GetString(buf);
                    } while (clientSocket.Available > 0);
                    // закрываем сокет
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
                catch (Exception ex) { Console.WriteLine("Exception: " + ex.Message); return; }

                //Выводим ответ сервера
                Console.WriteLine("SRV> " + str);

                //Ожидаем новое сообщение
                Console.Write("msg>  ");
                msg = Console.ReadLine();
            }// while (msg != String.Empty)

            Console.WriteLine("Bye...");
        }
    }
}
