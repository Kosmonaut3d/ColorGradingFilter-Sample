using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColorGrading_Sample.SampleGame
{
    public class AiShip : Ship
    {
        private readonly Color _color;
        public Vector3 ColorV3;

        public AiShip(Vector2 position, float angle, Color color) : base(position, angle)
        {
            _color = color;
            ColorV3 = color.ToVector3();
        }
        
        public void Update(float delta, PlayerShip playerShip)
        {
            //Get Angle between this and the player!
            
            float targetAngle = (float) Math.Atan2(Position.Y - playerShip.Position.Y, Position.X - playerShip.Position.X) - HALFPI;

            float distance = Vector2.DistanceSquared(playerShip.Position, Position);

            if (distance < 200*200) targetAngle += PI;

            float offset = targetAngle - Angle;

            if (offset > Math.PI) offset -= PI2;
            if (offset < -Math.PI) offset += PI2;

            if (offset > 0.1f) Angle += ROTATIONSPEED * delta;
            else if (offset < -0.1f) Angle -= ROTATIONSPEED * delta;

            if(offset < 0.2f || distance < 200*200) Accelerate(delta / 2);

            base.Update(delta);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle((int)(Position.X), (int)(Position.Y), (int)(Offset.X * 2), (int)(Offset.Y * 2)), null, _color, Angle, Offset, SpriteEffects.None, 0);
        }
    }
}