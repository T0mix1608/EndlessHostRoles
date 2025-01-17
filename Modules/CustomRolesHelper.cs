using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AmongUs.GameOptions;
using EHR.Modules;
using EHR.Roles.Crewmate;
using EHR.Roles.Impostor;
using EHR.Roles.Neutral;
using UnityEngine;

namespace EHR;

internal static class CustomRolesHelper
{
    public static bool CanCheck = false;

    public static readonly List<CustomRoles> OnlySpawnsWithPetsRoleList =
    [
        CustomRoles.Tunneler,
        CustomRoles.Tornado,
        CustomRoles.Swiftclaw,
        CustomRoles.Adventurer,
        CustomRoles.Sentry,
        CustomRoles.Cherokious,
        CustomRoles.Chemist,

        // HnS
        CustomRoles.Jet,
        CustomRoles.Dasher
    ];

    public static RoleBase GetRoleClass(this CustomRoles role)
    {
        var roleClass = role switch
        {
            // Roles that use the same code as another role need to be handled here
            CustomRoles.Nuker => new Bomber(),
            CustomRoles.Undertaker => new Assassin(),
            CustomRoles.Chameleon => new Swooper(),
            CustomRoles.BloodKnight => new Wildling(),
            CustomRoles.HexMaster => new Witch(),
            CustomRoles.Imitator => new Greedier(),
            CustomRoles.Jinx => new CursedWolf(),
            CustomRoles.Juggernaut => new Sans(),
            CustomRoles.Medusa => new Cleaner(),
            CustomRoles.Poisoner => new Vampire(),
            CustomRoles.Reckless => new Sans(),
            CustomRoles.Ritualist => new EvilDiviner(),
            CustomRoles.Wraith => new Swooper(),
            CustomRoles.Goose => new Penguin(),

            // Else, the role class is the role name - if the class doesn't exist, it defaults to VanillaRole
            _ => Main.AllRoleClasses.FirstOrDefault(x => x.GetType().Name.Equals(role.ToString(), StringComparison.OrdinalIgnoreCase)) ?? new VanillaRole()
        };

        return Activator.CreateInstance(roleClass.GetType()) as RoleBase;
    }

