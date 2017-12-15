using Core.Database;
namespace Game {
    class Databases {
        public static MySQL Game;

        public static bool Init() {
            Game = new MySQL(Config.GAME_DATABASE[0], int.Parse(Config.GAME_DATABASE[1]), Config.GAME_DATABASE[2], Config.GAME_DATABASE[3], Config.GAME_DATABASE[4]);
            return Game.Open();
        }
    }
}
