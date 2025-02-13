using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Outlook;
using Microsoft.Office.Tools.Ribbon;

namespace Demo.UI
{
    public partial class CustomRibbon : IRibbonExtensibility
    {
        private IRibbonUI ribbon;

        //private void Ribbon_Load(object sender, RibbonUIEventArgs e)
        //{
        //    this.ribbon = ribbonUI;     

        //}

        private void replyButton_Click(object sender, RibbonControlEventArgs e)
        {

        }

        public void Ribbon_Load(IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;
        }


        public string GetCustomUI(string RibbonID)
        {
            return @"
            <customUI xmlns='http://schemas.microsoft.com/office/2009/07/customui'>
                <ribbon>
                    <tabs>
                        <tab id='CustomTab' label='My Add-in'>
                            <group id='ReplyGroup' label='Quick Reply'>
                                <button id='ReplyButton' label='Quick Reply' 
                                    onAction='OnReplyButtonClick' 
                                    imageMso='Reply' size='large'/>
                            </group>
                        </tab>
                    </tabs>
                </ribbon>
            </customUI>";
        }

        public void OnReplyButtonClick(IRibbonControl control)
        {
            try
            {
                Microsoft.Office.Interop.Outlook.Application outlookApp = new Microsoft.Office.Interop.Outlook.Application();
                Inspector currentInspector = outlookApp.ActiveInspector();

                if (currentInspector != null)
                {
                    MailItem mailItem = currentInspector.CurrentItem as MailItem;
                    if (mailItem != null)
                    {
                        MailItem replyMail = mailItem.Reply();
                        replyMail.Body = "Hello,\n\nThis is a predefined response.\n\nBest regards,\nYour Name";
                        replyMail.Display();
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