    public static CustomRoles GetVNRole(this CustomRoles role, bool checkDesyncRole = false)
    {
        if (role.IsGhostRole()) return CustomRoles.GuardianAngel;
        if (role.IsVanilla()) return role;
        if (checkDesyncRole && role.GetDYRole() == RoleTypes.Impostor) return CustomRoles.Impostor;
        bool UsePets = Options.UsePets.GetBool();
        return role switch
        {
            CustomRoles.Sniper => UsePets ? CustomRoles.Impostor : CustomRoles.Shapeshifter,
            CustomRoles.Jester => Jester.JesterCanVent.GetBool() ? CustomRoles.Engineer : CustomRoles.Crewmate,
            CustomRoles.Mayor => Mayor.MayorHasPortableButton.GetBool() ? CustomRoles.Engineer : CustomRoles.Crewmate,
            CustomRoles.Monitor => Monitor.CanVent.GetBool() ? CustomRoles.Engineer : CustomRoles.Crewmate,
            CustomRoles.Vulture => Vulture.CanVent.GetBool() ? CustomRoles.Engineer : CustomRoles.Crewmate,
            CustomRoles.Opportunist => CustomRoles.Engineer,
            CustomRoles.Vindicator => CustomRoles.Impostor,
            CustomRoles.Snitch => CustomRoles.Crewmate,
            CustomRoles.ParityCop => CustomRoles.Crewmate,
            CustomRoles.Marshall => CustomRoles.Crewmate,
            CustomRoles.SabotageMaster => CustomRoles.Engineer,
            CustomRoles.Mafia => Options.LegacyMafia.GetBool() ? CustomRoles.Shapeshifter : CustomRoles.Impostor,
            CustomRoles.Terrorist => CustomRoles.Engineer,
            CustomRoles.Executioner => Executioner.CRoleChangeRoles[Executioner.ChangeRolesAfterTargetKilled.GetValue()].GetVNRole(checkDesyncRole: true),
            CustomRoles.Lawyer => CustomRoles.Crewmate,
            CustomRoles.NiceSwapper => CustomRoles.Crewmate,
            CustomRoles.Ignitor => CustomRoles.Crewmate,
            CustomRoles.Jailor => UsePets && Jailor.UsePet.GetBool() ? CustomRoles.Crewmate : CustomRoles.Impostor,
            CustomRoles.Vampire => CustomRoles.Impostor,
            CustomRoles.BountyHunter => CustomRoles.Impostor,
            CustomRoles.Trickster => CustomRoles.Impostor,
            CustomRoles.Witch => CustomRoles.Impostor,
            CustomRoles.ShapeshifterEHR => CustomRoles.Shapeshifter,
            CustomRoles.Agitater => CustomRoles.Impostor,
            CustomRoles.ImpostorEHR => CustomRoles.Impostor,
            CustomRoles.EvilDiviner => CustomRoles.Impostor,
            CustomRoles.Wildling => Wildling.CanShapeshiftOpt.GetBool() ? CustomRoles.Shapeshifter : CustomRoles.Impostor,
            CustomRoles.Morphling => CustomRoles.Shapeshifter,
            CustomRoles.Warlock => UsePets ? CustomRoles.Impostor : CustomRoles.Shapeshifter,
            CustomRoles.SerialKiller => CustomRoles.Impostor,
            CustomRoles.FireWorks => CustomRoles.Shapeshifter,
            CustomRoles.SpeedBooster => CustomRoles.Crewmate,
            CustomRoles.Dictator => CustomRoles.Crewmate,
            CustomRoles.Inhibitor => CustomRoles.Impostor,
            CustomRoles.Kidnapper => CustomRoles.Shapeshifter,
            CustomRoles.Overheat => UsePets ? CustomRoles.Impostor : CustomRoles.Shapeshifter,
            CustomRoles.Generator => CustomRoles.Shapeshifter,
            CustomRoles.Blackmailer => CustomRoles.Impostor,
            CustomRoles.Commander => CustomRoles.Shapeshifter,
            CustomRoles.Freezer => CustomRoles.Shapeshifter,
            CustomRoles.Changeling => CustomRoles.Shapeshifter,
            CustomRoles.Swapster => CustomRoles.Shapeshifter,
            CustomRoles.Kamikaze => CustomRoles.Impostor,
            CustomRoles.Librarian => CustomRoles.Shapeshifter,
            CustomRoles.Cantankerous => CustomRoles.Impostor,
            CustomRoles.Swiftclaw => CustomRoles.Impostor,
            CustomRoles.YinYanger => CustomRoles.Impostor,
            CustomRoles.Duellist => CustomRoles.Shapeshifter,
            CustomRoles.Consort => CustomRoles.Impostor,
            CustomRoles.Mafioso => CustomRoles.Impostor,
            CustomRoles.Chronomancer => CustomRoles.Impostor,
            CustomRoles.Nullifier => CustomRoles.Impostor,
            CustomRoles.Stealth => CustomRoles.Impostor,
            CustomRoles.Penguin => CustomRoles.Impostor,
            CustomRoles.Sapper => UsePets ? CustomRoles.Impostor : CustomRoles.Shapeshifter,
            CustomRoles.Mastermind => CustomRoles.Impostor,
            CustomRoles.RiftMaker => UsePets ? CustomRoles.Impostor : CustomRoles.Shapeshifter,
            CustomRoles.Gambler => CustomRoles.Impostor,
            CustomRoles.Hitman => CustomRoles.Shapeshifter,
            CustomRoles.Saboteur => CustomRoles.Impostor,
            CustomRoles.Doctor => CustomRoles.Scientist,
            CustomRoles.ScientistEHR => CustomRoles.Scientist,
            CustomRoles.Tracefinder => CustomRoles.Scientist,
            CustomRoles.Puppeteer => CustomRoles.Impostor,
            CustomRoles.TimeThief => CustomRoles.Impostor,
            CustomRoles.EvilTracker => CustomRoles.Shapeshifter,
            CustomRoles.Paranoia => UsePets ? CustomRoles.Crewmate : CustomRoles.Engineer,
            CustomRoles.EngineerEHR => CustomRoles.Engineer,
            CustomRoles.TimeMaster => UsePets ? CustomRoles.Crewmate : CustomRoles.Engineer,
            CustomRoles.CrewmateEHR => CustomRoles.Crewmate,
            CustomRoles.Cleanser => CustomRoles.Crewmate,
            CustomRoles.Miner => UsePets ? CustomRoles.Impostor : CustomRoles.Shapeshifter,
            CustomRoles.Psychic => CustomRoles.Crewmate,
            CustomRoles.Needy => CustomRoles.Crewmate,
            CustomRoles.Twister => UsePets ? CustomRoles.Impostor : CustomRoles.Shapeshifter,
            CustomRoles.SuperStar => CustomRoles.Crewmate,
            CustomRoles.Hacker => CustomRoles.Shapeshifter,
            CustomRoles.Visionary => CustomRoles.Shapeshifter,
            CustomRoles.Assassin => UsePets ? CustomRoles.Impostor : CustomRoles.Shapeshifter,
            CustomRoles.Undertaker => UsePets ? CustomRoles.Impostor : CustomRoles.Shapeshifter,
            CustomRoles.Luckey => CustomRoles.Crewmate,
            CustomRoles.CyberStar => CustomRoles.Crewmate,
            CustomRoles.Demolitionist => CustomRoles.Crewmate,
            CustomRoles.Ventguard => CustomRoles.Engineer,
            CustomRoles.Express => CustomRoles.Crewmate,
            CustomRoles.NiceEraser => CustomRoles.Crewmate,
            CustomRoles.TaskManager => CustomRoles.Crewmate,
            CustomRoles.Adventurer => CustomRoles.Engineer,
            CustomRoles.Randomizer => CustomRoles.Crewmate,
            CustomRoles.Beacon => CustomRoles.Crewmate,
            CustomRoles.Rabbit => CustomRoles.Crewmate,
            CustomRoles.Shiftguard => CustomRoles.Crewmate,
            CustomRoles.Mole => UsePets ? CustomRoles.Crewmate : CustomRoles.Engineer,
            CustomRoles.Markseeker => CustomRoles.Crewmate,
            CustomRoles.Sentinel => UsePets ? CustomRoles.Crewmate : CustomRoles.Engineer,
            CustomRoles.Electric => CustomRoles.Crewmate,
            CustomRoles.Philantropist => CustomRoles.Crewmate,
            CustomRoles.Tornado => CustomRoles.Crewmate,
            CustomRoles.Druid => UsePets ? CustomRoles.Crewmate : CustomRoles.Engineer,
            CustomRoles.Insight => CustomRoles.Crewmate,
            CustomRoles.Tunneler => CustomRoles.Crewmate,
            CustomRoles.Detour => CustomRoles.Crewmate,
            CustomRoles.Drainer => CustomRoles.Engineer,
            CustomRoles.Benefactor => CustomRoles.Crewmate,
            CustomRoles.GuessManagerRole => CustomRoles.Crewmate,
            CustomRoles.Altruist => CustomRoles.Crewmate,
            CustomRoles.Transmitter => CustomRoles.Crewmate,
            CustomRoles.Autocrat => CustomRoles.Crewmate,
            CustomRoles.Sentry => CustomRoles.Crewmate,
            CustomRoles.Perceiver => UsePets ? CustomRoles.Crewmate : CustomRoles.Engineer,
            CustomRoles.Convener => UsePets ? CustomRoles.Crewmate : CustomRoles.Engineer,
            CustomRoles.Mathematician => CustomRoles.Crewmate,
            CustomRoles.Nightmare => CustomRoles.Crewmate,
            CustomRoles.CameraMan => UsePets ? CustomRoles.Crewmate : CustomRoles.Engineer,
            CustomRoles.Spy => CustomRoles.Crewmate,
            CustomRoles.Ricochet => CustomRoles.Crewmate,
            CustomRoles.Tether => UsePets ? CustomRoles.Crewmate : CustomRoles.Engineer,
            CustomRoles.Doormaster => UsePets ? CustomRoles.Crewmate : CustomRoles.Engineer,
            CustomRoles.Aid => UsePets && Aid.UsePet.GetBool() ? CustomRoles.Crewmate : CustomRoles.Impostor,
            CustomRoles.Escort => UsePets && Escort.UsePet.GetBool() ? CustomRoles.Crewmate : CustomRoles.Impostor,
            CustomRoles.DonutDelivery => UsePets && DonutDelivery.UsePet.GetBool() ? CustomRoles.Crewmate : CustomRoles.Impostor,
            CustomRoles.Gaulois => UsePets && Gaulois.UsePet.GetBool() ? CustomRoles.Crewmate : CustomRoles.Impostor,
            CustomRoles.Analyst => UsePets && Analyst.UsePet.GetBool() ? CustomRoles.Crewmate : CustomRoles.Impostor,
            CustomRoles.Escapee => UsePets ? CustomRoles.Impostor : CustomRoles.Shapeshifter,
            CustomRoles.NiceGuesser => CustomRoles.Crewmate,
            CustomRoles.EvilGuesser => CustomRoles.Impostor,
            CustomRoles.Detective => CustomRoles.Crewmate,
            CustomRoles.God => CustomRoles.Crewmate,
            CustomRoles.GuardianAngelEHR => CustomRoles.GuardianAngel,
            CustomRoles.Zombie => CustomRoles.Impostor,
            CustomRoles.Mario => CustomRoles.Engineer,
            CustomRoles.AntiAdminer => AntiAdminer.EnableExtraAbility.GetBool() ? CustomRoles.Shapeshifter : CustomRoles.Impostor,
            CustomRoles.Sans => CustomRoles.Impostor,
            CustomRoles.Bomber => UsePets ? CustomRoles.Impostor : CustomRoles.Shapeshifter,
            CustomRoles.Nuker => UsePets ? CustomRoles.Impostor : CustomRoles.Shapeshifter,
            CustomRoles.BoobyTrap => CustomRoles.Impostor,
            CustomRoles.Scavenger => CustomRoles.Impostor,
            CustomRoles.Transporter => CustomRoles.Crewmate,
            CustomRoles.Veteran => UsePets ? CustomRoles.Crewmate : CustomRoles.Engineer,
            CustomRoles.Capitalism => CustomRoles.Impostor,
            CustomRoles.Bodyguard => CustomRoles.Crewmate,
            CustomRoles.Grenadier => UsePets ? CustomRoles.Crewmate : CustomRoles.Engineer,
            CustomRoles.Lighter => UsePets ? CustomRoles.Crewmate : CustomRoles.Engineer,
            CustomRoles.SecurityGuard => UsePets ? CustomRoles.Crewmate : CustomRoles.Engineer,
            CustomRoles.Gangster => CustomRoles.Impostor,
            CustomRoles.Cleaner => CustomRoles.Impostor,
            CustomRoles.Konan => CustomRoles.Crewmate,
            CustomRoles.Divinator => CustomRoles.Crewmate,
            CustomRoles.Oracle => CustomRoles.Crewmate,
            CustomRoles.BallLightning => CustomRoles.Impostor,
            CustomRoles.Greedier => CustomRoles.Impostor,
            CustomRoles.Workaholic => CustomRoles.Engineer,
            CustomRoles.Speedrunner => CustomRoles.Crewmate,
            CustomRoles.CursedWolf => CustomRoles.Impostor,
            CustomRoles.Collector => CustomRoles.Crewmate,
            CustomRoles.ImperiusCurse => CustomRoles.Shapeshifter,
            CustomRoles.QuickShooter => UsePets ? CustomRoles.Impostor : CustomRoles.Shapeshifter,
            CustomRoles.Eraser => CustomRoles.Impostor,
            CustomRoles.OverKiller => CustomRoles.Impostor,
            CustomRoles.Hangman => CustomRoles.Shapeshifter,
            CustomRoles.Sunnyboy => CustomRoles.Scientist,
            CustomRoles.Phantom => Options.PhantomCanVent.GetBool() ? CustomRoles.Engineer : CustomRoles.Crewmate,
            CustomRoles.Judge => CustomRoles.Crewmate,
            CustomRoles.Councillor => CustomRoles.Impostor,
            CustomRoles.Mortician => CustomRoles.Crewmate,
            CustomRoles.Mediumshiper => CustomRoles.Crewmate,
            CustomRoles.Bard => CustomRoles.Impostor,
            CustomRoles.Swooper => CustomRoles.Impostor,
            CustomRoles.Crewpostor => CustomRoles.Engineer,
            CustomRoles.Cherokious => CustomRoles.Engineer,
            CustomRoles.Observer => CustomRoles.Crewmate,
            CustomRoles.DovesOfNeace => UsePets ? CustomRoles.Crewmate : CustomRoles.Engineer,
            CustomRoles.Disperser => UsePets ? CustomRoles.Impostor : CustomRoles.Shapeshifter,
            CustomRoles.Camouflager => CustomRoles.Shapeshifter,
            CustomRoles.Dazzler => CustomRoles.Shapeshifter,
            CustomRoles.Devourer => CustomRoles.Shapeshifter,
            CustomRoles.Deathpact => CustomRoles.Shapeshifter,
            CustomRoles.Bloodhound => CustomRoles.Crewmate,
            CustomRoles.Tracker => CustomRoles.Crewmate,
            CustomRoles.Deathknight => CustomRoles.Crewmate,
            CustomRoles.Merchant => CustomRoles.Crewmate,
            CustomRoles.Lookout => CustomRoles.Crewmate,
            CustomRoles.Guardian => CustomRoles.Crewmate,
            CustomRoles.Enigma => CustomRoles.Crewmate,
            CustomRoles.Addict => CustomRoles.Engineer,
            CustomRoles.Alchemist => CustomRoles.Engineer, // Needs to vent to use the invisibility potion
            CustomRoles.Chameleon => CustomRoles.Engineer,
            CustomRoles.Lurker => CustomRoles.Impostor,
            CustomRoles.Doomsayer => CustomRoles.Crewmate,
            CustomRoles.Godfather => CustomRoles.Impostor,
            CustomRoles.Silencer => Silencer.SilenceMode.GetValue() == 1 ? CustomRoles.Shapeshifter : CustomRoles.Impostor,

            // Hide And Seek
            CustomRoles.Hider => CustomRoles.Crewmate,
            CustomRoles.Seeker => CustomRoles.Impostor,
            CustomRoles.Fox => CustomRoles.Crewmate,
            CustomRoles.Troll => CustomRoles.Crewmate,
            CustomRoles.Jumper => CustomRoles.Engineer,
            CustomRoles.Detector => CustomRoles.Crewmate,
            CustomRoles.Jet => CustomRoles.Crewmate,
            CustomRoles.Dasher => CustomRoles.Impostor,
            CustomRoles.Locator => CustomRoles.Impostor,
            CustomRoles.Venter => CustomRoles.Impostor,
            CustomRoles.Agent => CustomRoles.Impostor,
            CustomRoles.Taskinator => CustomRoles.Crewmate,

            _ => role.IsImpostor() ? CustomRoles.Impostor : CustomRoles.Crewmate
        };
    }

