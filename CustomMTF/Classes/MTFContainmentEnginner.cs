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
            { ItemType.Ammo556x45, 100 },
            { ItemType.Ammo9x19, 40 },
        };

        /// <inheritdoc/>
        public override List<string> Inventory { get; set; } = new List<string>()
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

            // player.SetGUI("cc_mtf_ce", API.GUI.PseudoGUIPosition.BOTTOM, string.Format(PluginHandler.Instance.Translation.PlayingAs, PluginHandler.Instance.Translation.MtfPrivateColor, PluginHandler.Instance.Translation.MtfContainmentEnginner));
        }

        /// <inheritdoc/>
        protected override void RoleRemoved(Player player)
        {
            base.RoleRemoved(player);
            RLogger.Log("MTF CONTAINMENT ENGINNER", "DEATH", $"Player {player.PlayerToString()} is no longer a {this.Name}");

            // player.SetGUI("cc_mtf_ce", API.GUI.PseudoGUIPosition.BOTTOM, null);
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Server.RespawningTeam += this.Server_RespawningTeam;
            Exiled.Events.Handlers.Server.RoundStarted += this.Server_RoundStarted;
            Exiled.Events.Handlers.Player.InteractingDoor += this.Player_InteractingDoor;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Server.RespawningTeam -= this.Server_RespawningTeam;
            Exiled.Events.Handlers.Server.RoundStarted -= this.Server_RoundStarted;
            Exiled.Events.Handlers.Player.InteractingDoor -= this.Player_InteractingDoor;
        }

        private static readonly List<Player> Campers = new List<Player>();

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

            MEC.Timing.CallDelayed(1.5f, () =>
            {
                var players = ev.Players.Where(x => x.Role.Type != RoleType.NtfCaptain && !Registered.Any(c => c.TrackedPlayers.Contains(x))).ToList();
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

            Module.RunSafeCoroutine(this.DoRoundLoop(), nameof(this.DoRoundLoop), true);
        }

        private void Player_InteractingDoor(InteractingDoorEventArgs ev)
        {
            if (ev.Player is null)
                return;

            if (!this.Check(ev.Player))
                return;

            var item = ev.Player.CurrentItem;
            if (item == null || item.Type != ItemType.KeycardContainmentEngineer)
                return;

            if (ev.Door.Type == DoorType.Scp106Primary || ev.Door.Type == DoorType.Scp106Bottom || ev.Door.Type == DoorType.Scp106Secondary)
                ev.IsAllowed = true;
        }

        private IEnumerator<float> DoRoundLoop()
        {
            yield return MEC.Timing.WaitForSeconds(60);
            while (Round.IsStarted)
            {
                yield return MEC.Timing.WaitForSeconds(PluginHandler.Instance.Config.MtfContainmentEnginnerSpawnPointTime);
                if (Campers.Count != 0)
                    this.spawnChance++;
            }
        }
    }
}
