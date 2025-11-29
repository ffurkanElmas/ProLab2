using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsApp2.src.Interfaces;

namespace WinFormsApp2.src.Models.Towers
{
    public class ArcherTower : ITower
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int Range => 4;             // 9x9 kare = 4 yarıçap
        public int Cost => 50;
        public float Damage => 20f;        // Temel hasar örnek
        public float FireRate => 1f;       // 1 saniye
        public float FireCooldown { get; set; } = 0;

        public ArcherTower(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool CanAttack(IEnemy enemy)
        {
            return true; // Uçan / zırhlı / standart → hepsine saldırır
        }

        public void Attack(List<IEnemy> enemies)
        {
            if (FireCooldown > 0) return;

            // Menzildeki tüm düşmanları bul
            var hedefler = enemies
                .Where(e => !e.IsDead &&
                            Math.Abs(e.X - X) <= Range &&
                            Math.Abs(e.Y - Y) <= Range)
                .ToList();

            if (hedefler.Count == 0)
                return;

            // Üsse en yakın olan düşman: X en büyük olan
            var hedef = hedefler.OrderByDescending(e => e.X).First();

            float dmg = Damage;

            if (hedef.IsArmored)
                dmg *= 0.5f;   // Zırhlıya %50 az hasar

            //hedef.TakeDamage(dmg);

            FireCooldown = FireRate;
        }
    }
}
