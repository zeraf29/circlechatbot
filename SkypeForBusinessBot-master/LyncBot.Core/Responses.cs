using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;
using System.Net.Cache;

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

        public static List<string> WeatherResponse(string zoneCode)
        {
            if (string.IsNullOrEmpty(zoneCode))
            {
                return new List<string>
                {
                    "지역을 입력해주세요."
                };
            }
            string result = string.Empty;
            string temp = string.Empty;
            string wfKor = string.Empty;
            string pop = string.Empty;
            string wdKor = string.Empty;
            string ws = string.Empty;
            string reh = string.Empty;

            string reqUrl = "http://www.kma.go.kr/wid/queryDFSRSS.jsp?zone=" + zoneCode;

            HttpWebRequest HttpWRequest = (HttpWebRequest)WebRequest.Create(reqUrl);
            HttpWRequest.Proxy = null;
            HttpWRequest.PreAuthenticate = true;
            HttpWRequest.ServicePoint.ConnectionLimit = 32;
            HttpWRequest.Pipelined = true; //
            HttpWRequest.KeepAlive = true; //this is the default

            HttpWRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            HttpWRequest.Headers["Cache-Control"] = "no-cache";
            HttpWRequest.Headers["Pragma"] = "no-cache";

            HttpWRequest.Timeout = 6000;
            HttpWRequest.ContentType = "text/xml";

            System.IO.Stream responseStream = HttpWRequest.GetResponse().GetResponseStream();

            XmlDocument doc = new XmlDocument();
            doc.Load(responseStream);

            XmlNode rootNode = doc.SelectSingleNode("/rss");
            if (rootNode != null)
            {
                XmlNode channelNode = rootNode.SelectSingleNode("channel");
                if (channelNode != null)
                {
                    XmlNode itemNode = channelNode.SelectSingleNode("item");
                    if (itemNode != null)
                    {
                        XmlNode descriptionNode = itemNode.SelectSingleNode("description");
                        if (descriptionNode != null)
                        {
                            XmlNode bodyNode = descriptionNode.SelectSingleNode("body");
                            if (bodyNode != null)
                            {
                                XmlNodeList dataList = bodyNode.SelectNodes("data");
                                foreach (XmlNode dataNode in dataList)
                                {
                                    if (dataNode.Attributes.GetNamedItem("seq").InnerText == "0")
                                    {
                                        XmlNodeList nodeList = dataNode.ChildNodes;
                                        {
                                            if (nodeList != null)
                                            {
                                                foreach (XmlNode node in nodeList)
                                                {
                                                    if (string.Equals(node.Name, "temp")) //현재 온도
                                                    {
                                                        temp = node.InnerText;
                                                    }
                                                    if (string.Equals(node.Name, "wfKor")) //구름
                                                    {
                                                        wfKor = node.InnerText;
                                                    }
                                                    if (string.Equals(node.Name, "pop")) //강수확률
                                                    {
                                                        pop = node.InnerText;
                                                    }
                                                    if (string.Equals(node.Name, "wdKor")) //풍향
                                                    {
                                                        wdKor = node.InnerText;
                                                    }
                                                    if (string.Equals(node.Name, "ws")) //풍속
                                                    {
                                                        ws = node.InnerText;
                                                    }
                                                    if (string.Equals(node.Name, "reh")) //습도
                                                    {
                                                        reh = node.InnerText;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            result = "현재 온도 " + temp + "˚C며, 하늘은 " + wfKor + "이며 강수 확률은 " + pop + "%입니다. " + wdKor + "풍이 " + ws + "m/s로 불고 있으며, 습도는 " + reh + "%입니다.";

            return new List<string>
            {
                result
            };
        }

        private static List<string> AppendName(List<string> responses, string name)
        {
            if (!string.IsNullOrEmpty(name))
                responses = responses.Concat(responses.Select(t => t + " " + name)).ToList();
            return responses;
        }
    }
}
