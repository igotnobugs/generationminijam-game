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
using NAudio.Wave;

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
        private List<Structure> Platforms = new List<Structure>();
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
        private Structure Platform = new Structure() {
            Position = new Vector3(0, 0, -50),
            Scale = new Vector3(100, 0, 100),
            Color = new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
            Grid = true
        };

        //System Related
        private Vector3 mouseVector = new Vector3();
        private Vector3 mousePos = new Vector3();
        public IWavePlayer waveOutDevice = new WaveOut();
        public Mp3FileReader hypnothisMus = new Mp3FileReader("Resources/hypnothis-by-kevin-macleod.mp3");

        Vector3 standardPosition = new Vector3(100, 300, 0);
        Vector4 blueColor = new Vector4(0.4f, 0.4f, 0.9f, 1.0f);
        Vector4 greenColor = new Vector4(0.1f, 0.7f, 0.1f, 1.0f);
        Vector4 yellowColor = new Vector4(0.7f, 0.7f, 0.1f, 1.0f);
        Vector4 whiteTransparentColor = new Vector4(0.1f, 0.1f, 0.1f, 0.8f);
    
        public bool levelLoaded = false;
        public bool goalLoaded = false;
        private bool dialogueEnabled = false;
        private float dialogueDuration = 30;
        private float dialogueNextDuration;
        private int dialogueIndex = 0;
        private int level = 0;
        private bool ActorInPlatform = true;
        private bool dying = false;
        private int startTime = 20;
        private int resetCountdown = 30;
        private bool win = false;
        private int SpinningMeshRotate;

        //x,y = center, x -> Left/Right, y -> Up/Down
        private int[,] level1Map = { { 0, 0 }, { 1, 0 }, { 2, 0 } };
        private int[,] level1Goal = { { 3, 0 } };
        private int[,] level2Map = { { 0, 0 }, { 1, 0 }, { 1, -1} };
        private int[,] level2Goal = { { 2, -1 } };
        private int[,] level3Map = { { 0, 0 }, { 1, 0 }, { 1, 1 } , { 2, 1 } , { 4, 1 }};
        private int[,] level3Goal = { { 5, 1 } };
        private int[,] level4Map = { { 0, 0 }, { 0, -1 }, { 0, -2 }, { 0, -3 }, { -1, -3 }, { 1, -3 }, { -2, -3 }, { 2, -3 }, { -2, -2 }, { 2, -2 }, { -2, -1 }, { 2, -1 }, { -2, 0 }, { 2, 0 } };
        private int[,] level4Goal = { { -2, 1 } };

        public bool isStartMusPlayed = false;


        public void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args) {

            OpenGL gl = args.OpenGL;
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            RenderColor(gl);
            Render3D(gl);

            #region DrawLevel
            switch (level) {
                case 0:
                    gl.Rotate(20, 24 + SpinningMeshRotate, 20);
                    SpinningMesh.DrawCube(gl);
                    SpinningMeshRotate++;
                    gl.LoadIdentity();
                    if (Keyboard.IsKeyDown(Key.Enter)) {
                        level = 1;
                    }
                    break;
                case 1:
                    LoadLevel(level1Map);
                    LoadGoal(level1Goal);
                    break;
                case 2:
                    LoadLevel(level2Map);
                    LoadGoal(level2Goal);
                    break;
                case 3:
                    LoadLevel(level3Map);
                    LoadGoal(level3Goal);
                    break;
                case 4:
                    LoadLevel(level4Map);
                    LoadGoal(level4Goal);
                    break;
            }
            #endregion

            if (Keyboard.IsKeyDown(Key.Escape)) {
                Environment.Exit(0);
            }

            if (level != 0) {
                gl.LookAt(Player.Position.x + mousePos.x / 12, GameUtils.Constrain(100.0f + Player.Position.z, 50, 400), GameUtils.Constrain(-10.0f + Player.Position.z, -200, 200), Player.Position.x + mousePos.x / 12, -90.0f, GameUtils.Constrain(-250.0f + Player.Position.z, -300.0f, -250.0f), 0, 1, 0);
                gl.Translate(0, 0, -50.0f);

                if (!isStartMusPlayed) {
                    waveOutDevice.Init(hypnothisMus);
                    waveOutDevice.Play();
                    isStartMusPlayed = true;
                }
        
                Player.DrawCube(gl);
                if (level == 1) {
                    Enemy.DrawCube(gl);
                    Enemy.ChasePlayer(Player, true);
                }
                ActorInPlatform = false;

                if (Player.Position.y > Player.Scale.y) {
                    Player.isJumping = true;
                    Player.ApplyGravity();
                }
                else {
                    Player.Velocity.y = 0;
                    Player.isJumping = false;
                    Player.Position.y = Player.Scale.y;
                }

                foreach (var platform in Platforms) {
                    platform.Draw(gl);
                    if ((Player.HasCollidedWith(platform)) && (!ActorInPlatform)) {
                        Player.EnableControl(true);
                        ActorInPlatform = true;
                        if ((platform.Goal == true) && (!dying)) {
                            win = true;
                            dying = true;
                        }
                    }
                }

                if (startTime > 0) {
                    startTime--;
                } else {
                    if ((!ActorInPlatform) && (level > 0) && (!Player.isJumping)) {
                        dying = true;
                        Player.EnableControl(false);
                    }
                }

                if (dying) {
                    waveOutDevice.Volume = 0.5f;
                    resetCountdown--;
                    Player.Color.a -= 0.05f;
                    if (Player.Scale.x > 0) {
                        Player.Scale -= new Vector3(0.2f, 0.2f, 0.2f);
                    }
                }

                if ((resetCountdown < 0)  && (!win)) {
                    dying = false;
                    resetCountdown = 60;
                    Player.Color = new Vector4(0.0f, 1.0f, 1.0f, 1.0f);
                    Player.Position = new Vector3(0, 10, -50);
                    Player.Scale = new Vector3(10, 10, 10);
                    waveOutDevice.Volume = 1.0f;
                }

                gl.LoadIdentity();

            }
            Render2D(gl); //UI

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
                            DialogBox(gl, "Hey Blu, what is that?", greenColor, whiteTransparentColor, standardPosition, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 2:
                            DialogBox(gl, "Ah, it's just some AI that I created.", blueColor, whiteTransparentColor, standardPosition, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 3:
                            DialogBox(gl, "What does it do?", greenColor, whiteTransparentColor, standardPosition, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 4:
                            DialogBox(gl, "Nothing much yet, but it can move to the right.", blueColor, whiteTransparentColor, standardPosition, 600, 100);
                            DialogBox(gl, "(Press D)", yellowColor, new Vector4(0, 0, 0, 0), standardPosition, 600, 100, 22, 20, 80);
                            Player.EnableControl(true);
                            Player.canMoveRight = true;
                            dialogueNextDuration = 80;
                            break;
                        case 5:
                            dialogueEnabled = false;
                            if (win) {
                                dialogueIndex = 6;
                                dialogueEnabled = true;
                                dialogueNextDuration = 80;
                            }
                            break;
                        case 6:
                            DialogBox(gl, "... That's it?", greenColor, whiteTransparentColor, standardPosition, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 7:
                            DialogBox(gl, "Yeah, but it's amazing isn't it?", blueColor, whiteTransparentColor, standardPosition, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 8:
                            DialogBox(gl, "I guess so...", greenColor, whiteTransparentColor, standardPosition, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 9:
                            startTime = 10;
                            win = false;
                            dialogueEnabled = false;
                            level++;
                            levelLoaded = false;
                            goalLoaded = false;
                            Platforms = new List<Structure>();
                            break;
                    }
                    break;
                case 2:
                    switch (dialogueIndex) {
                        case 0:
                            dialogueEnabled = true;
                            DialogBox(gl, "2 years later", yellowColor, whiteTransparentColor, standardPosition, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 1:
                            DialogBox(gl, "Ah, I remember this, how is it going along?.", greenColor, whiteTransparentColor, standardPosition, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 2:
                            DialogBox(gl, "This time, it can move in all directions!", blueColor, whiteTransparentColor, standardPosition, 600, 100);
                            DialogBox(gl, "(W, A, S, D)", yellowColor, new Vector4(0, 0, 0, 0), standardPosition, 600, 100, 22, 20, 80);
                            Player.canMoveUp = true;
                            Player.canMoveLeft = true;
                            Player.canMoveDown = true;
                            dialogueNextDuration = 80;
                            break;
                        case 3:
                            DialogBox(gl, "It's been 2 years, right?", greenColor, whiteTransparentColor, standardPosition, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 4:
                            dialogueEnabled = false;
                            if (win) {
                                dialogueIndex = 5;
                                dialogueEnabled = true;
                                dialogueNextDuration = 80;
                            }
                            break;
                        case 5:
                            DialogBox(gl, "Aren't you going a bit too slow?", greenColor, whiteTransparentColor, standardPosition, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 6:
                            DialogBox(gl, "Nonsense. You have to be careful with AI.", blueColor, whiteTransparentColor, standardPosition, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 7:
                            DialogBox(gl, "Next thing you know they might develop sentience", blueColor, whiteTransparentColor, standardPosition, 600, 100);
                            DialogBox(gl, "and rebel against us! Know what I mean?", blueColor, new Vector4(0, 0, 0, 0), standardPosition, 600, 100, 22, 20, 80);
                            dialogueNextDuration = 80;
                            break;
                        case 8:
                            DialogBox(gl, "With the rate you're going,", greenColor, whiteTransparentColor, standardPosition, 600, 100);
                            DialogBox(gl, "we will probably destroy ourselves sooner.", greenColor, new Vector4(0, 0, 0, 0), standardPosition, 600, 100, 22, 20, 80);
                            dialogueNextDuration = 80;
                            break;
                        case 9:
                            win = false;
                            startTime = 10;
                            dialogueEnabled = false;
                            level++;
                            levelLoaded = false;
                            goalLoaded = false;
                            Platforms = new List<Structure>();
                            break;
                    }
                    break;
                case 3:
                    switch (dialogueIndex) {
                        case 0:
                            dialogueEnabled = true;
                            DialogBox(gl, "4 years later", yellowColor, whiteTransparentColor, standardPosition, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 1:
                            DialogBox(gl, "How is your kids doing?.", greenColor, whiteTransparentColor, standardPosition, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 2:
                            DialogBox(gl, "They are fine but more importantly,", blueColor, whiteTransparentColor, standardPosition, 600, 100);
                            DialogBox(gl, "look at this!", blueColor, new Vector4(0, 0, 0, 0), standardPosition, 600, 100, 22, 20, 80);
                            Player.canSprint= true;
                            Player.canJump = true;
                            dialogueNextDuration = 80;
                            break;
                        case 3:
                            DialogBox(gl, "You still working on that?", greenColor, whiteTransparentColor, standardPosition, 600, 100);
                            DialogBox(gl, "Can it talk now?", greenColor, new Vector4(0,0,0,0), standardPosition, 600, 100,22,20,80);
                            dialogueNextDuration = 80;
                            break;
                        case 4:
                            DialogBox(gl, "No no, it can Jump and Run!", blueColor, whiteTransparentColor, standardPosition, 600, 100);
                            DialogBox(gl, "(Space, LeftShift)", yellowColor, new Vector4(0, 0, 0, 0), standardPosition, 600, 100, 22, 20, 80);
                            Player.canJump = true;
                            dialogueNextDuration = 80;
                            break;
                        case 5:
                            DialogBox(gl, "Guugle already have develop AI personalities you know?", greenColor, whiteTransparentColor, standardPosition, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 6:
                            dialogueEnabled = false;
                            if (win) {
                                dialogueIndex = 7;
                                dialogueEnabled = true;
                                dialogueNextDuration = 80;
                            }
                            break;
                        case 7:
                            DialogBox(gl, "What's important is that you're improving.", blueColor, whiteTransparentColor, standardPosition, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 8:
                            DialogBox(gl, "But what's the point", greenColor, whiteTransparentColor, standardPosition, 600, 100);
                            DialogBox(gl, "if someone already did it way better?", greenColor, new Vector4(0, 0, 0, 0), standardPosition, 600, 100, 22, 20, 80);

                            dialogueNextDuration = 80;
                            break;
                        case 9:
                            DialogBox(gl, "You'll understand it if you make your own as well", blueColor, whiteTransparentColor, standardPosition, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 10:
                            DialogBox(gl, "No thanks, I'll pass.", greenColor, whiteTransparentColor, standardPosition, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 11:
                            win = false;
                            startTime = 10;
                            dialogueEnabled = false;
                            level++;
                            levelLoaded = false;
                            goalLoaded = false;
                            Platforms = new List<Structure>();
                            break;
                    }
                    break;
                case 4:
                    switch (dialogueIndex) {
                        case 0:
                            dialogueEnabled = true;
                            DialogBox(gl, "1 year later", yellowColor, whiteTransparentColor, standardPosition, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 1:
                            DialogBox(gl, "It's getting cold out there.", greenColor, whiteTransparentColor, standardPosition, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 2:
                            DialogBox(gl, "Eh, we had worse.", blueColor, whiteTransparentColor, standardPosition, 600, 100);
                            Player.canSprint = true;
                            Player.canJump = true;
                            dialogueNextDuration = 80;
                            break;
                        case 3:
                            DialogBox(gl, "What's the progress?", greenColor, whiteTransparentColor, standardPosition, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 4:
                            DialogBox(gl, "Nothing,", blueColor, whiteTransparentColor, standardPosition, 600, 100);
                            DialogBox(gl, "I'm not sure what else to add anymore.", blueColor, new Vector4(0, 0, 0, 0), standardPosition, 600, 100, 22, 20, 80);
                            Player.canJump = true;
                            dialogueNextDuration = 80;
                            break;
                        case 5:
                            DialogBox(gl, "How about... giving it something to fight with?", greenColor, whiteTransparentColor, standardPosition, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 6:
                            dialogueEnabled = false;
                            if (win) {
                                dialogueIndex = 7;
                                dialogueEnabled = true;
                                dialogueNextDuration = 80;
                            }
                            break;
                        case 7:
                            DialogBox(gl, "What? Why would I do that?", blueColor, whiteTransparentColor, standardPosition, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 8:
                            DialogBox(gl, "I don't know,", greenColor, whiteTransparentColor, standardPosition, 600, 100);
                            DialogBox(gl, "Maybe just a test?", greenColor, new Vector4(0, 0, 0, 0), standardPosition, 600, 100, 22, 20, 80);

                            dialogueNextDuration = 80;
                            break;
                        case 9:
                            DialogBox(gl, "I don't want to see it get hurt.", blueColor, whiteTransparentColor, standardPosition, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 10:
                            DialogBox(gl, "Are you serious?", greenColor, whiteTransparentColor, standardPosition, 600, 100);
                            dialogueNextDuration = 80;
                            break;
                        case 11:
                            win = false;
                            startTime = 10;
                            dialogueEnabled = false;
                            level++;
                            levelLoaded = false;
                            goalLoaded = false;
                            Platforms = new List<Structure>();
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
            if (level == 0) {
                DialogBox(gl, "Blunondrum", blueColor, new Vector4(0,0,0,0), new Vector3((float)Width / 4, 50, 0), 280, 60, 50);
                DialogBox(gl, "Press Enter to Start", greenColor, whiteTransparentColor, new Vector3((float)Width/3, 300, 0), 280, 60, 25);
            }
            else {
                DialogBox(gl, "Level", blueColor, new Vector4(0.3f, 0.3f, 0.3f, 0.3f), new Vector3(20, 20, 0), 90, 20, 20, 5, 45);
                DialogBox(gl, level.ToString(), greenColor, new Vector4(0, 0, 0, 0), new Vector3(20, 20, 0), 100, 20, 20, 55, 45);
            }
            #endregion
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

        public void LoadLevel(int[,] level) {
            while (!levelLoaded) {
                for (int i = 0; i < level.Length / 2; i++) {
                    int x = level[i, 0];
                    int z = level[i, 1];
                    Platforms.Add(new Structure() {
                        Position = new Vector3(x * 100, 0, -50 + (z * 100)),
                        Scale = new Vector3(50, 0, 50),
                        Color = new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                        Grid = true
                    });
                }
                dialogueIndex = 0;
                levelLoaded = true;
            }
        }
        public void LoadGoal(int[,] level) {
            while (!goalLoaded) {
                for (int i = 0; i < 1; i++) {
                    int x = level[i, 0];
                    int z = level[i, 1];
                    Platforms.Add(new Structure() {
                        Position = new Vector3(x * 100, 0, -50 + (z * 100)),
                        Scale = new Vector3(50, 0, 50),
                        Color = new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                        Grid = true,
                        Goal = true
                    });
                }
                goalLoaded = true;
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
