using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsApp2.src.Models;
using WinFormsApp2.src.Interfaces;

namespace WinFormsApp2.src.Controllers
{
    public class TowerController
    {
        public List<ITower> Towers { get; private set; } = new();


        public Response AddTower(ITower tower)
        {
            if (tower.Cost <= Form1.money)
            {
                Towers.Add(tower);
                Form1.money -= tower.Cost;

                Response response = new Response();
                response.success = true;
                response.message = "Kule eklendi.";
                return response;
            }
            else
            {
                Response response = new Response();
                response.success = false;
                response.message = "Kule eklenemedi. Para yetersiz!";
                return response;
            }
        }
    }
}
