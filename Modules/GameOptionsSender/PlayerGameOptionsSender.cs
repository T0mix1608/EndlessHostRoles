﻿using System;
using System.Linq;
using AmongUs.GameOptions;
using EHR.Roles.AddOns.GhostRoles;
using EHR.Roles.Crewmate;
using EHR.Roles.Impostor;
using EHR.Roles.Neutral;
using Hazel;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using InnerNet;
using Mathf = UnityEngine.Mathf;

namespace EHR.Modules;

public class PlayerGameOptionsSender(PlayerControl player) : GameOptionsSender
{
    public PlayerControl player = player;

    public override IGameOptions BasedGameOptions =>
        Main.RealOptionsData.Restore(new NormalGameOptionsV07(new UnityLogger().Cast<ILogger>()).Cast<IGameOptions>());

    public override bool IsDirty { get; protected set; }

    public static void SetDirty(byte playerId)
    {
        foreach (GameOptionsSender allSender in AllSenders)
        {
            if (allSender is PlayerGameOptionsSender sender && sender.player.PlayerId == playerId)
            {
                sender.SetDirty();
            }
        }
    }

    public static void SetDirtyToAll()
    {
        foreach (GameOptionsSender allSender in AllSenders)
        {
            if (allSender is PlayerGameOptionsSender sender)
            {
                sender.SetDirty();
            }
        }
    }

    // For lights call/fix
    public static void SetDirtyToAllV2()
    {
        foreach (GameOptionsSender allSender in AllSenders)
        {
            if (allSender is PlayerGameOptionsSender { IsDirty: false } sender && sender.player.IsAlive() && (sender.player.GetCustomRole().NeedUpdateOnLights() || sender.player.Is(CustomRoles.Torch) || sender.player.Is(CustomRoles.Mare)))
            {
                sender.SetDirty();
            }
        }
    }

    // For Grenadier blidning/restoring
    public static void SetDirtyToAllV3()
    {
        foreach (GameOptionsSender allSender in AllSenders)
        {
            if (allSender is PlayerGameOptionsSender { IsDirty: false } sender && sender.player.IsAlive() && ((Grenadier.GrenadierBlinding.Count > 0 && (sender.player.GetCustomRole().IsImpostor() || (sender.player.GetCustomRole().IsNeutral() && Options.GrenadierCanAffectNeutral.GetBool()))) || (Grenadier.MadGrenadierBlinding.Count > 0 && !sender.player.GetCustomRole().IsImpostorTeam() && !sender.player.Is(CustomRoles.Madmate))))
            {
                sender.SetDirty();
            }
        }
    }

    // For players with kill buttons
    public static void SetDirtyToAllV4()
    {
        foreach (GameOptionsSender allSender in AllSenders)
        {
            if (allSender is PlayerGameOptionsSender { IsDirty: false } sender && sender.player.IsAlive() && sender.player.CanUseKillButton())
            {
                sender.SetDirty();
            }
        }
    }

    public void SetDirty() => IsDirty = true;

    public override void SendGameOptions()
    {
        if (player.AmOwner)
        {
            var opt = BuildGameOptions();
            if (GameManager.Instance?.LogicComponents != null)
            {
                foreach (var com in GameManager.Instance.LogicComponents)
                {
                    if (com.TryCast<LogicOptions>(out var lo))
                        lo.SetGameOptions(opt);
                }
            }

            GameOptionsManager.Instance.CurrentGameOptions = opt;
        }
        else base.SendGameOptions();
    }

    protected override void SendOptionsArray(Il2CppStructArray<byte> optionArray)
    {
        try
        {
            byte i = 0;
            foreach (var logicComponent in GameManager.Instance.LogicComponents)
            {
                if (logicComponent.TryCast<LogicOptions>(out _))
                {
                    SendOptionsArray(optionArray, i, player.GetClientId());
                }

                i++;
            }
        }
        catch (Exception ex)
        {
            Logger.Fatal(ex.ToString(), "PlayerGameOptionsSender.SendOptionsArray");
        }
    }

    public static void RemoveSender(PlayerControl player)
    {
        var sender = AllSenders.OfType<PlayerGameOptionsSender>()
            .FirstOrDefault(sender => sender.player.PlayerId == player.PlayerId);
        if (sender == null) return;
        sender.player = null;
        AllSenders.Remove(sender);
    }

