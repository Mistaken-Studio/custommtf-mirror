// -----------------------------------------------------------------------
// <copyright file="Tau5Soldier.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using Mistaken.API.CustomRoles;
using Mistaken.API.Shield;

namespace Mistaken.CustomMTF.Classes
{
    /// <inheritdoc/>
    [CustomRole(RoleType.NtfCaptain)]
    public sealed class Tau5Soldier : MistakenCustomRole
    {
        /// <inheritdoc/>
        public override MistakenCustomRoles CustomRole => MistakenCustomRoles.TAU_5;

        /// <inheritdoc/>
        public override RoleType Role { get; set; } = RoleType.NtfCaptain;

        /// <inheritdoc/>
        public override int MaxHealth { get; set; } = 200;

        /// <inheritdoc/>
        public override string Name { get; set; } = PluginHandler.Instance.Translation.MTFTauSoldier;

        /// <inheritdoc/>
        public override string Description { get; set; } = PluginHandler.Instance.Translation.MTFTauSoldierDescription;

        // /// <inheritdoc/>
        // public override List<CustomAbility> CustomAbilities { get; set; } = new()
        // {
        //     CustomAbility.Get("Self Revive"),
        // };

        /// <inheritdoc/>
        public override bool KeepInventoryOnSpawn { get; set; } = false;

        /// <inheritdoc/>
        public override bool KeepRoleOnDeath { get; set; } = false;

        /// <inheritdoc/>
        public override bool RemovalKillsPlayer { get; set; } = false;

        /// <inheritdoc/>
        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.GunE11SR.ToString(),
            ItemType.ArmorHeavy.ToString(),
            ItemType.Medkit.ToString(),
            ItemType.Radio.ToString(),
            ItemType.GrenadeHE.ToString(),
        };

        /// <inheritdoc/>
        public override KeycardPermissions BuiltInPermissions =>
            KeycardPermissions.ExitGates |
            KeycardPermissions.AlphaWarhead |
            KeycardPermissions.Intercom |
            KeycardPermissions.Checkpoints |
            KeycardPermissions.ArmoryLevelOne |
            KeycardPermissions.ArmoryLevelTwo |
            KeycardPermissions.ArmoryLevelThree |
            KeycardPermissions.ContainmentLevelOne |
            KeycardPermissions.ContainmentLevelTwo |
            KeycardPermissions.ContainmentLevelThree;

        /// <inheritdoc/>
        public override bool SetLatestUnitName => true;

        /// <inheritdoc/>
        public override bool InfiniteAmmo => true;

        /// <inheritdoc/>
        public override string DisplayName => $"<color=#C00>{this.Name}</color>";

        /// <inheritdoc/>
        public override void Init()
        {
            base.Init();
            Instance = this;
        }

        internal static Tau5Soldier Instance { get; private set; }

        /// <inheritdoc/>
        protected override void RoleAdded(Player player)
        {
            base.RoleAdded(player);
            player.ArtificialHealth = 1;
            player.GetEffect(EffectType.MovementBoost).Intensity = 10;
            Shield.Ini<Miscellaneous.Tau5Shield>(player);
        }
    }
}
