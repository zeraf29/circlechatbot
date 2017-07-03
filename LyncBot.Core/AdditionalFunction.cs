using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace LyncBot.Core
{
    public static class AdditionalFunction
    {
        /*단축 URL 생성 함수
        *Parameter : String, 본래 URL
        * 전달된 본래의 URL에 대해 naver api에서 생성한 단축 URL 리턴(String)
        */
        public static string SimpleUrl(string orgUrl)
        {
            string url = "https://openapi.naver.com/v1/util/shorturl"; //Naver API URL
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("X-Naver-Client-Id", "Q96pCi1xiWukmgaxhD_P");
            request.Headers.Add("X-Naver-Client-Secret", "rTwY0sP75h"); //Key 값 세팅
            request.Method = "POST";
            string query = orgUrl;
            byte[] byteDataParams = Encoding.UTF8.GetBytes("url=" + query);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteDataParams.Length;
            Stream st = request.GetRequestStream();
            st.Write(byteDataParams, 0, byteDataParams.Length);
            st.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            string text = reader.ReadToEnd();
            stream.Close();
            response.Close();
            reader.Close();
            JObject json = JObject.Parse(text);//JSon 형식으로 전달받은 Stream 값을 Json으로 변환
            return json["result"]["url"].ToString();
        }
        /*HTML 태그 제거 함수
        *Parameter: String, HTML 태그
        *정규식을 통해 HTML 태그가 제거된 문자열을 Return
        */
        public static string remove_html_tag(string html_str)
        {
            // 정규표현을 이용한 HTML태그 삭제
            return Regex.Replace(html_str, @"[<][a-z|A-Z|/](.|)*?[>]", "");
        }
        /*이스케이프 특수문자 제거 함수
         * Parameter: String, 문자열
         * 이스케이프 처리되어 변형된 특수문자를 본래의 문자로 변환하여 리턴
         */
        public static string ReplaceSpecialString(string str)
        {
            Dictionary<string, string> dicSpStr = new Dictionary<string, string>
            {
                {"&nbsp;", " "}
                ,{"&lt;", "<"}
                ,{"&gt;", ">"}
                ,{"&amp;", "&"}
                ,{"&quot;", "\""}
                ,{"&lsquo;", "'"}
                ,{"&rsquo;", "'"}
                ,{"&middot;", "·"}
                ,{"&#8228;", "·"}
            };
            foreach (KeyValuePair<string, string> spStr in dicSpStr)
            {
                str = str.Replace(spStr.Key, spStr.Value);
            }
            return str;
        }
    }
}
