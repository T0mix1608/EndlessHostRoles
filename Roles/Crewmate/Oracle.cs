using System.Collections.Generic;
using static EHR.Options;
using static EHR.Translator;

namespace EHR.Roles.Crewmate;

public class Oracle : RoleBase
{
    private const int Id = 7600;
    private static List<byte> playerIdList = [];

    public static OptionItem CheckLimitOpt;
    public static OptionItem HideVote;
    public static OptionItem FailChance;
    public static OptionItem OracleAbilityUseGainWithEachTaskCompleted;
    public static OptionItem AbilityChargesWhenFinishedTasks;
    public static OptionItem CancelVote;

    public static List<byte> didVote = [];

    public override bool IsEnable => playerIdList.Count > 0;

    public static void SetupCustomOption()
    {
        SetupRoleOptions(Id, TabGroup.CrewmateRoles, CustomRoles.Oracle);
        CheckLimitOpt = IntegerOptionItem.Create(Id + 10, "OracleSkillLimit", new(0, 10, 1), 0, TabGroup.CrewmateRoles).SetParent(CustomRoleSpawnChances[CustomRoles.Oracle])
            .SetValueFormat(OptionFormat.Times);
        HideVote = BooleanOptionItem.Create(Id + 12, "OracleHideVote", false, TabGroup.CrewmateRoles).SetParent(CustomRoleSpawnChances[CustomRoles.Oracle]);
        FailChance = IntegerOptionItem.Create(Id + 13, "FailChance", new(0, 100, 5), 0, TabGroup.CrewmateRoles)
            .SetParent(CustomRoleSpawnChances[CustomRoles.Oracle])
            .SetValueFormat(OptionFormat.Percent);
        OracleAbilityUseGainWithEachTaskCompleted = FloatOptionItem.Create(Id + 14, "AbilityUseGainWithEachTaskCompleted", new(0f, 5f, 0.05f), 0.2f, TabGroup.CrewmateRoles)
            .SetParent(CustomRoleSpawnChances[CustomRoles.Oracle])
            .SetValueFormat(OptionFormat.Times);
        AbilityChargesWhenFinishedTasks = FloatOptionItem.Create(Id + 15, "AbilityChargesWhenFinishedTasks", new(0f, 5f, 0.05f), 0.2f, TabGroup.CrewmateRoles)
            .SetParent(CustomRoleSpawnChances[CustomRoles.Oracle])
            .SetValueFormat(OptionFormat.Times);
        CancelVote = CreateVoteCancellingUseSetting(Id + 11, CustomRoles.Oracle, TabGroup.CrewmateRoles);
    }

    public override void Init()
    {
        playerIdList = [];
    }

    public override void Add(byte playerId)
    {
        playerIdList.Add(playerId);
        playerId.SetAbilityUseLimit(CheckLimitOpt.GetInt());
    }

    public static bool OnVote(PlayerControl player, PlayerControl target)
    {
        if (player == null || target == null) return false;
        if (didVote.Contains(player.PlayerId) || Main.DontCancelVoteList.Contains(player.PlayerId)) return false;
        didVote.Add(player.PlayerId);

        if (player.GetAbilityUseLimit() < 1)
        {
            Utils.SendMessage(GetString("OracleCheckReachLimit"), player.PlayerId, Utils.ColorString(Utils.GetRoleColor(CustomRoles.Oracle), GetString("OracleCheckMsgTitle")));
            return false;
        }

        player.RpcRemoveAbilityUse();

        if (player.PlayerId == target.PlayerId)
        {
            Utils.SendMessage(GetString("OracleCheckSelfMsg") + "\n\n" + string.Format(GetString("OracleCheckLimit"), player.GetAbilityUseLimit()), player.PlayerId, Utils.ColorString(Utils.GetRoleColor(CustomRoles.Oracle), GetString("OracleCheckMsgTitle")));
            return false;
        }

        string text;

        if (target.GetCustomRole().IsImpostor()) text = "Imp";
        else if (target.GetCustomRole().IsNeutral()) text = "Neut";
        else text = "Crew";

        if (FailChance.GetInt() > 0)
        {
            int random_number_1 = IRandom.Instance.Next(1, 101);
            if (random_number_1 <= FailChance.GetInt())
            {
                int random_number_2 = IRandom.Instance.Next(1, 3);
                switch (text)
                {
                    case "Crew":
                        if (random_number_2 == 1) text = "Neut";
                        else if (random_number_2 == 2) text = "Imp";
                        break;
                    case "Neut":
                        if (random_number_2 == 1) text = "Crew";
                        if (random_number_2 == 2) text = "Imp";
                        break;
                    case "Imp":
                        if (random_number_2 == 1) text = "Neut";
                        if (random_number_2 == 2) text = "Crew";
                        break;
                }
            }
        }

        string msg = string.Format(GetString("OracleCheck." + text), target.GetRealName());

        Utils.SendMessage(GetString("OracleCheck") + "\n" + msg + "\n\n" + string.Format(GetString("OracleCheckLimit"), player.GetAbilityUseLimit()), player.PlayerId, Utils.ColorString(Utils.GetRoleColor(CustomRoles.Oracle), GetString("OracleCheckMsgTitle")));

        Main.DontCancelVoteList.Add(player.PlayerId);
        return true;
    }
}