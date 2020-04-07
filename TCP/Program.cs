using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Config
{
    public static class Ini
    {
        public static string
            host = "127.0.0.1";             // localhost - петля на локальную машину
        public static IPAddress
            IP = IPAddress.Parse(host);     // определение IP адресса
        public static int
            port = 8080;                    // порт - способ разделения программ на одном хосте
        public static IPEndPoint
            endPoint = new IPEndPoint(IP, port);  // Пункт назначения - хост : порт
        public static System.Text.Encoding        // Транспортная кодировка
            communicationEncoding = System.Text.Encoding.Unicode;

    }
}

namespace Server
{

    class Program
    {   // Заготавливаем TCP сокет серверный мезанизм
        static Socket serverSocket = new Socket(
                AddressFamily.InterNetwork,  // IPv4 адрессация
                SocketType.Stream,           // Потоковый (двусторонний) сокет
                ProtocolType.Tcp             // Transport control protocol
        );

        static void StartServer()
        {

            Socket request = null;  // сокет  для установленного соединения через него читаем и пишем
            try 
            {
                serverSocket.Bind(Config.Ini.endPoint);  // привязываем сокет к пункту назначения
                serverSocket.Listen(100);  //слушаем порт, допускаем очередь из 100 запросов (101-й 
                                           // получит сообщение сервер занят)
                Console.WriteLine("Сервер успешно запущен....");

                byte[] buf = new byte[256];  // буфер для считаных данных
                string str;                  // строка для перевода байт в символы
                int n;                       // кол-во символов в буфере
                
                while (Console.KeyAvailable == false)  // постоянно считываем данные с порта true 
                {
                    // ожидаем запрос и создаем сокет для соединения - обработки запроса
                    request = serverSocket.Accept();  // зависание потока до прихода запроса от клиента
                    str = "";
                    // начинаем прием данных
                    do
                    {
                        n = request.Receive(buf);  // получаем данные в байт-буфер
                        // переводим байты в символы и дописываем к строке
                        // чтобы не зачищать байт-буфер указываем кол-во принятых байт n
                        str += Config.Ini.communicationEncoding.GetString(buf, 0, n);
                    } while (request.Available > 0);  // пока есть доступные байты

                    // Выводим данные о полученном запросе
                    string receiveTime = DateTime.Now.ToString();
                    Console.WriteLine(receiveTime + " -  " + str);

                    // отправляем ответ
                    // готовим сообщение
                    string msg = "Cообщение успешно доставлено " + receiveTime;
                    // переводим в байты
                    buf = Config.Ini.communicationEncoding.GetBytes(msg);
                    request.Send(buf);

                    // закрываем сокет
                    request.Shutdown(SocketShutdown.Both);
                    request.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Попытка запуска сервера ... ");
            Thread srv = new Thread(StartServer);
            srv.Start();
            Console.ReadLine();
            Console.WriteLine("Попытка останова сервера ... ");
            serverSocket.Close(); // глобальная переменная 
        }
    }
}
