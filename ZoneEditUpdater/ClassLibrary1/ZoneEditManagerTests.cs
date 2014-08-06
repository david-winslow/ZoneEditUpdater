using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ZoneEditUpdater;
using Match = System.Text.RegularExpressions.Match;

namespace ClassLibrary1
{

    [TestFixture]
    public class ZoneEditManagerTests
    {
        private ZoneEditManager _zoneEditManager;
        private Mock<IZoneEditUpdater> _zoneEditUpdaterMock;
        private Mock<IDynamicIp> _dynamicIpFetcherMock;
        private DomainUpdate _domainUpdate1;
        private IPAddress _ipAddress;

        [SetUp]
        public void SetUp()
        {
            _zoneEditUpdaterMock = new Mock<IZoneEditUpdater>();
            _dynamicIpFetcherMock = new Mock<IDynamicIp>();
            _domainUpdate1 = new DomainUpdate("x", "y", "z");
            _zoneEditManager = new ZoneEditManager(new List<DomainUpdate> { _domainUpdate1},_dynamicIpFetcherMock.Object,_zoneEditUpdaterMock.Object);
            _dynamicIpFetcherMock.Setup(x => x.HasChanged()).Returns(true);
            _ipAddress = IPAddress.Parse("10.0.0.1");
            _dynamicIpFetcherMock.SetupGet(x => x.IpAddress).Returns(_ipAddress);
            _zoneEditUpdaterMock.Setup(x => x.PerformUpdate(_domainUpdate1, _ipAddress)).Returns(new DomainUpdateResult() { Status = HttpStatusCode.OK, Domain = _domainUpdate1, IpAddress = _ipAddress });
        }


       

        [Test]
        public void ShouldPerformDomainUpdateIfIpHasChanged()
        {
            var results = _zoneEditManager.Execute();
            Assert.AreEqual(HttpStatusCode.OK,results.First().Status);
            Assert.AreEqual(_ipAddress, results.First().IpAddress);
        }

        [Test]
        public void ShouldReturnEmptyResultWhenIpHasNotChanged()
        {
            _dynamicIpFetcherMock.Setup(x => x.HasChanged()).Returns(false);
            var results = _zoneEditManager.Execute();
            Assert.AreEqual(0,results.Count());
        }



        
      


    }

 
}
