using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsApp2.src.Models;
using WinFormsApp2.src.Interfaces;
using WinFormsApp2.src.Enums;
using WinFormsApp2.src.Services; // LOG EKLEMEK İÇİN GEREKLİ

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

            // 🔵 LOG: Düşman oyuna girdi
            LogsManager.Log($"Düşman spawn oldu: {enemy.Type} ({enemy.X},{enemy.Y}) HP:{enemy.Health}");
        }

        public void UpdateAll()
        {
            foreach (var enemy in Enemies.ToList())
            {
                Move(enemy);

                // 🔵 LOG: Düşman kaleye ulaştı
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
            enemy.MoveAccumulator += enemy.Speed;

            if (enemy.MoveAccumulator < 1f)
                return;

            enemy.MoveAccumulator -= 1f;

            int x = enemy.X;
            int y = enemy.Y;

            if (x + 1 < map[y].Length && map[y][x + 1] == 'O')
            {
                enemy.X++;
                enemy.LastMove = Direction.Right;
                return;
            }

            if (x + 1 < map[y].Length && map[y][x + 1] == 'E')
            {
                enemy.ReachedBase = true;
                return;
            }

            if (enemy.LastMove != Direction.Up &&
                y + 1 < map.Length &&
                map[y + 1][x] == 'O')
            {
                enemy.Y++;
                enemy.LastMove = Direction.Down;
                return;
            }

            if (enemy.LastMove != Direction.Down &&
                y - 1 >= 0 &&
                map[y - 1][x] == 'O')
            {
                enemy.Y--;
                enemy.LastMove = Direction.Up;
                return;
            }

            if (x - 1 >= 0 && map[y][x - 1] == 'O')
            {
                enemy.X--;
                enemy.LastMove = Direction.Left;
            }
        }

        public void TakeDamage(IEnemy enemy, float damage)
        {
            if (enemy.IsDead) return;

            enemy.Health -= damage;
            if (enemy.Health < 0)
                enemy.Health = 0;

            // 🔵 LOG: Hasar bilgisi
            LogsManager.Log($"Düşman hasar aldı: {enemy.Type} -{damage} dmg (Kalan HP: {enemy.Health})");

            // 🔴 Öldüyse logla
            if (enemy.Health == 0 && !enemy.IsDead)
            {
                LogsManager.Log($"☠️ Düşman öldü: {enemy.Type} Para: +{enemy.RewardOnDeath}");
            }
        }

        public void Attack(IEnemy enemy)
        {
            // 🔵 LOG: Kaleye vurdu
            LogsManager.Log($"🏰 Kale hasar aldı: {enemy.Type} (-{enemy.DamageToBase} HP)");

            Form1.health -= enemy.DamageToBase;
        }
    }
}
