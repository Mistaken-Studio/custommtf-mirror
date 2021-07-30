// -----------------------------------------------------------------------
// <copyright file="StickyGrenadeItem.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Features;
using Grenades;
using Mistaken.API;
using Mistaken.API.Extensions;
using Mistaken.CustomItems;
using UnityEngine;

namespace Mistaken.CustomMTF.Items
{
    /// <inheritdoc/>
    public class StickyGrenadeItem : CustomItem
    {
        /// <summary>
        /// Spawns sticky grenade, adds momentum, disables SCP268 effect if enabled, removes item if <paramref name="item"/> has value.
        /// </summary>
        /// <param name="owner">Grenade owner.</param>
        /// <param name="slow">If throw should only be with half throw power.</param>
        /// <param name="item">Item converted to grenade.</param>
        /// <returns>Spawned grenade.</returns>
        public static Grenade ThrowGrenade(Player owner, bool slow, Inventory.SyncItemInfo? item = null)
        {
            if (owner.GetEffectActive<CustomPlayerEffects.Scp268>())
                owner.DisableEffect<CustomPlayerEffects.Scp268>();
            var instance = SpawnGrenade(owner, slow);
            instance.GetComponent<Rigidbody>().AddForce(new Vector3(instance.NetworkserverVelocities.linear.x * 1.5f, instance.NetworkserverVelocities.linear.y / 2f, instance.NetworkserverVelocities.linear.z * 1.5f), ForceMode.VelocityChange);
            if (item.HasValue)
                owner.RemoveItem(item.Value);

            instance.gameObject.AddComponent<Components.StickyComponent>();

            return instance;
        }

        /// <summary>
        /// Spawns sticky grenade, adds momentum.
        /// </summary>
        /// <param name="owner">Grenade owner.</param>
        /// <param name="slow">If throw should only be with half throw power.</param>
        /// <returns>Spawned grenade.</returns>
        public static Grenade SpawnGrenade(Player owner, bool slow)
        {
            var instance = UnityEngine.Object.Instantiate(owner.GrenadeManager.availableGrenades[0].grenadeInstance);
            Grenade grenade = instance.GetComponent<Grenade>();
            grenade.InitData(owner.GrenadeManager, Vector3.zero, owner.CameraTransform.forward, slow ? 0.5f : 1f);
            Handlers.StickyGrenadeHandler.Grenades.Add(grenade.gameObject);
            Mirror.NetworkServer.Spawn(instance);

            instance.AddComponent<Components.StickyComponent>();

            return grenade;
        }

        /// <inheritdoc cref="CustomItem.CustomItem()"/>
        public StickyGrenadeItem() => this.Register();

        /// <inheritdoc/>
        public override string ItemName => "Sticky Grenade";

        /// <inheritdoc/>
        public override SessionVarType SessionVarType => SessionVarType.CI_STICKY_GRENADE;

        /// <inheritdoc/>
        public override ItemType Item => ItemType.GrenadeFrag;

        /// <inheritdoc/>
        public override int Durability => 002;

        /// <inheritdoc/>
        public override Vector3 Size => base.Size;

        /// <inheritdoc/>
        public override void OnStartHolding(Player player, Inventory.SyncItemInfo item)
        {
            player.SetGUI("sticky", API.GUI.PseudoGUIPosition.BOTTOM, "Trzymasz <color=yellow>Granat Samoprzylepny</color>");
        }

        /// <inheritdoc/>
        public override void OnStopHolding(Player player, Inventory.SyncItemInfo item)
        {
            player.SetGUI("sticky", API.GUI.PseudoGUIPosition.BOTTOM, null);
        }

        /// <inheritdoc/>
        public override void OnForceclass(Player player)
        {
            player.SetGUI("sticky", API.GUI.PseudoGUIPosition.BOTTOM, null);
        }

        /// <inheritdoc/>
        public override bool OnThrow(Player player, Inventory.SyncItemInfo item, bool slow)
        {
            Action action = () =>
            {
                ThrowGrenade(player, slow, item);
                this.OnStopHolding(player, item);
            };

            Handlers.StickyGrenadeHandler.Instance.CallDelayed(1f, action, "OnThrow");
            return false;
        }
    }
}
