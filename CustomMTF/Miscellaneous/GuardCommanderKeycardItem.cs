// -----------------------------------------------------------------------
// <copyright file="GuardCommanderKeycardItem.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs;
using InventorySystem.Disarming;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.Pickups;
using Mistaken.API;
using Mistaken.API.CustomItems;
using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;
using Mistaken.API.GUI;
using Mistaken.RoundLogger;
using UnityEngine;

namespace Mistaken.CustomMTF.Miscellaneous
{
    /// <summary>
    /// Keycard that Guard commander uses.
    /// </summary>
    [CustomItem(ItemType.KeycardNTFOfficer)]
    public sealed class GuardCommanderKeycardItem : MistakenCustomItem
    {
        /// <inheritdoc/>
        public override MistakenCustomItems CustomItem => MistakenCustomItems.GUARD_COMMANDER_KEYCARD;

        /// <inheritdoc/>
        public override ItemType Type { get; set; } = ItemType.KeycardNTFOfficer;

        /// <inheritdoc/>
        public override string Name { get; set; } = "Karta Dowódcy Ochrony";

        /// <inheritdoc/>
        public override string DisplayName => "Karta Dowódcy Ochrony";

        /// <inheritdoc/>
        public override string Description { get; set; } = "Guard Commander's Keycard";

        /// <inheritdoc/>
        public override float Weight { get; set; } = 0.01f;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; }

        /// <inheritdoc/>
        public override void Init()
        {
            Instance = this;
            base.Init();
        }

        /// <inheritdoc/>
        public override Pickup Spawn(Vector3 position, Player previousOwner = null)
        {
            return this.Spawn(position, this.CreateCorrectItem(), previousOwner);
        }

        /// <inheritdoc/>
        public override Pickup Spawn(Vector3 position, Item item, Player previousOwner = null)
        {
            var pickup = base.Spawn(position, item, previousOwner);
            RLogger.Log("GUARD COMMANDER KEYCARD", "SPAWN", $"{this.Name} spawned");
            pickup.Scale = KeycardSize;
            return pickup;
        }

        internal static GuardCommanderKeycardItem Instance { get; private set; }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            Exiled.Events.Handlers.Server.RoundStarted += this.Server_RoundStarted;
            Exiled.Events.Handlers.Player.InteractingDoor += this.Player_InteractingDoor;
            Exiled.Events.Handlers.Player.ChangingRole += this.Player_ChangingRole;
            Exiled.Events.Handlers.Player.UnlockingGenerator += this.Player_UnlockingGenerator;
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
            Exiled.Events.Handlers.Map.Decontaminating -= this.Map_Decontaminating;
        }

        private static readonly Vector3 KeycardSize = new(1, 5, 1);
        private static bool _afterEscort = false;
        private static bool _afterDecontamination = false;

        private void Map_Decontaminating(DecontaminatingEventArgs ev)
        {
            _afterDecontamination = true;
        }

        private void Player_ChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Reason != SpawnReason.Escaped)
                return;

            if (ev.NewRole.GetTeam() != Team.MTF)
                return;

            if (!_afterEscort)
            {
                foreach (var item in Classes.GuardCommander.Instance.TrackedPlayers)
                    item.SetGUI("GuardCommander_Escort", PseudoGUIPosition.TOP, PluginHandler.Instance.Translation.GuardCommanderEscort, 10);
            }

            _afterEscort = true;
        }

        private void Player_UnlockingGenerator(UnlockingGeneratorEventArgs ev)
        {
            if (_afterEscort)
                return;

            if (this.Check(ev.Player.CurrentItem))
                ev.IsAllowed = false;
        }

        private void Server_RoundStarted()
        {
            _afterEscort = false;
            _afterDecontamination = false;

            static void AfterEscortAccess()
            {
                if (!_afterEscort)
                {
                    foreach (var item in Classes.GuardCommander.Instance.TrackedPlayers)
                        item.SetGUI("GuardCommander_Access", PseudoGUIPosition.TOP, PluginHandler.Instance.Translation.GuardCommanderAccess, 10);

                    _afterEscort = true;
                }
            }

            Module.CallSafeDelayed(60 * 6, AfterEscortAccess, nameof(AfterEscortAccess), true);
        }

        private void Player_InteractingDoor(InteractingDoorEventArgs ev)
        {
            if (ev.Player is null)
                return;

            if (!this.Check(ev.Player.CurrentItem))
            {
                if (!ev.Player.TryGetSessionVariable<ItemPickupBase>(SessionVarType.THROWN_ITEM, out var item))
                    return;

                if (item is not KeycardPickup keycard)
                    return;

                if (!this.Check(Pickup.Get(keycard)))
                    return;
            }

            if (ev.Door.RequiredPermissions.RequiredPermissions.HasFlag(Interactables.Interobjects.DoorUtils.KeycardPermissions.ContainmentLevelTwo))
            {
                ev.IsAllowed = _afterEscort;
                return;
            }

            if (ev.IsAllowed)
                return;

            switch (ev.Door.Type)
            {
                case DoorType.Intercom:
                    {
                        ev.IsAllowed = true;
                        return;
                    }

                case DoorType.GateA:
                case DoorType.GateB:
                    {
                        if (_afterEscort)
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
                    {
                        if (_afterDecontamination)
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
