// -----------------------------------------------------------------------
// <copyright file="MTFContainmentEnginner.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs;
using MEC;
using Mistaken.API;
using Mistaken.API.Components;
using Mistaken.API.CustomRoles;
using Mistaken.API.Diagnostics;
using Mistaken.RoundLogger;
using UnityEngine;

namespace Mistaken.CustomMTF.Classes
{
    /// <inheritdoc/>
    [CustomRole(RoleType.NtfPrivate)]
    public sealed class MTFContainmentEnginner : MistakenCustomRole
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
        public override string Name { get; set; } = PluginHandler.Instance.Translation.MtfContainmentEnginner;

        /// <inheritdoc/>
        public override string Description { get; set; } = PluginHandler.Instance.Translation.MtfContainmentEnginnerDescription;

        /// <inheritdoc/>
        public override string DisplayName => $"<color=#6FC3FF>{this.Name}</color>";

        /// <inheritdoc/>
        public override bool KeepInventoryOnSpawn { get; set; } = false;

        /// <inheritdoc/>
        public override bool KeepRoleOnDeath { get; set; } = false;

        /// <inheritdoc/>
        public override bool RemovalKillsPlayer { get; set; } = false;

        /// <inheritdoc/>
        public override Dictionary<ItemType, ushort> Ammo => new()
        {
            { ItemType.Ammo556x45, 100 },
            { ItemType.Ammo9x19, 40 },
        };

        /// <inheritdoc/>
        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.KeycardNTFOfficer.ToString(),
            ItemType.KeycardContainmentEngineer.ToString(),
            ItemType.GunE11SR.ToString(),
            ItemType.Medkit.ToString(),
            ItemType.GrenadeHE.ToString(),
            ItemType.Radio.ToString(),
            ItemType.ArmorCombat.ToString(),
        };

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            RoleSpawnPoints = new()
            {
                new()
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
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Server.RespawningTeam += this.Server_RespawningTeam;
            Exiled.Events.Handlers.Server.RoundStarted += this.Server_RoundStarted;
            Exiled.Events.Handlers.Player.InteractingDoor += this.Player_InteractingDoor;
            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Server.RespawningTeam -= this.Server_RespawningTeam;
            Exiled.Events.Handlers.Server.RoundStarted -= this.Server_RoundStarted;
            Exiled.Events.Handlers.Player.InteractingDoor -= this.Player_InteractingDoor;
            base.UnsubscribeEvents();
        }

        private static readonly HashSet<Player> Campers = new();

        private static readonly Action<Player> OnEnter = (player) => Campers.Add(player);

        private static readonly Action<Player> OnExit = (player) => Campers.Remove(player);

        private float spawnChance = 0;

        private void Server_RespawningTeam(RespawningTeamEventArgs ev)
        {
            if (ev.NextKnownTeam != Respawning.SpawnableTeamType.NineTailedFox)
                return;

            if (!ev.IsAllowed)
                return;

            if (!MapPlus.IsLCZDecontaminated())
                return;

            if (ev.Players.Count == 0)
                return;

            Timing.CallDelayed(1.5f, () =>
            {
                var players = ev.Players.Where(x => x.Role.Type != RoleType.NtfCaptain && !Registered.Any(y => y.TrackedPlayers.Contains(x))).ToArray();
                players.ShuffleList();

                if (UnityEngine.Random.Range(0, 100) <= this.spawnChance)
                {
                    this.AddRole(players[0]);
                    this.spawnChance = 0;
                }
            });
        }

        private void Server_RoundStarted()
        {
            Campers.Clear();
            this.spawnChance = 0;

            var scp106 = Room.List.First(x => x.Type == RoomType.Hcz106);

            // first 106 trigger
            // offset: 30 -7 -13.7 scale: 9 26 37
            InRange.Spawn(scp106.Transform, new Vector3(30, -7, -13.7f), new Vector3(9, 26, 37), OnEnter, OnExit);

            // second 106 trigger
            // offset: 7.5 -17 -14.5 scale: 35 7 35
            InRange.Spawn(scp106.Transform, new Vector3(7.5f, -17, -14.5f), new Vector3(35, 7, 35), OnEnter, OnExit);

            Module.RunSafeCoroutine(this.UpdateSpawnPoints(), nameof(this.UpdateSpawnPoints), true);
        }

        private void Player_InteractingDoor(InteractingDoorEventArgs ev)
        {
            if (ev.Player is null)
                return;

            if (!this.Check(ev.Player))
                return;

            var item = ev.Player.CurrentItem;
            if (item is null || item.Type != ItemType.KeycardContainmentEngineer)
                return;

            if (ev.Door.Type == DoorType.Scp106Primary || ev.Door.Type == DoorType.Scp106Bottom || ev.Door.Type == DoorType.Scp106Secondary)
                ev.IsAllowed = true;
        }

        private IEnumerator<float> UpdateSpawnPoints()
        {
            yield return Timing.WaitForSeconds(60);
            while (true)
            {
                yield return Timing.WaitForSeconds(PluginHandler.Instance.Config.MtfContainmentEnginnerSpawnPointTime);

                if (Campers.Count != 0)
                    this.spawnChance++;
            }
        }
    }
}
