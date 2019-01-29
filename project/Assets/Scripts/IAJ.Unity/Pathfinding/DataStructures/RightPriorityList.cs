using System;
using System.Collections.Generic;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    public class RightPriorityList : IOpenSet, IComparer<NodeRecord>
    {
        private List<NodeRecord> Open { get; set; }

        public RightPriorityList()
        {
            this.Open = new List<NodeRecord>();    
        }
        public void Initialize()
        {
            //TODO implement
            throw new NotImplementedException();
        }

        public void Replace(NodeRecord nodeToBeReplaced, NodeRecord nodeToReplace)
        {
            //TODO implement
            throw new NotImplementedException();
        }

        public NodeRecord GetBestAndRemove()
        {
            //TODO implement
            throw new NotImplementedException();
        }

        public NodeRecord PeekBest()
        {
            //TODO implement
            throw new NotImplementedException();
        }

        public void AddToOpen(NodeRecord nodeRecord)
        {
            //a little help here, notice the difference between this method and the one for the LeftPriority list
            //...this one uses a different comparer with an explicit compare function (which you will have to define below)
            int index = this.Open.BinarySearch(nodeRecord,this);
            if (index < 0)
            {
                this.Open.Insert(~index, nodeRecord);
            }
        }

        public void RemoveFromOpen(NodeRecord nodeRecord)
        {
            //TODO implement
            throw new NotImplementedException();
        }

        public NodeRecord SearchInOpen(NodeRecord nodeRecord)
        {
            //TODO implement
            throw new NotImplementedException();
        }

        public ICollection<NodeRecord> All()
        {
            //TODO implement
            throw new NotImplementedException();
        }

        public int CountOpen()
        {
            //TODO implement
            throw new NotImplementedException();
        }

        public int Compare(NodeRecord x, NodeRecord y)
        {
            //TODO implement
            throw new NotImplementedException();
        }
    }
}
