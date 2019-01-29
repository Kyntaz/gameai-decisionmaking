using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class WorldModel
    {
        private Dictionary<string, object> Properties { get; set; }
        private List<Action> Actions { get; set; }
        protected IEnumerator<Action> ActionEnumerator { get; set; }

		public bool UseFixedList = true;
		public Dictionary<string, object> PropertyList;
		public string[] PropertyNames = new string[] {"Mana", "HP", "MAXHP", "SHIELDHP", "XP", "Time", "Money", "Level", "Position",
													  "Skeleton1", "Skeleton2", "Skeleton3", "Skeleton4", "Skeleton5", "Skeleton6", "Skeleton7",
													  "Orc1", "Orc2", "Dragon", "ManaPotion1", "ManaPotion2", "HealthPotion1", "HealthPotion2",
													  "Chest1", "Chest2", "Chest3", "Chest4", "Chest5"};

        private Dictionary<string, float> GoalValues { get; set; } 

        protected WorldModel Parent { get; set; }

        public WorldModel(List<Action> actions)
        {
			if (!this.UseFixedList) this.Properties = new Dictionary<string, object>();
            this.GoalValues = new Dictionary<string, float>();
            this.Actions = actions;
            this.ActionEnumerator = actions.GetEnumerator();
        }

        public WorldModel(WorldModel parent)
        {
			if (!this.UseFixedList) this.Properties = new Dictionary<string, object>();
			else {
				this.PropertyList = new Dictionary<string, object>();
				foreach (string name in this.PropertyNames ) {
					this.SetProperty(name, parent.GetProperty(name));
				}
			}

            this.GoalValues = new Dictionary<string, float>();
            this.Actions = parent.Actions;
            this.Parent = parent;
            this.ActionEnumerator = this.Actions.GetEnumerator();
        }

        public virtual object GetProperty(string propertyName)
        {
            //recursive implementation of WorldModel
			if (!this.UseFixedList) {
	            if (this.Properties.ContainsKey(propertyName))
	            {
	                return this.Properties[propertyName];
	            }
	            else if (this.Parent != null)
	            {
	                return this.Parent.GetProperty(propertyName);
	            }
	            else
	            {
	                return null;
	            }
			}
			else {
				if (!this.PropertyList.ContainsKey(propertyName)) return false;
				return this.PropertyList[propertyName];
			}
        }

        public virtual void SetProperty(string propertyName, object value)
        {
			if (!this.UseFixedList) this.Properties[propertyName] = value;
			else {
				this.PropertyList[propertyName] = value;
			}
        }

        public virtual float GetGoalValue(string goalName)
        {
            //recursive implementation of WorldModel
            if (this.GoalValues.ContainsKey(goalName))
            {
                return this.GoalValues[goalName];
            }
            else if (this.Parent != null)
            {
                return this.Parent.GetGoalValue(goalName);
            }
            else
            {
                return 0;
            }
        }

        public virtual void SetGoalValue(string goalName, float value)
        {
            var limitedValue = value;
            if (value > 10.0f)
            {
                limitedValue = 10.0f;
            }

            else if (value < 0.0f)
            {
                limitedValue = 0.0f;
            }

            this.GoalValues[goalName] = limitedValue;
        }

        public virtual WorldModel GenerateChildWorldModel()
        {
            return new WorldModel(this);
        }

        public float CalculateDiscontentment(List<Goal> goals)
        {
            var discontentment = 0.0f;

            foreach (var goal in goals)
            {
                var newValue = this.GetGoalValue(goal.Name);

                discontentment += goal.GetDiscontentment(newValue);
            }

            return discontentment;
        }

        public virtual Action GetNextAction()
        {
            Action action = null;
            //returns the next action that can be executed or null if no more executable actions exist
            if (this.ActionEnumerator.MoveNext())
            {
                action = this.ActionEnumerator.Current;
            }

            while (action != null && !action.CanExecute(this))
            {
                if (this.ActionEnumerator.MoveNext())
                {
                    action = this.ActionEnumerator.Current;    
                }
                else
                {
                    action = null;
                }
            }

            return action;
        }

        public virtual Action[] GetExecutableActions()
        {
            return this.Actions.Where(a => a.CanExecute(this)).ToArray();
        }

        public virtual bool IsTerminal()
        {
            return true;
        }
        

        public virtual float GetScore()
        {
            return 0.0f;
        }

        public virtual int GetNextPlayer()
        {
            return 0;
        }

        public virtual void CalculateNextPlayer()
        {
        }

		public int GetPropertyIndex(string property) {
			for (int i = 0; i < this.PropertyNames.Length; i++) {
				if (this.PropertyNames[i].CompareTo(property) == 0)
					return i;
			}
			return -1;
		}
    }
}
