using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleGame
{
    public class Ball
    {
        private Texture2D texture;
        private float rotation = 0;
        private bool mouseWasPressed = false;
        private float radius;
        public float health = 10;
        public Vector2 Position { get; private set; }
        public Vector2 Velocity { get; private set; }
        private Vector2 gravity = new Vector2(0, 500.0f);
        private Game1 thisGame;

        public Ball(Vector2 position, Vector2 velocity, Game1 game)
        {
            Position = position;
            Velocity = velocity;
            thisGame = game;
        }
        private int GetTileIndex(int x, int y, BasicTilemap t)
        {
            if (x >= 0 && x < t.MapWidth && y >= 0 && y < t.MapHeight) // Check bounds
            {
                return t.TileIndices[y * t.MapWidth + x];
            }
            return -1; // Return -1 or some other value to indicate an invalid position
        }
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("pball");
            radius = texture.Width / 2;
        }
        private void SetTileAt(int x, int y, int tileIndex, BasicTilemap t)
        {
            if (x >= 0 && x < t.MapWidth && y >= 0 && y < t.MapHeight) // Check bounds
            {
                t.TileIndices[y * t.MapWidth + x] = tileIndex;
            }
        }
        
        public void Initialize()
        {
            // Initialization code here
        }
        private Point ScreenToTilePosition(Vector2 screenPosition, BasicTilemap t)
        {
            int tileX = (int)(screenPosition.X / t.TileWidth);
            int tileY = (int)(screenPosition.Y / t.TileHeight);

            return new Point(tileX, tileY);
        }
        public void Update(GameTime gameTime, BasicTilemap tilemap, MouseState mouseState)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Apply gravity to the velocity
            Velocity += gravity * deltaTime;

            // Update the position based on the current velocity
            Position += Velocity * deltaTime;


            // Optional: Implement collision detection and response here
            Point tilePosition = ScreenToTilePosition(Position, tilemap);
            int tileType = GetTileIndex(tilePosition.X, tilePosition.Y, tilemap);
            if (Position.X < 0 || Position.X > 800)
            {
                this.Velocity = new Vector2(-this.Velocity.X, this.Velocity.Y);
            }
            if (Position.Y < 0 || Position.Y > 500)
            {
                this.Velocity = new Vector2(this.Velocity.X, -this.Velocity.Y);
            }
            if (tileType != 0 &&  tileType != -1)
            {
                // Calculate the center of the tile
                Vector2 tileCenter = new Vector2(tilePosition.X * 32 + 32 / 2,
                                                 tilePosition.Y * 32 + 32 / 2);

                // Vector from the tile center to the ball center
                Vector2 collisionVector = Position - tileCenter;

                // Normalize this vector
                collisionVector.Normalize();

                // Reflect the ball's velocity around this vector
                // Assuming Velocity is a Vector2
                if (!thisGame.specialState)
                {
                    this.Velocity = Vector2.Reflect(this.Velocity, collisionVector);
                }
                // Handle tile removal or other logic
                if (thisGame.specialState)
                {
                    health = health - .5f;
                }
                else
                {
                    health = health - 1;
                }
                
                thisGame.ballCollision(Position);
            }
        }
        private static bool IsColliding(Ball ball, Rectangle square)
        {
            // Find the closest point to the circle within the rectangle
            float closestX = MathHelper.Clamp(ball.Position.X, square.Left, square.Right);
            float closestY = MathHelper.Clamp(ball.Position.Y, square.Top, square.Bottom);

            // Calculate the distance between the circle's center and this closest point
            float distanceX = ball.Position.X - closestX;
            float distanceY = ball.Position.Y - closestY;

            // If the distance is less than the circle's radius, an intersection occurs
            float distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);
            return distanceSquared < (ball.radius * ball.radius);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color color = new Color((int)health*25, (int)health *25, (int)health *25, (int)health *25);
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Rectangle rect = new Rectangle(0, 0, texture.Width, texture.Height);
            if (thisGame.specialState)
            {
                spriteBatch.Draw(texture, Position, rect, new Color(255,215,0,(int)health*25), rotation, origin, 1.0f, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(texture, Position, rect, color, rotation, origin, 1.0f, SpriteEffects.None, 0);
            }
        }
        public void moveTop()
        {
            Random r = new Random();
            Position = new Vector2(Position.X, r.Next(20));
            Velocity = new Vector2(r.Next(-500,500), r.Next(-500, 500));
        }
    }
}
