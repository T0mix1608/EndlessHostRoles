﻿using static EHR.Options;

namespace EHR.Roles.AddOns.Impostor
{
    internal class Madmate : IAddon
    {
        public AddonTypes Type => AddonTypes.Mixed;

        public void SetupCustomOption()
        {
            SetupAdtRoleOptions(15800, CustomRoles.Madmate, canSetNum: true, canSetChance: false);
            MadmateSpawnMode = StringOptionItem.Create(15810, "MadmateSpawnMode", MadmateSpawnModeStrings, 0, TabGroup.Addons)
                .SetParent(CustomRoleSpawnChances[CustomRoles.Madmate]);
            MadmateCountMode = StringOptionItem.Create(15811, "MadmateCountMode", MadmateCountModeStrings, 0, TabGroup.Addons)
                .SetParent(CustomRoleSpawnChances[CustomRoles.Madmate]);
            SheriffCanBeMadmate = BooleanOptionItem.Create(15812, "SheriffCanBeMadmate", false, TabGroup.Addons)
                .SetParent(CustomRoleSpawnChances[CustomRoles.Madmate]);
            MayorCanBeMadmate = BooleanOptionItem.Create(15813, "MayorCanBeMadmate", false, TabGroup.Addons)
                .SetParent(CustomRoleSpawnChances[CustomRoles.Madmate]);
            NGuesserCanBeMadmate = BooleanOptionItem.Create(15814, "NGuesserCanBeMadmate", false, TabGroup.Addons)
                .SetParent(CustomRoleSpawnChances[CustomRoles.Madmate]);
            MarshallCanBeMadmate = BooleanOptionItem.Create(15815, "MarshallCanBeMadmate", false, TabGroup.Addons)
                .SetParent(CustomRoleSpawnChances[CustomRoles.Madmate]);
            FarseerCanBeMadmate = BooleanOptionItem.Create(15816, "FarseerCanBeMadmate", false, TabGroup.Addons)
                .SetParent(CustomRoleSpawnChances[CustomRoles.Madmate]);
            SnitchCanBeMadmate = BooleanOptionItem.Create(15818, "SnitchCanBeMadmate", false, TabGroup.Addons)
                .SetParent(CustomRoleSpawnChances[CustomRoles.Madmate]);
            MadSnitchTasks = IntegerOptionItem.Create(15819, "MadSnitchTasks", new(0, 90, 1), 3, TabGroup.Addons)
                .SetParent(SnitchCanBeMadmate)
                .SetValueFormat(OptionFormat.Pieces);
            JudgeCanBeMadmate = BooleanOptionItem.Create(15820, "JudgeCanBeMadmate", false, TabGroup.Addons)
                .SetParent(CustomRoleSpawnChances[CustomRoles.Madmate]);
        }
    }
}
