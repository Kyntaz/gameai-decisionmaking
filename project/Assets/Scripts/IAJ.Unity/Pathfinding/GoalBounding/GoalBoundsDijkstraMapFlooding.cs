using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.GoalBounding;
using RAIN.Navigation.Graph;
using RAIN.Navigation.NavMesh;
using System.Collections.Generic;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.GoalBounding
{
    //The Dijkstra algorithm is similar to the A* but with a couple of differences
    //1) no heuristic function
    //2) it will not stop until the open list is empty
    //3) we dont need to execute the algorithm in multiple steps (because it will be executed offline)
    //4) we don't need to return any path (partial or complete)
    //5) we don't need to do anything when a node is already in closed
    public class GoalBoundsDijkstraMapFlooding
    {
        public NavMeshPathGraph NavMeshGraph { get; protected set; }
        public NavigationGraphNode StartNode { get; protected set; }
        public NodeGoalBounds NodeGoalBounds { get; protected set; }
        protected NodeRecordArray NodeRecordArray { get; set; }
        public IOpenSet Open { get; protected set; }
        public IClosedSet Closed { get; protected set; }
        
        public GoalBoundsDijkstraMapFlooding(NavMeshPathGraph graph)
        {
            this.NavMeshGraph = graph;
            //do not change this
            var nodes = this.GetNodesHack(graph);
            this.NodeRecordArray = new NodeRecordArray(nodes);
            this.Open = this.NodeRecordArray;
            this.Closed = this.NodeRecordArray;
        }

        public void Search(NavigationGraphNode startNode, NodeGoalBounds nodeGoalBounds)
        {
            this.Open.Initialize();
            this.Closed.Initialize();

            NodeRecord startNodeRecord = this.NodeRecordArray.GetNodeRecord(startNode);
            startNodeRecord.gValue = 0;
            startNodeRecord.fValue = 0;
            startNodeRecord.parent = null;
            startNodeRecord.StartNodeOutConnectionIndex = 0;
            Open.AddToOpen(startNodeRecord);

			while (true) {
				if (this.Open.CountOpen() == 0) return;

				NodeRecord currNode = this.Open.GetBestAndRemove();
				this.Closed.AddToClosed(currNode);
                nodeGoalBounds.connectionBounds[currNode.StartNodeOutConnectionIndex].UpdateBounds(currNode.node.Position);

                int nOutConnections = currNode.node.OutEdgeCount;
				for (int i = 0; i < nOutConnections; i++) {
					this.ProcessChildNode(currNode, currNode.node.EdgeOut(i), i);
				}
			}
			//TODO: Implement the algorithm that calculates the goal bounds using a dijkstra
			//Given that the nodes in the graph correspond to the edges of a polygon, we won't be able to use the vertices of the polygon to update the bounding boxes
        }

       
        protected void ProcessChildNode(NodeRecord parent, NavigationGraphEdge connectionEdge, int connectionIndex)
        {
            var childNode = connectionEdge.ToNode;
            //TODO: implement the rest of your code here
            //this is where you process a child node 
            var gValue = parent.gValue + (childNode.LocalPosition - parent.node.LocalPosition).magnitude;


            var childNodeRecord = new NodeRecord
            {
                node = childNode,
                parent = parent,
                gValue = gValue,
                fValue = gValue
            };

            if (parent.parent == null)
                childNodeRecord.StartNodeOutConnectionIndex = connectionIndex;
            else
                childNodeRecord.StartNodeOutConnectionIndex = parent.StartNodeOutConnectionIndex;

            var openChildNode = Open.SearchInOpen(childNodeRecord);
            var closedChildNode = Closed.SearchInClosed(childNodeRecord);

            if (openChildNode == null && closedChildNode == null)
            {
                Open.AddToOpen(childNodeRecord);
            }
            else if (openChildNode != null && openChildNode.fValue > childNodeRecord.fValue)
            {
                Open.Replace(openChildNode, childNodeRecord);
            }
            
            //TODO: Implement this method that processes a child node. Then you can use it in the Search method above.
        }

        private List<NavigationGraphNode> GetNodesHack(NavMeshPathGraph graph)
        {
            //this hack is needed because in order to implement NodeArrayA* you need to have full acess to all the nodes in the navigation graph in the beginning of the search
            //unfortunately in RAINNavigationGraph class the field which contains the full List of Nodes is private
            //I cannot change the field to public, however there is a trick in C#. If you know the name of the field, you can access it using reflection (even if it is private)
            //using reflection is not very efficient, but it is ok because this is only called once in the creation of the class
            //by the way, NavMeshPathGraph is a derived class from RAINNavigationGraph class and the _pathNodes field is defined in the base class,
            //that's why we're using the type of the base class in the reflection call
            return (List<NavigationGraphNode>)Utils.Reflection.GetInstanceField(typeof(RAINNavigationGraph), graph, "_pathNodes");
        }

    }
}
