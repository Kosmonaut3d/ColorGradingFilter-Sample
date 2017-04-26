using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ColorGrading_Sample.Filters.ColorGrading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ColorGrading_Sample.SampleGame.Shaders
{
    //Just a basic read-back shader (persistence)
    //The interesting thing it does is to draw lines between 2 points, so framerate doesn't matter for continuity.
    public class SampleShader
    {
        private Effect _shaderEffect;
        private FullScreenQuadRenderer _fsq;
        private RenderTarget2D renderTarget0;
        private RenderTarget2D renderTarget1;

        private Vector2[] Positions;
        private Vector2[] LastPositions;
        private Vector3[] Colors;
        private EffectParameter _positionsParam;
        private EffectParameter _lastPositionsParam;
        private EffectParameter _colorsParam;
        private EffectParameter _texParam;
        private EffectPass _trailPass;

        private bool offframe = false;

        public SampleShader(GraphicsDevice graphics, ContentManager content, string shaderPath)
        {
            _shaderEffect = content.Load<Effect>(shaderPath);
            _lastPositionsParam = _shaderEffect.Parameters["LastPositions"];
            _positionsParam = _shaderEffect.Parameters["Positions"];
            _colorsParam = _shaderEffect.Parameters["Colors"];
            _texParam = _shaderEffect.Parameters["Tex"];

            _trailPass = _shaderEffect.Techniques["Trails"].Passes[0];
            _fsq = new FullScreenQuadRenderer(graphics);
        }

        public void Draw(GraphicsDevice graphics, int width, int height, List<AiShip> ships, PlayerShip playerShip)
        {
            if (renderTarget0 == null || renderTarget0.Width != width || renderTarget0.Height != height)
            {
                renderTarget0?.Dispose();
                renderTarget1?.Dispose();
                renderTarget0 = new RenderTarget2D(graphics, width, height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                renderTarget1 = new RenderTarget2D(graphics, width, height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

                //Clear
                graphics.SetRenderTarget(renderTarget0); graphics.Clear(Color.Black);
                graphics.SetRenderTarget(renderTarget1); graphics.Clear(Color.Black);
            }

            offframe = !offframe;

            graphics.SetRenderTarget(offframe ? renderTarget1 : renderTarget0);

            //Initialize
            if (Positions == null)
            {
                Positions = new Vector2[ships.Count + 1];
                LastPositions = new Vector2[ships.Count + 1];
                Colors = new Vector3[ships.Count + 1];

                //Ai ships
                for (var index = 0; index < ships.Count; index++)
                {
                    AiShip ship = ships[index];
                    Colors[index] = ship.ColorV3;

                    LastPositions[index] = ship.Position;
                    Positions[index] = ship.Position;
                }
                //Player ship
                Colors[ships.Count] = Vector3.One;
                LastPositions[ships.Count] = playerShip.Position;
                Positions[ships.Count] = playerShip.Position;
            }

            for (var index = 0; index < ships.Count; index++)
            {
                AiShip ship = ships[index];

                if (Vector2.DistanceSquared(ship.Position, LastPositions[index]) > 49)
                {
                    LastPositions[index] = Positions[index];
                    Positions[index] = ship.Position;
                }
            }

            if (Vector2.DistanceSquared(playerShip.Position, LastPositions[ships.Count]) > 49)
            {
                LastPositions[ships.Count] = Positions[ships.Count];
                Positions[ships.Count] = playerShip.Position;
            }

            //Colors[0] = Vector3.One;
            //Positions[0] = Mouse.GetState().Position.ToVector2();

            _positionsParam.SetValue(Positions);
            _lastPositionsParam.SetValue(LastPositions);

            _colorsParam.SetValue(Colors);

            _texParam.SetValue(offframe ? renderTarget0 : renderTarget1);

            graphics.BlendState = BlendState.Opaque;

            _trailPass.Apply();
            _fsq.RenderFullscreenQuad(graphics);

        }


        public RenderTarget2D GetRenderTarget()
        {
            return offframe ? renderTarget1 : renderTarget0;
        }
        public RenderTarget2D GetRenderTargetOff()
        {
            return offframe ? renderTarget0 : renderTarget1;
        }

    }
}
