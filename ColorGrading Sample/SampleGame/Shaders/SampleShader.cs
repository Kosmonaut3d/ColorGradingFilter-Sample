using System.Collections.Generic;
using ColorGrading_Sample.Filters.ColorGrading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ColorGrading_Sample.SampleGame.Shaders
{
    //Just a basic read-back shader (persistence)
    //The interesting thing it does is to draw lines between 2 points, so framerate doesn't matter for continuity.
    public class SampleShader
    {
        private readonly FullScreenQuadRenderer _fsq;
        private RenderTarget2D _renderTarget0;
        private RenderTarget2D _renderTarget1;

        private Vector2[] _positions;
        private Vector2[] _lastPositions;
        private Vector3[] _colors;
        private readonly EffectParameter _positionsParam;
        private readonly EffectParameter _lastPositionsParam;
        private readonly EffectParameter _colorsParam;
        private readonly EffectParameter _texParam;
        private readonly EffectPass _trailPass;

        private bool _offframe;

        public SampleShader(GraphicsDevice graphics, ContentManager content, string shaderPath)
        {
            var shaderEffect = content.Load<Effect>(shaderPath);
            _lastPositionsParam = shaderEffect.Parameters["LastPositions"];
            _positionsParam = shaderEffect.Parameters["Positions"];
            _colorsParam = shaderEffect.Parameters["Colors"];
            _texParam = shaderEffect.Parameters["Tex"];

            _trailPass = shaderEffect.Techniques["Trails"].Passes[0];
            _fsq = new FullScreenQuadRenderer(graphics);
        }

        public void Draw(GraphicsDevice graphics, int width, int height, List<AiShip> ships, PlayerShip playerShip)
        {
            if (_renderTarget0 == null || _renderTarget0.Width != width || _renderTarget0.Height != height)
            {
                _renderTarget0?.Dispose();
                _renderTarget1?.Dispose();
                _renderTarget0 = new RenderTarget2D(graphics, width, height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                _renderTarget1 = new RenderTarget2D(graphics, width, height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

                //Clear
                graphics.SetRenderTarget(_renderTarget0); graphics.Clear(Color.Black);
                graphics.SetRenderTarget(_renderTarget1); graphics.Clear(Color.Black);
            }

            _offframe = !_offframe;

            graphics.SetRenderTarget(_offframe ? _renderTarget1 : _renderTarget0);

            //Initialize
            if (_positions == null)
            {
                _positions = new Vector2[ships.Count + 1];
                _lastPositions = new Vector2[ships.Count + 1];
                _colors = new Vector3[ships.Count + 1];

                //Ai ships
                for (var index = 0; index < ships.Count; index++)
                {
                    AiShip ship = ships[index];
                    _colors[index] = ship.ColorV3;

                    _lastPositions[index] = ship.Position;
                    _positions[index] = ship.Position;
                }
                //Player ship
                _colors[ships.Count] = Vector3.One;
                _lastPositions[ships.Count] = playerShip.Position;
                _positions[ships.Count] = playerShip.Position;
            }

            for (var index = 0; index < ships.Count; index++)
            {
                AiShip ship = ships[index];

                if (Vector2.DistanceSquared(ship.Position, _lastPositions[index]) > 49)
                {
                    _lastPositions[index] = _positions[index];
                    _positions[index] = ship.Position;
                }
            }

            if (Vector2.DistanceSquared(playerShip.Position, _lastPositions[ships.Count]) > 49)
            {
                _lastPositions[ships.Count] = _positions[ships.Count];
                _positions[ships.Count] = playerShip.Position;
            }

            //Colors[0] = Vector3.One;
            //Positions[0] = Mouse.GetState().Position.ToVector2();

            _positionsParam.SetValue(_positions);
            _lastPositionsParam.SetValue(_lastPositions);

            _colorsParam.SetValue(_colors);

            _texParam.SetValue(_offframe ? _renderTarget0 : _renderTarget1);

            graphics.BlendState = BlendState.Opaque;

            _trailPass.Apply();
            _fsq.RenderFullscreenQuad(graphics);

        }


        public RenderTarget2D GetRenderTarget()
        {
            return _offframe ? _renderTarget1 : _renderTarget0;
        }

    }
}
