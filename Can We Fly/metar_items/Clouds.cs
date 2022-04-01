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


    }
}
