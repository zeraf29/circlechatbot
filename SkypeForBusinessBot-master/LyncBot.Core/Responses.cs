using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml;
using System.Net.Cache;
using System.IO;

using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace LyncBot.Core
{
    public static class Responses
    {
        public static List<string> HiGreetingsResponse(string name)
        {
            var greetings = new List<string> { "hi", "hello", "hey" };
            return AppendName(greetings, name);
        }
        public static List<string> GoodMorningGreetingsResponse(string name)
        {
            var greetings = GetGreeting();
            return AppendName(greetings, name);
        }

        public static List<string> CallResponse()
        {
            return new List<string>
            {
                "I am little busy right now. Can we talk after an hour?",
                "In a meeting",
                "Busy now. Can we talk later?"
            };
        }

        public static List<string> ChannelResponse()
        {
            return new List<string>
            {
                "[링크:채널H] "+AdditionalFunction.SimpleUrl("http://snc.eagleoffice.co.kr/api/branch/common/slo/goSloTarget.mvc?authType=2&destination=http://ch.hanwha.com/slo/linkslo?url=http://ch.hanwha.com/&company_id=418") + "\r\n"
            };
        }

        private static List<string> GetGreeting()
        {
            var afternoon = 12;
            var evening = 16;
            if (DateTime.Now.Hour < afternoon)
                return new List<string> { "Good Morning", "gm", "vgm" };
            else if (DateTime.Now.Hour < evening)
                return new List<string> { "Good Afternoon" };
            else
                return new List<string> { "Good Evening" };
        }

        private static List<string> AppendName(List<string> responses, string name)
        {
            if (!string.IsNullOrEmpty(name))
                responses = responses.Concat(responses.Select(t => t + " " + name)).ToList();
            return responses;
        }
        
        public static List<string> SearchApiResponse(string searchWords)
        {
            Console.WriteLine("검색어: "+searchWords);
            if (string.IsNullOrEmpty(searchWords))
            {
                return new List<string>
                {
                    "검색어를 입력해 주세요."
                };
            }
            string result = string.Empty;
            string temp = string.Empty;
            string wfKor = string.Empty;
            string pop = string.Empty;
            string wdKor = string.Empty;
            string ws = string.Empty;
            string reh = string.Empty;
            //searchWords = "서울";
            string searchQuery = "query=" + searchWords + "&display=10&start=1";
            
            string reqUrl = "https://openapi.naver.com/v1/search/webkr.xml?" + searchQuery;

            HttpWebRequest HttpWRequest = (HttpWebRequest)WebRequest.Create(reqUrl);
            HttpWRequest.Headers.Add("X-Naver-Client-Id", "Q96pCi1xiWukmgaxhD_P"); // 클라이언트 아이디
            HttpWRequest.Headers.Add("X-Naver-Client-Secret", "rTwY0sP75h");       // 클라이언트 시크릿
            HttpWRequest.ContentType = "text/xml";
            HttpWebResponse response = (HttpWebResponse)HttpWRequest.GetResponse();

            result = "<검색어> " + searchWords + "\r\n";
            string status = response.StatusCode.ToString();
            if (status == "OK")
            {
                Console.WriteLine(status);
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string text = reader.ReadToEnd();
                response.Close();
                stream.Close();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(text);
                int number = 1;
                foreach (XmlNode node in doc.SelectNodes("rss/channel/item")){
                    result += "[검색결과"+number+"]" + AdditionalFunction.ReplaceSpecialString(AdditionalFunction.remove_html_tag(node.SelectSingleNode("title").InnerText)) + "\r\n"+"[링크]"+ AdditionalFunction.SimpleUrl(node.SelectSingleNode("link").InnerText) + "\r\n";
                    number++;
                }
            }
            else
            {
                Console.WriteLine("Error 발생=" + status);
            }
            
            return new List<string>
            {
                result
            };
        }
    }
}
