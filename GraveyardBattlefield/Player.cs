using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraveyardBattlefield
{
    enum Walking
    {
        Left,
        Right,
        Up,
        Down
    }
    public class Player : GameObject
    {
        //FIELDS
        private int frame;
        private double timeCounter;
        private double timePerFrame;

        // CONSTANT FOR SPRITESHEETS
        const int WalkFrameCount = 5;
        const int VerticalPlayerOffsetY = 10;
        const int HorizontalPlayerOffsetY = 80;
        const int PlayerHeight = 60;     // The height of a single frame
        const int PlayerWidth = 64;      // The width of a single frame

        Walking walkingState;

        //PLAYER FIELDS
        private int health;

        //PROPERTIES
        public int Health
        {
            get { return health; }
            set
            {
                health = value;
            }
        }

        //CONSTRUCTOR
        public Player(Vector2 position, Texture2D asset)
            : base(position, asset)
        {
            frame = 0;
            timeCounter = 0;
            timePerFrame = 0.1;
            Health = 100;
        }

        //PLAYER MOVEMENT
        public void Movement(KeyboardState currentKbState)
        {
            if (currentKbState.IsKeyDown(Keys.W)) //W for up
            {
                position.Y -= 1.5f;
                if (position.Y < 0)
                {
                    position.Y = 0;
                }
                walkingState = Walking.Up;

            }
            if (currentKbState.IsKeyDown(Keys.A)) //A for left
            {
                position.X -= 1.5f;
                if (position.X < 0)
                {
                    position.X = 0;
                }
                walkingState = Walking.Left;

            }
            if (currentKbState.IsKeyDown(Keys.S)) //S for down
            {
                position.Y += 1.5f;
                if (position.Y > Game1.Height - PlayerHeight)
                {
                    position.Y = Game1.Height - PlayerHeight;
                }
                walkingState = Walking.Down;

            }
            if (currentKbState.IsKeyDown(Keys.D)) //D for right
            {
                position.X += 1.5f;
                if (position.X > Game1.Width - PlayerWidth)
                {
                    position.X = Game1.Width - PlayerWidth;
                }
                walkingState = Walking.Right;
            }
        }

        //Update
        public override void Update(GameTime gametime, KeyboardState currentKbState)
        {
            Movement(currentKbState);
            UpdateAnimation(gametime);
        }

        //Draw according to movement
        public override void Draw(SpriteBatch sb)
        {
            switch (walkingState)
            {
                case Walking.Up:
                    DrawWalkingVerticalUp(SpriteEffects.None, sb);
                    break;
                case Walking.Left:
                    DrawWalkingHorizontal(SpriteEffects.None, sb);
                    break;
                case Walking.Down:
                    DrawWalkingVertical(SpriteEffects.None, sb);
                    break;
                case Walking.Right:
                    DrawWalkingHorizontal(SpriteEffects.FlipHorizontally, sb);
                    break;
            }
        }

        //MOVEMENT METHODS
        //Horizontal - sprite will just be left as is or flipped horizontally respectively for left and right
        private void DrawWalkingHorizontal(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                Texture,                         // - The texture to draw
                Position,                               // - Where to draw it
                new Rectangle(                          // - The rectangle to draw
                    frame * PlayerWidth,
                    HorizontalPlayerOffsetY,
                    PlayerWidth,
                    PlayerHeight),
                Color.White,                            // - No color
                0,                                      // - No Rotation
                Vector2.Zero,                           // - Start counting in the second row
                1.0f,                                   // - no scale change
                flipSprite, 0);                         // - flip if necessary
        }

        //Verticle downward
        private void DrawWalkingVertical(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                Texture,                         // - The texture to draw
                Position,                               // - Where to draw it
                new Rectangle(                          // - The rectangle to draw
                    frame * PlayerWidth,
                    VerticalPlayerOffsetY,
                    PlayerWidth,
                    PlayerHeight),
                Color.White,                            // - No color
                0,                                      // - No Rotation
                Vector2.Zero,                           // - Start counting in the second row
                1.0f,                                   // - no scale change
                flipSprite,                             // - flip if necessary
                0);
        }
        
        //Verticle upward - Since we cant just rotate this b/c that would look wrong, the color is changed to black so that it looks like player is facing upward
        private void DrawWalkingVerticalUp(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                Texture,                         // - The texture to draw
                Position,                               // - Where to draw it
                new Rectangle(                          // - The rectangle to draw
                    frame * PlayerWidth,
                    VerticalPlayerOffsetY,
                    PlayerWidth,
                    PlayerHeight),
                Color.Black,                            // - Black
                0,                                      // - No Rotation
                Vector2.Zero,                           // - Start counting in the second row
                1.0f,                                   // - no scale change
                flipSprite,                             // - flip if necessary
                0);
        }

        //UPDATE FOR ANIMATION
        public void UpdateAnimation(GameTime gameTime)
        {
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            // If enough time has passed:
            if (timeCounter >= timePerFrame)
            {
                frame += 1;                     // Adjust the frame to the next image

                if (frame > WalkFrameCount)    //double check bounds of frames
                {
                    frame = 0;
                }

                timeCounter -= timePerFrame;
            }
        }
    
        //FOR PLAYER TAKING DMG
        public void TakeDamage()
        {
            Health -= 1; //will decrease count for dmg taken
        }
    }
}
