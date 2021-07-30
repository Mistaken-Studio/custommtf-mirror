// -----------------------------------------------------------------------
// <copyright file="GrenadeLauncherItem.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Grenades;
using Mirror;
using Mistaken.API;
using Mistaken.API.Extensions;
using Mistaken.API.GUI;
using Mistaken.CustomItems;
using UnityEngine;

namespace Mistaken.CustomMTF.Items
{
    /// <inheritdoc/>
    public class GrenadeLauncherItem : CustomItem
    {
        /// <inheritdoc cref="CustomItem.CustomItem()"/>
        public GrenadeLauncherItem() => this.Register();

        /// <inheritdoc/>
        public override string ItemName => "Grenade Launcher";

        /// <inheritdoc/>
        public override ItemType Item => ItemType.GunUSP;

        /// <inheritdoc/>
        public override SessionVarType SessionVarType => SessionVarType.CI_GRENADE_LAUNCHER;

        /// <inheritdoc/>
        public override int Durability => 589;

        /// <inheritdoc/>
        public override Vector3 Size => new Vector3(2f, 1.5f, 1.5f);

        /// <inheritdoc/>
        public override bool OnReload(Player player, Inventory.SyncItemInfo item)
        {
            var dur = this.GetInternalDurability(item);
            if (dur != 0)
            {
                player.SetGUI("grenadeLauncherWarn", PseudoGUIPosition.BOTTOM, "Nie możesz przeładować nie mając pustego magazynka", 3);
                return false;
            }

            if (!player.Inventory.items.Any(i => i.id == ItemType.GrenadeFrag))
            {
                player.SetGUI("grenadeLauncherWarn", PseudoGUIPosition.BOTTOM, "Nie masz amunicji(Granat Odłamkowy)", 3);
                return false;
            }

            this.SetInternalDurability(player, item, MagSize + 1);
            player.RemoveItem(player.Inventory.items.First(i => i.id == ItemType.GrenadeFrag));
            player.SetGUI("grenadeLauncherWarn", PseudoGUIPosition.BOTTOM, "Przeładowano", 3);
            return false;
        }

        /// <inheritdoc/>
        public override bool OnShoot(Player player, Inventory.SyncItemInfo item, GameObject go, Vector3 position)
        {
            int dur = (int)Math.Floor(this.GetInternalDurability(item));
            Log.Debug($"Ammo: {dur} | {item.durability}");
            if (dur == 0)
            {
                player.SetGUI("grenadeLauncherWarn", PseudoGUIPosition.BOTTOM, "Nie możesz strzelać z pustym magazynkiem", 3);
                player.ReferenceHub.weaponManager.RpcEmptyClip();
                return false;
            }

            var grenade = StickyGrenadeItem.ThrowGrenade(player, false);
            grenade.GetComponent<Components.StickyComponent>().DamageMultiplayer = 0.02f;
            this.SetInternalDurability(player, item, dur - 1);
            return false;
        }

        /// <inheritdoc/>
        public override void OnStartHolding(Player player, Inventory.SyncItemInfo item)
        {
            Log.Debug(player.Ammo[(int)AmmoType.Nato9]);
            player.Ammo[(int)AmmoType.Nato9] += 1;
            Log.Debug(player.Ammo[(int)AmmoType.Nato9]);
            base.OnStartHolding(player, item);

            player.SetGUI("grenadeLauncher", PseudoGUIPosition.BOTTOM, $"Trzymasz <color=yellow>{this.ItemName}</color>");
        }

        /// <inheritdoc/>
        public override void OnStopHolding(Player player, Inventory.SyncItemInfo item)
        {
            Log.Debug(player.Ammo[(int)AmmoType.Nato9]);
            player.Ammo[(int)AmmoType.Nato9]--;
            Log.Debug(player.Ammo[(int)AmmoType.Nato9]);
            player.SetGUI("grenadeLauncher", PseudoGUIPosition.BOTTOM, null);
            base.OnStopHolding(player, item);
        }

        private const int MagSize = 3;
    }
}
