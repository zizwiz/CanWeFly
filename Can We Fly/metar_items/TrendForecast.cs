
using Can_We_Fly.common_items;

namespace Can_We_Fly.metar_items
{
    class TrendForecast
    {


        public static string CheckTrendForecast(string trend)
        {
            string answer = "";

            if (trend.Length == 3)
            {
                if (trend == "NSW")
                {
                    answer = "No significant weather to report\r";
                }
                else if (trend == "RE")
                {
                    answer = "No significant cloud seen\r";
                }
                else
                {
                    answer = "Unknown item = " + trend + "\r";
                }

            }
            else if (trend.Length == 4)
            {
                if (trend.Substring(0, 2) == "RE")
                {
                    // Recent weather
                    //RETS 'recent thunderstorm' RE = Recent, weather codes given above. Up to three groups may be present.
                    answer = "Recent " + PresentWeather.GetPresentWeather(trend.Substring(2, 2)) + "\r";
                }
                else if (utils.IsItNumber(trend))
                {
                    answer = trend + "m\r";
                }
                else
                {
                    answer = "Unknown item = " + trend + "\r";
                }
            }
            else if (trend.Length==5)
            {
                if (trend == "BECMG")
                {
                    answer = "Becoming ";
                }
                else if (trend == "TEMPO")
                {
                    answer = "Temporarily ";
                }
                else if (trend == "NOSIG")
                {
                    answer = "No significant impending weather change\r";
                }
                else
                {
                    answer = "Unknown item = " + trend + "\r";
                }

            }
            else if (trend.Length == 6)
            {
                if (trend.Substring(0, 2) == "AT")
                {
                    answer = "at " + trend.Substring(2, 4) + " Zulu Time ";

                }
                else if (trend.Substring(0, 2) == "TL")
                {
                    answer = "till " + trend.Substring(2, 4) + " Zulu Time ";
                }
                else if (trend.Substring(0, 2) == "FM")
                {
                    answer = "from " + trend.Substring(2, 4) + " Zulu Time ";
                }
                else
                {
                    answer = "Unknown item = " + trend + "\r";
                }
            }
            else
            {
                if (Clouds.CheckIfClouds(trend.Substring(0,3)))
                {
                    //clouds
                    answer = Clouds.GetCloudInfo(trend);

                }
                else
                {
                    answer = "Unknown item = " + trend + "\r";
                }
                
            }


            

            // Wind shear
            //WS RWY24 
            // 'wind shear runway 
            // two four'
            // Will not be reported at present for UK aerodromes

            // Colour states
            //Military reports also display a colour state BLU, WHT, GRN, YLO, AMB or RED, coded according to 
            // cloud and visibility.
            // BLACK may also be reported when the runway is unusable.

            //Runway states
            //Refer to the CAA’s CAP 746 - Meteorological Observations at Aerodromes

            //----------------------------------------------------
            //BECMG FM1100
            //23035G50KT
            //    TEMPO FM0630
            //    TL 0830 3000
            //SHRA
            //-----------------------------------------------
            //'becoming from 1100, 
            //230 degrees 35 KT, max
            //50 KT, temporarily
            //    from 0630 until 0830, 
            //3000 metres, Moderate
            //    rain showers'
            //----------------------------------------------------
            

            return answer;
        }





    }
}
