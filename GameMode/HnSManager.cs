﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AmongUs.GameOptions;
using EHR.GameMode.HideAndSeekRoles;
using HarmonyLib;
using UnityEngine;

namespace EHR
{
    internal static class HnSManager
    {
        public static int TimeLeft;
        private static long LastUpdate;
        public static bool IsBlindTime;

        private static OptionItem MaxGameLength;
        private static OptionItem MinNeutrals;
        private static OptionItem MaxNeutrals;
        private static OptionItem DangerMeter;
        private static OptionItem PlayersSeeRoles;

        public static Dictionary<Team, Dictionary<CustomRoles, int>> HideAndSeekRoles = [];
        public static Dictionary<byte, (IHideAndSeekRole Interface, CustomRoles Role)> PlayerRoles = [];
        public static Dictionary<byte, byte> ClosestImpostor = [];
        public static Dictionary<byte, int> Danger = [];

        public static List<CustomRoles> AllHnSRoles = [];

        public static int SeekerNum => Math.Max(Main.RealOptionsData.GetInt(Int32OptionNames.NumImpostors), 1);

        public static void SetupCustomOption()
        {
            const int id = 69_211_001;
            Color color = new(52, 94, 235, byte.MaxValue);

            MaxGameLength = IntegerOptionItem.Create(id, "FFA_GameTime", new(0, 1200, 10), 600, TabGroup.GameSettings)
                .SetGameMode(CustomGameMode.HideAndSeek)
                .SetValueFormat(OptionFormat.Seconds)
                .SetColor(color);

            MinNeutrals = IntegerOptionItem.Create(id + 1, "HNS.MinNeutrals", new(0, 13, 1), 1, TabGroup.GameSettings)
                .SetGameMode(CustomGameMode.HideAndSeek)
                .SetColor(color);
            MaxNeutrals = IntegerOptionItem.Create(id + 2, "HNS.MaxNeutrals", new(0, 13, 1), 3, TabGroup.GameSettings)
                .SetGameMode(CustomGameMode.HideAndSeek)
                .SetColor(color);

            DangerMeter = BooleanOptionItem.Create(id + 3, "HNS.DangerMeter", true, TabGroup.GameSettings)
                .SetGameMode(CustomGameMode.HideAndSeek)
                .SetColor(color);

            PlayersSeeRoles = BooleanOptionItem.Create(id + 4, "HNS.PlayersSeeRoles", true, TabGroup.GameSettings)
                .SetGameMode(CustomGameMode.HideAndSeek)
                .SetColor(color);
        }

        public static void Init()
        {
            TimeLeft = MaxGameLength.GetInt() + 8;
            LastUpdate = Utils.TimeStamp;

            Type[] types = GetAllHnsRoleTypes();

            AllHnSRoles = GetAllHnsRoles(types);

            HideAndSeekRoles = types
                .Select(x => (IHideAndSeekRole)Activator.CreateInstance(x))
                .Where(x => x != null)
                .Join(AllHnSRoles, x => x.GetType().Name.ToLower(), x => x.ToString().ToLower(), (Interface, Enum) => (Enum, Interface))
                .Where(x => (!x.Enum.OnlySpawnsWithPets() || Options.UsePets.GetBool()) && (x.Enum != CustomRoles.Agent || SeekerNum >= 2) && x.Interface.Count > 0 && x.Interface.Chance > IRandom.Instance.Next(100))
                .OrderBy(x => x.Enum is CustomRoles.Seeker or CustomRoles.Hider ? 100 : IRandom.Instance.Next(100))
                .GroupBy(x => x.Interface.Team)
                .ToDictionary(x => x.Key, x => x.ToDictionary(y => y.Enum, y => y.Interface.Count));

            PlayerRoles = [];
            ClosestImpostor = [];

            if (Options.CurrentGameMode != CustomGameMode.HideAndSeek) return;

            IsBlindTime = true;
            Utils.MarkEveryoneDirtySettingsV4();
            _ = new LateTask(() =>
            {
                IsBlindTime = false;

                Main.AllAlivePlayerControls
                    .Join(PlayerRoles, x => x.PlayerId, x => x.Key, (pc, role) => (pc, role.Value.Interface))
                    .Where(x => x.Interface.Team == Team.Impostor)
                    .Do(x => x.pc.MarkDirtySettings());
            }, Seeker.BlindTime.GetFloat() + 8f, "Blind Time Expire");
        }

        public static List<CustomRoles> GetAllHnsRoles(IEnumerable<Type> types)
        {
            return types
                .Select(x => ((CustomRoles)Enum.Parse(typeof(CustomRoles), ignoreCase: true, value: x.Name)))
                .Where(role => role is CustomRoles.Seeker or CustomRoles.Hider || role.GetMode() != 0)
                .ToList();
        }

