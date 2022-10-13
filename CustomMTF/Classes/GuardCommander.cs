// -----------------------------------------------------------------------
// <copyright file="GuardCommander.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using InventorySystem.Disarming;
using Mistaken.API;
using Mistaken.API.CustomItems;
using Mistaken.API.CustomRoles;
using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;
using Mistaken.API.GUI;
using Mistaken.RoundLogger;
using UnityEngine;

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
        public override Dictionary<ItemType, ushort> Ammo => new ()
        {
            { ItemType.Ammo556x45, 80 },
            { ItemType.Ammo9x19, 50 },
        };

        /// <inheritdoc/>
        public override List<string> Inventory { get; set; } = new List<string>()
        {
            ((int)MistakenCustomItems.GUARD_COMMANDER_KEYCARD).ToString(),
            ItemType.GunCrossvec.ToString(),
            ItemType.Radio.ToString(),
            ((int)MistakenCustomItems.TASER).ToString(),
            ((int)MistakenCustomItems.IMPACT_GRENADE).ToString(),
            ItemType.ArmorCombat.ToString(),
            ItemType.Medkit.ToString(),
        };

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties()
        {
            RoleSpawnPoints = new List<RoleSpawnPoint>()
            {
                new RoleSpawnPoint()
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
            Exiled.Events.Handlers.Player.InteractingDoor += this.Player_InteractingDoor;
            Exiled.Events.Handlers.Player.ChangingRole += this.Player_ChangingRole;
            Exiled.Events.Handlers.Player.UnlockingGenerator += this.Player_UnlockingGenerator;
            Exiled.Events.Handlers.Scp914.UpgradingPlayer += this.Scp914_UpgradingPlayer;
            Exiled.Events.Handlers.Map.Decontaminating += this.Map_Decontaminating;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            Exiled.Events.Handlers.Server.RoundStarted -= this.Server_RoundStarted;
            Exiled.Events.Handlers.Player.InteractingDoor -= this.Player_InteractingDoor;
            Exiled.Events.Handlers.Player.ChangingRole -= this.Player_ChangingRole;
            Exiled.Events.Handlers.Player.UnlockingGenerator -= this.Player_UnlockingGenerator;
            Exiled.Events.Handlers.Scp914.UpgradingPlayer -= this.Scp914_UpgradingPlayer;
            Exiled.Events.Handlers.Map.Decontaminating -= this.Map_Decontaminating;
        }

        private bool hasCommanderEscorted = false;
        private bool isCommanderNow;

        private void Map_Decontaminating(Exiled.Events.EventArgs.DecontaminatingEventArgs ev)
        {
            this.isCommanderNow = true;
        }

        private void Player_ChangingRole(Exiled.Events.EventArgs.ChangingRoleEventArgs ev)
        {
            if (ev.Reason != SpawnReason.Escaped)
                return;

            if (ev.NewRole.GetTeam() != Team.MTF)
                return;

            if (!this.hasCommanderEscorted)
            {
                foreach (var item in this.TrackedPlayers)
                    item.SetGUI("GuardCommander_Escort", PseudoGUIPosition.TOP, PluginHandler.Instance.Translation.GuardCommanderEscort, 10);
            }

            this.hasCommanderEscorted = true;
        }

        private void Player_UnlockingGenerator(Exiled.Events.EventArgs.UnlockingGeneratorEventArgs ev)
        {
            if (this.hasCommanderEscorted)
                return;

            if (!Items.GuardCommanderKeycardItem.Instance.Check(ev.Player.CurrentItem))
                return;

            if (this.TrackedPlayers.Contains(ev.Player) || Items.GuardCommanderKeycardItem.Instance.CurrentOwner == ev.Player)
            {
                ev.IsAllowed = false;
                return;
            }
        }

        private void Scp914_UpgradingPlayer(Exiled.Events.EventArgs.UpgradingPlayerEventArgs ev)
        {
            if (ev.KnobSetting == Scp914.Scp914KnobSetting.OneToOne)
            {
                if (Items.GuardCommanderKeycardItem.Instance.Check(ev.Player.CurrentItem))
                    Items.GuardCommanderKeycardItem.Instance.CurrentOwner = ev.Player;
            }
        }

        private void Server_RoundStarted()
        {
            this.hasCommanderEscorted = false;
            this.isCommanderNow = false;
            var rid = RoundPlus.RoundId;
            void AfterEscortAccess()
            {
                if (!this.hasCommanderEscorted)
                {
                    foreach (var item in this.TrackedPlayers)
                        item.SetGUI("GuardCommander_Access", PseudoGUIPosition.TOP, PluginHandler.Instance.Translation.GuardCommanderAccess, 10);

                    this.hasCommanderEscorted = true;
                }
            }

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

            Module.CallSafeDelayed(60 * 6, AfterEscortAccess, nameof(AfterEscortAccess), true);
            Module.CallSafeDelayed(1.2f, SpawnGuardCommander, nameof(SpawnGuardCommander));
        }

        private void Player_InteractingDoor(Exiled.Events.EventArgs.InteractingDoorEventArgs ev)
        {
            if (ev.Player is null)
                return;

            if (!Items.GuardCommanderKeycardItem.Instance.Check(ev.Player.CurrentItem))
                return;

            if (!this.Check(ev.Player) && Items.GuardCommanderKeycardItem.Instance.CurrentOwner != ev.Player)
            {
                ev.IsAllowed = false;
                return;
            }

            if (ev.IsAllowed)
                return;

            var type = ev.Door.Type;
            switch (type)
            {
                case DoorType.Intercom:
                    {
                        ev.IsAllowed = true;
                        return;
                    }

                case DoorType.NukeSurface:
                    {
                        foreach (var player in RealPlayers.List.ToArray())
                        {
                            if (ev.Player == player)
                                continue;

                            if (player.Role.Type == RoleType.FacilityGuard)
                                continue;

                            if (player.Role.Team != Team.MTF)
                                continue;

                            if (Vector3.Distance(player.Position, ev.Player.Position) < 10)
                            {
                                ev.IsAllowed = true;
                                return;
                            }
                        }
                    }

                    break;

                case DoorType.Scp106Primary:
                case DoorType.Scp106Secondary:
                case DoorType.Scp106Bottom:
                    {
                        if (!this.isCommanderNow)
                            return;

                        bool tmp = false;
                        foreach (var player in RealPlayers.List.ToArray())
                        {
                            if (ev.Player == player)
                                continue;

                            if (player.Role.Type != RoleType.NtfCaptain && player.Role.Type != RoleType.NtfSpecialist)
                                continue;

                            if (Vector3.Distance(player.Position, ev.Player.Position) < 10)
                            {
                                if (!tmp)
                                    tmp = true;
                                else
                                {
                                    ev.IsAllowed = true;
                                    return;
                                }
                            }
                        }
                    }

                    break;

                case DoorType.GateA:
                case DoorType.GateB:
                    {
                        if (this.hasCommanderEscorted)
                        {
                            ev.IsAllowed = true;
                            return;
                        }

                        foreach (var player in RealPlayers.List.ToArray())
                        {
                            if (ev.Player == player)
                                continue;

                            if (player.Role.Team != Team.RSC && ((player.Role.Team != Team.CDP && player.Role.Team != Team.CHI) || !player.Inventory.IsDisarmed()))
                                continue;

                            if (Vector3.Distance(player.Position, ev.Player.Position) < 10)
                            {
                                ev.IsAllowed = true;
                                return;
                            }
                        }
                    }

                    break;

                case DoorType.HID:
                case DoorType.Scp079First:
                case DoorType.Scp079Second:
                    {
                        if (this.isCommanderNow)
                        {
                            ev.IsAllowed = true;
                            return;
                        }
                    }

                    break;
            }
        }
    }
}
