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
                if (player.GetEffectActive<CustomPlayerEffects.Scp268>())
                    player.DisableEffect<CustomPlayerEffects.Scp268>();
                Grenade grenade = UnityEngine.Object.Instantiate(player.GrenadeManager.availableGrenades[0].grenadeInstance).GetComponent<Grenade>();
                grenade.InitData(player.GrenadeManager, Vector3.zero, player.CameraTransform.forward, slow ? 0.5f : 1f);
                Handlers.StickyGrenadeHandler.Grenades.Add(grenade.gameObject);
                Mirror.NetworkServer.Spawn(grenade.gameObject);
                grenade.GetComponent<Rigidbody>().AddForce(new Vector3(grenade.NetworkserverVelocities.linear.x * 1.5f, grenade.NetworkserverVelocities.linear.y / 2f, grenade.NetworkserverVelocities.linear.z * 1.5f), ForceMode.VelocityChange);
                player.RemoveItem(item);
                grenade.gameObject.AddComponent<Components.StickyComponent>();
                grenade.gameObject.AddComponent<Components.StickyComponent2>();
                this.OnStopHolding(player, item);
            };
            Handlers.StickyGrenadeHandler.Instance.CallDelayed(1f, action, "OnThrow");
            return false;
        }
    }
}
