using System;
using System.Collections.Generic;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    public class LeftPriorityList : IOpenSet
    {
        private List<NodeRecord> Open { get; set; }

        public LeftPriorityList()
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
            //a little help here
            //is very nice that the List<T> already implements a binary search method
            int index = this.Open.BinarySearch(nodeRecord);
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
    }
}
