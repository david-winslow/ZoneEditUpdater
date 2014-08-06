using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using log4net;
using log4net.Core;

namespace ZoneEditUpdater
{
    public class ZoneEdit:IZoneEditUpdater
    {
        public DomainUpdateResult PerformUpdate(DomainUpdate domain, IPAddress ipAddress)
        {
            DomainUpdateResult result = new DomainUpdateResult();
            var webResponse = UpdateZoneEdit(domain, ipAddress);
            result.Status = webResponse.StatusCode;
            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
            {
                string s = reader.ReadToEnd();
                result.Message = s;
                reader.Close();
            }
            return result;
        }

        private static HttpWebResponse UpdateZoneEdit(DomainUpdate domain, IPAddress ipAddress)
        {
            var url = string.Format("https://dynamic.zoneedit.com/auth/dynamic.html?host={0}&dnsto={1}", domain, ipAddress);
            var webRequest = (HttpWebRequest) WebRequest.Create(url);
            webRequest.Method = "GET";
            webRequest.ContentType = "application/json";
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:28.0) Gecko/20100101 Firefox/28.0";
            string autorization = string.Format("{0}:{1}", domain.User, domain.Pwd);
            byte[] binaryAuthorization = System.Text.Encoding.UTF8.GetBytes(autorization);
            autorization = Convert.ToBase64String(binaryAuthorization);
            autorization = "Basic " + autorization;
            webRequest.Headers.Add("AUTHORIZATION", autorization);
            var webResponse = (HttpWebResponse) webRequest.GetResponse();
            return webResponse;
        }
    }


    public class DynamicIP : IDynamicIp
    {
        private readonly ILog _log;

      

        public DynamicIP(ILog log)
        {
            _log = log;
        }

        public IPAddress IpAddress { get; private set; }
        public IPAddress OldIpAddress { get; private set; }
        public bool HasChanged()
        {
            IpAddress = Fetch();
            bool hasChanged = !IpAddress.Equals(OldIpAddress);
            if(hasChanged)
                _log.Info(string.Format("IP changed to {0}", IpAddress));
            OldIpAddress = IpAddress;
            
            return hasChanged;
        }


        private static IPAddress Fetch()
        {
            var webResponse1 = WebRequest.Create("http://checkip.dyndns.org").GetResponse();
            var input = new StreamReader(webResponse1.GetResponseStream()).ReadToEnd();

            Match match = Regex.Match(input, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
            var dynIp = IPAddress.Parse(match.Value);
            return dynIp;
        }
    }
}