using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZoneEditUpdater
{
    public class X
    {
        private static string _ipFilePath;

        public static void Main(string[] args)
        {

            if (!File.Exists("log.txt"))
            {
                var fileStream = File.Create("log.txt");
                fileStream.Close();
            }

            var dynIp = IpGetDynamicAddress();
            if (!CheckIfDynamicIpHasChanged(dynIp))
            {
                log(string.Format("{0} the ip ({1}) has not changed", DateTime.Now, dynIp));
                return;
            }
            
            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
            {
                string domain = key;
                string userName = ConfigurationManager.AppSettings[key].Split('|')[0];
                string pwd = ConfigurationManager.AppSettings[key].Split('|')[1];
                UpdateIpAddressForZoneEdit(dynIp, domain, userName, pwd);
               
            }
            using (var streamWriter = new StreamWriter(File.Open(_ipFilePath, FileMode.Truncate)))
            {
                streamWriter.Write(dynIp);
                streamWriter.Flush();
            }
        }


        private static void UpdateIpAddressForZoneEdit(IPAddress dynIp, string domain, string userName, string pwd)
        {
        
            var url = string.Format("https://dynamic.zoneedit.com/auth/dynamic.html?host={0}&dnsto={1}",domain,dynIp);
            var webRequest = (HttpWebRequest) WebRequest.Create(url);
            webRequest.Method = "GET";
            webRequest.ContentType = "application/json";
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:28.0) Gecko/20100101 Firefox/28.0";
            string autorization = string.Format("{0}:{1}", userName, pwd);
            byte[] binaryAuthorization = System.Text.Encoding.UTF8.GetBytes(autorization);
            autorization = Convert.ToBase64String(binaryAuthorization);
            autorization = "Basic " + autorization;
            webRequest.Headers.Add("AUTHORIZATION", autorization);
            var webResponse = (HttpWebResponse) webRequest.GetResponse();
            if (webResponse.StatusCode != HttpStatusCode.OK) log(webResponse.Headers.ToString());
            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
            {
                string s = reader.ReadToEnd();
                log(s);
                reader.Close();
            }
        }

        private static void log(string logEntry)
        {
            var logFile = "log.txt";
            using (var streamWriter = new StreamWriter(File.Open(logFile,FileMode.Append)))
            {
               streamWriter.WriteLine(logEntry);
               streamWriter.Flush();
            }
        }

        private static bool CheckIfDynamicIpHasChanged(IPAddress dynIp)
        {
            _ipFilePath = "ip.txt";
            if (!File.Exists(_ipFilePath))
            {
                using (var streamWriter = new StreamWriter(File.OpenWrite(_ipFilePath)))
                {
                    streamWriter.Write(dynIp);
                    streamWriter.Flush();
                }
                return true;
            }
            var streamReader = File.OpenText(_ipFilePath);
            var ip = streamReader.ReadToEnd();
            streamReader.Close();
            return (!dynIp.Equals(IPAddress.Parse(ip)));
            
        }

        private static IPAddress IpGetDynamicAddress()
        {
            var webResponse1 = WebRequest.Create("http://checkip.dyndns.org").GetResponse();
            var input = new StreamReader(webResponse1.GetResponseStream()).ReadToEnd();

            Match match = Regex.Match(input, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
            var dynIp = IPAddress.Parse(match.Value);
            return dynIp;
        }
    }
}