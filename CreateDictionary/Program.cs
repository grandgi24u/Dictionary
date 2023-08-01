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
            WriteOutputToFile(CreateList(ParseMmcifFile(mmcifFilePath),ParseUbdbAssignLog(ubdbAssignLogFilePath)));
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

        static List<string[]> CreateList(List<string[]> listOfMmcif, List<string[]> listofLog)
        {
            List<string[]> output = new List<string[]>();
            foreach (string[] item in listOfMmcif)
            {
                string[] itemToAdd = new string[3];
                itemToAdd[0] = item[2];
                itemToAdd[1] = item[0].Substring(0, item[0].IndexOf('_')) ;
                foreach (string[] item2 in listofLog)
                {
                    if(item2[0] == item[0])
                    {
                        itemToAdd[2] = item2[2];
                    }
                }
                output.Add(itemToAdd);
            }
            return output;
        }

        static void WriteOutputToFile(List<string[]> residueDictionary)
        {
            string outputFolder = "../../../output/";
            foreach (var kvp in residueDictionary)
            {
                string file = outputFolder + kvp[0] + ".txt";
                string residue = kvp[0];
                string atomName = kvp[1];
                string atomType = kvp[2];
                if (File.Exists(file))
                {
                    using (StreamWriter writer = File.AppendText(file))
                    {
                        writer.WriteLine($"{residue}\t{atomName}\t{atomType}");
                    }
                } else
                {
                    using (StreamWriter writer = new StreamWriter(file))
                    {
                        writer.WriteLine("Residue\nAtom_name\nAtome_type");
                        writer.WriteLine($"{residue}\t{atomName}\t{atomType}");
                    }                    
                }
            }
        }
    }
}
