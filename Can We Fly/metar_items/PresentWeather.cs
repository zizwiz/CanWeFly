namespace Can_We_Fly.metar_items
{
    class PresentWeather
    {
        public static string GetPresentWeather(string data)
        {
            switch (data)
            {
                // Descriptor
                case "BC": data = "Patches "; break;
                case "BL": data = "Blowing (6 feet or more above the ground) "; break;
                case "DR": data = "Drifting (Less than 6 feet above the ground) "; break;
                case "FZ": data = "Freezing "; break;
                case "MI": data = "Shallow"; break;
                case "PR": data = "Banks "; break;
                case "SH": data = "Showers "; break;
                case "TS": data = "Thunderstorm "; break;

                // Precipitation
                case "DZ": data = "Drizzle "; break;
                case "GR": data = "Hail (diameter 5mm or greater) "; break;
                case "GS": data = "Small hail or snow pellets (diameter less than 5mm) "; break;
                case "IC": data = "Ice crystals "; break;
                case "PL": data = "Ice pellets "; break;
                case "RA": data = "Rain "; break;
                case "SG": data = "Snow grains "; break;
                case "SN": data = "Snow "; break;
                case "UP": data = "Unidentified Precipitation(AUTO METARs only) "; break;

                //Obscuration
                case "BR": data = "Mist "; break;
                case "DU": data = "Dust "; break;
                case "FG": data = "Fog (Visibility less than 1000m (5/8SM))"; break;
                case "FU": data = "Smoke "; break;
                case "HZ": data = "Haze "; break;
                case "SA": data = "Sand "; break;
                case "VA": data = "Volcanic ash "; break;

                //other
                case "DS": data = "Dust storm "; break;
                case "FC": data = "Funnel cloud "; break;
                case "PO": data = "Dust devils "; break;
                case "SQ": data = "Squalls "; break;
                case "SS": data = "Sandstorm "; break;
                case "VC": data = "In vicinity (Within 8KM (5SM) of the aerodrome but not at the aerodrome) "; break;
            }

            return data;
        }
    }
}
