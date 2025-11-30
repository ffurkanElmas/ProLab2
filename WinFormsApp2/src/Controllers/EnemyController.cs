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
                {
                    Attack(enemy); 
                    Enemies.Remove(enemy);
                    continue;
                }
            }
        }

        private void Move(IEnemy enemy)
        {
            // Hız puanını ekle
            enemy.MoveAccumulator += enemy.Speed;

            // Yeterli puan yoksa hareket etme
            if (enemy.MoveAccumulator < 1f)
                return;

            // 1 karelik hareket hakkı kadar azalt
            enemy.MoveAccumulator -= 1f;

            int x = enemy.X;
            int y = enemy.Y;

            // Sağ
            if (x + 1 < map[y].Length && map[y][x + 1] == 'O')
            {
                enemy.X++;
                enemy.LastMove = Direction.Right;
                return;
            }

            // Bitiş (E)
            if (x + 1 < map[y].Length && map[y][x + 1] == 'E')
            {
                enemy.ReachedBase = true;
                return;
            }

            // Aşağı
            if (enemy.LastMove != Direction.Up &&
                y + 1 < map.Length &&
                map[y + 1][x] == 'O')
            {
                enemy.Y++;
                enemy.LastMove = Direction.Down;
                return;
            }

            // Yukarı
            if (enemy.LastMove != Direction.Down &&
                y - 1 >= 0 &&
                map[y - 1][x] == 'O')
            {
                enemy.Y--;
                enemy.LastMove = Direction.Up;
                return;
            }

            // Sol (zorunlu durum)
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


        public void Attack(IEnemy enemy)
        {
            Form1.health -= enemy.DamageToBase;
        }





    }
}
