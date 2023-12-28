using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityProjectAnalyzer.Models;
using UnityProjectAnalyzer.Utils;
using YamlDotNet.RepresentationModel;

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
                            hierarchy.Add(new StringBuilder().Insert(0, "--", depth).ToString() + gameObjectName);
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
                        }
                        else if (isTransformOfGameObject)
                        {
                            if (!line.Contains("m_Father: {fileID: ")) continue;


                            UnityFileExtractor unityFileExtractor = new UnityFileExtractor();
                            string fatherId = unityFileExtractor.ExtractNumberFromFatherId(line);
                            if (fatherId == "0")
                            {
                                return 0;
                            }

                            return GoThroughAllFathers(fatherId);
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

            return 0;
        }

        private int GoThroughAllFathers(string fatherId, int i = 1)
        {

            if (File.Exists(_scenePath))
            {
                string[] lines = File.ReadAllLines(_scenePath);
                bool inTransformSection = false;
                foreach (string line in lines)
                {
                    if (line.Contains("--- !u!4 &" + fatherId))
                    {
                        inTransformSection = true;
                    }
                    else if (inTransformSection)
                    {
                        if (line.Contains("m_Father: {fileID: "))
                        {
                            UnityFileExtractor unityFileExtractor = new UnityFileExtractor();
                            string nextFatherId = unityFileExtractor.ExtractNumberFromFatherId(line);
                            if (nextFatherId == "0")
                            {
                                return i;
                            }

                            return GoThroughAllFathers(nextFatherId, i + 1);
                        }

                        if (line.Contains("---"))
                        {
                            inTransformSection = false;

                        }
                    }

                }
            }
            else
            {
                Console.WriteLine("File not found!");
            }

            return i;

        }


        public List<Transform> getAllTransforms()
        {
            List<Transform> transforms = new List<Transform>();
            String transformId = "";
            String gameObjectId = "";
            if (File.Exists(_scenePath))
            {
                string[] lines = File.ReadAllLines(_scenePath);
                bool inTransformSection = false;
                bool inTransformChildrenSection = false;
                List<String> transformChildren = new List<String>();

                foreach (string line in lines)
                {
                    if (line.Contains("--- !u!4 &"))
                    {
                        transformId = line.Split('&')[1].Trim();
                        inTransformSection = true;
                    }
                    else if (inTransformSection)
                    {
                        var regex = new Regex(@"m_GameObject:\s+\{fileID:\s+(\d+)");
                        var matchGameObject = regex.Match(line);

                        if (matchGameObject.Success)
                        {
                            gameObjectId = matchGameObject.Groups[1].Value;

                        }
                        else if (line.Contains("m_Children:"))
                        {
                            inTransformChildrenSection = true;
                            transformChildren = new List<String>();
                        }
                        else if (inTransformChildrenSection)
                        {
                            if (!line.Contains("- {fileID: "))
                            {
                                inTransformChildrenSection = false;
                            }
                            else
                            {
                                string patternChildrenTransformId = @"fileID:\s*(\d+)";

                                Match match = Regex.Match(line, patternChildrenTransformId);

                                if (match.Success)
                                {
                                    transformChildren.Add(match.Groups[1].Value);
                                }
                            }

                            
                        }
                        if (line.Contains("---"))
                        {
                            if (inTransformSection)
                            {
                                Transform transform = new Transform(transformId, gameObjectId, transformChildren);
                                transforms.Add(transform);
                            }

                            inTransformSection = false;

                        }

                    }
                }

                


            }
            return transforms;

        }

        public List<GameObject> getAllGameObjects()
        {
            List<GameObject> gameObjects = new List<GameObject>();
            String gameObjectId = "";
            String gameObjectName = "";
            if (File.Exists(_scenePath))
            {
                string[] lines = File.ReadAllLines(_scenePath);
                bool inGameObjectSection = false;

                foreach (string line in lines)
                {
                    if (line.Contains("--- !u!1 &"))
                    {
                        gameObjectId = line.Split('&')[1].Trim();
                        inGameObjectSection = true;
                    }
                    else if (inGameObjectSection)
                    {
                        // Regular expression pattern to extract the string value of m_Name
                        string pattern = @"m_Name:\s*(.+)";

                        Match match = Regex.Match(line, pattern);

                        if (match.Success)
                        {
                            gameObjectName = match.Groups[1].Value;
                        }

                        if (line.Contains("---"))
                        {
                            if (inGameObjectSection)
                            {
                                GameObject gameObject = new GameObject(gameObjectId, gameObjectName);
                                gameObjects.Add(gameObject);
                            }

                            inGameObjectSection = false;

                        }

                    }
                }

            }
            return gameObjects;

        }
    }
}