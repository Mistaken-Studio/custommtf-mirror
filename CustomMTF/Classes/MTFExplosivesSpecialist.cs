// -----------------------------------------------------------------------
// <copyright file="MTFExplosivesSpecialist.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs;
using Mistaken.API.CustomItems;
using Mistaken.API.CustomRoles;
using Mistaken.RoundLogger;

namespace Mistaken.CustomMTF.Classes
{
    /// <inheritdoc/>
    [CustomRole(RoleType.NtfSergeant)]
    public sealed class MTFExplosivesSpecialist : MistakenCustomRole
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
        public override string Name { get; set; } = PluginHandler.Instance.Translation.MtfExplosivesSpecialist;

        /// <inheritdoc/>
        public override string Description { get; set; } = PluginHandler.Instance.Translation.MtfExplosivesSpecialistDescription;

        /// <inheritdoc/>
        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>()
        {
            CustomAbility.Get(nameof(Abilities.ExplosiveDeathAbility)),
        };

        /// <inheritdoc/>
        public override string DisplayName => $"<color=#0096FF>{this.Name}</color>";

        /// <inheritdoc/>
        public override bool KeepInventoryOnSpawn { get; set; } = false;

        /// <inheritdoc/>
        public override bool KeepRoleOnDeath { get; set; } = false;

        /// <inheritdoc/>
        public override bool RemovalKillsPlayer { get; set; } = false;

        /// <inheritdoc/>
        public override Dictionary<ItemType, ushort> Ammo => new ()
        {
            { ItemType.Ammo556x45, 40 },
            { ItemType.Ammo9x19, 100 },
        };

        /// <inheritdoc/>
        public override List<string> Inventory { get; set; } = new List<string>()
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
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties()
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
        public override void Init()
        {
            base.Init();
            Instance = this;
        }

        /// <inheritdoc/>
        protected override void RoleAdded(Player player)
        {
            base.RoleAdded(player);
            RLogger.Log("MTF EXPLOSIVES SPECIALIST", "SPAWN", $"Player {player.PlayerToString()} is now a MTF Explosives Specialist");
        }

        /// <inheritdoc/>
        protected override void RoleRemoved(Player player)
        {
            base.RoleRemoved(player);
            RLogger.Log("MTF EXPLOSIVES SPECIALIST", "DEATH", $"Player {player.PlayerToString()} is no longer a MTF Explosives Specialist");
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Server.RespawningTeam += this.Server_RespawningTeam;
            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Server.RespawningTeam += this.Server_RespawningTeam;
            base.UnsubscribeEvents();
        }

        private void Server_RespawningTeam(RespawningTeamEventArgs ev)
        {
            if (ev.NextKnownTeam != Respawning.SpawnableTeamType.NineTailedFox)
                return;

            if (!ev.IsAllowed)
                return;

            MEC.Timing.CallDelayed(1.5f, () =>
            {
                var players = ev.Players.Where(x => x.Role.Type != RoleType.NtfCaptain && !Registered.Any(y => y.TrackedPlayers.Contains(x))).ToArray();
                players.ShuffleList();
                var count = Math.Floor(players.Length * (PluginHandler.Instance.Config.MtfExplosivesSpecialistSpawnChance / 100f));

                for (int i = 0; i < count; i++)
                    this.AddRole(players[i]);
            });
        }
    }
}
