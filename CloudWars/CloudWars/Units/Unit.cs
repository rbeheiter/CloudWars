using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using CloudWars.Animation;

namespace CloudWars
{
    public abstract class Unit
    {
        float MOVE_SPEED = 3;

        public abstract string Title { get; }
        public abstract float MaxHP { get; }
        public abstract int MaxMove { get; }
        public abstract int AttackPower { get; }
        public abstract Texture2D UnitTexture { get; }
        public abstract Texture2D SeletedUnitTexture { get; }
        public abstract SpriteAnimation MoveAnimation { get; }

        public string Name { get; set; }
        public float CurrentHP { get; set; }
        public Point GridLocation { get; set; }
        public bool IsSelected { get; set; }
        public Point RealLocation { get; set; }

        protected static Texture2D HpTexture;

        public string HitPoints
        {
            get
            {
                return string.Format("{0}/{1}", CurrentHP, MaxHP);
            }
        }

        public Unit(string name, int x, int y)
        {
            Name = name;
            GridLocation = new Point(x, y);
            IsSelected = false;
            CurrentHP = MaxHP;
            RealLocation = new Point(Game1.GRID_TILE_SIZE * GridLocation.X, Game1.GRID_TILE_SIZE * GridLocation.Y);
        }

        public static void LoadTextures(ContentManager contentManager)
        {
            HpTexture = contentManager.Load<Texture2D>("hpbar");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var unitPos = new Rectangle(RealLocation.X,
                                        RealLocation.Y,
                                        Game1.GRID_TILE_SIZE,
                                        Game1.GRID_TILE_SIZE);

            Point endLocation = new Point(Game1.GRID_TILE_SIZE * GridLocation.X, Game1.GRID_TILE_SIZE * GridLocation.Y);

            if (RealLocation != endLocation)
            {
                MoveAnimation.Position = new Vector2(RealLocation.X, RealLocation.Y);
                MoveAnimation.Draw(spriteBatch, RealLocation.X > endLocation.X ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            }
            else
            {
                spriteBatch.Draw(IsSelected ? SeletedUnitTexture : UnitTexture, unitPos, Color.White);
            }
            
            DrawHP(spriteBatch);
        }

        private void DrawHP(SpriteBatch spriteBatch)
        {
            var maxHpPos = new Rectangle(RealLocation.X + 8,
                                      RealLocation.Y + 4,
                                      HpTexture.Width,
                                      HpTexture.Height);

            var curhpPos = new Rectangle(RealLocation.X + 8,
                                      RealLocation.Y + 4,
                                      (int)(HpTexture.Width * CurrentHP / MaxHP),
                                      HpTexture.Height);

            spriteBatch.Draw(HpTexture, maxHpPos, Color.Black);
            spriteBatch.Draw(HpTexture, curhpPos, Color.White);
        }

        public void Update(GameTime gameTime)
        {
            Point endLocation = new Point(Game1.GRID_TILE_SIZE * GridLocation.X, Game1.GRID_TILE_SIZE * GridLocation.Y);

            if (RealLocation != endLocation)
            {
                MoveAnimation.Update(gameTime);

                int distance = endLocation.DistanceTo(RealLocation);

                if (distance >= MOVE_SPEED)
                {
                    int dX = (int)(MOVE_SPEED * (endLocation.X - RealLocation.X)) / distance;
                    int dY = (int)(MOVE_SPEED * (endLocation.Y - RealLocation.Y)) / distance;

                    RealLocation = new Point(RealLocation.X + dX, RealLocation.Y + dY);
                }
                else
                {
                    RealLocation = endLocation;
                }
            }
        }
    }
}
