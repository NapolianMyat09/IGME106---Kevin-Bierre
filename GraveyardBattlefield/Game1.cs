using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;
using System.Timers;
using System.IO;

namespace GraveyardBattlefield
{
    /*
     * Project: Graveyard BattleField
     * Names: Tracy Chun, Jason Wang, Napolian Myat
     * Class: Game
     * Purpose: handles monogame's update and draws
     * 
     * Updates:
     * 
     */
    public enum GameState
    {
        Menu,
        Wave1,
        Wave2,
        Wave3,
        FinalWave,
        GameOver,
    }
    public enum BulletState
    {
        HaveBullet,
        DontHaveBullet,
        TotallyDontHaveBullet
    }
    public class Game1 : Game
    {
        //FIELDS
        //SPRITEBATCH
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private BulletState bulletState = BulletState.HaveBullet;
        private GameState gameState = GameState.Menu;

        //ASSET THAT MOVES AND RELATED VARIABLES
        private Player player;
        private int playerScore;
        private int highestRecord;
        private List<Enemy> zombies = new List<Enemy>();
        //ammo assets
        private List<bullet> bullets = new List<bullet>();
        private int playerBullet = 150;
        private int playerBackupBullet = 600;
        private double reloadindTime;

        //PREVIOUS KEYBOARD/MOUSE STATES
        private KeyboardState previousKBState;
        private MouseState previousMState;

        //RANDOM
        private Random randGen = new Random();

        //SCREEN
        private static int screenWidth; //screen width
        private static int screenHeight; //screen height

        //Number of Waves
        private int wave = 1;
        private int doOnce = 0;
        private int countDown;


        //TEXTURE2D
        //For Menu
        private Texture2D menuScreen;
        private Texture2D startButton;
        private Rectangle startButtonRect;//StartButton Rect
        private Texture2D exitButton;
        private Rectangle exitButtonRect;

        //For Gameplay
        private Texture2D zombieAsset;
        private Texture2D playerAsset;
        private Texture2D bulletTexture;

        //For Waves
        private Texture2D waveOneBackGround;
        private Texture2D waveTwoBackGround;
        private Texture2D woodSign;

        //For GameFinished
        //Victory
        private Texture2D gameVictoryAsset;
        private int gameVictoryWidth;
        private int gameVictoryHeight;
        private bool playerIsVictor;
        private Texture2D zombieVictoryAsset;
        private Texture2D victorySymbol;
        private Texture2D victoryBackground;
        //Defeat
        private Texture2D defeatScreen;




        //FONTS
        private SpriteFont font;
        private SpriteFont titleFont;
        private SpriteFont bigTitleFont;

        //PROPERTIES
        public static int Width
        {
            get { return screenWidth; }
        }
        public static int Height
        {
            get { return screenHeight; }
        }

