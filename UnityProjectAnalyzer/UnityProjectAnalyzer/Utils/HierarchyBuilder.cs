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
        private readonly List<Transform> _transforms;
        private readonly List<GameObject> _gameObjects;

        public HierarchyBuilder(List<Transform> transforms, List<GameObject> gameObjects)
        {
            this._transforms = transforms;
            this._gameObjects=gameObjects;
        }

        public string GetHierarchy()
        {
            StringBuilder stringBuilder = new StringBuilder();
           
            foreach (var transform in _transforms.Where(transform => !IsChild(transform.TransformId)))
            {
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
                stringBuilder.Append(GetGameObjectName(FindTransformById(childId).GameObjectId) + "\n");

                // If you have child hierarchy, call this method recursively
                // For example, assuming you have a method to get children of a particular child ID
                List<string> grandchildren = GetChildren(childId); // Get the children of the current ID
                WriteChildren(grandchildren, stringBuilder, i + 1); // Recursive call for the children
            }

        }

        private Transform FindTransformById(string transformId)
        {
            foreach (Transform transform in _transforms)
            {
                if(transform.TransformId == transformId) return transform;
            }

            return null;
        }

        private List<string> GetChildren(string childId)
        {
            foreach (var transform in _transforms.Where(transform => childId == transform.TransformId))
            {
                return transform.ChildrenTransformIds;
            }

            return new List<string>();
        }

        private bool IsChild(String transformId)
        {
            return _transforms.Any(transform => transform.ChildrenTransformIds.Any(transformChildrenTransformId => transformChildrenTransformId.Equals(transformId)));
        }

        private string GetGameObjectName(string gameObjectId)
        {
            foreach (var gameObject in _gameObjects.Where(gameObject => gameObject.GameObjectId == gameObjectId))
            {
                return gameObject.GameObjectName;
            }

            return "Null";
        }
    }
}
