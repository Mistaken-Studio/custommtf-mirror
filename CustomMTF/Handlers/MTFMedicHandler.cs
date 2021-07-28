// -----------------------------------------------------------------------
// <copyright file="MTFMedicHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs;
using Grenades;
using MEC;
using Mistaken.API;
using Mistaken.API.Diagnostics;

namespace Mistaken.CustomMTF.Classes
{
    /// <summary>
    /// Gun that heals hit players.
    /// </summary>
    public partial class MTFMedicHandler : Module
    {
        /// <inheritdoc cref="Module.Module(IPlugin{IConfig})"/>
        public MTFMedicHandler(IPlugin<IConfig> plugin)
            : base(plugin)
        {
            Instance = this;
            new MTFMedic();
        }

        /// <inheritdoc/>
        public override string Name => nameof(MTFMedicHandler);

        /// <inheritdoc/>
        public override void OnEnable()
        {
            Exiled.Events.Handlers.Server.RespawningTeam += this.Handle<RespawningTeamEventArgs>((ev) => this.Server_RespawningTeam(ev));
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.RespawningTeam -= this.Handle<RespawningTeamEventArgs>((ev) => this.Server_RespawningTeam(ev));
        }

        internal static MTFMedicHandler Instance { get; private set; }

        private const float SpawnChance = 10; // %

        private void Server_RespawningTeam(RespawningTeamEventArgs ev)
        {
            if (!ev.IsAllowed)
            {
                return;
            }

            var players = ev.Players;
            players.ShuffleList();

            var count = Math.Floor(players.Count * (SpawnChance / 100));
            for (int i = 0; i < count; i++)
            {
                MTFMedic.Instance.Spawn(players[i]);
            }
        }
    }
}
