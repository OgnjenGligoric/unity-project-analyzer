using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityProjectAnalyzer.Models;

namespace UnityProjectAnalyzer.Utils
{
    internal class HierarchyBuilder
    {
        private List<Transform> transforms;
        private List<GameObject> gameObjects;

        public HierarchyBuilder(List<Transform> transforms, List<GameObject> gameObjects)
        {
            this.transforms = transforms;
            this.gameObjects=gameObjects;
        }

        public string GetHierarchy()
        {
            StringBuilder stringBuilder = new StringBuilder();
           
            foreach (Transform transform in transforms)
            {
                if(isChild(transform.TransformId)) continue;
                stringBuilder.Append( GetGameObjectName(transform.GameObjectId)+"\n");
                WriteChildren(transform.ChildrenTransformIds, stringBuilder, 1);
            }
           

            return stringBuilder.ToString();
        }

        private void WriteChildren(List<string> transformChildrenTransformIds, StringBuilder stringBuilder, int i)
        {
            foreach (string childId in transformChildrenTransformIds)
            {
                // Indentation logic based on 'i' (depth)
                stringBuilder.Append(new string('-', i * 2)); 
                stringBuilder.Append(GetGameObjectName(findTransformById(childId).GameObjectId) + "\n");

                // If you have child hierarchy, call this method recursively
                // For example, assuming you have a method to get children of a particular child ID
                List<string> grandchildren = GetChildren(childId); // Get the children of the current ID
                WriteChildren(grandchildren, stringBuilder, i + 1); // Recursive call for the children
            }

        }

        private Transform findTransformById(string transformId)
        {
            foreach (Transform transform in transforms)
            {
                if(transform.TransformId == transformId) return transform;
            }

            return null;
        }

        private List<string> GetChildren(string childId)
        {
            foreach (Transform transform in transforms)
            {
                if (childId == transform.TransformId)
                {
                    return transform.ChildrenTransformIds;
                }
            }
            return new List<string>();
        }

        private bool isChild(String transformId)
        {
            foreach (Transform transform in transforms)
            {
                foreach (String transformChildrenTransformId in transform.ChildrenTransformIds)
                {
                    if(transformChildrenTransformId.Equals(transformId)) return true;
                }
            }

            return false;
        }

        private string GetGameObjectName(string gameObjectId)
        {

            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject.GameObjectId == gameObjectId)
                {
                    return gameObject.GameObjectName;
                }
            }
            
            return "Null"; 
        }
    }
}
