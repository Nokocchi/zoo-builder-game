using System.Collections.Generic;

namespace ZooBuilder.ui.achievement.AchievementsList;

public class AchievementList
{
    // Represents collections of achievements that are defined in Steam;
    public static List<IAchievementLine> AchievementLines { get; } =
    [
        new AchievementLineGamesWon(),
        new AchievementLineTraveled()
    ];
}