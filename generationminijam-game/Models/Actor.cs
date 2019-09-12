using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace generationminijam_game.Models {
    class Actor : Mesh {

        public float maxSpeed;
        public float frictionCoefficient;
        public Vector3 paddleAcceleration;
        public Vector3 paddleAcceleration2;
        public Vector3 jumpForce;

        public Key KeyUp = Key.W;
        public Key KeyDown = Key.S;
        public Key KeyLeft = Key.A;
        public Key KeyRight = Key.D;
        public Key KeySprint = Key.LeftShift;
        public Key KeyJump = Key.Space;

        public bool canMoveUp;
        public bool canMoveDown;
        public bool canMoveLeft;
        public bool canMoveRight;
        public bool canSprint;
        public bool canJump;
        public bool isJumping;
        public bool isEnemy;

        public Actor() {
            Mass = 1.0f;
            maxSpeed = 3.0f;
            frictionCoefficient = 0.5f;
            paddleAcceleration = new Vector3(0, 0, 1);
            paddleAcceleration2 = new Vector3(1, 0, 0);
            jumpForce = new Vector3(0, 2, 0);
            isEnemy = false;

            canMoveUp = false;
            canMoveDown = false;
            canMoveLeft = false;
            canMoveRight = false;
            canSprint = false;
            canJump = false;
            isJumping = false;
        }

        public void EnableControl(bool enable = false) {

            if (!enable) {
                return;
            }

            if ((Keyboard.IsKeyDown(KeySprint)) && (canSprint)) {
                maxSpeed = 6.0f;
            } else {
                maxSpeed = 3.0f;
            }

            if ((Keyboard.IsKeyDown(KeyJump)) && (canJump) && !(isJumping)) {
                ApplyForce(jumpForce);
                isJumping = true;
            }

            if ((Keyboard.IsKeyDown(KeyUp)) && (canMoveUp))  {
                if (Velocity.z < -maxSpeed) {
                    ApplyForce(paddleAcceleration * -1);
                }
                else {
                    Velocity.z = -maxSpeed;
                }
            }
            else if ((Keyboard.IsKeyDown(KeyDown)) && (canMoveDown)) {
                if (Velocity.z > maxSpeed) {
                    ApplyForce(paddleAcceleration);
                }
                else {
                    Velocity.z = maxSpeed;
                }
            } else {
                ApplyFriction(frictionCoefficient, 1, "z");
            }

            if ((Keyboard.IsKeyDown(KeyLeft)) && (canMoveLeft)) {
                if (Velocity.x > -maxSpeed) {
                    ApplyForce(paddleAcceleration2 * -1);
                }
                else {
                    Velocity.x = -maxSpeed;
                }
            }
            else if ((Keyboard.IsKeyDown(KeyRight)) && (canMoveRight)) {
                if (Velocity.x < maxSpeed) {
                    ApplyForce(paddleAcceleration2);
                }
                else {
                    Velocity.x = maxSpeed;
                }
            }
            else {
                ApplyFriction(frictionCoefficient, 1 ,"x");
            }
        }

        public void ChasePlayer(Actor player, bool enable) {
            if (!enable) {
                return;
            }

            Vector3 PlayerPosition = player.Position;
            Vector3 direction = PlayerPosition - Position;

            ApplyForce(direction.Normalized());

            ApplyFriction(frictionCoefficient, 1);
        }
    }
}
