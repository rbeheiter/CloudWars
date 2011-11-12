using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using CloudWars.Units;

namespace CloudWars
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public const int GRID_TILE_SIZE = 50;
        public const int GRID_HEIGHT = 8;
        public const int GRID_WIDTH = 15;
        public const int INPUT_LOCKOUT_MILLISECONDS = 100;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D cursorTexture;
        Texture2D grassTileTexture;
        SpriteFont nameFont;
        SpriteFont titleFont;

        Point cursorLocation;
        int inputLockoutCounter = 0;
        bool preventInput = false;

        List<Player> Players;
        Player CurrentPlayer;
        Unit CurrentSelectedUnit;

        List<Unit> AllUnits
        {
            get
            {
                List<Unit> list = new List<Unit>();

                foreach (Player player in Players)
                {
                    list.AddRange(player.Units);
                }

                return list;
            }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            cursorLocation = new Point(0, 0);

            Players = new List<Player>();

            Player player1 = new Player();
            player1.Units.Add(new Infantry("Bob", 2, 3));
            player1.Units.Add(new Infantry("Joe", 2, 4) { CurrentHP = 85 });
            player1.Units.Add(new Soldier("Lu", 2, 5) { CurrentHP = 50 });

            Player player2 = new Player();
            player2.Units.Add(new Infantry("Bob", 6, 3));
            player2.Units.Add(new Infantry("Joe", 6, 4) { CurrentHP = 85 });
            player2.Units.Add(new Soldier("Lu", 6, 5) { CurrentHP = 50 });

            Players.Add(player1);
            Players.Add(player2);

            CurrentPlayer = player1;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            cursorTexture = Content.Load<Texture2D>("selection");
            nameFont = Content.Load<SpriteFont>("NameFont");
            titleFont = Content.Load<SpriteFont>("TitleFont");
            grassTileTexture = Content.Load<Texture2D>("grass");

            Unit.LoadTextures(Content);
            Infantry.LoadTextures(Content);
            Soldier.LoadTextures(Content);
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Y)
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (preventInput)
            {
                inputLockoutCounter += gameTime.ElapsedGameTime.Milliseconds;

                if (inputLockoutCounter >= INPUT_LOCKOUT_MILLISECONDS)
                {
                    inputLockoutCounter = 0;
                    preventInput = false;
                }
            }
            else
            {
                GetCursorInput();
            }

            UpdateUnits(gameTime);

            base.Update(gameTime);
        }

        private void UpdateUnits(GameTime gameTime)
        {
            foreach (Player player in Players)
                foreach (Unit unit in player.Units)
                {
                    unit.Update(gameTime);
                }
        }

        private void GetCursorInput()
        {
            int stickX = (int)(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X * 2);
            int stickY = (int)(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y * 2);

            if ((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadUp)
                || Keyboard.GetState().IsKeyDown(Keys.Up)
                || stickY > 0) && cursorLocation.Y > 0)
            {
                cursorLocation.Y--;
                preventInput = true;
            }
            if ((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadDown)
                || Keyboard.GetState().IsKeyDown(Keys.Down)
                || stickY < 0) && cursorLocation.Y < GRID_HEIGHT)
            {
                cursorLocation.Y++;
                preventInput = true;
            }
            if ((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadRight)
                || Keyboard.GetState().IsKeyDown(Keys.Right)
                || stickX > 0) && cursorLocation.X < GRID_WIDTH)
            {
                cursorLocation.X++;
                preventInput = true;
            }
            if ((GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft)
                || Keyboard.GetState().IsKeyDown(Keys.Left)
                || stickX < 0) && cursorLocation.X > 0)
            {
                cursorLocation.X--;
                preventInput = true;
            }
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A)
                || Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                SelectUnit();
            }
        }

        private void SelectUnit()
        {
            Unit unit = CurrentPlayer.Units.Where(u => cursorLocation == u.GridLocation).FirstOrDefault();

            if (unit != null)
            {
                if (CurrentSelectedUnit != null)
                {
                    CurrentSelectedUnit.IsSelected = false;
                }

                unit.IsSelected = true;
                CurrentSelectedUnit = unit;
            }
            else
            {
                //Move unit to position
                MoveUnit();
            }
        }

        private void MoveUnit()
        {
            if (CurrentSelectedUnit == null)
            {
                // select a unit first
                return;
            }


            if (cursorLocation.DistanceTo(CurrentSelectedUnit.GridLocation) <= CurrentSelectedUnit.MaxMove)
            {
                CurrentSelectedUnit.GridLocation = cursorLocation;
            }
            else
            {
                //can't move!
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Beige);

            spriteBatch.Begin();

            DrawTiles();
            DrawUnits();
            DrawCursor();
            DrawHoverUnitDescription();
            DrawCurrentUnitDescription();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawTiles()
        {
            for (int x = 0; x <= GRID_WIDTH; x++)
            {
                for (int y = 0; y <= GRID_HEIGHT; y++)
                {
                    var tilePos = new Rectangle(GRID_TILE_SIZE * x,
                                                GRID_TILE_SIZE * y,
                                                GRID_TILE_SIZE,
                                                GRID_TILE_SIZE);

                    spriteBatch.Draw(grassTileTexture, tilePos, Color.White);

                }
            }
        }

        private void DrawHoverUnitDescription()
        {
            Unit unit = AllUnits.Where(u => cursorLocation == u.GridLocation).FirstOrDefault();

            if (unit != null)
            {
                spriteBatch.DrawString(nameFont, unit.Name, new Vector2(5, 450), Color.Black);
                spriteBatch.DrawString(titleFont, unit.Title, new Vector2(5, 465), Color.Black);
                spriteBatch.DrawString(nameFont, unit.HitPoints, new Vector2(45, 455), Color.DarkRed);

                spriteBatch.DrawString(titleFont, string.Format("Attack: {0}", unit.AttackPower), new Vector2(110, 455), Color.Black);
                spriteBatch.DrawString(titleFont, string.Format("Move:   {0}", unit.MaxMove), new Vector2(110, 465), Color.Black);
            }
        }

        private void DrawCurrentUnitDescription()
        {
            if (CurrentSelectedUnit != null)
            {
                //For Debug
                spriteBatch.DrawString(nameFont, string.Format("[Distance: {0}]", cursorLocation.DistanceTo(CurrentSelectedUnit.GridLocation)), new Vector2(450, 455), Color.DarkBlue);

                spriteBatch.DrawString(nameFont, "Selected:", new Vector2(575, 455), Color.DarkBlue);
                spriteBatch.DrawString(nameFont, CurrentSelectedUnit.Name, new Vector2(655, 450), Color.DarkBlue);
                spriteBatch.DrawString(titleFont, CurrentSelectedUnit.Title, new Vector2(655, 465), Color.DarkBlue);
                spriteBatch.DrawString(nameFont, CurrentSelectedUnit.HitPoints, new Vector2(700, 455), Color.DarkRed);
            }
        }

        private void DrawCursor()
        {
            var cursorPos = new Rectangle(GRID_TILE_SIZE * cursorLocation.X,
                                          GRID_TILE_SIZE * cursorLocation.Y,
                                          GRID_TILE_SIZE,
                                          GRID_TILE_SIZE);

            spriteBatch.Draw(cursorTexture, cursorPos, Color.White);
        }

        private void DrawUnits()
        {
            foreach (Unit unit in AllUnits)
            {
                unit.Draw(spriteBatch);
            }
        }
    }
}
