// -----------------------------------------------------------------------
// <copyright file="MTFContainmentEnginner.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Spawn;
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
        public override string DisplayName => this.Name;

        /// <inheritdoc/>
        public override bool KeepInventoryOnSpawn { get; set; } = false;

        /// <inheritdoc/>
        public override bool KeepRoleOnDeath { get; set; } = false;

        /// <inheritdoc/>
        public override bool RemovalKillsPlayer { get; set; } = false;

        /// <inheritdoc/>
        public override Dictionary<ItemType, ushort> Ammo => new Dictionary<ItemType, ushort>()
        {
            { ItemType.Ammo556x45, 80 },
            { ItemType.Ammo9x19, 50 },
        };

        /// <inheritdoc/>
        public override List<string> Inventory { get; set; } = new List<string>()
        {
            ItemType.GunE11SR.ToString(),
            ItemType.GunCOM18.ToString(),
            ItemType.Medkit.ToString(),
            ItemType.GrenadeHE.ToString(),
            ItemType.Radio.ToString(),
            ItemType.ArmorCombat.ToString(),
        };

        /// <inheritdoc/>
        public override KeycardPermissions BuiltInPermissions =>
            KeycardPermissions.ContainmentLevelOne |
            KeycardPermissions.ContainmentLevelTwo |
            KeycardPermissions.ContainmentLevelThree |
            KeycardPermissions.ArmoryLevelOne |
            KeycardPermissions.AlphaWarhead |
            KeycardPermissions.Checkpoints |
            KeycardPermissions.Intercom;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties()
        {
            RoleSpawnPoints = new List<RoleSpawnPoint>()
            {
                new RoleSpawnPoint()
                {
                    Chance = 100,
                    Role = RoleType.NtfPrivate,
                },
            },
        };

        /// <inheritdoc/>
        public override void Init()
        {
            base.Init();
            Instance = this;
        }

        /// <inheritdoc/>
        protected override void RoleAdded(Player player)
        {
            base.RoleAdded(player);
            RLogger.Log("MTF CONTAINMENT ENGINNER", "SPAWN", $"Player {player.PlayerToString()} is now a {this.Name}");
            player.SetGUI("cc_mtf_ce", API.GUI.PseudoGUIPosition.BOTTOM, string.Format(PluginHandler.Instance.Translation.PlayingAs, PluginHandler.Instance.Translation.MtfPrivateColor, PluginHandler.Instance.Translation.MtfContainmentEnginner));
        }

        /// <inheritdoc/>
        protected override void RoleRemoved(Player player)
        {
            base.RoleRemoved(player);
            RLogger.Log("MTF CONTAINMENT ENGINNER", "DEATH", $"Player {player.PlayerToString()} is no longer a {this.Name}");
            player.SetGUI("cc_mtf_ce", API.GUI.PseudoGUIPosition.BOTTOM, null);
        }
    }
}
