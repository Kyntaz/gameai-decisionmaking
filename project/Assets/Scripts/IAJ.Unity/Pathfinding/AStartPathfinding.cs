using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using RAIN.Navigation.Graph;
using RAIN.Navigation.NavMesh;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding
{
    public class AStarPathfinding
    {
        public NavMeshPathGraph NavMeshGraph { get; protected set; }
        //how many nodes do we process on each call to the search method (assuming this method will be called every frame when there is a pathfinding process active)
        public uint NodesPerFrame { get; set; }

        //properties used for debugging purposes
        public uint TotalExploredNodes { get; protected set; }
        public int MaxOpenNodes { get; protected set; }
        public float TotalProcessingTime { get; protected set; }

        public bool InProgress { get; protected set; }

        public IOpenSet Open { get; protected set; }
        public IClosedSet Closed { get; protected set; }

        public NavigationGraphNode GoalNode { get; protected set; }
        public NavigationGraphNode StartNode { get; protected set; }
        public Vector3 StartPosition { get; protected set; }
        public Vector3 GoalPosition { get; protected set; }

        //heuristic function
        public IHeuristic Heuristic { get; protected set; }

        public AStarPathfinding(NavMeshPathGraph graph, IOpenSet open, IClosedSet closed, IHeuristic heuristic)
        {
            this.NavMeshGraph = graph;
            this.Open = open;
            this.Closed = closed;
			this.NodesPerFrame = int.MaxValue; //by default we process all nodes in a single request
            this.InProgress = false;
            this.Heuristic = heuristic;
        }

        public virtual void InitializePathfindingSearch(Vector3 startPosition, Vector3 goalPosition)
        {
            this.StartPosition = startPosition;
            this.GoalPosition = goalPosition;
            this.StartNode = this.Quantize(this.StartPosition);
            this.GoalNode = this.Quantize(this.GoalPosition);

            //if it is not possible to quantize the positions and find the corresponding nodes, then we cannot proceed
            if (this.StartNode == null || this.GoalNode == null) return;

            //I need to do this because in Recast NavMesh graph, the edges of polygons are considered to be nodes and not the connections.
            //Theoretically the Quantize method should then return the appropriate edge, but instead it returns a polygon
            //Therefore, we need to create one explicit connection between the polygon and each edge of the corresponding polygon for the search algorithm to work
            ((NavMeshPoly)this.StartNode).AddConnectedPoly(this.StartPosition);
            ((NavMeshPoly)this.GoalNode).AddConnectedPoly(this.GoalPosition);

            this.InProgress = true;
            this.TotalExploredNodes = 0;
            this.TotalProcessingTime = 0.0f;
            this.MaxOpenNodes = 0;

            var initialNode = new NodeRecord
            {
                gValue = 0,
                hValue = this.Heuristic.H(this.StartNode, this.GoalNode),
                node = this.StartNode
            };

            initialNode.fValue = AStarPathfinding.F(initialNode);

            this.Open.Initialize(); 
			this.Closed.Initialize();
            this.Open.AddToOpen(initialNode);
        }

        protected virtual void ProcessChildNode(NodeRecord parentNode, NavigationGraphEdge connectionEdge, int edgeIndex)
        {
            //this is where you process a child node 
            var childNode = GenerateChildNodeRecord(parentNode, connectionEdge);

            var openChildNode = Open.SearchInOpen(childNode);
            var closedChildNode = Closed.SearchInClosed(childNode);
            if (openChildNode == null && closedChildNode == null){
                Open.AddToOpen(childNode);
            }
            else if (openChildNode != null && openChildNode.fValue > childNode.fValue)
            {
                Open.Replace(openChildNode, childNode);
            }
            else if (closedChildNode != null && closedChildNode.fValue > childNode.fValue)
            {
                Closed.RemoveFromClosed(closedChildNode);
                Open.AddToOpen(childNode);
            }
            
        }

        public bool Search(out GlobalPath solution, bool returnPartialSolution = false)
        {
			//TODO: implement this
			//to determine the connections of the selected nodeRecord you need to look at the NavigationGraphNode' EdgeOut  list
			//something like this
			//var outConnections = bestNode.node.OutEdgeCount;
			//for (int i = 0; i < outConnections; i++)
			//{
			//this.ProcessChildNode(bestNode, bestNode.node.EdgeOut(i));

			// Start time
			float t0 = Time.realtimeSinceStartup;
			int nCycles = 0;

			NodeRecord currNode = this.Open.PeekBest();


			while (!returnPartialSolution || nCycles < this.NodesPerFrame) {
				if (this.Open.CountOpen() > this.MaxOpenNodes)
					this.MaxOpenNodes = this.Open.CountOpen();

				// A star cycle
				if (this.Open.CountOpen() == 0) {
					solution = null;
					this.InProgress = false;
					this.TotalProcessingTime += Time.realtimeSinceStartup - t0;
					return true;
				}

				currNode = this.Open.GetBestAndRemove();
				this.TotalExploredNodes++;
				nCycles++;

				if (currNode.node == this.GoalNode) {
					this.InProgress = false;
					solution = this.CalculateSolution(currNode, false);
					this.TotalProcessingTime += Time.realtimeSinceStartup - t0;
					return true;
				}

				this.Closed.AddToClosed(currNode);

				int nOutConnections = currNode.node.OutEdgeCount;
				for (int i = 0; i < nOutConnections; i++) {
					this.ProcessChildNode(currNode, currNode.node.EdgeOut(i), i);
				}
			}

			solution = this.CalculateSolution(currNode, true);
			this.TotalProcessingTime += Time.realtimeSinceStartup - t0;
			//this.InProgress = false;
			return false;
        }

        protected NavigationGraphNode Quantize(Vector3 position)
        {
            return this.NavMeshGraph.QuantizeToNode(position, 1.0f);
        }

        protected void CleanUp()
        {
            //I need to remove the connections created in the initialization process
            if (this.StartNode != null)
            {
                ((NavMeshPoly)this.StartNode).RemoveConnectedPoly();
            }

            if (this.GoalNode != null)
            {
                ((NavMeshPoly)this.GoalNode).RemoveConnectedPoly();    
            }
        }

        protected virtual NodeRecord GenerateChildNodeRecord(NodeRecord parent, NavigationGraphEdge connectionEdge)
        {
            var childNode = connectionEdge.ToNode;
            var childNodeRecord = new NodeRecord
            {
                node = childNode,
                parent = parent,
                gValue = parent.gValue + (childNode.LocalPosition-parent.node.LocalPosition).magnitude,
                hValue = this.Heuristic.H(childNode, this.GoalNode)
            };

            childNodeRecord.fValue = F(childNodeRecord);

            return childNodeRecord;
        }

        protected GlobalPath CalculateSolution(NodeRecord node, bool partial)
        {
            var path = new GlobalPath
            {
                IsPartial = partial,
                Length = node.gValue
            };
            var currentNode = node;

            path.PathPositions.Add(this.GoalPosition);

            //I need to remove the first Node and the last Node because they correspond to the dummy first and last Polygons that were created by the initialization.
            //And we don't want to be forced to go to the center of the initial polygon before starting to move towards my destination.

            //skip the last node, but only if the solution is not partial (if the solution is partial, the last node does not correspond to the dummy goal polygon)
            if (!partial && currentNode.parent != null)
            {
                currentNode = currentNode.parent;
            }
            
            while (currentNode.parent != null)
            {
                path.PathNodes.Add(currentNode.node); //we need to reverse the list because this operator add elements to the end of the list
                path.PathPositions.Add(currentNode.node.LocalPosition);

                if (currentNode.parent.parent == null) break; //this skips the first node
                currentNode = currentNode.parent;
            }

            path.PathNodes.Reverse();
            path.PathPositions.Reverse();
            return path;

        }

        public static float F(NodeRecord node)
        {
            return F(node.gValue,node.hValue);
        }

        public static float F(float g, float h)
        {
            return g + h;
        }

    }
}
