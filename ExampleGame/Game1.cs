using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;
//using System.Windows.Forms;


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
        private SpriteFont bangers;
        private SpriteFont bangers2;
        private int _selectedBlockIndex = 0;
        private int _essence = 0;
        private int[] costs = { 20, 4, 8, 40 };



        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            

        }

        protected override void Initialize()
        {
            string currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // Navigate up one level to examplegame/
            string projectRoot = Directory.GetParent(currentDirectory).FullName;

            // Now move into content directory

            int levelsToMoveUp = 3;  // Adjust this number as needed
            string path = currentDirectory;

            for (int i = 0; i < levelsToMoveUp; i++)
            {
                path = Directory.GetParent(path)?.FullName;
                if (path == null)
                {
                    // Break out of the loop if there's no parent directory
                    break;
                }
            }

            // Rest of your code...


         
                // Modify the tilemap content
                // Assuming you have your projectRoot pointing to examplegame/
                string contentDirectory = Path.Combine(path, "Content");
                string tilemapPath = Path.Combine(contentDirectory, "example.tmap");

                // Now, tilemapPath should point to examplegame/content/tilemap.tmap


         
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // _tilemap = Content.Load<BasicTilemap>("example");
            // Current directory is examplegame/bin
            bangers = Content.Load<SpriteFont>("bangers");
            bangers2 = Content.Load<SpriteFont>("bangers2");
            _tilemap = Content.Load<BasicTilemap>("example");
        }





        private Point ScreenToTilePosition(Vector2 screenPosition, BasicTilemap t)
        {
            int tileX = (int)(screenPosition.X / t.TileWidth);
            int tileY = (int)(screenPosition.Y / t.TileHeight);

            return new Point(tileX, tileY);
        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            

            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.S))
            {
                SaveCurrentState();
            }


            if (keyboardState.IsKeyDown(Keys.D1)) _selectedBlockIndex = 0;
            if (keyboardState.IsKeyDown(Keys.D2)) _selectedBlockIndex = 1;
            if (keyboardState.IsKeyDown(Keys.D3)) _selectedBlockIndex = 2;
            if (keyboardState.IsKeyDown(Keys.D4)) _selectedBlockIndex = 3;

            MouseState mouseState = Mouse.GetState();
            if (mouseState.RightButton == ButtonState.Pressed)
            {
                Point tilePosition = ScreenToTilePosition(new Vector2(mouseState.X, mouseState.Y), _tilemap);
                int tileType = GetTileIndex(tilePosition.X, tilePosition.Y, _tilemap);
                if (_essence >= costs[_selectedBlockIndex] && tileType != _selectedBlockIndex+1)
                {
                    SetTileAt(tilePosition.X, tilePosition.Y, _selectedBlockIndex + 1, _tilemap);
                    _essence -= costs[_selectedBlockIndex];
                }
                    
                
                
            }
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                Point tilePosition = ScreenToTilePosition(new Vector2(mouseState.X, mouseState.Y), _tilemap);
                int tileType = GetTileIndex(tilePosition.X, tilePosition.Y, _tilemap);
                if (tileType != 0)
                {
                    SetTileAt(tilePosition.X, tilePosition.Y, 0, _tilemap);  // Assuming 0 is air/empty
                    if (tileType == 1)
                    {
                        _essence += 5;
                    }
                    if (tileType == 2)
                    {
                        _essence += 1;
                    }
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {

                Random _random = new Random();
                int terrainHeight = _random.Next(1, 20);
                for (int x = 0; x < _tilemap.MapWidth; x++)
                {
                    for (int y = 0; y < _tilemap.MapHeight; y++)
                    {
                        SetTileAt(x, y, 0, _tilemap); // Assuming 0 is the index for your empty or air texture
                    }
                }
                // Iterate over each column in the tilemap
                for (int x = 0; x < _tilemap.MapWidth; x++)
                {
                    // Generate a random height for the terrain in each column

                    terrainHeight += _random.Next(-2, 3);
                    if (terrainHeight > 13)
                    {
                        terrainHeight = 13;
                    }
                    if (terrainHeight < 3)
                    {
                        terrainHeight = 3;
                    }
                    // Fill tiles from the bottom up to the terrain height with a block
                 
                        SetTileAt(x, terrainHeight, 1, _tilemap); 
                    for (int y = terrainHeight; y < 15; y++)
                    {
                        SetTileAt(x, y+1, 2, _tilemap);
                    }
                   

                    // Fill the rest of the column with air or another tile to represent empty space
                    for (int y = _tilemap.MapHeight - terrainHeight - 1; y >= 0; y--)
                    {
                     //   SetTileAt(x, y, 0, _tilemap); // Assuming 0 is the index for your empty or air texture
                    }
                    
                }
            }

            base.Update(gameTime);
        }

        private void SaveCurrentState()
        {
            StringBuilder sb = new StringBuilder();

            // Add essence to the file
            sb.AppendLine($"Essence: {_essence}");

            // Add tilemap to the file
            for (int y = 0; y < _tilemap.MapHeight; y++)
            {
                for (int x = 0; x < _tilemap.MapWidth; x++)
                {
                    sb.Append(GetTileIndex(x, y, _tilemap));
                    if (x != _tilemap.MapWidth - 1) sb.Append(",");
                }
                sb.AppendLine();
            }

            // Offload to an STA thread
            Thread staThread = new Thread(() =>
            {
                // Prompt user for the filename
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Text Files|*.txt";
                    sfd.Title = "Save Map";
                    sfd.InitialDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(sfd.FileName, sb.ToString());
                    }
                }
            });

            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();
        }



        private void SetTileAt(int x, int y, int tileIndex, BasicTilemap t)
        {
            if (x >= 0 && x < t.MapWidth && y >= 0 && y < t.MapHeight) // Check bounds
            {
                t.TileIndices[y * t.MapWidth + x] = tileIndex;
            }
        }
        private int GetTileIndex(int x, int y, BasicTilemap t)
        {
            if (x >= 0 && x < t.MapWidth && y >= 0 && y < t.MapHeight) // Check bounds
            {
                return t.TileIndices[y * t.MapWidth + x];
            }
            return -1; // Return -1 or some other value to indicate an invalid position
        }

     

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
           
            // Drawing the tilemap
            _spriteBatch.Begin();
            _tilemap.Draw(gameTime, _spriteBatch);
            _tilemap.DrawBlock(gameTime, _spriteBatch, _selectedBlockIndex);
           
            _spriteBatch.End();
            _spriteBatch.Begin();
            _spriteBatch.DrawString(bangers, "Essenece: " + _essence, new Vector2(1, 1), Color.Gold);

            _spriteBatch.DrawString(bangers2, "Right click to place a block, Left click to destroy one", new Vector2(1, 30), Color.Black);
            _spriteBatch.DrawString(bangers2, "You get essence by destroying blocks", new Vector2(1, 45), Color.Black);
            _spriteBatch.DrawString(bangers2, "Press 1-4 to change your block", new Vector2(1, 60), Color.Black);
            _spriteBatch.DrawString(bangers2, "Press R to generate", new Vector2(1, 75), Color.Red);
            _spriteBatch.DrawString(bangers2, "Press S to save", new Vector2(1, 90), Color.Cyan);
            _spriteBatch.DrawString(bangers2, "Press L to load", new Vector2(1, 105), Color.Blue);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}