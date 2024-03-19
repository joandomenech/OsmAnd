using System.Xml.Linq;
using System.Linq;

class Program
{
    static void Main()
    {
        XNamespace gpxNs = "http://www.topografix.com/GPX/1/1";
        XNamespace osmandNs = "http://www.osmand.net/osmand";

        //Open all files from POI folder
        string[] files = [""];

        if (Directory.Exists("POI"))
        {
            files = Directory.GetFiles("POI", "*.gpx");
        }
        else
        {
            Console.WriteLine("Cal que existeixi el directori POI amb els fitxers a convertir");
            Environment.Exit(0);
        } 

        foreach (var file in files)
        {
            // Load the origen GPX file
            var origenDoc = XDocument.Load(file);

            var fileName = Path.GetFileNameWithoutExtension(file);

            // Create the metadata of the desti GPX file
            var metadata = new XElement("metadata",
                new XElement("name", "favorites-"+fileName),
                new XElement("time", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"))
            );

            // Create the root of the desti GPX file
            var gpx = new XElement("gpx",
                new XAttribute("version", "OsmAnd 4.6.6"),
                new XAttribute("creator", "OsmAnd Maps 4.6.6 (4.6.6.1)"),
                new XAttribute("xmls", "http://www.topografix.com/GPX/1/1"),
                new XAttribute(XNamespace.Xmlns + "osmand", osmandNs),
                new XAttribute(XNamespace.Xmlns + "gpxtpx","http://www.garmin.com/xmlschemas/TrackPointExtension/v1"),
                new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                new XAttribute(XNamespace.Xmlns + "schemaLocation", "gpxNs http://www.topografix.com/GPX/1/1/gpx.xsd"));

            gpx.Add(metadata);

            // Iterate over waypoints in the origen GPX file
            foreach (var wpt in origenDoc.Descendants(gpxNs + "wpt"))
            {
                
                // Create a waypoint for each waypoint in the origen file
                var newWpt = new XElement("wpt",
                    new XAttribute("lat", wpt.Attribute("lat").Value),
                    new XAttribute("lon", wpt.Attribute("lon").Value),
                    new XElement("time", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")),
                    new XElement("name", wpt.Element(gpxNs + "name").Value),
                    new XElement("type", fileName),
                    new XElement("extensions",
                        new XElement(osmandNs+"address", wpt.Element(gpxNs + "name").Value),
                        new XElement(osmandNs+"icon", "mountain"),
                        new XElement(osmandNs+"background", "circle"),
                        new XElement(osmandNs+"color", "#ffff0000"),
                        new XElement(osmandNs+"amenity_subtype", "user_defined_other_postcode"),
                        new XElement(osmandNs+"amenity_type", "user_defined_other"),
                        new XElement(osmandNs+"amenity_origin", "Amenity:: :"))
                        );
                gpx.Add(newWpt);
            }

            var lastElement = new XElement("extensions",
                new XElement(osmandNs+"points_groups",
                    new XElement(osmandNs+"group",
                        new XAttribute("color", "#ffff0000"),
                        new XAttribute("background", "circle"),
                        new XAttribute("icon", "mountain"),
                        new XAttribute("name", fileName))));
            gpx.Add(lastElement);

            // Write the desti GPX file
            string directoryPath = "POI_OsmAnd";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            var destiDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), gpx);
            string filePath = Path.Combine(directoryPath, fileName + "_osmand.gpx");
            destiDoc.Save(filePath);
        }
    }
}
