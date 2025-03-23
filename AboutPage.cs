using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Don_t_show_my_mouse_to_teacher
{
    public partial class AboutPage : Form
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void GitHub_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            VisitLink();
        }
        private void VisitLink()
        {
            linkLabel1.LinkVisited = true;
            Process.Start(new ProcessStartInfo("https://github.com/zyhfunny") { UseShellExecute = true });
        }
    }
}
