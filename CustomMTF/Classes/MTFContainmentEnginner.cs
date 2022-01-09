// -----------------------------------------------------------------------
// <copyright file="MTFContainmentEnginner.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Mistaken.API.CustomRoles;
using Mistaken.API.Extensions;
using Mistaken.RoundLogger;

namespace Mistaken.CustomMTF.Classes
{
    /// <inheritdoc/>
    public class MTFContainmentEnginner : MistakenCustomRole
    {
        /// <summary>
        /// Gets the MTF containment enginner instance.
        /// </summary>
        public static MTFContainmentEnginner Instance { get; private set; }

        /// <inheritdoc/>
        public override MistakenCustomRoles CustomRole => MistakenCustomRoles.MTF_CONTAINMENT_ENGINNER;

        /// <inheritdoc/>
        public override RoleType Role { get; set; } = RoleType.NtfPrivate;

        /// <inheritdoc/>
        public override int MaxHealth { get; set; } = 100;

        /// <inheritdoc/>
        public override string Name { get; set; } = "MTF Containment Enginner";

        /// <inheritdoc/>
        public override string Description { get; set; } = "MTF Containment Enginner";

        /// <inheritdoc/>
        public override void Init()
        {
            Instance = this;
        }

        /// <inheritdoc/>
        public override void AddRole(Player player)
        {
            base.AddRole(player);
            MEC.Timing.CallDelayed(2, () =>
            {
                player.Ammo[ItemType.Ammo556x45] = 80;
                player.Ammo[ItemType.Ammo9x19] = 50;
            });
        }

        /// <inheritdoc/>
        protected override KeycardPermissions BuiltInPermissions =>
            KeycardPermissions.ContainmentLevelOne |
            KeycardPermissions.ContainmentLevelTwo |
            KeycardPermissions.ContainmentLevelThree |
            KeycardPermissions.ArmoryLevelOne |
            KeycardPermissions.AlphaWarhead |
            KeycardPermissions.Checkpoints |
            KeycardPermissions.Intercom;

        /// <inheritdoc/>
        protected override bool KeepInventoryOnSpawn { get; set; } = false;

        /// <inheritdoc/>
        protected override bool KeepRoleOnDeath { get; set; } = false;

        /// <inheritdoc/>
        protected override bool RemovalKillsPlayer { get; set; } = true;

        /// <inheritdoc/>
        protected override List<string> Inventory { get; set; } = new List<string>()
        {
            ItemType.GunE11SR.ToString(),
            ItemType.GunCOM18.ToString(),
            ItemType.Medkit.ToString(),
            ItemType.GrenadeHE.ToString(),
            ItemType.Radio.ToString(),
            ItemType.ArmorCombat.ToString(),
        };

        /// <inheritdoc/>
        protected override void RoleAdded(Player player)
        {
            player.SetSessionVariable(API.SessionVarType.BUILTIN_DOOR_ACCESS, this.BuiltInPermissions);
            RLogger.Log("MTF CONTAINMENT ENGINNER", "SPAWN", $"Player {player.PlayerToString()} is now a {this.Name}");
            player.SetGUI("cc_mtf_ce", API.GUI.PseudoGUIPosition.BOTTOM, string.Format(PluginHandler.Instance.Translation.PlayingAs, PluginHandler.Instance.Translation.MtfPrivateColor, PluginHandler.Instance.Translation.MtfContainmentEnginner));
        }

        /// <inheritdoc/>
        protected override void RoleRemoved(Player player)
        {
            player.SetSessionVariable(API.SessionVarType.BUILTIN_DOOR_ACCESS, null);
            RLogger.Log("MTF CONTAINMENT ENGINNER", "DEATH", $"Player {player.PlayerToString()} is no longer a {this.Name}");
            player.SetGUI("cc_mtf_ce", API.GUI.PseudoGUIPosition.BOTTOM, null);
        }
    }
}
