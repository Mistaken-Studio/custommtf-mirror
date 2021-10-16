// -----------------------------------------------------------------------
// <copyright file="MTFContainmentEnginner.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Exiled.API.Features;
using Mistaken.API.CustomRoles;
using Mistaken.API.Extensions;
using Mistaken.RoundLogger;

namespace Mistaken.CustomMTF.Classes
{
    /// <inheritdoc/>
    public class MTFContainmentEnginner : MistakenCustomRole
    {
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
        protected override bool KeepInventoryOnSpawn { get; set; } = false;

        /// <inheritdoc/>
        protected override bool KeepRoleOnDeath { get; set; } = false;

        /// <inheritdoc/>
        protected override bool RemovalKillsPlayer { get; set; } = true;

        /// <inheritdoc/>
        protected override List<string> Inventory { get; set; } = new List<string>()
        {
            ItemType.KeycardContainmentEngineer.ToString(),
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
            RLogger.Log("MTF CONTAINMENT ENGINNER", "SPAWN", $"Player {player.PlayerToString()} is now a {this.Name}");
            player.SetGUI("cc_mtf_ce", API.GUI.PseudoGUIPosition.BOTTOM, $"<color=yellow>Grasz</color> jako <color=#70C3FF>{this.Name}</color>");
        }

        /// <inheritdoc/>
        protected override void RoleRemoved(Player player)
        {
            RLogger.Log("MTF CONTAINMENT ENGINNER", "DEATH", $"Player {player.PlayerToString()} is no longer a {this.Name}");
            player.SetGUI("cc_mtf_ce", API.GUI.PseudoGUIPosition.BOTTOM, null);
        }
    }
}
