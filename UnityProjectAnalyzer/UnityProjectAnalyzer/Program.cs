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
        string inputPath = args[0];
        string outputPath = args[1];

        string projectPath = Directory.GetCurrentDirectory(); // Get current directory (project directory)

        Console.WriteLine("Project path: " + projectPath);

        string inputDirectoryPath = Path.Combine(projectPath, inputPath); // Use your directory name
        string outputDirectoryPath = Path.Combine(projectPath, outputPath); // Use your directory name


        // Now you can use projectPath and outputPath in your program
        Console.WriteLine("Input Path: " + inputPath);
        Console.WriteLine("Output Path: " + outputPath);
        Console.WriteLine("========================================");

        DirectoryParser directoryParser = new DirectoryParser(projectPath);
        directoryParser.ListDirectoriesAndFiles(inputDirectoryPath);


    }

    
}