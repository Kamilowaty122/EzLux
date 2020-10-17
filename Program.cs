﻿namespace Template {
  // namespace
  using System;
  using System.Linq;
  using EnsoulSharp;
  using EnsoulSharp.SDK;
  using EnsoulSharp.SDK.Events;
  using EnsoulSharp.SDK.MenuUI;
  using EnsoulSharp.SDK.MenuUI.Values;
  using EnsoulSharp.SDK.Prediction;
  using EnsoulSharp.SDK.Utility;
  using SharpDX;
  using Color = System.Drawing.Color;
  using System.Collections.Generic;
  using SharpDX.Direct3D9;
  using static EnsoulSharp.SDK.Prediction.SpellPrediction;
  using Keys = System.Windows.Forms.Keys;


  public class Program {
    private static Menu MainMenu;
    private static Spell Q,W,E,R,QTrue;
    private static GameObject _eGameObject;
    public static Menu drawingMenu, comboMenu, miscMenu, clearMenu, harassMenu;

    private static void Main(string[] args) {
      // start init script when game already load
      GameEvent.OnGameLoad += OnGameLoad;
    }

    private static void OnGameLoad() {
      // judge champion Name
      if (ObjectManager.Player.CharacterName != "Lux") {
        Console.WriteLine("You ain't lux mate");
        return;
      }
      Console.WriteLine("You lux mate");

      Q = new Spell(SpellSlot.Q, 1175);
      W = new Spell(SpellSlot.W, 1075);
      E = new Spell(SpellSlot.E, 1100);
      R = new Spell(SpellSlot.R, 3340);
      QTrue = new Spell(SpellSlot.Q, 1175);


      Q.SetSkillshot(0.25f, 80, 1200, false, SkillshotType.Line);
      W.SetSkillshot(0.25f, 150, 1200, false, SkillshotType.Line);
      E.SetSkillshot(0.25f, 275, 1300, false, SkillshotType.Circle);
      R.SetSkillshot(1.375f, 190, 3000, false, SkillshotType.Line);
      QTrue.SetSkillshot(0.25f, 80, 1200, true, SkillshotType.Line);

      SetupMenu();
      // events
      Game.OnUpdate += OnUpdate;
      Drawing.OnDraw += OnDraw;
    }

        private static bool HasPassive(AIBaseClient target)
        {
            return target.HasBuff("luxilluminatingfraulein");
        }

    private static void Combo() {
      // cast q (is skillshot spell)
      // if menuitem enabled + Q ready
      if (MainMenu["Combo"]["comboQ"].GetValue < MenuBool > ().Enabled && Q.IsReady()) {
        // get target
        foreach(var target in GameObjects.EnemyHeroes.Where(x =>x.IsValidTarget(Q.Range) && !x.IsInvulnerable)) {
          // judge target is valid
          if (target != null && target.IsValidTarget(Q.Range)) {
            
            // get pred
            var minpred = QTrue.GetPrediction(target, true); //fuck this qtrue shit 
            var pred = Q.GetPrediction(target, true);
            //var minions = pred.AoeTargetsHitCount;
            var minions = minpred.CollisionObjects.Count(thing => thing.IsMinion || thing.IsMonster);

            
            //Console.WriteLine("minions " + minions); //debug shit

            if (pred.Hitchance >= HitChance.High && minions<2) {
              // cast skillshot
              Q.Cast(pred.CastPosition);
            }
            
            //}
          }

        }
      }

      // cast w (is charge spell)
      // if menuitem enabled + Q ready
      if (MainMenu["Combo"]["comboW"].GetValue < MenuBool > ().Enabled && W.IsReady()) {
        // get target
        var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
        // or like this



        // both work for get target

          // get pred

            // cast skillshot
            W.Cast(target);
          
        
      
      }
      // cast e (is selfcast spell)
      // if menuitem enabled + E ready
      if (MainMenu["Combo"]["comboE"].GetValue < MenuBool > ().Enabled && E.IsReady()) {
        
        
        if (_eGameObject != null && E.IsReady()) //pop e
            {
              E.Cast();
            }

        
        
        
        // get target
        var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
        // or like this
        target = TargetSelector.GetTarget(E.Range);
        // both work for get target
        // judge target is valid
        if (target != null && target.IsValidTarget(E.Range)) {
          // get pred
          var pred = E.GetPrediction(target);
          if (pred.Hitchance >= HitChance.High) {
            // cast skillshot
            E.Cast(pred.CastPosition);
          }
        }
      }

      // cast r (is target spell)
      // if menuitem enabled + Q ready
      if (MainMenu["Combo"]["comboR"].GetValue < MenuBool > ().Enabled && R.IsReady()) {
        //var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
        if (MainMenu["Combo"]["Rkill"].GetValue < MenuBool > ().Enabled) {
          foreach(var target in GameObjects.EnemyHeroes.Where(x =>x.IsValidTarget(R.Range) && !x.IsInvulnerable)) {
            if (target.IsValidTarget(R.Range) && target.Health < R.GetDamage(target)) {
              var Pred = R.GetPrediction(target);
              if (Pred.Hitchance >= HitChance.High) {
                R.Cast(Pred.CastPosition);
              }
            }
          }
        }
        else {
          foreach(var target in GameObjects.EnemyHeroes.Where(x =>x.IsValidTarget(R.Range) && !x.IsInvulnerable)) {
            if (target.IsValidTarget(R.Range)) {
              var Pred = R.GetPrediction(target);
              if (Pred.Hitchance >= HitChance.High) {
                R.Cast(Pred.CastPosition);
              }
            }
          }

        }

      }
    }

    private static void Clear() 
    {
      // check out Ashe or Kalista
      // it already have example
      // get Minion
      //var minions = GameObjects.EnemyMinions.Where(x =>x.IsValidTarget(Q.Range) && x.IsMinion());

      // get Mob
      //var mobs = GameObjects.Jungle.Where(x =>x.IsValidTarget(Q.Range));

      // get Legendary Mob (Dragon, Baron, ect)
      //var lMobs = GameObjects.Jungle.Where(x =>x.IsValidTarget(Q.Range) && x.GetJungleType() == JungleType.Legendary);

      // get Large Mob (Red Buff, Blue Buff, ect)
      //var bMobs = GameObjects.Jungle.Where(x =>x.IsValidTarget(Q.Range) && x.GetJungleType() == JungleType.Large);
      
      
      //var minionshit = MainMenu["Clear"]["EMinMana"].GetValue<MenuSlider>().Value;
      //Console.WriteLine(minionshit);




      if(MainMenu["Clear"]["EMinion"].GetValue < MenuBool > ().Enabled)
      {
        var minions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(E.Range) && x.IsMinion()).Cast<AIBaseClient>().ToList();

        if (_eGameObject != null && E.IsReady()  ) //pop e    _eGameObject.BoundingRadius
              {
                E.Cast();
              }
      
        if(MainMenu["Clear"]["EMinMana"].GetValue<MenuSlider>().Value < ObjectManager.Player.ManaPercent )
        {
          if(minions.Any())
          {
            var EFarmLocation = E.GetCircularFarmLocation(minions);
            if(EFarmLocation.MinionsHit >= MainMenu["Clear"]["EMinionCount"].GetValue<MenuSlider>().Value)
            {
              E.Cast(EFarmLocation.Position);
              return;
            }
          }
       }
      }//koniec EMinion

     if(MainMenu["Clear"]["QMinion"].GetValue < MenuBool > ().Enabled)
     {
       if(MainMenu["Clear"]["EMinMana"].GetValue<MenuSlider>().Value < ObjectManager.Player.ManaPercent )
       {
         var minions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(E.Range) && x.IsMinion()).Cast<AIBaseClient>().ToList();
         var QFarmLocation = Q.GetLineFarmLocation(minions);
         Q.Cast(QFarmLocation.Position);
       }

     }


    if(MainMenu["Clear"]["EJungle"].GetValue < MenuBool > ().Enabled)
      {
        var mobs = GameObjects.Jungle.Where(x => x.IsValidTarget(E.Range) && x.IsJungle()).Cast<AIBaseClient>().ToList();

        if (_eGameObject != null && E.IsReady()  ) //pop e    _eGameObject.BoundingRadius
              {
                E.Cast();
              }
      
        if(MainMenu["Clear"]["EMinMana"].GetValue<MenuSlider>().Value < ObjectManager.Player.ManaPercent )
        {
          if(mobs.Any())
          {
            var EFarmLocation = E.GetCircularFarmLocation(mobs);

              E.Cast(EFarmLocation.Position);
              return;

          }
       }
      }//koniec E jugnle

     if(MainMenu["Clear"]["QJungle"].GetValue < MenuBool > ().Enabled)
     {
       if(MainMenu["Clear"]["EMinMana"].GetValue<MenuSlider>().Value < ObjectManager.Player.ManaPercent )
       {
         var mobs = GameObjects.Jungle.Where(x => x.IsValidTarget(E.Range) && x.IsJungle()).Cast<AIBaseClient>().ToList();
         var QFarmLocation = Q.GetLineFarmLocation(mobs);
         Q.Cast(QFarmLocation.Position);
       }

     }


    }//koniec clear

    private static void Harass() 
    {
      if (MainMenu["Harass"]["QHarass"].GetValue < MenuBool > ().Enabled && Q.IsReady()) 
      {
        // get target
        foreach(var target in GameObjects.EnemyHeroes.Where(x =>x.IsValidTarget(Q.Range) && !x.IsInvulnerable)) 
        {
          // judge target is valid
          if (target != null && target.IsValidTarget(Q.Range)) 
          {
            
            // get pred
            var minpred = QTrue.GetPrediction(target, true); //fuck this qtrue shit 
            var pred = Q.GetPrediction(target, true);
            //var minions = pred.AoeTargetsHitCount;
            var minions = minpred.CollisionObjects.Count(thing => thing.IsMinion || thing.IsMonster);

            
            //Console.WriteLine("minions " + minions); //debug shit

            if (pred.Hitchance >= HitChance.High && minions<2) 
            {
              // cast skillshot
              Q.Cast(pred.CastPosition);
            }
            
            //}
          }

        }
      }

        if (MainMenu["Harass"]["EHarass"].GetValue < MenuBool > ().Enabled && E.IsReady()) {
        
        
         if (_eGameObject != null && E.IsReady()) //pop e
             {
               E.Cast();
             }

        
        
        
        // get target
        var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
        // or like this
        target = TargetSelector.GetTarget(E.Range);
        // both work for get target
        // judge target is valid
        if (target != null && target.IsValidTarget(E.Range)) {
          // get pred
          var pred = E.GetPrediction(target);
          if (pred.Hitchance >= HitChance.High) {
            // cast skillshot
            E.Cast(pred.CastPosition);
          }
        }
      }

        if (MainMenu["Harass"]["RHarass"].GetValue < MenuBool > ().Enabled && R.IsReady()) {
        //var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
          foreach(var target in GameObjects.EnemyHeroes.Where(x =>x.IsValidTarget(R.Range) && !x.IsInvulnerable)) {
            if (target.IsValidTarget(R.Range)) {
              var Pred = R.GetPrediction(target);
              if (Pred.Hitchance >= HitChance.High) {
                R.Cast(Pred.CastPosition);
              }
            }
          }

        }

      

    }//koniec harass
    private static void OnUpdate(EventArgs args) {
      KillSecure();

      switch (Orbwalker.ActiveMode) {
      case OrbwalkerMode.Combo:
        Combo();
        break;
      case OrbwalkerMode.LaneClear:
        Clear();
        break;
      case OrbwalkerMode.Harass:
        Harass();
        break;
      }
    } //koniec onUpdate

    private static void OnDraw(EventArgs args) {
      // if Player is Dead not Draw Range
      if (MainMenu["isDead"].GetValue < MenuBool > ().Enabled) {
        if (ObjectManager.Player.IsDead) {
          return;
        }
      }

      // draw Q Range
      if (MainMenu["Draw"]["drawQ"].GetValue < MenuBool > ().Enabled) {
        // Draw Circle
        Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Aqua);
      }
      // draw W Range
      if (MainMenu["Draw"]["drawW"].GetValue < MenuBool > ().Enabled) {
        // Draw Circle
        Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Aqua);
      }
      // draw E Range
      if (MainMenu["Draw"]["drawE"].GetValue < MenuBool > ().Enabled) {
        // Draw Circle
        Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Aqua);
      }
      // draw R Range
      if (MainMenu["Draw"]["drawR"].GetValue < MenuBool > ().Enabled) {
        // Draw Circle
        Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Aqua);
      }

    }

    private static void KillSecure()
    {   
          foreach(var target in GameObjects.EnemyHeroes.Where(x =>!x.IsInvulnerable)) {
            if((MainMenu["Misc"]["RKillsteal"].GetValue < MenuBool > ().Enabled)){
            if (target.IsValidTarget(R.Range) && target.Health < R.GetDamage(target)) {
              var Pred = R.GetPrediction(target);
              if (Pred.Hitchance >= HitChance.High) {
                R.Cast(Pred.CastPosition);
              }
            }
            }
            if((MainMenu["Misc"]["EKillsteal"].GetValue < MenuBool > ().Enabled)){
            if (target.IsValidTarget(E.Range) && target.Health < E.GetDamage(target)) {
              var Pred = E.GetPrediction(target);
              if (Pred.Hitchance >= HitChance.High) {
                E.Cast(Pred.CastPosition);
              }
            }
            }
            if((MainMenu["Misc"]["QKillsteal"].GetValue < MenuBool > ().Enabled)){
            if (target.IsValidTarget(Q.Range) && target.Health < Q.GetDamage(target)) {
              var Pred = Q.GetPrediction(target);
              if (Pred.Hitchance >= HitChance.High) {
                Q.Cast(Pred.CastPosition);
              }
            }
            }
            }//koniec foreacha
        }//koniec ks

    private static void SetupMenu() {
      
      // create menu
      MainMenu = new Menu("Template", "EzLux", true);

      // combo menu
      var comboMenu = new Menu("Combo", "Combo Settings");
      comboMenu.Add(new MenuBool("comboQ", "Use Q", true));
      comboMenu.Add(new MenuBool("comboW", "Use W (bugged like shit dont use, use W maunal, I cant be bothered to fix that shit)", false));
      comboMenu.Add(new MenuBool("comboE", "Use E", true));
      comboMenu.Add(new MenuBool("comboR", "Use R", true));
      comboMenu.Add(new MenuBool("Rkill", "Use R Only to kill", true));
      MainMenu.Add(comboMenu);

      // draw menu 
      var drawMenu = new Menu("Draw", "Draw Settings");
      drawMenu.Add(new MenuBool("drawQ", "Draw Q Range", true));
      drawMenu.Add(new MenuBool("drawW", "Draw W Range", false));
      drawMenu.Add(new MenuBool("drawE", "Draw E Range", true));
      drawMenu.Add(new MenuBool("drawR", "Draw R Range", true));
      MainMenu.Add(drawMenu);

      //misc menu
      var miscMenu = new Menu("Misc", "Misc Settings");
      miscMenu.Add(new MenuBool("QKillsteal", "Kill Secure with Q", true));
      miscMenu.Add(new MenuBool("EKillsteal", "Kill Secure with E", true));
      miscMenu.Add(new MenuBool("RKillsteal", "Kill Secure with R", true));
      //miscMenu.AddKey("R", "SemiRKey", "Semi-manual R Key", Keys.T, KeyBindType.Press);
      MainMenu.Add(miscMenu);

      //Lane clear und Jungle Clear
      var clearMenu = new Menu("Clear", "Clear Settings");
      clearMenu.Add(new MenuSlider("EMinMana", "Clear if Your Mana% >=", 30,0,100));
      clearMenu.Add(new MenuBool("EMinion", "Use E on minion clear", true));
 
      clearMenu.Add(new MenuSlider("EMinionCount", "Use E if hits >= minions", 3,0,6));
      clearMenu.Add(new MenuBool("QMinion", "Use Q on minion clear", false));
      
      clearMenu.Add(new MenuSlider("Seperator lul", "JUNGLE CLEAR", 0,0,0));
      
      clearMenu.Add(new MenuBool("EJungle", "Use E on Jungle clear", true));
      clearMenu.Add(new MenuBool("QJungle", "Use Q on Jungle clear", false));
      
      
      MainMenu.Add(clearMenu);
      
      //harass menu
      var harassMenu = new Menu("Harass", "Harass Settings");
      
      harassMenu.Add(new MenuBool("QHarass", "Use Q on Harass", false));
      harassMenu.Add(new MenuBool("EHarass", "Use E on Harass", true));
      harassMenu.Add(new MenuBool("RHarass", "Use R on Harass xd", true));

      MainMenu.Add(harassMenu);
      //example boolean on MainMenu
      MainMenu.Add(new MenuBool("isDead", "if Player is Dead not Draw Range", true));

      // init MainMenu
      MainMenu.Attach();

    }
  }
}