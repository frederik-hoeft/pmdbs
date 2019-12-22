using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pmdbs
{
    public partial class TEST_NOTIFICATION : LunaTransparencyDialogBase
    {
        public TEST_NOTIFICATION()
        {
            InitializeComponent();
            Load += TEST_NOTIFICATION_Load;
        }

        private void TEST_NOTIFICATION_Load(object sender, EventArgs e)
        {
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
            this.Left = screenWidth - this.Width - 20;
            this.Top = screenHeight - this.Height - 20;
        }
    }
}
