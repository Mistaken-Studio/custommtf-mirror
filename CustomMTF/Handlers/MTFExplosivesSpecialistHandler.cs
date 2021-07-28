// -----------------------------------------------------------------------
// <copyright file="MTFExplosivesSpecialistHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs;
using Mistaken.API.Diagnostics;

namespace Mistaken.CustomMTF.Classes
{
    /// <summary>
    /// An MTF with special grenade.
    /// </summary>
    public partial class MTFExplosivesSpecialistHandler : Module
    {
        /// <inheritdoc cref="Module.Module(IPlugin{IConfig})"/>
        public MTFExplosivesSpecialistHandler(IPlugin<IConfig> plugin)
            : base(plugin)
        {
            Instance = this;
            new MTFExplosivesSpecialist();
        }

        /// <inheritdoc/>
        public override string Name => nameof(MTFExplosivesSpecialistHandler);

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

        internal static MTFExplosivesSpecialistHandler Instance { get; private set; }

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
                MTFExplosivesSpecialist.Instance.Spawn(players[i]);
            }
        }
    }
}
