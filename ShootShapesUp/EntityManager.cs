using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootShapesUp
{
    static class EntityManager
    {
        static List<Entity> entities = new List<Entity>();
        static List<Enemy> enemies = new List<Enemy>(1);
        static List<Bullet> bullets = new List<Bullet>();
        static List<Enemy> boss = new List<Enemy>();
  
        static bool isUpdating;
        static List<Entity> addedEntities = new List<Entity>();

        public static int Count { get { return entities.Count; } }

        public static int Score;



        public static void Add(Entity entity)
        {
            if (GameRoot.gameState != GameRoot.GameState.LevelThree)
            {
                if (!isUpdating)
                    AddEntity(entity);
                else
                    addedEntities.Add(entity);
            }
            else
            {
                if (!isUpdating)
                    AddEntity(entity);
                else
                    addedEntities.Add(entity);
            }
        }

        private static void AddEntity(Entity entity)
        {
            entities.Add(entity);
            if (entity is Bullet)
                bullets.Add(entity as Bullet);
            else if (entity is Enemy)
                enemies.Add(entity as Enemy);
            else if (entity is Enemy)
                boss.Add(entity as Enemy);
        }

        public static void Update()
        {
            isUpdating = true;
            HandleCollisions();

            foreach (var entity in entities)
            {
                entity.Update();
                
            }
            isUpdating = false;

            foreach (var entity in addedEntities)
            {
                AddEntity(entity);
                
            }

            addedEntities.Clear();

            entities = entities.Where(x => !x.IsExpired).ToList();
            bullets = bullets.Where(x => !x.IsExpired).ToList();
            enemies = enemies.Where(x => !x.IsExpired).ToList();
            boss = boss.Where(x => !x.IsExpired).ToList();
        }

        static void HandleCollisions()
        {
            // handle collisions between enemies
            if (GameRoot.gameState != GameRoot.GameState.LevelThree)
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    for (int j = i + 1; j < enemies.Count; j++)
                    {
                        if (IsColliding(enemies[i], enemies[j]))
                        {
                            enemies[i].HandleCollision(enemies[j]);
                            enemies[j].HandleCollision(enemies[i]);
                        }
                    }
                }
            }


            // handle collisions between bullets and enemies
            for (int i = 0; i < enemies.Count; i++)
                for (int j = 0; j < bullets.Count; j++)
                {
                    if (IsColliding(enemies[i], bullets[j]))
                    {
                        enemies[i].WasShot();
                        bullets[j].IsExpired = true;
                        Score = Score + 100;
                    }
                }
            for (int i = 0; i < bullets.Count; i++)
            {
                if(IsColliding(PlayerShip.Instance, bullets[i]))
                {
                    PlayerShip.Instance.Kill();
                }
            }

            for (int i = 0; i < boss.Count; i++)
                for (int j = 0; j < bullets.Count; j++)
                {
                    if (IsColliding(enemies[i], bullets[j]))
                    {
                        enemies[i].WasShot();
                        bullets[j].IsExpired = true;
                        Score = Score + 100;
                    }
                }

            // handle collisions between the player and enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].IsActive && IsColliding(PlayerShip.Instance, enemies[i]))
                {
                    if (GameRoot.gameState == GameRoot.GameState.LevelOne || GameRoot.gameState == GameRoot.GameState.LevelTwo)
                    {
                        PlayerShip.Instance.Kill();
                        enemies.ForEach(x => x.WasShot());
                        EnemySpawner.Reset();
                        break;
                    }
                    else if (GameRoot.gameState == GameRoot.GameState.LevelThree)
                    {
                        PlayerShip.Instance.Kill();
                        enemies.ForEach(x => x.WasShot());
                        break;
                    }
                }
            }
        }

        private static bool IsColliding(Entity a, Entity b)
        {
            float radius = a.Radius + b.Radius;
            return !a.IsExpired && !b.IsExpired && Vector2.DistanceSquared(a.Position, b.Position) < radius * radius;
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (var entity in entities)
                entity.Draw(spriteBatch);
        }
    }
}
