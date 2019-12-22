using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gma.UserActivityMonitor;

namespace pmdbs
{
    public abstract partial class LunaTransparencyDialogBase : Form
    {
        private Timer animationTimer = null;
        private Timer timeoutTimer = null;
        private bool isVisible = true;
        private DialogResult result = DialogResult.Abort;
        private IContainer container = new Container();
        private bool forceClose = false;
        private bool timeoutEnabled = true;
        private bool mouseOverForm = false;
        // TODO: Is used when user manually closes the notification.
        private bool manuallClosed = false;
        public LunaTransparencyDialogBase()
        {
            InitializeComponent();
            if (animationTimer == null)
            {
                animationTimer = new Timer(container)
                {
                    Interval = 50
                };
                animationTimer.Tick += AnimationTimer_Tick;
            }
            if (timeoutTimer == null)
            {
                timeoutTimer = new Timer(container)
                {
                    Interval = 5000
                };
                timeoutTimer.Tick += TimeoutTimer_Tick;
            }
            // HookManager.MouseMove += HookManager_MouseMove;
        }

        private void TimeoutTimer_Tick(object sender, EventArgs e)
        {
            Close();
        }

        private void HookManager_MouseMove(object sender, MouseEventArgs e)
        {
            Point formLocation = PointToScreen(new Point(0, 0));
            // check if mouse hovers over form
            if (e.X > formLocation.X && e.X < formLocation.X + Width && e.Y > formLocation.Y && e.Y < formLocation.Y + Height)
            {
                if (!mouseOverForm)
                {
                    mouseOverForm = true;
                    if (timeoutEnabled)
                    {
                        timeoutTimer.Stop();
                    }
                }
            }
            else
            {
                if (mouseOverForm)
                {
                    mouseOverForm = false;
                    if (timeoutEnabled)
                    {
                        timeoutTimer.Start();
                    }
                }
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (mouseOverForm && !manuallClosed)
            {
                animationTimer.Stop();
                Opacity = 1;
            }
            if (isVisible)
            {
                if (Opacity < 1)
                {
                    Opacity += 0.05;
                }
                else
                {
                    animationTimer.Stop();
                }
            }
            else
            {
                if (Opacity > 0)
                {
                    Opacity -= 0.05;
                }
                else
                {
                    animationTimer.Stop();
                    forceClose = true;
                    Close();
                    Dispose();
                }
            }
        }

        private void LunaTransparencyDialogBase_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!forceClose)
            {
                result = DialogResult;
                e.Cancel = true;
                isVisible = false;
                animationTimer.Start();
            }
            else
            {
                DialogResult = result;
            }
        }

        private void LunaTransparencyDialogBase_Load(object sender, EventArgs e)
        {
            Opacity = 0.0;
            isVisible = true;
            animationTimer.Start();
            if (timeoutEnabled)
            {
                timeoutTimer.Start();
            }
        }
    }
}
