using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ParserBaucenter
{
    class Program
    {
        static void Main(string[] args)
        {
            //Парсинг данных с сайта Baucenter.ru

            //Артикул товара(Пример)
            var code = "416001653";

            //Прокси-сервер(через Fiddler Classic)
            var proxy = new WebProxy("127.0.0.1:8888");

            var cookieContainer = new CookieContainer();

            //Post-запрос. Получение адреса данных товара
            PostRequest postRequest = new PostRequest("https://baucenter.ru/");
            postRequest.Data = $"ajax_call=y&INPUT_ID=title-search-input&q={code}&l=2";
            postRequest.Accept = "*/*";
            postRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36";
            postRequest.ContentType = "application/x-www-form-urlencoded";
            postRequest.Referer = "https://baucenter.ru/";
            postRequest.Host = "baucenter.ru";
            postRequest.Proxy = proxy;
            postRequest.Headers.Add("Bx-ajax", "true");
            postRequest.Headers.Add("Origin", "https://baucenter.ru");
            postRequest.Headers.Add("sec-ch-ua", "\" Not A; Brand\";v=\"99\", \"Chromium\";v=\"96\", \"Google Chrome\";v=\"96\"");
            postRequest.Headers.Add("sec-ch-ua-mobile", "?0");
            postRequest.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
            postRequest.Headers.Add("Sec-Fetch-Dest", "empty");
            postRequest.Headers.Add("Sec-Fetch-Mode", "cors");
            postRequest.Headers.Add("Sec-Fetch-Site", "same-origin");

            postRequest.Run(cookieContainer);

            // поиск в HTML-коде адрес данных товара
            var strStart = postRequest.Response.IndexOf("search-result-group search-result-product");
            strStart = postRequest.Response.IndexOf("<a href=", strStart) + 9;
            var strEnd = postRequest.Response.IndexOf("\"", strStart);
            var getPath = postRequest.Response.Substring(strStart, strEnd - strStart);


            //Get-запрос. Получение данных товара
            var getRequest = new GetRequest($"https://baucenter.ru{getPath}");
            getRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            getRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
            getRequest.Referer = "https://baucenter.ru/";
            getRequest.Host = "baucenter.ru";
            getRequest.Proxy = proxy;
            getRequest.Run(cookieContainer);

            //Запись данных в класс Card и вывод в консоли
            var card = new Card();
            card.Parse(getRequest.Response);

            Console.WriteLine($"Название={card.Title}");
            Console.WriteLine($"Цена={card.Price}");

            //Логика добавления в БД..

            Console.ReadKey();
        }
    }
}
