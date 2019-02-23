using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootShapesUp
{
    class Enemy : Entity
    {
        public static Random rand = new Random();

        private List<IEnumerator<int>> behaviours = new List<IEnumerator<int>>();
        private int timeUntilStart = 60;
        public bool IsActive { get { return timeUntilStart <= 0; } }
        public int PointValue { get; private set; }

        const int cooldownFrames = 30;
        int cooldownRemaining = 0;

        public Enemy(Texture2D image, Vector2 position)
        {
            this.image = image;
            Position = position;
            Radius = image.Width / 3f;
            color = Color.Transparent;
            PointValue = 1;
        }

        public static Enemy CreateSeeker(Vector2 position)
        {
            var enemy = new Enemy(GameRoot.Seeker, position);
            enemy.AddBehaviour(enemy.FollowPlayer());
            enemy.PointValue = 2;

            return enemy;
        }

        public static Enemy CreateSeeker2(Vector2 position)
        {
            var enemy2 = new Enemy(GameRoot.Seeker2, position);
            enemy2.PointValue = 2;
            enemy2.AddBehaviour(enemy2.ShootPlayer());
            return enemy2;
        }

        public static Enemy CreateSeeker3(Vector2 position)
        {
            var enemy3 = new Enemy(GameRoot.Seeker3, position);
            enemy3.AddBehaviour(enemy3.BossMovement());
            enemy3.AddBehaviour(enemy3.BossShoot());
            enemy3.PointValue = 2;
            return enemy3;
        }

        public override void Update()
        {
            if (timeUntilStart <= 0)
                ApplyBehaviours();
            else
            {
                timeUntilStart--;
                color = Color.White * (1 - timeUntilStart / 60f);
            }

            Position += Velocity;
            Position = Vector2.Clamp(Position, Size / 2, GameRoot.ScreenSize - Size / 2);

            Velocity *= 0.8f;             
        }

        private void AddBehaviour(IEnumerable<int> behaviour)
        {
            behaviours.Add(behaviour.GetEnumerator());
        }

        private void ApplyBehaviours()
        {
            for (int i = 0; i < behaviours.Count; i++)
            {
                if (!behaviours[i].MoveNext())
                    behaviours.RemoveAt(i--);
            }
        }

        public void HandleCollision(Enemy other)
        {
            var d = Position - other.Position;
            Velocity += 10 * d / (d.LengthSquared() + 1);
        }

        public void WasShot()
        {
            IsExpired = true;
            //GameRoot.Explosion.Play(0.5f, rand.NextFloat(-0.2f, 0.2f), 0);

        }

        #region Behaviours
        IEnumerable<int> FollowPlayer(float acceleration = 1f)
        {
            while (true)
            {
                if (!PlayerShip.Instance.IsDead)
                    Velocity += (PlayerShip.Instance.Position - Position) * (acceleration / (PlayerShip.Instance.Position - Position).Length());

                if (Velocity != Vector2.Zero)
                    Orientation = Velocity.ToAngle();

                yield return 0;
            }
        }

         IEnumerable<int> BossMovement(float acceleration = 1f)
        {
            Position.Y = 100;
            Position.Y -= 1;
            if (Position.Y == 1000)
            {
                //Position.Y = Position.Y;
                //Position.X += 1;
                Position.X += (float)Math.Sin(Position.X / 100);
                if (Position.X > GameRoot.ScreenSize.X)
                {
                    Position.X -= (float)Math.Sin(Position.X / 100);
                }

                if (Position.X < GameRoot.ScreenSize.X)
                {
                    Position.X += (float)Math.Sin(Position.X / 100);
                }
            }

            yield return 0;
        }

        IEnumerable<int> ShootPlayer(float acceleration = 1f)
        {
            // GameRoot.Shot.Play(0.2f, rand.NextFloat(-0.2f, 0.2f), 0);
            if (cooldownRemaining > 0)
                cooldownRemaining--;

            while (true)
            {
                var aim = PlayerShip.Instance.Position - Position; //r
                if (!PlayerShip.Instance.IsDead)
                {
                    if (aim.LengthSquared() > 0 && cooldownRemaining <= 0)
                    {
                        //var aim = PlayerShip.Instance.Position - Position; //r
                        aim += (PlayerShip.Instance.Position - Position) * (acceleration / (PlayerShip.Instance.Position - Position).Length()); // Aim towards the player.
                        cooldownRemaining = cooldownFrames;
                        float aimAngle = aim.ToAngle();
                        Quaternion aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, aimAngle);

                        float randomSpread = rand.NextFloat(-0.04f, 0.04f) + rand.NextFloat(-0.04f, 0.04f);
                        Vector2 vel = 11f * new Vector2((float)Math.Cos(aimAngle + randomSpread), (float)Math.Sin(aimAngle + randomSpread));

                        Vector2 offset;

                        offset = Vector2.Transform(new Vector2(35, -50), aimQuat);
                        EntityManager.Add(new Bullet(Position + offset, vel));

                        offset = Vector2.Transform(new Vector2(35, 50), aimQuat);
                        EntityManager.Add(new Bullet(Position + offset, vel));
                    }
                }
                if (Velocity != Vector2.Zero)
                    Orientation = Velocity.ToAngle();

                yield return 0;
            }
        }

        IEnumerable<int> BossShoot(float acceleration = 1f)
        {
            // GameRoot.Shot.Play(0.2f, rand.NextFloat(-0.2f, 0.2f), 0);
            int cooldownFrames = 6;
            int cooldownRemaining = 0;

            if (cooldownRemaining > 0)
                cooldownRemaining--;

            while (true)
            {
                var aim = PlayerShip.Instance.Position - Position; //r
                if (!PlayerShip.Instance.IsDead)
                {
                    if (aim.LengthSquared() > 0 && cooldownRemaining <= 0)
                    {
                        //var aim = PlayerShip.Instance.Position - Position; //r
                        aim += (PlayerShip.Instance.Position - Position) * (acceleration / (PlayerShip.Instance.Position - Position).Length()); // Aim towards the player.
                        cooldownRemaining = cooldownFrames;
                        float aimAngle = aim.ToAngle();
                        Quaternion aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, aimAngle);

                        float randomSpread = rand.NextFloat(-0.04f, 0.04f) + rand.NextFloat(-0.04f, 0.04f);
                        Vector2 vel = 11f * new Vector2((float)Math.Cos(aimAngle + randomSpread), (float)Math.Sin(aimAngle + randomSpread));

                        Vector2 offset;

                        offset = Vector2.Transform(new Vector2(600, -70), aimQuat);
                        EntityManager.Add(new Bullet(Position + offset, vel));

                        offset = Vector2.Transform(new Vector2(600, 70), aimQuat);
                        EntityManager.Add(new Bullet(Position + offset, vel));
                    }
                }
                if (Velocity != Vector2.Zero)
                    Orientation = Velocity.ToAngle();

                yield return 0;
            }
        }
        #endregion
    }
}
