using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace bot_supreme
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length > 0 && args[0] == "start")
            {
                Application.Run(new Form1(true,false));
            }
            else if (args.Length > 0 && args[0] == "startdebug")
            {
                Application.Run(new Form1(true, true));
            }
//            else
//            {
#if DEBUG
                Application.Run(new Form1(false, true));
#else
                                   Application.Run(new LoginForm());
#endif
//            }

        }
    }
}
