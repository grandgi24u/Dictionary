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
            string mmcifFilePath = "../../../Input/MMCIF/";
            string ubdbAssignLogFilePath = "../../../Input/UBDBAssign/";
            string outputFolder = "../../../output/";
            string outputFolder2 = "../../../output/OutputGroup/";
            string outputFolder3 = "../../../output/OutputGroupWithCoord/";
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
            fileEntries = Directory.GetFiles(outputFolder2);
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
                CountSameLineInFile(file, outputFolder2 + file.Substring(file.LastIndexOf('/') + 1));
                CountSameLineInFile2(file, outputFolder3 + file.Substring(file.LastIndexOf('/') + 1));
            }
            Console.WriteLine("Finish");
            Console.ReadLine();
        }

        static List<string[]> ParseMmcifFile(string filePath)
        {
            List<string[]> output = new List<string[]>();
            string[] lines = File.ReadAllLines(filePath);
            string fileName = filePath.Substring(filePath.LastIndexOf('/') + 1);
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
                            string[] toAdd = { columns[2].Replace("\"", "") + "_" + columns[0], columns[1], columns[4], columns[0], columns[7], fileName };
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
                    if (columns.Length == 3)
                    {
                        string[] toAdd = { columns[0], columns[1], columns[2], "", "", "", "" };
                        output.Add(toAdd);
                    } else if(columns.Length > 3)
                    {
                        string[] toAdd = { columns[0], columns[1], columns[2], columns[3], columns[4], columns[5], columns[6] };
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
                string[] itemToAdd = new string[10];
                itemToAdd[0] = item[2];
                itemToAdd[1] = item[0].Substring(0, item[0].IndexOf('_')) ;
                foreach (string[] item2 in listofLog)
                {
                    if(item2[0] == item[0])
                    {
                        itemToAdd[2] = item2[2];
                        itemToAdd[3] = item2[3];
                        itemToAdd[4] = item2[4];
                        itemToAdd[5] = item2[5];
                        itemToAdd[6] = item2[6];
                    }
                }
                itemToAdd[7] = item[3];
                itemToAdd[8] = item[4];
                itemToAdd[9] = item[5];
                output.Add(itemToAdd);
            }
            return output;
        }

        static void WriteOutputToFile(string outputFolder, List<string[]> residueDictionary)
        {
            foreach (var kvp in residueDictionary)
            {
                string file = outputFolder + kvp[0] + ".txt";
                if (File.Exists(file))
                {
                    using (StreamWriter writer = File.AppendText(file))
                    {
                        writer.WriteLine($"{kvp[0]}\t{kvp[1]}\t{kvp[2]}\t\t{kvp[3]} {kvp[4]} {kvp[5]} {kvp[6]}\t\t{kvp[7]}\t{kvp[8]}\t{kvp[9]}");
                    }
                } else
                {
                    using (StreamWriter writer = new StreamWriter(file))
                    {
                        writer.WriteLine("Residue\nAtom_name\nAtome_type\nCoord_system\nAtom_number\nResidue_number\nFile_name");
                        writer.WriteLine($"{kvp[0]}\t{kvp[1]}\t{kvp[2]}\t\t{kvp[3]} {kvp[4]} {kvp[5]} {kvp[6]}\t\t{kvp[7]}\t{kvp[8]}\t{kvp[9]}");
                    }                    
                }
            }
        }

        static void CountSameLineInFile(string file, string outFile)
        {
            string[] lines = File.ReadAllLines(file);
            lines = lines.Skip(7).ToArray();
            List<string> lines2 = new List<string>();
            foreach(string line in lines)
            {
                string[] columns = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                lines2.Add(columns[0] + "\t" + columns[1] + "\t" + columns[2]);
            }
            lines2 = lines2.OrderBy(x => x).ToList();
            Dictionary<string, int> counts = lines2.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
            using (StreamWriter writer = new StreamWriter(outFile))
            {
                writer.WriteLine("Residue\nAtom_name\nAtome_type");
                foreach (var item in counts)
                {
                    writer.WriteLine($"{item.Key}\t{item.Value}");
                }      
            }      
        }

        static void CountSameLineInFile2(string file, string outFile)
        {
            string[] lines = File.ReadAllLines(file);
            lines = lines.Skip(7).ToArray();
            List<string> lines2 = new List<string>();
            foreach (string line in lines)
            {
                string[] columns = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                lines2.Add(columns[0] + "\t" + columns[1] + "\t" + columns[2] + "\t" + columns[3]);
            }
            lines2 = lines2.OrderBy(x => x).ToList();
            Dictionary<string, int> counts = lines2.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
            using (StreamWriter writer = new StreamWriter(outFile))
            {
                writer.WriteLine("Residue\nAtom_name\nAtome_type\nCoord_system");
                foreach (var item in counts)
                {
                    writer.WriteLine($"{item.Key}\t{item.Value}");
                }
            }
        }
    }
}
