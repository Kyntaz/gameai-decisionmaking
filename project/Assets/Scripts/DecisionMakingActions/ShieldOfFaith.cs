using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;
using System;

namespace Assets.Scripts.DecisionMakingActions
{
    public class ShieldOfFaith : IAJ.Unity.DecisionMaking.GOB.Action
    {
        public AutonomousCharacter Character { get; set; }

		public ShieldOfFaith(AutonomousCharacter character) : base("ShieldOfFaith")
		{
            this.Character = character;

			base.Duration = 0;
		}

		public override float GetGoalChange(Goal goal)
		{
			//TODO: implemented
			var change = base.GetGoalChange(goal);

			if (goal.Name == AutonomousCharacter.SURVIVE_GOAL)
			{
				change -= 5;
			}

			return change;
		}

		public override bool CanExecute()
		{
			//TODO: implemented
			return this.Character.GameManager.characterData.Mana >= 5 && this.Character.GameManager.characterData.ShieldHP < 5;
		}

		public override bool CanExecute(WorldModel worldModel)
		{
			//TODO: implemented
			var mana = (int)worldModel.GetProperty(Properties.MANA);
			var shieldHP = (int)worldModel.GetProperty(Properties.SHIELDHP);
			return mana >= 5 && shieldHP < 5;
		}

		public override void Execute()
		{
			//TODO: implement
			this.Character.GameManager.ShieldOfFaith();
		}


		public override void ApplyActionEffects(WorldModel worldModel)
		{
			//TODO: implement
			worldModel.SetProperty(Properties.SHIELDHP, 5);
		}
        public override float GetHValue(WorldModel worldModel)
        {
			int level = (int)worldModel.GetProperty(Properties.LEVEL);
			if (level == 1) return 20.0f;
			else return 50.0f;
        }
    }
}
