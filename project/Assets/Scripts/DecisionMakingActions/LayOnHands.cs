using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
	public class LayOnHands : IAJ.Unity.DecisionMaking.GOB.Action
    {
		public AutonomousCharacter Character;

		public LayOnHands(AutonomousCharacter character) : base("LayOnHands")
        {
			this.Character = character;
			this.Duration = 0;
        }

		public override bool CanExecute()
		{
			//TODO: implemented
			//if (!base.CanExecute()) return false;
			return this.Character.GameManager.characterData.HP < this.Character.GameManager.characterData.MaxHP &&
				this.Character.GameManager.characterData.Mana >= 7 &&
				this.Character.GameManager.characterData.Level >= 2;
		}

		public override bool CanExecute(WorldModel worldModel)
		{
			//TODO: implemented
			//if (!base.CanExecute(worldModel)) return false;

			var hp = (int)worldModel.GetProperty(Properties.HP);
			var mana = (int)worldModel.GetProperty(Properties.MANA);
			var level = (int)worldModel.GetProperty(Properties.LEVEL);
			var maxHP = (int)worldModel.GetProperty(Properties.MAXHP);

			return hp < this.Character.GameManager.characterData.MaxHP &&
				mana >= 7 &&
				level >= 2;
		}

		public override void Execute()
		{
			//TODO: implemented
			//base.Execute();
			this.Character.GameManager.LayOnHands();
		}

		public override void ApplyActionEffects(WorldModel worldModel)
		{
			//TODO: implemented
			//base.ApplyActionEffects(worldModel);
			int maxHP = (int)worldModel.GetProperty(Properties.MAXHP);
			int mana = (int)worldModel.GetProperty(Properties.MANA);

			worldModel.SetProperty(Properties.HP, maxHP);
			worldModel.SetProperty(Properties.MANA, mana - 7);
		}
        public override float GetHValue(WorldModel worldModel)
        {
            float maxHP = (int)worldModel.GetProperty(Properties.MAXHP);
            float currentMana = (int)worldModel.GetProperty(Properties.MANA);
            float currentHP = (int)worldModel.GetProperty(Properties.HP);
			int nManaPotions = ((bool)worldModel.GetProperty("ManaPotion1") ? 1 : 0)
				+ ((bool)worldModel.GetProperty("ManaPotion2") ? 1 : 0);
			
			if (nManaPotions == 0) return 20.0f;
			else return 10.0f - (maxHP - currentHP) * 0.3f;

            //return ((maxHP - currentHP) * 0.6f + (10 - currentMana) * 0.4f) * 1.0f;
        }
    }
}
