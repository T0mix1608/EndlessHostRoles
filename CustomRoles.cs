﻿namespace EHR;

public enum CustomRoles
{
    // Default
    Crewmate = 0,

    // Impostors (Vanilla)
    Impostor,
    Shapeshifter,

    // Vanilla Remakes
    ImpostorEHR,
    ShapeshifterEHR,

    // Impostors

    Hacker, // Anonymous
    AntiAdminer,
    Sans, // Arrogance
    Bard,
    Blackmailer,
    Bomber,
    BountyHunter,
    OverKiller, // Butcher
    Camouflager,
    Capitalism,
    Cantankerous,
    Changeling,
    Chronomancer,
    Cleaner,
    Commander,
    EvilDiviner, // Consigliere
    Consort,
    Councillor,
    Crewpostor,
    CursedWolf,
    Deathpact,
    Devourer,
    Disperser,
    Duellist,
    Dazzler,
    Escapee,
    Eraser,
    EvilGuesser,
    EvilTracker,
    FireWorks,
    Freezer,
    Gambler,
    Gangster,
    Generator,
    Godfather,
    Greedier,
    Hangman,
    Hitman,
    Inhibitor,
    Kamikaze,
    Kidnapper,
    Minimalism, // Killing Machine
    BallLightning, // Lightning
    Librarian,
    Lurker,
    Mafioso,
    Mastermind,
    Mafia, // Nemesis
    SerialKiller, // Mercenary
    Miner,
    Morphling,
    Assassin, // Ninja
    Nuker,
    Nullifier,
    Overheat,
    Parasite,
    Penguin,
    Puppeteer,
    QuickShooter,
    Refugee,
    RiftMaker,
    Saboteur,
    Sapper,
    Scavenger,
    Silencer,
    Sniper,
    ImperiusCurse, // Soul Catcher
    Swapster,
    Swiftclaw,
    Swooper,
    Stealth,
    TimeThief,
    BoobyTrap, // Trapster
    Trickster,
    Twister,
    Underdog,
    Undertaker,
    Vampire,
    Vindicator,
    Visionary,
    Warlock,
    Wildling,
    Witch,
    YinYanger,
    Zombie,

    // Crewmates (Vanilla)
    Engineer,
    GuardianAngel,
    Scientist,

    // Vanilla Remakes
    CrewmateEHR,
    EngineerEHR,
    GuardianAngelEHR,
    ScientistEHR,

    // Crewmates

    Addict,
    Adventurer,
    Aid,
    Alchemist,
    Altruist,
    Analyst,
    Autocrat,
    Beacon,
    Benefactor,
    Bodyguard,
    CameraMan,
    CyberStar, // Celebrity
    Chameleon,
    Cleanser,
    Convener,
    CopyCat,
    Bloodhound, // Coroner
    Crusader,
    Demolitionist,
    Deputy,
    Detective,
    Detour,
    Dictator,
    Doctor,
    DonutDelivery,
    Doormaster,
    DovesOfNeace,
    Drainer,
    Druid,
    Electric,
    Enigma,
    Escort,
    Express,
    Farseer,
    Divinator, // Fortune Teller
    Gaulois,
    Goose,
    Grenadier,
    GuessManagerRole,
    Guardian,
    Ignitor,
    Insight,
    ParityCop, // Inspector
    Jailor,
    Judge,
    Needy, // Lazy Guy
    Lighter,
    Lookout,
    Luckey,
    Markseeker,
    Marshall,
    Mathematician,
    Mayor,
    SabotageMaster, // Mechanic
    Medic,
    Mediumshiper,
    Merchant,
    Monitor,
    Mole,
    Monarch,
    Mortician,
    NiceEraser,
    NiceGuesser,
    NiceHacker,
    NiceSwapper,
    Nightmare,
    Observer,
    Oracle,
    Paranoia,
    Perceiver,
    Philantropist,
    Psychic,
    Rabbit,
    Randomizer,
    Ricochet,
    SecurityGuard,
    Sentinel,
    Sentry,
    Sheriff,
    Shiftguard,
    Snitch,
    Spiritualist,
    Speedrunner,
    SpeedBooster,
    Spy,
    SuperStar,
    TaskManager,
    Tether,
    TimeManager,
    TimeMaster,
    Tornado,
    Tracker,
    Transmitter,
    Transporter,
    Tracefinder,
    Tunneler,
    Ventguard,
    Veteran,
    SwordsMan, // Vigilante
    Witness,

