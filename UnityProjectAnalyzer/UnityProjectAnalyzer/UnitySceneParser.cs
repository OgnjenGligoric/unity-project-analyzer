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


        public List<Transform> GetAllTransforms()
        {
            List<Transform> transforms = new List<Transform>();
            String transformId = "";
            String gameObjectId = "";

            if (!File.Exists(_scenePath)) return transforms;

            string[] lines = File.ReadAllLines(_scenePath);
            bool inTransformSection = false;
            bool inTransformChildrenSection = false;
            List<String> transformChildren = new List<String>();

            ParseAllTransforms(lines, transformId, inTransformSection, gameObjectId, inTransformChildrenSection, transformChildren, transforms);

            return transforms;

        }

        private static void ParseAllTransforms(string[] lines, string transformId, bool inTransformSection, string gameObjectId,
            bool inTransformChildrenSection, List<string> transformChildren, List<Transform> transforms)
        {
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

                    if (!line.Contains("---")) continue;

                    if (inTransformSection)
                    {
                        Transform transform = new Transform(transformId, gameObjectId, transformChildren);
                        transforms.Add(transform);
                    }

                    inTransformSection = false;
                }
            }
        }

        public List<GameObject> GetAllGameObjects()
        {
            List<GameObject> gameObjects = new List<GameObject>();
            String gameObjectId = "";
            String gameObjectName = "";
            if (!File.Exists(_scenePath)) return gameObjects;

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

                    if (!line.Contains("---")) continue;

                    if (inGameObjectSection)
                    {
                        GameObject gameObject = new GameObject(gameObjectId, gameObjectName);
                        gameObjects.Add(gameObject);
                    }

                    inGameObjectSection = false;

                }
            }
            return gameObjects;

        }

        public void GetAllUsages(HashSet<string> scriptUsages)
        {
            if (!File.Exists(_scenePath)) return;

            string[] lines = File.ReadAllLines(_scenePath);

            foreach (string line in lines)
            {
                string pattern = @"m_Script:\s*\{.*guid:\s*(\w+)";
                Match match = Regex.Match(line, pattern);

                if (!match.Success) continue;

                // Extract the guid value

                string guidValue = match.Groups[1].Value;
                scriptUsages.Add(guidValue);
                Console.WriteLine("GUID: " + guidValue);
            }

        }
    }
}