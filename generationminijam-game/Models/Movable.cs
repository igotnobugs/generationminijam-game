using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace generationminijam_game.Models {
    public class Movable {

        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 Acceleration;
        public Vector3 Friction;
        public float Rotation;

        public float Mass = 1;

        public void ApplyForce(Vector3 force) {
            this.Acceleration += (force / Mass);
        }

        public void ApplyGravity(float scalar = 0.1f) {
            this.Acceleration += (new Vector3(0, -scalar * Mass, 0) / Mass);
        }

        public void ApplyFriction(float frictionCoefficient = 0.1f, float normalForce = 1.0f, string axis = null) {
            var frictionMagnitude = frictionCoefficient * normalForce;

            Friction = this.Velocity;
            Friction *= -1;
            Friction.Normalize();
            Friction *= frictionMagnitude;
            if (axis == "x") {
                Friction.z *= 0;
            }
            if (axis == "z") {
                Friction.x *= 0;
            }
            this.ApplyForce(Friction);

        }

        public void ChangeAngle(float angle) {
            var speed = this.Velocity.GetLength();
            //Vector Velocity changed to Specific Angle Multiplied by Speed
            Velocity = new Vector3((float)Math.Cos(GameUtils.DegreeToRadian(angle)), (float)Math.Sin(GameUtils.DegreeToRadian(angle)), Velocity.z) * speed;
        }

        public void IncreaseSpeed(float speed) {
            var curSpeed = this.Velocity.GetLength();
            Velocity = (Velocity.Normalized() * (curSpeed + speed));
        }
    }
}
