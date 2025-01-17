﻿using System.Collections.Generic;
using System.Linq;
using EHR.Roles.Neutral;
using static EHR.Options;
using static EHR.Translator;

namespace EHR.Roles.Crewmate
{
    internal class Merchant : RoleBase
    {
        private const int Id = 7300;
        private static readonly List<byte> PlayerIdList = [];

        public static Dictionary<byte, int> addonsSold = [];
        public static Dictionary<byte, List<byte>> bribedKiller = [];

        private static List<CustomRoles> Addons = [];
        private static Dictionary<AddonTypes, List<CustomRoles>> GroupedAddons = [];

        private static OptionItem OptionMaxSell;
        private static OptionItem OptionMoneyPerSell;
        private static OptionItem OptionMoneyRequiredToBribe;
        private static OptionItem OptionNotifyBribery;
        private static OptionItem OptionCanTargetCrew;
        private static OptionItem OptionCanTargetImpostor;
        private static OptionItem OptionCanTargetNeutral;
        private static OptionItem OptionCanSellMixed;
        private static OptionItem OptionCanSellHelpful;
        private static OptionItem OptionCanSellHarmful;
        private static OptionItem OptionSellOnlyEnabledAddons;
        private static OptionItem OptionSellOnlyHarmfulToEvil;
        private static OptionItem OptionSellOnlyHelpfulToCrew;
        private static OptionItem OptionGivesAllMoneyOnBribe;

        public override bool IsEnable => PlayerIdList.Count > 0;

        private static int GetCurrentAmountOfMoney(byte playerId) => (addonsSold[playerId] * OptionMoneyPerSell.GetInt()) - (bribedKiller[playerId].Count * OptionMoneyRequiredToBribe.GetInt());

        public static void SetupCustomOption()
        {
            SetupRoleOptions(Id, TabGroup.CrewmateRoles, CustomRoles.Merchant);
            OptionMaxSell = IntegerOptionItem.Create(Id + 2, "MerchantMaxSell", new(1, 20, 1), 5, TabGroup.CrewmateRoles).SetParent(CustomRoleSpawnChances[CustomRoles.Merchant]).SetValueFormat(OptionFormat.Times);
            OptionMoneyPerSell = IntegerOptionItem.Create(Id + 3, "MerchantMoneyPerSell", new(1, 20, 1), 1, TabGroup.CrewmateRoles).SetParent(CustomRoleSpawnChances[CustomRoles.Merchant]).SetValueFormat(OptionFormat.Times);
            OptionMoneyRequiredToBribe = IntegerOptionItem.Create(Id + 4, "MerchantMoneyRequiredToBribe", new(1, 70, 1), 5, TabGroup.CrewmateRoles).SetParent(CustomRoleSpawnChances[CustomRoles.Merchant]).SetValueFormat(OptionFormat.Times);
            OptionNotifyBribery = BooleanOptionItem.Create(Id + 5, "MerchantNotifyBribery", false, TabGroup.CrewmateRoles).SetParent(CustomRoleSpawnChances[CustomRoles.Merchant]);
            OptionCanTargetCrew = BooleanOptionItem.Create(Id + 6, "MerchantTargetCrew", true, TabGroup.CrewmateRoles).SetParent(CustomRoleSpawnChances[CustomRoles.Merchant]);
            OptionCanTargetImpostor = BooleanOptionItem.Create(Id + 7, "MerchantTargetImpostor", true, TabGroup.CrewmateRoles).SetParent(CustomRoleSpawnChances[CustomRoles.Merchant]);
            OptionCanTargetNeutral = BooleanOptionItem.Create(Id + 8, "MerchantTargetNeutral", true, TabGroup.CrewmateRoles).SetParent(CustomRoleSpawnChances[CustomRoles.Merchant]);
            OptionCanSellMixed = BooleanOptionItem.Create(Id + 9, "MerchantSellMixed", true, TabGroup.CrewmateRoles).SetParent(CustomRoleSpawnChances[CustomRoles.Merchant]);
            OptionCanSellHelpful = BooleanOptionItem.Create(Id + 10, "MerchantSellHelpful", true, TabGroup.CrewmateRoles).SetParent(CustomRoleSpawnChances[CustomRoles.Merchant]);
            OptionCanSellHarmful = BooleanOptionItem.Create(Id + 11, "MerchantSellHarmful", true, TabGroup.CrewmateRoles).SetParent(CustomRoleSpawnChances[CustomRoles.Merchant]);
            OptionSellOnlyEnabledAddons = BooleanOptionItem.Create(Id + 12, "MerchantSellOnlyEnabledAddons", false, TabGroup.CrewmateRoles).SetParent(CustomRoleSpawnChances[CustomRoles.Merchant]);
            OptionSellOnlyHarmfulToEvil = BooleanOptionItem.Create(Id + 13, "MerchantSellHarmfulToEvil", false, TabGroup.CrewmateRoles).SetParent(CustomRoleSpawnChances[CustomRoles.Merchant]);
            OptionSellOnlyHelpfulToCrew = BooleanOptionItem.Create(Id + 14, "MerchantSellHelpfulToCrew", false, TabGroup.CrewmateRoles).SetParent(CustomRoleSpawnChances[CustomRoles.Merchant]);
            OptionGivesAllMoneyOnBribe = BooleanOptionItem.Create(Id + 15, "MerchantGivesAllMoneyOnBribe", false, TabGroup.CrewmateRoles).SetParent(CustomRoleSpawnChances[CustomRoles.Merchant]);

            OverrideTasksData.Create(Id + 18, TabGroup.CrewmateRoles, CustomRoles.Merchant);
        }

