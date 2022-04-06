using System.Text.RegularExpressions;


namespace Can_We_Fly.common_items
{
    class utils
    {

        public static bool IsItNumber(string data)
        {
            return Regex.IsMatch(data, @"^\d+$");
        }

        public static bool FindCAVOK(string[] data)
        {
            bool flag = false;
            string stringToCheck1 = "CAVOK";

            foreach (string s in data) // looking for Q
            {
                if (s.ToUpper().Contains(stringToCheck1))
                {
                    flag = true;
                    break;
                }
            }

            return flag;
        }

        public static bool InMaintenance(string[] data)
        {
            bool flag = false;
            string stringToCheck1 = "$";

            foreach (string s in data) // looking for $ 
            {
                if (s.ToUpper().Contains(stringToCheck1))
                {
                    flag = true;
                    break;
                }
            }

            return flag;
        }

        public static int FindPressure(string[] data)
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
                    if ((s.Substring(0, 1).Contains(stringToCheck2)) && (!s.Contains("METAR")) && (!s.Contains("AUTO"))
                        && (s.Length == 5))
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