    public static CustomRoles GetErasedRole(this CustomRoles role)
    {
        if (role.IsVanilla()) return role;
        var vnRole = role.GetVNRole();
        if (role.GetDYRole() == RoleTypes.Impostor) vnRole = CustomRoles.Impostor;
        return vnRole switch
        {
            CustomRoles.Crewmate => CustomRoles.CrewmateEHR,
            CustomRoles.Engineer => CustomRoles.EngineerEHR,
            CustomRoles.Scientist => CustomRoles.ScientistEHR,
            CustomRoles.Impostor when role.IsCrewmate() => CustomRoles.CrewmateEHR,
            CustomRoles.Impostor => CustomRoles.ImpostorEHR,
            CustomRoles.Shapeshifter => CustomRoles.ShapeshifterEHR,
            _ => role.IsImpostor() ? CustomRoles.ImpostorEHR : CustomRoles.CrewmateEHR
        };
    }

    public static RoleTypes GetDYRole(this CustomRoles role, bool load = false)
    {
        bool UsePets = !load && Options.UsePets.GetBool();
        return role switch
        {
            // SoloKombat
            CustomRoles.KB_Normal => RoleTypes.Impostor,
            // FFA
            CustomRoles.Killer => RoleTypes.Impostor,
            // Move And Stop
            CustomRoles.Tasker => RoleTypes.Crewmate,
            // Hot Potato
            CustomRoles.Potato => RoleTypes.Crewmate,
            // Standard
            CustomRoles.Executioner => Executioner.CRoleChangeRoles[Executioner.ChangeRolesAfterTargetKilled.GetValue()].GetDYRole(),
            CustomRoles.Sheriff => UsePets && Sheriff.UsePet.GetBool() ? RoleTypes.GuardianAngel : RoleTypes.Impostor,
            CustomRoles.Crusader => UsePets && Crusader.UsePet.GetBool() ? RoleTypes.GuardianAngel : RoleTypes.Impostor,
            CustomRoles.CopyCat => UsePets && CopyCat.UsePet.GetBool() ? RoleTypes.GuardianAngel : RoleTypes.Impostor,
            CustomRoles.Refugee => RoleTypes.Impostor,
            CustomRoles.Amnesiac => RoleTypes.Impostor,
            CustomRoles.Agitater => RoleTypes.Impostor,
            CustomRoles.Monarch => UsePets && Monarch.UsePet.GetBool() ? RoleTypes.GuardianAngel : RoleTypes.Impostor,
            CustomRoles.Deputy => UsePets && Deputy.UsePet.GetBool() ? RoleTypes.GuardianAngel : RoleTypes.Impostor,
            CustomRoles.Arsonist => RoleTypes.Impostor,
            CustomRoles.Jackal => RoleTypes.Impostor,
            CustomRoles.Medusa => RoleTypes.Impostor,
            CustomRoles.Sidekick => RoleTypes.Impostor,
            CustomRoles.SwordsMan => UsePets && SwordsMan.UsePet.GetBool() ? RoleTypes.GuardianAngel : RoleTypes.Impostor,
            CustomRoles.Innocent => RoleTypes.Impostor,
            CustomRoles.Pelican => RoleTypes.Impostor,
            CustomRoles.Aid => UsePets && Aid.UsePet.GetBool() ? RoleTypes.GuardianAngel : RoleTypes.Impostor,
            CustomRoles.Escort => UsePets && Escort.UsePet.GetBool() ? RoleTypes.GuardianAngel : RoleTypes.Impostor,
            CustomRoles.DonutDelivery => UsePets && DonutDelivery.UsePet.GetBool() ? RoleTypes.GuardianAngel : RoleTypes.Impostor,
            CustomRoles.Gaulois => UsePets && Gaulois.UsePet.GetBool() ? RoleTypes.GuardianAngel : RoleTypes.Impostor,
            CustomRoles.Analyst => UsePets && Analyst.UsePet.GetBool() ? RoleTypes.GuardianAngel : RoleTypes.Impostor,
            CustomRoles.Witness => UsePets && Options.WitnessUsePet.GetBool() ? RoleTypes.GuardianAngel : RoleTypes.Impostor,
            CustomRoles.Goose => RoleTypes.Impostor,
            CustomRoles.Pursuer => RoleTypes.Impostor,
            CustomRoles.Revolutionist => RoleTypes.Impostor,
            CustomRoles.FFF => RoleTypes.Impostor,
            CustomRoles.Medic => UsePets && Medic.UsePet.GetBool() ? RoleTypes.GuardianAngel : RoleTypes.Impostor,
            CustomRoles.Gamer => RoleTypes.Impostor,
            CustomRoles.HexMaster => RoleTypes.Impostor,
            CustomRoles.Wraith => RoleTypes.Impostor,
            CustomRoles.Glitch => RoleTypes.Impostor,
            CustomRoles.Jailor => UsePets && Jailor.UsePet.GetBool() ? RoleTypes.GuardianAngel : RoleTypes.Impostor,
            CustomRoles.Juggernaut => RoleTypes.Impostor,
            CustomRoles.Jinx => RoleTypes.Impostor,
            CustomRoles.DarkHide => RoleTypes.Impostor,
            CustomRoles.Provocateur => RoleTypes.Impostor,
            CustomRoles.BloodKnight => RoleTypes.Impostor,
            CustomRoles.Poisoner => RoleTypes.Impostor,
            CustomRoles.NSerialKiller => RoleTypes.Impostor,
            CustomRoles.Rogue => RoleTypes.Impostor,
            CustomRoles.Patroller => RoleTypes.Impostor,
            CustomRoles.Simon => RoleTypes.Impostor,
            CustomRoles.Chemist => RoleTypes.Impostor,
            CustomRoles.Samurai => RoleTypes.Impostor,
            CustomRoles.QuizMaster => RoleTypes.Impostor,
            CustomRoles.Bargainer => RoleTypes.Impostor,
            CustomRoles.Tiger => RoleTypes.Impostor,
            CustomRoles.SoulHunter => RoleTypes.Impostor,
            CustomRoles.Enderman => RoleTypes.Impostor,
            CustomRoles.Mycologist => RoleTypes.Impostor,
            CustomRoles.Bubble => RoleTypes.Impostor,
            CustomRoles.Hookshot => RoleTypes.Impostor,
            CustomRoles.Sprayer => RoleTypes.Impostor,
            CustomRoles.PlagueDoctor => RoleTypes.Impostor,
            CustomRoles.Postman => RoleTypes.Impostor,
            CustomRoles.SchrodingersCat => RoleTypes.Impostor,
            CustomRoles.Impartial => RoleTypes.Impostor,
            CustomRoles.Predator => RoleTypes.Impostor,
            CustomRoles.Reckless => RoleTypes.Impostor,
            CustomRoles.Magician => RoleTypes.Impostor,
            CustomRoles.WeaponMaster => RoleTypes.Impostor,
            CustomRoles.Pyromaniac => RoleTypes.Impostor,
            CustomRoles.Eclipse => RoleTypes.Impostor,
            CustomRoles.Vengeance => RoleTypes.Impostor,
            CustomRoles.HeadHunter => RoleTypes.Impostor,
            CustomRoles.Imitator => RoleTypes.Impostor,
            CustomRoles.Werewolf => RoleTypes.Impostor,
            CustomRoles.Bandit => RoleTypes.Impostor,
            CustomRoles.Maverick => RoleTypes.Impostor,
            CustomRoles.Parasite => RoleTypes.Impostor,
            CustomRoles.Totocalcio => RoleTypes.Impostor,
            CustomRoles.Romantic => RoleTypes.Impostor,
            CustomRoles.VengefulRomantic => RoleTypes.Impostor,
            CustomRoles.RuthlessRomantic => RoleTypes.Impostor,
            CustomRoles.Succubus => RoleTypes.Impostor,
            CustomRoles.Necromancer => RoleTypes.Impostor,
            CustomRoles.Virus => RoleTypes.Impostor,
            CustomRoles.Farseer => UsePets && Farseer.UsePet.GetBool() ? RoleTypes.GuardianAngel : RoleTypes.Impostor,
            CustomRoles.Ritualist => RoleTypes.Impostor,
            CustomRoles.Pickpocket => RoleTypes.Impostor,
            CustomRoles.Traitor => RoleTypes.Impostor,
            CustomRoles.PlagueBearer => RoleTypes.Impostor,
            CustomRoles.Pestilence => RoleTypes.Impostor,
            CustomRoles.Spiritcaller => RoleTypes.Impostor,
            CustomRoles.Doppelganger => RoleTypes.Impostor,
            _ => RoleTypes.GuardianAngel
        };
    }

    public static bool IsAdditionRole(this CustomRoles role) => role > CustomRoles.NotAssigned;

    public static bool IsNonNK(this CustomRoles role, bool check = false) => (!check && role == CustomRoles.Arsonist && CanCheck && Options.IsLoaded && Options.ArsonistCanIgniteAnytime != null && !Options.ArsonistCanIgniteAnytime.GetBool()) || role is
        CustomRoles.Jester or
        CustomRoles.Postman or
        CustomRoles.SchrodingersCat or
        CustomRoles.Impartial or
        CustomRoles.Predator or
        CustomRoles.SoulHunter or
        CustomRoles.Terrorist or
        CustomRoles.Opportunist or
        CustomRoles.Executioner or
        CustomRoles.Mario or
        CustomRoles.Lawyer or
        CustomRoles.God or
        CustomRoles.Amnesiac or
        CustomRoles.Innocent or
        CustomRoles.Vulture or
        CustomRoles.Pursuer or
        CustomRoles.Revolutionist or
        CustomRoles.Provocateur or
        CustomRoles.FFF or
        CustomRoles.Workaholic or
        CustomRoles.Collector or
        CustomRoles.Sunnyboy or
        CustomRoles.Maverick or
        CustomRoles.Phantom or
        CustomRoles.Totocalcio or
        CustomRoles.Romantic or
        CustomRoles.VengefulRomantic or
        CustomRoles.Doomsayer or
        CustomRoles.Deathknight;

