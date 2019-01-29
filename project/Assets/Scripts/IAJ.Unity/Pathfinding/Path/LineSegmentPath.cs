using Assets.Scripts.IAJ.Unity.Utils;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Path
{
    public class LineSegmentPath : LocalPath
    {
		protected Vector3 LineVector { get; set;}
		public float StartParam { get; protected set; }
		public float EndParam { get; protected set; }
		public float PathEndParam { get; protected set; }
		public float ParamSize { get; protected set; }

        public LineSegmentPath(float startParam, Vector3 start, Vector3 end)
        {
            this.StartPosition = start;
            this.EndPosition = end;
            this.LineVector = end - start;
			this.StartParam = startParam;
			this.ParamSize = this.LineVector.magnitude;
			this.EndParam = this.StartParam + this.ParamSize;
			this.PathEndParam = this.EndParam - MathConstants.EPSILON;
        }

        public override Vector3 GetPosition(float param)
        {
			if (param > this.EndParam) 
			{
				return this.EndPosition;
			} 
			else if (param < this.StartParam) 
			{
				return this.StartPosition;
			}
			else return this.StartPosition + this.LineVector*(param-this.StartParam)/this.ParamSize;
        }

        public override bool PathEnd(float param)
        {
			return param > PathEndParam;
        }

        public override float GetParam(Vector3 position, float lastParam)
        {
			var internalParam = MathHelper.closestParamInLineSegmentToPoint(this.StartPosition, this.EndPosition, position);
			return this.StartParam + internalParam * this.ParamSize;
        }
    }
}