        //CONSTRUCTOR
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        //METHODS
        protected override void Initialize()
        {

            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 1500;
            _graphics.PreferredBackBufferHeight = 950;
            screenWidth = _graphics.PreferredBackBufferWidth;
            screenHeight = _graphics.PreferredBackBufferHeight;

            //playerVictoryAsset
            gameVictoryWidth = screenWidth / 2;
            gameVictoryHeight = screenHeight;
            playerIsVictor = false;
            reloadindTime = 3.5;
            ReadHighestScore();
            playerScore = 0;

            _graphics.ApplyChanges(); //apply screen change
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //load the textures and rectangle and intialize the player object
            //GAMEPLAY
            zombieAsset = this.Content.Load<Texture2D>("zombieKid");
            playerAsset = this.Content.Load<Texture2D>("playerSpriteSheet");
            bulletTexture = this.Content.Load<Texture2D>("bullet");
            player = new Player(new Vector2(300, 300), playerAsset);

            //MENU
            menuScreen = this.Content.Load<Texture2D>("mainMenuScreen");
            startButton = this.Content.Load<Texture2D>("StartButton");
            startButtonRect = new Rectangle((screenWidth - 350) / 2, 700, 350, 150);
            exitButton = this.Content.Load<Texture2D>("ExitButton");
            exitButtonRect = new Rectangle(screenWidth - 115, 20, 85, 85);

            //WAVES
            waveOneBackGround = this.Content.Load<Texture2D>("graveyard");
            waveTwoBackGround = this.Content.Load<Texture2D>("snowfield");
            woodSign = this.Content.Load<Texture2D>("woodSign");

            //GAMEFINISH SCREEN
            gameVictoryAsset = this.Content.Load<Texture2D>("SoldierVictory");
            zombieVictoryAsset = this.Content.Load<Texture2D>("zombieVictory");
            victorySymbol = this.Content.Load<Texture2D>("VictorySymbol");
            victoryBackground = this.Content.Load<Texture2D>("VictoryBackground");
            defeatScreen = this.Content.Load<Texture2D>("DefeatImage");


            //gameOverAsset = this.Content.Load<Texture2D>("");

            //LOAD FONT
            font = Content.Load<SpriteFont>("Font");
            titleFont = Content.Load<SpriteFont>("TitleFont");
            bigTitleFont = Content.Load<SpriteFont>("BigTitleFont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //check current keyboard state then update
            KeyboardState kbstate = Keyboard.GetState();
            MouseState mState = Mouse.GetState(); //get mouseState
            switch (gameState)
            {
                case GameState.Menu:
                    {
                        zombies.Clear(); //Reset zombies
                        ResetGame(); //Reset previous progress

                        //for keyboard press enter, will start game, OLD CODE IN REGION
                        #region
                        //if (Process.SingleKeyPress(kbstate, Keys.Enter))
                        //{
                        //    gameState = Stage.Wave1;
                        //}
                        #endregion

                        if (mState.LeftButton == ButtonState.Released && previousMState.LeftButton == ButtonState.Pressed)
                        //if mouse was clicked
                        {
                            if (exitButtonRect.Contains(mState.Position))//and if mouse was clicking on exitButton
                            {
                                zombies.Clear(); //reset zombie count
                                Exit();
                            }
                        }
                        if (Process.MouseClick(mState, startButtonRect))
                        {
                            gameState = GameState.Wave1; //progresses to wave1
                        }
                        previousMState = mState; //update previous mouse state

                        break;
                    }
                case GameState.Wave1:
                    {
                        WaveSpawn(gameTime, 20); //Spawn zombie wave
                        //Want to have a countdown to display victory message and give play time to celebrate before moving to next stage

                        if (zombies.Count <= 0) //when player defeat zombie wave....
                        {
                            gameState = GameState.Wave2; //transition to next stage
                            if (gameState == GameState.Wave2)
                            {
                                ResetCountDown(5); //reset countDown timer
                                zombies.Clear(); //just in case, clear zombie list
                                doOnce = 0; //reset doOnce so that we spawn zombie only once each wave
                                wave = 2; //change current wave to 2 for display
                            }
                        }
                        //check player's bullet and reload if needed
                        if (bulletState == BulletState.DontHaveBullet)
                        {
                            reloadindTime -= gameTime.ElapsedGameTime.TotalSeconds;
                        }
                        if (reloadindTime < 0)
                        {
                            Reload();
                        }
                        break;
                    }
                case GameState.Wave2:
                    {
                        WaveSpawn(gameTime, 40); //Spawn zombie wave
                        //Want to have a countdown to display victory message and give play time to celebrate before moving to next stage

                        if (zombies.Count <= 0) //when player defeat zombie wave....
                        {
                            gameState = GameState.Wave3; //transition to next stage
                            if (gameState == GameState.Wave3)
                            {
                                ResetCountDown(5); //reset countDown timer
                                zombies.Clear(); //just in case, clear zombie list
                                doOnce = 0; //reset doOnce so that we spawn zombie only once each wave
                                wave = 3; //change current wave to 3 for display
                            }
                        }
                        //check player's bullet and reload if needed
                        if (bulletState == BulletState.DontHaveBullet)
                        {
                            reloadindTime -= gameTime.ElapsedGameTime.TotalSeconds;
                        }
                        if (reloadindTime < 0)
                        {
                            Reload();
                        }
                        break;
                    }
                case GameState.Wave3:
                    {
                        WaveSpawn(gameTime, 60); //Spawn zombie wave
                        //Want to have a countdown to display victory message and give play time to celebrate before moving to next stage

                        if (zombies.Count <= 0) //when player defeat zombie wave....
                        {
                            gameState = GameState.FinalWave; //transition to next stage
                            if (gameState == GameState.FinalWave)
                            {
                                ResetCountDown(5); //reset countDown timer
                                zombies.Clear(); //just in case, clear zombie list
                                doOnce = 0; //reset doOnce so that we spawn zombie only once each wave
                                wave = 4; //change current wave to 4 for display
                            }
                        }
                        //check player's bullet and reload if needed
                        if (bulletState == BulletState.DontHaveBullet)
                        {
                            reloadindTime -= gameTime.ElapsedGameTime.TotalSeconds;
                        }
                        if (reloadindTime < 0)
                        {
                            Reload();
                        }
                        break;
                    }
                case GameState.FinalWave:
                    {
                        WaveSpawn(gameTime, 200); //Spawn zombie wave
                                                  //Show VICTORY MESSAGE

                        //Press space to loop back to main menu
                        if (Process.SingleKeyPress(kbstate, Keys.Space))
                        {
                            gameState = GameState.Menu;
                        }
                        //check player's bullet and reload if needed
                        if (bulletState == BulletState.DontHaveBullet)
                        {
                            reloadindTime -= gameTime.ElapsedGameTime.TotalSeconds;
                        }
                        if (reloadindTime < 0)
                        {
                            Reload();
                        }
                        break;
                    }
                case GameState.GameOver:
                    {
                        if (playerScore > highestRecord)
                        {
                            UpdateHihestScore();
                        }
                        if (Process.SingleKeyPress(kbstate, Keys.Space))
                        {
                            ResetGame();
                            gameState = GameState.Menu;
                            wave = 1;
                        }
                        break;
                    }
            }
            Process.PreviousKbState = kbstate;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            switch (gameState)
            {
                case GameState.Menu:
                    {
                        //the main menu
                        _spriteBatch.Draw(menuScreen, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);

                        //IF We want to press enter to play game, old code in this region
                        #region
                        //In case we dont want click, but pressing enter
                        //_spriteBatch.DrawString(Font, $"Press 'Enter' to start the game", new Vector2(300, 650), Color.White);
                        #endregion

                        //Will have startButton start the game
                        //Start Button
                        _spriteBatch.Draw(startButton, startButtonRect, Color.DarkRed);

                        //Exit Button
                        _spriteBatch.Draw(exitButton, exitButtonRect, Color.White);
                        break;
                    }
                case GameState.Wave1:
                    {
                        DrawWave(waveOneBackGround, Color.White); //draw wave1Assets
                        break;
                    }
                case GameState.Wave2:
                    {
                        DrawWave(waveTwoBackGround, Color.Purple); //draw wave2Assets
                        break;
                    }
                case GameState.Wave3:
                    {
                        DrawWave(waveTwoBackGround, Color.Brown); //draw wave3Assets
                        break;
                    }
                case GameState.FinalWave:
                    {
                        DrawWave(waveTwoBackGround, Color.DarkBlue); //draw finalWaveAssets
                        if (zombies.Count <= 0)
                        {
                            //Victory Screen
                            _spriteBatch.Draw(victoryBackground, new Rectangle(0, 0, screenWidth, screenHeight), Color.White); //Background
                            _spriteBatch.Draw(gameVictoryAsset, new Rectangle(screenWidth - gameVictoryWidth, 1, gameVictoryWidth, gameVictoryHeight), Color.White); //Soldier
                            _spriteBatch.Draw(zombieVictoryAsset, new Rectangle(1, 1, gameVictoryWidth, gameVictoryHeight), Color.White); //Zombie
                            _spriteBatch.Draw(victorySymbol, new Rectangle((screenWidth - gameVictoryWidth) / 2, (screenHeight - gameVictoryHeight) / 2, gameVictoryWidth, gameVictoryHeight), Color.White); //Victory Declaration
                            _spriteBatch.DrawString(titleFont, "Press 'Space' to return to Main Menu", new Vector2(50, 750), Color.White); //Instruction to go back to main menu
                            _spriteBatch.DrawString(titleFont, "Score: " + playerScore, new Vector2(550, 100), Color.White);
                        }
                        break;
                    }
                case GameState.GameOver: //if player dies, transition to this stage
                    {
                        //Defeat Screen
                        _spriteBatch.Draw(defeatScreen, new Rectangle(0, 0, screenWidth, screenHeight), Color.White); //background
                        _spriteBatch.DrawString(bigTitleFont, "You Have Died", new Vector2((screenWidth - 900) / 2, screenHeight / 2 - 100), Color.DeepPink); //You have died announcement
                        _spriteBatch.DrawString(titleFont, "Press 'Space' to return to Main Menu", new Vector2(50, 750), Color.DeepPink); //Instruction to go back to main menu
                        _spriteBatch.DrawString(titleFont, "Score: " + playerScore, new Vector2(550, 100), Color.White);
                        break;
                    }
            }

            //draw reload notification when player run out of bullets
            if (bulletState == BulletState.DontHaveBullet && gameState != GameState.Menu && gameState != GameState.GameOver)
            {
                _spriteBatch.DrawString(font, $"Reloading! {string.Format("{0:0.00}", reloadindTime)}seconds before reload is done!", new Vector2(400, (screenHeight / 2)), Color.White);
            }
            else if (bulletState == BulletState.TotallyDontHaveBullet && gameState != GameState.Menu && gameState != GameState.GameOver)
            {
                _spriteBatch.DrawString(font, $"You are doomed! No bullets left for you, wait till you death!", new Vector2(400, (screenHeight / 2)), Color.White);
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void NextWave(int numOfZombiesInWave)
        {
            for (int i = 0; i < numOfZombiesInWave; i++)
            {
                int randNum = randGen.Next(0, 4);
                if (randNum == 0)
                    zombies.Add(new Enemy(new Rectangle(0, randGen.Next(0, screenHeight), 30, 30), zombieAsset));
                else if (randNum == 1)
                    zombies.Add(new Enemy(new Rectangle(screenWidth, randGen.Next(0, screenHeight), 30, 30), zombieAsset));
                else if (randNum == 2)
                    zombies.Add(new Enemy(new Rectangle(randGen.Next(0, screenWidth), 0, 30, 30), zombieAsset));
                else if (randNum == 3)
                    zombies.Add(new Enemy(new Rectangle(randGen.Next(0, screenWidth), screenHeight, 30, 30), zombieAsset));
            }
        }

        //need to convert 
        private void AddBullet()
        {
            KeyboardState kbstate = Keyboard.GetState();

            //while player did not run out of bullets
            if (playerBullet > 0)
            {
                //Upward
                if (kbstate.IsKeyDown(Keys.Up))
                {
                    //Upward Left
                    if (kbstate.IsKeyDown(Keys.Left) && kbstate.IsKeyDown(Keys.Up))
                    {
                        bullets.Add(new bullet(screenWidth, screenHeight, new Rectangle(Convert.ToInt32(player.Position.X), Convert.ToInt32(player.Position.Y), bulletTexture.Width, bulletTexture.Height), bulletTexture, "upleft"));
                        playerBullet--;
                    }
                    //Upward Right
                    else if (kbstate.IsKeyDown(Keys.Right) && kbstate.IsKeyDown(Keys.Up))
                    {
                        bullets.Add(new bullet(screenWidth, screenHeight, new Rectangle(Convert.ToInt32(player.Position.X), Convert.ToInt32(player.Position.Y), bulletTexture.Width, bulletTexture.Height), bulletTexture, "upright"));
                        playerBullet--;
                    }
                    else //just upward
                    {
                        bullets.Add(new bullet(screenWidth, screenHeight, new Rectangle(Convert.ToInt32(player.Position.X), Convert.ToInt32(player.Position.Y), bulletTexture.Width, bulletTexture.Height), bulletTexture, "up"));
                        playerBullet--;
                    }
                }
                //Downward
                else if (kbstate.IsKeyDown(Keys.Down))
                {
                    //Downward Left
                    if (kbstate.IsKeyDown(Keys.Down) && kbstate.IsKeyDown(Keys.Left))
                    {
                        bullets.Add(new bullet(screenWidth, screenHeight, new Rectangle(Convert.ToInt32(player.Position.X), Convert.ToInt32(player.Position.Y), bulletTexture.Width, bulletTexture.Height), bulletTexture, "downleft"));
                        playerBullet--;
                    }
                    //Downward Right
                    else if (kbstate.IsKeyDown(Keys.Down) && kbstate.IsKeyDown(Keys.Right))
                    {
                        bullets.Add(new bullet(screenWidth, screenHeight, new Rectangle(Convert.ToInt32(player.Position.X), Convert.ToInt32(player.Position.Y), bulletTexture.Width, bulletTexture.Height), bulletTexture, "downright"));
                        playerBullet--;
                    }
                    else //just downward
                    {
                        bullets.Add(new bullet(screenWidth, screenHeight, new Rectangle(Convert.ToInt32(player.Position.X), Convert.ToInt32(player.Position.Y), bulletTexture.Width, bulletTexture.Height), bulletTexture, "down"));
                        playerBullet--;
                    }
                }
                //Right
                else if (kbstate.IsKeyDown(Keys.Right))
                {
                    bullets.Add(new bullet(screenWidth, screenHeight, new Rectangle(Convert.ToInt32(player.Position.X), Convert.ToInt32(player.Position.Y), bulletTexture.Width, bulletTexture.Height), bulletTexture, "right"));
                    playerBullet--;
                }
                //Left
                else if (kbstate.IsKeyDown(Keys.Left))
                {
                    bullets.Add(new bullet(screenWidth, screenHeight, new Rectangle(Convert.ToInt32(player.Position.X), Convert.ToInt32(player.Position.Y), bulletTexture.Width, bulletTexture.Height), bulletTexture, "left"));
                    playerBullet--;
                }
            }
            if (playerBullet == 0&& playerBackupBullet != 0)
            {
                bulletState = BulletState.DontHaveBullet;
            }
            else if (playerBullet == 0 && playerBackupBullet == 0)
            {
                bulletState = BulletState.TotallyDontHaveBullet;
            }
        }
        public void Reload()
        {
            if (playerBackupBullet >= 150)
            {
                if (playerBullet == 0)
                {
                    playerBullet = 150;
                    playerBackupBullet -= 150;
                }
                bulletState = BulletState.HaveBullet;
                reloadindTime = 3.5;
            }
            else if (playerBackupBullet == 0)
            {
                bulletState = BulletState.TotallyDontHaveBullet;
            }
        }
        //reset game method
        public void ResetGame()
        {
            player.Health = 100;
            playerBullet = 150;
            playerBackupBullet = 600;
            ResetCountDown(5);
            doOnce = 0;
            zombies.Clear();
            playerScore = 0;
        }

        /// <summary>
        /// Reset CountDown
        /// </summary>
        public void ResetCountDown(int timeInSec)
        {
            countDown = timeInSec * 60; //5 s multiplied by 60 miliseconds gives us 5 seconds in milliseconds
        }

        /// <summary>
        /// Populate Wave with zombies
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="numOfZombieInWave"></param>
        public void WaveSpawn(GameTime gameTime, int numOfZombieInWave)
        {
            //Decrease increment in countdown
            countDown--;
            KeyboardState kbstate = Keyboard.GetState();

            //Player can still move even if countdown is not 0
            player.Update(gameTime, kbstate);

            if ((zombies == null || zombies.Count == 0) && doOnce != 1) //if zombies do not exist, or is null, we will spawn zombies only once
            {
                NextWave(numOfZombieInWave); //spawn zombie
                doOnce++; //increment doOnce so we only do this once
            }

            //can still shoot even if countdown does not reach 0
            AddBullet();

            //For bullets shot hitting zombie
            foreach (bullet Bullet in bullets)
            {
                Bullet.shootBullet();
                for (int j = 0; j < zombies.Count; j++)
                {
                    if (zombies[j].Position.Contains(Bullet.Position.X, Bullet.Position.Y))
                    //plus 15 is to make sure bullet hit zombie in middle
                    //if zombie position is inside bullet, will take dmg(Suppose to be vice-versa but it runs so its a hassle to change it)
                    {
                        zombies[j].TakeDamage();
                        Bullet.Hitted = true;
                    }
                }
            }
            for (int i = 0; i < bullets.Count; i++)
            {
                if (bullets[i].Hitted)
                {
                    bullets.Remove(bullets[i]);
                }
            }
            if (countDown <= 0) //implement a countdown, will load zombies when countdown reaches 0
            {
                //OLD CODE FOR ZOMBIES UPDATE
                #region
                //foreach (Enemy zombie in zombies)
                //{
                //    if (zombie.IsAlive == false)
                //    {
                //        zombies.Remove(zombie); //remove him, hes dead
                //    }
                //    else
                //    {
                //        zombie.Update(gameTime, player);
                //    }
                //}
                #endregion

                for (int i = 0; i < zombies.Count; i++)
                {
                    if (zombies[i].IsAlive == false)
                    {
                        zombies.Remove(zombies[i]);
                        playerScore += 10;
                    }
                    else
                    {
                        zombies[i].Update(gameTime, player);
                    }
                }
            }

            if (player.Health <= 0) //if player health reaches 0 or less
            {
                player.Health = 0; //we want to display health = 0 instead of negatives for whatever reason
                gameState = GameState.GameOver; //display gamestate gameover
            }
            //loop through the game
        }

        /// <summary>
        /// Pre-Wave introduction
        /// Shows the controls for the game and a timer before the game starts
        /// </summary>
        /// <param name="background"></param>
        /// <param name="color"></param>
        public void DrawWave(Texture2D background, Color color)
        {
            //Before we start the game, we want to have a countdown to get players time to be ready
            countDown--;

            //DONT DRAW ANYTHING BEFORE BACKGROUND OTHERWISE IT WONT SHOW
            //DrawBackGround
            _spriteBatch.Draw(background, new Rectangle(0, 0, screenWidth, screenHeight), color);

            //DrawPLayer&Enemy Asset
            player.Draw(_spriteBatch);
            foreach (Enemy zombies in zombies)
            {
                zombies.Draw(_spriteBatch);
            }

            foreach (bullet bullets in bullets)
            {
                bullets.Draw(_spriteBatch);
            }

            //Draw STATS Last because of order, we want it to show above zombie/player
            if (playerIsVictor == false)
            {
                _spriteBatch.DrawString(font, $"Player remaining health: {player.Health}\n" + //Health
                    $"Ammo: {playerBullet}/{playerBackupBullet}\n" +
                    $"Score: {playerScore}\n" +
                    $"Highest: {highestRecord}", new Vector2(10, 10), Color.White); //Ammos

                switch (wave)
                {
                    case 1:
                    case 2:
                    case 3:
                        _spriteBatch.DrawString(font, $"Wave#: {wave}", new Vector2(screenWidth - 200, 10), Color.White);
                        break;
                    case 4:
                        _spriteBatch.DrawString(font, $"Wave#: Final Wave", new Vector2(screenWidth - 400, 10), Color.White);
                        break;
                    default:
                        break; //oops
                }
            }
            if (countDown > 0 && wave == 1) //draw countdown
            {
                _spriteBatch.Draw(woodSign, new Vector2(100, (screenHeight / 3)+20), Color.White);
                _spriteBatch.DrawString(font,
                    "Controls:\n" +
                    "\nW - Up          Up Arrow Key         - Shoot Upward" +
                    "\nA - Left          Left Arrow Key       - Shoot Left" +
                    "\nS - Down       Down Arrow Key    - Shoot Downward" +
                    "\nD - Right        Right Arrow Key    - Shoot Right"
                    , new Vector2(350, (screenHeight / 2) - 80), Color.Black);

                _spriteBatch.DrawString(font, $"{countDown / 60} seconds before zombies break in!"
                    , new Vector2(400, (screenHeight/2) - 200), Color.White);//num of seconds remaining
            }
            else if (countDown > 0)
            {
                _spriteBatch.DrawString(font, $"{countDown / 60} seconds before next zombie wave break in!"
                    , new Vector2(400, (screenHeight / 2) - 200), Color.White);//num of seconds remaining
            }
        }

        public void ReadHighestScore()
        {
            //text file created 
            String filename = "..\\..\\..\\Score.txt";

            // Create the variable outside the try
            StreamReader input = null;
            try
            {
                // Creating the stream reader opens the file
                input = new StreamReader(filename);
                highestRecord = Convert.ToInt16(input.ReadLine());
                input.Close();
            }
            catch
            {
                return;
            }
        }

        public void UpdateHihestScore()
        {
            //delete the old file and replace it with new
            File.Delete("..\\..\\..\\Score.txt");
            String newfile = "..\\..\\..\\Score.txt";
            StreamWriter output = new StreamWriter(newfile);

            //loop through to write data
            output.WriteLine($"{playerScore}");
            highestRecord = playerScore;
            output.Close();
        }
    }
}