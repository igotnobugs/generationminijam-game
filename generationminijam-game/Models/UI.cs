using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGL;

namespace generationminijam_game.Models
{
    class UI : Mesh {

        int width;
        int height;

        public void Panel() {
            width = 0;
            height = 0;
        }
        public void DrawPanelX(OpenGL gl, float lineWidth = 1.0f) {
            gl.LineWidth(lineWidth);
            gl.Color(Color.r, Color.g, Color.b, Color.a);

            gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
            gl.Vertex(Position.x - (width/2), Position.y + (height/2), Position.z);
            gl.Vertex(Position.x - (width/2), Position.y - (height/2), Position.z);
            gl.Vertex(Position.x + (width/2), Position.y - (height/2), Position.z);
            gl.Vertex(Position.x - (width/2), Position.y + (height/2), Position.z);
            gl.Vertex(Position.x + (width/2), Position.y + (height/2), Position.z);
            gl.Vertex(Position.x + (width/2), Position.y - (height/2), Position.z);
            gl.End();


        }

        public void DrawText(string text, int size, Vector4 color) {

        }


    }
}
