using System;


namespace Game.Handlers.Game.Ingame
{
    class Heal : Networking.GameDataHandler
    {
        protected override void Handle()
        {
            byte targetSlot = GetByte(2); // The target 
            bool _healTower = GetBool(3);
            bool heal       = GetBool(5); // If user needed to be healed or killed
            ushort _healAmount = 0;

            if(targetSlot >= 0 && targetSlot  <= Room.MaximumPlayers)
            {
                Entities.Player Target = null;

                try
                {
                    Room.Players.TryGetValue(targetSlot, out Target);
                }
                catch
                {
                    Target = null;
                    ServerLogger.Instance.Append(ServerLogger.AlertLevel.ServerError, "Cannot heal player " + targetSlot.ToString() + " player not found");
                }
                
                if(Target != null)
                {
                        
                  if(Target.Health < 1000)
                    {
                        if(Target.Team == Player.Team)
                        {
                            if (Room.Mode == Enums.Mode.Free_For_All)
                                Target = Player;
                            else
                            {
                                if (Player != Target)
                                    Player.AddAssists(1);
                            }

                            if (_healTower)
                                _healAmount = 400;
                            else
                            {
                                if (Player.Weapon == 65) //Adrenaline //ITEM ID IS A BAD WAY TO DO SO AS THEY MAY CHANGE BETWEEN PATCHES
                                {
                                    if (Target.Health < 400)
                                        _healAmount = (ushort)(400 - Target.Health);
                                    else
                                        _healAmount = 100;
                                }
                                else
                                    _healAmount = GetHealAmount(Player.Weapon);      
                            }

                            Target.Health += _healAmount;

                            if (Target.Health > 1000)
                                Target.Health = 1000;

                            Set(3, Target.Health);
                            respond = true;
                        }
                    }             
 
                }      
           }
            
        }

        ushort GetHealAmount(int _weaponIndex)
        {
            ushort _healAmount = 0;

            switch(_weaponIndex) //ITEM ID IS A BAD WAY TO DO SO AS THEY MAY CHANGE BETWEEN PATCHES
            {

                case 60:                  //Med.Kit 1
                    _healAmount = 300;
                    break;
                case 61:                   //Med.Kit 2
                    _healAmount = 450;
                    break;
                case 62:                    //Med. Kit 3
                    _healAmount = 600;
                    break;
                //case 85:                   // hpKit //Is it even in game in this version?
                //    _healAmount = 200;
                //    break;
                default:
                    _healAmount = 0;
                    break;
            }

            return _healAmount;
        }

    }
}
