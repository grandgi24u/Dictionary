# Dictionary

## Authors

- [@grandgi24u](https://www.github.com/grandgi24u)

# Program Description and Usage Guide

This C# application, built using Visual Studio 2019, serves the purpose of creating a data dictionary by processing files from specific input folders and generating organized output files. This guide will help you understand the application's functionality, usage, and directory structure.

The **CreateDictionary** application is designed to process molecular structure data files and corresponding assignment log files. It extracts valuable information from these files and generates organized output files, facilitating further analysis. This guide explains the application's intended workflow and how to use it effectively.

## Input Structure

The application expects a specific directory structure in the **Input** folder:

Input/  
├── MMCIF/  
│ ├── [nomMmcif1].mmcif  
│ ├── [nomMmcif2].mmcif  
│ └── ...  
└── UBDBAssign/  
├── ubdbAssign_[nomMmcif1].log  
├── ubdbAssign_[nomMmcif2].log  
└── ...  

Place your MMCIF files in the `MMCIF` directory and the corresponding assignment log files in the `UBDBAssign` directory.

## Output Structure

The application generates output files in two different formats, each in its respective directory:

1. **Output**: This directory contains files for each residue, we have the details of each corresponding atoms :

Residue Atom_name Atome_type Coord_system         Atom_number Residue_number File_name
ALA  	  N	        N315		   X C_1633 Y CA_1656		1655	      45	           1A02.mmcif
ALA	    CA	      C421		   X N_1655 Y C_1657		1656	      45	           1A02.mmcif
ALA	    C	        C304		   X O_1658 Y N_1665		1657	      45	           1A02.mmcif
ALA	    O	        O1051		   X C_1657 Y N_1665		1658	      45	           1A02.mmcif
...

2. **OutputGroup**: This directory contains files for each residue, summarizing information for atoms within that residue. The file format is as follows:

Residue Atom_name Atom_type Count
ALA     C         -         926
ALA     C         C301      6
ALA     C         C304      5547
ALA     CA        -         968
...

3. **OutputGroupWithCoord**: This directory also contains files for each residue, but includes additional coordinate information for specific atom pairs. The file format is as follows:

Residue Atom_name Atom_type Coord_system Count
ALA     C         -         -            926
ALA     C         C301      Z CA X O     2
ALA     C         C301      Z CA X OXT   4
ALA     C         C304      X O Y N      5547
ALA     CA        -         -            968
...

## Usage

Follow these steps to use the **CreateDictionary** application effectively:

1. Clone the repository: `git clone https://github.com/grandgi24u/Dictionary.git`
2. Open the project in Visual Studio 2019.
3. In the `Program.cs` file, find the configuration section at the beginning of the `Main` method.
4. Update the file paths for `mmcifFilePath`, `ubdbAssignLogFilePath`, `outputFolder`, `outputFolder2`, and `outputFolder3` according to your directory structure.
5. Build and run the application.

## Configuration

In the `Main` method of the `Program.cs` file, you'll find a configuration section where you can set the file paths for input and output directories. Modify these paths to match your directory structure and file locations.

```csharp
string mmcifFilePath = "../../../Input/MMCIF/";
string ubdbAssignLogFilePath = "../../../Input/UBDBAssign/";
string outputFolder = "../../../output/";
string outputFolder2 = "../../../output/OutputGroup/";
string outputFolder3 = "../../../output/OutputGroupWithCoord/";
```


