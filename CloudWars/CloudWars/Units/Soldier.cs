using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using CloudWars.Animation;

namespace CloudWars.Units
{
    public class Soldier : Unit
    {
        public override string Title 
        {
            get
            {
                return "Mech";
            }
        }
        public override float MaxHP 
        { 
            get 
            { 
                return 150; 
            } 
        }
        public override int MaxMove
        {
            get
            {
                return 2;
            }
        }
        public override int AttackPower
        {
            get
            {
                return 30;
            }
        }

        private static Texture2D unitTexture;
        private static Texture2D seletedUnitTexture;
        private static SpriteAnimation moveAnimation;

        public override Texture2D UnitTexture
        {
            get
            {
                return unitTexture;
            }
        }
        public override Texture2D SeletedUnitTexture
        {
            get
            {
                return seletedUnitTexture;
            }
        }
        public override SpriteAnimation MoveAnimation
        {
            get
            {
                return moveAnimation;
            }
        }

        public new static void LoadTextures(ContentManager contentManager)
        {
            unitTexture = contentManager.Load<Texture2D>("demoman");
            seletedUnitTexture = contentManager.Load<Texture2D>("demoman_selected");
            moveAnimation = new SpriteAnimation(contentManager.Load<Texture2D>("infantry_move_sprite"), 6);
            moveAnimation.Position = new Vector2(50, 50);
            moveAnimation.IsLooping = true;
            moveAnimation.FramesPerSecond = 15;
        }

        public Soldier(string name, int x, int y)
            : base(name, x, y)
        {
        }
    }
}
