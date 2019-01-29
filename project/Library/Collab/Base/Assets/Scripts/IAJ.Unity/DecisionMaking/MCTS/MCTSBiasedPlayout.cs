using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.GameManager;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTSBiasedPlayout : MCTS
    {
        public MCTSBiasedPlayout(CurrentStateWorldModel currentStateWorldModel) : base(currentStateWorldModel)
        {
			this.NPlayouts = 5;
        }
        
        protected override Reward Playout(WorldModel initialPlayoutState)
        {
            int playOutDepth = 0;
            var state = initialPlayoutState.GenerateChildWorldModel();

            while (!state.IsTerminal())
            {
                GOB.Action[] actions = state.GetExecutableActions();
                if (actions.Length == 0) break;
                GOB.Action action = SelectActionBasedOnHeurisitic(actions,state);
                action.ApplyActionEffects(state);
                state.CalculateNextPlayer();
                playOutDepth++;
            }

            if (playOutDepth > this.MaxPlayoutDepthReached) this.MaxPlayoutDepthReached = playOutDepth;

            Reward reward = new Reward()
            {
                Value = state.GetScore(),
            };
            return reward;
        }

        protected GOB.Action SelectActionBasedOnHeurisitic(GOB.Action[] actions,WorldModel state)
        {
            GOB.Action bestAction = null; 
			List<float> heuristics = new List<float>();
			float sumExp = 0;

			// Calculate the heuristics and apply exponential
			foreach (GOB.Action action in actions) {
				float exp = Mathf.Exp(-action.GetHValue(state));
                if(exp ==float.PositiveInfinity || exp == float.NegativeInfinity)
                {
                    exp = float.MaxValue;
                }

				sumExp += exp;
				heuristics.Add(exp);
			}
				
			// Aggregate and divide.
			heuristics[0] /= sumExp;
			for (int i = 1; i < heuristics.Count; i++) {
				heuristics[i] /= sumExp;
				heuristics[i] += heuristics[i-1];
			}

			float r = UnityEngine.Random.Range(0, heuristics[heuristics.Count - 1]);
			for (int i = 0; i < heuristics.Count; i++) {
				if (r < heuristics[i])
					return actions[i];
			}
            
            return null;
 
        }
        
       
        
    }
}
