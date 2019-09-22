using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pmdbs
{
    public partial class PromptForm : MetroFramework.Forms.MetroForm
    {
        public PromptForm(string promptMain, string promptAction)
        {
            InitializeComponent();
            HelperMethods.InvokeOutputLabel("Waiting for user confirmation ...");
            LabelTitle.Text = promptMain;
            LabelMailInfo.Text = "An email containing a verification code has been sent to " + GlobalVarPool.email + ".";
            LabelAction.Text = promptAction;
        }

        private void WindowButtonMinimize_OnClickEvent(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void WindowButtonClose_OnClickEvent(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void AnimatedButtonSubmit_Click(object sender, EventArgs e)
        {
            Submit();
        }

        private void EditFieldCode_EnterKeyPressed(object sender, EventArgs e)
        {
            Submit();
        }

        private void Submit()
        {
            string code = EditFieldCode.TextTextBox;
            if (code.Equals(string.Empty))
            {
                LabelCode.ForeColor = Color.Firebrick;
                LabelCode.Text = "*Enter code (this field is required)";
                return;
            }
            if (!Regex.IsMatch(code, "^[0-9]{6}$"))
            {
                LabelCode.ForeColor = Color.Firebrick;
                LabelCode.Text = "*Enter code (6 digits)";
                return;
            }
            if (!GlobalVarPool.connected)
            {
                CustomException.ThrowNew.NetworkException("Not connected!");
                return;
            }
            if (string.IsNullOrEmpty(GlobalVarPool.promptCommand))
            {
                CustomException.ThrowNew.GenericException("User entered code but command has not been set!");
                return;
            }
            // DEEP COPY SCHEDULED TASKS
            List<AutomatedTaskFramework.Task> scheduledTasks = AutomatedTaskFramework.Tasks.GetAll().ConvertAll(task => new AutomatedTaskFramework.Task(task.TaskType, task.SearchCondition, task.FinishedCondition, task.TaskAction));
            AutomatedTaskFramework.Tasks.Clear();
            switch (GlobalVarPool.promptCommand)
            {
                case "ACTIVATE_ACCOUNT":
                    {
                        AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.Contains, "ACCOUNT_VERIFIED", () => NetworkAdapter.MethodProvider.ActivateAccount(code));
                        AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.In, "ALREADY_LOGGED_IN|LOGIN_SUCCESSFUL", NetworkAdapter.MethodProvider.Login);
                        break;
                    }
                case "CONFIRM_NEW_DEVICE":
                    {
                        AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.Contains, "LOGIN_SUCCESSFUL", () => NetworkAdapter.MethodProvider.ConfirmNewDevice(code));
                        for (int i = 1; i < scheduledTasks.Count; i++)
                        {
                            AutomatedTaskFramework.Tasks.Add(scheduledTasks[i]);
                        }
                        break;
                    }
                case "VERIFY_PASSWORD_CHANGE":
                    {
                        AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.Contains, "PASSWORD_CHANGED", () => NetworkAdapter.MethodProvider.CommitPasswordChange(GlobalVarPool.plainMasterPassword, code));
                        break;
                    }
            }
            AutomatedTaskFramework.Tasks.Execute();
            this.DialogResult = DialogResult.OK;
        }

        private void LinkLabelResendCode_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!GlobalVarPool.connected)
            {
                CustomException.ThrowNew.NetworkException("Not connected.");
                return;
            }
            NetworkAdapter.MethodProvider.ResendCode();
            // TODO: DISPLAY "THE EMAIL HAS BEEN RESEND" NOTIFICATION
        }

        private void PromptForm_Shown(object sender, EventArgs e)
        {
            this.Focus();
        }
    }
}
