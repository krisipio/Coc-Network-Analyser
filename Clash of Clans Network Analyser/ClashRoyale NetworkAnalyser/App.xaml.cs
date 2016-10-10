using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ClashRoyale_NetworkAnalyser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Thread ProxyThread { get; set; }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            ProxyThread.Abort();
            Application.Current.Shutdown(0);
        }
    }
}
