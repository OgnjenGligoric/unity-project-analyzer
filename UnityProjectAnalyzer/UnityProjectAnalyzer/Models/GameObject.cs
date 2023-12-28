using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityProjectAnalyzer.Models
{
    internal class GameObject
    {
        public String GameObjectId { get; set; }
        public String GameObjectName { get; set; }

        public GameObject(String gameObjectId, String gameObjectName)
        {
            this.GameObjectId = gameObjectId;
            this.GameObjectName = gameObjectName;
        }


        public override string ToString()
        {
            return $"GameObjectId: {GameObjectId}, GameObjectName: {GameObjectName}";
        }
    }
}
