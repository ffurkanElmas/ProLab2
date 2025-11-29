using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsApp2.src.Interfaces;
using WinFormsApp2.src.Enums;

namespace WinFormsApp2.src.Models.Enemies
{
    public class ArmoredEnemy : IEnemy
    {
        public int X { get; set; }
        public int Y { get; set; }
        public float Health { get; set; } = 75;   
        public float Speed { get; set; } = 25;    
        public bool IsFlying => false;
        public bool IsArmored => true;
        public bool IsDead => Health <= 0;
        public int RewardOnDeath => 20;
        public int DamageToBase => 10;
        public bool ReachedBase { get; set; }
        Direction LastMove { get; set; } = Direction.Right;
        Direction IEnemy.LastMove { get => LastMove; set => LastMove = value; }
    }
}
