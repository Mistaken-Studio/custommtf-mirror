// -----------------------------------------------------------------------
// <copyright file="StickyGrenadeItem.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs;
using InventorySystem.Items.ThrowableProjectiles;
using MEC;
using Mirror;
using Mistaken.API.Extensions;
using Mistaken.RoundLogger;
using UnityEngine;

namespace Mistaken.CustomMTF.Items
{
    /// <inheritdoc/>
    public class StickyGrenadeItem : CustomGrenade
    {
        /// <summary>
        /// Throws Sticky Grenade.
        /// </summary>
        /// <param name="player">Throwing Player.</param>
        /// <returns>Thrown projectile.</returns>
        public static ThrownProjectile Throw(Player player = null)
        {
            var grenade = new Throwable(ItemType.GrenadeHE, player);
            if (grenade.Base.Owner.characterClassManager.CurRole.team == Team.CHI || grenade.Base.Owner.characterClassManager.CurClass == RoleType.ClassD)
            {
                Respawning.GameplayTickets.Singleton.HandleItemTickets(grenade.Base.OwnerInventory.CurItem);
            }

            ThrownProjectile thrownProjectile = UnityEngine.Object.Instantiate<ThrownProjectile>(grenade.Base.Projectile, grenade.Base.Owner.PlayerCameraReference.position, grenade.Base.Owner.PlayerCameraReference.rotation);
            InventorySystem.Items.Pickups.PickupSyncInfo pickupSyncInfo = new InventorySystem.Items.Pickups.PickupSyncInfo
            {
                ItemId = grenade.Type,
                Locked = !grenade.Base._repickupable,
                Serial = grenade.Serial,
                Weight = 0.01f,
                Position = thrownProjectile.transform.position,
                Rotation = new LowPrecisionQuaternion(thrownProjectile.transform.rotation),
            };
            thrownProjectile.NetworkInfo = pickupSyncInfo;
            thrownProjectile.PreviousOwner = new Footprinting.Footprint(grenade.Base.Owner);
            NetworkServer.Spawn(thrownProjectile.gameObject, ownerConnection: null);
            Patches.ExplodeDestructiblesPatch.Grenades.Add(thrownProjectile);
            thrownProjectile.InfoReceived(default(InventorySystem.Items.Pickups.PickupSyncInfo), pickupSyncInfo);
            Rigidbody rb;
            if (thrownProjectile.TryGetComponent<Rigidbody>(out rb))
                grenade.Base.PropelBody(rb, new Vector3(10, 10, 0), 25, 0.16f);

            thrownProjectile.gameObject.AddComponent<Components.StickyComponent>();
            thrownProjectile.ServerActivate();
            return thrownProjectile;
        }

        /// <inheritdoc/>
        public override ItemType Type { get; set; } = ItemType.GrenadeHE;

        /// <inheritdoc/>
        public override bool ExplodeOnCollision { get; set; } = false;

        /// <inheritdoc/>
        public override float FuseTime { get; set; } = 3f;

        /// <inheritdoc/>
        public override uint Id { get; set; } = 3;

        /// <inheritdoc/>
        public override string Name { get; set; } = "Sticky Grenade";

        /// <inheritdoc/>
        public override string Description { get; set; } = "A Sticky Grenade";

        /// <inheritdoc/>
        public override float Weight { get; set; } = 0.01f;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; }

        /// <inheritdoc/>
        public override Pickup Spawn(Vector3 position)
        {
            var pickup = base.Spawn(position);
            pickup.Base.Info.Serial = pickup.Serial;
            RLogger.Log("STICKY GRENADE", "SPAWN", $"Spawned {this.Name}");
            return pickup;
        }

        /// <inheritdoc/>
        public override Pickup Spawn(Vector3 position, Item item)
        {
            var pickup = base.Spawn(position, item);
            pickup.Base.Info.Serial = pickup.Serial;
            RLogger.Log("STICKY GRENADE", "SPAWN", $"Spawned {this.Name}");
            return pickup;
        }

        /// <inheritdoc/>
        protected override void OnThrowing(ThrowingItemEventArgs ev)
        {
            if (ev.RequestType != ThrowRequest.BeginThrow)
            {
                Patches.ServerThrowPatch.ThrowedItems.Add(ev.Item.Base);
                ev.Player.RemoveItem(ev.Item);
                RLogger.Log("STICKY GRENADE", "THROW", $"Player {ev.Player.PlayerToString()} threw a {this.Name}");
            }
        }

        /// <inheritdoc/>
        protected override void ShowSelectedMessage(Player player)
        {
            Handlers.StickyGrenadeHandler.Instance.RunCoroutine(this.UpdateInterface(player));
        }

        private IEnumerator<float> UpdateInterface(Player player)
        {
            yield return Timing.WaitForSeconds(0.1f);
            while (this.Check(player.CurrentItem))
            {
                player.SetGUI("stickyhold", API.GUI.PseudoGUIPosition.BOTTOM, "Trzymasz <color=yellow>Granat Samoprzylepny</color>");
                yield return Timing.WaitForSeconds(1f);
            }

            player.SetGUI("stickyhold", API.GUI.PseudoGUIPosition.BOTTOM, null);
        }
    }
}
