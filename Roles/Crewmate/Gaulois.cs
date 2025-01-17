﻿using System.Collections.Generic;
using AmongUs.GameOptions;
using static EHR.Options;

namespace EHR.Roles.Crewmate
{
    public class Gaulois : RoleBase
    {
        private const int Id = 643070;
        private static List<byte> playerIdList = [];

        private static OptionItem CD;
        private static OptionItem AdditionalSpeed;
        private static OptionItem UseLimitOpt;
        public static OptionItem UsePet;

        public static List<byte> IncreasedSpeedPlayerList = [];

        public static void SetupCustomOption()
        {
            SetupRoleOptions(Id, TabGroup.CrewmateRoles, CustomRoles.Gaulois);
            CD = FloatOptionItem.Create(Id + 5, "AbilityCooldown", new(0f, 60f, 2.5f), 30f, TabGroup.CrewmateRoles)
                .SetParent(CustomRoleSpawnChances[CustomRoles.Gaulois])
                .SetValueFormat(OptionFormat.Seconds);
            AdditionalSpeed = FloatOptionItem.Create(Id + 6, "GauloisSpeedBoost", new(0f, 2f, 0.05f), 0.5f, TabGroup.CrewmateRoles)
                .SetParent(CustomRoleSpawnChances[CustomRoles.Gaulois])
                .SetValueFormat(OptionFormat.Multiplier);
            UseLimitOpt = IntegerOptionItem.Create(Id + 7, "AbilityUseLimit", new(1, 14, 1), 3, TabGroup.CrewmateRoles)
                .SetParent(CustomRoleSpawnChances[CustomRoles.Gaulois])
                .SetValueFormat(OptionFormat.Times);
            UsePet = CreatePetUseSetting(Id + 8, CustomRoles.Gaulois);
        }

        public override void Init()
        {
            playerIdList = [];
            IncreasedSpeedPlayerList = [];
        }

        public override void Add(byte playerId)
        {
            playerIdList.Add(playerId);
            playerId.SetAbilityUseLimit(UseLimitOpt.GetInt());

            if (!AmongUsClient.Instance.AmHost || (UsePets.GetBool() && UsePet.GetBool())) return;
            if (!Main.ResetCamPlayerList.Contains(playerId))
                Main.ResetCamPlayerList.Add(playerId);
        }

        public override bool IsEnable => playerIdList.Count > 0;

        public override void SetKillCooldown(byte playerId)
        {
            if (!IsEnable) return;
            Main.AllPlayerKillCooldown[playerId] = playerId.GetAbilityUseLimit() > 0 ? CD.GetFloat() : 300f;
        }

        public override bool CanUseKillButton(PlayerControl pc) => pc.GetAbilityUseLimit() >= 1;
        public override void ApplyGameOptions(IGameOptions opt, byte playerId) => opt.SetVision(false);

        public override bool OnCheckMurder(PlayerControl killer, PlayerControl target)
        {
            if (!IsEnable || killer == null || target == null || killer.GetAbilityUseLimit() <= 0) return false;

            Main.AllPlayerSpeed[target.PlayerId] += AdditionalSpeed.GetFloat();
            IncreasedSpeedPlayerList.Add(target.PlayerId);

            killer.RpcRemoveAbilityUse();
            killer.SetKillCooldown();

            target.MarkDirtySettings();

            return false;
        }
    }
}