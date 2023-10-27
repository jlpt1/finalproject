using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Text;

namespace ExampleGame
{
    /// <summary>
    /// This game demonstrates the use of a Tilemap loaded through 
    /// the content pipeline with a custom importer and processor
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private BasicTilemap _tilemap;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            File.WriteAllText("../Fuck.txt", "You");
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _tilemap = Content.Load<BasicTilemap>("example");
            string currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string projectRoot = Directory.GetParent(Directory.GetParent(currentDirectory).FullName).FullName;
            string contentDirectory = Path.Combine(projectRoot, "Content");

            try
            {
                // Modify the tilemap content
                string fakepath = Path.Combine(Content.RootDirectory, "example.tmap");
                string tilemapPath = Path.Combine(contentDirectory, "example.tmap");
                if (File.Exists(fakepath))
                {
                    string content = File.ReadAllText(tilemapPath, Encoding.UTF8);
                    content = content.Replace("4", "3");
                    File.WriteAllText(tilemapPath, content, Encoding.UTF8);
                }

                // Load the modified tilemap
                _tilemap = Content.Load<BasicTilemap>("example");
            }
            catch (Exception ex)
            {
                // Log the error to a file
                File.AppendAllText("log.txt", $"Error during tilemap processing: {ex.Message}\n");
            }
            _tilemap = Content.Load<BasicTilemap>("example");
        }
        


 

        

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Drawing the tilemap
            _spriteBatch.Begin();
            _tilemap.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}