using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsApp2.src.Models;
using WinFormsApp2.src.Interfaces;
using WinFormsApp2.src.Enums;

namespace WinFormsApp2.src.Controllers
{
    public class EnemyController
    {
        private string[] map;
        public List<IEnemy> Enemies { get; private set; } = new();

        public EnemyController(string[] map)
        {
            this.map = map;
        }

        public void AddEnemy(IEnemy enemy)
        {
            Enemies.Add(enemy);
        }

        public void UpdateAll()
        {
            foreach (var enemy in Enemies.ToList())
            {
                Move(enemy);

                if (enemy.ReachedBase)
                    Enemies.Remove(enemy);
            }
        }

        private void Move(IEnemy enemy)
        {
            int x = enemy.X;
            int y = enemy.Y;

            if (x + 1 < map[y].Length && map[y][x + 1] == 'O') // Öncelik sağ
            {
                enemy.X++;
                enemy.LastMove = Direction.Right;
                return;
            }

            // Bitiş
            if (x + 1 < map[y].Length && map[y][x + 1] == 'E')
            {
                enemy.ReachedBase = true;
                return;
            }

            // Alternatif yönler
            if (enemy.LastMove != Direction.Up && y + 1 < map.Length && map[y + 1][x] == 'O')
            {
                enemy.Y++;
                enemy.LastMove = Direction.Down;
                return;
            }

            if (enemy.LastMove != Direction.Down && y - 1 >= 0 && map[y - 1][x] == 'O')
            {
                enemy.Y--;
                enemy.LastMove = Direction.Up;
                return;
            }

            // Eğer yukarı ve aşağı takılı kaldıysa sola deneyebilir
            if (x - 1 >= 0 && map[y][x - 1] == 'O')
            {
                enemy.X--;
                enemy.LastMove = Direction.Left;
            }
        }

        public void TakeDamage(IEnemy enemy, float damage)
        {
            if (!enemy.IsDead)
            {
                enemy.Health -= damage;
                if (enemy.Health < 0)
                    enemy.Health = 0;
            }
        }





    }
}
