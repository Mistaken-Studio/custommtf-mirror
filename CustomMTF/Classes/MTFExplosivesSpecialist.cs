// -----------------------------------------------------------------------
// <copyright file="MTFExplosivesSpecialist.cs" company="Mistaken">
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
    public class MTFExplosivesSpecialist : MistakenCustomRole
    {
        /// <inheritdoc/>
        public override MistakenCustomRoles CustomRole => MistakenCustomRoles.MTF_EXPLOSIVE_SPECIALIST;

        /// <inheritdoc/>
        public override RoleType Role { get; set; } = RoleType.NtfSergeant;

        /// <inheritdoc/>
        public override int MaxHealth { get; set; } = 100;

        /// <inheritdoc/>
        public override string Name { get; set; } = "MTF Explosives Specialist";

        /// <inheritdoc/>
        public override string Description { get; set; } = "MTF Explosives Specialist";

        /// <inheritdoc/>
        public override void AddRole(Player player)
        {
            base.AddRole(player);
            MEC.Timing.CallDelayed(2, () =>
            {
                player.Ammo[ItemType.Ammo556x45] = 40;
                player.Ammo[ItemType.Ammo9x19] = 100;
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
            ItemType.KeycardNTFLieutenant.ToString(),
            ItemType.GunFSP9.ToString(),
            "Grenade Launcher",
            ItemType.Painkillers.ToString(),
            ItemType.GrenadeHE.ToString(),
            ItemType.GrenadeHE.ToString(),
            ItemType.Radio.ToString(),
            ItemType.ArmorHeavy.ToString(),
        };

        /// <inheritdoc/>
        protected override void RoleAdded(Player player)
        {
            RLogger.Log("MTF EXPLOSIVES SPECIALIST", "SPAWN", $"Player {player.PlayerToString()} is now a {this.Name}");
            player.SetGUI("cc_mtf_es", API.GUI.PseudoGUIPosition.BOTTOM, $"<color=yellow>Grasz</color> jako <color=#0095FF>{this.Name}</color>");
        }

        /// <inheritdoc/>
        protected override void RoleRemoved(Player player)
        {
            RLogger.Log("MTF EXPLOSIVES SPECIALIST", "DEATH", $"Player {player.PlayerToString()} is no longer a {this.Name}");
            player.SetGUI("cc_mtf_es", API.GUI.PseudoGUIPosition.BOTTOM, null);
        }
    }
}
