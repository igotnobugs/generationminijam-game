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
        public float Speed;
        public Vector3 jumpForce;
        public Vector3 startingPosition;

        public bool gravity = false;

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
            Speed = 1;
            jumpForce = new Vector3(0, 5.5f, 0);
            isEnemy = false;
            frictionCoefficient = 0.5f;

            canMoveUp = false;
            canMoveDown = false;
            canMoveLeft = false;
            canMoveRight = false;
            canSprint = false;
            canJump = false;
            isJumping = false;
        }

        public void EnableAll() {
            EnableControl(true);
            canMoveUp = true;
            canMoveDown = true;
            canMoveLeft = true;
            canMoveRight = true;
            canSprint = true;
            canJump = true;
        }

        //Shrink mesh equally based on rate
        public void Shrink(float rate = 0.2f) {
            if (Scale.x > 0) {
                Scale -= new Vector3(rate, rate, 0.2f);
            }
        }

        //Fade mesh color by reducing alpga
        public void Fade(float rate = 0.05f) {
            Color.a -= rate;
        }

        //Reset player to default position
        public void ResetToPosition() {
            Position = startingPosition;
        }

        //Apply Gravity
        public void AllowGravity(List<Structure> floors, bool enable = false) {
            if (!enable) {
                return;
            }

            foreach (var floor in floors) {              
                if (!HasCollidedWith(floor)) {
                    ApplyGravity();
                }  else {
                    Velocity.y *= 0;
                    Position.y = floor.Position.y + Scale.y;
                    isJumping = false;                 
                }
            }        
        }

        public void EnableControl(bool enable = false) {

            if (!enable) {
                return;
            }

            if (Keyboard.IsKeyDown(KeySprint) && canSprint) {
                maxSpeed = 6.0f;
            } else {
                maxSpeed = 3.0f;
            }

            if (Keyboard.IsKeyDown(KeyJump) && canJump && !isJumping) {
                ApplyForce(jumpForce);
                isJumping = true;
            }
            
            if (Keyboard.IsKeyDown(KeyUp) && canMoveUp)  {
                if (Velocity.z < -maxSpeed) {
                    ApplyForce(new Vector3( 0, 0, Speed) * -1);
                }
                else {
                    Velocity.z = -maxSpeed;
                }
            }
            else if (Keyboard.IsKeyDown(KeyDown) && canMoveDown) {
                if (Velocity.z > maxSpeed) {
                    ApplyForce(new Vector3(0, 0, Speed));
                }
                else {
                    Velocity.z = maxSpeed;
                }
            } else {
                ApplyFriction(frictionCoefficient, 1, "z");
            }

            if (Keyboard.IsKeyDown(KeyLeft) && canMoveLeft) {
                if (Velocity.x > -maxSpeed) {
                    ApplyForce(new Vector3(Speed, 0, 0) * -1);
                }
                else {
                    Velocity.x = -maxSpeed;
                }
            }
            else if (Keyboard.IsKeyDown(KeyRight) && canMoveRight) {
                if (Velocity.x < maxSpeed) {
                    ApplyForce(new Vector3(Speed, 0, 0));
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
