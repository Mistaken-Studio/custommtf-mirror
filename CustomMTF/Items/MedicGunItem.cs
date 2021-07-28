// -----------------------------------------------------------------------
// <copyright file="MedicGunItem.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Features;
using Mistaken.API;
using Mistaken.API.Extensions;
using Mistaken.CustomItems;
using UnityEngine;

namespace Mistaken.CustomMTF.Items
{
    /// <inheritdoc/>
    public class MedicGunItem : CustomItem
    {
        /// <inheritdoc cref="CustomItem.CustomItem()"/>
        public MedicGunItem() => this.Register();

        /// <inheritdoc/>
        public override string ItemName => "Medic Gun";

        /// <inheritdoc/>
        public override SessionVarType SessionVarType => SessionVarType.CI_HEALGUN;

        /// <inheritdoc/>
        public override ItemType Item => ItemType.GunCOM15;

        /// <inheritdoc/>
        public override int Durability => 001;

        /// <inheritdoc/>
        public override Vector3 Size => base.Size;

        /// <inheritdoc/>
        public override void OnStartHolding(Player player, Inventory.SyncItemInfo item)
        {
            player.SetGUI("ci_medic_gun_hold", API.GUI.PseudoGUIPosition.BOTTOM, "Trzymasz <color=yellow>Pistolet Medyczny</color>");
        }

        /// <inheritdoc/>
        public override void OnStopHolding(Player player, Inventory.SyncItemInfo item)
        {
            player.SetGUI("ci_medic_gun_hold", API.GUI.PseudoGUIPosition.BOTTOM, null);
        }

        /// <inheritdoc/>
        public override void OnForceclass(Player player)
        {
            player.SetGUI("ci_medic_gun_hold", API.GUI.PseudoGUIPosition.BOTTOM, null);
        }

        /// <inheritdoc/>
        public override bool OnShoot(Player player, Inventory.SyncItemInfo item, GameObject target, Vector3 position)
        {
            var ammo = this.GetInternalDurability(item);
            if (ammo < 1)
                return false;
            ammo--;
            this.SetInternalDurability(player, item, ammo);

            var targetPlayer = Player.Get(target);
            if (targetPlayer == null)
            {
                player.ReferenceHub.weaponManager.RpcConfirmShot(false, player.ReferenceHub.weaponManager.curWeapon);
                return false;
            }

            var hpToHeal = Math.Min(targetPlayer.MaxHealth - targetPlayer.Health, Handlers.MedicGunHandler.HealAmount);
            var ahpToHeal = Handlers.MedicGunHandler.HealAmount - hpToHeal;
            targetPlayer.Health += hpToHeal;
            targetPlayer.ArtificialHealth += ahpToHeal;
            player.ReferenceHub.weaponManager.RpcConfirmShot(true, player.ReferenceHub.weaponManager.curWeapon);
            return false;
        }
    }
}
