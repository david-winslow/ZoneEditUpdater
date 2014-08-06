using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using log4net;
using log4net.Config;

namespace ZoneEditUpdater
{
    public class ZoneEditManager
    {
        private readonly IEnumerable<DomainUpdate> _domainUpdates;
        private readonly IDynamicIp _dynamicIp;
        private readonly IZoneEditUpdater _zoneEditUpdater;

        static ZoneEditManager()
        {
            XmlConfigurator.Configure(typeof(ZoneEditManager).Assembly.GetManifestResourceStream(typeof(ZoneEditManager).Assembly.GetManifestResourceNames().First()));
             _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        private static string _ipFilePath;
        private static ILog _log;

        public ZoneEditManager(IEnumerable<DomainUpdate> domainsToUpdate):this(domainsToUpdate,new DynamicIP(_log), new ZoneEdit())
        {

        }

        public ZoneEditManager(IEnumerable<DomainUpdate> domainUpdates,IDynamicIp dynamicIp,IZoneEditUpdater zoneEditUpdater)
        {
            _domainUpdates = domainUpdates;
            _dynamicIp = dynamicIp;
            _zoneEditUpdater = zoneEditUpdater;
            
        }

        public IEnumerable<DomainUpdateResult> Execute()
        {
            if (_dynamicIp.HasChanged())
             return  from domain in _domainUpdates select _zoneEditUpdater.PerformUpdate(domain, _dynamicIp.IpAddress);
            return new List<DomainUpdateResult>();
        }

        public void Run()
        {
            StringBuilder sb = new StringBuilder();
            _domainUpdates.ToList().ForEach(d => sb.AppendLine(d.Domain));
            _log.Info(string.Format("beginning update for:\n{0}", sb));
            var domainUpdateResults = Execute();
            if (domainUpdateResults.Count() == 0) _log.Info("nothing to update");
            domainUpdateResults.ToList().ForEach( d =>_log.Info(string.Format("result: {0} {1} {2}", d.Domain.Domain, d.Status, d.Message)));
        }
    }
}