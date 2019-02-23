using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootShapesUp
{
    static class EnemySpawner
    {
        static Random rand = new Random();
        static float inverseSpawnChance = 60;

        public static void Update()
        {
            if (GameRoot.gameState == GameRoot.GameState.LevelTwo || GameRoot.gameState == GameRoot.GameState.LevelOne)
                if (!PlayerShip.Instance.IsDead && EntityManager.Count < 200)
                {
                    if (rand.Next((int)inverseSpawnChance) == 0)
                        EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition()));
                }

            if (GameRoot.gameState == GameRoot.GameState.LevelTwo || GameRoot.gameState == GameRoot.GameState.LevelThree)
            {
                if (!PlayerShip.Instance.IsDead && EntityManager.Count < 200)
                {
                    if (rand.Next((int)inverseSpawnChance) == 0)
                        EntityManager.Add(Enemy.CreateSeeker2(GetSpawnPosition()));
                }
            }

            if (GameRoot.gameState == GameRoot.GameState.LevelThree)
            {
                if (EntityManager.Score >= 5000 && EntityManager.Score <= 11000)
                    EntityManager.Add(Enemy.CreateSeeker3(BossSpawnPosition()));
            }

            // slowly increase the spawn rate as time progresses
            if (inverseSpawnChance > 20)
                inverseSpawnChance -= 0.005f;
        }

        private static Vector2 GetSpawnPosition()
        {
            Vector2 pos = new Vector2 (0,0);
                do
                {
                    pos = new Vector2(rand.Next((int)GameRoot.ScreenSize.X), rand.Next((int)GameRoot.ScreenSize.Y));
                }
                while (Vector2.DistanceSquared(pos, PlayerShip.Instance.Position) < 250 * 250);

            return pos;
        }

        public static void Reset()
        {
            inverseSpawnChance = 60;
        }

        public static Vector2 BossSpawnPosition()
        {
            Vector2 pos = new Vector2(1200,100);

            return pos;
        }
    }
}
