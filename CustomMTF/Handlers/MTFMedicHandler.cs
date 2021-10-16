// -----------------------------------------------------------------------
// <copyright file="MTFMedicHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using Exiled.API.Interfaces;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs;
using Mistaken.API.CustomRoles;
using Mistaken.API.Diagnostics;

namespace Mistaken.CustomMTF.Handlers
{
    /// <summary>
    /// Gun that heals hit players.
    /// </summary>
    public class MTFMedicHandler : Module
    {
        /// <inheritdoc cref="Module.Module(IPlugin{IConfig})"/>
        public MTFMedicHandler(IPlugin<IConfig> plugin)
            : base(plugin)
        {
            Instance = this;
            new Classes.MTFMedic().TryRegister();
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
                return;

            var players = ev.Players.Where(x => x.Role != RoleType.NtfCaptain && !CustomRole.Registered.Any(c => c.TrackedPlayers.Contains(x))).ToList();
            players.ShuffleList();

            var count = Math.Floor(players.Count * (SpawnChance / 100));
            for (int i = 0; i < count; i++)
                MistakenCustomRoles.MTF_MEDIC.Get().AddRole(players[i]);
        }
    }
}
