using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColorGrading_Sample.SampleGame.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ColorGrading_Sample.SampleGame
{
    /// <summary>
    /// Simple "game". Control with WASD, the other ships will follow you but then try to back away when close.
    /// </summary>
    public class SampleGameManager
    {
        private PlayerShip _playerShip;
        private List<AiShip> _aiShips = new List<AiShip>();

        private SampleShader _sampleShader;

        public void Initialize()
        {
            //Spawn player in the middle of the screen
            _playerShip = new PlayerShip(new Vector2(640, 400), 0);


            //Spawn some other ships

            Random random = new Random();

            int cols = 10;
            int rows = 7;

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    if (y == rows/2) continue;

                    Vector2 position = new Vector2(ColorGradingSample.Width / (cols + 1) * (x + 1), ColorGradingSample.Height / (rows +1) *(y+1));

                    Color randomColor = new Color(random.Next(255), random.Next(255), random.Next(255));
                    _aiShips.Add(new AiShip(position, 0, randomColor));
                }
            }
        }

        public void Load(ContentManager content, GraphicsDevice graphics)
        {
            _playerShip.Load(content);

            _sampleShader = new SampleShader(graphics, content, "SampleGame/SampleGameShader");
        }

        public void DrawBackground(GraphicsDevice graphics, int width, int height)
        {
            _sampleShader.Draw(graphics, width, height, _aiShips, _playerShip);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Background
            
            spriteBatch.Draw(_sampleShader.GetRenderTarget(), new Rectangle(0, 0, ColorGradingSample.Width, ColorGradingSample.Height), new Color(1, 1, 1, 0.5f));
            //Foreground

            _playerShip.Draw(spriteBatch);

            foreach (AiShip aiShip in _aiShips)
            {
                aiShip.Draw(spriteBatch);
            }
        }

        public void Update(GameTime gameTime, KeyboardState state)
        {
            float delta = (float) (gameTime.ElapsedGameTime.TotalMilliseconds * 60.0 / 1000.0);

            _playerShip.Update(delta, state);

            foreach (AiShip aiShip in _aiShips)
            {
                aiShip.Update(delta, _playerShip);
            }

            //Collision
            for (var i = 0; i < _aiShips.Count; i++)
            {
                Ship ship = _aiShips[i];
                //With all the other ships!
                for (var j = i + 1; j < _aiShips.Count + 1; j++)
                {
                    Ship opponent;

                    if (j < _aiShips.Count) opponent = _aiShips[j];
                    else opponent = _playerShip;

                    if (Vector2.DistanceSquared(ship.Position, opponent.Position) < 45 * 45)
                    {
                        //Both get a boost in opposite directions
                        Vector2 Bounce = ship.Position - opponent.Position;
                        Bounce.Normalize();
                        
                        ship.Speed += Bounce * (opponent == _playerShip ? 8 : 2);
                        opponent.Speed -= Bounce * (opponent == _playerShip ? 0.5f : 2);
                    }
                }
            }
        }
    }
}
