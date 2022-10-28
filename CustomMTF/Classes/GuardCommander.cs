// -----------------------------------------------------------------------
// <copyright file="GuardCommander.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Mistaken.API;
using Mistaken.API.CustomItems;
using Mistaken.API.CustomRoles;
using Mistaken.API.Diagnostics;

namespace Mistaken.CustomMTF.Classes
{
    /// <inheritdoc/>
    [CustomRole(RoleType.NtfCaptain)]
    public sealed class GuardCommander : MistakenCustomRole
    {
        /// <inheritdoc/>
        public override MistakenCustomRoles CustomRole => MistakenCustomRoles.GUARD_COMMANDER;

        /// <inheritdoc/>
        public override RoleType Role { get; set; } = RoleType.NtfCaptain;

        /// <inheritdoc/>
        public override int MaxHealth { get; set; } = 100;

        /// <inheritdoc/>
        public override string Name { get; set; } = PluginHandler.Instance.Translation.GuardCommander;

        /// <inheritdoc/>
        public override string Description { get; set; } = PluginHandler.Instance.Translation.GuardCommanderDescription;

        /// <inheritdoc/>
        public override bool KeepInventoryOnSpawn { get; set; } = false;

        /// <inheritdoc/>
        public override bool KeepRoleOnDeath { get; set; } = false;

        /// <inheritdoc/>
        public override bool RemovalKillsPlayer { get; set; } = false;

        /// <inheritdoc/>
        public override string DisplayName => $"<color=#003ECA>{this.Name}</color>";

        /// <inheritdoc/>
        public override Dictionary<ItemType, ushort> Ammo => new()
        {
            { ItemType.Ammo9x19, 80 },
        };

        /// <inheritdoc/>
        public override List<string> Inventory { get; set; } = new()
        {
            ((int)MistakenCustomItems.GUARD_COMMANDER_KEYCARD).ToString(),
            ItemType.GunFSP9.ToString(),
            ItemType.ArmorCombat.ToString(),
            ItemType.Radio.ToString(),
            ItemType.GrenadeFlash.ToString(),
            ItemType.Medkit.ToString(),

            // ((int)MistakenCustomItems.TASER).ToString(),
            // ((int)MistakenCustomItems.IMPACT_GRENADE).ToString(),
        };

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            RoleSpawnPoints = new()
            {
                new()
                {
                    Chance = 100,
                    Role = RoleType.FacilityGuard,
                },
            },
        };

        /// <inheritdoc/>
        public override void Init()
        {
            base.Init();
            Instance = this;
        }

        internal static GuardCommander Instance { get; private set; }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            Exiled.Events.Handlers.Server.RoundStarted += this.Server_RoundStarted;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            Exiled.Events.Handlers.Server.RoundStarted -= this.Server_RoundStarted;
        }

        private void Server_RoundStarted()
        {
            void SpawnGuardCommander()
            {
                try
                {
                    var guards = RealPlayers.Get(RoleType.FacilityGuard).ToArray();

                    if (guards.Length < 3)
                        return;

                    this.AddRole(guards[UnityEngine.Random.Range(0, guards.Length)]);
                }
                catch (System.Exception ex)
                {
                    Log.Error(ex.Message);
                    Log.Error(ex.StackTrace);
                }
            }

            Module.CallSafeDelayed(1.2f, SpawnGuardCommander, nameof(SpawnGuardCommander));
        }
    }
}
