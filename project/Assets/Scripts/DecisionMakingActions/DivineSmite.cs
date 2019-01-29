using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;
using System;

namespace Assets.Scripts.DecisionMakingActions
{
    public class DivineSmite : WalkToTargetAndExecuteAction
    {
        private int xpChange;
		private int manaChange;

		public DivineSmite(AutonomousCharacter character, GameObject target) : base("DivineSmite",character,target)
		{
			//TODO: implemented
			if (target.tag.Equals("Skeleton"))
			{
				this.manaChange = -2;
				this.xpChange = 3;
			}
		}

		public override float GetGoalChange(Goal goal)
		{
			var change = base.GetGoalChange(goal);

			if (goal.Name == AutonomousCharacter.SURVIVE_GOAL)
			{
				change += 0;
			}
			else if (goal.Name == AutonomousCharacter.GAIN_XP_GOAL)
			{
				change += -this.xpChange;
			}

			return change;
		}

		public override bool CanExecute()
		{
			//TODO: implemented
			if (!base.CanExecute()) return false;

			if (!base.Target.tag.Equals("Skeleton")) return false;
			if (base.Character.GameManager.characterData.Mana < 2) return false;
			return true;
		}

		public override bool CanExecute(WorldModel worldModel)
		{
			//TODO: implemented
			if (!base.CanExecute(worldModel)) return false;

			if (!base.Target.tag.Equals("Skeleton")) return false;
			if ((int)worldModel.GetProperty(Properties.MANA) < 2) return false;
			return true;
		}

		public override void Execute()
		{
			//TODO: implemented
			base.Execute();
			this.Character.GameManager.DivineSmite(this.Target);
		}


		public override void ApplyActionEffects(WorldModel worldModel)
		{
			//TODO: implemented
			base.ApplyActionEffects(worldModel);

			var xpValue = worldModel.GetGoalValue(AutonomousCharacter.GAIN_XP_GOAL);
			worldModel.SetGoalValue(AutonomousCharacter.GAIN_XP_GOAL,xpValue-this.xpChange); 

			var surviveValue = worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);
			worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL,surviveValue);

			var hp = (int)worldModel.GetProperty(Properties.HP);
			worldModel.SetProperty(Properties.HP,hp + 0);
			var xp = (int)worldModel.GetProperty(Properties.XP);
			worldModel.SetProperty(Properties.XP, xp + this.xpChange);


			//disables the target object so that it can't be reused again
			worldModel.SetProperty(this.Target.name,false);
		}

        public override float GetHValue(WorldModel worldModel)
        {
            float lvl = (int)worldModel.GetProperty(Properties.LEVEL);
			return (lvl - 1.0f) * 4;
        }

    }
}
