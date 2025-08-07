using System.Collections.Generic;

namespace ZooBuilder.ui.achievement.AchievementsList;

public interface IAchievementLine
{
    AchievementLineKey Key();
    List<string> AchievementNamesOrdered();
}