using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpGL;
using generationminijam_game.Models;

//For MiniGameJam 12Sept19 by Tabalong

namespace generationminijam_game {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
            Loaded += new RoutedEventHandler(delegate (object sender, RoutedEventArgs args) {
                //Load directly to the center
                Top = (SystemParameters.VirtualScreenHeight / 2) - (Height / 2);
                Left = (SystemParameters.VirtualScreenWidth / 2) - (Width / 2);
            });
            Title = "Blunondrum";
        }

        private void OnMouseMove(object sender, MouseEventArgs e) {
            var position = e.GetPosition(this);
            mousePos.x = (float)position.X - (float)Width / 2.0f;
            mousePos.y = -((float)position.Y - (float)Height / 2.0f);
            //Console.WriteLine((mousePos.x) + " " + (mousePos.y));
        }

        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args) {
            OpenGL gl = args.OpenGL;

            gl.Enable(OpenGL.GL_DEPTH_TEST);

            float[] global_ambient = new float[] { 0.5f, 0.5f, 0.5f, 1.0f };
            float[] light0pos = new float[] { 0.0f, 5.0f, 10.0f, 1.0f };
            float[] light0ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] light0diffuse = new float[] { 0.3f, 0.3f, 0.3f, 1.0f };
            float[] light0specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };

            float[] lmodel_ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, lmodel_ambient);

            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);

            gl.ShadeModel(OpenGL.GL_SMOOTH);
        }

        private void OpenGLControl_Resized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args) {
            //  Get the OpenGL object.
            OpenGL gl = args.OpenGL;
            //  Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            //  Load the identity.
            gl.LoadIdentity();
            //  Create a perspective transformation.
            gl.Perspective(90.0f, (double)Width / (double)Height, 0.01, 500.0);
            //  Use the 'look at' helper function to position and aim the camera.
            gl.LookAt(0, 1.0f, 45.0f, 0, 1.0f, -250.0f, 0, 1, 0);
            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        //Meshes      
        private Actor Player = new Actor() {
            Position = new Vector3(0, 10, -50),
            Scale = new Vector3(10, 10, 10),
            Color = new Vector4(0.0f, 1.0f, 1.0f, 1.0f)
        };
        private Actor Enemy = new Actor() {
            isEnemy = true,
            Position = new Vector3(200, 10, -70),
            Scale = new Vector3(10, 10, 10),
            Color = new Vector4(1.0f, 0.4f, 0.4f, 1.0f)
        };
        private Mesh SpinningMesh = new Mesh() {
            Position = new Vector3(0, 10, -6),
            Scale = new Vector3(10, 10, 10),
            Color = new Vector4(0.0f, 1.0f, 1.0f, 1.0f)

        };
        private List<Structure> Platforms = new List<Structure>();
        private Structure Platform = new Structure() {
            Position = new Vector3(0, 0, -50),
            Scale = new Vector3(100, 0, 100),
            Color = new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
            Grid = true
        };

        //System Related
        private Vector3 mouseVector = new Vector3();
        private Vector3 mousePos = new Vector3();
        //public IWavePlayer waveOutDevice = new WaveOut();
        //public Mp3FileReader hypnothisMus = new Mp3FileReader("Resources/hypnothis-by-kevin-macleod.mp3");
        //public Mp3FileReader werqMus = new Mp3FileReader("Resources/werq-by-kevin-macleod.mp3");

        Vector3 STANDARD_POSITION = new Vector3(100, 300, 0);
        Vector4 BLUE_COLOR = new Vector4(0.4f, 0.4f, 0.9f, 1.0f);
        Vector4 GREEN_COLOR = new Vector4(0.1f, 0.7f, 0.1f, 1.0f);
        Vector4 YELLOW_COLOR = new Vector4(0.7f, 0.7f, 0.1f, 1.0f);
        Vector4 WHITE_TRANSPARENT_COLOR = new Vector4(0.1f, 0.1f, 0.1f, 0.8f);

        public bool levelLoaded = false;
        public bool goalLoaded = false;
        private bool dialogueEnabled = false;
        private float dialogueDuration = 30;
        private float dialogueNextDuration;
        private int dialogueIndex = 0;
        private int level = -1;
        private bool dying = false;
        private int resetCountdown = 30;
        private int SpinningMeshRotate;
        private bool isGoalReached = false;

        private float deathHeight = -50.0f;

        //1st level Array - Position (left/right,up/down,depth). 2nd (n/a, n/a, goal)
        private int[,,] testmap = { { { 0, 0, 0 }, { 1, 0, 10 }, { 2, 0, 0 }, { 0, 1, 10 }, { 0, -1, -20 }, { -1, 0, -10 }, {3, 0, 0 } },
                                    { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, {0, 0, 1 } } };
        private int[,,] level1Map = { { { 0, 0, 0 }, { 1, 0, 0}, { 2, 0, 0}, { 3, 0, 0 } },
                                      { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 1 } } };
        private int[,,] level2Map = { { { 0, 0, 0 }, { 1, 0, 0 }, { 1, -1, 0}, { 2, -1, 0 } },
                                      { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 1 } } };
        private int[,,] level3Map = { { { 0, 0, 0 }, { 1, 0, 0 }, { 1, 1, 0 }, { 2, 1, 0 }, { 4, 1, 0 }, { 5, 1, 0} },
                                      { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 1} } };
        private int[,,] level4Map = { { { 0, 0, 0 }, { 0, -1, 0 }, { 0, -2, 0 }, { 0, -3, 0 }, { -1, -3, 0 }, { 1, -3, 0 }, { -2, -3, 0 }, { 2, -3, 0 }, { -2, -2, 0 }, { 2, -2, 0 }, { -2, -1, 0 }, { 2, -1, 0 }, { -2, 0, 0 }, { 2, 0, 0 }, { -2, 1, 0 } },
                                      { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0}, { 0, 0, 0}, { 0, 0, 0}, { 0, 0, 0}, { 0, 0, 0}, { 0, 0, 0}, { 0, 0, 0}, { 0, 0, 0}, { 0, 0, 0}, { 0, 0, 1} } };

        public bool isStartMusPlayed = false;

        public void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args) {
            OpenGL gl = args.OpenGL;
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            RenderColor(gl);
            Render3D(gl);

            #region DrawLevel
            switch (level) {
                case -1: //TEST LEVEL
                    Player.EnableAll();
                    Player.startingPosition = new Vector3(0, 10, -50);
                    LoadLevel(gl, testmap);                   
                    break;
                case 0: //MAIN MENU
                    gl.Rotate(20, 24 + SpinningMeshRotate, 20);
                    SpinningMesh.DrawCube(gl);
                    SpinningMeshRotate++;
                    gl.LoadIdentity();
                    if (Keyboard.IsKeyDown(Key.Enter)) {
                        level = 1;
                    }
                    break;
                case 1:
                    Player.startingPosition = new Vector3(0, 10, -50);
                    LoadLevel(gl, level1Map);
                    break;
                case 2:
                    Player.startingPosition = new Vector3(0, 10, -50);
                    LoadLevel(gl, level2Map);
                    break;
                case 3:
                    Player.startingPosition = new Vector3(0, 10, -50);
                    LoadLevel(gl, level3Map);
                    break;
                case 4:
                    Player.startingPosition = new Vector3(0, 10, -50);
                    LoadLevel(gl, level4Map);
                    break;
            }
            #endregion

            #region Game Loop
            if (level != 0) {
                gl.LookAt(Player.Position.x + mousePos.x / 12, GameUtils.Constrain(100.0f + Player.Position.z, 50, 400), GameUtils.Constrain(-10.0f + Player.Position.z, -200, 200), Player.Position.x + mousePos.x / 12, -90.0f, GameUtils.Constrain(-250.0f + Player.Position.z, -300.0f, -250.0f), 0, 1, 0);
                gl.Translate(0, 0, -50.0f);

                //Draw Player
                Player.DrawCube(gl);
                //Then draw platforms
                foreach (var platform in Platforms) {
                    platform.Draw(gl);
                    if (Player.HasCollidedWith(platform)) {
                        if (platform.Goal) {
                            isGoalReached = true;
                        }
                    }
                }
                //Enable Player Gravity, floor is platforms
                Player.AllowGravity(Platforms);

                Player.EnableControl(true);

                if (Player.Position.y < deathHeight) {
                    dying = true;
                }

                //Effect when player dies
                if (dying) {
                    resetCountdown--;
                    Player.Fade();
                }

                //Effect when player reacjed goal
                if ((isGoalReached) && (resetCountdown > 0)) {
                    resetCountdown--;
                    Player.Shrink();
                }

                if ((resetCountdown <= 0) && (!isGoalReached)) {
                    Player.Velocity *= 0;
                    dying = false;
                    resetCountdown = 60;
                    Player.EnableControl(true);
                    Player.Color = new Vector4(0.0f, 1.0f, 1.0f, 1.0f);
                    Player.ResetToPosition();
                    Player.Scale = new Vector3(10, 10, 10);
                }

                if ((resetCountdown <= 0) && (isGoalReached)) {
                    Player.Velocity *= 0;
                    Player.AllowGravity(Platforms, false);
                }
            }
            #endregion

            Render2D(gl); 

            #region Dialogue
            switch (level) {
                case 0:
                    break;
                case 1:
                    switch (dialogueIndex) {
                        case 0:
                            dialogueNextDuration = 40;
                            dialogueEnabled = true;
                            break;
                        case 1:
                            DialogBox(gl, "Hey Blu, what is that?", GREEN_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 2:
                            DialogBox(gl, "Ah, it's just some AI that I created.", BLUE_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 3:
                            DialogBox(gl, "What does it do?", GREEN_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 4:
                            DialogBox(gl, "Nothing much yet, but it can move to the right.", BLUE_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            DialogBox(gl, "(Press D)", YELLOW_COLOR, new Vector4(0, 0, 0, 0), STANDARD_POSITION, 600, 100, 22, 20, 80);
                            Player.canMoveRight = true;
                            dialogueNextDuration = 80;
                            break;
                        case 5:
                            dialogueEnabled = false;
                            if (isGoalReached) {
                                dialogueIndex = 6;
                                dialogueEnabled = true;
                                dialogueNextDuration = 80;
                            }
                            break;
                        case 6:
                            DialogBox(gl, "... That's it?", GREEN_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 7:
                            DialogBox(gl, "Yeah, but it's amazing isn't it?", BLUE_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 8:
                            DialogBox(gl, "I guess so...", GREEN_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 9:
                            resetCountdown = 60;
                            isGoalReached = false;
                            dialogueEnabled = false;
                            levelLoaded = false;
                            goalLoaded = false;
                            Player.Scale = new Vector3(10, 10, 10);
                            Player.Color = new Vector4(0.0f, 1.0f, 1.0f, 1.0f);
                            level++;
                            break;
                    }
                    break;
                case 2:
                    switch (dialogueIndex) {
                        case 0:
                            dialogueEnabled = true;
                            DialogBox(gl, "2 years later", YELLOW_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 1:
                            DialogBox(gl, "Ah, I remember this, how is it going along?.", GREEN_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 2:
                            DialogBox(gl, "This time, it can move in all directions!", BLUE_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            DialogBox(gl, "(W, A, S, D)", YELLOW_COLOR, new Vector4(0, 0, 0, 0), STANDARD_POSITION, 600, 100, 22, 20, 80);
                            Player.canMoveUp = true;
                            Player.canMoveLeft = true;
                            Player.canMoveDown = true;
                            dialogueNextDuration = 80;
                            break;
                        case 3:
                            DialogBox(gl, "It's been 2 years, right?", GREEN_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 4:
                            dialogueEnabled = false;
                            if (isGoalReached) {
                                dialogueIndex = 5;
                                dialogueEnabled = true;
                                dialogueNextDuration = 80;
                            }
                            break;
                        case 5:
                            DialogBox(gl, "Aren't you going a bit too slow?", GREEN_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 6:
                            DialogBox(gl, "Nonsense. You have to be careful with AI.", BLUE_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 7:
                            DialogBox(gl, "Next thing you know they might develop sentience", BLUE_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            DialogBox(gl, "and rebel against us! Know what I mean?", BLUE_COLOR, new Vector4(0, 0, 0, 0), STANDARD_POSITION, 600, 100, 22, 20, 80);
                            dialogueNextDuration = 80;
                            break;
                        case 8:
                            DialogBox(gl, "With the rate you're going,", GREEN_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            DialogBox(gl, "we will probably destroy ourselves sooner.", GREEN_COLOR, new Vector4(0, 0, 0, 0), STANDARD_POSITION, 600, 100, 22, 20, 80);
                            dialogueNextDuration = 80;
                            break;
                        case 9:
                            isGoalReached = false;
                            dialogueEnabled = false;
                            levelLoaded = false;
                            goalLoaded = false;
                            level++;
                            break;
                    }
                    break;
                case 3:
                    switch (dialogueIndex) {
                        case 0:
                            dialogueEnabled = true;
                            DialogBox(gl, "4 years later", YELLOW_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 1:
                            DialogBox(gl, "How is your kids doing?.", GREEN_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 2:
                            DialogBox(gl, "They are fine but more importantly,", BLUE_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            DialogBox(gl, "look at this!", BLUE_COLOR, new Vector4(0, 0, 0, 0), STANDARD_POSITION, 600, 100, 22, 20, 80);
                            Player.canSprint= true;
                            Player.canJump = true;
                            dialogueNextDuration = 80;
                            break;
                        case 3:
                            DialogBox(gl, "You still working on that?", GREEN_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            DialogBox(gl, "Can it talk now?", GREEN_COLOR, new Vector4(0,0,0,0), STANDARD_POSITION, 600, 100,22,20,80);
                            dialogueNextDuration = 80;
                            break;
                        case 4:
                            DialogBox(gl, "No no, it can Jump and Run!", BLUE_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            DialogBox(gl, "(Space, LeftShift)", YELLOW_COLOR, new Vector4(0, 0, 0, 0), STANDARD_POSITION, 600, 100, 22, 20, 80);
                            Player.canJump = true;
                            dialogueNextDuration = 80;
                            break;
                        case 5:
                            DialogBox(gl, "Guugle already have develop AI personalities you know?", GREEN_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 6:
                            dialogueEnabled = false;
                            if (isGoalReached) {
                                dialogueIndex = 7;
                                dialogueEnabled = true;
                                dialogueNextDuration = 80;
                            }
                            break;
                        case 7:
                            DialogBox(gl, "What's important is that you're improving.", BLUE_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 8:
                            DialogBox(gl, "But what's the point", GREEN_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            DialogBox(gl, "if someone already did it way better?", GREEN_COLOR, new Vector4(0, 0, 0, 0), STANDARD_POSITION, 600, 100, 22, 20, 80);

                            dialogueNextDuration = 80;
                            break;
                        case 9:
                            DialogBox(gl, "You'll understand it if you make your own as well", BLUE_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 10:
                            DialogBox(gl, "No thanks, I'll pass.", GREEN_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 11:
                            isGoalReached = false;
                            dialogueEnabled = false;
                            levelLoaded = false;
                            goalLoaded = false;
                            level++;
                            break;
                    }
                    break;
                case 4:
                    switch (dialogueIndex) {
                        case 0:
                            dialogueEnabled = true;
                            DialogBox(gl, "1 year later", YELLOW_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 1:
                            DialogBox(gl, "It's getting cold out there.", GREEN_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 2:
                            DialogBox(gl, "Eh, we had worse.", BLUE_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            Player.canSprint = true;
                            Player.canJump = true;
                            dialogueNextDuration = 80;
                            break;
                        case 3:
                            DialogBox(gl, "What's the progress?", GREEN_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 4:
                            DialogBox(gl, "Nothing,", BLUE_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            DialogBox(gl, "I'm not sure what else to add anymore.", BLUE_COLOR, new Vector4(0, 0, 0, 0), STANDARD_POSITION, 600, 100, 22, 20, 80);
                            Player.canJump = true;
                            dialogueNextDuration = 80;
                            break;
                        case 5:
                            DialogBox(gl, "How about... giving it something to fight with?", GREEN_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 6:
                            dialogueEnabled = false;
                            if (isGoalReached) {
                                dialogueIndex = 7;
                                dialogueEnabled = true;
                                dialogueNextDuration = 80;
                            }
                            break;
                        case 7:
                            DialogBox(gl, "What? Why would I do that?", BLUE_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 8:
                            DialogBox(gl, "I don't know,", GREEN_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            DialogBox(gl, "Maybe just a test?", GREEN_COLOR, new Vector4(0, 0, 0, 0), STANDARD_POSITION, 600, 100, 22, 20, 80);

                            dialogueNextDuration = 80;
                            break;
                        case 9:
                            DialogBox(gl, "I don't want to see it get hurt.", BLUE_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 10:
                            DialogBox(gl, "Are you serious?", GREEN_COLOR, WHITE_TRANSPARENT_COLOR, STANDARD_POSITION, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 11:
                            isGoalReached = false;
                            dialogueEnabled = false;
                            levelLoaded = false;
                            goalLoaded = false;
                            //Platforms = new List<Structure>();
                            level++;
                            break;
                    }
                    break;
            }

            if (dialogueEnabled) {
                if (dialogueDuration > 0) {
                    dialogueDuration -= 1;
                }
                else {
                    dialogueIndex++;
                    dialogueDuration = dialogueNextDuration;
                }
            }

            #endregion

            #region UI
            if (level == 0) { //MAIN MENU
                DialogBox(gl, "Blunondrum", BLUE_COLOR, new Vector4(0,0,0,0), new Vector3((float)Width / 4, 50, 0), 280, 60, 50);
                DialogBox(gl, "Press Enter to Start", GREEN_COLOR, WHITE_TRANSPARENT_COLOR, new Vector3((float)Width/3, 300, 0), 280, 60, 25);
            }
            else {
                DialogBox(gl, "Level", BLUE_COLOR, new Vector4(0.3f, 0.3f, 0.3f, 0.3f), new Vector3(20, 20, 0), 90, 20, 20, 5, 45);
                DialogBox(gl, level.ToString(), GREEN_COLOR, new Vector4(0, 0, 0, 0), new Vector3(20, 20, 0), 100, 20, 20, 55, 45);
            }
            #endregion

            if (Keyboard.IsKeyDown(Key.Escape)) {
                Environment.Exit(0);
            }
        }

        private void RenderColor(OpenGL gl) {
            //Set Background Color
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);

            float[] global_ambient = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] light0pos = new float[] { 50.0f, 10.0f, 6.0f, 1.0f };
            float[] light0ambient = new float[] { 1.0f, 1.0f, 1.0f, 0.5f };
            float[] light0diffuse = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] light0specular = new float[] { 0.0f, 0.0f, 1.0f, 1.0f };
            float[] lmodel_ambient = new float[] { 1.2f, 1.2f, 1.2f, 1.0f };

            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, lmodel_ambient);
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);

            gl.ColorMaterial(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT_AND_DIFFUSE);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);

            gl.ShadeModel(OpenGL.GL_SMOOTH);
            gl.Enable(OpenGL.GL_LINE_SMOOTH);

            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            gl.Enable(OpenGL.GL_BLEND);
        }

        public void Render3D(OpenGL gl) {
            //  Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            //  Load the identity.
            gl.LoadIdentity();
            //  Create a perspective transformation.
            gl.Perspective(90.0f, (double)Width / (double)Height, 0.01, 700.0);
            //  Use the 'look at' helper function to position and aim the camera.
            gl.LookAt(0, 1.0f, 45.0f, 0, 1.0f, -250.0f, 0, 1, 0);
            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        public void Render2D(OpenGL gl) {
            gl.LoadIdentity();
            //  Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            //  Load the identity.
            gl.LoadIdentity();
            //  Create a perspective transformation.
            gl.Ortho(0, Width, Height, 0, 0, 100);
            //  Use the 'look at' helper function to position and aim the camera.
            gl.LookAt(0, 1.0f, 45.0f, 0, 1.0f, -250.0f, 0, 1, 0);
            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        public void DialogBox(OpenGL gl, string Text, Vector4 TextColor, Vector4 BoxColor, Vector3 Position, float width, float height, int textSize = 22, int correctionx = 20, int correctiony = 40) {
            Mesh DialoguePanel = new Mesh {
                Position = Position,
                Scale = new Vector3(width, height, 0),
                Color = BoxColor
            };
            //Portrait.DrawPanel(gl);
            DialoguePanel.DrawPanel(gl);
            gl.DrawText((int)Position.x + correctionx, (int)Height - (int)Position.y - correctiony, TextColor.r, TextColor.g, TextColor.b, "Calibri", textSize, Text);
        }

        public void LoadLevel(OpenGL gl, int[,,] level) {        
            while (!levelLoaded) {
                Platforms = new List<Structure>();
                for (int i = 0; i < level.GetLength(0); i++) {
                    for (int ii = 0; ii < level.GetLength(1); ii++) {
                        switch(i) {
                            case 0:
                                int x = level[i, ii, 0];
                                int y = level[i, ii, 2];
                                int z = level[i, ii, 1];
                                Platforms.Add(new Structure() {
                                    ID = ii,
                                    Scale = new Vector3(50, 0, 50),
                                    Position = new Vector3(x * 100, y, -50 + (z * 100)),
                                    Color = new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                                    Grid = true
                                });
                                break;
                            case 1:
                                if (level[i, ii, 2] == 1) {
                                    int index = Platforms.FindIndex(id => id.ID == ii);
                                    Platforms[index].Goal = true;
                                }
                                break;
                        }                 
                    }
                }
                //Sort list based on height, bottom gets to be drawn first for the aesthetic
                Platforms.Sort((x, y) => x.Position.y.CompareTo(y.Position.y));
                dialogueIndex = 0;
                Player.ResetToPosition();
                levelLoaded = true;
            }
        }
    }
}
