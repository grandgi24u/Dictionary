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
        public static void Main()
        {
            // Remplacez ces chemins par les chemins vers vos fichiers réels
            string mmcifFilePath = "../../../Input/MMCIF/1A0A.mmcif";
            string ubdbAssignLogFilePath = "../../../Input/UBDBAssign/ubdbAssign_1A0A.log";

            // Créez un dictionnaire pour stocker les informations d'atomes
            Dictionary<string, Dictionary<string, string>> residueDictionary = new Dictionary<string, Dictionary<string, string>>();

            // Parsez le fichier mmcif et mettez à jour le dictionnaire avec les informations d'atomes
            ParseMmcifFile(mmcifFilePath, residueDictionary);

            // Parsez le fichier .log et mettez à jour le dictionnaire avec les informations d'attribution des types d'atomes
            ParseUbdbAssignLog(ubdbAssignLogFilePath, residueDictionary);

            // Écrivez les informations dans le fichier de sortie
            string outputFilePath = "../../../votre_fichier_sortie.txt";
            WriteOutputToFile(outputFilePath, residueDictionary);
        }

        private static void ParseMmcifFile(string filePath, Dictionary<string, Dictionary<string, string>> residueDictionary)
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                // Vérifiez si la ligne commence par "ATOM"
                if (line.StartsWith("ATOM"))
                {
                    string[] columns = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (columns.Length >= 11)
                    {
                        string residue = columns[5];
                        string atomName = columns[2];

                        // Mettez à jour le dictionnaire avec les informations d'atome
                        if (!residueDictionary.ContainsKey(residue))
                            residueDictionary[residue] = new Dictionary<string, string>();

                        residueDictionary[residue][atomName] = string.Empty; // Vous pouvez initialiser avec une valeur vide si nécessaire.
                    }
                }
            }
        }

        private static void ParseUbdbAssignLog(string filePath, Dictionary<string, Dictionary<string, string>> residueDictionary)
        {
            string[] lines = File.ReadAllLines(filePath);

            bool startParsing = false;
            foreach (string line in lines)
            {
                // Vérifiez si le parsing peut commencer
                if (line.StartsWith("(2) Atom Type Assignemnt"))
                {
                    startParsing = true;
                    continue;
                }

                // Vérifiez si le parsing doit s'arrêter
                if (line.StartsWith("################################################################################"))
                {
                    startParsing = false;
                    break;
                }

                // Commencez le parsing et mettez à jour le dictionnaire avec les informations d'attribution des types d'atomes
                if (startParsing && line.Contains("   "))
                {
                    string[] columns = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (columns.Length >= 3)
                    {
                        string atomName = columns[0];
                        string atomType = columns[2];

                        foreach (var residueAtoms in residueDictionary.Values)
                        {
                            if (residueAtoms.ContainsKey(atomName))
                            {
                                residueAtoms[atomName] = atomType;
                            }
                        }
                    }
                }
            }
        }

        private static void WriteOutputToFile(string filePath, Dictionary<string, Dictionary<string, string>> residueDictionary)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("RESIDUE\tATOM_NAME\tATOM_TYPE");

                foreach (var kvp in residueDictionary)
                {
                    string residue = kvp.Key;

                    foreach (var atomKvp in kvp.Value)
                    {
                        string atomName = atomKvp.Key;
                        string atomType = atomKvp.Value;

                        writer.WriteLine($"{residue}\t{atomName}\t{atomType}");
                    }
                }
            }
        }
    }
}
