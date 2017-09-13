using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace CaptchaSolver
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main(string[] arg)
        {
            if (arg.Length == 0 )
                return;
            try
            {
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,ProtocolType.Udp);
                IPAddress broadcast = IPAddress.Parse("127.0.0.1");
                byte[] sendbuf = Encoding.ASCII.GetBytes(GetSolvedCaptcha());
                IPEndPoint ep = new IPEndPoint(broadcast, int.Parse(arg[0]));
                s.SendTo(sendbuf, ep);
                Application.Exit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
              
            }
   
        }

        public static string GetSolvedCaptcha()
        {

            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            var settings = new ChromeOptions();
            ChromeDriver driver = new ChromeDriver(driverService, settings);
            try
            {
                driver.Navigate().GoToUrl("https://www.supremenewyork.com/about");
                IJavaScriptExecutor js = driver as IJavaScriptExecutor;
                string title = (string) js.ExecuteScript(CaptchaSolver.Properties.Resources.Script);
                while (driver.FindElementById("g-recaptcha-response").GetAttribute("value").Length < 5)
                {
                    Application.DoEvents();
                }
                return driver.FindElementById("g-recaptcha-response").GetAttribute("value");

            }
            catch (Exception exception)
            {

            }
            finally
            {
                try {   driver.Close(); }   catch { }
               
            }
            return null;
        }
    }
}
