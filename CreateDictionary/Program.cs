using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateDictionary
{
    public class Program
    {

        static void Main()
        {
            string mmcifFilePath = "../../../Input/MMCIF/1A0A.mmcif";
            string ubdbAssignLogFilePath = "../../../Input/UBDBAssign/ubdbAssign_1A0A.log";

            /**List<string[]> res = ParseMmcifFile(mmcifFilePath);
            foreach(string[] r in res)
            {
                Console.WriteLine(r[0] + " " + r[1] + " " + r[2] );
            }*/

            ParseUbdbAssignLog(ubdbAssignLogFilePath);
            string outputFilePath = "../../../votre_fichier_sortie.txt";
            Console.ReadLine();
        }

        static List<string[]> ParseMmcifFile(string filePath)
        {
            List<string[]> output = new List<string[]>();
            string[] lines = File.ReadAllLines(filePath);
            bool startParsing = false;
            foreach (string line in lines)
            {
                if (line.StartsWith("_atom_site.calc_flag"))
                {
                    startParsing = true;
                    continue;
                }
                if (startParsing)
                {
                    string[] columns = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (columns.Length >= 9)
                    {
                        if(columns[4] != "HOH")
                        {
                            string[] toAdd = { columns[2].Replace("\"", "") + "_" + columns[0], columns[1], columns[4] };
                            output.Add(toAdd);
                        }
                    }
                }
            }
            return output;
        }

        static List<string[]> ParseUbdbAssignLog(string filePath)
        {
            List<string[]> output = new List<string[]>();
            string[] lines = File.ReadAllLines(filePath);
            bool startParsing = false;
            foreach (string line in lines)
            {
                if (line.Trim().StartsWith("(2) Atom Type Assignemnt"))
                {
                    startParsing = true;
                    continue;
                }
                if (startParsing && line.Contains("   "))
                {
                    string[] columns = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (columns.Length >= 3)
                    {
                        string[] toAdd = { columns[0], columns[1], columns[2] };
                        output.Add(toAdd);
                    }
                }
            }
            return output;
        }

    }
}
