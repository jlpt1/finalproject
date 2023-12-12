using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using SharpDX.XAudio2;

namespace ExampleGame
{
    public class Cannon
    {
        private Texture2D texture;
        private float rotation = 0;
        private Vector2 cannonPosition = new Vector2(400, 0);

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("cannon");

        }
        public void Initalize()
        {
           
        }
        public void Update(GameTime gameTime, MouseState mouseState)
        {
            // Get the cannon's position
          // Assuming this is the cannon's position

            // Get the current mouse position
            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);

            // Calculate the direction vector from the cannon to the mouse
            Vector2 direction = mousePosition - cannonPosition;

            // Calculate the rotation angle in radians
            // Math.Atan2 returns the angle between the positive x-axis and the direction vector
            rotation = (float)Math.Atan2(direction.Y, direction.X) - (float)Math.PI / 2;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
           
            Vector2 rect = new Vector2(150, 150);
            Rectangle origin = new Rectangle(0, 0, 300, 300);
                spriteBatch.Draw(texture, cannonPosition, origin, Color.White, rotation, rect, .5f, SpriteEffects.None, 0);
            
        }
       
    }
}
