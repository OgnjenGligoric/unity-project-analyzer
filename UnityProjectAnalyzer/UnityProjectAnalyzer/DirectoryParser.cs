using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityProjectAnalyzer
{
    public class DirectoryParser
    {
        
        private readonly String _projectPath;

        public DirectoryParser(string projectPath)
        {
            _projectPath = projectPath;
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
