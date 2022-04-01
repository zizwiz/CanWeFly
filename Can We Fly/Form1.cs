﻿using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Can_We_Fly.metar_items;

namespace Can_We_Fly
{
    public partial class Form1 : Form
    {
        TimeZone curTimeZone = TimeZone.CurrentTimeZone;

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

            Text += " : v" + Assembly.GetExecutingAssembly().GetName().Version; // put in the version number
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
            string[] MetarWords = new[] { "" };
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
            rchtxtbx_results.AppendText(MetarWords[count] + " = The ICAO code of the airport / weather station = " +
                                        icao_numbers.FindICAOInfo(MetarWords[count]) + "\r\r");
            count++;


            //////////////////////////////////////////////////////////////////////////////////////////
            //Issuance Time DDHHMMZ(UTC)
            //280925Z = Date and time
            //We convert this to teh local time so you do not need to work out Zulu to local time.
            //////////////////////////////////////////////////////////////////////////////////////////

            string timezone = (MetarWords[count].Substring(6, 1).ToUpper() == "Z")
                ? timezone = "UTC"
                : timezone = "Unknown";
            rchtxtbx_results.AppendText("Day of month = " + MetarWords[count].Substring(0, 2) + "\rTime = " +
                                        MetarWords[count].Substring(2, 2) + ":" + MetarWords[count].Substring(4, 2)
                                        + "\rTimezone = " + timezone + "\r");

            if (timezone == "UTC")
            {
                //Correct Zulu for Local time.
                DateTime convertedDate = DateTime.SpecifyKind(
                    DateTime.Parse(MetarWords[count].Substring(2, 2) + ":" +
                                   MetarWords[count].Substring(4, 2)), //MetarWords[count].Substring(2, 4)),
                    DateTimeKind.Utc);

                rchtxtbx_results.AppendText("Local Time of Metar observation was " +
                                            convertedDate.ToLocalTime().ToString().Substring(11, 5) + "\r");
            }

            //Does country have Daylight Saving on? 
            if (curTimeZone.IsDaylightSavingTime(DateTime.Now))
            {
                rchtxtbx_results.AppendText("Daylight Saving is on.\r\r");
            }
            else
            {
                rchtxtbx_results.AppendText("\r");
            }

            count++;


