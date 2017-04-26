using System;
using System.Security.Cryptography;
using Bloom_Sample;
using ColorGrading_Sample.Filters.ColorGrading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ColorGrading_Sample.SampleGame;

namespace ColorGrading_Sample
{
    /// <summary>
    /// A sample application that showcases the use of color grading. It also makes use of bloom and has a basic interactive "game" inside.
    /// </summary>
    public class ColorGradingSample : Game
    {

        #region fields
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static int Width = 1280;
        public static int Height = 800;

        private KeyboardState _state;
        private SampleGameManager _sampleGame;

        private RenderTarget2D _backbufferRenderTarget;

        private BloomFilter _bloomFilter;
        private ColorGradingFilter _colorGradingFilter;

        private Texture2D _lut_32_default;
        private Texture2D _lut_default;
        private Texture2D _lut_ver1;
        private Texture2D _lut_ver2;
        private Texture2D _lut_ver3;
        private Texture2D _lut_ver4;
        private Texture2D _lut_ver5;
        private Texture2D _lut_ver6;
        private Texture2D _lut_ver7;

        private Texture2D _kingfisher;
        private Texture2D _winebar;
        private Texture2D _church;

        private DisplayModes _displayMode;
        private LUTModes _lutModes;
        public enum DisplayModes
        {
            Kingfisher,
            Winebar,
            Church,
            Game
        }

        //Originally with specific names, like "red filter". 
        public enum LUTModes
        {
             ver1, ver2, ver3, ver4, ver5, ver6, ver7
        }

        #endregion

        public ColorGradingSample()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = Width;
            graphics.PreferredBackBufferHeight = Height;

            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            IsFixedTimeStep = true;
            //TargetElapsedTime = TimeSpan.FromMilliseconds(100);
            graphics.SynchronizeWithVerticalRetrace = false;
            IsMouseVisible = true;

            graphics.ApplyChanges();
        }
        
        protected override void Initialize()
        {
            _sampleGame = new SampleGameManager();
            _sampleGame.Initialize();

            Load();
        }
        
        protected void Load()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _sampleGame.Load(Content, GraphicsDevice);
            _bloomFilter = new BloomFilter();
            _bloomFilter.Load(GraphicsDevice, Content, Width, Height, SurfaceFormat.Color);

            _colorGradingFilter = new ColorGradingFilter(GraphicsDevice, Content, "Shaders/ColorGrading/ColorGrading");

            _lut_32_default = Content.Load<Texture2D>("Shaders/ColorGrading/lut_32_default");
            _lut_default = Content.Load<Texture2D>("Shaders/ColorGrading/lut_default");
            _lut_ver5 = Content.Load<Texture2D>("Shaders/ColorGrading/lut_ver1");
            _lut_ver1 = Content.Load<Texture2D>("Shaders/ColorGrading/lut_ver2");
            _lut_ver3 = Content.Load<Texture2D>("Shaders/ColorGrading/lut_ver3");
            _lut_ver4 = Content.Load<Texture2D>("Shaders/ColorGrading/lut_ver4");
            _lut_ver2 = Content.Load<Texture2D>("Shaders/ColorGrading/lut_ver5");
            _lut_ver6 = Content.Load<Texture2D>("Shaders/ColorGrading/lut_ver6");
            _lut_ver7 = Content.Load<Texture2D>("Shaders/ColorGrading/lut_ver7");

