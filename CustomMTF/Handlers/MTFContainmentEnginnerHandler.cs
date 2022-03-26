// -----------------------------------------------------------------------
// <copyright file="MTFContainmentEnginnerHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs;
using Mistaken.API;
using Mistaken.API.Components;
using Mistaken.API.Diagnostics;
using UnityEngine;

namespace Mistaken.CustomMTF.Handlers
{
    /// <summary>
    /// An Containment Enginner in MTF.
    /// </summary>
    public class MTFContainmentEnginnerHandler : Module
    {
        /// <inheritdoc cref="Module.Module(IPlugin{IConfig})"/>
        public MTFContainmentEnginnerHandler(IPlugin<IConfig> plugin)
            : base(plugin)
        {
            Instance = this;
        }

        /// <inheritdoc/>
        public override string Name => nameof(MTFContainmentEnginnerHandler);

        /// <inheritdoc/>
        public override void OnEnable()
        {
            Exiled.Events.Handlers.Server.RespawningTeam += this.Server_RespawningTeam;
            Exiled.Events.Handlers.Server.RoundStarted += this.Server_RoundStarted;
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.RespawningTeam -= this.Server_RespawningTeam;
            Exiled.Events.Handlers.Server.RoundStarted -= this.Server_RoundStarted;
        }

        internal static MTFContainmentEnginnerHandler Instance { get; private set; }

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
                var players = ev.Players.Where(x => x.Role != RoleType.NtfCaptain && !CustomRole.Registered.Any(c => c.TrackedPlayers.Contains(x))).ToList();
                players.ShuffleList();

                if (UnityEngine.Random.Range(0, 100) <= this.spawnChance)
                {
                    Classes.MTFContainmentEnginner.Instance.AddRole(players[0]);
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

            Instance.RunCoroutine(this.DoRoundLoop());
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
