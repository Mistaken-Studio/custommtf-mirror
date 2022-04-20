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
using Mistaken.API.Extensions;
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

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Server.RespawningTeam += this.Server_RespawningTeam;
            Exiled.Events.Handlers.Server.RoundStarted += this.Server_RoundStarted;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Server.RespawningTeam -= this.Server_RespawningTeam;
            Exiled.Events.Handlers.Server.RoundStarted -= this.Server_RoundStarted;
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
                var players = ev.Players.Where(x => x.Role != RoleType.NtfCaptain && !Registered.Any(c => c.TrackedPlayers.Contains(x))).ToList();
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

            var nuke = Room.List.First(x => x.Type == Exiled.API.Enums.RoomType.HczNuke);

            // first nuke trigger
            // 40.5 989.5 -35.6   6 4 7.5
            InRange.Spawn(nuke.Transform, new Vector3(40.5f, 989.5f, -35.6f), new Vector3(6, 4, 7.5f), OnEnter, OnExit);

            var scp106 = Room.List.First(x => x.Type == Exiled.API.Enums.RoomType.Hcz106);

            // first 106 trigger
            // 14 -5 -29.8   40 30 8.5
            InRange.Spawn(scp106.Transform, new Vector3(14, -5, -29.8f), new Vector3(40, 30, 8.5f), OnEnter, OnExit);

            // second 106 trigger
            // 14 -16 -12   35 8.5 45
            InRange.Spawn(scp106.Transform, new Vector3(14, -16, -12), new Vector3(35, 8.5f, 45), OnEnter, OnExit);

            /*var scp079 = Map.Rooms.First(x => x.Type == Exiled.API.Enums.RoomType.Hcz079);

            // first 079 trigger
            // 14 -2.4 -5   31.5 10 18
            InRange.Spawn(scp079.Transform, new Vector3(14, -2.4f, -5), new Vector3(31.5f, 10, 18), OnEnter, OnExit);

            // second 079 trigger
            // 8.2 -2.4 -16.5   8.5 5 21.5
            InRange.Spawn(scp079.Transform, new Vector3(8.2f, -2.4f, -16.5f), new Vector3(8.5f, 5, 21.5f), OnEnter, OnExit);*/

            Module.RunSafeCoroutine(this.DoRoundLoop(), nameof(this.DoRoundLoop));
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
