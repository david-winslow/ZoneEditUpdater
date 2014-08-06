using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace ZoneEditUpdater
{
    public class DomainUpdate
    {
        public string Domain { get; set; }
        public string User { get; set; }
        public string Pwd { get; set; }


        public DomainUpdate(string domain,string user,string pwd)
        {
            Domain = domain;
            User = user;
            Pwd = pwd;
        }

        private static DomainUpdate Parse(string arg)
        {
            
            var args = arg.Split('|');
            if(args.Length != 3) throw new ArgumentException("args must be in the format domain|user|pwd");
            return new DomainUpdate(args[0],args[1],args[2]);
        }

        public static IEnumerable<DomainUpdate> Parse(string[] args)
        {
           return from a in args
            select Parse(a);
        }
    }
}