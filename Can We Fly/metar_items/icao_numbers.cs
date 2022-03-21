using System.Xml;

namespace Can_We_Fly.metar_items
{
    class icao_numbers
    {

        public static string FindICAOInfo(string ICAO_number)
        {
            string reply = "Unknown";

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("airport_data.xml");

                XmlNodeList nodeList;
                XmlNode root = doc.DocumentElement;

                nodeList = root.SelectNodes("descendant::airport_info[icao_code ='" + ICAO_number + "']");


                foreach (XmlNode data in nodeList)
                {
                    reply = data["airport_name"].InnerText;
                }
            }
            catch 
            {
                reply = "Check the ICAO code is correct";
            }
           

            return reply;
        }
    }
}
