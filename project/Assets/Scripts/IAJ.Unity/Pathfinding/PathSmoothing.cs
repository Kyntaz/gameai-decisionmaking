using System;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using System.Collections.Generic;
using RAIN.Navigation.Graph;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding
{
    public class PathSmoothing
    {

        private static bool Walkable(Vector3 p1, Vector3 p2)
        {
            Vector3 vec = p2 - p1;
			Vector3 o = p1;
			return !Physics.Raycast(o, vec.normalized, vec.magnitude);
        }

        public static GlobalPath SmoothPath(Vector3 position, GlobalPath path){
			int i = 0;
			while (i < path.PathPositions.Count - 2)
            {
                if(Walkable(path.PathPositions[i], path.PathPositions[i+2])){
					path.PathPositions.RemoveAt(i+1);
				} else i++;
            }
            return path;
        }
    }
}
