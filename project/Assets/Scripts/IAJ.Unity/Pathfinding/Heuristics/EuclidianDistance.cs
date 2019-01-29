using RAIN.Navigation.Graph;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics
{
    public class EuclideanDistance : IHeuristic
    {
        public float H(NavigationGraphNode node, NavigationGraphNode goalNode)
        {
			return (node.Position - goalNode.Position).magnitude;
        }
    }
}
