﻿// -----------------------------------------------------------------------
// <copyright file="GrenadeLauncherItem.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs;
using InventorySystem.Items.Firearms.BasicMessages;
using MEC;
using Mistaken.API.Extensions;
using Mistaken.API.GUI;
using Mistaken.RoundLogger;
using UnityEngine;

namespace Mistaken.CustomMTF.Items
{
    /// <inheritdoc/>
    public class GrenadeLauncherItem : CustomWeapon
    {
        /// <inheritdoc/>
        public override uint Id { get; set; } = 5;

        /// <inheritdoc/>
        public override ItemType Type { get; set; } = ItemType.GunCOM18;

        /// <inheritdoc/>
        public override string Name { get; set; } = "Grenade Launcher";

        /// <inheritdoc/>
        public override string Description { get; set; } = "Sticky Grenade Launcher";

        /// <inheritdoc/>
        public override float Weight { get; set; } = 0.7f;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; }

        /// <inheritdoc/>
        public override Modifiers Modifiers { get; set; }

        /// <inheritdoc/>
        public override float Damage { get; set; } = 0;

        /// <inheritdoc/>
        public override byte ClipSize { get; set; } = 3;

        /// <inheritdoc/>
        public override void Give(Player player, bool displayMessage)
        {
            RLogger.Log("GRENADE LAUNCHER", "GIVE", $"Grenade Launcher given to {player.PlayerToString()}");
            base.Give(player, displayMessage);
        }

        /// <inheritdoc/>
        public override Pickup Spawn(Vector3 position)
        {
            var pickup = base.Spawn(position);
            pickup.Scale = Size;
            pickup.Base.Info.Serial = pickup.Serial;
            RLogger.Log("GRENADE LAUNCHER", "SPAWN", $"Grenade Launcher spawned");
            return pickup;
        }

        /// <inheritdoc/>
        public override Pickup Spawn(Vector3 position, Item item)
        {
            var pickup = base.Spawn(position, item);
            pickup.Scale = Size;
            pickup.Base.Info.Serial = pickup.Serial;
            RLogger.Log("GRENADE LAUNCHER", "SPAWN", $"Grenade Launcher spawned");
            return pickup;
        }

        internal static readonly Vector3 Size = new Vector3(2f, 1.5f, 1.5f);

        /// <inheritdoc/>
        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (ev.Firearm.Ammo >= this.ClipSize + 1)
            {
                ev.Player.SetGUI("grenadeLauncherWarn", PseudoGUIPosition.BOTTOM, "Nie możesz przeładować mając pełny magazynek", 3);
                ev.IsAllowed = false;
                return;
            }

            if (!ev.Player.Items.Any(i => i.Type == ItemType.GrenadeHE))
            {
                ev.Player.SetGUI("grenadeLauncherWarn", PseudoGUIPosition.BOTTOM, "Nie masz amunicji (zwykły <color=yellow>Granat Odłamkowy</color>)", 3);
                ev.IsAllowed = false;
                return;
            }

            RLogger.Log("GRENADE LAUNCHER", "RELOAD", $"Player {ev.Player.PlayerToString()} reloaded Grenade Launcher");
            ev.Player.RemoveItem(ev.Player.Items.First(i => i.Type == ItemType.GrenadeHE));
            ev.Player.SetGUI("grenadeLauncherWarn", PseudoGUIPosition.BOTTOM, "Przeładowano", 3);
            ev.Player.Connection.Send(new RequestMessage(ev.Firearm.Serial, RequestType.Reload));
            ev.Firearm.Ammo++;
            ev.IsAllowed = false;
        }

        /// <inheritdoc/>
        protected override void OnShooting(ShootingEventArgs ev)
        {
            if (((Firearm)ev.Shooter.CurrentItem).Ammo == 0)
            {
                ev.Shooter.SetGUI("grenadeLauncherWarn", PseudoGUIPosition.BOTTOM, "Nie możesz strzelać z pustym magazynkiem", 3);
                ev.IsAllowed = false;
                return;
            }

            StickyGrenadeItem.Throw(ev.Shooter);
            RLogger.Log("GRENADE LAUNCHER", "FIRE", $"Player {ev.Shooter.PlayerToString()} fired Grenade Launcher");
            ((Firearm)ev.Shooter.CurrentItem).Ammo--;
            Hitmarker.SendHitmarker(ev.Shooter.Connection, 5f);
            ev.IsAllowed = false;
            return;
        }

        /// <inheritdoc/>
        protected override void ShowPickedUpMessage(Player player)
        {
        }

        /// <inheritdoc/>
        protected override void ShowSelectedMessage(Player player)
        {
            Handlers.GrenadeLauncherHandler.Instance.RunCoroutine(this.UpdateInterface(player));
        }

        private IEnumerator<float> UpdateInterface(Player player)
        {
            bool ammogiven = false;
            yield return Timing.WaitForSeconds(0.1f);
            while (this.Check(player.CurrentItem))
            {
                if (!ammogiven)
                {
                    player.Ammo[ItemType.Ammo9x19]++;
                    ammogiven = true;
                }

                player.SetGUI("grenadeLauncher", PseudoGUIPosition.BOTTOM, $"Trzymasz <color=yellow>Wyrzutnię Granatów</color>");
                yield return Timing.WaitForSeconds(1);
            }

            player.Ammo[ItemType.Ammo9x19]--;
            player.SetGUI("grenadeLauncher", PseudoGUIPosition.BOTTOM, null);
        }
    }
}
