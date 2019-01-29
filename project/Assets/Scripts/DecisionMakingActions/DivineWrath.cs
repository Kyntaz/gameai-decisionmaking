using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
	public class DivineWrath : IAJ.Unity.DecisionMaking.GOB.Action
    {
		public AutonomousCharacter Character;

		public DivineWrath(AutonomousCharacter character) : base("LayOnHands")
        {
			this.Character = character;
			this.Duration = 0;
        }

		public override bool CanExecute()
		{
			//TODO: implemented
			//if (!base.CanExecute()) return false;
			return this.Character.GameManager.characterData.Mana >= 10 &&
				this.Character.GameManager.characterData.Level >= 3;
		}

		public override bool CanExecute(WorldModel worldModel)
		{
			//TODO: implemented
			//if (!base.CanExecute(worldModel)) return false;

			//var hp = (int)worldModel.GetProperty(Properties.HP);
			var mana = (int)worldModel.GetProperty(Properties.MANA);
			var level = (int)worldModel.GetProperty(Properties.LEVEL);
			return mana >= 10 &&
				level >= 3;
		}

		public override void Execute()
		{
			//TODO: implemented
			//base.Execute();
			this.Character.GameManager.DivineWrath();
		}

		public override void ApplyActionEffects(WorldModel worldModel)
		{
			var xp = (int)worldModel.GetProperty(Properties.XP);
			var mana = (int)worldModel.GetProperty(Properties.MANA);

			foreach (var enemy in this.Character.GameManager.enemies) {
				worldModel.SetProperty(enemy.name, false);
				if (enemy.tag.Equals("Skeleton"))
				{
					worldModel.SetProperty(Properties.XP, xp + 3);
				}
				else if (enemy.tag.Equals("Orc"))
				{
					worldModel.SetProperty(Properties.XP, xp + 10);
				}
				else if (enemy.tag.Equals("Dragon"))
				{
					worldModel.SetProperty(Properties.XP, xp + 20);
				}
			}

			worldModel.SetProperty(Properties.MANA, mana - 10);
		}

        public override float GetHValue(WorldModel worldModel)
        {
            return -100.0f;
        }
    }
}
