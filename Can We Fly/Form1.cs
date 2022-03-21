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

namespace Can_We_Fly
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
            lbl_version.Text = "v" + System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
        }
        private void btn_exit_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void btn_metar_workout_Click(object sender, EventArgs e)
        {

        }


    }
}
