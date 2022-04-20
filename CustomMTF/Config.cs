// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;
using Mistaken.Updater.Config;

namespace Mistaken.CustomMTF
{
    /// <inheritdoc/>
    public class Config : IAutoUpdatableConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether debug should be displayed.
        /// </summary>
        [Description("If true then debug will be displayed")]
        public bool VerbouseOutput { get; set; }

        /// <summary>
        /// Gets or sets time needed to recover one ammo in Medic Gun.
        /// </summary>
        [Description("Abilities Settings")]
        public float MedicGunBulletRecoveryTime { get; set; } = 70f;

        /// <summary>
        /// Gets or sets spawn chance of MTF Medic class.
        /// </summary>
        [Description("Classes Settings")]
        public float MtfMedicSpawnChance { get; set; } = 10f;

        /// <summary>
        /// Gets or sets spawn chance of MTF Explosives Specialist class.
        /// </summary>
        public float MtfExplosivesSpecialistSpawnChance { get; set; } = 10f;

        /// <summary>
        /// Gets or sets a time for earning one spawn point (in seconds).
        /// </summary>
        public float MtfContainmentEnginnerSpawnPointTime { get; set; } = 1f;

        /// <inheritdoc/>
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
