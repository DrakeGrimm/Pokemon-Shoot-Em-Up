using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootShapesUp
{
    class PlayerShip : Entity
    {
        private static PlayerShip instance;
        public static PlayerShip Instance
        {
            get
            {
                if (instance == null)
                    instance = new PlayerShip();

                return instance;
            }
        }

        MouseState mouseState;

        const int cooldownFrames = 6;
        int cooldownRemaining = 0;

        int framesUntilRespawn = 0;
        public bool IsDead { get { return framesUntilRespawn > 0; } }

        static Random rand = new Random();

        private PlayerShip()
        {
            image = GameRoot.Player;
            Position = GameRoot.ScreenSize / 2;
            Radius = 25;
        }

        public override void Update()
        {
            if (IsDead)
            {
                --framesUntilRespawn;
                EntityManager.Score = 0;
                return;
                
            }
         
            var aim = Input.GetAimDirection();
            if (aim.LengthSquared() > 0 && cooldownRemaining <= 0)
            {
                cooldownRemaining = cooldownFrames;
                float aimAngle = aim.ToAngle();
                Quaternion aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, aimAngle);

                float randomSpread = rand.NextFloat(-0.04f, 0.04f) + rand.NextFloat(-0.04f, 0.04f);
                Vector2 vel = 11f * new Vector2((float)Math.Cos(aimAngle + randomSpread), (float)Math.Sin(aimAngle + randomSpread));

                Vector2 offset;

                mouseState = Mouse.GetState();

                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    GameRoot.Shot.Play(0.5f, rand.NextFloat(-0.2f, 0.2f), 0);
                    offset = Vector2.Transform(new Vector2(50, -8), aimQuat);
                    EntityManager.Add(new Bullet(Position + offset, vel));

                    offset = Vector2.Transform(new Vector2(50, 8), aimQuat);
                    EntityManager.Add(new Bullet(Position + offset, vel));

                    if (GameRoot.gameState == GameRoot.GameState.LevelTwo || GameRoot.gameState == GameRoot.GameState.LevelThree)
                    {
                        offset = Vector2.Transform(new Vector2(50, 20), aimQuat);
                        EntityManager.Add(new Bullet(Position + offset, vel));
                        offset = Vector2.Transform(new Vector2(50, -20), aimQuat);
                        EntityManager.Add(new Bullet(Position + offset, vel));
                    }
                }
               // GameRoot.Shot.Play(0.2f, rand.NextFloat(-0.2f, 0.2f), 0);
            }

            if (cooldownRemaining > 0)
                cooldownRemaining--;

            const float speed = 10;
            Velocity = speed * Input.GetMovementDirection();
            Position += Velocity;
            Position = Vector2.Clamp(Position, Size / 2, GameRoot.ScreenSize - Size / 2);

            if (Velocity.LengthSquared() > 0)
                Orientation = Velocity.ToAngle();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsDead)
            {
                base.Draw(spriteBatch);
            }

        }

        public void Kill()
        {
            GameRoot.gameState = GameRoot.GameState.MainMenu;
            framesUntilRespawn = 5;
        }
    }
}
