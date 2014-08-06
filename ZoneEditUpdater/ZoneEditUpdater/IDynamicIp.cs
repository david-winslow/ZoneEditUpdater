using System.Net;

namespace ZoneEditUpdater
{
    public interface IDynamicIp
    {

        IPAddress IpAddress { get; }
        IPAddress OldIpAddress { get; }
        bool HasChanged();
    }
}