        public static Type[] GetAllHnsRoleTypes()
        {
            return Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => (typeof(IHideAndSeekRole)).IsAssignableFrom(t) && !t.IsInterface)
                .ToArray();
        }

        public static void AssignRoles()
        {
            Dictionary<PlayerControl, CustomRoles> result = [];
            List<PlayerControl> allPlayers = [.. Main.AllAlivePlayerControls];
            allPlayers = allPlayers.Shuffle();

            Dictionary<Team, int> memberNum = new()
            {
                [Team.Neutral] = IRandom.Instance.Next(MinNeutrals.GetInt(), MaxNeutrals.GetInt() + 1),
                [Team.Impostor] = SeekerNum
            };
            memberNum[Team.Crewmate] = allPlayers.Count - memberNum.Values.Sum();

            Logger.Warn($"Number of impostors: {memberNum[Team.Impostor]}", "debug");

            foreach (var item in Main.SetRoles)
            {
                PlayerControl pc = allPlayers.FirstOrDefault(x => x.PlayerId == item.Key);
                if (pc == null) continue;

                result[pc] = item.Value;
                allPlayers.Remove(pc);

                var role = HideAndSeekRoles.FirstOrDefault(x => x.Value.ContainsKey(item.Value));
                role.Value[item.Value]--;
                memberNum[role.Key]--;

                Logger.Warn($"Pre-Set Role Assigned: {pc.GetRealName()} => {item.Value}", "CustomRoleSelector");
            }

            var playerTeams = Enum.GetValues<Team>()[1..]
                .SelectMany(x => Enumerable.Repeat(x, memberNum[x]))
                .Shuffle()
                .Zip(allPlayers)
                .GroupBy(x => x.First, x => x.Second)
                .ToDictionary(x => x.Key, x => x.ToArray());

            foreach ((Team team, Dictionary<CustomRoles, int> roleCounts) in HideAndSeekRoles)
            {
                try
                {
                    if (playerTeams[team].Length == 0 || memberNum[team] <= 0) continue;
                }
                catch (KeyNotFoundException)
                {
                    continue;
                }

                foreach ((CustomRoles role, int count) in roleCounts)
                {
                    for (int i = 0; i < count; i++)
                    {
                        try
                        {
                            PlayerControl pc = playerTeams[team][0];
                            if (pc == null) continue;

                            result[pc] = role;
                            allPlayers.Remove(pc);
                            playerTeams[team] = playerTeams[team][1..];
                            memberNum[team]--;

                            if (memberNum[team] <= 0) break;
                        }
                        catch (Exception e)
                        {
                            if (e is IndexOutOfRangeException) break;
                            Utils.ThrowException(e);
                        }
                    }

                    if (playerTeams[team].Length == 0 || memberNum[team] <= 0) break;
                }
            }

            foreach (PlayerControl pc in allPlayers.Except(result.Keys).ToArray())
            {
                Logger.Warn($"Unassigned, force Hider: {pc.GetRealName()} => {CustomRoles.Hider}", "debug");
                result[pc] = CustomRoles.Hider;
                memberNum[Team.Crewmate]--;
                allPlayers.Remove(pc);
            }

            if (allPlayers.Count > 0) Logger.Error($"Some players were not assigned a role: {allPlayers.Join(x => x.GetRealName())}", "CustomRoleSelector");
            Logger.Msg($"Roles: {result.Join(x => $"{x.Key.GetRealName()} => {x.Value}")}", "HideAndSeekRoleSelector");

            var roleInterfaces = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => typeof(IHideAndSeekRole).IsAssignableFrom(x) && !x.IsInterface)
                .Select(x => (IHideAndSeekRole)Activator.CreateInstance(x))
                .Where(x => x != null)
                .ToDictionary(x => x.GetType().Name, x => x);
            PlayerRoles = result.ToDictionary(x => x.Key.PlayerId, x => (roleInterfaces[x.Value.ToString()], x.Value));