    public override IGameOptions BuildGameOptions()
    {
        try
        {
            Main.RealOptionsData ??= new(GameOptionsManager.Instance.CurrentGameOptions);

            var opt = BasedGameOptions;
            AURoleOptions.SetOpt(opt);
            var state = Main.PlayerStates[player.PlayerId];
            opt.BlackOut(state.IsBlackOut);

            CustomRoles role = player.GetCustomRole();

            switch (Options.CurrentGameMode)
            {
                case CustomGameMode.FFA:
                    if (FFAManager.FFALowerVisionList.ContainsKey(player.PlayerId))
                    {
                        opt.SetVision(true);
                        opt.SetFloat(FloatOptionNames.CrewLightMod, FFAManager.FFALowerVision.GetFloat());
                        opt.SetFloat(FloatOptionNames.ImpostorLightMod, FFAManager.FFALowerVision.GetFloat());
                    }
                    else
                    {
                        opt.SetVision(true);
                        opt.SetFloat(FloatOptionNames.CrewLightMod, 1.25f);
                        opt.SetFloat(FloatOptionNames.ImpostorLightMod, 1.25f);
                    }

                    break;
                case CustomGameMode.HotPotato:
                case CustomGameMode.MoveAndStop:
                    opt.SetVision(true);
                    opt.SetFloat(FloatOptionNames.CrewLightMod, 1.25f);
                    opt.SetFloat(FloatOptionNames.ImpostorLightMod, 1.25f);
                    break;
                case CustomGameMode.HideAndSeek:
                    HnSManager.ApplyGameOptions(opt, player);
                    break;
            }

            switch (player.GetCustomRoleTypes())
            {
                case CustomRoleTypes.Impostor:
                    AURoleOptions.ShapeshifterCooldown = Options.DefaultShapeshiftCooldown.GetFloat();
                    AURoleOptions.GuardianAngelCooldown = Spiritcaller.SpiritAbilityCooldown.GetFloat();
                    break;
                case CustomRoleTypes.Neutral:
                    AURoleOptions.GuardianAngelCooldown = Spiritcaller.SpiritAbilityCooldown.GetFloat();
                    break;
                case CustomRoleTypes.Crewmate:
                    AURoleOptions.GuardianAngelCooldown = Spiritcaller.SpiritAbilityCooldown.GetFloat();
                    break;
            }

            switch (role)
            {
                case CustomRoles.ShapeshifterEHR:
                    AURoleOptions.ShapeshifterCooldown = Options.ShapeshiftCD.GetFloat();
                    AURoleOptions.ShapeshifterDuration = Options.ShapeshiftDur.GetFloat();
                    break;
                case CustomRoles.ScientistEHR:
                    AURoleOptions.ScientistCooldown = Options.ScientistCD.GetFloat();
                    AURoleOptions.ScientistBatteryCharge = Options.ScientistDur.GetFloat();
                    break;
            }

            Main.PlayerStates[player.PlayerId].Role.ApplyGameOptions(opt, player.PlayerId);

            if (Main.AllPlayerControls.Any(x => x.Is(CustomRoles.Bewilder) && !x.IsAlive() && x.GetRealKiller()?.PlayerId == player.PlayerId && !x.Is(CustomRoles.Hangman)))
            {
                opt.SetVision(false);
                opt.SetFloat(FloatOptionNames.CrewLightMod, Options.BewilderVision.GetFloat());
                opt.SetFloat(FloatOptionNames.ImpostorLightMod, Options.BewilderVision.GetFloat());
            }

            if ((Grenadier.GrenadierBlinding.Count > 0 &&
                 (role.IsImpostor() ||
                  (role.IsNeutral() && Options.GrenadierCanAffectNeutral.GetBool()))) ||
                (Grenadier.MadGrenadierBlinding.Count > 0 && !role.IsImpostorTeam() && !player.Is(CustomRoles.Madmate)))
            {
                opt.SetVision(false);
                opt.SetFloat(FloatOptionNames.CrewLightMod, Options.GrenadierCauseVision.GetFloat());
                opt.SetFloat(FloatOptionNames.ImpostorLightMod, Options.GrenadierCauseVision.GetFloat());
            }

            switch (role)
            {
                case CustomRoles.Alchemist when ((Alchemist)Main.PlayerStates[player.PlayerId].Role).VisionPotionActive:
                    opt.SetVisionV2();
                    if (Utils.IsActive(SystemTypes.Electrical)) opt.SetFloat(FloatOptionNames.CrewLightMod, Alchemist.VisionOnLightsOut.GetFloat() * 5);
                    else opt.SetFloat(FloatOptionNames.CrewLightMod, Alchemist.Vision.GetFloat());
                    break;
                case CustomRoles.Mayor when Mayor.MayorSeesVoteColorsWhenDoneTasks.GetBool() && player.GetTaskState().IsTaskFinished:
                    opt.SetBool(BoolOptionNames.AnonymousVotes, false);
                    break;
            }

            if (Sprayer.LowerVisionList.Contains(player.PlayerId))
            {
                opt.SetVision(false);
                opt.SetFloat(FloatOptionNames.CrewLightMod, Sprayer.LoweredVision.GetFloat());
                opt.SetFloat(FloatOptionNames.ImpostorLightMod, Sprayer.LoweredVision.GetFloat());
            }

            if (Minion.BlindPlayers.Contains(player.PlayerId))
            {
                opt.SetVision(false);
                opt.SetFloat(FloatOptionNames.CrewLightMod, 0);
                opt.SetFloat(FloatOptionNames.ImpostorLightMod, 0);
            }

            if (Sentinel.IsPatrolling(player.PlayerId))
            {
                opt.SetVision(false);
                opt.SetFloat(FloatOptionNames.CrewLightMod, Sentinel.LoweredVision.GetFloat());
                opt.SetFloat(FloatOptionNames.ImpostorLightMod, Sentinel.LoweredVision.GetFloat());
            }

            if (Beacon.IsAffectedPlayer(player.PlayerId))
            {
                opt.SetFloat(FloatOptionNames.CrewLightMod, Beacon.IncreasedVision);
                opt.SetFloat(FloatOptionNames.ImpostorLightMod, Beacon.IncreasedVision);
            }

            Dazzler.SetDazzled(player, opt);
            Deathpact.SetDeathpactVision(player, opt);

            Spiritcaller.ReduceVision(opt, player);

            if (Randomizer.HasSuperVision(player))
            {
                opt.SetVision(true);
                opt.SetFloat(FloatOptionNames.CrewLightMod, 1.5f);
                opt.SetFloat(FloatOptionNames.ImpostorLightMod, 1.5f);
            }
            else if (Randomizer.IsBlind(player))
            {
                opt.SetVision(false);
                opt.SetFloat(FloatOptionNames.CrewLightMod, 0);
                opt.SetFloat(FloatOptionNames.ImpostorLightMod, 0);
            }

            var array = Main.PlayerStates[player.PlayerId].SubRoles;
            foreach (CustomRoles subRole in array)
            {
                if (subRole.IsGhostRole() && subRole != CustomRoles.EvilSpirit)
                {
                    AURoleOptions.GuardianAngelCooldown = GhostRolesManager.AssignedGhostRoles.First(x => x.Value.Role == subRole).Value.Instance.Cooldown;
                    continue;
                }

                switch (subRole)
                {
                    case CustomRoles.Watcher:
                        opt.SetBool(BoolOptionNames.AnonymousVotes, false);
                        break;
                    case CustomRoles.Flashman:
                        Main.AllPlayerSpeed[player.PlayerId] = Options.FlashmanSpeed.GetFloat();
                        break;
                    case CustomRoles.Giant:
                        Main.AllPlayerSpeed[player.PlayerId] = Options.GiantSpeed.GetFloat();
                        break;
                    case CustomRoles.Mare when Options.MareHasIncreasedSpeed.GetBool():
                        Main.AllPlayerSpeed[player.PlayerId] = Options.MareSpeedDuringLightsOut.GetFloat();
                        break;
                    case CustomRoles.Sleep when Utils.IsActive(SystemTypes.Electrical):
                        opt.SetVision(false);
                        opt.SetFloat(FloatOptionNames.CrewLightMod, 0);
                        opt.SetFloat(FloatOptionNames.ImpostorLightMod, 0);
                        break;
                    case CustomRoles.Torch:
                        if (!Utils.IsActive(SystemTypes.Electrical))
                        {
                            opt.SetVision(true);
                            opt.SetFloat(FloatOptionNames.CrewLightMod, Options.TorchVision.GetFloat());
                            opt.SetFloat(FloatOptionNames.ImpostorLightMod, Options.TorchVision.GetFloat());
                        }
                        else if (!Options.TorchAffectedByLights.GetBool())
                        {
                            opt.SetVision(true);
                            opt.SetFloat(FloatOptionNames.CrewLightMod, Options.TorchVision.GetFloat() * 5);
                            opt.SetFloat(FloatOptionNames.ImpostorLightMod, Options.TorchVision.GetFloat() * 5);
                        }

                        break;
                    case CustomRoles.Bewilder:
                        opt.SetVision(false);
                        opt.SetFloat(FloatOptionNames.CrewLightMod, Options.BewilderVision.GetFloat());
                        opt.SetFloat(FloatOptionNames.ImpostorLightMod, Options.BewilderVision.GetFloat());
                        break;
                    case CustomRoles.Sunglasses:
                        opt.SetVision(false);
                        opt.SetFloat(FloatOptionNames.CrewLightMod, Options.SunglassesVision.GetFloat());
                        opt.SetFloat(FloatOptionNames.ImpostorLightMod, Options.SunglassesVision.GetFloat());
                        break;
                    case CustomRoles.Reach:
                        opt.SetInt(Int32OptionNames.KillDistance, 2);
                        break;
                    case CustomRoles.Madmate:
                        opt.SetVision(Options.MadmateHasImpostorVision.GetBool());
                        break;
                    case CustomRoles.Nimble when player.GetRoleTypes() == RoleTypes.Engineer:
                        AURoleOptions.EngineerCooldown = Options.NimbleCD.GetFloat();
                        AURoleOptions.EngineerInVentMaxTime = Options.NimbleInVentTime.GetFloat();
                        break;
                    case CustomRoles.Physicist when player.GetRoleTypes() == RoleTypes.Scientist:
                        AURoleOptions.ScientistCooldown = Options.PhysicistCD.GetFloat();
                        AURoleOptions.ScientistBatteryCharge = Options.PhysicistViewDuration.GetFloat();
                        break;
                }
            }

            if (Magician.BlindPPL.ContainsKey(player.PlayerId))
            {
                opt.SetVision(false);
                opt.SetFloat(FloatOptionNames.CrewLightMod, 0);
                opt.SetFloat(FloatOptionNames.ImpostorLightMod, 0);
            }

            if (player.IsCrewmate() && Main.PlayerStates.Values.Any(s => s.Role is Adventurer { IsEnable: true } av && av.ActiveWeapons.Contains(Adventurer.Weapon.Lantern)))
            {
                opt.SetVision(true);
                opt.SetFloat(FloatOptionNames.CrewLightMod, 1.5f);
                opt.SetFloat(FloatOptionNames.ImpostorLightMod, 1.5f);
            }

            if (Chemist.Instances.Any(x => x.IsBlinding && player.PlayerId != x.ChemistPC.PlayerId))
            {
                opt.SetVision(false);
                opt.SetFloat(FloatOptionNames.CrewLightMod, 0);
                opt.SetFloat(FloatOptionNames.ImpostorLightMod, 0);
            }

            if (Changeling.ChangedRole.TryGetValue(player.PlayerId, out var changed) && changed && player.GetRoleTypes() != RoleTypes.Shapeshifter)
            {
                AURoleOptions.ShapeshifterCooldown = 300f;
                AURoleOptions.ShapeshifterDuration = 1f;
            }

            // ===================================================================================================================

            AURoleOptions.EngineerCooldown = Mathf.Max(0.01f, AURoleOptions.EngineerCooldown);

            if (Main.AllPlayerKillCooldown.TryGetValue(player.PlayerId, out var killCooldown))
            {
                AURoleOptions.KillCooldown = Mathf.Max(0.01f, killCooldown);
            }

            if (Main.AllPlayerSpeed.TryGetValue(player.PlayerId, out var speed))
            {
                AURoleOptions.PlayerSpeedMod = Mathf.Clamp(speed, Main.MinSpeed, 3f);
            }

            state.taskState.hasTasks = Utils.HasTasks(player.Data, false);
            if (Options.GhostCanSeeOtherVotes.GetBool() && player.Data.IsDead)
                opt.SetBool(BoolOptionNames.AnonymousVotes, false);
            if (Options.AdditionalEmergencyCooldown.GetBool() &&
                Options.AdditionalEmergencyCooldownThreshold.GetInt() <= Utils.AllAlivePlayersCount)
            {
                opt.SetInt(
                    Int32OptionNames.EmergencyCooldown,
                    Options.AdditionalEmergencyCooldownTime.GetInt());
            }

            if (Options.SyncButtonMode.GetBool() && Options.SyncedButtonCount.GetValue() <= Options.UsedButtonCount)
            {
                opt.SetInt(Int32OptionNames.EmergencyCooldown, 3600);
            }

            MeetingTimeManager.ApplyGameOptions(opt);

            AURoleOptions.ShapeshifterCooldown = Mathf.Max(1f, AURoleOptions.ShapeshifterCooldown);
            AURoleOptions.ProtectionDurationSeconds = 0f;
            AURoleOptions.ImpostorsCanSeeProtect = false;

            return opt;
        }
        catch (Exception e)
        {
            Logger.Fatal($"Error for {player.GetRealName()} ({player.GetCustomRole()}): {e}", "PlayerGameOptionsSender.BuildGameOptions");
            Logger.SendInGame($"Error syncing settings for {player.GetRealName()} - Please report this bug to the developer AND SEND LOGS");
            return BasedGameOptions;
        }
    }

    public override bool AmValid()
    {
        return base.AmValid() && player != null && !player.Data.Disconnected && Main.RealOptionsData != null;
    }
}