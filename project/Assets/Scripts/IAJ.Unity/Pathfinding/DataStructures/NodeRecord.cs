using System;
using RAIN.Navigation.Graph;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    public enum NodeStatus
    {
        Unvisited,
        Open,
        Closed
    }

    public class NodeRecord  : IComparable<NodeRecord>
    {
        public NavigationGraphNode node;
        public NodeRecord parent;
        public float gValue;
        public float hValue;
        public float fValue;
        public NodeStatus status;

        //used only for the GoalBounds
        public int StartNodeOutConnectionIndex;

        public int CompareTo(NodeRecord other)
        {
            return this.fValue.CompareTo(other.fValue);
        }

        //two node records are equal if they refer to the same node
        public override bool Equals(object obj)
        {
            var target = obj as NodeRecord;
            if (target != null) return this.node == target.node;
            else return false;
        }

        public override int GetHashCode()
        {
            return this.node.GetHashCode();
        }
    }
}
