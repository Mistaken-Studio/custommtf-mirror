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
            Exiled.Events.Handlers.Server.RespawningTeam += this.Server_RespawningTeam;
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.RespawningTeam -= this.Server_RespawningTeam;
        }

        internal static MTFMedicHandler Instance { get; private set; }

        private const float SpawnChance = 30; // %

        private void Server_RespawningTeam(RespawningTeamEventArgs ev)
        {
            if (ev.NextKnownTeam != Respawning.SpawnableTeamType.NineTailedFox)
                return;
            if (!ev.IsAllowed)
                return;

            MEC.Timing.CallDelayed(1.5f, () =>
            {
                var players = ev.Players.Where(x => x.Role != RoleType.NtfCaptain && !CustomRole.Registered.Any(c => c.TrackedPlayers.Contains(x))).ToList();
                players.ShuffleList();
                players.SpawnPlayerWithRole(Classes.MTFMedic.Instance, SpawnChance);
            });
        }
    }
}
