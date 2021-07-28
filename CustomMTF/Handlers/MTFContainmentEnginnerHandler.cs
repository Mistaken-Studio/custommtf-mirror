// -----------------------------------------------------------------------
// <copyright file="MTFContainmentEnginnerHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Interfaces;
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
            new Classes.MTFContainmentEnginner();
        }

        /// <inheritdoc/>
        public override string Name => nameof(MTFContainmentEnginnerHandler);

        /// <inheritdoc/>
        public override void OnEnable()
        {
            Exiled.Events.Handlers.Server.RespawningTeam += this.Handle<RespawningTeamEventArgs>((ev) => this.Server_RespawningTeam(ev));
            Exiled.Events.Handlers.Server.RoundStarted += this.Handle(() => this.Server_RoundStarted(), "RoundStart");
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.RespawningTeam -= this.Handle<RespawningTeamEventArgs>((ev) => this.Server_RespawningTeam(ev));
            Exiled.Events.Handlers.Server.RoundStarted -= this.Handle(() => this.Server_RoundStarted(), "RoundStart");
        }

        internal static MTFContainmentEnginnerHandler Instance { get; private set; }

        private static readonly List<Player> Campers = new List<Player>();

        private static readonly Action<Player> OnEnter = (player) => Campers.Add(player);

        private static readonly Action<Player> OnExit = (player) => Campers.Remove(player);

        private float spawnChance = 0; // %

        private void Server_RespawningTeam(RespawningTeamEventArgs ev)
        {
            if (!ev.IsAllowed)
                return;
            if (!MapPlus.IsLCZDecontaminated())
                return;

            var players = ev.Players;
            players.ShuffleList();

            if (UnityEngine.Random.Range(0, 100) < this.spawnChance)
            {
                this.spawnChance = 0;
                Classes.MTFExplosivesSpecialist.Instance.Spawn(players[0]);
            }
        }

        private void Server_RoundStarted()
        {
            Campers.Clear();
            this.spawnChance = 0;

            // first nuke trigger
            // 40.5 989.5 -35.6   6 4 7.5
            InRange.Spawn(new Vector3(40.5f, 989.5f, -35.6f), new Vector3(6, 4, 7.5f), OnEnter, OnExit);

            // first 106 trigger
            // 14 -5 -29.8   40 30 8.5
            InRange.Spawn(new Vector3(14, -5, -29.8f), new Vector3(40, 30, 8.5f), OnEnter, OnExit);

            // second 106 trigger
            // 14 -16 -12   35 8.5 45
            InRange.Spawn(new Vector3(14, -16, -12), new Vector3(35, 8.5f, 45), OnEnter, OnExit);

            // first 079 trigger
            // 14 -2.4 -5   31.5 10 18
            InRange.Spawn(new Vector3(14, -2.4f, -5), new Vector3(31.5f, 10, 18), OnEnter, OnExit);

            // second 079 trigger
            // 8.2 -2.4 -16.5   8.5 5 21.5
            InRange.Spawn(new Vector3(8.2f, -2.4f, -16.5f), new Vector3(8.5f, 5, 21.5f), OnEnter, OnExit);

            Instance.RunCoroutine(this.DoRoundLoop());
        }

        private IEnumerator<float> DoRoundLoop()
        {
            MEC.Timing.WaitForSeconds(1);
            while (Round.IsStarted)
            {
                yield return MEC.Timing.WaitForSeconds(1);
                if (Campers.Count != 0)
                    this.spawnChance++;
            }
        }
    }
}
