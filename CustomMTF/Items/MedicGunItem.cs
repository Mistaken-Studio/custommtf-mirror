// -----------------------------------------------------------------------
// <copyright file="MedicGunItem.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using Mistaken.API.Extensions;
using Mistaken.RoundLogger;
using UnityEngine;

namespace Mistaken.CustomMTF.Items
{
    /// <inheritdoc/>
    public class MedicGunItem : CustomWeapon
    {
        /// <inheritdoc/>
        public override uint Id { get; set; } = 4;

        /// <inheritdoc/>
        public override ItemType Type { get; set; } = ItemType.GunRevolver;

        /// <inheritdoc/>
        public override string Name { get; set; } = "Medic Gun";

        /// <inheritdoc/>
        public override string Description { get; set; } = "Medic Gun";

        /// <inheritdoc/>
        public override float Weight { get; set; } = 0.75f;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; }

        /// <inheritdoc/>
        public override Modifiers Modifiers { get; set; }

        /// <inheritdoc/>
        public override float Damage { get; set; } = 0;

        /// <inheritdoc/>
        public override byte ClipSize { get; set; } = 4;

        /// <inheritdoc/>
        public override void Give(Player player, bool displayMessage)
        {
            Item item = player.AddItem(this.Type);
            Firearm firearm = item as Firearm;
            if (firearm != null)
            {
                firearm.Ammo = this.ClipSize;
                firearm.Base.Status = new InventorySystem.Items.Firearms.FirearmStatus(firearm.Ammo, firearm.Base.Status.Flags, 594);
            }

            RLogger.Log("MEDIC GUN", "GIVE", $"Given {this.Name} to {player.PlayerToString()}");
            this.TrackedSerials.Add(item.Serial);
        }

        /// <inheritdoc/>
        public override Pickup Spawn(Vector3 position)
        {
            var item = new Item(this.Type);
            Firearm firearm = item as Firearm;
            if (!(firearm is null))
            {
                firearm.Ammo = this.ClipSize;
                firearm.Base.Status = new InventorySystem.Items.Firearms.FirearmStatus(this.ClipSize, firearm.Base.Status.Flags, 594);
            }

            return this.Spawn(position, item);
        }

        /// <inheritdoc/>
        public override Pickup Spawn(Vector3 position, Item item)
        {
            var pickup = base.Spawn(position, item);
            pickup.Base.Info.Serial = pickup.Serial;
            pickup.Scale = Handlers.MedicGunHandler.Size;
            Firearm firearm = (Firearm)item;
            if (!(firearm is null))
                ((InventorySystem.Items.Firearms.FirearmPickup)pickup.Base).Status = firearm.Base.Status;
            RLogger.Log("MEDIC GUN", "SPAWN", $"Spawned {this.Name}");
            return pickup;
        }

        /// <inheritdoc/>
        protected override void ShowPickedUpMessage(Player player)
        {
        }

        /// <inheritdoc/>
        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        /// <inheritdoc/>
        protected override void OnShot(ShotEventArgs ev)
        {
            if (!(ev.Target is null))
            {
                var hpToHeal = Math.Min(ev.Target.MaxHealth - ev.Target.Health, Handlers.MedicGunHandler.HealAmount);
                var ahpToHeal = Handlers.MedicGunHandler.HealAmount - hpToHeal;
                ev.Target.Health += hpToHeal;
                ev.Target.ArtificialHealth += ahpToHeal;
                RLogger.Log("MEDIC GUN", "HEAL", $"Player {ev.Shooter.PlayerToString()} hit player {ev.Target.PlayerToString()} and regenerated {hpToHeal} hp and {ahpToHeal} ahp");
                ev.CanHurt = false;
            }
        }

        /// <inheritdoc/>
        protected override void ShowSelectedMessage(Player player)
        {
            Handlers.MedicGunHandler.Instance.RunCoroutine(this.UpdateInterface(player));
        }

        private IEnumerator<float> UpdateInterface(Player player)
        {
            yield return Timing.WaitForSeconds(0.1f);
            while (this.Check(player.CurrentItem))
            {
                player.SetGUI("ci_medic_gun_hold", API.GUI.PseudoGUIPosition.BOTTOM, "Trzymasz <color=yellow>Pistolet Medyczny</color>");
                yield return Timing.WaitForSeconds(1f);
            }

            player.SetGUI("ci_medic_gun_hold", API.GUI.PseudoGUIPosition.BOTTOM, null);
        }
    }
}
