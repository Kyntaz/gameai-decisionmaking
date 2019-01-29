using RAIN.Navigation.Graph;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics
{
	public class AvoidWallHeuristic : IHeuristic
	{
		public float H(NavigationGraphNode node, NavigationGraphNode goalNode)
		{
			Vector3 dir = goalNode.Position - node.Position;
			float distance = dir.magnitude;
			if (!Physics.Raycast(node.Position, dir, dir.magnitude)) {
				return distance;
			} else {
				return distance * 1.2f;
			}
		}
	}
}