    public static bool IsNK(this CustomRoles role, bool check = false) => (role == CustomRoles.Arsonist && (check || !CanCheck || !Options.IsLoaded || Options.ArsonistCanIgniteAnytime == null || Options.ArsonistCanIgniteAnytime.GetBool())) || role is
        CustomRoles.Jackal or
        CustomRoles.Glitch or
        CustomRoles.Sidekick or
        CustomRoles.HexMaster or
        CustomRoles.Doppelganger or
        CustomRoles.Succubus or
        CustomRoles.Gamer or
        CustomRoles.Crewpostor or
        CustomRoles.Cherokious or
        CustomRoles.Necromancer or
        CustomRoles.Agitater or
        CustomRoles.Wraith or
        CustomRoles.Bandit or
        CustomRoles.Medusa or
        CustomRoles.Pelican or
        CustomRoles.Convict or
        CustomRoles.DarkHide or
        CustomRoles.Juggernaut or
        CustomRoles.Jinx or
        CustomRoles.Poisoner or
        CustomRoles.Refugee or
        CustomRoles.Simon or
        CustomRoles.Patroller or
        CustomRoles.Rogue or
        CustomRoles.Parasite or
        CustomRoles.NSerialKiller or
        CustomRoles.Chemist or
        CustomRoles.QuizMaster or
        CustomRoles.Samurai or
        CustomRoles.Bargainer or
        CustomRoles.Tiger or
        CustomRoles.Enderman or
        CustomRoles.Mycologist or
        CustomRoles.Bubble or
        CustomRoles.Hookshot or
        CustomRoles.Sprayer or
        CustomRoles.Magician or
        CustomRoles.WeaponMaster or
        CustomRoles.Reckless or
        CustomRoles.Eclipse or
        CustomRoles.Pyromaniac or
        CustomRoles.Vengeance or
        CustomRoles.HeadHunter or
        CustomRoles.Imitator or
        CustomRoles.Werewolf or
        CustomRoles.Ritualist or
        CustomRoles.Pickpocket or
        CustomRoles.PlagueDoctor or
        CustomRoles.Traitor or
        CustomRoles.Virus or
        CustomRoles.BloodKnight or
        CustomRoles.Spiritcaller or
        CustomRoles.RuthlessRomantic or
        CustomRoles.PlagueBearer or
        CustomRoles.Pestilence;

    public static bool IsSnitchTarget(this CustomRoles role) => role.IsNK() || role.Is(Team.Impostor);

    public static bool IsGhostRole(this CustomRoles role) => role == CustomRoles.EvilSpirit || GhostRolesManager.CreateGhostRoleInstance(role, check: true) != null;

    public static bool IsNE(this CustomRoles role) => role is
        CustomRoles.Jester or
        CustomRoles.Gamer or
        CustomRoles.Arsonist or
        CustomRoles.Executioner or
        CustomRoles.Doomsayer or
        CustomRoles.Innocent;

    public static bool IsNB(this CustomRoles role) => role is
        CustomRoles.Opportunist or
        CustomRoles.Lawyer or
        CustomRoles.Amnesiac or
        CustomRoles.God or
        CustomRoles.Postman or
        CustomRoles.SchrodingersCat or
        CustomRoles.Predator or
        CustomRoles.Pursuer or
        CustomRoles.FFF or
        CustomRoles.Sunnyboy or
        CustomRoles.Maverick or
        CustomRoles.Romantic or
        CustomRoles.VengefulRomantic or
        CustomRoles.Totocalcio;

    public static bool IsNC(this CustomRoles role) => role is
        CustomRoles.Mario or
        CustomRoles.Terrorist or
        CustomRoles.Revolutionist or
        CustomRoles.Impartial or
        CustomRoles.Vulture or
        CustomRoles.Phantom or
        CustomRoles.Workaholic or
        CustomRoles.Collector or
        CustomRoles.Provocateur;

    public static bool IsCK(this CustomRoles role) => role is
        CustomRoles.SwordsMan or
        CustomRoles.Veteran or
        CustomRoles.CopyCat or
        CustomRoles.Bodyguard or
        CustomRoles.Crusader or
        CustomRoles.NiceGuesser or
        CustomRoles.Sheriff or
        CustomRoles.Jailor;

    public static bool IsImpostor(this CustomRoles role) => role is
        CustomRoles.Impostor or
        CustomRoles.Godfather or
        CustomRoles.Shapeshifter or
        CustomRoles.ShapeshifterEHR or
        CustomRoles.ImpostorEHR or
        CustomRoles.EvilDiviner or
        CustomRoles.Wildling or
        CustomRoles.Silencer or
        CustomRoles.Morphling or
        CustomRoles.BountyHunter or
        CustomRoles.Vampire or
        CustomRoles.Witch or
        CustomRoles.Vindicator or
        CustomRoles.Zombie or
        CustomRoles.Warlock or
        CustomRoles.Assassin or
        CustomRoles.Undertaker or
        CustomRoles.Hacker or
        CustomRoles.Visionary or
        CustomRoles.Miner or
        CustomRoles.Escapee or
        CustomRoles.SerialKiller or
        CustomRoles.Overheat or
        CustomRoles.Underdog or
        CustomRoles.Inhibitor or
        CustomRoles.Kidnapper or
        CustomRoles.Generator or
        CustomRoles.Blackmailer or
        CustomRoles.Commander or
        CustomRoles.Freezer or
        CustomRoles.Changeling or
        CustomRoles.Swapster or
        CustomRoles.Kamikaze or
        CustomRoles.Librarian or
        CustomRoles.Cantankerous or
        CustomRoles.Swiftclaw or
        CustomRoles.Duellist or
        CustomRoles.YinYanger or
        CustomRoles.Consort or
        CustomRoles.Mafioso or
        CustomRoles.Nullifier or
        CustomRoles.Chronomancer or
        CustomRoles.Stealth or
        CustomRoles.Penguin or
        CustomRoles.Sapper or
        CustomRoles.Hitman or
        CustomRoles.RiftMaker or
        CustomRoles.Mastermind or
        CustomRoles.Gambler or
        CustomRoles.Councillor or
        CustomRoles.Saboteur or
        CustomRoles.Puppeteer or
        CustomRoles.TimeThief or
        CustomRoles.Trickster or
        CustomRoles.Mafia or
        CustomRoles.Minimalism or
        CustomRoles.FireWorks or
        CustomRoles.Sniper or
        CustomRoles.EvilTracker or
        CustomRoles.EvilGuesser or
        CustomRoles.AntiAdminer or
        CustomRoles.Sans or
        CustomRoles.Bomber or
        CustomRoles.Nuker or
        CustomRoles.Scavenger or
        CustomRoles.BoobyTrap or
        CustomRoles.Capitalism or
        CustomRoles.Gangster or
        CustomRoles.Cleaner or
        CustomRoles.BallLightning or
        CustomRoles.Greedier or
        CustomRoles.CursedWolf or
        CustomRoles.ImperiusCurse or
        CustomRoles.QuickShooter or
        CustomRoles.Eraser or
        CustomRoles.OverKiller or
        CustomRoles.Hangman or
        CustomRoles.Bard or
        CustomRoles.Swooper or
        CustomRoles.Disperser or
        CustomRoles.Dazzler or
        CustomRoles.Deathpact or
        CustomRoles.Devourer or
        CustomRoles.Camouflager or
        CustomRoles.Twister or
        CustomRoles.Lurker;

    public static bool IsNeutral(this CustomRoles role, bool check = false) => role.IsNK(check: check) || role.IsNonNK(check: check);

    public static bool IsAbleToBeSidekicked(this CustomRoles role) => role.GetDYRole() == RoleTypes.Impostor && !role.IsImpostor() && !role.IsRecruitingRole() && role is not
        CustomRoles.Deathknight and not
        CustomRoles.Gangster;

    public static bool IsEvilAddon(this CustomRoles role) => role is
        CustomRoles.Madmate or
        CustomRoles.Egoist or
        CustomRoles.Charmed or
        CustomRoles.Recruit or
        CustomRoles.Contagious or
        CustomRoles.Rascal;

    public static bool IsRecruitingRole(this CustomRoles role) => role is
        CustomRoles.Jackal or
        CustomRoles.Succubus or
        CustomRoles.Necromancer or
        CustomRoles.Virus or
        CustomRoles.Spiritcaller;

    public static bool IsMadmate(this CustomRoles role) => role is
        CustomRoles.Crewpostor or
        CustomRoles.Convict or
        CustomRoles.Refugee or
        CustomRoles.Parasite;

    public static bool IsTasklessCrewmate(this CustomRoles role) => !role.UsesPetInsteadOfKill() && role is
        CustomRoles.Sheriff or
        CustomRoles.Medic or
        CustomRoles.CopyCat or
        CustomRoles.Crusader or
        CustomRoles.Aid or
        CustomRoles.Escort or
        CustomRoles.DonutDelivery or
        CustomRoles.Gaulois or
        CustomRoles.Analyst or
        CustomRoles.Witness or
        CustomRoles.Monarch or
        CustomRoles.Jailor or
        CustomRoles.Farseer or
        CustomRoles.SwordsMan or
        CustomRoles.Deputy;

    public static bool PetActivatedAbility(this CustomRoles role)
    {
        if (!Options.UsePets.GetBool()) return false;
        if (role.UsesPetInsteadOfKill()) return true;

        var type = role.GetRoleClass().GetType();
        return type.GetMethod("OnPet")?.DeclaringType == type;
    }

