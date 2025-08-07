using System.Collections.Generic;

namespace ZooBuilder.ui.achievement.AchievementsList;

public class AchievementLineTraveled : IAchievementLine
{
    private readonly List<string> _achievementNamesOrdered =
    [
        "ACH_TRAVEL_FAR_SINGLE",
        "ACH_TRAVEL_FAR_ACCUM"
    ];
    
    public AchievementLineKey Key()
    {
        return AchievementLineKey.DistanceTraveled;
    }

    public List<string> AchievementNamesOrdered()
    {
        return _achievementNamesOrdered;
    }
}