using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityProjectAnalyzer.Utils;

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
                String gameObjectId = "";
                foreach (string line in lines)
                {
                    if (line.Contains("--- !u!1 &"))
                    {
                        gameObjectId = line.Split('&')[1].Trim();
                        inGameObjectSection = true;
                    }
                    else if (inGameObjectSection)
                    {
                        Match match = Regex.Match(line, @"\s+m_Name: (.+)");
                        if (match.Success)
                        {
                            string gameObjectName = line.Split(": ")[1].Trim();
                            int depth = GetDepthForGameObject(gameObjectId);
                            hierarchy.Add(gameObjectName);
                        }
                        else if (line.Contains("---"))
                        {
                            inGameObjectSection = false;
                            gameObjectId = "";
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
        public int GetDepthForGameObject(string gameObjectId)
        {
            int depth = 0;
            if (File.Exists(_scenePath))
            {
                string[] lines = File.ReadAllLines(_scenePath);
                bool inTransformSection = false;
                bool isTransformOfGameObject = false;
                foreach (string line in lines)
                {
                    if (line.Contains("--- !u!4 &"))
                    {
                        inTransformSection = true;
                    }
                    else if (inTransformSection)
                    {
                        if (line.Contains("m_GameObject: {fileID: " + gameObjectId + "}"))
                        {
                            isTransformOfGameObject = true;
                            string gameObjectName = line.Split(": ")[1].Trim();
                        }else if (isTransformOfGameObject)
                        {
                            if (line.Contains("m_Father: {fileID: "))
                            {
                                UnityFileExtractor unityFileExtractor = new UnityFileExtractor();
                                string fatherId = unityFileExtractor.ExtractNumberFromFatherId(line);
                                if (fatherId == "0")
                                {
                                    return depth;
                                }

                            }
                        }
                        else if (line.Contains("---"))
                        {
                            inTransformSection = false;
                            gameObjectId = "";
                        }
                    }

                }
            }
            else
            {
                Console.WriteLine("File not found!");
            }
            return depth;
        }
    }
}
