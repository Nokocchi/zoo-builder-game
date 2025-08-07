using System.Collections.Generic;

namespace ZooBuilder.ui.achievement.AchievementsList;

public class AchievementLineGamesWon : IAchievementLine
{
    
    // These are the names of the achievements defined in Steam.
    // The order of this list is the order in which they should be unlocked/shown in the game
    private readonly List<string> _achievementNamesOrdered =
    [
        "ACH_WIN_ONE_GAME",
        "ACH_WIN_100_GAMES"
    ];
    
    public AchievementLineKey Key()
    {
        return AchievementLineKey.GamesWon;
    }

    public List<string> AchievementNamesOrdered()
    {
        return _achievementNamesOrdered;
    }
}