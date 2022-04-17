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
using Mistaken.RoundLogger;

namespace Mistaken.CustomMTF.Classes.Abilities
{
    /// <summary>
    /// Ability that makes Medic Gun regenerate ammo.
    /// </summary>
    [CustomAbility]
    public class MedicGunAmmoRegenAbility : PassiveAbility
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = nameof(MedicGunAmmoRegenAbility);

        /// <inheritdoc/>
        public override string Description { get; set; } = "Let's you regenerate Medic Gun's ammo";

        /// <inheritdoc/>
        protected override void AbilityAdded(Player player)
        {
            Module.RunSafeCoroutine(this.RegenerateAmmo(player), "custommtf_medicgun_ability");
        }

        private IEnumerator<float> RegenerateAmmo(Player player)
        {
            yield return Timing.WaitForSeconds(5f);
            Firearm item = (Firearm)player.Items.FirstOrDefault(x => MistakenCustomItems.MEDIC_GUN.TryGet(out var y) && y.Check(x));
            while (this.Players.Contains(player))
            {
                if (!player?.IsConnected ?? true)
                    break;
                if (!player.HasItem(item))
                    item = (Firearm)player.Items.FirstOrDefault(x => MistakenCustomItems.MEDIC_GUN.TryGet(out var y) && y.Check(x));
                while (item.Ammo < 4)
                {
                    yield return Timing.WaitForSeconds(PluginHandler.Instance.Config.MedicGunBulletRecoveryTime);
                    if (player?.HasItem(item) ?? false)
                        item.Ammo++;
                    RLogger.Log("MTF MEDIC", "ABILITY", $"Regenerated 1 ammo for {player.Nickname}");
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
