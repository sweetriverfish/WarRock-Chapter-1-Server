/*

    Project: Cristina player assistant
    Author: Santiago (Razgriz/DarkR4ptor) García
    Started on: 19-5-15
    Last Update: 30-1-16
    
    INFO: Cristina is an assistant for the player. It provides info during gameplay
 
    This is Cristina´s core, implemented as a singleton. Defines Cristina basic functions
  
  
 */
using System;
using System.IO;

namespace Game.Cristina
{
  public sealed class Core
    {
      static readonly Core _cristina = new Core();
      public static Core Cristina { get { return _cristina; } }
      
      //Cristina CORE information
      public string Name { get { return "Cristina"; } }
      public double Version = 0.4;
      
      //Cristina modules
      public Modules.Chat Chat;
      public Modules.Localization Localization;
 
      
      Core()
      {
          InitializeModules();
      }

      private void InitializeModules()
      {
               Chat = new Modules.Chat();
               Localization = new Modules.Localization();
      }

    }
}
