using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGL;

namespace generationminijam_game.Models {
    public partial class Mesh : Movable {

        public bool enabledUpdate = true;
        public Vector3 Scale = new Vector3(0.5f, 0.5f, 0.5f);
        public Vector4 Color = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
        public float Radius = 0.5f;
        public bool enabledCollision = false;

        //public bool EnabledUpdate { get; set; }

        public Mesh() {
            this.Position = new Vector3();
            this.Velocity = new Vector3();
            this.Acceleration = new Vector3();
            this.Rotation = 0;
        }

        public Mesh(Vector3 initPos) {
            this.Position = initPos;
            this.Velocity = new Vector3();
            this.Acceleration = new Vector3();
            this.Rotation = 0;
        }

        public Mesh(float x, float y, float z, int r) {
            this.Position = new Vector3();
            this.Velocity = new Vector3();
            this.Acceleration = new Vector3();
            this.Position.x = x;
            this.Position.y = y;
            this.Position.z = z;
            this.Rotation = r;
        }

        public void DrawSquare(OpenGL gl, float lineWidth = 1.0f) {

            gl.LineWidth(lineWidth);
            gl.Color(Color.r, Color.g, Color.b, Color.a);
            gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y + this.Scale.y, this.Position.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y + this.Scale.y, this.Position.z);
            gl.End();

            if (enabledUpdate) {
                UpdateMotion();
            }
        }
        //For DialogBoxes in Orthographic x width, y height
        public void DrawPanel(OpenGL gl, float lineWidth = 1.0f) {
            gl.LineWidth(lineWidth);
            gl.Color(Color.r, Color.g, Color.b, Color.a);

            gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
            gl.Vertex(this.Position.x , this.Position.y, this.Position.z);
            gl.Vertex(this.Position.x, this.Position.y + Scale.y, this.Position.z);
            gl.Vertex(this.Position.x + Scale.x, this.Position.y, this.Position.z);
            gl.Vertex(this.Position.x + Scale.x, this.Position.y + Scale.y, this.Position.z);
            gl.End();

        }

        public void DrawTriangle(OpenGL gl, float lineWidth = 1.0f) {
            gl.LineWidth(lineWidth);
            gl.Color(Color.r, Color.g, Color.b, Color.a);
            gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z);
            gl.Vertex(this.Position.x, this.Position.y + this.Scale.y, this.Position.z);
            gl.End();
        }

        public void DrawCube(OpenGL gl) {
            gl.LineWidth(1);
            gl.Color(0, 0, 0, Color.a);
            gl.Begin(OpenGL.GL_LINE_LOOP);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y + this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z - this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y + this.Scale.y, this.Position.z - this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y + this.Scale.y, this.Position.z + this.Scale.z);
            gl.End();

            gl.Begin(OpenGL.GL_LINE_LOOP);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z - this.Scale.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z - this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z + this.Scale.z);
            gl.End();

            gl.Begin(OpenGL.GL_LINE_LOOP);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z - this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z - this.Scale.z);
            gl.End();

            //POLYGON
            gl.Color(Color.r, Color.g, Color.b, Color.a);
            gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
            //Front face
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y + this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z + this.Scale.z);

            //Right face
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y + this.Scale.y, this.Position.z - this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z - this.Scale.z);

            //Back face
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z - this.Scale.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z - this.Scale.z);
            //Left face

            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z + this.Scale.z);
            gl.End();

            gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
            //Top face      
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y + this.Scale.y, this.Position.z + this.Scale.z);
            //gl.Color(0, 0, 0);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z - this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y + this.Scale.y, this.Position.z - this.Scale.z);
            gl.End();

            gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
            //Bottom face
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z - this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z - this.Scale.z);
            gl.End();
            if (enabledUpdate) {
                UpdateMotion();
            }
        }

        public void DrawCircle(OpenGL gl, float lineWidth = 1.0f, int Resolution = 50) {

            gl.LineWidth(lineWidth);
            gl.Color(Color.r, Color.g, Color.b, Color.a);
            Resolution = (int)GameUtils.Constrain(Resolution, 10, 100);

            gl.Begin(OpenGL.GL_LINE_LOOP);
            for (int ii = 0; ii < Resolution; ii++) {
                double angle = 2.0f * Math.PI * ii / Resolution;
                double x = Radius * Math.Cos(angle);
                double y = Radius * Math.Sin(angle);
                gl.Vertex(x + this.Position.x, y + this.Position.y, this.Position.z);
            }
            gl.End();
            if (enabledUpdate) {
                UpdateMotion();
            }
        }

        public void DrawLine(OpenGL gl, Vector3 origin, Vector3 target, float lineWidth = 1.0f) {
            gl.LineWidth(lineWidth);
            gl.Color(Color.r, Color.g, Color.b, Color.a);
            gl.Begin(OpenGL.GL_LINE_STRIP);
            gl.Vertex(origin.x, origin.y, origin.z);
            gl.Vertex(target.x, target.y, target.z);
            gl.End();
            if (enabledUpdate) {
                UpdateMotion();
            }
        }

        public void DrawDottedLine(OpenGL gl, Vector3 origin, Vector3 target, float lineWidth = 1.0f, float MultScale = 1.0f, float space = 0) {
            gl.LineWidth(lineWidth);
            gl.Color(Color.r, Color.g, Color.b, Color.a);
            gl.Begin(OpenGL.GL_LINE_STRIP);
            gl.Vertex(origin.x, origin.y, origin.z);
            gl.Vertex(target.x, target.y, target.z);
            gl.End();
        }

        private void UpdateMotion() {
            Velocity += Acceleration;
            Position += Velocity;
            Acceleration *= 0;
        }

        public bool HasCollidedWith(Mesh target) {
            bool xHasNotCollided =
                this.Position.x - this.Scale.x - (this.Velocity.x / 2) > target.Position.x + target.Scale.x ||
                this.Position.x + this.Scale.x + (this.Velocity.x / 2) < target.Position.x - target.Scale.x;

            bool yHasNotCollided =
                this.Position.y - this.Scale.y + (this.Velocity.y / 2) > target.Position.y + target.Scale.y ||
                this.Position.y + this.Scale.y - (this.Velocity.y / 2) < target.Position.y - target.Scale.y;

            bool zHasNotCollided =
                this.Position.z - this.Scale.z > target.Position.z + target.Scale.z ||
                this.Position.z + this.Scale.z < target.Position.z - target.Scale.z;

            return !(xHasNotCollided || yHasNotCollided || zHasNotCollided);
            
        }
    }
}
