﻿// -----------------------------------------------------------------------
// <copyright file="MTFExplosivesSpecialist.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using Mistaken.API.CustomItems;
using Mistaken.API.CustomRoles;
using Mistaken.API.Extensions;
using Mistaken.RoundLogger;

namespace Mistaken.CustomMTF.Classes
{
    /// <inheritdoc/>
    public class MTFExplosivesSpecialist : MistakenCustomRole
    {
        /// <summary>
        /// Gets the MTF explosives specialist instance.
        /// </summary>
        public static MTFExplosivesSpecialist Instance { get; private set; }

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
        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>()
        {
            new Abilities.ExplosiveDeathAbility(),
        };

        /// <inheritdoc/>
        public override void Init()
        {
            base.Init();
            Instance = this;
        }

        /// <inheritdoc/>
        protected override string DisplayName => this.Name;

        /// <inheritdoc/>
        protected override bool KeepInventoryOnSpawn { get; set; } = false;

        /// <inheritdoc/>
        protected override bool KeepRoleOnDeath { get; set; } = false;

        /// <inheritdoc/>
        protected override bool RemovalKillsPlayer { get; set; } = false;

        /// <inheritdoc/>
        protected override Dictionary<ItemType, ushort> Ammo => new Dictionary<ItemType, ushort>()
        {
            { ItemType.Ammo556x45, 40 },
            { ItemType.Ammo9x19, 100 },
        };

        /// <inheritdoc/>
        protected override List<string> Inventory { get; set; } = new List<string>()
        {
            ItemType.KeycardNTFLieutenant.ToString(),
            ItemType.GunFSP9.ToString(),
            ((int)MistakenCustomItems.GRENADE_LAUNCHER).ToString(),
            ItemType.Painkillers.ToString(),
            ItemType.GrenadeHE.ToString(),
            ItemType.GrenadeHE.ToString(),
            ItemType.Radio.ToString(),
            ItemType.ArmorHeavy.ToString(),
        };

        /// <inheritdoc/>
        protected override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties()
        {
            RoleSpawnPoints = new List<RoleSpawnPoint>()
            {
                new RoleSpawnPoint()
                {
                    Chance = 100,
                    Role = RoleType.NtfSergeant,
                },
            },
        };

        /// <inheritdoc/>
        protected override void RoleAdded(Player player)
        {
            base.RoleAdded(player);
            RLogger.Log("MTF EXPLOSIVES SPECIALIST", "SPAWN", $"Player {player.PlayerToString()} is now a {this.Name}");
            player.SetGUI("cc_mtf_es", API.GUI.PseudoGUIPosition.BOTTOM, string.Format(PluginHandler.Instance.Translation.PlayingAs, PluginHandler.Instance.Translation.MtfSergantColor, PluginHandler.Instance.Translation.MtfExplosivesSpecialist));
        }

        /// <inheritdoc/>
        protected override void RoleRemoved(Player player)
        {
            base.RoleRemoved(player);
            RLogger.Log("MTF EXPLOSIVES SPECIALIST", "DEATH", $"Player {player.PlayerToString()} is no longer a {this.Name}");
            player.SetGUI("cc_mtf_es", API.GUI.PseudoGUIPosition.BOTTOM, null);
        }
    }
}