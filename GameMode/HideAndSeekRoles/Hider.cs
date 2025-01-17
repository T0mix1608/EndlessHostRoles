﻿namespace EHR.GameMode.HideAndSeekRoles
{
    internal class Hider : RoleBase, IHideAndSeekRole
    {
        public static bool On;

        public static OptionItem Vision;
        public static OptionItem Speed;
        public static OptionItem TimeDecreaseOnTaskComplete;

        public override bool IsEnable => On;
        public Team Team => Team.Crewmate;
        public int Chance => 100;
        public int Count => Main.AllPlayerControls.Length;
        public float RoleSpeed => Speed.GetFloat();
        public float RoleVision => Vision.GetFloat();

        public static void SetupCustomOption()
        {
            TextOptionItem.Create(69_211_105, "Hider", TabGroup.CrewmateRoles)
                .SetGameMode(CustomGameMode.HideAndSeek)
                .SetHeader(true)
                .SetColor(new(52, 94, 235, byte.MaxValue));

            Vision = FloatOptionItem.Create(69_211_101, "HiderVision", new(0.05f, 5f, 0.05f), 1.25f, TabGroup.CrewmateRoles)
                .SetGameMode(CustomGameMode.HideAndSeek)
                .SetValueFormat(OptionFormat.Multiplier)
                .SetColor(new(52, 94, 235, byte.MaxValue));
            Speed = FloatOptionItem.Create(69_211_102, "HiderSpeed", new(0.05f, 5f, 0.05f), 1.25f, TabGroup.CrewmateRoles)
                .SetGameMode(CustomGameMode.HideAndSeek)
                .SetValueFormat(OptionFormat.Multiplier)
                .SetColor(new(52, 94, 235, byte.MaxValue));
            TimeDecreaseOnTaskComplete = IntegerOptionItem.Create(69_211_103, "TimeDecreaseOnTaskComplete", new(0, 60, 1), 5, TabGroup.CrewmateRoles)
                .SetGameMode(CustomGameMode.HideAndSeek)
                .SetValueFormat(OptionFormat.Seconds)
                .SetColor(new(52, 94, 235, byte.MaxValue));
        }

        public override void Add(byte playerId)
        {
            On = true;
        }

        public override void Init()
        {
            On = false;
        }

        public override void OnTaskComplete(PlayerControl pc, int completedTaskCount, int totalTaskCount)
        {
            HnSManager.TimeLeft -= TimeDecreaseOnTaskComplete.GetInt();
            pc.Notify(Translator.GetString("TimeDecreased"));
            if (60 - (HnSManager.TimeLeft % 60) <= TimeDecreaseOnTaskComplete.GetInt()) Utils.NotifyRoles();
        }
    }
}