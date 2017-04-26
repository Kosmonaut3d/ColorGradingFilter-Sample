using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ColorGrading_Sample.SampleGame
{
    public class Ship
    {
        public Vector2 Position;
        public Vector2 Speed;
        protected float Angle;

        private const float ACCELERATION = 0.4f;
        protected const float ROTATIONSPEED = 0.1f;
        protected const float HALFPI = (float) (Math.PI / 2.0f);
        protected const float PI2 = (float)(Math.PI * 2.0f);
        protected const float PI = (float)(Math.PI);

        protected static Texture2D Texture;
        protected static Vector2 Offset;

        protected Ship(Vector2 position, float angle)
        {
            Position = position;
            Angle = angle;
        }

        public void Load(ContentManager content)
        {
            Texture = content.Load<Texture2D>("SampleGame/ship");
            Offset = new Vector2(Texture.Width/2.0f, Texture.Height/2.0f);
        }

        protected void Accelerate(float delta)
        {
            Speed += new Vector2((float)Math.Cos(Angle - HALFPI), (float)Math.Sin(Angle - HALFPI)) * ACCELERATION * delta;
        }

        protected void Update(float delta)
        {
            Speed *= 0.98f;

            Position += Speed * delta;

            //Bounce off of walls

            if (Position.X - Offset.X < 0) Speed.X = Math.Abs(Speed.X);
            if (Position.Y - Offset.Y < 0) Speed.Y = Math.Abs(Speed.X);
            if (Position.X + Offset.X > ColorGradingSample.Width) Speed.X = -Math.Abs(Speed.X);
            if (Position.Y + Offset.Y > ColorGradingSample.Height) Speed.Y = -Math.Abs(Speed.Y);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle( (int) (Position.X), (int) (Position.Y), (int) (Offset.X*2), (int) (Offset.Y * 2)), null, Color.White, Angle, Offset, SpriteEffects.None, 0);
        }
    }
}
