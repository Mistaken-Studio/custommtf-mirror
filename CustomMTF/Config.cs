// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;
using Mistaken.Updater.Config;

namespace Mistaken.CustomMTF
{
    internal class Config : IAutoUpdatableConfig
    {
        public bool IsEnabled { get; set; } = true;

        [Description("If true then debug will be displayed")]
        public bool VerbouseOutput { get; set; }

        [Description("Abilities Settings")]
        public float MedicGunBulletRecoveryTime { get; set; } = 70f;

        [Description("Classes Settings")]
        public float MtfMedicSpawnChance { get; set; } = 10f;

        public float MtfExplosivesSpecialistSpawnChance { get; set; } = 10f;

        public float MtfContainmentEnginnerSpawnPointTime { get; set; } = 1f;

        [Description("Auto Update Settings")]
        public System.Collections.Generic.Dictionary<string, string> AutoUpdateConfig { get; set; } = new System.Collections.Generic.Dictionary<string, string>
        {
            { "Url", "https://git.mistaken.pl/api/v4/projects/23" },
            { "Token", string.Empty },
            { "Type", "GITLAB" },
            { "VerbouseOutput", "false" },
        };
    }
}
