using System.Net;

namespace ZoneEditUpdater
{
    public class DomainUpdateResult
    {
        public HttpStatusCode Status;
        public DomainUpdate Domain;
        public IPAddress IpAddress { get; set; }
        public string Message { get; set; }
    }
}