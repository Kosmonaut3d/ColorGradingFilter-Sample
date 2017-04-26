using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ColorGrading_Sample.SampleGame
{
    public class PlayerShip : Ship
    {
        public PlayerShip(Vector2 position, float angle) : base(position, angle)
        {

        }
        public void Update(float delta, KeyboardState state)
        {

            if (state.IsKeyDown(Keys.A)) Angle -= ROTATIONSPEED * delta;
            if (state.IsKeyDown(Keys.D)) Angle += ROTATIONSPEED * delta;
            if (state.IsKeyDown(Keys.W)) Accelerate(delta);

            base.Update(delta);
        }
    }
}