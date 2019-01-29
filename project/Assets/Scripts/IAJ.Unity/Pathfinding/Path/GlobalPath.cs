using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.Utils;
using RAIN.Navigation.Graph;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Path
{
    public class GlobalPath : Path
    {
        public List<NavigationGraphNode> PathNodes { get; protected set; }
        public List<Vector3> PathPositions { get; protected set; } 
        public bool IsPartial { get; set; }
        public float Length { get; set; }
		public List<LineSegmentPath> LocalPaths { get; protected set; }
		public float EndParam { get; protected set; }
		protected int CurrentLocalPathIndex { get; set; }


        public GlobalPath()
        {
            this.PathNodes = new List<NavigationGraphNode>();
            this.PathPositions = new List<Vector3>();
            this.LocalPaths = new List<LineSegmentPath>();
        }

        public void CalculateLocalPathsFromPathPositions(Vector3 initialPosition)
        {
            Vector3 previousPosition = initialPosition;
			float currentParam = 0;
            for (int i = 0; i < this.PathPositions.Count; i++)
            {
				if(!previousPosition.Equals(this.PathPositions[i]))
				{
					var localPath = new LineSegmentPath (currentParam, previousPosition, this.PathPositions [i]);
					this.LocalPaths.Add(localPath);
					currentParam = localPath.EndParam;
					previousPosition = this.PathPositions[i];
				}
            }
			
			this.EndParam = currentParam;

			this.CurrentLocalPathIndex = 0;
        }

        public override float GetParam(Vector3 position, float previousParam)
        {
			List<int> indexList = new List<int> ();
			indexList.Add (this.CurrentLocalPathIndex);
			indexList.Add (this.CurrentLocalPathIndex+1);
			indexList.Add (this.CurrentLocalPathIndex+2);
			//indexList.Add (this.CurrentLocalPathIndex+3);
			float shortedDistance = float.PositiveInfinity;
			float bestParam = 0.0f;

			foreach (var index in indexList) 
			{
				if (index < this.LocalPaths.Count) 
				{
					var localPath = this.LocalPaths [index];
					var param = localPath.GetParam (position, previousParam);
					var localPos = localPath.GetPosition (param);
					var sqrDistance = (localPos - position).sqrMagnitude;
					if (sqrDistance < shortedDistance) 
					{
						shortedDistance = sqrDistance;
						this.CurrentLocalPathIndex = index;
						bestParam = param;
					}
				}	

			}

			return bestParam;
        }

        public override Vector3 GetPosition(float param)
        {
			foreach (var localPath in this.LocalPaths) 
			{
				if (localPath.EndParam > param) 
				{
					return localPath.GetPosition (param);
				}
			}

			return this.LocalPaths [this.LocalPaths.Count - 1].GetPosition (param);
        }

        public override bool PathEnd(float param)
        {
				return param > this.EndParam - MathConstants.EPSILON;
        }
    }
}
