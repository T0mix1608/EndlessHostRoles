﻿using System.Collections.Generic;
using AmongUs.GameOptions;
using EHR.Roles.Neutral;
using UnityEngine;
using static EHR.Options;
using static EHR.Translator;
using static EHR.Utils;

namespace EHR.Roles.Impostor
{
    public class RiftMaker : RoleBase
    {
        private const int Id = 640900;
        public static List<byte> playerIdList = [];

        public List<Vector2> Marks = [];
        public long LastTP = TimeStamp;

        public static OptionItem KillCooldown;
        public static OptionItem ShapeshiftCooldown;

        public static void SetupCustomOption()
        {
            SetupRoleOptions(Id, TabGroup.ImpostorRoles, CustomRoles.RiftMaker);
            KillCooldown = FloatOptionItem.Create(Id + 10, "KillCooldown", new(0f, 180f, 2.5f), 25f, TabGroup.ImpostorRoles).SetParent(CustomRoleSpawnChances[CustomRoles.RiftMaker])
                .SetValueFormat(OptionFormat.Seconds);
            ShapeshiftCooldown = FloatOptionItem.Create(Id + 11, "ShapeshiftCooldown", new(0f, 180f, 2.5f), 10f, TabGroup.ImpostorRoles).SetParent(CustomRoleSpawnChances[CustomRoles.RiftMaker])
                .SetValueFormat(OptionFormat.Seconds);
        }

        public override void Init()
        {
            playerIdList = [];
            Marks = [];
        }

        public override void Add(byte playerId)
        {
            playerIdList.Add(playerId);
            LastTP = TimeStamp;
            Marks = [];
        }

        public override bool IsEnable => playerIdList.Count > 0;

        public override void SetKillCooldown(byte id)
        {
            Main.AllPlayerKillCooldown[id] = KillCooldown.GetFloat();
        }

        public override void ApplyGameOptions(IGameOptions opt, byte playerId)
        {
            if (UsePets.GetBool()) return;
            AURoleOptions.ShapeshifterDuration = 1f;
            AURoleOptions.ShapeshifterCooldown = ShapeshiftCooldown.GetFloat();
            AURoleOptions.ShapeshifterLeaveSkin = true;
        }

        public override void OnFixedUpdate(PlayerControl player)
        {
            if (!GameStates.IsInTask) return;
            if (Pelican.IsEaten(player.PlayerId) || player.Data.IsDead) return;
            if (!player.Is(CustomRoles.RiftMaker)) return;
            if (Marks.Count != 2) return;
            if (Vector2.Distance(Marks[0], Marks[1]) <= 4f)
            {
                player.Notify(GetString("IncorrectMarks"));
                Marks.Clear();
                return;
            }

            if (LastTP + 5 > TimeStamp) return;

            Vector2 position = player.transform.position;

            bool isTP = false;
            Vector2 from = Marks[0];

            foreach (Vector2 mark in Marks.ToArray())
            {
                var dis = Vector2.Distance(mark, position);
                if (dis > 2f) continue;

                isTP = true;
                from = mark;
            }

            if (isTP)
            {
                LastTP = TimeStamp;
                if (from == Marks[0])
                {
                    player.TP(Marks[1]);
                }
                else if (from == Marks[1])
                {
                    player.TP(Marks[0]);
                }
                else
                {
                    Logger.Error($"Teleport failed - from: {from}", "RiftMakerTP");
                }
            }
        }

        public override void OnReportDeadBody()
        {
            LastTP = TimeStamp;
        }

        public override void OnEnterVent(PlayerControl player, Vent vent)
        {
            Marks.Clear();
            player.Notify(GetString("MarksCleared"));

            player.MyPhysics?.RpcBootFromVent(vent.Id);
        }

        public override bool OnShapeshift(PlayerControl player, PlayerControl target, bool shapeshifting)
        {
            if (player == null) return false;
            if (!shapeshifting) return true;
            if (Marks.Count >= 2) return false;

            Marks.Add(player.transform.position);
            if (Marks.Count == 2) LastTP = TimeStamp;
            player.Notify(GetString("MarkDone"));

            return false;
        }

        public override string GetProgressText(byte playerId, bool comms) => $" <color=#777777>-</color> {(Marks.Count == 2 ? "<color=#00ff00>" : "<color=#777777>")}{Marks.Count}/2</color>";
    }
}