using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace EuroPull.Service
{
    public partial class EuroPullService : ServiceBase
    {

        private Timer timer;
        private bool timerTaskSuccess;

        public EuroPullService()
        {
            InitializeComponent();
        }
        internal void TestStartUpAndStop(string[] args)
        {
            this.OnStart(args);
            Console.ReadLine();
            this.OnStop();
        }

        protected override void OnStart(string[] args)
        {
            timer = new Timer();
            timer.Interval = 300000;
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = false;
            timer.Start();

            timerTaskSuccess = false;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                timerTaskSuccess = true;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if(timerTaskSuccess)
                {
                    timer.Start();
                }
            }
        }

        protected override void OnStop()
        {
            try
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            } catch(Exception ex)
            {

            }
        }
    }
}
