using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp2.src.Interfaces
{
    public interface ITower
    {
        int X { get; set; }                          
        int Y { get; set; }
        int Range { get; }                          
        int Cost { get; }                            
        float Damage { get; }                    
        float FireRate { get; }                      
        float FireCooldown { get; set; }                        
    }
}
