using System;
using System.Collections.Generic;
using System.Linq;
using WinFormsApp2.src.Interfaces;
using WinFormsApp2.src.Services;

namespace WinFormsApp2.src.Models.Towers
{
    public class IceTower : ITower
    {
        public string Type => "Buz Kulesi";
        public int X { get; set; }
        public int Y { get; set; }
        public int Range => 4;
        public int Cost => 60;
        public float Damage => 0f; // Hasar yok
        public float FireRate => 2f;
        public float FireCooldown { get; set; } = 0;

        public IceTower(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool CanAttack(IEnemy enemy) => true;

        public void Attack(List<IEnemy> enemies)
        {
            if (FireCooldown > 0) return;

            var targets = enemies
                .Where(e => !e.IsDead &&
                            Math.Abs(e.X - X) <= Range &&
                            Math.Abs(e.Y - Y) <= Range)
                .ToList();

            if (targets.Count == 0) return;

            var e = targets.OrderByDescending(t => t.X).First();

            e.IsSlowed = true;
            e.SlowTimer = 3f; // 3 saniye
            e.Speed = e.OriginalSpeed * 0.5f;

            LogsManager.Log($"{Type} → {e.Type} düşmanını yavaşlattı! (Konum: {e.X},{e.Y})");


            FireCooldown = FireRate;
        }
    }
}
