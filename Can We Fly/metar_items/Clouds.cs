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
            //FEW007 BKN014CB BKN017

           switch (isItClouds)
            {
                case "SKC":
                    answer = "Amount of cloud = Sky Clear(clear below 12, 000 for ASOS / AWOS)\r";
                    break;
                case "NSC":
                    answer = "Amount of cloud = No significant clouds \r";
                    break;
                case "FEW":
                    answer = "Amount of cloud = Few(1 / 8 to 2 / 8 sky cover)\r";
                    break;
                case "SCT":
                    answer = "Amount of cloud = Scattered(3 / 8 to 4 / 8 sky cover)\r";
                    break;
                case "BKN":
                    answer = "Amount of cloud = Broken(5 / 8 to 7 / 8 sky cover)\r";
                    break;
                case "OVC":
                    answer = "Amount of cloud = Overcast(8 / 8 sky cover)\r";
                    break;
                case "NCD":
                    answer = "Amount of cloud = No cloud detected\r";
                    break;
                default:
                    answer = "Unknown amount of cloud\r";
                    break;


            }






            ////clouds
            //TCU = towering cumulus 
            //CB = cumulonimbus







            return answer;
        }

    }
}
