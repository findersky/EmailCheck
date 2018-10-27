using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using CheckEmail;

namespace CheckEmailWinForm
{
    public partial class CheckEmail : Form
    {
        public CheckEmail()
        {
            InitializeComponent();
        }
        private delegate void CorssConrolString(string key);
        private delegate void CorssConrolButton(Button btnControl,bool isEnabled);
        private EmailCheckHelper emailCheck = new EmailCheckHelper();
        private Thread th = null;
        private void btnQuery_Click(object sender, EventArgs e)
        {
            string email = this.txtEmailAddress.Text;
            this.richText.Text = "开始验证……";
            this.btnQuery.Enabled = false;
            th = new Thread(delegate(){
                if (emailCheck.checkEmail(email))
                {
                    threadCorssControl(string.Format("Email:{0}存在\r\n", email));
                }
                else
                {
                    threadCorssControl(string.Format("Email:{0}不存在\r\n", email));
                }
                threadCorssControl( emailCheck.ToString());

                threadCorssControlButton(this.btnQuery,true);
            });

            th.Start();
           
        }
        private void threadCorssControl(string key)
        {
            if (this.richText.InvokeRequired)
            {
                this.richText.Invoke(new CorssConrolString(threadCorssControl),key);
            }
            else
            {
                this.richText.Text = key;
            }
        
        }

        private void threadCorssControlButton(Button btnControl,bool isEnabled)
        {
            if (btnControl.InvokeRequired)
            {
                btnControl.Invoke(new CorssConrolButton(threadCorssControlButton),btnControl, isEnabled);
            }
            else
            {
                btnControl.Enabled = isEnabled;
            }

        }
        private void CheckEmail_Load(object sender, EventArgs e)
        {
            //this.txtEmailAddress.Text = "ps_recruiter@baidu.com";
        }
    }
}
