using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsApp2.src.Models;
using WinFormsApp2.src.Interfaces;
using WinFormsApp2.src.Services;

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
                LogsManager.Log($"Kule eklendi → Tür: {tower.Type}, Konum: ({tower.X},{tower.Y}), Maliyet: {tower.Cost}");
                return response;
            }
            else
            {
                Response response = new Response();
                response.success = false;
                response.message = "Kule eklenemedi. Para yetersiz!";
                LogsManager.Log($"Kule eklenemedi → Tür: {tower.Type}, Maliyet: {tower.Cost}, Mevcut Para: {Form1.money}"); 
                return response;
            }
        }
    }
}
