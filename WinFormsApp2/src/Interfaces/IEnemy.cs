using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WinFormsApp2.src.Enums;

namespace WinFormsApp2.src.Interfaces
{
    public interface IEnemy
    {
        int X { get; set; }
        int Y { get; set; }
        float Health { get; set; }
        float Speed { get; set; }
        bool IsFlying { get; }
        bool IsArmored { get; }
        bool IsDead { get; }
        int RewardOnDeath { get; }
        int DamageToBase { get; }
        bool ReachedBase { get; set; }
        float MoveAccumulator { get; set; }
        Direction LastMove { get; set; }
        float OriginalSpeed { get; set; }  // Başlangıç hızı
        bool IsSlowed { get; set; }        // Yavaşlama var mı
        float SlowTimer { get; set; }      // Yavaşlama süresi
    }


}
