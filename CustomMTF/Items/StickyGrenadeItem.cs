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
                GrenadeGo = grenade.gameObject;
                Handlers.StickyGrenadeHandler.Grenades.Add(GrenadeGo);
                Mirror.NetworkServer.Spawn(GrenadeGo);
                GrenadeGo.GetComponent<Rigidbody>().AddForce(new Vector3(grenade.NetworkserverVelocities.linear.x * 1.5f, grenade.NetworkserverVelocities.linear.y / 2f, grenade.NetworkserverVelocities.linear.z * 1.5f), ForceMode.VelocityChange);
                player.RemoveItem(item);
                GrenadeGo.AddComponent<Components.StickyComponent>();
                Handlers.StickyGrenadeHandler.Instance.CallDelayed(0.2f, () => Mistaken.API.Components.InRange.Spawn(GrenadeGo.transform, Vector3.zero, new Vector3(1, 1, 1), OnEnter, OnExit));
                this.OnStopHolding(player, item);
            };
            Handlers.StickyGrenadeHandler.Instance.CallDelayed(1f, action, "OnThrow");
            return false;
        }

        internal static GameObject GrenadeGo { get; private set; }

        internal static Player GrenadePlayer { get; private set; }

        private static readonly Action<Player> OnEnter = (player) =>
        {
            GrenadePlayer = player;
        };

        private static readonly Action<Player> OnExit = (player) =>
        {
            GrenadePlayer = null;
        };
    }
}
