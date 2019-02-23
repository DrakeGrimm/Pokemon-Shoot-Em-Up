using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;

namespace ShootShapesUp
{
    public class GameRoot : Game
    {
        // some helpful static properties
        public static GameRoot Instance { get; private set; }
        public static Viewport Viewport { get { return Instance.GraphicsDevice.Viewport; } }
        public static Vector2 ScreenSize { get { return new Vector2(Viewport.Width, Viewport.Height); } }
        public static GameTime GameTime { get; private set; }

        public static Texture2D Player { get; private set; }
        public static Texture2D Seeker { get; private set; }
        public static Texture2D Bullet { get; private set; }
        public static Texture2D Pointer { get; private set; }
        //public static Texture2D Boss { get; private set; }
        public static Texture2D Seeker2 { get; private set; }
        public static Texture2D Seeker3 { get; private set; }

        public static SpriteFont Font { get; private set; }

        public static Song Music { get; private set; }

        private static readonly Random rand = new Random();

        private static SoundEffect[] explosions;
        // return a random explosion sound
        public static SoundEffect Explosion { get { return explosions[rand.Next(explosions.Length)]; } }

        private static SoundEffect[] shots;
        public static SoundEffect Shot { get { return shots[rand.Next(shots.Length)]; } }

        private static SoundEffect[] spawns;
        public static SoundEffect Spawn { get { return spawns[rand.Next(spawns.Length)]; } }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        MouseState mouseState;
        MouseState previousMouseState;

        private Vector2 startButtonPosition;
        private Vector2 exitButtonPosition;

        private Texture2D startButton;
        private Texture2D exitButton;

        //Game Complete
        private Texture2D gameComplete;
        private Vector2 gameCompletePosition;

        //Score
        private Vector2 scorePosition;
        private SpriteFont scoreFont;
        public static int currentScore = EntityManager.Score;


        //Final Stage
        private Texture2D stage2Title;
        private Vector2 stage2TitlePosition;
        
        //Background
        private Texture2D background;
        private Vector2 backgroundPosition;
        Rectangle rectangle1;
        Rectangle rectangle2;

        //Main Menu title screen
        Texture2D titleTheme;
        Vector2 titlePosition;
        Texture2D pokeballTitle;
        Vector2 pokeballPosition;

        //All Stages
        public enum GameState
        {
            MainMenu,
            LevelOne,
            LevelTwo,
            LevelThree,
            GameFinish,
            GameOver
        }

        public static GameState gameState;

