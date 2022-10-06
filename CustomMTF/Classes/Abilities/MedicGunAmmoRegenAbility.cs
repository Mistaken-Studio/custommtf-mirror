// -----------------------------------------------------------------------
// <copyright file="MedicGunAmmoRegenAbility.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.CustomRoles.API.Features;
using MEC;
using Mistaken.API.CustomItems;
using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;
using Mistaken.RoundLogger;

namespace Mistaken.CustomMTF.Classes.Abilities
{
    /// <summary>
    /// Ability that makes Medic Gun regenerate ammo.
    /// </summary>
    [CustomAbility]
    public sealed class MedicGunAmmoRegenAbility : PassiveAbility
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = nameof(MedicGunAmmoRegenAbility);

        /// <inheritdoc/>
        public override string Description { get; set; } = "Let's you regenerate Medic Gun's ammo";

        /// <inheritdoc/>
        protected override void AbilityAdded(Player player)
        {
            Module.RunSafeCoroutine(this.RegenerateAmmo(player), nameof(this.RegenerateAmmo));
        }

        private IEnumerator<float> RegenerateAmmo(Player player)
        {
            yield return Timing.WaitForSeconds(5f);
            var medicGun = MistakenCustomItems.MEDIC_GUN.Get();
            Firearm item = (Firearm)player.Items.FirstOrDefault(x => medicGun.Check(x));
            while (this.Players.Contains(player))
            {
                if (!player.IsConnected())
                    break;

                if (item is null || !player.HasItem(item))
                    item = (Firearm)player.Items.FirstOrDefault(x => medicGun.Check(x));

                while (item is not null && item.Ammo < 4)
                {
                    yield return Timing.WaitForSeconds(PluginHandler.Instance.Config.MedicGunBulletRecoveryTime);
                    if (!player.IsConnected())
                        break;

                    if (item is not null && player.HasItem(item))
                        item.Ammo++;

                    RLogger.Log("MTF MEDIC", "ABILITY", $"Regenerated 1 ammo for {player.Nickname}");
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
