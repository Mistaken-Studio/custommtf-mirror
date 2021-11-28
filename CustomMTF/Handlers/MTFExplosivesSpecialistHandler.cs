// -----------------------------------------------------------------------
// <copyright file="MTFExplosivesSpecialistHandler.cs" company="Mistaken">
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
    /// An MTF with special grenade.
    /// </summary>
    public class MTFExplosivesSpecialistHandler : Module
    {
        /// <inheritdoc cref="Module.Module(IPlugin{IConfig})"/>
        public MTFExplosivesSpecialistHandler(IPlugin<IConfig> plugin)
            : base(plugin)
        {
            Instance = this;

            new Classes.MTFExplosivesSpecialist().TryRegister();
        }

        /// <inheritdoc/>
        public override string Name => nameof(MTFExplosivesSpecialistHandler);

        /// <inheritdoc/>
        public override void OnEnable()
        {
            Exiled.Events.Handlers.Server.RespawningTeam += this.Server_RespawningTeam;
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.RespawningTeam -= this.Server_RespawningTeam;
        }

        internal static MTFExplosivesSpecialistHandler Instance { get; private set; }

        private const float SpawnChance = 10; // %

        private void Server_RespawningTeam(RespawningTeamEventArgs ev)
        {
            if (!ev.IsAllowed)
                return;

            var players = ev.Players.Where(x => x.Role != RoleType.NtfCaptain && !CustomRole.Registered.Any(c => c.TrackedPlayers.Contains(x))).ToList();
            players.ShuffleList();

            var count = Math.Floor(players.Count * (SpawnChance / 100));

            for (int i = 0; i < count; i++)
                Classes.MTFExplosivesSpecialist.Instance.AddRole(players[i]);
        }
    }
}
