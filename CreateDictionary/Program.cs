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
            Console.WriteLine("Program CreateDictionary developped by Clément GRANDGIRARD");
            string mmcifFilePath = "./Input/MMCIF/";
            string ubdbAssignLogFilePath = "./Input/UBDBAssign/";
            string outputFolder = "./output/";
            Dictionary<string, string> listofFile = new Dictionary<string, string>();           
            string[] fileEntries = Directory.GetFiles(mmcifFilePath);
            DirectoryInfo d = new DirectoryInfo(mmcifFilePath);
            foreach (var file in d.GetFiles("*.mmcif"))
            {
                string filename = file.Name.Substring(0, file.Name.IndexOf("."));
                if(File.Exists(ubdbAssignLogFilePath + "ubdbAssign_" + filename + ".log"))
                {
                    listofFile.Add(mmcifFilePath + file.Name, ubdbAssignLogFilePath + "ubdbAssign_" + filename + ".log");
                }
            }
            fileEntries = Directory.GetFiles(outputFolder);
            foreach (var file in fileEntries)
            {
                File.Delete(file);
            }
            foreach (var key in listofFile)
            {
                Console.WriteLine("Start process for file : " + key.Key);
                WriteOutputToFile(outputFolder, CreateList(ParseMmcifFile(key.Key), ParseUbdbAssignLog(key.Value)));
            }
            fileEntries = Directory.GetFiles(outputFolder);
            foreach (var file in fileEntries)
            {
                CountSameLineInFile(file);
            }
            Console.WriteLine("Finish");
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

        static void WriteOutputToFile(string outputFolder, List<string[]> residueDictionary)
        {
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

        static void CountSameLineInFile(string file)
        {
            string[] lines = File.ReadAllLines(file);
            lines = lines.Skip(3).ToArray();
            Dictionary<string, int> counts = lines.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
            File.Delete(file);
            using (StreamWriter writer = new StreamWriter(file))
            {
                writer.WriteLine("Residue\nAtom_name\nAtome_type");
                foreach (var item in counts)
                {
                    writer.WriteLine($"{item.Key}\t{item.Value}");
                }      
            }       
        }
    }
}
