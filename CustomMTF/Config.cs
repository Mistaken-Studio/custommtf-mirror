// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;
using Exiled.API.Interfaces;

namespace Mistaken.CustomMTF
{
    internal sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        [Description("If true then debug will be displayed")]
        public bool VerboseOutput { get; set; }

        [Description("Abilities Settings")]
        public float MedicGunBulletRecoveryTime { get; set; } = 70f;

        [Description("Classes Settings")]
        public float MtfMedicSpawnChance { get; set; } = 10f;

        public float MtfExplosivesSpecialistSpawnChance { get; set; } = 10f;

        public float MtfContainmentEnginnerSpawnPointTime { get; set; } = 1f;
    }
}
