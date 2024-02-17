namespace FortniteBot.Data.Stats
{
    public class StatsData
    {
        public Data Data { get; set; }
    }

    public class Data
    {
        public BattlePass BattlePass { get; set; }
        public Stats Stats { get; set; }
        public string Image {  get; set; }
    }

    public class BattlePass
    {
        public int Level { get; set; } // Niveau
    }

    public class Stats
    {
        public FStats All { get; set; }
        public FStats KeyboardMouse { get; set; }
        public FStats Gamepad { get; set; }
        public FStats Touch { get; set; }
    }

    public class FStats
    {
        public OverallStats Overall { get; set; }
    }

    public class OverallStats
    {
        public int Score { get; set; }
        public int Matches { get; set; } // Parties jouées
        public int Wins { get; set; }    // Victoires
        public int Top3 {  get; set; } // TOP 3
        public int Top5 { get; set; } // TOP 5
        public int Kills { get; set; }   // Kills
        public int Deaths {  get; set; } // Deaths
        public float KD { get; set; } // Ratio Kill/Death
    }
}
