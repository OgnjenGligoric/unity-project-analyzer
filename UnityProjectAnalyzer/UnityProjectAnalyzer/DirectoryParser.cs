using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityProjectAnalyzer.Models;

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
                        foreach (Transform transform in transforms)
                        {
                            Console.WriteLine(transform.ToString());
                        }
                        foreach (GameObject gameObject in gameObjects)
                        {
                            Console.WriteLine(gameObject.ToString());
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private static string GetRelativePath(string projectPath, string fullPath)
        {
            // Make the path relative to the project directory
            return Path.GetRelativePath(projectPath, fullPath);
        }
    }
}
