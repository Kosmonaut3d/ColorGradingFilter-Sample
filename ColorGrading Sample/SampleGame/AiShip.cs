using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ColorGrading_Sample.SampleGame
{
    public class AiShip : Ship
    {
        private Color _color;
        public Vector3 ColorV3;

        public AiShip(Vector2 position, float angle, Color color) : base(position, angle)
        {
            _color = color;
            ColorV3 = color.ToVector3();
        }
        
        public void Update(float delta, PlayerShip _playerShip)
        {
            //Get Angle between this and the player!
            
            float targetAngle = (float) Math.Atan2(Position.Y - _playerShip.Position.Y, Position.X - _playerShip.Position.X) - HALFPI;

            float distance = Vector2.DistanceSquared(_playerShip.Position, Position);

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
            spriteBatch.Draw(_texture, new Rectangle((int)(Position.X), (int)(Position.Y), (int)(_offset.X * 2), (int)(_offset.Y * 2)), null, _color, Angle, _offset, SpriteEffects.None, 0);
        }
    }
}