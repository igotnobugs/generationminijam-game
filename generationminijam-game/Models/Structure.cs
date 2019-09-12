using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGL;

namespace generationminijam_game.Models {
    class Structure : Mesh {

        public bool Grid;
        public int GridSize;
        public bool Goal;

        public Structure() {
            Grid = true;
            GridSize = 20;
            Goal = false;
        }

        public void Draw(OpenGL gl) {
            Mesh Line = new Mesh {
                Color = new Vector4(Color.r, Color.g, Color.b, Color.a)
            };
            Line.DrawLine(gl, new Vector3(Position.x - Scale.x, Position.y, Position.z - Scale.z), new Vector3(Position.x - Scale.x, Position.y, Position.z + Scale.z), 2);
            Line.DrawLine(gl, new Vector3(Position.x - Scale.x, Position.y, Position.z + Scale.z), new Vector3(Position.x + Scale.x, Position.y, Position.z + Scale.z), 2);
            Line.DrawLine(gl, new Vector3(Position.x + Scale.x, Position.y, Position.z + Scale.z), new Vector3(Position.x + Scale.x, Position.y, Position.z - Scale.z), 2);
            Line.DrawLine(gl, new Vector3(Position.x + Scale.x, Position.y, Position.z - Scale.z), new Vector3(Position.x - Scale.x, Position.y, Position.z - Scale.z), 2);
            if (Grid) {
                int xLinesToDraw = (int)(Scale.x * 2) / GridSize;
                int zLinesToDraw = (int)(Scale.z * 2) / GridSize;
                //Lines Left To Right
                for (int i = 0; i < xLinesToDraw; i++) {
                    Line.DrawLine(gl, new Vector3((Position.x - Scale.x) + (GridSize * i) , Position.y, Position.z + Scale.z), new Vector3((Position.x - Scale.x) + (GridSize * i), Position.y, Position.z - Scale.z));
                }
                //Lines Top to Bottom
                for (int i = 0; i < zLinesToDraw; i++) {
                    Line.DrawLine(gl, new Vector3(Position.x - Scale.x, Position.y, (GridSize * i) + (Position.z - Scale.z)), new Vector3(Position.x + Scale.x, Position.y, (GridSize * i) + (Position.z - Scale.z)));
                }
            }
        }



        public void DrawMovingGridLines(OpenGL gl) {
            Mesh Line = new Mesh {
                Color = new Vector4(1.0f, 0.0f, 1.0f, 1.0f)
            };
            //Center Line
            Line.DrawLine(gl, new Vector3(-550, 0, -200), new Vector3(550, 0, -200));
            //Bottom Line
            Line.DrawLine(gl, new Vector3(-250, 0, 0), new Vector3(250, 0, 0));
            //Vertical Lines To Right
            for (int i = 0; i < 25; i++) {
                Line.DrawLine(gl, new Vector3(20 * i, 0, 0), new Vector3(20 * i, 0, -200));
            }
            //Vertical Lines to Left
            for (int i = 1; i < 25; i++) {
                Line.DrawLine(gl, new Vector3(-20 * i, -0, 0), new Vector3(-20 * i, 0, -200));
            }
            //Lines Top to Bottom
            for (int i = 0; i < 10; i++) {
                Line.DrawLine(gl, new Vector3(-550, 0 - (3 * i), -200 + (20 * i)), new Vector3(550, 0 - (3 * i), -200 + (20 * i)));
            }

        }

    }
}
