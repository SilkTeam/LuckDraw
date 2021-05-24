﻿using LuckDraw.Handles;
using LuckDraw.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace LuckDraw.Controllers
{
    public class LuckController : BaseController
    {
        private CoreEntities EF { get; }
        public LuckController(CoreEntities _ef) : base(_ef)
        {
            EF = _ef;
        }

        public IActionResult Index()
        {
            return View(EF.Lucks.Where(x => x.SignID == HttpContext.Session.GetModel<Sign>("User").ID).ToList());
        }

        [HttpGet]
        public IActionResult Add(int ParentID)
        {
            return View(ParentID);
        }
        [HttpPost]
        public IActionResult Add(Luck luck)
        {
            try
            {
                luck.SignID = HttpContext.Session.GetModel<Sign>("User").ID;
                EF.Lucks.Add(luck);
                EF.SaveChanges();
                return Content("success");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult MulitAdd(int ID, string Text)
        {
            var list = Text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var userID = HttpContext.Session.GetModel<Sign>("User").ID;
            try
            {
                foreach (var item in list)
                {
                    var luck = new Luck
                    {
                        Name = item,
                        Description = item,
                        ParentID = ID,
                        SignID = userID,
                        Weigh = 1,
                    };
                    EF.Lucks.Add(luck);
                }
                EF.SaveChanges();
                return Content("success");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Edit(int ID)
        {
            return View(EF.Lucks.FirstOrDefault(x => x.ID == ID));
        }
        [HttpPost]
        public IActionResult Edit(Luck luck)
        {
            var mod = EF.Lucks.FirstOrDefault(x => x.ID == luck.ID && x.SignID == HttpContext.Session.GetModel<Sign>("User").ID);
            if (mod == null)
                return Content("数据请求异常");

            try
            {
                mod.Name = luck.Name;
                mod.Description = luck.Description;
                mod.Weigh = luck.Weigh;
                EF.SaveChanges();
                return Content("success");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Delete(int ID)
        {
            try
            {
                var mod = EF.Lucks.FirstOrDefault(x => x.ID == ID);
                var info = EF.Lucks.Where(x => x.ParentID == mod.ID).ToList();
                if (info.Count > 0)
                {
                    foreach (var item in info)
                    {
                        var del = EF.LuckDraws.FirstOrDefault(x => x.LuckID == item.ID);
                        EF.Remove(del);
                        EF.Remove(item);
                    }
                }

                EF.Remove(mod);
                EF.SaveChanges();
                return Content("success");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}