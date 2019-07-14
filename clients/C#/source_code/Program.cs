using pmdbs.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pmdbs
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDPIAware();
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BreachForm("2013-10-04", "Adobe", Resources.breach, new string[] { "Dates of birth", "Email addresses", "Genders", "Geographic locations", "IP addresses", "Races", "Relationship statuses", "Sexual orientations", "Spoken languages", "Usernames" }, "In October 2013, 153 million Adobe accounts were breached with each containing an internal ID, username, email, <em>encrypted</em> password and a password hint in plain text. The password cryptography was poorly done and <a href=\"http://stricture-group.com/files/adobe-top100.txt\" target=\"_blank\" rel=\"noopener\">many were quickly resolved back to plain text</a>. The unencrypted hints also <a href=\"http://www.troyhunt.com/2013/11/adobe-credentials-and-serious.html\" target=\"_blank\" rel=\"noopener\">disclosed much about the passwords</a> adding further to the risk that hundreds of millions of Adobe customers already faced.", 152445165, true, "adobe.com"));
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}
