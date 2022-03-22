using System;
using System.IO;
using System.Threading;
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
            //http://www.wx-now.com/weather/metardecode

            rchtxtbx_results.Clear();
            rchtxtbx_output.Clear();

            string[] MetarWords = txtbx_metar_input.Text.Split(new[] { " " }, StringSplitOptions.None);

            
            foreach (string s in MetarWords)
            {
                rchtxtbx_output.AppendText(s + "\r");
            }

            //METAR EGMJ 280925Z AUTO 21009G19KT 060V130 5000 -RA FEW007 BKN014CB BKN017 02/M01 Q1001 BECMG 6000


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
            //Issuance Time DDHHMMz(UTC)
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

            //Wind
            //21009G19KT 060V130 = Wind
            rchtxtbx_results.AppendText("Mean wind direction = " + MetarWords[count].Substring(0,3) + "°\r"+
                                        "Variable between " + MetarWords[count+1].Substring(0, 3) + "° and" +
                                        MetarWords[count+1].Substring(4, 3) + "°\r" +
                                        "Average wind speed = " + MetarWords[count].Substring(3, 2) + MetarWords[count].Substring(8, 2).ToLower() + "\r" +
                                        "Gusting to " + MetarWords[count].Substring(6, 2) + MetarWords[count].Substring(8, 2).ToLower() + "\r" +
                                                      "\r\r");
            // G = Gusts
            // VRB = Variable direction
            // V = varying from 060 to 130 

            // KT = knots, KMH = kilometers / hour, MPS = meters / second

            // 00000KT represents a calm wind
            // VRB004KT (variable direction blowing at four knots).






            // Horizontal Visibility
            //5000






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
