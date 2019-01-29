using System;
using System.Collections.Generic;
using RAIN.Navigation.Graph;
using System.Linq;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    public class NodeRecordArray : IOpenSet, IClosedSet
    {
        private NodeRecord[] NodeRecords { get; set; }
        private List<NodeRecord> SpecialCaseNodes { get; set; } 
        private NodePriorityHeap Open { get; set; }

        public NodeRecordArray(List<NavigationGraphNode> nodes)
        {
            //this method creates and initializes the NodeRecordArray for all nodes in the Navigation Graph
            this.NodeRecords = new NodeRecord[nodes.Count];
            
            for(int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                node.NodeIndex = i; //we're setting the node Index because RAIN does not do this automatically
                this.NodeRecords[i] = new NodeRecord {node = node, status = NodeStatus.Unvisited};
            }

            this.SpecialCaseNodes = new List<NodeRecord>();

            this.Open = new NodePriorityHeap();
        }

        public NodeRecord GetNodeRecord(NavigationGraphNode node)
        {
            //do not change this method
            //here we have the "special case" node handling
            if (node.NodeIndex == -1)
            {
                for (int i = 0; i < this.SpecialCaseNodes.Count; i++)
                {
                    if (node == this.SpecialCaseNodes[i].node)
                    {
                        return this.SpecialCaseNodes[i];
                    }
                }
                return null;
            }
            else
            {
                return  this.NodeRecords[node.NodeIndex];
            }
        }

        public void AddSpecialCaseNode(NodeRecord node)
        {
            this.SpecialCaseNodes.Add(node);
        }

        void IOpenSet.Initialize()
        {
            this.Open.Initialize();
            //we want this to be very efficient (that's why we use for)
            for (int i = 0; i < this.NodeRecords.Length; i++)
            {
                this.NodeRecords[i].status = NodeStatus.Unvisited;
            }

            this.SpecialCaseNodes.Clear();
        }

        void IClosedSet.Initialize()
        {
			//this.Open.Initialize();
			for (int i = 0; i < this.NodeRecords.Length; i++)
			{
				this.NodeRecords[i].status = NodeStatus.Unvisited;
			}
			this.SpecialCaseNodes.Clear();
        }

        public void AddToOpen(NodeRecord nodeRecord)
		{
			var record = this.GetNodeRecord(nodeRecord.node);
			record.fValue = nodeRecord.fValue;
			record.gValue = nodeRecord.gValue;
			record.hValue = nodeRecord.hValue;
			record.parent = nodeRecord.parent;
            record.StartNodeOutConnectionIndex = nodeRecord.StartNodeOutConnectionIndex;
            record.status = NodeStatus.Open;
			this.Open.AddToOpen(record);
        }

        public void AddToClosed(NodeRecord nodeRecord)
        {
			var record = this.GetNodeRecord(nodeRecord.node);
			record.fValue = nodeRecord.fValue;
			record.gValue = nodeRecord.gValue;
			record.hValue = nodeRecord.hValue;
			record.parent = nodeRecord.parent;
            record.StartNodeOutConnectionIndex = nodeRecord.StartNodeOutConnectionIndex;

            record.status = NodeStatus.Closed;
        }

        public NodeRecord SearchInOpen(NodeRecord nodeRecord)
        {
			var record = this.GetNodeRecord(nodeRecord.node);
			return record.status == NodeStatus.Open ? record : null;
        }

        public NodeRecord SearchInClosed(NodeRecord nodeRecord)
        {
			var record = this.GetNodeRecord(nodeRecord.node);
			return record.status == NodeStatus.Closed ? record : null;
        }

        public NodeRecord GetBestAndRemove()
        {
			return this.Open.GetBestAndRemove();
        }

        public NodeRecord PeekBest()
        {
			return this.Open.PeekBest();
        }

        public void Replace(NodeRecord nodeToBeReplaced, NodeRecord nodeToReplace)
        {
			var record = this.GetNodeRecord(nodeToBeReplaced.node);
			record.fValue = nodeToReplace.fValue;
			record.gValue = nodeToReplace.gValue;
			record.hValue = nodeToReplace.hValue;
			record.parent = nodeToReplace.parent;
			record.status = NodeStatus.Open;
			this.Open.Replace(nodeToBeReplaced, nodeToReplace);
        }

        public void RemoveFromOpen(NodeRecord nodeRecord)
        {
			this.Open.RemoveFromOpen(nodeRecord);
        }

        public void RemoveFromClosed(NodeRecord nodeRecord)
        {
			nodeRecord.status = NodeStatus.Unvisited;
        }

        ICollection<NodeRecord> IOpenSet.All()
        {
			return this.Open.All();
        }

        ICollection<NodeRecord> IClosedSet.All()
        {
			var query = from node in this.NodeRecords
					where node.status == NodeStatus.Closed
				select node;
			return query.ToList();
        }

        public int CountOpen()
        {
			return this.Open.CountOpen();
        }
    }
}