            //////////////////////////////////////////////////////////////////////////////////////////
            //Report Type
            //AUTO = Report Type
            if (MetarWords[count].Length <= 4)
            {
                switch (MetarWords[count].ToUpper())
                {
                    case "COR":
                        rchtxtbx_results.AppendText(
                            "COR = This observation replaces a previously drawn up report, this report is done by a human.\r\r");
                        break;
                    case "AUTO":
                        rchtxtbx_results.AppendText(
                            "AUTO = This observation is done automatically and not by a human. Automatic observations are more " +
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
            }

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

            double wind = 0;

            if (MetarWords[count].Substring(0, 3).ToUpper() == "VRB")
            {
                //VRB02KT

                rchtxtbx_results.AppendText("Variable Wind with speed averaging " + MetarWords[count].Substring(3, 2) +
                                            MetarWords[count].Substring(5, 2).ToLower() + "\r\r");

                wind = Double.Parse(MetarWords[count].Substring(3, 2));
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
                                            "Average wind speed = " + MetarWords[count].Substring(3, 2) +
                                            MetarWords[count].Substring(8, 2).ToLower() + "\r" +
                                            "Gusting to " + MetarWords[count].Substring(6, 2) +
                                            MetarWords[count].Substring(8, 2).ToLower() + "\r" +
                                            "\r\r");

                wind = Double.Parse(MetarWords[count].Substring(6, 2));

            }
            else if (MetarWords[count].Length == 7)
            {
                // 29001KT

                rchtxtbx_results.AppendText("Mean wind direction = " + MetarWords[count].Substring(0, 3) + "°\r" +
                                            "Average wind speed = " + MetarWords[count].Substring(3, 2) +
                                            MetarWords[count].Substring(5, 2).ToLower() + "\r" +
                                            "\r\r");

                wind = Double.Parse(MetarWords[count].Substring(3, 2));
            }
            else
            {
                rchtxtbx_results.AppendText("Wind Unknown\r\r");
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

                rchtxtbx_results.AppendText("Wind direction variable between " + MetarWords[count].Substring(0, 3) +
                                            "° and " +
                                            MetarWords[count].Substring(4, 3) + "°" +
                                            "\r\r");
                count++;
            }



            #endregion


            #region Visibility


            // Horizontal Visibility
            //5000
            //1200SW


            //0000 = 'less than 50 metres'
            //9999 = 'ten kilometres or more'. N
            //The minimum visibility is also included alongside the prevailing visibility when the visibility in one 
            // direction, which is not the prevailing visibility, is less than 1500 metres or less than 50% of the 
            // prevailing visibility. A direction is also added as one of the eight points of the compass.

            if (MetarWords[count].Length == 4)
            {
                if (MetarWords[count] == "0000")
                {
                    rchtxtbx_results.AppendText("Visibility is less than 50m\r\r");
                }
                else if (MetarWords[count] == "9999")
                {
                    rchtxtbx_results.AppendText("Visibility is greater than 10km\r\r");
                }
                else if (MetarWords[count].Substring(MetarWords[count].Length - 1, 1).ToUpper() == "M")
                {
                    //In visibility, M indicates "less than"
                    rchtxtbx_results.AppendText("Visibility is less than " +
                                                MetarWords[count].Substring(0, MetarWords[count].Length - 1) + "m\r\r");
                }
                else if ((int.Parse(MetarWords[count]) > 0000) && (int.Parse(MetarWords[count]) < 9999))
                {
                    rchtxtbx_results.AppendText("Visibility is " + MetarWords[count] + "m\r\r");
                }
                else
                {
                    rchtxtbx_results.AppendText("Visibility Unknown\r\r");
                }
            }
            else if (MetarWords[count].Length == 6)
            {
                rchtxtbx_results.AppendText("Visibility is " + MetarWords[count].Substring(0, 4) + "m" +
                                            " in the " + MetarWords[count].Substring(5, 2) + " direction.\r\r");
            }
            else
            {
                rchtxtbx_results.AppendText("Visibility Unknown\r\r");
            }

            count++;

            #endregion

            #region present_weather

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Present Weather (Constructed sequentially):               
            ///     Intensity               
            ///     Descriptor               
            ///     Precipitation (Dominant type is listed first if more than one type reported)               
            ///     Obscuration               
            ///     Other
            ///
            /// Up to three groups may be present, constructed by selecting and combining from the above. 
            /// Group omitted if no weather to report.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            //-RA

            int index = 0;
            string qualifier = "Moderate ";
            bool flag = false;


            //if (MetarWords[count].Length > 2)
            //{
            //    string isItClouds = MetarWords[count].Substring(0, 3);
            //} 


            if ((MetarWords[count].Substring(index, 1) == "+") || (MetarWords[count].Substring(index, 1) == "-"))
            {
                switch (MetarWords[count].Substring(index, 1))
                {
                    case "+":
                        qualifier = "Heavy ";
                        flag = true;
                        index++;
                        break;
                    case "-":
                        qualifier = "Light ";
                        flag = true;
                        index++;
                        break;
                }

                rchtxtbx_results.AppendText(qualifier);

            }
            //is it cloud or still present weather
            else if ((!flag) && (MetarWords[count].Length > 2) &&
                     (Clouds.CheckIfClouds(MetarWords[count].Substring(0, 3))))
            {

            }


            if (flag)
            {
                do
                {
                    rchtxtbx_results.AppendText(
                        PresentWeather.GetPresentWeather(MetarWords[count].Substring(index, 2)) + "\r\r");
                    index += 2;
                } while (index < MetarWords[count].Length);

                count++;

            }












            #endregion


            #region Cloud

            /////////////////////////////////////////////////////////////////////////////////////////////
            /// Cloud types
            ///
            /// FEW='few' (1-2 oktas), SCT='Scattered' (3-4 oktas), BKN='Broken' (5-7 oktas), OVC='Overcast', 
            /// NSC = 'No significant cloud'(none below 5000ft and no TCU or CB).
            /// There are only two cloud types reported; TCU = towering cumulus and CB = cumulonimbus.VV
            /// ='state of sky obscured' (cloud  base not discernable): Figures in lieu of '///' give vertical
            /// visibility in hundreds of feet.Up to  three, but occasionally more, cloud groups may be reported.
            /// Cloud heights are given in feet above airfield height.
            /// Cloud amounts are measured in oktas - one okta = one eighth of cloud cover.
            ////////////////////////////////////////////////////////////////////////////////////////////
            /// 
            //FEW007 BKN014CB BKN017




            //Amount in eights(octas)
            //SKC = Sky Clear(clear below 12, 000 for ASOS / AWOS)
            //NSC = No significant clouds
            //FEW = Few(1 / 8 to 2 / 8 sky cover)
            //SCT = Scattered(3 / 8 to 4 / 8 sky cover)
            //BKN = Broken(5 / 8 to 7 / 8 sky cover)
            //OVC = Overcast(8 / 8 sky cover)
            //NCD = No cloud detected 

            ////clouds
            //TCU = towering cumulus 
            //CB = cumulonimbus

            #endregion


            #region cavok



            #endregion






            #region temp_dew_point

            //find the pressure and come one back to find temperature
            int pressurecount = FindPressure(MetarWords);


            //////////////////////////////////////////////////////////////////////////
            //Terperature/Dewpoint (whole °C) (preceded by M=minus)
            //02/M01
            /////////////////////////////////////////////////////////////////////////

            //If dew point is missing, example would be reported as 10///.
            // M indicates a negative value
            //(whole °C) (preceded by M = minus)
            if (pressurecount > 0) //We found the QNH
            {
                double T = 0;
                double Td = 0;
                int idx = 0;

                string temperatures = MetarWords[pressurecount - 1]; //Always item before QNH

                //Temperature
                if (temperatures.StartsWith("M")) // Temperature is minus
                {
                    rchtxtbx_results.AppendText("Temperature = -" + temperatures.Substring(1, 2) + "°C\r");
                    T = Double.Parse(temperatures.Substring(1, 2));
                    idx = 4; //position of /
                }
                else
                {
                    rchtxtbx_results.AppendText("Temperature = " + temperatures.Substring(0, 2) + "°C\r");
                    T = Double.Parse(temperatures.Substring(0, 2));
                    idx = 3;
                }

                // Dew Point
                temperatures = MetarWords[pressurecount - 1].Substring(idx);

                if (temperatures.StartsWith("M")) // Temperature is minus
                {
                    rchtxtbx_results.AppendText("Dew Point = -" + temperatures.Substring(1, 2) + "°C\r");
                    Td = Double.Parse(temperatures.Substring(1, 2));
                }
                else
                {
                    rchtxtbx_results.AppendText("Dew Point = " + temperatures.Substring(0, 2) + "°C\r");
                    Td = Double.Parse(temperatures.Substring(0, 2));
                }

                //////////////////////////////////////////////////////////////////////////
                //Relative Humidity
                //This is derived from temperature and Dew point so is not 100% accurate
                // rh = 100 - 5(temp-dewpoint)
                /////////////////////////////////////////////////////////////////////////

                /* Dew point< 55  Dry and comfortable
                 55 < dew point < 65 Sticky
                     Dew point > 65  Heavy humid*/



                double rh = 100 - 5 * (T - Td);

                rchtxtbx_results.AppendText("Relative Humidity = " + rh.ToString() + "%\r");


                //////////////////////////////////////////////////////////////////////////
                // Wind Chill
                // This is derived from temperature and Dew point so is not 100% accurate
                //
                // Wind(mph) = 1.15077945 × Wind(kts)
                //
                // TF = 1.8 × TC + 32
                // TC = 0.55556 × (TF - 32)
                //
                //double windChill1 = 0.55556*((35.74 + 0.6215 * _temp + (0.4275 * _temp - 35.75) * Math.Pow(wind, 0.16))-32);
                /////////////////////////////////////////////////////////////////////////

                //temp in celcius and wind in knots
                double windChill = Math.Round(0.55556 * ((35.74 + 0.6215 * (1.8 * T + 32) +
                                                          (0.4275 * (1.8 * T + 32) - 35.75) *
                                                          Math.Pow(1.15077945 * wind, 0.16)) -
                                                         32), 2);

                rchtxtbx_results.AppendText("Windchill = " + windChill.ToString() + "°C\r\r");
            }

            #endregion

            //Altimeter setting (QNH) and indicator (A=InHg, Q=hPa)
            //Q1001
            if (pressurecount > 0) //We found the QNH
            {
                string QNH = MetarWords[pressurecount].Substring(1);

                if (MetarWords[pressurecount].Substring(0,1) == "Q")
                {
                    rchtxtbx_results.AppendText("QNH = " + QNH + "hPa\r\r");
                }
                else if (MetarWords[pressurecount].Substring(0, 1) == "A")
                {
                    rchtxtbx_results.AppendText("QNH = " + QNH +"InHg\r\r");
                }
                else
                {
                    rchtxtbx_results.AppendText("QNH = Unknown\r\r");
                }
            }
            else
            {
                rchtxtbx_results.AppendText("QNH = Unknown\r\r");
            }

            //Trend Forecast (2 hours from time of observation) 
            //BECMG




            //6000



        }

        private static int FindPressure(string[] data)
        {
            bool flag = false;
            int answer = 0;
            string stringToCheck1 = "Q"; // Find the sea level pressure hectopascals, QNH.
            string stringToCheck2 = "A"; // Find the sea level pressure  inches and hundredths, QNH.


            foreach (string s in data) // looking for Q
            {
                if (s.Substring(0, 1).Contains(stringToCheck1))
                {
                    flag = true;
                    break;
                }
                
                answer++;
            }

            if (!flag) // Not found Q so look for Axxxx
            {
                //lots more items to look for here.
                answer = 0;

                foreach (string s in data)
                {
                   if ((s.Substring(0, 1).Contains(stringToCheck2))&&(!s.Contains("METAR")) && (!s.Contains("AUTO"))
                       && (s.Length==5))     
                    {
                        flag = true;
                        break;
                    }

                    answer++;
                }
            }

            if (!flag) answer = 0;

            return answer;
        }
    }
}