            result.IntersectBy(Main.PlayerStates.Keys, x => x.Key.PlayerId).Do(x => x.Key.RpcSetCustomRole(x.Value));
        }

        public static void ApplyGameOptions(IGameOptions opt, PlayerControl pc)
        {
            var role = PlayerRoles.GetValueOrDefault(pc.PlayerId);
            bool isBlind = role.Interface.Team == Team.Impostor && IsBlindTime;
            Main.AllPlayerSpeed[pc.PlayerId] = isBlind ? Main.MinSpeed : role.Interface.RoleSpeed;
            opt.SetFloat(FloatOptionNames.CrewLightMod, isBlind ? 0f : role.Interface.RoleVision);
            opt.SetFloat(FloatOptionNames.ImpostorLightMod, isBlind ? 0f : role.Interface.RoleVision);
            opt.SetFloat(FloatOptionNames.PlayerSpeedMod, Main.AllPlayerSpeed[pc.PlayerId]);
        }

        public static bool KnowTargetRoleColor(PlayerControl seer, PlayerControl target, ref string color)
        {
            if (seer.PlayerId == target.PlayerId || PlayersSeeRoles.GetBool()) return true;

            var targetRole = PlayerRoles[target.PlayerId];
            var seerRole = PlayerRoles[seer.PlayerId];

            if (targetRole.Interface.Team == Team.Impostor && (PlayersSeeRoles.GetBool() || targetRole.Role != CustomRoles.Agent || seerRole.Interface.Team == Team.Impostor))
            {
                color = Main.RoleColors[CustomRoles.Seeker];
                return true;
            }

            return false;
        }

        public static bool HasTasks(GameData.PlayerInfo playerInfo)
        {
            var role = PlayerRoles[playerInfo.PlayerId];
            return role.Interface.Team == Team.Crewmate || role.Role == CustomRoles.Taskinator;
        }

        public static bool IsRoleTextEnabled(PlayerControl seer, PlayerControl target)
        {
            return seer.PlayerId == target.PlayerId || PlayersSeeRoles.GetBool();
        }

        public static string GetSuffixText(PlayerControl seer, PlayerControl target, bool isHUD = false)
        {
            if (seer.PlayerId != target.PlayerId) return string.Empty;

            string dangerMeter = GetDangerMeter(seer);

            if (!isHUD && seer.IsModClient()) return string.Empty;
            if (TimeLeft <= 60)
            {
                return $"{dangerMeter}\n<color={Main.RoleColors[CustomRoles.Hider]}>{Translator.GetString("TimeLeft")}:</color> {TimeLeft}s";
            }

            var remainingMinutes = TimeLeft / 60;
            var remainingSeconds = $"{(TimeLeft % 60) + 1}";
            if (remainingSeconds.Length == 1) remainingSeconds = $"0{remainingSeconds}";
            return dangerMeter + "\n" + (isHUD ? $"{remainingMinutes}:{remainingSeconds}" : $"{string.Format(Translator.GetString("MinutesLeft"), $"{remainingMinutes}-{remainingMinutes + 1}")}");
        }

        private static string GetDangerMeter(PlayerControl seer)
        {
            return Danger.TryGetValue(seer.PlayerId, out int danger)
                ? danger <= 5
                    ? $"\n<color={GetColorFromDanger()}>{new('\u25a0', 5 - danger)}{new('\u25a1', danger)}</color>"
                    : $"\n<color=#ffffff>{new('\u25a1', 5)}</color>"
                : string.Empty;

            string GetColorFromDanger() // 0: Highest, 4: Lowest
            {
                return danger switch
                {
                    0 => "#ff1313",
                    1 => "#ff6a00",
                    2 => "#ffaa00",
                    3 => "#ffea00",
                    4 => "#ffff00",
                    _ => "#ffffff"
                };
            }
        }

        public static string GetRoleInfoText(PlayerControl seer)
        {
            return $"<size=90%>{Utils.ColorString(Utils.GetRoleColor(seer.GetCustomRole()), seer.GetRoleInfo())}</size>";
        }

        public static bool CheckForGameEnd(out GameOverReason reason)
        {
            reason = GameOverReason.ImpostorByKill;

            var alivePlayers = Main.AllAlivePlayerControls;

            // If there are 0 players alive, the game is over and only foxes win
            if (alivePlayers.Length == 0)
            {
                CustomWinnerHolder.ResetAndSetWinner(CustomWinner.None);
                AddFoxesToWinners();
                return true;
            }

            // If there are no crew roles left, the game is over and only impostors win
            if (alivePlayers.All(x => PlayerRoles[x.PlayerId].Interface.Team != Team.Crewmate))
            {
                CustomWinnerHolder.ResetAndSetWinner(CustomWinner.Seeker);
                CustomWinnerHolder.WinnerIds.UnionWith(PlayerRoles.Where(x => x.Value.Interface.Team == Team.Impostor).Select(x => x.Key));
                AddFoxesToWinners();
                return true;
            }

            // If time is up, the game is over and crewmates win
            if (TimeLeft <= 0)
            {
                reason = GameOverReason.HumansByTask;
                CustomWinnerHolder.ResetAndSetWinner(CustomWinner.Hider);
                CustomWinnerHolder.WinnerIds.UnionWith(PlayerRoles.Where(x => x.Value.Interface.Team == Team.Crewmate).Select(x => x.Key));
                AddFoxesToWinners();
                return true;
            }

            return false;
        }

        public static void AddFoxesToWinners()
        {
            var foxes = Main.PlayerStates.Where(x => x.Value.MainRole == CustomRoles.Fox).Select(x => x.Key).ToList();
            foxes.RemoveAll(x =>
            {
                var pc = Utils.GetPlayerById(x);
                return pc == null || !pc.IsAlive();
            });
            if (foxes.Count == 0) return;
            CustomWinnerHolder.AdditionalWinnerTeams.Add(AdditionalWinners.Fox);
            CustomWinnerHolder.WinnerIds.UnionWith(foxes);
        }

        public static string GetTaskBarText()
        {
            var text = Main.PlayerStates.IntersectBy(PlayerRoles.Keys, x => x.Key).Aggregate("<size=80%>", (current, state) => $"{current}{GetStateText(state)}\n");
            return $"{text}</size>\r\n\r\n<#00ffa5>{Translator.GetString("HNS.TaskCount")}</color> {GameData.Instance.CompletedTasks}/{GameData.Instance.TotalTasks}";

            static string GetStateText(KeyValuePair<byte, PlayerState> state)
            {
                string name = Main.AllPlayerNames.GetValueOrDefault(state.Key, $"ID {state.Key}");
                name = Utils.ColorString(Main.PlayerColors.GetValueOrDefault(state.Key, Color.white), name);
                bool isSeeker = PlayerRoles[state.Key].Interface.Team == Team.Impostor;
                bool alive = !state.Value.IsDead;

                TaskState ts = state.Value.TaskState;
                string stateText = string.Empty;
                if (PlayersSeeRoles.GetBool()) stateText = $" ({GetRole().ToColoredString()}){GetTaskCount()}";
                else if (isSeeker) stateText = $" ({CustomRoles.Seeker.ToString()})";
                if (!alive) stateText += $"  <color=#ff0000>{Translator.GetString("Dead")}</color>";

                stateText = $"{name}{stateText}";
                return stateText;

                CustomRoles GetRole() => state.Value.MainRole == CustomRoles.Agent ? CustomRoles.Hider : state.Value.MainRole;
                string GetTaskCount() => CustomRoles.Agent.IsEnable() || !ts.hasTasks ? string.Empty : $" ({ts.CompletedTasksCount}/{ts.AllTasksCount})";
            }
        }

        public static void OnCheckMurder(PlayerControl killer, PlayerControl target)
        {
            if (killer == null || target == null || PlayerRoles[killer.PlayerId].Interface.Team != Team.Impostor || PlayerRoles[target.PlayerId].Interface.Team == Team.Impostor) return;

            killer.Kill(target);

            // If the Troll is killed, they win
            if (target.Is(CustomRoles.Troll))
            {
                CustomWinnerHolder.ResetAndSetWinner(CustomWinner.Troll);
                CustomWinnerHolder.WinnerIds.Add(target.PlayerId);
                AddFoxesToWinners();
            }
        }

        public static void OnCoEnterVent(PlayerPhysics physics, int ventId)
        {
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        class FixedUpdatePatch
        {
            public static void Postfix(PlayerControl __instance)
            {
                if (!AmongUsClient.Instance.AmHost || !GameStates.IsInTask || Options.CurrentGameMode != CustomGameMode.HideAndSeek) return;

                long now = Utils.TimeStamp;
                if (LastUpdate == now) return;
                LastUpdate = now;

                TimeLeft--;

                if (DangerMeter.GetBool() || (TimeLeft + 1) % 60 == 0 || TimeLeft <= 60) Utils.NotifyRoles();

                PlayerRoles = PlayerRoles.Where(x => Utils.GetPlayerById(x.Key) != null).ToDictionary(x => x.Key, x => x.Value);

                var imps = PlayerRoles.Where(x => x.Value.Interface.Team == Team.Impostor).ToDictionary(x => x.Key, x => Utils.GetPlayerById(x.Key).Pos());
                var nonImps = PlayerRoles.Where(x => x.Value.Interface.Team is Team.Crewmate or Team.Neutral).ToArray();
                ClosestImpostor = nonImps.ToDictionary(x => x.Key, x => imps.MinBy(y => Vector2.Distance(y.Value, Utils.GetPlayerById(x.Key).Pos())).Key);
                Danger = nonImps.ToDictionary(x => x.Key, x => (1 + (int)Vector2.Distance(Utils.GetPlayerById(x.Key).Pos(), Utils.GetPlayerById(ClosestImpostor[x.Key]).Pos())) / 2);
            }
        }
    }
}