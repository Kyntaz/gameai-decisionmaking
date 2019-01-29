using Assets.Scripts.GameManager;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class DepthLimitedGOAPDecisionMaking
    {
        public const int MAX_DEPTH = 3;
        public int ActionCombinationsProcessedPerFrame { get; set; }
        public float TotalProcessingTime { get; set; }
        public int TotalActionCombinationsProcessed { get; set; }
        public bool InProgress { get; set; }

        public CurrentStateWorldModel InitialWorldModel { get; set; }
        private List<Goal> Goals { get; set; }
        private WorldModel[] Models { get; set; }
        private Action[] ActionPerLevel { get; set; }
        public Action[] BestActionSequence { get; private set; }
        public Action BestAction { get; private set; }
        public float BestDiscontentmentValue { get; private set; }
        private int CurrentDepth {  get; set; }

        public DepthLimitedGOAPDecisionMaking(CurrentStateWorldModel currentStateWorldModel, List<Action> actions, List<Goal> goals)
        {
            this.ActionCombinationsProcessedPerFrame = 200;
            this.Goals = goals;
            this.InitialWorldModel = currentStateWorldModel;
        }

        public void InitializeDecisionMakingProcess()
        {
            this.InProgress = true;
            this.TotalProcessingTime = 0.0f;
            this.TotalActionCombinationsProcessed = 0;
            this.CurrentDepth = 0;
            this.Models = new WorldModel[MAX_DEPTH + 1];
            this.Models[0] = this.InitialWorldModel;
            this.ActionPerLevel = new Action[MAX_DEPTH];
            this.BestActionSequence = new Action[MAX_DEPTH];
            this.BestAction = null;
            this.BestDiscontentmentValue = float.MaxValue;
            this.InitialWorldModel.Initialize();
        }

        public Action ChooseAction()
        {
			
			var processedActions = 0;

			var startTime = Time.realtimeSinceStartup;

			//TODO: Implemented
			float currentValue;
			Action nextAction;
			while (this.CurrentDepth >= 0) {
				if (this.CurrentDepth >= MAX_DEPTH) {
                    TotalActionCombinationsProcessed++;
					currentValue = this.Models[this.CurrentDepth].CalculateDiscontentment(this.Goals);

					if (currentValue < this.BestDiscontentmentValue) {
						this.BestDiscontentmentValue = currentValue;
						this.BestAction = this.ActionPerLevel[0]; // or is it? tan tan taaaaan
						this.BestActionSequence = (Action[])this.ActionPerLevel.Clone();
					}

					this.CurrentDepth -= 1;
					continue;
				}

				nextAction = this.Models[this.CurrentDepth].GetNextAction();
				if (nextAction != null) {
                    this.Models[this.CurrentDepth + 1] = this.Models[this.CurrentDepth].GenerateChildWorldModel();
					nextAction.ApplyActionEffects(this.Models[this.CurrentDepth + 1]);

                    this.ActionPerLevel[this.CurrentDepth] = nextAction;
					this.CurrentDepth += 1;
                    
                }
				else {
					this.CurrentDepth -= 1;
				}
			}

			this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;
			this.InProgress = false;
			return this.BestAction;
        }
    }
}
