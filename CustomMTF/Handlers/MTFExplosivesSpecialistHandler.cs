// -----------------------------------------------------------------------
// <copyright file="MTFExplosivesSpecialistHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using Exiled.API.Interfaces;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs;
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
            new Classes.Abilities.ExplosiveDeathAbility().TryRegister();
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
                players.SpawnPlayerWithRole(Classes.MTFExplosivesSpecialist.Instance, PluginHandler.Instance.Config.MtfExplosivesSpecialistSpawnChance);
            });
        }
    }
}