    // Neutrals

    Agitater,
    Amnesiac,
    Arsonist,
    Bandit,
    Bargainer,
    BloodKnight,
    Bubble,
    Chemist,
    Cherokious,
    Collector,
    Deathknight,
    Gamer, // Demon
    Doppelganger,
    Doomsayer,
    Eclipse,
    Enderman,
    Executioner,
    Totocalcio, // Follower
    Glitch,
    God,
    FFF, // Hater
    HeadHunter,
    HexMaster,
    Hookshot,
    Imitator,
    Impartial,
    Innocent,
    Jackal,
    Jester,
    Jinx,
    Juggernaut,
    Konan,
    Lawyer,
    Magician,
    Mario,
    Maverick,
    Medusa,
    Mycologist,
    Necromancer,
    Opportunist,
    Patroller,
    Pelican,
    Pestilence,
    Phantom,
    Pickpocket,
    PlagueBearer,
    PlagueDoctor,
    Poisoner,
    Postman,
    Predator,
    Provocateur,
    Pursuer,
    Pyromaniac,
    QuizMaster,
    Reckless,
    Revolutionist,
    Ritualist,
    Rogue,
    Romantic,
    RuthlessRomantic,
    Samurai,
    SchrodingersCat,
    NSerialKiller, // Serial Killer
    Sidekick,
    Simon,
    SoulHunter,
    Spiritcaller,
    Sprayer,
    DarkHide, // Stalker
    Succubus,
    Sunnyboy,
    Terrorist,
    Tiger,
    Traitor,
    Vengeance,
    VengefulRomantic,
    Virus,
    Vulture,
    Wraith,
    Werewolf,
    WeaponMaster,
    Workaholic,

    // Solo Kombat
    KB_Normal,

    // FFA
    Killer,

    // Move And Stop
    Tasker,

    // Hot Potato
    Potato,

    // H&S
    Hider,
    Seeker,
    Fox,
    Troll,
    Jumper,
    Detector,
    Jet,
    Dasher,
    Locator,
    Venter,
    Agent,
    Taskinator,

    // GM
    GM,

    // ????
    Convict,


    // Sub-role after 500
    NotAssigned = 500,
    Antidote,
    AntiTP,
    Asthmatic,
    Autopsy,
    Avanger,
    Bait,
    Busy,
    Trapper, // Beartrap
    Bewilder,
    Bloodlust,
    Bloodmoon, // Ghost role
    Charmed,
    Circumvent,
    Cleansed,
    Clumsy,
    Contagious,
    Damocles,
    DeadlyQuota,
    Disco,
    Diseased,
    Dynamo,
    Unreportable, // Disregarded
    DoubleShot,
    Egoist,
    EvilSpirit,
    Flashman,
    Fool,
    Giant,
    Glow,
    Gravestone,
    Guesser,
    Haste,
    Haunter, // Ghost role
    Knighted,
    LastImpostor,
    Lazy,
    Lovers,
    Loyal,
    Lucky,
    Madmate,
    Magnet,
    Mare,
    Mimic,
    Minion, // Ghost role
    Mischievous,
    Necroview,
    Ntr, // Neptune
    Nimble,
    Oblivious,
    Onbound,
    Sleep,
    Specter, // Ghost role
    Physicist,
    Rascal,
    Reach,
    Recruit,
    DualPersonality, // Schizophrenic
    Seer,
    Sleuth,
    Sonar,
    Stained,
    Taskcounter,
    TicketsStealer, // Stealer
    Stressed,
    Swift,
    Sunglasses,
    Brakar, // Tiebreaker
    Torch,
    Truant,
    Undead,
    Unlucky,
    Warden, // Ghost role
    Watcher,
    Workhorse,
    Youtuber
}