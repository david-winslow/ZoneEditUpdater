using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZoneEditUpdater;
using System.Timers;
using Timer = System.Timers.Timer;

namespace ZoneEditUpdaterService
{
    public partial class Service1 : ServiceBase
    {
        private ZoneEditManager _zoneEditManager;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _zoneEditManager = new ZoneEditManager(DomainUpdate.Parse(args));
            // Create a timer with a two second interval.
            Timer aTimer = new Timer(3600000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.Enabled = true;
            aTimer.Start();

        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            _zoneEditManager.Run();
            
        }

        protected override void OnStop()
        {
        }
    }
}
