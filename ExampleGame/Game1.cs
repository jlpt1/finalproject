using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.DirectWrite;
using System;
using System.ComponentModel;
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
        private BasicTilemap _oremap;
        private SpriteFont bangers;
        private SpriteFont bangers2;
        private int _selectedBlockIndex = 0;
        private int _essence = 100;
        private int[] costs = { 20, 4, 8, 40 };
        private bool saved = true;
        private double health = 100;
        private int level = 1;
        private int orecount = 0;
        private int lastGemCollected = 0;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            

        }

        protected override void Initialize()
        {
           

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
            _essence = LoadFromFile();
            _tilemap = Content.Load<BasicTilemap>("example");
            _oremap = Content.Load<BasicTilemap>("example2");
 gen();

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

            health = health - (1 * Math.Sqrt(level)/3);
            if (health < 0)
            {
                health = 0;
            }
          
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.S))
            {
                SaveToFile(_essence.ToString());
                SaveCurrentState();
            }
          
            if (orecount <= 0)
            {
                gen();
                level++;
               
            }

            if (keyboardState.IsKeyDown(Keys.D1)) _selectedBlockIndex = 0;
            if (keyboardState.IsKeyDown(Keys.D2)) _selectedBlockIndex = 1;
            if (keyboardState.IsKeyDown(Keys.D3)) _selectedBlockIndex = 2;
            if (keyboardState.IsKeyDown(Keys.D4)) _selectedBlockIndex = 3;

            MouseState mouseState = Mouse.GetState();
            if (mouseState.RightButton == ButtonState.Pressed)
            {
                saved = false;
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
                if (health > 0)
                {


                    saved = false;
                    Point tilePosition = ScreenToTilePosition(new Vector2(mouseState.X, mouseState.Y), _tilemap);
                    int tileType = GetTileIndex(tilePosition.X, tilePosition.Y, _tilemap);
                    if (tileType != 0)
                    {
                        SetTileAt(tilePosition.X, tilePosition.Y, 0, _tilemap);  // Assuming 0 is air/empty
                        if (tileType == 1)
                        {
                            health += 1;
                        }
                        if (tileType == 2)
                        {
                            health += 1;
                        }
                        if (tileType == 3)
                        {
                            health += 1;

                        }
                        if (tileType == 4)
                        {
                            health += 1;
                        }
                    }
                    int oreType = GetTileIndex(tilePosition.X, tilePosition.Y, _oremap);
                    if (oreType > 0)
                    {
                        SetTileAt(tilePosition.X, tilePosition.Y, 0, _oremap);  // Assuming 0 is air/empty
                        orecount -= 1;
                        if (oreType == 1)
                        {
                            health += 30;
                        }
                        if (oreType == 2)
                        {
                            health += 40;
                        }
                        if (oreType == 3)
                        {
                            health += 5;

                        }
                        if (oreType == 4)
                        {
                            health += 7;
                        }
                    }
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
               if (health <= 0)
                {
                    gen();
                    health = 100;
                    level = 1;
                }
            }

            base.Update(gameTime);
        }
        private void gen()
        {
            saved = false;
            orecount = 0;
        Random _random = new Random();
            int terrainHeight = _random.Next(1, 20);
            for (int x = 0; x < _tilemap.MapWidth; x++)
            {
                for (int y = 0; y < _tilemap.MapHeight; y++)
                {
                    SetTileAt(x, y, 0, _tilemap); // Assuming 0 is the index for your empty or air texture
                    SetTileAt(x, y, 0, _oremap);
                }
            }
            // Iterate over each column in the tilemap
            for (int x = 0; x < 25; x++) //_tilemap.MapWidth
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
                for (int y = terrainHeight; y < terrainHeight + 4; y++)
                {
                    if (y < 14)
                    {
                        SetTileAt(x, y + 1, 2, _tilemap);
                        Random r = new Random();

                        if (r.Next(1, 21) == 5)
                        {

                            int temp = r.Next(3, 5);
      
                            if (temp == 3)
                            {
                                SetTileAt(x, y + 1, temp, _oremap);
                                orecount += 1;
                            }
                            if (temp == 4)
                            {
                                SetTileAt(x, y + 1, temp, _oremap);
                                orecount += 1;
                            }
                        }

                    }
                }
                for (int y = terrainHeight+4; y < 14; y++)
                {
                     SetTileAt(x, y + 1, 3, _tilemap);
                    Random r = new Random();

                    if (r.Next(1, 11) < 2)
                    {

                        int temp = r.Next(1, 11);
                        if (temp <= 5)
                        {
                            SetTileAt(x, y + 1, 1, _oremap);
                            orecount += 1;
                        }
                        if (temp >= 6 )
                        {
                            SetTileAt(x, y + 1, 2, _oremap);
                            orecount += 1;
                        }
                     
                    }

                }

                // Fill the rest of the column with air or another tile to represent empty space
                for (int y = _tilemap.MapHeight - terrainHeight - 1; y >= 0; y--)
                {
                    //   SetTileAt(x, y, 0, _tilemap); // Assuming 0 is the index for your empty or air texture
                }

            }
        }
        static void SaveToFile(string data)
        {
            File.WriteAllText("essence.txt", data);
        }
        static int LoadFromFile()
        {
            if (File.Exists("essence.txt"))
            {
                string fileContent = File.ReadAllText("essence.txt");

                // Option 1: Using int.Parse
                // This will throw an exception if the content is not a valid integer.
                // return int.Parse(fileContent);

                // Option 2: Using int.TryParse (safer)
                // This will not throw an exception; it will return 0 if the content is not a valid integer.
                if (int.TryParse(fileContent, out int result))
                {
                    return result;
                }
                else
                {
                    Console.WriteLine($"The content of essence.txt is not a valid integer.");
                    return 100;
                }
            }
            return 100;
        }
        /* private void LoadCurrentState()
         {

             Thread staThread = new Thread(() =>
             {
                 // Prompt user for the filename to load
                 using (OpenFileDialog ofd = new OpenFileDialog())
                 {
                     ofd.Filter = "Text Files|*.tmap";
                     ofd.Title = "Load Map";
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
                     ofd.InitialDirectory = contentDirectory;

                     if (ofd.ShowDialog() == DialogResult.OK)
                     {
                         string loadedContent = File.ReadAllText(ofd.FileName);
                         string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(ofd.FileName);
                         _tilemap = Content.Load<BasicTilemap>(fileNameWithoutExtension);
                         // Now, you can process the loadedContent as you wish
                     }
                 }
             });

             staThread.SetApartmentState(ApartmentState.STA);
             staThread.Start();
             staThread.Join();
         }*/
        private void SaveCurrentState()
        {
            StringBuilder sb = new StringBuilder();

            // Add essence to the file
            //sb.AppendLine($"Essence: {_essence}");
            sb.AppendLine("tileset.png");
            sb.AppendLine("32,32");
            sb.AppendLine("50,20");

            // Add tilemap to the file
            for (int y = 0; y < _tilemap.MapHeight; y++)
            {
                for (int x = 0; x < _tilemap.MapWidth; x++)
                {
                    sb.Append(GetTileIndex(x, y, _tilemap));
                    sb.Append(",");
                }
            }
            sb.Length--;
            // Get the directory of the executing assembly
            string baseDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // Define the path for the saved file
            string savePath = Path.Combine(baseDirectory, "Content/example.tmap");

            // Save the tilemap data to the file
            File.WriteAllText(savePath, sb.ToString());

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

            File.WriteAllText(tilemapPath, sb.ToString());
            saved = true;
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
           // _tilemap.DrawBlock(gameTime, _spriteBatch, _selectedBlockIndex);
           _oremap.Draw(gameTime, _spriteBatch);
            
            _spriteBatch.End();
            _spriteBatch.Begin();
            _spriteBatch.DrawString(bangers, "Level: " + level, new Vector2(1, 1), Color.Gold);
            _spriteBatch.DrawString(bangers, "Gems: " + orecount, new Vector2(1, 25), Color.Red);

            _spriteBatch.DrawString(bangers2, "Click on the blocks to destroy them", new Vector2(1, 55), Color.Black);
            _spriteBatch.DrawString(bangers2, "Gather all the gems to go to the next level", new Vector2(1, 70), Color.Black);
            _spriteBatch.DrawString(bangers2, "Your health metter on the right slowly declines", new Vector2(1, 85), Color.Black);
            _spriteBatch.DrawString(bangers2, "Gather gems / blocks to get health", new Vector2(1, 100), Color.Black);
            _spriteBatch.DrawString(bangers2, "The higher level you are the faster your health declines", new Vector2(1, 115), Color.Black);
            if (health <= 0)
            {
                _spriteBatch.DrawString(bangers, "You lose!", new Vector2(300, 200), Color.Red);
                _spriteBatch.DrawString(bangers, "Press R to Restart!", new Vector2(300, 250),Color.Purple);

            }
            Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            _spriteBatch.Draw(pixel, new Rectangle(550, 10, 229, 40), Color.Red);
            if (health >= 1600)
            {
                health = 1600;
            }
            _spriteBatch.Draw(pixel, new Rectangle(550, 10, (int)(Math.Min(health,100) * 2.3), 40), Color.Green);
         
                _spriteBatch.Draw(pixel, new Rectangle(550, 10, (int)(Math.Min((health-100),100) * 2.3), 40), Color.Blue);
            
        
                _spriteBatch.Draw(pixel, new Rectangle(550, 10, (int)(Math.Min((health - 200),200) * 1.15), 40), Color.Purple);
            _spriteBatch.Draw(pixel, new Rectangle(550, 10, (int)(Math.Min((health - 400), 400) * .575), 40), Color.Cyan);
            Color[] rainbowColors = new Color[] {
    Color.Red,
    Color.OrangeRed,
    Color.Orange,
    Color.Gold,
    Color.Yellow,
    Color.YellowGreen,
    Color.Green,
    Color.LightGreen,
    Color.SpringGreen,
    Color.Cyan,
    Color.SkyBlue,
    Color.Blue,
    Color.DarkBlue,
    Color.Indigo,
    Color.Violet,
    Color.Magenta,
    Color.DeepPink
};
            // Update this each frame; 'gameTime' is typically available in the update method
            float time = (float)gameTime.TotalGameTime.TotalSeconds;

            int baseHeight = 40; // Base height of each segment
            float pulseSpeed = 2.0f; // Speed of the pulsating effect
            int pulseHeight = 10; // Max additional height for the pulsating effect

            // Maximum width for the full health bar (change this value to fit your design)


            // Calculate the consistent width of each segment
            int temp = (int)(Math.Min((health - 800), 800) * .2875);

            if (temp > 0)
            {
                _spriteBatch.Draw(pixel, new Rectangle(550, 10, (int)230, 40), Color.White);
            }
            for (int i = 0; i < temp; i++)
            {
                // Calculate the pulsating height for this frame
                int currentHeight = baseHeight + (int)(Math.Sin(time * pulseSpeed + i) * pulseHeight);

                // Calculate the Y position to keep the bar vertically centered
                int yPos = 10 + (baseHeight - currentHeight) / 2;

                _spriteBatch.Draw(pixel, new Rectangle(550 + i, yPos, 1, currentHeight), rainbowColors[i%rainbowColors.Length]);
            }


            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}