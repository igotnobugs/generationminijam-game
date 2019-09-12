using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace generationminijam_game.Models {
    public class Vector4 {
        //Primarily used for colors
        public float r, g, b, a; 

        public Vector4() {
            r = 0; g = 0; b = 0; a = 0;
        }

        public Vector4(float r, float g, float b, float a = 1.0f) {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static Vector4 operator +(Vector4 left, Vector4 right) {
            return new Vector4(left.r + right.r,
                left.g + right.g,
                left.b + right.b,
                left.a + right.a);
        }

        public static Vector4 operator -(Vector4 left, Vector4 right) {
            return new Vector4(left.r - right.r,
                left.g - right.g,
                left.b - right.b,
                left.a - right.a);
        }

        public override string ToString() {
            return "r: " + r + " g: " + g + " b: " + b + " a: " + a;
        }
    }
}
