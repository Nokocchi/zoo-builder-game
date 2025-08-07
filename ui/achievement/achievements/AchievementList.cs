using System.Collections.Generic;

namespace ZooBuilder.ui.achievement.AchievementsList;

public class AchievementList
{
    public static List<IAchievementLine> AchievementLines { get; } =
    [
        new AchievementLineGamesWon(),
        new AchievementLineTraveled()
    ];
}