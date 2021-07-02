using System;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;

namespace getsmscode
{
    class Program
    {
        // Отображение заголовка в консоли по умолчанию
        private static void HeaderView()
        {
            Console.WriteLine("");
            Console.WriteLine("<--- Обмен информацией с API сервиса GetSMSCode.com --->");
            Console.WriteLine("");
            Console.WriteLine("Выберите действие (нажмите соответсвующую цифру на клавиатуре): ");
            Console.WriteLine("1. Получить информацию об аккаунте");
            Console.WriteLine("2. Получить активный номер телефона");
            Console.WriteLine("3. Получить список СМС активного номера");
            Console.WriteLine("4. Выход из программы");
            Console.WriteLine("");
        }

        // Функция реализующая задержку при получении СМС
        private static void DisplayCountDown(string Message)
        {
            for (int i = 11; i >= 0; --i)
            {
                int l = Console.CursorLeft;
                int t = Console.CursorTop;
                Console.CursorLeft = 0;
                Console.CursorTop = 0;
                Console.Write(Message + ": {0}    ",  i);
                Console.CursorLeft = l;
                Console.CursorTop = t;
                Thread.Sleep(1000);
            }
        }

        // Функция для получения данных из API
        private static string GetSummaryInfo(string URL, string Data)
        {
            // Создание Web-запроса
            System.Net.WebRequest req = System.Net.WebRequest.Create(URL);
            req.Method = "POST";
            req.Timeout = 100000;
            req.ContentType = "application/x-www-form-urlencoded";
            byte[] sentData = System.Text.Encoding.UTF8.GetBytes(Data);
            req.ContentLength = sentData.Length;

            // Обработка потока
            Stream sendStream = req.GetRequestStream();
            sendStream.Write(sentData, 0, sentData.Length);
            sendStream.Close();
            WebResponse res = req.GetResponse();
            Stream ReceiveStream = res.GetResponseStream();
            StreamReader sr = new StreamReader(ReceiveStream, Encoding.UTF8);
            
            // Кодировка указывается в зависимости от кодировки ответа сервера
            Char[] read = new Char[256];
            int count = sr.Read(read, 0, 256);
            string result = String.Empty;
            while (count > 0)
            {
                String str = new String(read, 0, count);
                result += str;
                count = sr.Read(read, 0, 256);
            }

            // Возвращаем результат запроса
            return result;
        } 
        static void Main(string[] args)
        {
            Console.Clear();
            HeaderView();

            // Данные авторизации
            const string APIURL = "http://api.getsmscode.com/do.php";
            const string username = "vadichka170@yandex.ru";
            const string token = "0d529c6bd1ac9382d887ca2d67f8cefd";
            const string pid = "3"; // PID для WhatsApp
            const string author = "hs3x@outlook.com";

            // Строка параметров
            string param = String.Empty;

            string mobile = String.Empty;
            char choice;

            do
            {
                choice = Console.ReadKey(true).KeyChar;

                string sms = String.Empty;

                Console.Clear();
                HeaderView();
                
                switch(choice)
                {
                    // Получение общей информации
                    case '1':
                        param = "action=login&username=" + username + "&token=" + token;
                        string summary = GetSummaryInfo(APIURL, param);
                        Console.WriteLine("Общая информация: " + summary); 
                        break;
                    
                    // Получение номера телефона
                    case '2':
                        param = "action=getmobile&username=" + username + "&token=" + token + "&pid=" + pid;
                        mobile = GetSummaryInfo(APIURL, param);
                        Console.WriteLine("Активный номер телефона: " + mobile);
                        DisplayCountDown("Для повторного запроса информации необходимо подождать");
                        break; 

                    // Получение СМС
                    case '3':
                        // Получим активный номер телефона WhatsApp
                        /*param = "action=getmobile&username=" + username + "&token=" + token + "&pid=" + pid;
                        mobile = GetSummaryInfo(APIURL, param);
                        DisplayCountDown("Получение активного номера телефона");*/

                        // Список СМС
                        param = "action=getsms&username=" + username + "&token=" + token + "&pid=" + pid + "&mobile=" + mobile + "&author=" + author;
                        sms = GetSummaryInfo(APIURL, param);

                        Console.WriteLine("Полученные СМС для номера " + mobile + " : " + sms);

                        // Чтобы не попасть в blacklist добавим задержку
                        DisplayCountDown("Для повторного запроса информации необходимо подождать");
                        break;
                }
            }
            while (choice!='4');

            Console.WriteLine("Выход из приложения!");
        }

    }
}