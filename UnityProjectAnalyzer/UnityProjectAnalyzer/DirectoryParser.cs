using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityProjectAnalyzer.Models;
using UnityProjectAnalyzer.Utils;

namespace UnityProjectAnalyzer
{
    public class DirectoryParser
    {
        
        private readonly String _projectPath;
        private readonly String _outputDirectory;

        public DirectoryParser(string projectPath, string outputDirectory)
        {
            _projectPath = projectPath;
            _outputDirectory=outputDirectory;
        }

        public void ListDirectoriesAndFiles(string directoryPath)
        {
            try
            {
                // Get directories and files within the specified directory
                string[] directories = Directory.GetDirectories(directoryPath);
                string[] files = Directory.GetFiles(directoryPath);

                // Display directories
                foreach (var directory in directories)
                {
                    string relativePath = GetRelativePath(_projectPath, directory);
                    Console.WriteLine("Directory: " + relativePath);
                    ListDirectoriesAndFiles(directory); // Recursively list contents
                }

                // Display files
                foreach (var file in files)
                {
                    
                    string relativePath = GetRelativePath(_projectPath, file);
                    Console.WriteLine("File: " + relativePath);

                    if (file.EndsWith(".unity"))
                    {
                        UnitySceneParser unitySceneParser = new UnitySceneParser(file);
                        List<Transform> transforms = unitySceneParser.getAllTransforms();
                        List<GameObject> gameObjects = unitySceneParser.getAllGameObjects();

                        var hierarchyBuilder = new HierarchyBuilder(transforms, gameObjects);
                        var hierarchy = hierarchyBuilder.GetHierarchy();

                        WriteDumpToOutputProject(file, hierarchy);
                        Console.WriteLine("\n======== HIERARCHY ========");
                        Console.WriteLine(hierarchy);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void WriteDumpToOutputProject(string file, string hierarchy)
        {

            char separator = Path.DirectorySeparatorChar;
            string outputFileDumpName = file.Split(separator)[^1] + ".dump";


            string dumpFilePath = Path.Combine(_outputDirectory, outputFileDumpName);

            try
            {
                if (File.Exists(dumpFilePath))
                {
                    // Replace content in an existing file
                    File.WriteAllText(dumpFilePath, hierarchy);
                    Console.WriteLine("Content replaced in the dump file at: " + dumpFilePath);
                }
                else
                {
                    // Create a new file and write content to it
                    using (StreamWriter writer = File.CreateText(dumpFilePath))
                    {
                        writer.WriteLine(hierarchy);
                    }
                    Console.WriteLine("New dump file created at: " + dumpFilePath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error updating the dump file: " + e.Message);
            }

        }

        private static string GetRelativePath(string projectPath, string fullPath)
        {
            // Make the path relative to the project directory
            return Path.GetRelativePath(projectPath, fullPath);
        }
    }
}