    public static bool UsesPetInsteadOfKill(this CustomRoles role) => Options.UsePets.GetBool() && role switch
    {
        CustomRoles.Gaulois when Gaulois.UsePet.GetBool() => true,
        CustomRoles.Aid when Aid.UsePet.GetBool() => true,
        CustomRoles.Escort when Escort.UsePet.GetBool() => true,
        CustomRoles.DonutDelivery when DonutDelivery.UsePet.GetBool() => true,
        CustomRoles.Analyst when Analyst.UsePet.GetBool() => true,
        CustomRoles.Jailor when Jailor.UsePet.GetBool() => true,
        CustomRoles.Sheriff when Sheriff.UsePet.GetBool() => true,
        CustomRoles.SwordsMan when SwordsMan.UsePet.GetBool() => true,
        CustomRoles.Medic when Medic.UsePet.GetBool() => true,
        CustomRoles.Monarch when Monarch.UsePet.GetBool() => true,
        CustomRoles.CopyCat when CopyCat.UsePet.GetBool() => true,
        CustomRoles.Farseer when Farseer.UsePet.GetBool() => true,
        CustomRoles.Deputy when Deputy.UsePet.GetBool() => true,
        CustomRoles.Crusader when Crusader.UsePet.GetBool() => true,
        CustomRoles.Witness when Options.WitnessUsePet.GetBool() => true,

        CustomRoles.Refugee => true,
        CustomRoles.Necromancer => true,
        CustomRoles.Deathknight => true,

        _ => false,
    };

    public static bool OnlySpawnsWithPets(this CustomRoles role) => OnlySpawnsWithPetsRoleList.Contains(role);

    public static bool NeedUpdateOnLights(this CustomRoles role) => (!role.UsesPetInsteadOfKill()) && (role.GetDYRole() != RoleTypes.GuardianAngel || role is
        CustomRoles.Convict or
        CustomRoles.Parasite or
        CustomRoles.Refugee or
        CustomRoles.Lighter or
        CustomRoles.SecurityGuard or
        CustomRoles.Ignitor or
        CustomRoles.Saboteur or
        CustomRoles.Inhibitor or
        CustomRoles.Gambler or
        CustomRoles.Deputy);

    public static bool IsBetrayalAddon(this CustomRoles role) => role is
        CustomRoles.Charmed or
        CustomRoles.Recruit or
        CustomRoles.Contagious or
        CustomRoles.Lovers or
        CustomRoles.Madmate or
        CustomRoles.Undead or
        CustomRoles.Egoist;

    public static bool IsImpOnlyAddon(this CustomRoles role) => role is
        CustomRoles.Mare or
        CustomRoles.LastImpostor or
        CustomRoles.Mare or
        CustomRoles.Mimic or
        CustomRoles.Damocles or
        CustomRoles.DeadlyQuota or
        CustomRoles.Circumvent or
        CustomRoles.TicketsStealer or
        CustomRoles.Taskcounter or
        CustomRoles.Swift;

    public static bool IsTaskBasedCrewmate(this CustomRoles role) => role is
        CustomRoles.Snitch or
        CustomRoles.Divinator or
        CustomRoles.Marshall or
        CustomRoles.TimeManager or
        CustomRoles.Ignitor or
        CustomRoles.Guardian or
        CustomRoles.Merchant or
        CustomRoles.Mayor or
        CustomRoles.Insight or
        CustomRoles.Transporter;

    public static bool IsNotKnightable(this CustomRoles role) => role is
        CustomRoles.Mayor or
        CustomRoles.Vindicator or
        CustomRoles.Dictator or
        CustomRoles.Knighted or
        CustomRoles.Glitch or
        CustomRoles.Pickpocket or
        CustomRoles.TicketsStealer;

    public static bool ForceCancelShapeshift(this CustomRoles role) => role is
        CustomRoles.Hangman or
        CustomRoles.Generator;

