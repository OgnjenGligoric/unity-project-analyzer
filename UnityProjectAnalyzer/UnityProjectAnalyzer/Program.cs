// See https://aka.ms/new-console-template for more information
using System;
using UnityProjectAnalyzer;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: ./tool.exe unity_project_path output_folder_path");
            return;
        }

        string projectPath = Directory.GetCurrentDirectory(); // Get current directory (project directory)

        string inputDirectoryPath = Path.Combine(projectPath, args[0]); // Use your directory name
        string outputDirectoryPath = Path.Combine(projectPath, args[1]); // Use your directory name

        if (Directory.Exists(outputDirectoryPath))
        {
            Directory.Delete(outputDirectoryPath, true); // Delete existing directory and its contents
        }

        Directory.CreateDirectory(outputDirectoryPath);


        DirectoryParser directoryParser = new DirectoryParser(projectPath, outputDirectoryPath);
        directoryParser.ListDirectoriesAndFiles(inputDirectoryPath);


    }

    
}