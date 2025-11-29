using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsApp2.src.Interfaces;

namespace WinFormsApp2.src.Models.Towers
{
    public class IceTower : ITower
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int Range => 4;
        public int Cost => 60;
        public float Damage => 10f;
        public float FireRate => 2f;
        public float FireCooldown { get; set; } = 0;

        public IceTower(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool CanAttack(IEnemy enemy)
        {
            return true; // Hepsine saldırır
        }

        public void Attack(List<IEnemy> enemies)
        {
            if (FireCooldown > 0) return;

            var hedefler = enemies
                .Where(e => !e.IsDead &&
                            Math.Abs(e.X - X) <= Range &&
                            Math.Abs(e.Y - Y) <= Range)
                .ToList();

            if (hedefler.Count == 0)
                return;

            // Üsse en yakın hedef
            var hedef = hedefler.OrderByDescending(e => e.X).First();

            //hedef.TakeDamage(Damage);

            // %50 slow → enemy.Speed yarıya düşürülür
            hedef.Speed *= 0.5f;

            FireCooldown = FireRate;
        }
    }
}