    public static bool CheckAddonConflict(CustomRoles role, PlayerControl pc) => role.IsAdditionRole() && (!Main.NeverSpawnTogetherCombos.TryGetValue(pc.GetCustomRole(), out var bannedAddon) || role != bannedAddon) && pc.GetCustomRole() is not CustomRoles.GuardianAngelEHR and not CustomRoles.God && !pc.Is(CustomRoles.Madmate) && !pc.Is(CustomRoles.Egoist) && !pc.Is(CustomRoles.GM) && role is not CustomRoles.Lovers && !pc.Is(CustomRoles.Needy) && (!pc.HasSubRole() || pc.GetCustomSubRoles().Count < Options.NoLimitAddonsNumMax.GetInt()) && (!Options.AddonCanBeSettings.TryGetValue(role, out var o) || ((o.Imp.GetBool() || !pc.GetCustomRole().IsImpostor()) && (o.Neutral.GetBool() || !pc.GetCustomRole().IsNeutral()) && (o.Crew.GetBool() || !pc.IsCrewmate()))) && role switch
    {
        CustomRoles.Sidekick when pc.Is(CustomRoles.Madmate) => false,
        CustomRoles.Madmate when pc.Is(CustomRoles.Sidekick) => false,
        CustomRoles.Egoist when pc.Is(CustomRoles.Sidekick) => false,
        CustomRoles.Autopsy when pc.Is(CustomRoles.Doctor) || pc.Is(CustomRoles.Tracefinder) || pc.Is(CustomRoles.Scientist) || pc.Is(CustomRoles.ScientistEHR) || pc.Is(CustomRoles.Sunnyboy) => false,
        CustomRoles.Necroview when pc.Is(CustomRoles.Doctor) => false,
        CustomRoles.Lazy when pc.Is(CustomRoles.Speedrunner) => false,
        CustomRoles.Mischievous when pc.Is(Team.Impostor) || pc.GetCustomRole().GetDYRole() != RoleTypes.Impostor || !pc.IsNeutralKiller() || Main.PlayerStates[pc.PlayerId].Role.CanUseSabotage(pc) => false,
        CustomRoles.Loyal when pc.IsCrewmate() && !Options.CrewCanBeLoyal.GetBool() => false,
        CustomRoles.DualPersonality when pc.IsCrewmate() && !Options.CrewCanBeDualPersonality.GetBool() => false,
        CustomRoles.Lazy when pc.Is(CustomRoles.Needy) || pc.Is(CustomRoles.Snitch) || pc.Is(CustomRoles.Marshall) || pc.Is(CustomRoles.Transporter) || pc.Is(CustomRoles.Guardian) => false,
        CustomRoles.Brakar when pc.Is(CustomRoles.Dictator) => false,
        CustomRoles.Stressed when !pc.IsCrewmate() || pc.GetCustomRole().IsTasklessCrewmate() => false,
        CustomRoles.Avanger when pc.GetCustomRole().IsImpostor() && !Options.ImpCanBeAvanger.GetBool() => false,
        CustomRoles.Lazy when pc.GetCustomRole().IsNeutral() || pc.GetCustomRole().IsImpostor() || (pc.GetCustomRole().IsTasklessCrewmate() && !Options.TasklessCrewCanBeLazy.GetBool()) || (pc.GetCustomRole().IsTaskBasedCrewmate() && !Options.TaskBasedCrewCanBeLazy.GetBool()) => false,
        CustomRoles.TicketsStealer or CustomRoles.Swift or CustomRoles.DeadlyQuota or CustomRoles.Damocles or CustomRoles.Mare when pc.Is(CustomRoles.Bomber) || pc.Is(CustomRoles.Nuker) || pc.Is(CustomRoles.BoobyTrap) || pc.Is(CustomRoles.Capitalism) => false,
        CustomRoles.TicketsStealer or CustomRoles.Mimic or CustomRoles.Swift or CustomRoles.DeadlyQuota or CustomRoles.Circumvent or CustomRoles.Damocles or CustomRoles.Taskcounter or CustomRoles.Mare when !pc.GetCustomRole().IsImpostor() => false,
        CustomRoles.Torch when !pc.IsCrewmate() || pc.Is(CustomRoles.Bewilder) || pc.Is(CustomRoles.Sunglasses) || pc.Is(CustomRoles.GuardianAngelEHR) => false,
        CustomRoles.Bewilder when pc.Is(CustomRoles.Torch) || pc.Is(CustomRoles.GuardianAngelEHR) || pc.Is(CustomRoles.Sunglasses) => false,
        CustomRoles.Sunglasses when pc.Is(CustomRoles.Torch) || pc.Is(CustomRoles.GuardianAngelEHR) || pc.Is(CustomRoles.Bewilder) => false,
        CustomRoles.Guesser when (pc.IsCrewmate() && (!Options.CrewCanBeGuesser.GetBool() || pc.Is(CustomRoles.NiceGuesser))) || (pc.GetCustomRole().IsNeutral() && !Options.NeutralCanBeGuesser.GetBool()) || (pc.GetCustomRole().IsImpostor() && !Options.ImpCanBeGuesser.GetBool()) || pc.Is(CustomRoles.EvilGuesser) || pc.Is(CustomRoles.GuardianAngelEHR) => false,
        CustomRoles.Madmate when !pc.CanBeMadmate() || pc.Is(CustomRoles.Egoist) || pc.Is(CustomRoles.Rascal) => false,
        CustomRoles.Oblivious when pc.Is(CustomRoles.Detective) || pc.Is(CustomRoles.Cleaner) || pc.Is(CustomRoles.Medusa) || pc.Is(CustomRoles.Mortician) || pc.Is(CustomRoles.Mediumshiper) || pc.Is(CustomRoles.GuardianAngelEHR) => false,
        CustomRoles.Youtuber when !pc.IsCrewmate() || pc.Is(CustomRoles.Madmate) || pc.Is(CustomRoles.Sheriff) || pc.Is(CustomRoles.GuardianAngelEHR) => false,
        CustomRoles.Egoist when !pc.GetCustomRole().IsImpostor() => false,
        CustomRoles.Damocles when pc.GetCustomRole() is CustomRoles.Bomber or CustomRoles.Nuker or CustomRoles.SerialKiller or CustomRoles.Cantankerous => false,
        CustomRoles.Damocles when !pc.CanUseKillButton() => false,
        CustomRoles.Flashman when pc.Is(CustomRoles.Swiftclaw) || pc.Is(CustomRoles.Giant) => false,
        CustomRoles.Giant when pc.Is(CustomRoles.Flashman) => false,
        CustomRoles.Necroview when pc.Is(CustomRoles.Visionary) => false,
        CustomRoles.Mimic when pc.Is(CustomRoles.Mafia) => false,
        CustomRoles.Rascal when !pc.IsCrewmate() => false,
        CustomRoles.Needy when pc.GetCustomRole().IsAdditionRole() => false,
        CustomRoles.TicketsStealer when pc.Is(CustomRoles.Vindicator) => false,
        CustomRoles.Bloodlust when !pc.GetCustomRole().IsCrewmate() || pc.GetCustomRole().IsTaskBasedCrewmate() => false,
        CustomRoles.Mare when pc.Is(CustomRoles.Underdog) => false,
        CustomRoles.Mare when pc.Is(CustomRoles.Inhibitor) => false,
        CustomRoles.Mare when pc.Is(CustomRoles.Saboteur) => false,
        CustomRoles.Mare when pc.Is(CustomRoles.Swift) => false,
        CustomRoles.Mare when pc.Is(CustomRoles.Mafia) => false,
        CustomRoles.Mare when pc.Is(CustomRoles.Sniper) => false,
        CustomRoles.Mare when pc.Is(CustomRoles.FireWorks) => false,
        CustomRoles.Mare when pc.Is(CustomRoles.Swooper) => false,
        CustomRoles.Mare when pc.Is(CustomRoles.Vampire) => false,
        CustomRoles.Torch when pc.Is(CustomRoles.Lighter) => false,
        CustomRoles.Torch when pc.Is(CustomRoles.Ignitor) => false,
        CustomRoles.Bewilder when pc.Is(CustomRoles.Lighter) => false,
        CustomRoles.Bewilder when pc.Is(CustomRoles.Ignitor) => false,
        CustomRoles.Sunglasses when pc.Is(CustomRoles.Lighter) => false,
        CustomRoles.Sunglasses when pc.Is(CustomRoles.Ignitor) => false,
        CustomRoles.Bait when pc.Is(CustomRoles.Demolitionist) => false,
        CustomRoles.Trapper when pc.Is(CustomRoles.Demolitionist) => false,
        CustomRoles.Lovers when pc.Is(CustomRoles.Romantic) => false,
        CustomRoles.Mare when pc.Is(CustomRoles.LastImpostor) => false,
        CustomRoles.Swift when pc.Is(CustomRoles.Mare) => false,
        CustomRoles.DualPersonality when (!pc.GetCustomRole().IsImpostor() && !pc.IsCrewmate()) || pc.Is(CustomRoles.Madmate) => false,
        CustomRoles.DualPersonality when pc.GetCustomRole().IsImpostor() && !Options.ImpCanBeDualPersonality.GetBool() => false,
        CustomRoles.Loyal when (!pc.GetCustomRole().IsImpostor() && !pc.IsCrewmate()) || pc.Is(CustomRoles.Madmate) => false,
        CustomRoles.Loyal when pc.GetCustomRole().IsImpostor() && !Options.ImpCanBeLoyal.GetBool() => false,
        CustomRoles.Seer when (pc.IsCrewmate() && (!Options.CrewCanBeSeer.GetBool() || pc.Is(CustomRoles.Mortician))) || (pc.GetCustomRole().IsNeutral() && !Options.NeutralCanBeSeer.GetBool()) || (pc.GetCustomRole().IsImpostor() && !Options.ImpCanBeSeer.GetBool()) || pc.Is(CustomRoles.GuardianAngelEHR) => false,
        CustomRoles.Bait when (pc.IsCrewmate() && !Options.CrewCanBeBait.GetBool()) || (pc.GetCustomRole().IsNeutral() && !Options.NeutralCanBeBait.GetBool()) || (pc.GetCustomRole().IsImpostor() && !Options.ImpCanBeBait.GetBool()) => false,
        CustomRoles.Bewilder when (pc.IsCrewmate() && !Options.CrewCanBeBewilder.GetBool()) || (pc.GetCustomRole().IsNeutral() && !Options.NeutralCanBeBewilder.GetBool()) || (pc.GetCustomRole().IsImpostor() && !Options.ImpCanBeBewilder.GetBool()) => false,
        CustomRoles.Sunglasses when (pc.IsCrewmate() && !Options.CrewCanBeSunglasses.GetBool()) || (pc.GetCustomRole().IsNeutral() && !Options.NeutralCanBeSunglasses.GetBool()) || (pc.GetCustomRole().IsImpostor() && !Options.ImpCanBeSunglasses.GetBool()) => false,
        CustomRoles.Autopsy when (pc.IsCrewmate() && !Options.CrewCanBeAutopsy.GetBool()) || (pc.GetCustomRole().IsNeutral() && !Options.NeutralCanBeAutopsy.GetBool()) || (pc.GetCustomRole().IsImpostor() && !Options.ImpCanBeAutopsy.GetBool()) => false,
        CustomRoles.Glow when (pc.IsCrewmate() && !Options.CrewCanBeGlow.GetBool()) || (pc.GetCustomRole().IsNeutral() && !Options.NeutralCanBeGlow.GetBool()) || (pc.GetCustomRole().IsImpostor() && !Options.ImpCanBeGlow.GetBool()) => false,
        CustomRoles.Trapper when (pc.IsCrewmate() && !Options.CrewCanBeTrapper.GetBool()) || (pc.GetCustomRole().IsNeutral() && !Options.NeutralCanBeTrapper.GetBool()) || (pc.GetCustomRole().IsImpostor() && !Options.ImpCanBeTrapper.GetBool()) => false,
        CustomRoles.Sidekick when pc.Is(CustomRoles.Madmate) => false,
        CustomRoles.Onbound when pc.Is(CustomRoles.SuperStar) => false,
        CustomRoles.Rascal when pc.Is(CustomRoles.SuperStar) || pc.Is(CustomRoles.Madmate) => false,
        CustomRoles.Madmate when pc.Is(CustomRoles.SuperStar) => false,
        CustomRoles.Gravestone when pc.Is(CustomRoles.SuperStar) => false,
        CustomRoles.Sidekick when pc.Is(CustomRoles.Jackal) => false,
        CustomRoles.Lucky when pc.Is(CustomRoles.Guardian) => false,
        CustomRoles.Bait when pc.Is(CustomRoles.GuardianAngelEHR) => false,
        CustomRoles.Bait when pc.Is(CustomRoles.Trapper) => false,
        CustomRoles.Trapper when pc.Is(CustomRoles.Bait) => false,
        CustomRoles.DualPersonality when pc.Is(CustomRoles.Dictator) => false,
        CustomRoles.Swift when pc.Is(CustomRoles.Swooper) => false,
        CustomRoles.Swift when pc.Is(CustomRoles.Vampire) => false,
        CustomRoles.Swift when pc.Is(CustomRoles.Scavenger) => false,
        CustomRoles.Swift when pc.Is(CustomRoles.Puppeteer) => false,
        CustomRoles.Swift when pc.Is(CustomRoles.Warlock) => false,
        CustomRoles.Swift when pc.Is(CustomRoles.EvilDiviner) => false,
        CustomRoles.Swift when pc.Is(CustomRoles.Witch) => false,
        CustomRoles.Swift when pc.Is(CustomRoles.Mafia) => false,
        CustomRoles.Reach when pc.Is(CustomRoles.Mafioso) => false,
        CustomRoles.Trapper when pc.Is(CustomRoles.GuardianAngelEHR) => false,
        CustomRoles.Reach when !pc.CanUseKillButton() => false,
        CustomRoles.Magnet when !pc.CanUseKillButton() => false,
        CustomRoles.Haste when !pc.CanUseKillButton() => false,
        CustomRoles.Watcher when (pc.IsCrewmate() && !Options.CrewCanBeWatcher.GetBool()) || (pc.GetCustomRole().IsNeutral() && !Options.NeutralCanBeWatcher.GetBool()) || (pc.GetCustomRole().IsImpostor() && !Options.ImpCanBeWatcher.GetBool()) => false,
        CustomRoles.Diseased when (pc.IsCrewmate() && !Options.CrewCanBeDiseased.GetBool()) || (pc.GetCustomRole().IsNeutral() && !Options.NeutralCanBeDiseased.GetBool()) || (pc.GetCustomRole().IsImpostor() && !Options.ImpCanBeDiseased.GetBool()) => false,
        CustomRoles.Antidote when (pc.IsCrewmate() && !Options.CrewCanBeAntidote.GetBool()) || (pc.GetCustomRole().IsNeutral() && !Options.NeutralCanBeAntidote.GetBool()) || (pc.GetCustomRole().IsImpostor() && !Options.ImpCanBeAntidote.GetBool()) => false,
        CustomRoles.Diseased when pc.Is(CustomRoles.Antidote) => false,
        CustomRoles.Antidote when pc.Is(CustomRoles.Diseased) => false,
        CustomRoles.Necroview when (pc.IsCrewmate() && !Options.CrewCanBeNecroview.GetBool()) || (pc.GetCustomRole().IsNeutral() && !Options.NeutralCanBeNecroview.GetBool()) || (pc.GetCustomRole().IsImpostor() && !Options.ImpCanBeNecroview.GetBool()) => false,
        CustomRoles.Oblivious when (pc.IsCrewmate() && !Options.CrewCanBeOblivious.GetBool()) || (pc.GetCustomRole().IsNeutral() && !Options.NeutralCanBeOblivious.GetBool()) || (pc.GetCustomRole().IsImpostor() && !Options.ImpCanBeOblivious.GetBool()) => false,
        CustomRoles.Brakar when (pc.IsCrewmate() && !Options.CrewCanBeTiebreaker.GetBool()) || (pc.GetCustomRole().IsNeutral() && !Options.NeutralCanBeTiebreaker.GetBool()) || (pc.GetCustomRole().IsImpostor() && !Options.ImpCanBeTiebreaker.GetBool()) => false,
        CustomRoles.Guesser when (pc.IsCrewmate() && !Options.CrewCanBeGuesser.GetBool() && pc.Is(CustomRoles.NiceGuesser)) || (pc.GetCustomRole().IsNeutral() && !Options.NeutralCanBeGuesser.GetBool()) || (pc.GetCustomRole().IsImpostor() && !Options.ImpCanBeGuesser.GetBool() && pc.Is(CustomRoles.EvilGuesser)) => false,
        CustomRoles.Onbound when (pc.IsCrewmate() && !Options.CrewCanBeOnbound.GetBool()) || (pc.GetCustomRole().IsNeutral() && !Options.NeutralCanBeOnbound.GetBool()) || (pc.GetCustomRole().IsImpostor() && !Options.ImpCanBeOnbound.GetBool()) => false,
        CustomRoles.Unreportable when (pc.IsCrewmate() && !Options.CrewCanBeUnreportable.GetBool()) || (pc.GetCustomRole().IsNeutral() && !Options.NeutralCanBeUnreportable.GetBool()) || (pc.GetCustomRole().IsImpostor() && !Options.ImpCanBeUnreportable.GetBool()) => false,
        CustomRoles.Lucky when (pc.IsCrewmate() && !Options.CrewCanBeLucky.GetBool()) || (pc.GetCustomRole().IsNeutral() && !Options.NeutralCanBeLucky.GetBool()) || (pc.GetCustomRole().IsImpostor() && !Options.ImpCanBeLucky.GetBool()) => false,
        CustomRoles.Unlucky when (pc.IsCrewmate() && !Options.CrewCanBeUnlucky.GetBool()) || (pc.GetCustomRole().IsNeutral() && !Options.NeutralCanBeUnlucky.GetBool()) || (pc.GetCustomRole().IsImpostor() && !Options.ImpCanBeUnlucky.GetBool()) => false,
        CustomRoles.Gravestone when (pc.IsCrewmate() && !Options.CrewCanBeGravestone.GetBool()) || (pc.GetCustomRole().IsNeutral() && !Options.NeutralCanBeGravestone.GetBool()) || (pc.GetCustomRole().IsImpostor() && !Options.ImpCanBeGravestone.GetBool()) => false,
        CustomRoles.Flashman or CustomRoles.Giant when pc.GetCustomRole() is CustomRoles.Swooper or CustomRoles.Wraith or CustomRoles.Chameleon or CustomRoles.Alchemist => false,
        CustomRoles.Bait when pc.Is(CustomRoles.Unreportable) => false,
        CustomRoles.Busy when !pc.GetTaskState().hasTasks => false,
        CustomRoles.Truant when pc.Is(CustomRoles.SoulHunter) => false,
        CustomRoles.Nimble when !pc.IsCrewmate() => false,
        CustomRoles.Physicist when !pc.IsCrewmate() => false,
        CustomRoles.Unreportable when pc.Is(CustomRoles.Bait) => false,
        CustomRoles.Oblivious when pc.Is(CustomRoles.Bloodhound) => false,
        CustomRoles.Oblivious when pc.Is(CustomRoles.Vulture) => false,
        CustomRoles.Vulture when pc.Is(CustomRoles.Oblivious) => false,
        CustomRoles.Guesser when pc.Is(CustomRoles.CopyCat) || pc.Is(CustomRoles.Doomsayer) => false,
        CustomRoles.CopyCat when pc.Is(CustomRoles.Guesser) => false,
        CustomRoles.DoubleShot when pc.Is(CustomRoles.CopyCat) => false,
        CustomRoles.CopyCat when pc.Is(CustomRoles.DoubleShot) => false,
        CustomRoles.Brakar when pc.Is(CustomRoles.Dictator) => false,
        CustomRoles.Lucky when pc.Is(CustomRoles.Luckey) => false,
        CustomRoles.Unlucky when pc.Is(CustomRoles.Luckey) => false,
        CustomRoles.Unlucky when pc.Is(CustomRoles.Lucky) => false,
        CustomRoles.Lucky when pc.Is(CustomRoles.Unlucky) => false,
        CustomRoles.Fool when (pc.IsCrewmate() && !Options.CrewCanBeFool.GetBool()) || (pc.GetCustomRole().IsNeutral() && !Options.NeutralCanBeFool.GetBool()) || (pc.GetCustomRole().IsImpostor() && !Options.ImpCanBeFool.GetBool()) || pc.Is(CustomRoles.SabotageMaster) || pc.Is(CustomRoles.GuardianAngelEHR) => false,
        CustomRoles.Bloodhound when pc.Is(CustomRoles.Oblivious) => false,
        CustomRoles.DoubleShot when (pc.Is(CustomRoleTypes.Impostor) && !Options.ImpCanBeDoubleShot.GetBool()) || (pc.Is(CustomRoleTypes.Crewmate) && !Options.CrewCanBeDoubleShot.GetBool()) || (pc.Is(CustomRoleTypes.Neutral) && !Options.NeutralCanBeDoubleShot.GetBool()) => false,
        CustomRoles.DoubleShot when !pc.Is(CustomRoles.EvilGuesser) && !pc.Is(CustomRoles.NiceGuesser) && !Options.GuesserMode.GetBool() => false,
        CustomRoles.DoubleShot when Options.ImpCanBeDoubleShot.GetBool() && !pc.Is(CustomRoles.Guesser) && !pc.Is(CustomRoles.EvilGuesser) && pc.Is(CustomRoleTypes.Impostor) && !Options.ImpostorsCanGuess.GetBool() => false,
        CustomRoles.DoubleShot when Options.CrewCanBeDoubleShot.GetBool() && !pc.Is(CustomRoles.Guesser) && !pc.Is(CustomRoles.NiceGuesser) && pc.Is(CustomRoleTypes.Crewmate) && !Options.CrewmatesCanGuess.GetBool() => false,
        CustomRoles.DoubleShot when Options.NeutralCanBeDoubleShot.GetBool() && !pc.Is(CustomRoles.Guesser) && ((pc.GetCustomRole().IsNonNK() && !Options.PassiveNeutralsCanGuess.GetBool()) || (pc.IsNeutralKiller() && !Options.NeutralKillersCanGuess.GetBool())) => false,
        _ => true
    };

