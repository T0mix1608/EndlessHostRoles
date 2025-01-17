﻿using System;
using AmongUs.GameOptions;

namespace EHR.Roles.AddOns.GhostRoles
{
    // TOU-R Phantom
    internal class Specter : IGhostRole, ISettingHolder
    {
        private static OptionItem SnatchWin;

        public bool IsWon;
        public Team Team => Team.Neutral;
        public int Cooldown => 900;

        public void OnAssign(PlayerControl pc)
        {
            IsWon = false;
            _ = new LateTask(() =>
            {
                var taskState = pc.GetTaskState();
                if (taskState == null) return;

                taskState.hasTasks = true;
                taskState.CompletedTasksCount = 0;
                taskState.AllTasksCount = Utils.TotalTaskCount - Main.RealOptionsData.GetInt(Int32OptionNames.NumCommonTasks);

                GameData.Instance.RpcSetTasks(pc.PlayerId, Array.Empty<byte>());
                pc.SyncSettings();
                pc.RpcResetAbilityCooldown();
                Utils.NotifyRoles(SpecifySeer: pc, SpecifyTarget: pc);
            }, 1f, "Specter Assign");
        }

        public void OnProtect(PlayerControl pc, PlayerControl target)
        {
        }

        public void SetupCustomOption()
        {
            Options.SetupRoleOptions(649100, TabGroup.OtherRoles, CustomRoles.Specter, zeroOne: true);
            SnatchWin = BooleanOptionItem.Create(649102, "SnatchWin", false, TabGroup.OtherRoles)
                .SetParent(Options.CustomRoleSpawnChances[CustomRoles.Specter]);
            Options.OverrideTasksData.Create(649103, TabGroup.OtherRoles, CustomRoles.Specter);
        }

        public void OnFinishedTasks(PlayerControl pc)
        {
            if (!SnatchWin.GetBool())
            {
                IsWon = true;
                return;
            }

            CustomWinnerHolder.ResetAndSetWinner(CustomWinner.Specter);
            CustomWinnerHolder.WinnerIds.Add(pc.PlayerId);
        }
    }
}