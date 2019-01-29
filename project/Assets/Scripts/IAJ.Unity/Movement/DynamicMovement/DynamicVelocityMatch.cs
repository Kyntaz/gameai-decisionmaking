using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicVelocityMatch : DynamicMovement
    {
        public override string Name
        {
            get { return "VelocityMatch"; }
        }

        public override KinematicData Target { get; set; }
        public float StopVelocityDelta { get; set; }
        public float TimeToTargetSpeed { get; set; }
        public KinematicData TargetVelocity { get; set; }
        private MovementOutput Output { get; set; }

        public DynamicVelocityMatch()
        {
            this.StopVelocityDelta = 0.05f;
            this.TimeToTargetSpeed = 0.5f;
            this.TargetVelocity = new KinematicData();
            this.Output = new MovementOutput();
        }
        public override MovementOutput GetMovement()
        {
            this.Output.linear = (this.TargetVelocity.velocity - this.Character.velocity);
            if(this.Output.linear.sqrMagnitude <= this.StopVelocityDelta)
            {
                this.Output.linear = Vector3.zero;
                this.Output.angular = 0;
                return this.Output;
            }    
                
            this.Output.linear/=this.TimeToTargetSpeed;

            if (this.Output.linear.sqrMagnitude > this.MaxAcceleration*this.MaxAcceleration)
            {
                this.Output.linear = Output.linear.normalized*this.MaxAcceleration;
            }
            this.Output.angular = 0;
            return this.Output;
        }
    }
}