    public static Team GetTeam(this CustomRoles role)
    {
        if (role.IsImpostorTeamV3()) return Team.Impostor;
        if (role.IsNeutralTeamV2()) return Team.Neutral;
        return role.IsCrewmateTeamV2() ? Team.Crewmate : Team.None;
    }

    public static bool Is(this CustomRoles role, Team team) => team switch
    {
        Team.Impostor => role.IsImpostorTeamV3(),
        Team.Neutral => role.IsNeutralTeamV2(),
        Team.Crewmate => role.IsCrewmateTeamV2(),
        Team.None => role.GetCountTypes() is CountTypes.OutOfGame or CountTypes.None || role == CustomRoles.GM,
        _ => false,
    };

    public static RoleTypes GetRoleTypes(this CustomRoles role)
    {
        if (role.GetDYRole() == RoleTypes.Impostor) return RoleTypes.Impostor;
        return role.GetVNRole() switch
        {
            CustomRoles.Impostor => RoleTypes.Impostor,
            CustomRoles.Scientist => RoleTypes.Scientist,
            CustomRoles.Engineer => RoleTypes.Engineer,
            CustomRoles.GuardianAngel => RoleTypes.GuardianAngel,
            CustomRoles.Shapeshifter => RoleTypes.Shapeshifter,
            CustomRoles.Crewmate => RoleTypes.Crewmate,
            _ => role.IsImpostor() ? RoleTypes.Impostor : RoleTypes.Crewmate,
        };
    }

    public static bool IsDesyncRole(this CustomRoles role) => role.GetDYRole() != RoleTypes.GuardianAngel;
    public static bool IsImpostorTeam(this CustomRoles role) => role.IsImpostor() || role == CustomRoles.Madmate;
    public static bool IsCrewmate(this CustomRoles role) => !role.IsImpostor() && !role.IsNeutral() && !role.IsMadmate();

    public static bool IsImpostorTeamV2(this CustomRoles role) => (role.IsImpostorTeam() && role != CustomRoles.Trickster && !role.IsConverted()) || role == CustomRoles.Rascal;
    public static bool IsNeutralTeamV2(this CustomRoles role) => role.IsConverted() || (role.IsNeutral() && role != CustomRoles.Madmate);
    public static bool IsCrewmateTeamV2(this CustomRoles role) => (!role.IsImpostorTeamV2() && !role.IsNeutralTeamV2()) || (role == CustomRoles.Trickster && !role.IsConverted());

    public static bool IsConverted(this CustomRoles role) => ((role is CustomRoles.Egoist) && (ParityCop.ParityCheckEgoistInt() == 1)) || role is
        CustomRoles.Charmed or
        CustomRoles.Recruit or
        CustomRoles.Contagious or
        CustomRoles.Sidekick or
        CustomRoles.Undead or
        CustomRoles.Lovers;

