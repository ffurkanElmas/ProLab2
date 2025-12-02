using System;
using System.Collections.Generic;
using System.Linq;
using WinFormsApp2.src.Interfaces;
using WinFormsApp2.src.Services;

namespace WinFormsApp2.src.Models.Towers
{
    public class CannonTower : ITower
    {
        public string Type => "Top";
        public int X { get; set; }
        public int Y { get; set; }
        public int Range => 4;
        public int Cost => 75;
        public float Damage => 40f;
        public float FireRate => 3f;
        public float FireCooldown { get; set; } = 0;
        private int splashRadius = 2;

        public CannonTower(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool CanAttack(IEnemy enemy) => !enemy.IsFlying; // Uçan düşman HAYIR

        public void Attack(List<IEnemy> enemies)
        {
            if (FireCooldown > 0) return;

            var targets = enemies
                .Where(e => !e.IsDead && !e.IsFlying &&
                            Math.Abs(e.X - X) <= Range &&
                            Math.Abs(e.Y - Y) <= Range)
                .ToList();

            if (targets.Count == 0) return;

            var center = targets.OrderByDescending(e => e.X).First();

            foreach (var e in enemies)
            {
                if (!e.IsFlying &&
                    Math.Abs(e.X - center.X) <= splashRadius &&
                    Math.Abs(e.Y - center.Y) <= splashRadius)
                {
                    float dmg = Damage;
                    if (e.IsArmored)
                        dmg *= 1 - (100f / (100f + 100f));

                    e.Health -= dmg;
                    if (e.Health < 0) e.Health = 0;

                    // 🔥 Log
                    LogsManager.Log($"{Type} → {e.Type} düşmanına {dmg:0} hasar verdi! Kalan HP: {e.Health:0} (Konum: {e.X},{e.Y})");
                }
            }

            FireCooldown = FireRate;
        }
    }
}