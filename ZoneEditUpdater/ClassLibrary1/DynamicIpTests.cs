using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Moq;
using NUnit.Framework;
using Should;
using ZoneEditUpdater;

namespace ClassLibrary1
{

    [TestFixture]
    public class DomainUpdateTests
    {
        [Test]
        public void ShouldParseValidString()
        {
            string[] args = new[] {"d|u|p", "d1|u1|p1"};
            var result = DomainUpdate.Parse(args);
            result.First().Domain.ShouldEqual("d");
            result.First().User.ShouldEqual("u");
            result.First().Pwd.ShouldEqual("p");
            result.Last().Domain.ShouldEqual("d1");
            result.Last().User.ShouldEqual("u1");
            result.Last().Pwd.ShouldEqual("p1");

        }

        [Test,ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowExceptionOnMissingArgs()
        {
            DomainUpdate.Parse(null);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowExceptionInvalidArgs()
        {
            var domainUpdates = DomainUpdate.Parse(new[]{"a","b"});
            domainUpdates.ToList();
        }
    }



    [TestFixture]
    public class DynamicIpTests
    {


        [Test]
        public void ShouldFetchIpFromInternet()
        {
            IDynamicIp dynamicIp = new DynamicIP(new Mock<ILog>().Object);
            dynamicIp.IpAddress.ShouldBeNull();
            dynamicIp.HasChanged().ShouldBeTrue();
            dynamicIp.IpAddress.ShouldNotBeNull();
        }

        [Test]
        public void ShouldSetOldEqualNewWhenFetched()
        {
            IDynamicIp dynamicIp = new DynamicIP(new Mock<ILog>().Object);
            dynamicIp.IpAddress.ShouldBeNull();
            dynamicIp.HasChanged().ShouldBeTrue();
            dynamicIp.IpAddress.ShouldEqual(dynamicIp.OldIpAddress);
        }
    }
}