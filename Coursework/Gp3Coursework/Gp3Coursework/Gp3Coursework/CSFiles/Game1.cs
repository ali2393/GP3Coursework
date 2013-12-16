using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Gp3Coursework
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
         
        #region User Defined Variables
        //------------------------------------------
        // Added for use with fonts
        //------------------------------------------
        SpriteFont fontToUse;

        //--------------------------------------------------
        // Added for use with playing Audio via Media player
        //--------------------------------------------------
        private Song bkgMusic;
        private String songInfo;
        //--------------------------------------------------
        //Set the sound effects to use
        //--------------------------------------------------
        private SoundEffect explosionSound;
        private SoundEffect firingSound;

        // Set the 3D model to draw.
        private Model mdlShip;
        private Matrix[] mdlShipTransforms;

        //Set the score and health meters of the player
        private int score = 0;
        private int health = 100;

        // The aspect ratio determines how to scale 3d to 2d projection.
        private float aspectRatio;

        // Set the position of the model in world space, and set the rotation.
         Vector3 mdlPosition = new Vector3(0.0f, 3.0f, 290.0f);
        private float mdlRotation = MathHelper.ToRadians(180);
        private Vector3 mdlVelocity = Vector3.Zero;

        // create an array of Asteroids
        private Model mdlAsteroid;
        private Matrix[] mdlAsteroidTransforms;
        private Asteroids[] AsteroidList = new Asteroids[GameConstants.NumAsteroids];

        //Creates an array of enemy ships
        private Model mdlEShip;
        private Matrix[] mdlEShipTransforms;
        private EShips[] EShipList = new EShips[GameConstants.NumEShips];

        //Creates an array of start
        private Model mdlStar;
        private Matrix[] mdlStarTransforms;
        private Stars[] StarList = new Stars[GameConstants.NumStars];

        // create an array of laser bullets
        private Model mdlLaser;
        private Matrix[] mdlLaserTransforms;
        private Laser[] laserList = new Laser[GameConstants.NumLasers];

        //sets up a new random
        private Random random = new Random();

        //Gets the previous keyboard state
        private KeyboardState lastState;

        //gets the previous mousestate
        private MouseState lastMState;

        //gets the previous GamepadState
        private GamePadState lastGState;

        private int hitCount;

        //Variable to state what control type is being used 1:Mouse 2:keyboard 3:Xbox controler
        private int controlType;

        //boolean for mute
        private bool IsSoundOn = true;

        //Cameras that will be used in the game
        private camera MainCamera;
        private camera camera1;
        private camera camera2;
        
        //Initilizes the game state
        private string gameState= "MainMenu";

      
        #endregion

        //called form program.cs which is the entry point for the program sets up the graphics devices and window for the game
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            //gets the directory that holds the reasources needed
            Content.RootDirectory = "Content";
            //makes the mouse cursor visable
            this.IsMouseVisible = true;
        }

        //Method that holds the controlls of the game
        private void MoveModel()
        {
            //get Keyboard
            KeyboardState keyboardState = Keyboard.GetState();

            //get Gamepad
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            //get MouseState
            MouseState mouse = Mouse.GetState();

            //gets the position of the mouse
            Vector2 mousePos = new Vector2(mouse.X, mouse.Y);

            //gets the position of the player
            Vector2 playerPos = new Vector2(mdlPosition.X, mdlPosition.Y);

            //Toggle for Mute 
            if (keyboardState.IsKeyDown(Keys.M) && (!lastState.IsKeyDown(Keys.M)))
            {
                if (IsSoundOn == true)
                {
                    IsSoundOn = false;
                    MediaPlayer.IsMuted = true;
                }
                else
                {
                    IsSoundOn = true;
                    MediaPlayer.IsMuted = false;
                }
            }

            //Exits game if wcape key is pressed
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            //Code to unpause if the game is paused
            if (gameState == "Paused")
            {
                if (keyboardState.IsKeyDown(Keys.O) && (!lastState.IsKeyDown(Keys.O)))
                {
                    gameState = "Game";
                    camera1.InitilizeTransform(graphics);
                    MainCamera = camera1;
                }
            }

            //Controls for when the game is running
            if (gameState == "Game")
            {
                //pauses the game
                if (keyboardState.IsKeyDown(Keys.P) && (!lastState.IsKeyDown(Keys.P)))
                {
                    gameState = "Paused";
                    camera2.InitilizeTransform(graphics);
                    MainCamera = camera2;
                }

                //Holds controls for mouse
                if (controlType == 1)
                {
                    //Clamp mouse coordinates to viewport
                    if (mousePos.X < 0) mousePos.X = 0;
                    if (mousePos.Y < 0) mousePos.Y = 0;
                    if (mousePos.X > graphics.GraphicsDevice.Viewport.Width) mousePos.X = (short)graphics.GraphicsDevice.Viewport.Width;
                    if (mousePos.Y > graphics.GraphicsDevice.Viewport.Height) mousePos.Y = (short)graphics.GraphicsDevice.Viewport.Height;

                    //checks if left mouse button is pressed if so shoots laser
                    if (mouse.LeftButton == ButtonState.Pressed && (lastMState.LeftButton == ButtonState.Released))
                    {
                        //add another bullet.  Find an inactive bullet slot and use it
                        //if all bullets slots are used, ignore the user input
                        for (int i = 0; i < GameConstants.NumLasers; i++)
                        {
                            if (!laserList[i].isActive)
                            {
                                Matrix shipTransform = Matrix.CreateRotationY(mdlRotation);
                                laserList[i].direction = shipTransform.Backward;
                                laserList[i].speed = GameConstants.LaserSpeedAdjustment;
                                laserList[i].position = mdlPosition + laserList[i].direction;
                                laserList[i].isActive = true;

                                if (IsSoundOn == true)
                                {
                                    firingSound.Play();
                                }
                                break; //exit the loop     
                            }
                        }
                    }

                    //bottom left corener to top right corner
                    Vector3 nearSource = new Vector3((float)mousePos.X, (float)mousePos.Y, 0.0f);
                    Vector3 farSource = new Vector3((float)mousePos.X, (float)mousePos.Y, 1.0f);

                    //gets the near and far point of the viewport
                    Vector3 nearPoint = graphics.GraphicsDevice.Viewport.Unproject(nearSource, MainCamera.projectionMatrix, MainCamera.camViewMatrix, Matrix.Identity);
                    Vector3 farPoint = graphics.GraphicsDevice.Viewport.Unproject(farSource, MainCamera.projectionMatrix, MainCamera.camViewMatrix, Matrix.Identity);

                    //Determine the direction vector by subtracting maxPointSource from minPointSource
                    Ray ray = new Ray(nearPoint, Vector3.Normalize(farPoint - nearPoint));

                    //Define a plane for mouse movement
                    Plane plane = new Plane(Vector3.Forward, 290);

                    //normalises the mouse movement against the plane 
                    float denominator = Vector3.Dot(plane.Normal, ray.Direction);
                    float numerator = Vector3.Dot(plane.Normal, ray.Position) + plane.D;
                    float t = -(numerator / denominator);

                    //moves the model with the mouse
                    mdlPosition = (nearPoint + ray.Direction * t);
                }

                //code for keyboard control
                if (controlType == 2)
                {
                    //binds the movement within the game screen
                    if (mdlPosition.X < -7) mdlPosition.X = -7;
                    if (mdlPosition.Y < 0) mdlPosition.Y = 0;
                    if (mdlPosition.X > 7) mdlPosition.X = 7;
                    if (mdlPosition.Y > 7) mdlPosition.Y = 7;

                    //directional movement of the ship
                    if (keyboardState.IsKeyDown(Keys.Left))
                    {
                        mdlPosition.X = mdlPosition.X - 0.5f;
                    }

                    if (keyboardState.IsKeyDown(Keys.Right))
                    {
                        mdlPosition.X = mdlPosition.X + 0.5f;
                    }

                    if (keyboardState.IsKeyDown(Keys.Up))
                    {
                        mdlPosition.Y = mdlPosition.Y + 0.5f;
                    }

                    if (keyboardState.IsKeyDown(Keys.Down))
                    {
                        mdlPosition.Y = mdlPosition.Y - 0.5f;
                    }

                    //fires a shot if the space key is pressed and there are lasers to fire
                    if (keyboardState.IsKeyDown(Keys.Space) && (!lastState.IsKeyDown(Keys.Space)))
                    {
                        //add another bullet.  Find an inactive bullet slot and use it
                        //if all bullets slots are used, ignore the user input
                        for (int i = 0; i < GameConstants.NumLasers; i++)
                        {
                            if (!laserList[i].isActive)
                            {
                                Matrix shipTransform = Matrix.CreateRotationY(mdlRotation);
                                laserList[i].direction = shipTransform.Backward;
                                laserList[i].speed = GameConstants.LaserSpeedAdjustment;
                                laserList[i].position = mdlPosition + laserList[i].direction;
                                laserList[i].isActive = true;

                                //plays laser firing sound
                                if (IsSoundOn == true)
                                {
                                    firingSound.Play();
                                }
                                break; //exit the loop     
                            }
                        }
                    }
                }

                //control for xbox controler
                if (controlType == 3)
                {
                    //locks ship to game area
                    if (mdlPosition.X < -7) mdlPosition.X = -7;
                    if (mdlPosition.Y < 0) mdlPosition.Y = 0;
                    if (mdlPosition.X > 7) mdlPosition.X = 7;
                    if (mdlPosition.Y > 7) mdlPosition.Y = 7;

                    // Allows the game to exit
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    {
                        this.Exit();
                    }

                    if (gamePadState.DPad.Left == ButtonState.Pressed)
                    {
                        // Moves left.
                        mdlPosition.X = mdlPosition.X - 0.5f;
                    }

                    if (gamePadState.DPad.Right == ButtonState.Pressed)
                    {
                        //Moves Right
                        mdlPosition.X = mdlPosition.X + 0.5f;
                    }

                    if (gamePadState.DPad.Up == ButtonState.Pressed)
                    {
                        // MovesUP left.
                        mdlPosition.Y = mdlPosition.Y + 0.5f;
                    }

                    if (gamePadState.DPad.Down == ButtonState.Pressed)
                    {
                        // Moves Down
                        mdlPosition.Y = mdlPosition.Y - 0.5f;
                    }

                    if (gamePadState.Buttons.Start == ButtonState.Pressed)
                    {
                        //Resets Game
                        mdlVelocity = Vector3.Zero;
                        mdlPosition = new Vector3(0.0f, 3.0f, 290.0f);
                        mdlRotation = MathHelper.ToRadians(180);
                        PositionModels();
                    }

                    //fires a laser when A button is pressed if there are lasers in the list to fire
                    if (gamePadState.Buttons.A == ButtonState.Pressed && (lastGState.Buttons.A == ButtonState.Released))
                    {
                        //add another bullet.  Find an inactive bullet slot and use it
                        //if all bullets slots are used, ignore the user input
                        for (int i = 0; i < GameConstants.NumLasers; i++)
                        {
                            if (!laserList[i].isActive)
                            {
                                Matrix shipTransform = Matrix.CreateRotationY(mdlRotation);
                                laserList[i].direction = shipTransform.Backward;
                                laserList[i].speed = GameConstants.LaserSpeedAdjustment;
                                laserList[i].position = mdlPosition + laserList[i].direction;
                                laserList[i].isActive = true;

                                if (IsSoundOn == true)
                                {
                                    firingSound.Play();
                                }
                                break; //exit the loop     
                            }
                        }
                    }
                }

                //resets the game
                if (keyboardState.IsKeyDown(Keys.R))
                {
                    mdlVelocity = Vector3.Zero;
                    mdlPosition = new Vector3(0.0f, 3.0f, 290.0f);
                    mdlRotation = MathHelper.ToRadians(180);

                    health = 100;
                    score = 0;
                    PositionModels();
                }
            }

            //code for main menu controls
            if (gameState == "MainMenu")
            {
                //sets the control scheme and starts the game
                if (keyboardState.IsKeyDown(Keys.D1))
                {
                    gameState = "Game";
                    controlType = 1;
                }

                if (keyboardState.IsKeyDown(Keys.D2))
                {
                    gameState = "Game";
                    controlType = 2;
                }

                if (keyboardState.IsKeyDown(Keys.D3))
                {
                    gameState = "Game";
                    controlType = 3;
                }
            }
            //updates the last state for the control types
            lastState = keyboardState;
            lastMState = mouse;
            lastGState = gamePadState;
        }

        //initilises the positions of the models
        private void PositionModels()
        {
            //sets up variables that will be used
            float xStart;
            float yStart;
            float zStart;

            for (int i = 0; i < GameConstants.NumAsteroids; i++)
            {
                //randomises the start postions of the asteroids
                xStart = random.Next(-7, 8);
                yStart = random.Next(-7, 8);
                zStart = random.Next(-7, 8);

                //sets the direction that the asteroids will move in
                double angle = random.NextDouble() * 2 * Math.PI;
                AsteroidList[i].direction.Z = 1;

                //positions and sets the asteroids moving
                AsteroidList[i].position = new Vector3(xStart, yStart, zStart);
                AsteroidList[i].direction.X = 0;
                AsteroidList[i].direction.Z = AsteroidList[i].direction.Z;
                AsteroidList[i].direction.Y = 0;
                AsteroidList[i].speed = GameConstants.AsteroidMinSpeed + (float)random.NextDouble() * GameConstants.AsteroidMaxSpeed;
                AsteroidList[i].isActive = true;

            }

            //same as asteroids but for the stars
            for (int i = 0; i < GameConstants.NumStars; i++)
            {

                xStart = random.Next(-7, 8);
                yStart = random.Next(-7, 8);
                zStart = random.Next(-7, 8);

                double angle = random.NextDouble() * 2 * Math.PI;
                StarList[i].direction.Z = 1;

                StarList[i].position = new Vector3(xStart, yStart, zStart);
                StarList[i].direction.X = 0;
                StarList[i].direction.Z = StarList[i].direction.Z;
                StarList[i].direction.Y = 0;
                StarList[i].speed = GameConstants.StarMinSpeed + (float)random.NextDouble() * GameConstants.StarMaxSpeed;
                StarList[i].isActive = true;

            }

            //same as asteroids but for the enemy ships
            for (int i = 0; i < GameConstants.NumEShips; i++)
            {

                xStart = random.Next(-7, 8);
                yStart = random.Next(-7, 8);
                zStart = random.Next(-7, 8);

                double angle = random.NextDouble() * 2 * Math.PI;
                EShipList[i].direction.Z = 1;

                EShipList[i].position = new Vector3(xStart, yStart, zStart);
                EShipList[i].direction.X = 0;
                EShipList[i].direction.Z = EShipList[i].direction.Z;
                EShipList[i].direction.Y = 0;
                EShipList[i].speed = GameConstants.EShipsMinSpeed + (float)random.NextDouble() * GameConstants.EShipsMaxSpeed;
                EShipList[i].isActive = true;

            }
        }

        //code to get the points of the model to draw and applys the camera effects to them
        private Matrix[] SetupEffectTransformDefaults(Model myModel)
        {
            Matrix[] absoluteTransforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(absoluteTransforms);

            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.Projection = MainCamera.projectionMatrix;
                    effect.View = MainCamera.camViewMatrix;
                }
            }
            return absoluteTransforms;
        }

        //code to set up the model to be drawn
        public void DrawModel(Model model, Matrix modelTransform, Matrix[] absoluteBoneTransforms)
        {

            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in model.Meshes)
            {
                //This is where the mesh orientation is set
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = absoluteBoneTransforms[mesh.ParentBone.Index] * modelTransform;
                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }

        //sets up sprite batches so that text can be written to the screen
        private void writeText(string msg, Vector2 msgPos, Color msgColour)
        {
            spriteBatch.Begin();
            string output = msg;
            // Find the center of the string
            Vector2 FontOrigin = fontToUse.MeasureString(output) / 2;
            Vector2 FontPos = msgPos;
            // Draw the string
            spriteBatch.DrawString(fontToUse, output, FontPos, msgColour);
            spriteBatch.End();
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = false;
            Window.Title = "Gp3Coursework";
            hitCount = 0;

            //initilizes the positions of the models
            PositionModels();

            //initilizes the positions of the cameras
            camera1 = new camera(new Vector3(0.0f, 3.0f, 300.0f));
            camera1.InitilizeTransform(graphics);

            camera2 = new camera(new Vector3(0.0f, 3.0f, 300.0f));
            camera2.InitilizeTransform(graphics);

            //sets the main camera
            MainCamera = camera1;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
            //-------------------------------------------------------------
            // added to load font
            //-------------------------------------------------------------
            fontToUse = Content.Load<SpriteFont>(".\\Fonts\\QuartzMS");
            //-------------------------------------------------------------
            // added to load Song
            //-------------------------------------------------------------
            bkgMusic = Content.Load<Song>(".\\Audio\\Dr_Nol_-_02_-_Space_Travel_In_The_1960s");
            MediaPlayer.Play(bkgMusic);
            MediaPlayer.IsRepeating = true;
            songInfo = "Song: " + bkgMusic.Name + " Song Duration: " + bkgMusic.Duration.Minutes + ":" + bkgMusic.Duration.Seconds;
            //-------------------------------------------------------------
            // added to load Models
            //-------------------------------------------------------------
            mdlShip = Content.Load<Model>(".\\Models\\Feisar_Ship");
            mdlShipTransforms = SetupEffectTransformDefaults(mdlShip);

            mdlAsteroid = Content.Load<Model>(".\\Models\\2012DA14");
            mdlAsteroidTransforms = SetupEffectTransformDefaults(mdlAsteroid);

            mdlEShip = Content.Load<Model>(".\\Models\\wasphunter");
            mdlEShipTransforms = SetupEffectTransformDefaults(mdlEShip);

            mdlStar = Content.Load<Model>(".\\Models\\Star");
            mdlStarTransforms = SetupEffectTransformDefaults(mdlStar);

            mdlLaser = Content.Load<Model>(".\\Models\\laser");
            mdlLaserTransforms = SetupEffectTransformDefaults(mdlLaser);

            //-------------------------------------------------------------
            // added to load SoundFX's
            //-------------------------------------------------------------
            explosionSound = Content.Load<SoundEffect>("Audio\\200465__wubitog__explosion-longer");
            firingSound = Content.Load<SoundEffect>("Audio\\191594__fins__laser");
        }



        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // calls the movemodelmethod 
            MoveModel();

            //code to call the camupdate when game is paused
            if (gameState == "Paused")
            {
                //MainCamera.RotateCamera(mdlPosition);
                MainCamera.camUpdate(mdlPosition);
            }

            //code for game updates
            if (gameState == "Game")
            {
                //end the gaem if health goes below 0
                if (health <= 0)
                {
                    gameState = "End";
                }

                //updates the game time
                float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

                //calls the update moethd on models other than the player
                for (int i = 0; i < GameConstants.NumAsteroids; i++)
                {
                    AsteroidList[i].Update(timeDelta);
                }

                for (int i = 0; i < GameConstants.NumStars; i++)
                {
                    if (StarList[i].isActive)
                    {
                        StarList[i].Update(timeDelta);
                    }
                }

                for (int i = 0; i < GameConstants.NumEShips; i++)
                {
                    if (EShipList[i].isActive)
                    {
                        EShipList[i].Update(timeDelta);
                    }
                }

                for (int i = 0; i < GameConstants.NumLasers; i++)
                {
                    if (laserList[i].isActive)
                    {
                        laserList[i].Update(timeDelta);
                    }
                }

                //sets up the players bounding sphere for collisions
                BoundingSphere ShipBox = new BoundingSphere(mdlPosition, mdlShip.Meshes[0].BoundingSphere.Radius * GameConstants.ShipScaler);

                //Check for collisions on asteroids
                for (int i = 0; i < AsteroidList.Length; i++)
                {
                    if (AsteroidList[i].isActive)
                    {   
                        //sets up asteroids bounding sphere
                        BoundingSphere AsteroidSphereA = new BoundingSphere(AsteroidList[i].position, mdlAsteroid.Meshes[0].BoundingSphere.Radius * GameConstants.AsteroidBoundingSphereScale);

                        //if they collide another asteroid move forward slightly in a effort so they dont move inside each other
                        if (AsteroidSphereA.Intersects(AsteroidSphereA))
                        {
                            AsteroidList[i].position = new Vector3(AsteroidList[i].position.X, AsteroidList[i].position.Y, AsteroidList[i].position.Z + 1);
                        }

                        //if they hit the player play a noise if sound if on,reduce player health and reset their position
                        if (AsteroidSphereA.Intersects(ShipBox)) //Check collision between Asteroid and Tardis
                        {
                            if (IsSoundOn == true)
                            {
                                explosionSound.Play();
                            }
                            health = health - 10;
                            AsteroidList[i].reset();
                        }
                        
                        //if they hit a laser disable it so it can be fired again
                        for (int k = 0; k < laserList.Length; k++)
                        {
                            if (laserList[k].isActive)
                            {
                                BoundingSphere laserSphere = new BoundingSphere(
                                  laserList[k].position, mdlLaser.Meshes[0].BoundingSphere.Radius *
                                         GameConstants.LaserBoundingSphereScale);

                                if (AsteroidSphereA.Intersects(laserSphere))
                                {
                                    laserList[k].isActive = false;
                                    hitCount++;
                                    break; //no need to check other bullets
                                }
                            }
                        }

                    }
                }

                //collision for enemy ship
                for (int i = 0; i < EShipList.Length; i++)
                {
                    //sets up enemy ship bounding sphere
                    BoundingSphere EShipBoundingSphere = new BoundingSphere(EShipList[i].position, mdlEShip.Meshes[0].BoundingSphere.Radius * GameConstants.EShipBoundingSphereScale);

                    //if it hit the player play sound if sound is on,reduce players health by 20 and reset position
                    if (EShipBoundingSphere.Intersects(ShipBox))
                    {
                        if (IsSoundOn == true)
                        {
                            explosionSound.Play();
                        }
                        health = health - 20;
                        AsteroidList[i].reset();
                    }

                    //if it hits a laser disable laser so it can be fired again, increase score and reset position
                    for (int k = 0; k < laserList.Length; k++)
                    {
                        if (laserList[k].isActive)
                        {
                            //sets up lasers bounding spheres
                            BoundingSphere laserSphere = new BoundingSphere(
                              laserList[k].position, mdlLaser.Meshes[0].BoundingSphere.Radius *
                                     GameConstants.LaserBoundingSphereScale);

                            if (EShipBoundingSphere.Intersects(laserSphere))
                            {
                                if (IsSoundOn == true)
                                {
                                    explosionSound.Play();
                                }
                                score = score + 10;
                                EShipList[i].reset();
                                laserList[k].isActive = false;
                                hitCount++;
                                break; //no need to check other bullets
                            }
                        }
                    }
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //sets up the screen
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            //what to draw if the game is in the main menu
            if (gameState == "MainMenu")
            {
                //write the menu and instructions on screen
                writeText("Asteroid Dodge\nAvoid Asteroids and Enemy Ships\nShoot Enemy Ships to gain points\nChoose your Control type\n[1]Mouse Control (mouse to move, left click to shoot\n[2]Keyboard Control (arrow keys to move, Space to shoot)\n[3]Xbox control (DPad to move, A to shoot, Start to pause, Back to quit) \n escape to Quit \n R to Restart \n M to Mute \n P to Pause", new Vector2(50, 50), Color.Blue);
            }

            //what to draw when the game is over
            if (gameState == "End")
            {
                //display the final score and exit instructions
                string gameovertext = "Game Over Your Score was:";
                writeText(gameovertext +score.ToString(), new Vector2(50,50), Color.Green);
                writeText("Press Escape or Back to exit", new Vector2(50,200), Color.Green);
            }

            //draw game scene and hoe to unpause when the game is stopped
            if (gameState == "Paused")
            {

                for (int i = 0; i < GameConstants.NumEShips; i++)
                {
                    if (EShipList[i].isActive)
                    {
                        Matrix EShipTransform = Matrix.CreateScale(GameConstants.EShipScalar) * Matrix.CreateTranslation(EShipList[i].position);
                        DrawModel(mdlEShip, EShipTransform, mdlEShipTransforms);
                    }
                }
                for (int i = 0; i < GameConstants.NumLasers; i++)
                {
                    if (laserList[i].isActive)
                    {
                        Matrix laserTransform = Matrix.CreateScale(GameConstants.LaserScalar) * Matrix.CreateTranslation(laserList[i].position);
                        DrawModel(mdlLaser, laserTransform, mdlLaserTransforms);
                    }
                }

                for (int i = 0; i < GameConstants.NumStars; i++)
                {
                    if (StarList[i].isActive)
                    {
                        Matrix StarTransform = Matrix.CreateScale(GameConstants.StarScalar) * Matrix.CreateTranslation(StarList[i].position);
                        DrawModel(mdlStar, StarTransform, mdlStarTransforms);
                    }
                }


                for (int i = 0; i < GameConstants.NumAsteroids; i++)
                {
                    if (AsteroidList[i].isActive)
                    {
                        Matrix AsteroidTransform = Matrix.CreateScale(GameConstants.AsteroidScalar) * Matrix.CreateTranslation(AsteroidList[i].position);
                        DrawModel(mdlAsteroid, AsteroidTransform, mdlAsteroidTransforms);
                    }
                }

                Matrix modelTransform = Matrix.CreateScale(GameConstants.ShipScaler) * Matrix.CreateRotationY(mdlRotation) * Matrix.CreateTranslation(mdlPosition);
                DrawModel(mdlShip, modelTransform, mdlShipTransforms);

                writeText("Press O to resume Game", new Vector2(GraphicsDevice.Viewport.Width / 2.5f, GraphicsDevice.Viewport.Height / 2), Color.Blue);
            }

            //display the main game scene when the game is running
            if (gameState == "Game")
            {
            
                //sets up and draws enemy ships
                for (int i = 0; i < GameConstants.NumEShips; i++)
                {
                    if (EShipList[i].isActive)
                    {
                        Matrix EShipTransform = Matrix.CreateScale(GameConstants.EShipScalar) * Matrix.CreateRotationY(mdlRotation) * Matrix.CreateTranslation(EShipList[i].position);
                        DrawModel(mdlEShip, EShipTransform, mdlEShipTransforms);
                    }
                }
                //sets up and draws lasers
                for (int i = 0; i < GameConstants.NumLasers; i++)
                {
                    if (laserList[i].isActive)
                    {
                        Matrix laserTransform = Matrix.CreateScale(GameConstants.LaserScalar) * Matrix.CreateTranslation(laserList[i].position);
                        DrawModel(mdlLaser, laserTransform, mdlLaserTransforms);
                    }
                }
                //sets up and draws stars
                for (int i = 0; i < GameConstants.NumStars; i++)
                {
                    if (StarList[i].isActive)
                    {
                        Matrix StarTransform = Matrix.CreateScale(GameConstants.StarScalar) * Matrix.CreateTranslation(StarList[i].position);
                        DrawModel(mdlStar, StarTransform, mdlStarTransforms);
                    }
                }

                //sets up and draws asteroids
                for (int i = 0; i < GameConstants.NumAsteroids; i++)
                {
                    if (AsteroidList[i].isActive)
                    {
                        Matrix AsteroidTransform = Matrix.CreateScale(GameConstants.AsteroidScalar) * Matrix.CreateTranslation(AsteroidList[i].position);
                        DrawModel(mdlAsteroid, AsteroidTransform, mdlAsteroidTransforms);
                    }
                }

                //sets up and draws player ship
                Matrix modelTransform = Matrix.CreateScale(GameConstants.ShipScaler) * Matrix.CreateRotationY(mdlRotation) * Matrix.CreateTranslation(mdlPosition);
                DrawModel(mdlShip, modelTransform, mdlShipTransforms);

                //writes the health and score onscreen
                string healthtext = "Health=";
                writeText(healthtext+health.ToString(), new Vector2(50, 10), Color.Yellow);
                string scoretext = "Score=";
                writeText(scoretext+score.ToString(), new Vector2(50, 40), Color.Yellow);
            }

           

            base.Draw(gameTime);
        }
    }
}
