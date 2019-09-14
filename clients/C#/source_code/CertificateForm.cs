using pmdbs.Properties;
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
    public partial class CertificateForm : MetroFramework.Forms.MetroForm
    {
        public CertificateForm(string domain, CryptoHelper.CertificateInformation cert)
        {
            InitializeComponent();
            labelDomain.Text = domain;
            Initialize(cert);
        }

        private async void Initialize(CryptoHelper.CertificateInformation cert)
        {
            lunaTextPanelIssuedBy.Text = cert.Issuer.Replace(", ", ",\n");
            lunaTextPanelIssuedFor.Text = cert.Subject.Replace(", ", ",\n");
            labelAlgorithm.Text = cert.SignatureAlgorithm;
            labelNotValidAfter.Text = cert.NotValidAfter;
            labelNotValidBefore.Text = cert.NotValidBefore;
            if (cert.IsSelfSigned)
            {
                Task<string> GetTrustedLocal = DataBaseHelper.GetSingleOrDefault(DataBaseHelper.Security.SQLInjectionCheckQuery(new string[] { "SELECT EXISTS (SELECT 1 FROM Tbl_certificates WHERE C_hash = \"", cert.Checksum, "\" AND C_accepted = \"0\" LIMIT 1);" }));
                string trustedLocal = await GetTrustedLocal;
                if (trustedLocal.Equals("1"))
                {
                    cert.Status = "Trusted";
                }
            }
            labelStatusDetails.Text = cert.Status;
            if (cert.Status.Equals("Trusted"))
            {
                pictureBoxStatusMain.Image = Resources.certificate_valid;
                pictureBoxStatusDetails.Image = Resources.confirmed2;
            }
            else if (cert.Status.Equals("Untrusted"))
            {
                pictureBoxStatusMain.Image = Resources.certificate_untrusted;
                pictureBoxStatusDetails.Image = Resources.warning;
            }
            else
            {
                pictureBoxStatusMain.Image = Resources.certificate_invalid;
                pictureBoxStatusDetails.Image = Resources.breach;
            }
            switch (cert.Status)
            {
                case "Trusted":
                    {
                        lunaTextPanelInfo.Text = "This certificate has been signed by a trusted Certification Authority (CA) and is valid. You can therefore be certain that the server really is the computer you think it is.";
                        break;
                    }
                case "Expired":
                    {
                        lunaTextPanelInfo.Text = "This certificate has expired and therefore is not valid anymore. You should report this issue to the server owner as an expired certificate violates the required security specifications.";
                        animatedButtonAccept.Enabled = false;
                        break;
                    }
                case "Invalid":
                    {
                        lunaTextPanelInfo.Text = "This certificate is invalid due to bad configuration. Please report this issue to the server owner.";
                        break;
                    }
                case "Untrusted":
                    {
                        lunaTextPanelInfo.Text = "This certificate is self-signed meaning that you have no guarantee that the server is the computer you think it is. The servers Certificate Checksum is :\n" + cert.Checksum + "\nIf you trust this host hit accept to add the certificate to the cache and carry on connecting.";
                        break;
                    }
            }
        }

        private void windowButtonMinimize_OnClickEvent(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.Close();
            this.Dispose();
        }

        private void windowButtonClose_OnClickEvent(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void CertificateForm_Shown(object sender, EventArgs e)
        {
            lunaTextPanelInfo.Refresh();
            lunaTextPanelIssuedBy.Refresh();
            lunaTextPanelIssuedFor.Refresh();
        }

        private void animatedButtonAccept_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
            this.Dispose();
        }

        private void animatedButtonReject_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
            this.Dispose();
        }
    }
}
