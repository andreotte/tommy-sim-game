using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TommySimBreakout.Models;
using System.Data.Entity.Migrations;

namespace TommySimBreakout.Controllers
{
    public class HomeController : Controller
    {
        public TommySimSavesEntities db = new TommySimSavesEntities();

        public ActionResult Index()
        {
            Session["GameId"] = 1;
            return View();
        }

        public ActionResult Game()
        {
            int id = (int) Session["GameId"];
            City city = db.Cities.Find(id);
            return View(city);
        }

        public ActionResult Harvest(string resource)
        {
            Random random = new Random();
            City city = db.Cities.Find(1);

            while(city.Actions > 0)
            {

                city.Actions--; 
                if(resource.ToLower() == "water")
                {
                    city.Water += random.Next(1, 6);
                }
                else if(resource.ToLower() == "wood")
                {
                    city.Wood += random.Next(1, 6);
                }
                else if(resource.ToLower() == "food")
                {
                    city.Food += random.Next(1, 5);
                }
                
                db.Cities.AddOrUpdate(city);
                db.SaveChanges();
                return RedirectToAction("Game");
            }

            return RedirectToAction("EndTurn");
        }

        public ActionResult Build(string structure)
        {
            City city = db.Cities.Find(1);

            while (city.Actions > 0)
            {
                if (structure.ToLower() == "house" && city.Wood >= 5)
                {
                    city.Actions--; 
                    city.Wood -= 5;
                    city.Houses++;
                    city.Villagers++;

                }
                else if (structure.ToLower() == "well" && city.Wood >= 6)
                {
                    city.Actions--;
                    city.Wood -= 6;
                    city.Wells += 1;
                }
                else
                {
                    //some message about them not having any actions left. Or just kicjk to next turn
                }
                db.Cities.AddOrUpdate(city);
                db.SaveChanges();
                return RedirectToAction("Game");
            }

            return RedirectToAction("EndTurn");

        }

        public ActionResult EndTurn()
        {
            City city = db.Cities.Find(1);

            //Add water
            for (int i = 0; i < city.Wells; i++)
            {
                city.Water++;
            }

            //Kill off villager if food or water run out.
            if (city.Water <= 0 || city.Food <= 0)
            {
                city.Houses--;
                city.Villagers--;
            }

            //Add actions
            for (int i = 0; i < city.Villagers; i++)
            {
                city.Actions++;
            }

            //each villager drinks and eats to start the day
            for (int i = 0; i < city.Villagers; i++)
            {
                city.Water--;
                city.Food--;
            }

            city.Day++;
            db.Cities.AddOrUpdate(city);
            db.SaveChanges();
            return RedirectToAction ("Game", city);
        }

       
    }
}