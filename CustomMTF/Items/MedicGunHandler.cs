// -----------------------------------------------------------------------
// <copyright file="MedicGunHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs;
using Grenades;
using MEC;
using Mistaken.API;
using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;
using Mistaken.CustomItems;
using UnityEngine;

namespace Mistaken.CustomMTF.Items
{
    /// <summary>
    /// Gun that heals hit players.
    /// </summary>
    public class MedicGunHandler : Module
    {
        /// <inheritdoc cref="Module.Module(IPlugin{IConfig})"/>
        public MedicGunHandler(IPlugin<IConfig> plugin)
            : base(plugin)
        {
            Instance = this;
            new MedicGunItem();
        }

        /// <inheritdoc/>
        public override string Name => "MedicGunHandler";

        /// <inheritdoc/>
        public override void OnEnable()
        {
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
        }

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
                    player.ReferenceHub.weaponManager.CallRpcConfirmShot(false, player.ReferenceHub.weaponManager.curWeapon);
                    return false;
                }

                var hpToHeal = Math.Min(targetPlayer.MaxHealth - targetPlayer.Health, HealAmount);
                var ahpToHeal = HealAmount - hpToHeal;
                targetPlayer.Health += hpToHeal;
                targetPlayer.ArtificialHealth += ahpToHeal;
                player.ReferenceHub.weaponManager.CallRpcConfirmShot(true, player.ReferenceHub.weaponManager.curWeapon);
                return false;
            }
        }

        internal static MedicGunHandler Instance { get; private set; }

        private const float HealAmount = 35;
    }
}
