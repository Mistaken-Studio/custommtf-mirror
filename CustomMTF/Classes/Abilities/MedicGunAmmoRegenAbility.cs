﻿// -----------------------------------------------------------------------
// <copyright file="MedicGunAmmoRegenAbility.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API.Features;
using MEC;
using Mistaken.API.CustomItems;
using Mistaken.RoundLogger;

namespace Mistaken.CustomMTF.Classes.Abilities
{
    /// <summary>
    /// Ability that makes Medic Gun regenerate ammo.
    /// </summary>
    public class MedicGunAmmoRegenAbility : PassiveAbility
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = nameof(MedicGunAmmoRegenAbility);

        /// <inheritdoc/>
        public override string Description { get; set; } = "Let's you regenerate Medic Gun's ammo";

        /// <inheritdoc/>
        protected override void AbilityAdded(Player player)
        {
            this.player = player;
            Exiled.Events.Handlers.Player.Shooting += this.Player_Shooting;
            Exiled.Events.Handlers.Player.PickingUpItem += this.Player_PickingUpItem;
        }

        /// <inheritdoc/>
        protected override void AbilityRemoved(Player player)
        {
            Exiled.Events.Handlers.Player.Shooting -= this.Player_Shooting;
            Exiled.Events.Handlers.Player.PickingUpItem -= this.Player_PickingUpItem;
        }

        private Player player;

        private bool isActive = false;

        private void Player_Shooting(Exiled.Events.EventArgs.ShootingEventArgs ev)
        {
            if (ev.IsAllowed && this.player == ev.Shooter)
            {
                if (CustomItem.TryGet(ev.Shooter.CurrentItem, out CustomItem item))
                {
                    if (item.Id == (uint)MistakenCustomItems.MEDIC_GUN)
                    {
                        if (!this.isActive)
                            Handlers.MTFMedicHandler.Instance.RunCoroutine(this.RegenerateAmmo(ev.Shooter));
                    }
                }
            }
        }

        private void Player_PickingUpItem(Exiled.Events.EventArgs.PickingUpItemEventArgs ev)
        {
            if (ev.IsAllowed && this.player == ev.Player)
            {
                if (CustomItem.TryGet(ev.Pickup, out CustomItem item))
                {
                    if (item.Id == (uint)MistakenCustomItems.MEDIC_GUN)
                    {
                        if (!this.isActive)
                            Handlers.MTFMedicHandler.Instance.RunCoroutine(this.RegenerateAmmo(ev.Player));
                    }
                }
            }
        }

        private IEnumerator<float> RegenerateAmmo(Player player)
        {
            RLogger.Log("MTF MEDIC", "ABILITY", $"Started regenerating ammo for {player.PlayerToString()}");
            this.isActive = true;
            Firearm medicgun = (Firearm)player.Items.FirstOrDefault(x => CustomItem.TryGet(x, out _));
            if (medicgun is null) yield break;
            while (medicgun.Ammo < 4)
            {
                yield return Timing.WaitForSeconds(PluginHandler.Instance.Config.MedicGunBulletRecoveryTime);
                medicgun = (Firearm)player.Items.FirstOrDefault(x => CustomItem.TryGet(x, out _));
                if (medicgun is null) break;
                RLogger.Log("MTF MEDIC", "ABILITY", $"Regenerated 1 ammo for {player.PlayerToString()}");
                medicgun.Ammo++;
            }

            RLogger.Log("MTF MEDIC", "ABILITY", $"Stopped regenerating ammo for {player.PlayerToString()}");
            this.isActive = false;
        }
    }
}
