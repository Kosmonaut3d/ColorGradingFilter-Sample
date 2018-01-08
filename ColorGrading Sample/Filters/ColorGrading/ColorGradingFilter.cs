using System;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ColorGrading_Sample.Filters.ColorGrading
{
    /// <summary>
    /// 
    ///     /// Version 1.1, 26. April. 2017
    /// 
    ///     Color Grading / Correction Filter, TheKosmonaut (kosmonaut3d@googlemail.com)
    /// 
    /// A post-process effect that changes colors of the image based on a look-up table (LUT). 
    /// For more information check out the github info file / readme.md
    /// You can use Draw() to apply the color grading / color correction to an image and use the returned texture for output.
    /// You can use CreateLUT to create default Look-up tables with unmodified colors.
    /// </summary>
    public class ColorGradingFilter : IDisposable
    {

        #region fields & properties

        #region fields
        private readonly Effect _shaderEffect;
        private readonly FullScreenQuadRenderer _fsq;

        private RenderTarget2D _renderTarget;
        
        private readonly EffectParameter _sizeParam;
        private readonly EffectParameter _sizeRootParam;
        private readonly EffectParameter _inputTextureParam;
        private readonly EffectParameter _lut0Param;
        private readonly EffectParameter _lut1Param;
        private readonly EffectParameter _progressParam;
        private readonly EffectPass _createLUTPass;
        private readonly EffectPass _applyLUTPass;
        
        private int _size;
        private float _progress;
        private Texture2D _inputTexture;
        private Texture2D _lookupTable0;
        private Texture2D _lookupTable1;

        public enum LUTSizes { Size16, Size32, Size64, Size4 };

        #endregion

        #region properties
        private int Size
        {
            get { return _size; }
            set
            {
                if (value != _size)
                {
                    if(value != 16 && value != 32 && value!= 64 && value!= 4) throw new NotImplementedException("only 16 and 32 supported right now");
                    _size = value;
                    _sizeParam.SetValue((float)_size);
                    _sizeRootParam.SetValue((float) (_size== 4 ? 2 : _size == 16 ? 4 : 8));
                }
            }
        }

        private float Progress
        {
            get { return _progress; }
            set
            {
                if (Math.Abs(value - _progress)>0.001f)
                {
                    _progress = value;
                    _progressParam.SetValue(value);
                }
            }
        }

        private Texture2D InputTexture
        {
            get { return _inputTexture; }
            set
            {
                if (value != _inputTexture)
                {
                    _inputTexture = value;
                    _inputTextureParam.SetValue(value);
                }
            }
        }

        private Texture2D LookUpTable0 
        {
            get { return _lookupTable0; }
            set
            {
                if (value != _lookupTable0)
                {
                    _lookupTable0 = value;
                    _lut0Param.SetValue(value);
                }
            }
        }

        private Texture2D LookUpTable1
        {
            get { return _lookupTable1; }
            set
            {
                if (value != _lookupTable1)
                {
                    _lookupTable1 = value;
                    _lut1Param.SetValue(value);
                }
            }
        }
        #endregion

        #endregion

        #region initialize
        /// <summary>
        /// A filter that allows color grading by using Look up tables
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="content"></param>
        /// <param name="shaderPath">the relative shader path needed for the content manager to load. For example "Shaders/ColorGrading/Colorgrading"</param>
        public ColorGradingFilter(GraphicsDevice graphics, ContentManager content, string shaderPath)
        {
            _shaderEffect      = content.Load<Effect>(shaderPath);
            _sizeParam         = _shaderEffect.Parameters["Size"];
            _sizeRootParam     = _shaderEffect.Parameters["SizeRoot"];
            _inputTextureParam = _shaderEffect.Parameters["InputTexture"];
            _lut0Param         = _shaderEffect.Parameters["LUT0"];
            _lut1Param         = _shaderEffect.Parameters["LUT1"];
            _progressParam     = _shaderEffect.Parameters["Progress"];

            _applyLUTPass  = _shaderEffect.Techniques["ApplyLUT"].Passes[0];
            _createLUTPass = _shaderEffect.Techniques["CreateLUT"].Passes[0];
            _fsq           = new FullScreenQuadRenderer(graphics);
        }

        public void Dispose()
        {
            _shaderEffect?.Dispose();
            _fsq?.Dispose();
            _renderTarget?.Dispose();
        }

        #endregion

        #region main functions

        /// <summary>
        /// returns a modified image with color grading applied.
        /// </summary>
        /// <param name="graphics"> GraphicsDevice</param>
        /// <param name="input"> The basic texture or rendertarget you want to modify</param>
        /// <param name="lookupTable"> The specific lookup table used</param>
        /// <returns></returns>
        public RenderTarget2D Draw(GraphicsDevice graphics, Texture2D input, Texture2D lookupTable0, Texture2D lookupTable1, float progress)
        {
            if(lookupTable0.Width != lookupTable1.Width)
            {
                throw new Exception("LUTs need to have the same resolution!");
            }

            //Set up rendertarget
            if (_renderTarget == null || _renderTarget.Width != input.Width || _renderTarget.Height != input.Height)
            {
                _renderTarget?.Dispose();
                _renderTarget = new RenderTarget2D(graphics, input.Width, input.Height, false, SurfaceFormat.Color, DepthFormat.None);
            }

            InputTexture = input;
            LookUpTable0 = lookupTable0;
            LookUpTable1 = lookupTable1;
            Size         = ((lookupTable0.Width == 512) ? 64 : (lookupTable0.Width == 256) ? 32 : (lookupTable0.Width == 64) ? 16 : 4);
            Progress     = progress;
                
            graphics.SetRenderTarget(_renderTarget);
            graphics.BlendState = BlendState.Opaque;

            _applyLUTPass.Apply();
            _fsq.RenderFullscreenQuad(graphics);
            return _renderTarget;
        }

        /// <summary>
        /// A function to create and save a new lookup-table with unmodified colors. 
        /// Check the github readme for use.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="lutsize">32 or 16. 32 will result in a larger LUT which results in better images but worse performance</param>
        /// <param name="relativeFilePath">for example "Lut16.png". The base directory is where the .exe is started from</param>
        public void CreateLUT(GraphicsDevice graphics, LUTSizes lutsize, string relativeFilePath)
        {
            _renderTarget?.Dispose();

            //_sizeParam.SetValue((float) ( lutsize == LUTSizes.Size16 ? 16 : lutsize == LUTSizes.Size32 ? 32 : 64));
            //_sizeRootParam.SetValue((float) (lutsize == LUTSizes.Size64 ? 8 : 4));
            Size = lutsize == LUTSizes.Size16 ? 16 : lutsize == LUTSizes.Size32 ? 32 : lutsize == LUTSizes.Size64 ? 64 : 4;
            int size = lutsize == LUTSizes.Size16 ? 16*4 : lutsize == LUTSizes.Size32 ? 32*8 : lutsize == LUTSizes.Size64 ? 64 * 8 : 4*2;

            _renderTarget = new RenderTarget2D(graphics, size, size  / (lutsize == LUTSizes.Size32 ? 2 : 1), false, SurfaceFormat.Color, DepthFormat.None);

            graphics.SetRenderTarget(_renderTarget);

            _createLUTPass.Apply();
            _fsq.RenderFullscreenQuad(graphics);

            //Save this texture
            Stream stream = File.Create(relativeFilePath);
            _renderTarget.SaveAsPng(stream, _renderTarget.Width, _renderTarget.Height);
            stream.Dispose();
        }

#endregion

    }
}
