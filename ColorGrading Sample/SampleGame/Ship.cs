using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ColorGrading_Sample.SampleGame
{
    public class Ship
    {
        public Vector2 Position;
        public Vector2 Speed;
        public float Angle;

        protected const float ACCELERATION = 0.4f;
        protected const float ROTATIONSPEED = 0.1f;
        protected const float HALFPI = (float) (Math.PI / 2.0f);
        protected const float PI2 = (float)(Math.PI * 2.0f);
        protected const float PI = (float)(Math.PI);

        protected static Texture2D _texture;
        protected static Vector2 _offset;

        public Ship(Vector2 position, float angle)
        {
            Position = position;
            Angle = angle;
        }

        public void Load(ContentManager content)
        {
            _texture = content.Load<Texture2D>("SampleGame/ship");
            _offset = new Vector2(_texture.Width/2.0f, _texture.Height/2.0f);
        }

        protected void Accelerate(float delta)
        {
            Speed += new Vector2((float)Math.Cos(Angle - HALFPI), (float)Math.Sin(Angle - HALFPI)) * ACCELERATION * delta;
        }

        public virtual void Update(float delta)
        {
            Speed *= 0.98f;

            Position += Speed;

            //Bounce off of walls

            if (Position.X - _offset.X < 0) Speed.X = Math.Abs(Speed.X);
            if (Position.Y - _offset.Y < 0) Speed.Y = Math.Abs(Speed.X);
            if (Position.X + _offset.X > ColorGradingSample.Width) Speed.X = -Math.Abs(Speed.X);
            if (Position.Y + _offset.Y > ColorGradingSample.Height) Speed.Y = -Math.Abs(Speed.Y);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, new Rectangle( (int) (Position.X), (int) (Position.Y), (int) (_offset.X*2), (int) (_offset.Y * 2)), null, Color.White, Angle, _offset, SpriteEffects.None, 0);
        }
    }
}
