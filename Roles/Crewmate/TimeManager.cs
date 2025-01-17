using System.Collections.Generic;

namespace EHR.Roles.Crewmate;

public class TimeManager : RoleBase
{
    private const int Id = 8200;
    public static List<byte> playerIdList = [];
    public static OptionItem IncreaseMeetingTime;
    public static OptionItem MeetingTimeLimit;
    public static OptionItem MadMinMeetingTimeLimit;

    public static void SetupCustomOption()
    {
        Options.SetupRoleOptions(Id, TabGroup.CrewmateRoles, CustomRoles.TimeManager);
        IncreaseMeetingTime = IntegerOptionItem.Create(Id + 10, "TimeManagerIncreaseMeetingTime", new(1, 30, 1), 5, TabGroup.CrewmateRoles).SetParent(Options.CustomRoleSpawnChances[CustomRoles.TimeManager])
            .SetValueFormat(OptionFormat.Seconds);
        MeetingTimeLimit = IntegerOptionItem.Create(Id + 11, "TimeManagerLimitMeetingTime", new(100, 500, 10), 150, TabGroup.CrewmateRoles).SetParent(Options.CustomRoleSpawnChances[CustomRoles.TimeManager])
            .SetValueFormat(OptionFormat.Seconds);
        MadMinMeetingTimeLimit = IntegerOptionItem.Create(Id + 12, "MadTimeManagerLimitMeetingTime", new(5, 150, 5), 50, TabGroup.CrewmateRoles).SetParent(Options.CustomRoleSpawnChances[CustomRoles.TimeManager])
            .SetValueFormat(OptionFormat.Seconds);
        Options.OverrideTasksData.Create(Id + 13, TabGroup.CrewmateRoles, CustomRoles.TimeManager);
    }

    public override void Init()
    {
        playerIdList = [];
    }

    public override void Add(byte playerId)
    {
        playerIdList.Add(playerId);
    }

    public override bool IsEnable => playerIdList.Count > 0;

    private static int AdditionalTime(byte id)
    {
        var pc = Utils.GetPlayerById(id);
        return playerIdList.Contains(id) && pc.IsAlive() ? IncreaseMeetingTime.GetInt() * pc.GetTaskState().CompletedTasksCount : 0;
    }
    public static int TotalIncreasedMeetingTime()
    {
        int sec = 0;
        foreach (byte playerId in playerIdList.ToArray())
        {
            if (Utils.GetPlayerById(playerId).Is(CustomRoles.Madmate)) sec -= AdditionalTime(playerId);
            else sec += AdditionalTime(playerId);
        }
        Logger.Info($"{sec}second", "TimeManager.TotalIncreasedMeetingTime");
        return sec;
    }
}