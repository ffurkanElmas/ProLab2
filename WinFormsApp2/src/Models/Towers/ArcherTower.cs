using System;
using System.Collections.Generic;
using System.Linq;
using WinFormsApp2.src.Interfaces;

namespace WinFormsApp2.src.Models.Towers
{
    public class ArcherTower : ITower
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int Range => 4;
        public int Cost => 50;
        public float Damage => 20f;
        public float FireRate => 1f;
        public float FireCooldown { get; set; } = 0;

        public ArcherTower(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool CanAttack(IEnemy enemy) => true; // Tüm düşmanlar

        public void Attack(List<IEnemy> enemies)
        {
            if (FireCooldown > 0) return;

            var targets = enemies
                .Where(e => !e.IsDead &&
                            Math.Abs(e.X - X) <= Range &&
                            Math.Abs(e.Y - Y) <= Range)
                .ToList();

            if (targets.Count == 0) return;

            foreach (var e in targets)
            {
                float dmg = Damage;

                if (e.IsArmored)
                    dmg *= 0.5f; // Zırhlıya %50 az

                // Hasar formülü
                dmg *= 1 - (100f / (100f + 100f)); // Zırh = 100 varsayım

                e.Health -= dmg;
                if (e.Health < 0) e.Health = 0;
            }

            FireCooldown = FireRate;
        }
    }
}
