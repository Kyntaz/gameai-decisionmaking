using UnityEngine;
using UnityEditor;
using RAIN.Navigation.NavMesh;
using System.Collections.Generic;
using RAIN.Navigation.Graph;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.GoalBounding;
using Assets.Scripts.IAJ.Unity.Pathfinding.GoalBounding;
using System.Threading;

public class IAJMenuItems 
{
	public const int MAX_THREADS = 8;

	[MenuItem("IAJ/Calculate Goal Bounds")]
    private static void CalculateGoalBounds()
    {
        //get the NavMeshGraph from the current scene
        NavMeshPathGraph navMesh = GameObject.Find("Navigation Mesh").GetComponent<NavMeshRig>().NavMesh.Graph;

		//this is needed because RAIN AI does some initialization the first time the QuantizeToNode method is called
		//if this method is not called, the connections in the navigationgraph are not properly initialized
		navMesh.QuantizeToNode (new Vector3 (0, 0, 0), 1.0f);

        GoalBoundingTable goalBoundingTable = ScriptableObject.CreateInstance<GoalBoundingTable>();
        var nodes = GetNodesHack(navMesh);
		goalBoundingTable.table = new NodeGoalBounds[nodes.Count];

		// List of threads to wait.
		List<Thread> threads = new List<Thread>();

        //calculate goal bounds for each edge
        for (int i=0; i < nodes.Count; i++)
        {
            if(nodes[i] is NavMeshEdge)
            {
				var dijkstra = new GoalBoundsDijkstraMapFlooding(navMesh);
				NodeGoalBounds auxGoalBounds;

                //initialize the GoalBounds structure for the edge
                auxGoalBounds = ScriptableObject.CreateInstance<NodeGoalBounds>();
                auxGoalBounds.connectionBounds = new Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.GoalBounding.Bounds[nodes[i].OutEdgeCount];
                for (int j = 0; j < nodes[i].OutEdgeCount; j++)
                {
                    auxGoalBounds.connectionBounds[j] = ScriptableObject.CreateInstance<Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.GoalBounding.Bounds>();
                    auxGoalBounds.connectionBounds[j].InitializeBounds(nodes[i].Position);
                }

                if(i%10 == 0)
                {
                    float percentage = (float)i / (float)nodes.Count;
                    EditorUtility.DisplayProgressBar("GoalBounding precomputation progress", "Calculating goal bounds for each edge", percentage);
                }

                //run a Dijkstra mapflooding for each node
				//run in a new thread:
                /*
                dijkstra.Search(nodes[i], auxGoalBounds);

                goalBoundingTable.table[i] = auxGoalBounds;
				*/

				var searchThread = new DijkstraSearchThread(nodes[i], auxGoalBounds, dijkstra, goalBoundingTable, i);
				Thread t = new Thread(new ThreadStart(searchThread.Search));
				threads.Add(t);

				t.Start();

				if (threads.Count >= MAX_THREADS) {
					threads[0].Join();
					threads.RemoveAt(0);
				}
            }
        }

		foreach (Thread t in threads) {
			t.Join();
		}
        
		//saving the assets, this takes forever using Unity's serialization mechanism
        goalBoundingTable.SaveToAssetDatabase();
        EditorUtility.ClearProgressBar();
    }


    private static List<NavigationGraphNode> GetNodesHack(NavMeshPathGraph graph)
    {
        //this hack is needed because in order to implement NodeArrayA* you need to have full acess to all the nodes in the navigation graph in the beginning of the search
        //unfortunately in RAINNavigationGraph class the field which contains the full List of Nodes is private
        //I cannot change the field to public, however there is a trick in C#. If you know the name of the field, you can access it using reflection (even if it is private)
        //using reflection is not very efficient, but it is ok because this is only called once in the creation of the class
        //by the way, NavMeshPathGraph is a derived class from RAINNavigationGraph class and the _pathNodes field is defined in the base class,
        //that's why we're using the type of the base class in the reflection call
        return (List<NavigationGraphNode>)Assets.Scripts.IAJ.Unity.Utils.Reflection.GetInstanceField(typeof(RAINNavigationGraph), graph, "_pathNodes");
    }

	private class DijkstraSearchThread {
		private NavigationGraphNode startNode;
		private NodeGoalBounds bounds;
		private GoalBoundsDijkstraMapFlooding dijkstra;
		private GoalBoundingTable goalBoundingTable;
		private int index;

		public DijkstraSearchThread(NavigationGraphNode startNode, NodeGoalBounds bounds, GoalBoundsDijkstraMapFlooding dijkstra, GoalBoundingTable goalBoundingTable, int index) {
			this.startNode = startNode;
			this.bounds = bounds;
			this.dijkstra = dijkstra;
			this.goalBoundingTable = goalBoundingTable;
			this.index = index;
		}

		public void Search() {
			this.dijkstra.Search(startNode, bounds);
			this.goalBoundingTable.table[index] = bounds;
		}
	}
}
