using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExampleGame
{
    public class Particle
    {
        private Texture2D sprite;
        private List<Vector2> positions;
        private List<Vector2> velocities;
        private List<float> rotations; // To store rotation angles
        private Rectangle sourceRectangle;
        private float scale; // To specify the scale of the particles
        private float transparency;
        public Particle(Texture2D sprite, Vector2 initialPosition, int numberOfParticles, Rectangle sourceRectangle, float scale)
        {
            this.sprite = sprite;
            this.sourceRectangle = sourceRectangle;
            this.scale = scale;
            positions = new List<Vector2>();
            velocities = new List<Vector2>();
            rotations = new List<float>();
            transparency = 1.0f;

            for (int i = 0; i < numberOfParticles; i++)
            {
                Random r = new Random();
                Vector2 rPos = new Vector2(initialPosition.X+r.Next(0,32), initialPosition.Y + r.Next(0, 32));
                positions.Add(rPos);
                velocities.Add(new Vector2((float)(-1.5 + (new Random().NextDouble()*3)), (float)(-1.5+(new Random().NextDouble()*3))));
                rotations.Add((float)(-Math.PI + (new Random().NextDouble() * 2 * Math.PI))); // Random rotation angle
            }
        }

        public void Update()
        {
            for (int i = 0; i < positions.Count; i++)
            {
                positions[i] += velocities[i];
                velocities[i] += new Vector2(0, 0.08f); // Add gravity here or adjust as needed
            }
            transparency -= .03f;
            transparency = Math.Max(0, transparency);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < positions.Count; i++)
            {

                Vector2 origin = new Vector2(sourceRectangle.Width / 2, sourceRectangle.Height / 2); // Center of the particle

                Color color = HexToColor("#CCCCCC") * transparency;
                spriteBatch.Draw(sprite, positions[i], sourceRectangle, color, rotations[i], origin, scale, SpriteEffects.None, 0);
            }
        }
        private Color HexToColor(string hex)
        {
            hex = hex.TrimStart('#');
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color(r, g, b);
        }
    }
}
