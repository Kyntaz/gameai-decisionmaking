using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Assets.Scripts.IAJ.Unity.Utils;
using System;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class SwordAttack : WalkToTargetAndExecuteAction
    {
        private float expectedHPChange;
        private float expectedXPChange;
        private int xpChange;
        private int enemyAC;
        //how do you like lambda's in c#?
        private Func<int> dmgRoll;

        public SwordAttack(AutonomousCharacter character, GameObject target) : base("SwordAttack",character,target)
        {
            if (target.tag.Equals("Skeleton"))
            {
                this.dmgRoll = () => RandomHelper.RollD6();
                this.expectedHPChange = 3.5f;
                this.xpChange = 3;
                this.expectedXPChange = 2.7f;
                this.enemyAC = 10;
            }
            else if (target.tag.Equals("Orc"))
            {
                this.dmgRoll = () => RandomHelper.RollD10() + RandomHelper.RollD10();
                this.expectedHPChange = 14.0f;
                this.xpChange = 10;
                this.expectedXPChange = 7.0f;
                this.enemyAC = 14;
            }
            else if (target.tag.Equals("Dragon"))
            {
                this.dmgRoll = () => RandomHelper.RollD12() + RandomHelper.RollD12() + RandomHelper.RollD12();
                this.expectedHPChange = 27.5f;
                this.xpChange = 20;
                this.expectedXPChange = 10.0f;
                this.enemyAC = 18;
            }
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL)
            {
                change += this.expectedHPChange;
            }
            else if (goal.Name == AutonomousCharacter.GAIN_XP_GOAL)
            {
                change += -this.expectedXPChange;
            }
            
            return change;
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.SwordAttack(this.Target);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            int hp = (int)worldModel.GetProperty(Properties.HP);
			int shieldHp = (int)worldModel.GetProperty(Properties.SHIELDHP);
            int xp = (int)worldModel.GetProperty(Properties.XP);
            //execute the lambda function to calculate received damage based on the creature type
			int damage = (int)((this.dmgRoll.Invoke() + this.expectedHPChange) / 2);

            //calculate player's damage
            int remainingDamage = damage - shieldHp;
            int remainingShield = Mathf.Max(0, shieldHp - damage);
            int remainingHP;

            if(remainingDamage > 0)
            {
                remainingHP = hp - remainingDamage;
                worldModel.SetProperty(Properties.HP, remainingHP);
            }

			worldModel.SetProperty(Properties.SHIELDHP, remainingShield);
            var surviveValue = worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, surviveValue - remainingDamage);


            //calculate Hit
            //attack roll = D20 + attack modifier. Using 7 as attack modifier (+4 str modifier, +3 proficiency bonus)
			//lower attack modifier, play it safe
            int attackRoll = RandomHelper.RollD20() + 7;

            if (attackRoll >= enemyAC)
            {
                //there was an hit, enemy is destroyed, gain xp
                //disables the target object so that it can't be reused again
                worldModel.SetProperty(this.Target.name, false);

                worldModel.SetProperty(Properties.XP, xp + this.xpChange);
                var xpValue = worldModel.GetGoalValue(AutonomousCharacter.GAIN_XP_GOAL);
                worldModel.SetGoalValue(AutonomousCharacter.GAIN_XP_GOAL, xpValue - this.xpChange);
            }
			//Debug.Log(damage);
        }
        public override float GetHValue(WorldModel worldModel)
        {
            float currentHP = (int)worldModel.GetProperty(Properties.HP);
            float currentShield = (int)worldModel.GetProperty(Properties.SHIELDHP);
			float level = (int)worldModel.GetProperty(Properties.LEVEL);

			if ((currentHP + currentShield - (this.expectedHPChange))*((enemyAC/27)) >= 1 && level < 3)
            {
                return 5.0f;
            }
            return 100.0f;
        }
    }
}
