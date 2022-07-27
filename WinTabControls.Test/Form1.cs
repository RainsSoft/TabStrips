using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinTabControls.Test
{
    public partial class Form1 : Form
    {
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            this.wizardControl1.NextButtonEnabled = true;
            this.wizardControl1.NextButtonText = "下一页";
            this.wizardControl1.NextButtonVisible = true;
            //
            new WizardControlTest.WizardControl_TestForm().Show();
            new TestApp.PagedControl_TestForm().Show();
            new TabControlTest.TabControl_TestForm().Show();
        }

        
    }
}
