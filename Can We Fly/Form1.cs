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

            lbl_date_time.Text = DateTime.Now.ToString("HH:mm:ss\rdd MMMM yyyy");

            if (File.Exists("metar_data.txt"))
            {
                //Add our test text file to the combobox.
                string[] lineOfContents = File.ReadAllLines("metar_data.txt");
                foreach (var line in lineOfContents)
                {
                    cmbobox_metar_data.Items.Add(line);
                }

                cmbobox_metar_data.SelectedIndex = 0;
            }

            // put user text box in focus
            txtbx_metar_input.Visible = false;
            cmbobox_metar_data.Visible = true;
        }

        private void chkbx_user_data_CheckedChanged(object sender, EventArgs e)
        {
            if (chkbx_user_data.Checked)
            {
                txtbx_metar_input.Visible = true;
                cmbobox_metar_data.Visible = false;
            }
            else
            {
                txtbx_metar_input.Visible = false;
                cmbobox_metar_data.Visible = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lbl_date_time.Text = DateTime.Now.ToString("HH:mm:ss\rdd MMMM yyyy");
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_metar_workout_Click(object sender, EventArgs e)
        {
            int count = 0;
            string[] MetarWords = new []{""};
            //http://www.wx-now.com/weather/metardecode

            rchtxtbx_results.Clear();
            rchtxtbx_output.Clear();

            if ((!chkbx_user_data.Checked) && (File.Exists("metar_data.txt")))
            {
                MetarWords = cmbobox_metar_data.Text.Split(new[] { " " }, StringSplitOptions.None);
            }
            else
            {
                MetarWords = txtbx_metar_input.Text.Split(new[] { " " }, StringSplitOptions.None);
            }

            

            
            foreach (string s in MetarWords)
            {
                rchtxtbx_output.AppendText(s + "\r");
            }

            //METAR EGMJ 280925Z AUTO 21009G19KT 060V130 5000 -RA FEW007 BKN014CB BKN017 02/M01 Q1001 BECMG 6000


            #region Identification

            ////////////////////////////////////////////////////////////////////////////////////////////////
            /*
             * Message Type               
               ·         METAR: routine weather report               
               ·         SPECI: special weather report, triggered by a weather change               
               ·         AUTO will be first item for ASOS/AWOS generated reports
             */
            if (MetarWords[count].ToUpper() == "METAR")
            {
                rchtxtbx_results.AppendText("METAR = Planned half hourly weather sighting\r\r");
                count++;
            }
            else if (MetarWords[count].ToUpper() == "SPECI")
            {
                rchtxtbx_results.AppendText("SPECI = Intermediate sighting\r\r");
                count++;
            }
            else if (MetarWords[count].ToUpper() == "AUTO")
            {
                rchtxtbx_results.AppendText("ASOS/AWOS generated reports\r\r");
                count++;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            //ICAO Identifier (4-letter)
            //EGMJ = Airport/Weather Station
            rchtxtbx_results.AppendText(MetarWords[count] + " = The ICAO code of the airport / weather station = " + icao_numbers.FindICAOInfo(MetarWords[count]) + "\r\r");
            count++;


            //////////////////////////////////////////////////////////////////////////////////////////
            //Issuance Time DDHHMMZ(UTC)
            //280925Z = Date and time
            string timezone = (MetarWords[count].Substring(6, 1).ToUpper() == "Z") ? timezone = "UTC" : timezone = "Unknown";
            rchtxtbx_results.AppendText("Day of month = " + MetarWords[count].Substring(0, 2) + "\rTime = " + MetarWords[count].Substring(2, 2) + ":" + MetarWords[count].Substring(4, 2)
                                        + "\rTimezone = " + timezone + "\r\r");
            count++;


            //////////////////////////////////////////////////////////////////////////////////////////
            //Report Type
            //AUTO = Report Type
            switch (MetarWords[count].ToUpper())
            {
                case "COR":
                    rchtxtbx_results.AppendText("COR = This observation replaces a previously drawn up report, this report is done by a human.\r\r");
                    break;
                case "AUTO":
                    rchtxtbx_results.AppendText("AUTO = This observation is done automatically and not by a human. Automatic observations are more " +
                                                "limited than manually generated observations.\r\r");
                    break;
                case "NIL":
                    rchtxtbx_results.AppendText("NIL = No data is known\r\r");
                    break;
                default:
                    rchtxtbx_results.AppendText("Unknown\r\r");
                    break;
            }

            count++;

            #endregion

            #region Wind


            ////////////////////////////////////////
            /// Wind direction/speed
            /////////////////////////////////////// 
            
            // Max only given if >= 10KT greater than the mean.VRB = variable. 00000KT = calm.
            // Wind direction is given in degrees true.
            // G = Gusts
            // VRB = Variable direction
            // KT = knots, KMH = kilometers / hour, MPS = meters / second


            if (MetarWords[count].Substring(0, 3).ToUpper() == "VRB")
            {
                //VRB02KT

                rchtxtbx_results.AppendText("Variable Wind with speed averaging " + MetarWords[count].Substring(3, 2) +
                                            MetarWords[count].Substring(5, 2).ToLower() + "\r\r");
            }
            else if (MetarWords[count].Substring(0, 5) == "00000")
            {
                //00000KT

                rchtxtbx_results.AppendText("No Wind therefore conditions are calm \r\r");
            }
            else if (MetarWords[count].Substring(5, 1).ToUpper() == "G")
            {
                // 21009G19KT 

                rchtxtbx_results.AppendText("Mean wind direction = " + MetarWords[count].Substring(0, 3) + "°\r" +
                                            "Average wind speed = " + MetarWords[count].Substring(3, 2) + MetarWords[count].Substring(8, 2).ToLower() + "\r" +
                                            "Gusting to " + MetarWords[count].Substring(6, 2) + MetarWords[count].Substring(8, 2).ToLower() + "\r" +
                                            "\r\r");

            }
            else if (MetarWords[count].Length == 7)
            {
                // 29001KT

                rchtxtbx_results.AppendText("Mean wind direction = " + MetarWords[count].Substring(0, 3) + "°\r" +
                                            "Average wind speed = " + MetarWords[count].Substring(3, 2) + MetarWords[count].Substring(5, 2).ToLower() + "\r" +
                                            "\r\r");
            }
            else
            {
                rchtxtbx_results.AppendText("Wind Unknown");
            }

            count++;

            ///////////////////////////////////////////////////////
            /// Extreme direction variance
            ///
            /// Not always given so you need to check if it is 
            /////////////////////////////////////////////////////// 

            // 060V130
            // V = varying from 060 to 130

            if ((MetarWords[count].Substring(3, 1).ToUpper() == "V") && (MetarWords[count].Length == 7))
            {

                rchtxtbx_results.AppendText("Wind direction variable between " + MetarWords[count].Substring(0, 3) + "° and" +
                                            MetarWords[count].Substring(4, 3) + "°" +
                                           "\r\r");
                count++;
            }



            #endregion


                #region Visibility


                // Horizontal Visibility
                //5000



                #endregion





                /*
                 * Present Weather (Constructed sequentially):               
                   ·         Intensity               
                   ·         Descriptor               
                   ·         Precipitation (Dominant type is listed first if more than one type reported)               
                   ·         Obscuration               
                   ·         Other
                 */
                //-RA






                //FEW007




                //((SkyCover??????))
                //BKN014CB BKN017



                //Terperature/Dewpoint (whole °C) (preceded by M=minus)
                //02/M01



                //Altimeter setting (QNH) and indicator (A=InHg, Q=hPa)
                //Q1001



                //Trend Forecast (2 hours from time of observation) 
                //BECMG




                //6000



            }


    }
}
