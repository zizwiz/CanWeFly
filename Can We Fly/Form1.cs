using System;
using System.IO;
using System.Windows.Forms;
using Can_We_Fly.metar_items;

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
            tabControl1.SelectedIndex = 2;
            lbl_version.Text = "v" + System.Reflection.Assembly.GetEntryAssembly().GetName().Version;

            //Get the data file from resources and write to file in same dir as the app.
            File.WriteAllText("airport_data.xml", Properties.Resources.airport_data);

        }
        private void btn_exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_metar_workout_Click(object sender, EventArgs e)
        {
            string[] MetarWords = txtbx_metar_input.Text.Split(new[] {" "}, StringSplitOptions.None);

            foreach (string s in MetarWords)
            {

                rchtxtbx_output.AppendText(s + "\r");


            }

            if (MetarWords[0].ToUpper() == "METAR")
            {
                lbl_metar_speci.Text = "METAR = Planned half hourly weather sighting";
            }
            else if (MetarWords[0].ToUpper() == "SPECI")
            {
                lbl_metar_speci.Text = "SPECI = intermediate sighting";
            }
            else
            {
                lbl_metar_speci.Text = "Unknown Report";
            }

            lbl_icao_code.Text = MetarWords[1] + " = The ICAO code of the airport / weather station = " + icao_numbers.FindICAOInfo(MetarWords[1]);




        }

    }
}
