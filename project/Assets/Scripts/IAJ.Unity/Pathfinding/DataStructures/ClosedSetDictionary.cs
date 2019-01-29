using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RAIN.Navigation.Graph;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    public class ClosedSetDictionary : IClosedSet
    {
        Dictionary<NavigationGraphNode, NodeRecord> dictionary;
        public ClosedSetDictionary(){ dictionary = new Dictionary<NavigationGraphNode, NodeRecord>(); }

        public void Initialize() { dictionary.Clear(); }

        public void AddToClosed(NodeRecord nodeRecord) { dictionary.Add(nodeRecord.node, nodeRecord); }

        public void RemoveFromClosed(NodeRecord nodeRecord) { dictionary.Remove(nodeRecord.node); }

        //should return null if the node is not found
        public NodeRecord SearchInClosed(NodeRecord nodeRecord)
        {
            NodeRecord result;
            if (dictionary.TryGetValue(nodeRecord.node, out result)) return result;
            return null;
        }

        public ICollection<NodeRecord> All() { return dictionary.Values; }
    }

}