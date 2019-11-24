using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pmdbs
{
    /// <summary>
    /// A generic wrapper class to access all sorts of Window Management functionality.
    /// </summary>
    public static class WindowManager
    {
        /// <summary>
        /// Allows access to a loading screen by providing a set of management functions.
        /// </summary>
        public static class LoadingScreen
        {
            private static ControlCollection Controls = null;
            /// <summary>
            /// The label to pipe status updates to.
            /// </summary>
            public static System.Windows.Forms.Label OutputLabel = null;
            /// <summary>
            /// Indicates which kind of loading event is running.
            /// </summary>
            public static LoadingType LoadingType = LoadingType.DEFAULT;
            /// <summary>
            /// Indicated whether piping to an output label is available.
            /// </summary>
            public static bool OutputAvailable = false;

            private static bool isInitialized = false;

            /// <summary>
            /// Initialized the LoadingScreen class and provides all needed information for the LoadingScreen to operate.
            /// </summary>
            /// <param name="controls">The controls of the loading screen.</param>
            public static void Initialize(ControlCollection controls)
            {
                Controls = controls;
                isInitialized = true;
            }

            /// <summary>
            /// Show the loading screen
            /// </summary>
            /// <param name="outputLabel">The label to pipe status updates to.</param>
            public static void Show(System.Windows.Forms.Label outputLabel)
            {
                if (!isInitialized) throw new NullReferenceException("LoadingScreen requires Controls to operate. Expected ControlCollection but got null");
                OutputLabel = outputLabel;
                OutputAvailable = true;
                Controls.LoadingPanel.ResumeLayout();
                if (Controls.AbortButton != null)
                {
                    Controls.AbortButton.Enabled = true;
                }
                Controls.LoadingSpinner.Start();
                Controls.LoadingPanel.BringToFront();
            }

            /// <summary>
            /// Show the loading screen and ensure thread-safety
            /// </summary>
            public static void InvokeShow()
            {
                if (!isInitialized) throw new NullReferenceException("LoadingScreen requires Controls to operate. Expected ControlCollection but got null");
                Controls.ParentForm.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    Controls.LoadingPanel.ResumeLayout();
                    if (Controls.AbortButton != null)
                    {
                        Controls.AbortButton.Enabled = true;
                    }
                    Controls.LoadingSpinner.Start();
                    Controls.LoadingPanel.BringToFront();
                });
            }

            /// <summary>
            /// Hide the loading screen 
            /// </summary>
            public static void Hide()
            {
                if (!isInitialized) throw new NullReferenceException("LoadingScreen requires Controls to operate. Expected ControlCollection but got null");
                Controls.LoadingPanel.SendToBack();
                OutputAvailable = false;
                Controls.LoadingSpinner.Stop();
                if (Controls.AbortButton != null)
                {
                    Controls.AbortButton.Enabled = false;
                }
                Controls.LoadingPanel.SuspendLayout();
            }

            /// <summary>
            /// Hide the loading screen and ensure thread-safety
            /// </summary>
            public static void InvokeHide()
            {
                if (!isInitialized) throw new NullReferenceException("LoadingScreen requires Controls to operate. Expected ControlCollection but got null");
                Controls.ParentForm.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    Controls.LoadingPanel.SendToBack();
                    OutputAvailable = false;
                    Controls.LoadingSpinner.Stop();
                    if (Controls.AbortButton != null)
                    {
                        Controls.AbortButton.Enabled = false;
                    }
                    Controls.LoadingPanel.SuspendLayout();
                });
            }

            /// <summary>
            /// Updates the status of the loading screen
            /// </summary>
            /// <param name="status">The new status to be shown</param>
            public static void SetStatus(string status)
            {
                if (!isInitialized) throw new NullReferenceException("LoadingScreen requires Controls to operate. Expected ControlCollection but got null");
                if (OutputAvailable)
                {
                    OutputLabel.Text = status;
                }
            }

            /// <summary>
            /// Updates the status of the loading screen and ensures thread-safety
            /// </summary>
            /// <param name="status">The new status to be shown</param>
            public static void InvokeSetStatus(string status)
            {
                if (!isInitialized) throw new NullReferenceException("LoadingScreen requires Controls to operate. Expected ControlCollection but got null");
                if (OutputAvailable)
                {
                    Controls.ParentForm.Invoke((System.Windows.Forms.MethodInvoker)delegate
                    {
                        OutputLabel.Text = status;
                    });
                }
            }

            /// <summary>
            /// Represents the collection of controls that have to be present on a loading screen.
            /// </summary>
            public class ControlCollection
            {
                /// <summary>
                /// Gets the main panel of the loading screen
                /// </summary>
                public readonly System.Windows.Forms.Panel LoadingPanel = null;
                /// <summary>
                /// Gets the spinner of the loading screen
                /// </summary>
                public readonly LunaForms.LunaProgressSpinnerFading LoadingSpinner = null;
                /// <summary>
                /// Gets the parent form of the loading screen
                /// </summary>
                public readonly MetroFramework.Forms.MetroForm ParentForm = null;
                /// <summary>
                /// Gets the abort button of the loading screen
                /// </summary
                public readonly LunaForms.LunaAnimatedButton AbortButton = null;
                /// <summary>
                /// Represents the collection of controls that have to be present on a loading screen.
                /// </summary>
                /// <param name="loadingPanel">The main panel of the loading screen</param>
                /// <param name="loadingSpinner">The spinner of the loading screen</param>
                /// <param name="abortButton">The abort button of the loading screen</param>
                /// <param name="parentForm">The parent form of the loading screen</param>
                public ControlCollection(System.Windows.Forms.Panel loadingPanel, LunaForms.LunaProgressSpinnerFading loadingSpinner, LunaForms.LunaAnimatedButton abortButton, MetroFramework.Forms.MetroForm parentForm)
                {
                    AbortButton = abortButton;
                    LoadingPanel = loadingPanel;
                    LoadingSpinner = loadingSpinner;
                    ParentForm = parentForm;
                }
            }
        }
    }

    /// <summary>
    /// Provides different loading presets.
    /// </summary>
    public enum LoadingType
    {
        /// <summary>
        /// The default preset.
        /// </summary>
        DEFAULT = 0,
        /// <summary>
        /// Preset for Login from settings.
        /// </summary>
        LOGIN = 1,
        /// <summary>
        /// Preset for Register from settings.
        /// </summary>
        REGISTER = 2,
        /// <summary>
        /// Preset for PasswordChange from settings.
        /// </summary>
        PASSWORD_CHANGE = 3
    }
}