            //https://pixabay.com
            //CC0 licence.
            _kingfisher = Content.Load<Texture2D>("SampleImages/Kingfisher");
            _church = Content.Load<Texture2D>("SampleImages/Church");
            _winebar = Content.Load<Texture2D>("SampleImages/Winebar");
        }
        
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _state = Keyboard.GetState();

            if(_displayMode == DisplayModes.Game)
                _sampleGame.Update(gameTime, _state);

            Window.Title = "F1 - F3: static images, F4: game, F5 - F11: LUTs";

            /*
            //Create LUT
            if (_state.IsKeyDown(Keys.F12))
            {
                _colorGradingFilter.CreateLUT(GraphicsDevice, ColorGradingFilter.LUTSizes.Size16, "LUT16.png");
            }
            */

            //Change display mode
            if (_state.IsKeyDown(Keys.F1))
            {
                _displayMode = DisplayModes.Kingfisher;
            }
            if (_state.IsKeyDown(Keys.F2))
            {
                _displayMode = DisplayModes.Church;
            }
            if (_state.IsKeyDown(Keys.F3))
            {
                _displayMode = DisplayModes.Winebar;
            }
            if (_state.IsKeyDown(Keys.F4))
            {
                _displayMode = DisplayModes.Game;
            }

            //Change LUTs
            if (_state.IsKeyDown(Keys.F5))
            {
                _lutModes = LUTModes.ver1;
            }
            if (_state.IsKeyDown(Keys.F6))
            {
                _lutModes = LUTModes.ver2;
            }
            if (_state.IsKeyDown(Keys.F7))
            {
                _lutModes = LUTModes.ver3;
            }
            if (_state.IsKeyDown(Keys.F8))
            {
                _lutModes = LUTModes.ver4;
            }
            if (_state.IsKeyDown(Keys.F9))
            {
                _lutModes = LUTModes.ver5;
            }
            if (_state.IsKeyDown(Keys.F10))
            {
                _lutModes = LUTModes.ver6;
            }
            if (_state.IsKeyDown(Keys.F11))
            {
                _lutModes = LUTModes.ver7;
            }
        }
        
        protected override void Draw(GameTime gameTime)
        {
            //Draw our game to a seperate texture
            if (_displayMode == DisplayModes.Game)
            {
                //Initialize our render target if not done yet
                if (_backbufferRenderTarget == null || _backbufferRenderTarget.Width != Width ||
                    _backbufferRenderTarget.Height != Height)
                {
                    _backbufferRenderTarget = new RenderTarget2D(GraphicsDevice, Width, Height);
                }

                //Background
                _sampleGame.DrawBackground(GraphicsDevice, Width, Height);

                //Foreground
                GraphicsDevice.SetRenderTarget(_backbufferRenderTarget);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                _sampleGame.Draw(spriteBatch);
                spriteBatch.End();
            }

            /*this is the important step
             * We need to provide an image for the color grading
             * filter to process, along with a look up table.
             * If we use this for a game we must draw our game to a texture (rendertarget) beforehand*/
            Texture2D colorGraded = _colorGradingFilter.Draw(GraphicsDevice, GetSelectedImage(), GetSelectedLUT());
            

            //Apply bloom filter if we draw the game!
            Texture2D bloom = null;
            if (_displayMode == DisplayModes.Game)
                bloom = _bloomFilter.Draw(colorGraded, Width, Height);

            //Draw to the backbuffer
            GraphicsDevice.SetRenderTarget(null);

            //Draw our images to the screen
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

            //The texture can be treated just as any other one.
            spriteBatch.Draw(colorGraded, new Rectangle(0,0,Width,Height), Color.White);

            //Bloom for our game
            if (_displayMode == DisplayModes.Game)
                spriteBatch.Draw(bloom, Vector2.Zero, Color.White);


            spriteBatch.End();
        }

        private Texture2D GetSelectedLUT()
        {
            switch (_lutModes)
            {
                case LUTModes.ver1:
                    return _lut_ver1;
                case LUTModes.ver2:
                    return _lut_ver2;
                case LUTModes.ver3:
                    return _lut_ver3;
                case LUTModes.ver4:
                    return _lut_ver4;
                case LUTModes.ver5:
                    return _lut_ver5;
                case LUTModes.ver6:
                    return _lut_ver6;
                case LUTModes.ver7:
                    return _lut_ver7;
            }
            return _lut_ver1;
        }

        private Texture2D GetSelectedImage()
        {
            switch (_displayMode)
            {
                case DisplayModes.Kingfisher:
                    return _kingfisher;
                case DisplayModes.Winebar:
                    return _winebar;
                case DisplayModes.Church:
                    return _church;
                case DisplayModes.Game:
                    return _backbufferRenderTarget;
            }
            return _kingfisher;
        }

        protected override void UnloadContent()
        {
            _backbufferRenderTarget?.Dispose();
            _bloomFilter?.Dispose();
            _colorGradingFilter?.Dispose();
        }
    }
}
