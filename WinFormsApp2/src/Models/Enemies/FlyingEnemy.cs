using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsApp2.src.Interfaces;
using WinFormsApp2.src.Enums;

namespace WinFormsApp2.src.Models.Enemies
{
    public class FlyingEnemy : IEnemy
    {
        public int X { get; set; }
        public int Y { get; set; }
        public float Health { get; set; } = 50;
        public float Speed { get; set; } = 1.5f;
        public bool IsFlying => true;
        public bool IsArmored => false;
        public bool IsDead => Health <= 0;
        public int RewardOnDeath => 15;
        public int DamageToBase => 5;
        public bool ReachedBase { get; set; }
        public float MoveAccumulator { get; set; } = 0;
        Direction LastMove { get; set; } = Direction.Right;
        Direction IEnemy.LastMove { get => LastMove; set => LastMove = value; }
    }
}