        public GameRoot()
        {
            Instance = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = @"Content";

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.IsFullScreen = false;
            //GraphicsDeviceManager.HardwareModeSwitch(true, false);
            //Window.IsBorderless = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            EntityManager.Add(PlayerShip.Instance);

            MediaPlayer.IsRepeating = true;

            MediaPlayer.Play(GameRoot.Music);

            //Initialise Main Menu buttons
            startButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2) - 50, 950);
            exitButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2) - 50, 1000);

            //Score
            scorePosition = new Vector2(1700, 100);

            IsMouseVisible = true;

            //Initialise Completion pictures
            gameCompletePosition = new Vector2(600, 250);
            stage2TitlePosition = new Vector2(600, 250);

            //Initialise Background position and dimensions
            backgroundPosition = new Vector2(50, 100);

            rectangle1 = new Rectangle(0, 0, 1920, 1080);
            rectangle2 = new Rectangle(1920, 0, 1920, 1080);

            //Initalise Main Menu Title images
            titlePosition = new Vector2(600, 250);
            pokeballPosition = new Vector2(405, 250);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            //Load Background and title themes
            titleTheme = this.Content.Load<Texture2D>("Art/Title2");
            background = this.Content.Load<Texture2D>("Art/Background");
            pokeballTitle = this.Content.Load<Texture2D>("Art/Pokeball Title");

            //Load entity images
            Player = Content.Load<Texture2D>("Art/Pokeball");
            Seeker = Content.Load<Texture2D>("Art/Lugia");
            Bullet = Content.Load<Texture2D>("Art/Bullet");
            Pointer = Content.Load<Texture2D>("Art/Pointer");
            Seeker2 = Content.Load<Texture2D>("Art/Articuno");
            Seeker3 = Content.Load<Texture2D>("Art/Kyogre");

            //Stage2
            stage2Title = Content.Load<Texture2D>("Art/Boss Incoming");

            //Game Complete
            gameComplete = Content.Load<Texture2D>("Art/Game Complete");

            Font = Content.Load<SpriteFont>("Font");

            Music = Content.Load<Song>("Sound/Pokémon");


            //These linq expressions are just a fancy way loading all sounds of each category into an array.
            //explosions = Enumerable.Range(1, 8).Select(x => Content.Load<SoundEffect>("Sound/explosion-0" + x)).ToArray();
            //shots = Enumerable.Range(1, 4).Select(x => Content.Load<SoundEffect>("Sound/shoot-0" + x)).ToArray();
            shots = Enumerable.Range(1, 1).Select(x => Content.Load<SoundEffect>("Sound/Return")).ToArray();
            //spawns = Enumerable.Range(1, 8).Select(x => Content.Load<SoundEffect>("Sound/spawn-0" + x)).ToArray();
            

            //Main Menu buttons
            startButton = Content.Load<Texture2D>("Buttons/start");
            exitButton = Content.Load<Texture2D>("Buttons/exit");

            //Score
            scoreFont = Content.Load<SpriteFont>("Score");
        }

        protected override void Update(GameTime gameTime)
        {
            GameTime = gameTime;
            Input.Update();

            // Allows the game to exit
            if (Input.WasButtonPressed(Buttons.Back) || Input.WasKeyPressed(Keys.Escape))
                this.Exit();

            LoadContent();
            EntityManager.Update();
            EnemySpawner.Update();

            //Background scrolling
            if (rectangle1.X + background.Width <= 0)
                rectangle1.X = rectangle2.X + background.Width;
            if (rectangle2.X + background.Width <= 0)
                rectangle2.X = rectangle1.X + background.Width;

            if (gameState == GameState.MainMenu)
            {
                rectangle1.X -= 5;
                rectangle2.X -= 5;
            }

            if (gameState == GameState.LevelOne)
            {
                rectangle1.X -= 5;
                rectangle2.X -= 5;
            }

            if (gameState == GameState.LevelTwo)
            {
                rectangle1.X -= 10;
                rectangle2.X -= 10;
            }
            
            // Mouse State with Buttons
            mouseState = Mouse.GetState();
            if (previousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
            {
                MouseClicked(mouseState.X, mouseState.Y);
            }
            previousMouseState = mouseState;

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);

            //Draw scrolling backgrounds
            spriteBatch.Draw(background, rectangle1, Color.White);
            spriteBatch.Draw(background, rectangle2, Color.White);

            spriteBatch.End();

            //Draw Stage 1
            if (gameState == GameState.LevelOne)
            {
                // Draw entities. Sort by texture for better batching.
                spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
                EntityManager.Draw(spriteBatch);

                spriteBatch.End();

                // Draw user interface
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

                spriteBatch.DrawString(scoreFont, "Score: " + EntityManager.Score.ToString(), scorePosition, Color.White);

                spriteBatch.End();
            }

            //Draw Stage 2
            if (gameState == GameState.LevelTwo)
            {
                //Draw entities. Sort by texture for better batching.
                spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
                EntityManager.Draw(spriteBatch);
                spriteBatch.End();

                // Draw user interface
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

                spriteBatch.DrawString(scoreFont, "Score: " + EntityManager.Score.ToString(), scorePosition, Color.White);

                spriteBatch.End();
            }

            //Draw Final Stage
            if (gameState == GameState.LevelThree)
            {
                //Draw entities. Sort by texture for better batching.
                spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
                EntityManager.Draw(spriteBatch);
                spriteBatch.End();


                // Draw user interface
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

                spriteBatch.DrawString(scoreFont, "Score: " + EntityManager.Score.ToString(), scorePosition, Color.White);

                spriteBatch.End();

                if(EntityManager.Score > 20000)
                {
                    gameState = GameState.GameFinish;
                }
            }
            
            //Change Stage to Main Menu
            if (gameState == GameState.MainMenu)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(startButton, startButtonPosition, Color.White);
                spriteBatch.Draw(exitButton, exitButtonPosition, Color.Red);
                spriteBatch.Draw(titleTheme, titlePosition, Color.White);
                spriteBatch.Draw(pokeballTitle, pokeballPosition, Color.White);
                spriteBatch.End();
                EntityManager.Score = 0;
            }

            //Change Stage to Finish Screen
            if (gameState == GameState.GameFinish)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(startButton, startButtonPosition, Color.White);
                spriteBatch.Draw(exitButton, exitButtonPosition, Color.Red);
                spriteBatch.Draw(gameComplete, gameCompletePosition, Color.White);
                spriteBatch.Draw(pokeballTitle, pokeballPosition, Color.White);
                spriteBatch.End();
            }

            //Change state to Stage 2
            if (EntityManager.Score >= 2000 && EntityManager.Score <= 2400)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(Font, "STAGE COMPLETED! Get ready for STAGE 2", new Vector2(900, 250), Color.White);
                spriteBatch.End();

                gameState = GameState.LevelTwo;
            }

            //Draw Final Stage
            if (EntityManager.Score >= 4000 && EntityManager.Score <= 5000)
            {
                spriteBatch.Begin();
                if (EntityManager.Score >= 4000 && EntityManager.Score <= 4400)
                {
                    spriteBatch.DrawString(Font, "STAGE COMPLETED! Get Ready for the FINAL STAGE", new Vector2(300, 250), Color.White);
                    if (EntityManager.Score % 300 == 0)
                        spriteBatch.Draw(stage2Title, stage2TitlePosition, Color.White);
                }
                spriteBatch.End();

                gameState = GameState.LevelThree;
            }

            base.Draw(gameTime);
        }

        private void DrawRightAlignedString(string text, float y)
        {
            var textWidth = GameRoot.Font.MeasureString(text).X;
            spriteBatch.DrawString(GameRoot.Font, text, new Vector2(ScreenSize.X - textWidth - 5, y), Color.White);
        }

        void MouseClicked(int x, int y)
        {
            Rectangle mouseClickRect = new Rectangle(x, y, 10, 10);

            if (gameState == GameState.MainMenu || gameState == GameState.GameFinish)
            {
                Rectangle startButtonRect = new Rectangle((int)startButtonPosition.X, (int)startButtonPosition.Y, 100, 20);
                Rectangle exitButtonRect = new Rectangle((int)exitButtonPosition.X, (int)exitButtonPosition.Y, 100, 20);

                if (mouseClickRect.Intersects(startButtonRect))
                {
                    gameState = GameState.LevelOne;
                    LoadContent();
                }
                else if (mouseClickRect.Intersects(exitButtonRect))
                {
                    Exit();
                }
            }
        }

        void ResetScore()
        {
            EntityManager.Score = 0;
        }
    }
}
