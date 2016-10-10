using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

namespace ClashRoyale_NetworkAnalyser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static string hostname = "gamea.clashofclans.com";
        public static int port = 9339;
        public static NetworkAnalyserViewModel Context { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Context = new NetworkAnalyserViewModel();
            this.DataContext = Context;

            App.ProxyThread = new Thread(this.InitiateProxy);
            App.ProxyThread.Name = "Proxy Cycle";
            App.ProxyThread.Start();

           
        }

        private void InitiateProxy()
        {
            try
            {
                Server server = new Server(port);
                server.StartServer();
            }
            catch (ThreadAbortException)
            {
                //ignore
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.ProxyThread.Abort();
            Application.Current.Shutdown(0);
        }

        private void Scroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange == 0)
            {   // Content unchanged : user scroll event
                if (Scroller.VerticalOffset == Scroller.ScrollableHeight)
                {   // Scroll bar is in bottom
                    // Set autoscroll mode
                    AutoScroll = true;
                }
                else
                {   // Scroll bar isn't in bottom
                    // Unset autoscroll mode
                    AutoScroll = false;
                }
            }

            // Content scroll event : autoscroll eventually
            if (AutoScroll && e.ExtentHeightChange != 0)
            {   // Content changed and autoscroll mode set
                // Autoscroll
                Scroller.ScrollToVerticalOffset(Scroller.ExtentHeight);
            }
        }

        public bool AutoScroll { get; set; }

        private void ListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Scroller.ScrollToVerticalOffset(Scroller.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void CommitOnEnterTB_OnKeyDown(object sender, KeyEventArgs e)
        {
            // Update value on enter press.
            if (e.Key == Key.Enter)
            {
                TextBox tBox = (TextBox)sender;
                DependencyProperty prop = TextBox.TextProperty;

                BindingExpression binding = BindingOperations.GetBindingExpression(tBox, prop);
                if (binding != null) { binding.UpdateSource(); }
            }
        }

        private void Button_Click_ReloadDefinitions(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dialogResult = MessageBox.Show("This will result in all of the definitions being reloaded. Proceed ?","WARNING",MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes)
            {
                Server.ReloadDefinitions();
            }
        }
    }
}
