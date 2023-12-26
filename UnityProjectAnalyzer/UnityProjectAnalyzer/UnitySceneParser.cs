using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UnityProjectAnalyzer
{
    internal class UnitySceneParser
    {
        private String _scenePath { get; set; }

        public UnitySceneParser(String scenePath)
        {
            _scenePath = scenePath;
        }

        public List<string> ParseUnityScene()
        {
            List<string> hierarchy = new List<string>();

            if (File.Exists(_scenePath))
            {
                string[] lines = File.ReadAllLines(_scenePath);

                bool inGameObjectSection = false;
                int tabCount = 0;

                foreach (string line in lines)
                {
                    if (line.Contains("--- !u!1 &"))
                    {
                        String gameObjectId = line.Split('&')[1].Trim();
                        inGameObjectSection = true;
                    }
                    else if (inGameObjectSection)
                    {
                        Match match = Regex.Match(line, @"\s+m_Name: (.+)");
                        if (match.Success)
                        {
                            string gameObjectName = line.Split(": ")[1].Trim();
                            hierarchy.Add(gameObjectName);
                        }
                        else if (line.Contains("---"))
                        {
                            inGameObjectSection = false;
                        }
                    }

                    // Track tab counts for nested GameObjects
                    tabCount = Math.Max(0, tabCount + (line.Split('-').Length - 1));
                }
            }
            else
            {
                Console.WriteLine("File not found!");
            }

            return hierarchy;
        }
        static List<string> ParseUnityTransitionForGameObject(string filePath)
        {
            List<string> hierarchy = new List<string>();

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                bool inGameObjectSection = false;
                int tabCount = 0;

                foreach (string line in lines)
                {
                    if (line.Contains("GameObject:"))
                    {
                        inGameObjectSection = true;
                    }
                    else if (inGameObjectSection)
                    {
                        Match match = Regex.Match(line, @"\s+m_Name: (.+)");
                        if (match.Success)
                        {
                            string gameObjectName = match.Groups[1].Value.Trim();
                            string formattedName = new string('\t', tabCount) + gameObjectName;
                            hierarchy.Add(formattedName);
                        }
                        else if (line.Contains("---"))
                        {
                            inGameObjectSection = false;
                        }
                    }

                    // Track tab counts for nested GameObjects
                    tabCount = Math.Max(0, tabCount + (line.Split('-').Length - 1));
                }
            }
            else
            {
                Console.WriteLine("File not found!");
            }

            return hierarchy;
        }
    }
}
