using System.Collections.Generic;
using System.Linq;
using static TOHE.Options;
using static TOHE.Translator;

namespace TOHE.Roles.Crewmate;

public static class Divinator
{
    private static readonly int Id = 6700;
    private static List<byte> playerIdList = [];

    public static OptionItem CheckLimitOpt;
    public static OptionItem AccurateCheckMode;
    public static OptionItem HideVote;
    public static OptionItem ShowSpecificRole;
    public static OptionItem AbilityUseGainWithEachTaskCompleted;
    public static OptionItem AbilityChargesWhenFinishedTasks;
    public static OptionItem CancelVote;

    public static List<byte> didVote = [];

    private static Dictionary<byte, List<CustomRoles>> AllPlayerRoleList = [];
    private const int RolesPerCategory = 5;

    public static void SetupCustomOption()
    {
        SetupRoleOptions(Id, TabGroup.CrewmateRoles, CustomRoles.Divinator);
        CheckLimitOpt = IntegerOptionItem.Create(Id + 10, "DivinatorSkillLimit", new(0, 20, 1), 1, TabGroup.CrewmateRoles, false).SetParent(CustomRoleSpawnChances[CustomRoles.Divinator])
            .SetValueFormat(OptionFormat.Times);
        AccurateCheckMode = BooleanOptionItem.Create(Id + 12, "AccurateCheckMode", false, TabGroup.CrewmateRoles, false).SetParent(CustomRoleSpawnChances[CustomRoles.Divinator]);
        ShowSpecificRole = BooleanOptionItem.Create(Id + 13, "ShowSpecificRole", false, TabGroup.CrewmateRoles, false).SetParent(CustomRoleSpawnChances[CustomRoles.Divinator]);
        HideVote = BooleanOptionItem.Create(Id + 14, "DivinatorHideVote", false, TabGroup.CrewmateRoles, false).SetParent(CustomRoleSpawnChances[CustomRoles.Divinator]);
        AbilityUseGainWithEachTaskCompleted = FloatOptionItem.Create(Id + 15, "AbilityUseGainWithEachTaskCompleted", new(0f, 5f, 0.1f), 1f, TabGroup.CrewmateRoles, false).SetParent(CustomRoleSpawnChances[CustomRoles.Divinator])
            .SetValueFormat(OptionFormat.Times);
        AbilityChargesWhenFinishedTasks = FloatOptionItem.Create(Id + 16, "AbilityChargesWhenFinishedTasks", new(0f, 5f, 0.1f), 0.2f, TabGroup.CrewmateRoles, false)
            .SetParent(CustomRoleSpawnChances[CustomRoles.Divinator])
            .SetValueFormat(OptionFormat.Times);
        CancelVote = CreateVoteCancellingUseSetting(Id + 11, CustomRoles.Divinator, TabGroup.CrewmateRoles);
        OverrideTasksData.Create(Id + 21, TabGroup.CrewmateRoles, CustomRoles.Divinator);
    }
    public static void Init()
    {
        playerIdList = [];
        AllPlayerRoleList = [];
    }
    public static void Add(byte playerId)
    {
        playerIdList.Add(playerId);
        playerId.SetAbilityUseLimit(CheckLimitOpt.GetInt());

        List<CustomRoles> AllRoles = [.. EnumHelper.GetAllValues<CustomRoles>().Where(x => !x.IsAdditionRole() && x is not CustomRoles.Killer and not CustomRoles.Tasker and not CustomRoles.KB_Normal and not CustomRoles.Potato)];
        var r = IRandom.Instance;

        foreach (var pc in Main.AllAlivePlayerControls)
        {
            AllPlayerRoleList[pc.PlayerId] = [];
            if (pc.PlayerId == playerId) continue;
            for (int i = 0; i < RolesPerCategory; i++)
            {
                if (AllRoles.Count == 0)
                {
                    Logger.Error("Not enough roles", "Divinator.Add");
                    return;
                }
                CustomRoles role = AllRoles[r.Next(0, AllRoles.Count)];
                AllPlayerRoleList[pc.PlayerId].Add(role);
                AllRoles.Remove(role);
            }
        }
    }
    public static bool IsEnable => playerIdList.Count > 0;

    public static bool OnVote(PlayerControl player, PlayerControl target)
    {
        if (player == null || target == null) return false;
        if (didVote.Contains(player.PlayerId) || Main.DontCancelVoteList.Contains(player.PlayerId)) return false;
        didVote.Add(player.PlayerId);

        if (player.GetAbilityUseLimit() < 1)
        {
            Utils.SendMessage(GetString("DivinatorCheckReachLimit"), player.PlayerId, Utils.ColorString(Utils.GetRoleColor(CustomRoles.Divinator), GetString("DivinatorCheckMsgTitle")));
            return false;
        }

        player.RpcRemoveAbilityUse();

        if (player.PlayerId == target.PlayerId)
        {
            Utils.SendMessage(GetString("DivinatorCheckSelfMsg") + "\n\n" + string.Format(GetString("DivinatorCheckLimit"), player.GetAbilityUseLimit()), player.PlayerId, Utils.ColorString(Utils.GetRoleColor(CustomRoles.Divinator), GetString("DivinatorCheckMsgTitle")));
            return false;
        }

        string msg;

        if ((player.AllTasksCompleted() || AccurateCheckMode.GetBool()) && ShowSpecificRole.GetBool())
        {
            msg = string.Format(GetString("DivinatorCheck.TaskDone"), target.GetRealName(), GetString(target.GetCustomRole().ToString()));
        }
        else
        {
            string roles = string.Join(", ", AllPlayerRoleList[target.PlayerId].Select(x => GetString(x.ToString())));
            msg = string.Format(GetString("DivinatorCheckResult"), target.GetRealName(), roles);
        }

        Utils.SendMessage(GetString("DivinatorCheck") + "\n" + msg + "\n\n" + string.Format(GetString("DivinatorCheckLimit"), player.GetAbilityUseLimit()), player.PlayerId, Utils.ColorString(Utils.GetRoleColor(CustomRoles.Divinator), GetString("DivinatorCheckMsgTitle")));

        Main.DontCancelVoteList.Add(player.PlayerId);
        return true;
    }
}