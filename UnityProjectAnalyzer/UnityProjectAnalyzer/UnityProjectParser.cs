using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityProjectAnalyzer.Models;
using UnityProjectAnalyzer.Utils;

namespace UnityProjectAnalyzer
{
    public class UnityProjectParser
    {
        
        private readonly String _inputProjectPath;
        private readonly String _outputDirectory;
        public HashSet<String> ScriptUsages;
        public HashSet<Script> AllScripts;
        public UnityProjectParser(string inputProjectPath, string outputDirectory)
        {
            _inputProjectPath = inputProjectPath;
            _outputDirectory=outputDirectory;
            ScriptUsages = new HashSet<string>();
            AllScripts = new HashSet<Script>();
        }

        public void ConstructSceneHierarchy(string currentDirectory)
        {
            try
            {
                // Get directories and files within the specified directory
                string[] directories = Directory.GetDirectories(currentDirectory);
                string[] files = Directory.GetFiles(currentDirectory);
            
                // Display directories
                foreach (var directory in directories)
                {
                    showDirectoryRelativePath(directory);
                }

                // Display files
                foreach (var file in files)
                {
                    
                    string relativePath = GetRelativePath(_inputProjectPath, file);
                    Console.WriteLine("File: " + relativePath);

                    if (file.EndsWith(".unity"))
                    {
                        SceneParsing(file);
                    }
                    else if (file.EndsWith(".cs.meta"))
                    {
                        ExtractAllExistingScripts(file);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void ExtractAllExistingScripts(string file)
        {
            ScriptParser scriptParser = new ScriptParser(file);
            AllScripts.Add(new Script(scriptParser.GetGUID(), GetRelativePath(_inputProjectPath, file)));
        }

        private void showDirectoryRelativePath(string directory)
        {
            string relativePath = GetRelativePath(_inputProjectPath, directory);
            Console.WriteLine("Directory: " + relativePath);
            ConstructSceneHierarchy(directory); // Recursively list contents
        }

        private void SceneParsing(string file)
        {
            UnitySceneParser unitySceneParser = new UnitySceneParser(file);
            List<Transform> transforms = unitySceneParser.GetAllTransforms();
            List<GameObject> gameObjects = unitySceneParser.GetAllGameObjects();
            unitySceneParser.GetAllUsages(ScriptUsages);
            var hierarchyBuilder = new HierarchyBuilder(transforms, gameObjects);
            var hierarchy = hierarchyBuilder.GetHierarchy();

            WriteDumpToOutputProject(file, hierarchy);
            Console.WriteLine("\n======== HIERARCHY ========");
            Console.WriteLine(hierarchy);
        }

        private void WriteDumpToOutputProject(string file, string hierarchy)
        {

            char separator = Path.DirectorySeparatorChar;
            string outputFileDumpName = file.Split(separator)[^1] + ".dump";


            string dumpFilePath = Path.Combine(_outputDirectory, outputFileDumpName);

            try
            {
                DumpHierarchy(hierarchy, dumpFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error updating the dump file: " + e.Message);
            }

        }

        private static void DumpHierarchy(string hierarchy, string dumpFilePath)
        {
            if (File.Exists(dumpFilePath))
            {
                File.WriteAllText(dumpFilePath, hierarchy);
                Console.WriteLine("Content replaced in the dump file at: " + dumpFilePath);
            }
            else
            {
                using (StreamWriter writer = File.CreateText(dumpFilePath))
                {
                    writer.WriteLine(hierarchy);
                }
                Console.WriteLine("New dump file created at: " + dumpFilePath);
            }
        }

        private static string GetRelativePath(string projectPath, string fullPath)
        {
            return Path.GetRelativePath(projectPath, fullPath);
        }


        public void WriteUnusedScripts(string outputDirectoryPath)
        {
            List<Script> unusedScripts = new List<Script>();
            foreach (Script script in AllScripts)
            {
                bool scriptUsed = false;
                foreach (string scriptUsage in ScriptUsages)
                {
                    if (!scriptUsage.Equals(script.Guid)) continue;

                    scriptUsed = true;
                    break;
                }

                if (!scriptUsed)
                {
                    unusedScripts.Add(script);
                }
            }

            WriteUnusedScriptsToFile(unusedScripts);
        }

        private void WriteUnusedScriptsToFile(List<Script> unusedScripts)
        {
            string filePath = Path.Combine(_outputDirectory, "UnusedScripts.txt");

            using StreamWriter writer = new StreamWriter(filePath);
            
            writer.WriteLine("Relative Path,GUID");

            foreach (Script script in unusedScripts)
            {
                writer.WriteLine($"{script.Path},{script.Guid}");
            }
        }
    }
}
