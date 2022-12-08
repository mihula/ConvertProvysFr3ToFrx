using System.Text;
using System.Xml;
using System.Xml.Linq;

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0) 
        {
            Console.WriteLine("ConvertProvysFr3ToFrx converts Provys xml report (XMLExport type) form FastReport3 to FastReportNET format");
            Console.WriteLine("Each parameter is consider as filename and converted to xml report (XMLExport type) with filename <report_name>_net.xml");
            Console.WriteLine();
            Console.WriteLine("Usage: ConvertProvysFr3ToFrx.exe file1.xml [file2.xml...]");
            return;
        }

        //// test
        //string sample = "./sample_fr3.report.xml";
        //string sampleout = "./sample_fr3_frnet.report.xml";
        //string output = new Convertor().ConvertFile(sample);
        //Console.WriteLine(output);
        //File.WriteAllText(sampleout, output, Encoding.UTF8);
        //Console.WriteLine("END");

        foreach (var arg in args) {
            var outFilename = arg.Replace(".report.xml", "_net.report.xml");
            Console.WriteLine($"Processing: {arg} >> {outFilename}");
            string output = new Convertor().ConvertFile(arg);
            File.WriteAllText(outFilename, output, Encoding.UTF8);
        }
        Console.WriteLine($"Bye.");
    }
}