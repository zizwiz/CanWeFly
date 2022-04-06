namespace Can_We_Fly.metar_items
{
    class Clouds
    {
        public static bool CheckIfClouds(string isItClouds)
        {
            bool answer = false;

            switch (isItClouds)
            {
                case "SKC":
                case "NSC":
                case "FEW":
                case "SCT":
                case "BKN":
                case "OVC":
                    answer = true;
                    break;
            }

            return answer;
        }

        public static string GetCloudInfo(string isItClouds)
        {
            string answer = "";
            string CloudType = "";
            string CloudHeight = "";

            if (isItClouds.Length > 3) CloudHeight = isItClouds.Substring(3, 3).TrimStart('0', ' ');
            if (isItClouds.Length > 6) CloudType = GetCloudType(isItClouds.Substring(6));


            //FEW007 BKN014CB BKN017

            switch (isItClouds.Substring(0, 3))
            {
                case "SKC":
                    answer = "Amount of cloud = Sky Clear (clear below 12000ft for ASOS / AWOS)\r";
                    break;
                case "NSC":
                    answer = "Amount of cloud = No significant clouds\r";
                    break;
                case "FEW":
                    answer = "Amount of cloud = Few (1/8 to 1/4 sky cover) " +
                             "@ " + CloudHeight + "00ft\r" + CloudType;
                    break;
                case "SCT":
                    answer = "Amount of cloud = Scattered (3/8 to 1/2 sky cover) " +
                             "@ " + CloudHeight + "00ft\r" + CloudType;
                    break;
                case "BKN":
                    answer = "Amount of cloud = Broken (5/8 to 7/8 sky cover) " +
                             "@ " + CloudHeight + "00ft\r" + CloudType;
                    break;
                case "OVC":
                    answer = "Amount of cloud = Overcast (full sky cover) " +
                             "@ " + CloudHeight + "00ft\r" + CloudType;
                    break;
                case "NCD":
                    answer = "Amount of cloud = No cloud detected\r";
                    break;
                default:
                    answer = "Unknown amount of cloud\r";
                    break;


            }
            return answer;
        }


        public static string GetCloudType(string CloudType)
        {
            string answer = "";

            ////clouds
            // ACSL Altocumulus or Lenticular
            // CB Cumulonimbus
            // CCSL Cirrocumulus
            // SCSL Stratocumulus
            // TCU Towering Cumulus

            switch (CloudType.ToUpper())
            {
                case "ACSL":
                    answer = "Cloudtype = Altocumulus or Lenticular\r";
                    break;
                case "CB":
                    answer = "Cloudtype = Cumulonimbus\r";
                    break;
                case "CCSL":
                    answer = "Cloudtype = Cirrocumulus\r";
                    break;
                case "SCSL":
                    answer = "Cloudtype = Stratocumulus\r";
                    break;
                case "TCU":
                    answer = "Cloudtype = Towering Cumulus\r";
                    break;
                case "=":
                    answer = "\r";
                    break;
                default:
                    answer = "Unknown cloud type\r";
                    break;
            }

            return answer;
           }



          
        

    }
}