        public override void Init()
        {
            PlayerIdList.Clear();

            addonsSold = [];
            bribedKiller = [];

            GroupedAddons = Options.GroupedAddons.ToDictionary(x => x.Key, x => x.Value.ToList());

            if (!OptionCanSellHarmful.GetBool()) GroupedAddons.Remove(AddonTypes.Harmful);
            if (!OptionCanSellHelpful.GetBool()) GroupedAddons.Remove(AddonTypes.Helpful);
            if (!OptionCanSellMixed.GetBool()) GroupedAddons.Remove(AddonTypes.Mixed);

            Addons = GroupedAddons.SelectMany(x => x.Value).ToList();

            if (OptionSellOnlyEnabledAddons.GetBool()) Addons.RemoveAll(x => x.GetMode() == 0);

            Addons.RemoveAll(x => x is CustomRoles.Nimble or CustomRoles.Physicist or CustomRoles.Bloodlust);
        }

        public override void Add(byte playerId)
        {
            PlayerIdList.Add(playerId);
            addonsSold.Add(playerId, 0);
            bribedKiller.Add(playerId, []);
        }

        public override void OnTaskComplete(PlayerControl player, int completedTaskCount, int totalTaskCount)
        {
            if (!player.IsAlive() || !player.Is(CustomRoles.Merchant) || (addonsSold[player.PlayerId] >= OptionMaxSell.GetInt()))
            {
                return;
            }

            if (Addons.Count == 0)
            {
                player.Notify(Utils.ColorString(Utils.GetRoleColor(CustomRoles.Merchant), GetString("MerchantAddonSellFail")));
                Logger.Info("No addons to sell.", "Merchant");
                return;
            }

            CustomRoles addon = Addons.RandomElement();

            var AllAlivePlayer =
                Main.AllAlivePlayerControls.Where(x =>
                    x.PlayerId != player.PlayerId
                    && !Pelican.IsEaten(x.PlayerId)
                    && !x.Is(addon)
                    && !CustomRolesHelper.CheckAddonConflict(addon, x)
                    && (Cleanser.CleansedCanGetAddon.GetBool() || (!Cleanser.CleansedCanGetAddon.GetBool() && !x.Is(CustomRoles.Cleansed)))
                    && ((OptionCanTargetCrew.GetBool() && x.IsCrewmate()) ||
                        (OptionCanTargetImpostor.GetBool() && x.GetCustomRole().IsImpostor()) ||
                        (OptionCanTargetNeutral.GetBool() && (x.GetCustomRole().IsNeutral() || x.IsNeutralKiller())))
                ).ToList();

            if (AllAlivePlayer.Count <= 0) return;

            bool helpfulAddon = GroupedAddons[AddonTypes.Helpful].Contains(addon);
            bool harmfulAddon = GroupedAddons[AddonTypes.Harmful].Contains(addon);

            if (helpfulAddon && OptionSellOnlyHarmfulToEvil.GetBool()) AllAlivePlayer.RemoveAll(x => !x.Is(Team.Crewmate));
            if (harmfulAddon && OptionSellOnlyHelpfulToCrew.GetBool()) AllAlivePlayer.RemoveAll(x => x.Is(Team.Crewmate));

            if (AllAlivePlayer.Count == 0)
            {
                player.Notify(Utils.ColorString(Utils.GetRoleColor(CustomRoles.Merchant), GetString("MerchantAddonSellFail")));
                return;
            }

            PlayerControl target = AllAlivePlayer.RandomElement();

            target.RpcSetCustomRole(addon);
            target.Notify(Utils.ColorString(Utils.GetRoleColor(CustomRoles.Merchant), GetString("MerchantAddonSell")));
            player.Notify(Utils.ColorString(Utils.GetRoleColor(CustomRoles.Merchant), GetString("MerchantAddonDelivered")));

            Utils.NotifyRoles(SpecifySeer: player, SpecifyTarget: target);
            Utils.NotifyRoles(SpecifySeer: target, SpecifyTarget: player);

            addonsSold[player.PlayerId]++;
        }

        public override bool OnCheckMurderAsTarget(PlayerControl killer, PlayerControl target)
        {
            if (IsBribedKiller(killer, target))
            {
                NotifyBribery(killer, target);
                return true;
            }

            if (GetCurrentAmountOfMoney(target.PlayerId) >= OptionMoneyRequiredToBribe.GetInt())
            {
                NotifyBribery(killer, target);
                bribedKiller[target.PlayerId].Add(killer.PlayerId);
                return true;
            }

            return false;
        }

        public static bool IsBribedKiller(PlayerControl killer, PlayerControl target) => bribedKiller[target.PlayerId].Contains(killer.PlayerId);

        private static void NotifyBribery(PlayerControl killer, PlayerControl target)
        {
            if (OptionGivesAllMoneyOnBribe.GetBool()) addonsSold[target.PlayerId] = 0;

            killer.Notify(Utils.ColorString(Utils.GetRoleColor(CustomRoles.Merchant), GetString("BribedByMerchant")));

            if (OptionNotifyBribery.GetBool())
            {
                target.Notify(Utils.ColorString(Utils.GetRoleColor(CustomRoles.Merchant), GetString("MerchantKillAttemptBribed")));
            }
        }
    }
}