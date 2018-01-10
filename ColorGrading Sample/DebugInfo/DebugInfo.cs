using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Resources;

public class DebugInfo
{
    private SpriteBatch _spriteBatch;
    private SpriteFont _monospaceFont;

    private readonly MngStringBuilder _mngStringBuilder = new MngStringBuilder(2048);

    public Color consoleColor = Color.Coral;
    
    private readonly StringBuilder sb_frameTime = new StringBuilder("Main: ");
    private readonly StringBuilder sb_ms        = new StringBuilder(" ms ");

    private readonly StringBuilder sb_fps          = new StringBuilder(" (FPS: ");
    private readonly StringBuilder sb_dotdotdot    = new StringBuilder(" ... ");
    private readonly StringBuilder sb_greaterthan  = new StringBuilder(" > ");
    private readonly StringBuilder sb_closeBracket = new StringBuilder(")");
    private readonly StringBuilder sb_multipliedBy = new StringBuilder(" x ");
    private readonly StringBuilder sb_emptySpace   = new StringBuilder(" ");

    // FPS metrics
    private double _fps;
    private int _accumulatedFrames    = 0;
    private double _accumulatedTime   = 0.0;
    private double _averagedFPS       = 0.0f;
    private double _minFPS            = 100000.0f;
    private double _minFPSAccumulated = 0.0f;


    public void LoadContent(SpriteBatch spriteBatch, ContentManager content)
    {
        _spriteBatch = spriteBatch;
        _monospaceFont = content.Load<SpriteFont>("Fonts/monospace");
    }


    public void Draw(GameTime gameTime)
    {
        double elapsedTime = gameTime.ElapsedGameTime.TotalMilliseconds;
        double currentFPS = 1000.0f / elapsedTime;
        
        //Some smoothing
        _fps = 0.9 * _fps + 0.1 * (currentFPS);

        GetRoundedFpsOverSeconds(elapsedTime, currentFPS);
        
        //clear
        _mngStringBuilder.Length = 0;

        _mngStringBuilder.Append(sb_frameTime);
        _mngStringBuilder.AppendTrim(gameTime.ElapsedGameTime.TotalMilliseconds);
        _mngStringBuilder.Append(sb_ms);

        _mngStringBuilder.AppendAt(15, sb_fps);
        _mngStringBuilder.Append((int)Math.Round(_fps));
        _mngStringBuilder.Append(sb_dotdotdot);
        _mngStringBuilder.Append((int)Math.Round(_averagedFPS));
        _mngStringBuilder.Append(sb_greaterthan);
        _mngStringBuilder.Append((int)Math.Round(_minFPSAccumulated));
        _mngStringBuilder.AppendLine(sb_closeBracket);

        _spriteBatch.Begin();

        //Shadow
        _spriteBatch.DrawString(_monospaceFont, _mngStringBuilder.StringBuilder,
            new Vector2(11.0f, 11.0f), Color.Black);
        //Info
        _spriteBatch.DrawString(_monospaceFont, _mngStringBuilder.StringBuilder,
            new Vector2(10.0f, 10.0f), consoleColor);

        _spriteBatch.End();
    }

    private void GetRoundedFpsOverSeconds(double elapsedTime, double currentFPS)
    {
        _accumulatedFrames++;
        _accumulatedTime += elapsedTime;

        if (currentFPS < _minFPS)
        {
            _minFPS = currentFPS;
        }

        if(_accumulatedTime > 1000.0)
        {
            _averagedFPS       = _accumulatedFrames / (_accumulatedTime * 0.001);
            _minFPSAccumulated = _minFPS;

            _accumulatedTime   = 0.0f;
            _accumulatedFrames = 0;
            _minFPS            = 10000.0f;
            
        }
    }
}
