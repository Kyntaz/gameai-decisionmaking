using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.GameManager;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
	public class MCTSLimitedPlayout : MCTSBiasedPlayout
    {
		private int MaxPlayoutIterations = 5;

		public MCTSLimitedPlayout(CurrentStateWorldModel currentStateWorldModel) : base(currentStateWorldModel)
        {
			this.NPlayouts = 20;
			this.MaxIterations = 500;
			this.MaxIterationsProcessedPerFrame = 5;
        }
        
        protected override Reward Playout(WorldModel initialPlayoutState)
        {
            int playOutDepth = 0;
            var state = initialPlayoutState.GenerateChildWorldModel();

			while (playOutDepth < this.MaxPlayoutIterations && !state.IsTerminal())
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
    }
}
