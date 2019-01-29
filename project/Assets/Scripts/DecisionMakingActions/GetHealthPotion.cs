using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class GetHealthPotion : WalkToTargetAndExecuteAction
    {
        public GetHealthPotion(AutonomousCharacter character, GameObject target) : base("GetHealthPotion",character,target)
        {
        }

		public override bool CanExecute()
		{
			//TODO: implemented
			if (!base.CanExecute()) return false;
			return this.Character.GameManager.characterData.HP < this.Character.GameManager.characterData.MaxHP;
		}

		public override bool CanExecute(WorldModel worldModel)
		{
			//TODO: implemented
			if (!base.CanExecute(worldModel)) return false;

			var hp = (int)worldModel.GetProperty(Properties.HP);
			return hp < this.Character.GameManager.characterData.MaxHP && !((bool)worldModel.GetProperty(this.Character.GameManager.nearbyEnemies[this.Target.name]));
		}

		public override void Execute()
		{
			//TODO: implemented
			base.Execute();
			this.Character.GameManager.GetHealthPotion(this.Target);
		}

		public override void ApplyActionEffects(WorldModel worldModel)
		{
			//TODO: implemented
			var maxHP = (int)worldModel.GetProperty(Properties.MAXHP);

			base.ApplyActionEffects(worldModel);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL,0.0f);
            worldModel.SetProperty(Properties.HP, maxHP);
			//disables the target object so that it can't be reused again
			worldModel.SetProperty(this.Target.name, false);
		}
        public override float GetHValue(WorldModel worldModel)
        {
            float maxHP = (int)worldModel.GetProperty(Properties.MAXHP);
            float currHP = (int)worldModel.GetProperty(Properties.HP);
            float currMana = (int)worldModel.GetProperty(Properties.MANA);
            bool existsMana1 = (bool)worldModel.GetProperty("ManaPotion1");
            bool existsMana2 = (bool)worldModel.GetProperty("ManaPotion2");

            if (maxHP == currHP) return 100.0f;
            if (currHP <= maxHP * 0.2f) {
                if (existsMana2 && currMana<2)
                    return new GetManaPotion(this.Character, GameObject.Find("ManaPotion2")).GetHValue(worldModel) + 1.0f;
                else if (existsMana1 && currMana < 2)
                    return new GetManaPotion(this.Character, GameObject.Find("ManaPotion1")).GetHValue(worldModel) + 1.0f;
                else return 1.0f;
            }

            else return 50.0f;
            //return 15.0f - (maxHP - currHP) * 0.3f;
        }
    }
}