    public static bool IsRevealingRole(this CustomRoles role, PlayerControl target) =>
        ((role is CustomRoles.Mayor) && Mayor.MayorRevealWhenDoneTasks.GetBool() && target.AllTasksCompleted()) ||
        ((role is CustomRoles.SuperStar) && Options.EveryOneKnowSuperStar.GetBool()) ||
        ((role is CustomRoles.Marshall) && target.AllTasksCompleted()) ||
        ((role is CustomRoles.Workaholic) && Workaholic.WorkaholicVisibleToEveryone.GetBool()) ||
        ((role is CustomRoles.Doctor) && Options.DoctorVisibleToEveryone.GetBool()) ||
        ((role is CustomRoles.Bait) && Options.BaitNotification.GetBool() && ParityCop.ParityCheckBaitCountType.GetBool());

    public static bool IsImpostorTeamV3(this CustomRoles role) => role.IsImpostor() || role.IsMadmate();

    public static bool IsVanilla(this CustomRoles role) => role is
        CustomRoles.Crewmate or
        CustomRoles.Engineer or
        CustomRoles.Scientist or
        CustomRoles.GuardianAngel or
        CustomRoles.Impostor or
        CustomRoles.Shapeshifter;

    public static CustomRoleTypes GetCustomRoleTypes(this CustomRoles role)
    {
        CustomRoleTypes type = CustomRoleTypes.Crewmate;
        if (role.IsImpostor()) type = CustomRoleTypes.Impostor;
        if (role.IsNeutral()) type = CustomRoleTypes.Neutral;
        if (role.IsAdditionRole()) type = CustomRoleTypes.Addon;
        return type;
    }

    public static bool RoleExist(this CustomRoles role, bool countDead = false) => Main.AllPlayerControls.Any(x => x.Is(role) && (countDead || x.IsAlive()));

    public static int GetCount(this CustomRoles role)
    {
        if (role.IsVanilla())
        {
            if (Options.DisableVanillaRoles.GetBool()) return 0;
            var roleOpt = Main.NormalOptions.RoleOptions;
            return role switch
            {
                CustomRoles.Engineer => roleOpt.GetNumPerGame(RoleTypes.Engineer),
                CustomRoles.Scientist => roleOpt.GetNumPerGame(RoleTypes.Scientist),
                CustomRoles.Shapeshifter => roleOpt.GetNumPerGame(RoleTypes.Shapeshifter),
                CustomRoles.GuardianAngel => roleOpt.GetNumPerGame(RoleTypes.GuardianAngel),
                CustomRoles.Crewmate => roleOpt.GetNumPerGame(RoleTypes.Crewmate),
                _ => 0
            };
        }

        return Options.GetRoleCount(role);
    }

    public static int GetMode(this CustomRoles role) => Options.GetRoleSpawnMode(role);

    public static float GetChance(this CustomRoles role)
    {
        if (role.IsVanilla())
        {
            var roleOpt = Main.NormalOptions.RoleOptions;
            return role switch
            {
                CustomRoles.Engineer => roleOpt.GetChancePerGame(RoleTypes.Engineer),
                CustomRoles.Scientist => roleOpt.GetChancePerGame(RoleTypes.Scientist),
                CustomRoles.Shapeshifter => roleOpt.GetChancePerGame(RoleTypes.Shapeshifter),
                CustomRoles.GuardianAngel => roleOpt.GetChancePerGame(RoleTypes.GuardianAngel),
                CustomRoles.Crewmate => roleOpt.GetChancePerGame(RoleTypes.Crewmate),
                _ => 0
            } / 100f;
        }

        return Options.GetRoleChance(role);
    }

    public static bool IsEnable(this CustomRoles role) => role.GetCount() > 0;

    public static CountTypes GetCountTypes(this CustomRoles role) => role switch
    {
        CustomRoles.GM => CountTypes.OutOfGame,
        CustomRoles.Sidekick => CountTypes.Jackal,
        CustomRoles.Deathknight => CountTypes.Necromancer,
        CustomRoles.Parasite => CountTypes.Impostor,
        CustomRoles.Crewpostor => CountTypes.Impostor,
        CustomRoles.Refugee => CountTypes.Impostor,
        CustomRoles.DarkHide => DarkHide.SnatchesWin.GetBool() ? CountTypes.Crew : CountTypes.DarkHide,
        CustomRoles.Arsonist => !Options.ArsonistKeepsGameGoing.GetBool() ? CountTypes.Crew : CountTypes.Arsonist,
        CustomRoles.SchrodingersCat => SchrodingersCat.WinsWithCrewIfNotAttacked.GetBool() ? CountTypes.Crew : CountTypes.OutOfGame,

        _ => Enum.TryParse(role.ToString(), true, out CountTypes type)
            ? type
            : role.Is(Team.Impostor)
                ? CountTypes.Impostor
                : CountTypes.Crew
    };

    public static RoleOptionType GetRoleOptionType(this CustomRoles role)
    {
        if (role.IsImpostor()) return RoleOptionType.Impostor;
        if (role.IsCrewmate()) return role.GetDYRole(load: true) == RoleTypes.Impostor ? RoleOptionType.Crewmate_ImpostorBased : RoleOptionType.Crewmate_Normal;
        if (role.IsNeutral(check: true)) return role.IsNK(check: true) ? RoleOptionType.Neutral_Killing : RoleOptionType.Neutral_NonKilling;
        return RoleOptionType.Crewmate_Normal;
    }

    public static Color GetRoleOptionTypeColor(this RoleOptionType type) => type switch
    {
        RoleOptionType.Impostor => Palette.ImpostorRed,
        RoleOptionType.Crewmate_Normal => Palette.CrewmateBlue,
        RoleOptionType.Crewmate_ImpostorBased => Utils.GetRoleColor(CustomRoles.Sheriff),
        RoleOptionType.Neutral_NonKilling => Utils.GetRoleColor(CustomRoles.Sprayer),
        RoleOptionType.Neutral_Killing => Utils.GetRoleColor(CustomRoles.Traitor),
        _ => Utils.GetRoleColor(CustomRoles.SwordsMan)
    };

    public static TabGroup GetTabFromOptionType(this RoleOptionType type) => type switch
    {
        RoleOptionType.Impostor => TabGroup.ImpostorRoles,
        RoleOptionType.Crewmate_Normal => TabGroup.CrewmateRoles,
        RoleOptionType.Crewmate_ImpostorBased => TabGroup.CrewmateRoles,
        RoleOptionType.Neutral_NonKilling => TabGroup.NeutralRoles,
        RoleOptionType.Neutral_Killing => TabGroup.NeutralRoles,
        _ => TabGroup.OtherRoles
    };

    public static SimpleRoleOptionType GetSimpleRoleOptionType(this RoleOptionType type) => type switch
    {
        RoleOptionType.Impostor => SimpleRoleOptionType.Impostor,
        RoleOptionType.Crewmate_Normal => SimpleRoleOptionType.Crewmate,
        RoleOptionType.Crewmate_ImpostorBased => SimpleRoleOptionType.Crewmate,
        RoleOptionType.Neutral_NonKilling => SimpleRoleOptionType.NNK,
        RoleOptionType.Neutral_Killing => SimpleRoleOptionType.NK,
        _ => SimpleRoleOptionType.Crewmate
    };

    public static SimpleRoleOptionType GetSimpleRoleOptionType(this CustomRoles role)
    {
        if (role.IsImpostor()) return SimpleRoleOptionType.Impostor;
        if (role.IsCrewmate()) return SimpleRoleOptionType.Crewmate;
        if (role.IsNeutral(check: true)) return role.IsNK(check: true) ? SimpleRoleOptionType.NK : SimpleRoleOptionType.NNK;
        return SimpleRoleOptionType.Crewmate;
    }

    public static Color GetAddonTypeColor(this AddonTypes type) => type switch
    {
        AddonTypes.ImpOnly => Palette.ImpostorRed,
        AddonTypes.Helpful => Palette.CrewmateBlue,
        AddonTypes.Harmful => Utils.GetRoleColor(CustomRoles.Sprayer),
        AddonTypes.Mixed => Utils.GetRoleColor(CustomRoles.TaskManager),
        _ => Palette.CrewmateBlue
    };

    public static string ToColoredString(this CustomRoles role) => Utils.ColorString(Utils.GetRoleColor(role), Translator.GetString($"{role}"));
}

#pragma warning disable IDE0079
[SuppressMessage("ReSharper", "InconsistentNaming")]
#pragma warning restore IDE0079
public enum RoleOptionType
{
    Impostor,
    Crewmate_Normal,
    Crewmate_ImpostorBased,
    Neutral_NonKilling,
    Neutral_Killing
}

public enum SimpleRoleOptionType
{
    Crewmate,
    Impostor,
    NK,
    NNK
}

public enum AddonTypes
{
    ImpOnly,
    Helpful,
    Harmful,
    Mixed
}

public enum CustomRoleTypes
{
    Crewmate,
    Impostor,
    Neutral,
    Addon
}

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public enum CountTypes
{
    OutOfGame,
    None,
    Crew,
    Impostor,
    Bloodlust,
    Jackal,
    Doppelganger,
    Pelican,
    Gamer,
    BloodKnight,
    Poisoner,
    Succubus,
    Necromancer,
    HexMaster,
    Wraith,
    NSerialKiller,
    Rogue,
    Patroller,
    Simon,
    Chemist,
    QuizMaster,
    Samurai,
    Bargainer,
    Tiger,
    Enderman,
    Mycologist,
    Bubble,
    Hookshot,
    Sprayer,
    PlagueDoctor,
    Magician,
    WeaponMaster,
    Reckless,
    Pyromaniac,
    Eclipse,
    Vengeance,
    HeadHunter,
    Imitator,
    Werewolf,
    Juggernaut,
    Agitater,
    Virus,
    DarkHide,
    Jinx,
    Ritualist,
    Pickpocket,
    Traitor,
    Medusa,
    Bandit,
    Spiritcaller,
    RuthlessRomantic,
    Pestilence,
    PlagueBearer,
    Glitch,
    Arsonist,
    Cherokious
}