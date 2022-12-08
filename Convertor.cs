using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;
using System.Xml;
using System.Xml.XPath;

internal class Convertor
{
    public Convertor()
    {
    }

    //internal string ConvertFile(string inputFile)
    //{
    //    var result = new StringBuilder();

    //    XmlReader xml = XmlReader.Create(inputFile);

    //    while (xml.Read())
    //    {
    //        switch (xml.NodeType)
    //        {
    //            case XmlNodeType.Element:
    //                // You may need to capture the last element to provide a context
    //                // for any comments you come across... so copy xmlRdr.Name, etc.
    //                Console.WriteLine($"Element: {xml.Name}");
    //                break;

    //            case XmlNodeType.XmlDeclaration:
    //                Console.WriteLine($"XMLDECLARATION: {xml.Name}");
    //                break;

    //            case XmlNodeType.ProcessingInstruction:
    //                Console.WriteLine($"PROCESSING: {xml.Name} | {xml.Value}");
    //                break;

    //            default:
    //                //Console.WriteLine($"TYPE: {xmlRdr.NodeType}");
    //                break;
    //        }

    //        //result.Append(xml.ToString());
    //    }

    //    return result.ToString();
    //}
    
    internal string ConvertFile(string inputFile)    
    {
        var result = new StringBuilder();

        var xml = new XmlDocument();
        xml.Load(inputFile);

        var root = xml.DocumentElement;

        if (root != null)
        {
            if (root.Name == "REPORT")
            {
                var oldName = "UNKNOWN";
                var newName = "UNKNOWN";
                // fix the name
                var attr = root.GetAttributeNode("NAME_NM");
                if (attr != null)
                {
                    oldName = attr.Value;
                    newName = $"{oldName}_NET";
                    attr.Value = newName;
                }

                // typ: fr_net
                var dataNode = root.SelectSingleNode("//REPORT/DATA/REPORTTYPE_RF");
                if (dataNode != null)
                { 
                    dataNode.InnerText = "N";
                }

                // name: {name} (NETCORE)
                var nameNode = root.SelectSingleNode("//REPORT/DATA/STDNAME");
                if (nameNode != null)
                {
                    nameNode.InnerText = $"{nameNode.InnerText} (NETCORE)";
                }

                // changelog: v1.0
                var changelogNode = root.SelectSingleNode("//CHANGELOG");
                if (changelogNode != null)
                {
                    var attrVer = changelogNode.Attributes?.GetNamedItem("VER");
                    if (attrVer != null)
                        attrVer.Value = "1.0";
                    changelogNode.InnerText = $"{DateTime.Today.ToString("dd.mm.yyyy")}   1.0  {Environment.UserName}   REQ006949   CONVERTED TO NETCORE (source: {oldName})";
                }

                // repselect reference to report name
                // REPSELECT REPORT_ID.NAME_NM = "PRG_PROG_LIST_FR3"
                var repselectNodes = root.SelectNodes("//REPSELECT")?.OfType<XmlNode>();
                if (repselectNodes != null)
                {
                    foreach (var repselectNode in repselectNodes)
                    {
                        var attrReportName = repselectNode.Attributes?.GetNamedItem("REPORT_ID.NAME_NM");
                        if (attrReportName != null)
                            attrReportName.Value = newName;
                    }
                }

                // report body
                var bodyNode = root.SelectSingleNode("//REPORT/DATA/REPORT_BODY");
                if (bodyNode != null)
                {
                    var body = bodyNode.InnerText;
                    var bodyFilename = $"{inputFile}.fr3";
                    File.WriteAllText(bodyFilename, body, Encoding.UTF8);
                    var pi = new ProcessStartInfo() {
                        FileName = "./fr3tofrx.exe",
                        Arguments = bodyFilename
                    };
                    var p = Process.Start(pi);
                    p.WaitForExit();

                    var newBodyFilename = $"{inputFile}.frx";
                    var newBody = File.ReadAllText(newBodyFilename, Encoding.UTF8);

                    bodyNode.InnerText = newBody;
                }

            }

            //root.SelectSingleNode("descendant::book[@bk:ISBN='1-861001-57-6']", nsmgr);

        }

        //xml.SelectSingleNode("//REPORT/Title").InnerText = "NewValue";

        result.Append(xml.OuterXml);

        return result.ToString();
    }
}