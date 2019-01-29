using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.GameManager;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTS
    {
        public const float C = 1.4f;
        public bool InProgress { get; private set; }
        public int MaxIterations { get; set; }
        public int MaxIterationsProcessedPerFrame { get; set; }
        public int MaxPlayoutDepthReached { get; protected set; }
        public int MaxSelectionDepthReached { get; protected set; }
        public float TotalProcessingTime { get; private set; }
        public MCTSNode BestFirstChild { get; set; }
        public List<GOB.Action> BestActionSequence { get; private set; }
        public int NPlayouts;
		public int Nnodes;


        public int CurrentIterations { get; set; }
        protected int CurrentIterationsInFrame { get; set; }
        protected int CurrentDepth { get; set; }

        protected CurrentStateWorldModel CurrentStateWorldModel { get; set; }
        protected MCTSNode InitialNode { get; set; }
        protected System.Random RandomGenerator { get; set; }
        
        

        public MCTS(CurrentStateWorldModel currentStateWorldModel)
        {
            this.InProgress = false;
            this.CurrentStateWorldModel = currentStateWorldModel;
            this.MaxIterations = 500;
            this.MaxIterationsProcessedPerFrame = 5;
            this.NPlayouts = 6;
            this.RandomGenerator = new System.Random();
			this.BestActionSequence = new List<GOB.Action>();
			this.TotalProcessingTime = 0.0f;
        }


        public void InitializeMCTSearch()
        {
            this.MaxPlayoutDepthReached = 0;
            this.MaxSelectionDepthReached = 0;
            this.CurrentIterations = 0;
            this.CurrentIterationsInFrame = 0;
            //this.TotalProcessingTime = 0.0f;
			this.Nnodes = 0;
            this.CurrentStateWorldModel.Initialize();
            this.InitialNode = new MCTSNode(this.CurrentStateWorldModel)
            {
                Action = null,
                Parent = null,
                PlayerID = 0
            };
            this.InProgress = true;
            this.BestFirstChild = null;
            this.BestActionSequence = new List<GOB.Action>();
        }

        public GOB.Action Run()
        {
			var startTime = Time.realtimeSinceStartup;
			MCTSNode selectedNode;
            Reward reward;

            this.CurrentIterationsInFrame = 0;
            foreach (var action in this.InitialNode.State.GetExecutableActions())
            {
                if (action.Name.Contains("Lev")||(action.Name.Contains("Pick") && action.GetHValue(InitialNode.State) < 0.0f))
                {
                    return action;
                }
            }
            
			while (this.CurrentIterationsInFrame < this.MaxIterationsProcessedPerFrame) {
				selectedNode = Selection(this.InitialNode);

                
				float val = 0;
				float minVal = float.MaxValue;
				for (int i = 0; i < this.NPlayouts; i++) {
					float v = Playout(selectedNode.State).Value;
					val += v;
					minVal = Math.Min(minVal, v);
				}
				val /= this.NPlayouts;
				reward = new Reward() {
					Value = val * 0.5f + minVal * 0.5f
				};

				Backpropagate(selectedNode, reward);
				this.CurrentIterationsInFrame++;
				this.CurrentIterations++;
				this.Nnodes++;
			}

			this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;
			if (this.CurrentIterations >= this.MaxIterations)
				this.InProgress = false;
			if (this.InProgress) return null;

			var bestChild = BestChild(this.InitialNode);
			BestActionSequence = new List<GOB.Action>();
            BestActionSequence.Add(bestChild.Action);

            return bestChild.Action;
        }

        private MCTSNode Selection(MCTSNode initialNode)
        {
            GOB.Action nextAction;
            MCTSNode currentNode = initialNode;
            MCTSNode bestChild;

			int depth = 0;

			while (!currentNode.State.IsTerminal()) {
                nextAction = currentNode.State.GetNextAction();
				if (nextAction != null) {
					return Expand(currentNode, nextAction);
				}
				else {
					bestChild = BestUCTChild(currentNode);
					if (bestChild == null) break;
					currentNode = bestChild;
				}
				depth++;
			}

			if (depth > this.MaxSelectionDepthReached) this.MaxSelectionDepthReached = depth;
			return currentNode;
        }

        protected virtual Reward Playout(WorldModel initialPlayoutState)
        {
			int playOutDepth = 0;
			var state = initialPlayoutState.GenerateChildWorldModel();

			while (!state.IsTerminal()) {
				GOB.Action[] actions = state.GetExecutableActions();
				if (actions.Length == 0) break;
				int r = this.RandomGenerator.Next(0, actions.Length);
				GOB.Action action = actions[r];
				action.ApplyActionEffects(state);
				state.CalculateNextPlayer();
				//Debug.Log(state.GetNextPlayer());

				playOutDepth++;
			}

			if (playOutDepth > this.MaxPlayoutDepthReached) this.MaxPlayoutDepthReached = playOutDepth;

			Reward reward = new Reward() {
				Value = state.GetScore(),
			};
			return reward;
        }

        private void Backpropagate(MCTSNode node, Reward reward)
        {
			while (node != null) {
                node.N += 1;
				node.Q += reward.Value;

				node = node.Parent;
			}
        }

        private MCTSNode Expand(MCTSNode parent, GOB.Action action)
        {
			WorldModel model = parent.State.GenerateChildWorldModel(); 
			action.ApplyActionEffects(model);
			model.CalculateNextPlayer();

			MCTSNode child = new MCTSNode(model);
			child.Action = action;
			child.Parent = parent;
			parent.ChildNodes.Add(child);
			return child;
        }

        //gets the best child of a node, using the UCT formula
        private MCTSNode BestUCTChild(MCTSNode node)
        {
			MCTSNode bestChild = node.ChildNodes[0];
			float bestUTC = float.MinValue;

			foreach (MCTSNode child in node.ChildNodes) {
				float niu = child.Q / child.N;
				float utc = niu + (C * Mathf.Sqrt(Mathf.Log(node.N) / child.N));

				if (utc > bestUTC) {
					bestChild = child;
					bestUTC = utc;
				}
			}

			return bestChild;
        }

        //this method is very similar to the bestUCTChild, but it is used to return the final action of the MCTS search, and so we do not care about
        //the exploration factor
        private MCTSNode BestChild(MCTSNode node)
        {
			// Max Child
			MCTSNode bestChild = node.ChildNodes[0];
			float bestNiu = float.MinValue;

			foreach (MCTSNode child in node.ChildNodes) {
				float niu = (child.Q / child.N);
                child.niu = niu;
                if (child.Action.Name.Contains("LevelUp") || (child.Action.Name.Contains("PickUpChest") && child.Action.GetHValue(node.State) < 0.0f))
                {
                    niu += 50;
                }
				if (niu > bestNiu) {
					bestChild = child;
					bestNiu = niu;
				}
			}

			Debug.Log(bestNiu);
			return bestChild;
        }
    }
}
