using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsApp2.src.Interfaces;

namespace WinFormsApp2.src.Models.Towers
{
    public class CannonTower : ITower
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int Range => 4;
        public int Cost => 75;
        public float Damage => 40f;       // Splash hasar
        public float FireRate => 3f;
        public float FireCooldown { get; set; } = 0;

        private int splashRadius = 2;     // Kare bazlı splash area

        public CannonTower(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool CanAttack(IEnemy enemy)
        {
            return !enemy.IsFlying; // Uçan düşman HAYIR
        }

        public void Attack(List<IEnemy> enemies)
        {
            if (FireCooldown > 0) return;

            // Menzildeki hedefler → uçanlar hariç
            var hedefler = enemies
                .Where(e => !e.IsDead &&
                            !e.IsFlying &&
                            Math.Abs(e.X - X) <= Range &&
                            Math.Abs(e.Y - Y) <= Range)
                .ToList();

            if (hedefler.Count == 0)
                return;

            // Üsse en yakın olan ana hedef
            var hedef = hedefler.OrderByDescending(e => e.X).First();

            // Splash → merkez hedef
            foreach (var e in enemies)
            {
                if (!e.IsFlying &&
                    Math.Abs(e.X - hedef.X) <= splashRadius &&
                    Math.Abs(e.Y - hedef.Y) <= splashRadius)
                {
                    //e.TakeDamage(Damage);
                }
            }

            FireCooldown = FireRate;
        }
    }
}
