using System.Net;

namespace ZoneEditUpdater
{
    public interface IZoneEditUpdater
    {
        DomainUpdateResult PerformUpdate(DomainUpdate domain, IPAddress ipAddress);
    }
}