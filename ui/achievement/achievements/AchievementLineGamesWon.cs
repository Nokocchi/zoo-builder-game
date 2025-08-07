using System.Collections.Generic;

namespace ZooBuilder.ui.achievement.AchievementsList;

public class AchievementLineGamesWon : IAchievementLine
{
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