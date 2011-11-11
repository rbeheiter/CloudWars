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

        List<Unit> Units;
        Unit CurrentSelectedUnit;

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
            Units = new List<Unit>();
            Units.Add(new Infantry("Bob", 2, 3));
            Units.Add(new Infantry("Joe", 2, 4) { CurrentHP = 85 });
            Units.Add(new Soldier("Lu", 2, 5) { CurrentHP = 50 });
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
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Y))
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
            foreach (Unit unit in Units)
            {
                unit.Update(gameTime);
            }
        }

        private void GetCursorInput()
        {

            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadUp) && cursorLocation.Y > 0)
            {
                cursorLocation.Y--;
                preventInput = true;
            }
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadDown) && cursorLocation.Y < GRID_HEIGHT)
            {
                cursorLocation.Y++;
                preventInput = true;
            }
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadRight) && cursorLocation.X < GRID_WIDTH)
            {
                cursorLocation.X++;
                preventInput = true;
            }
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft) && cursorLocation.X > 0)
            {
                cursorLocation.X--;
                preventInput = true;
            }
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A))
            {
                SelectUnit();
            }
        }

        private void SelectUnit()
        {
            Unit unit = Units.Where(u => cursorLocation == u.GridLocation).FirstOrDefault();

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
            Unit unit = Units.Where(u => cursorLocation == u.GridLocation).FirstOrDefault();

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
            foreach (Unit unit in Units)
            {
                unit.Draw(spriteBatch);
            }
        }
    }
}
