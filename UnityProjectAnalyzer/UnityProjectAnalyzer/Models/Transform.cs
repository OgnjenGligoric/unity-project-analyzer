using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityProjectAnalyzer.Models
{
    public class Transform
    {
        public String TransformId { get; set; }
        public String GameObjectId { get; set; }
        public List<String> ChildrenTransformIds;

        public Transform(String transformId, String gameObjectId, List<String> childrenTransformIds)
        {
            this.TransformId = transformId;
            this.GameObjectId = gameObjectId;
            this.ChildrenTransformIds = childrenTransformIds;
        }

        public Transform()
        {
        }

        public override string ToString()
        {
            string childrenIds = ChildrenTransformIds != null ? string.Join(", ", ChildrenTransformIds) : "No Children";
            return $"TransformId: {TransformId}, GameObjectId: {GameObjectId}, ChildrenTransformIds: [{childrenIds}]";
        }
    }
}
