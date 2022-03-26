// -----------------------------------------------------------------------
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
using Mistaken.API;
using Mistaken.API.CustomItems;
using Mistaken.API.Diagnostics;
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
            Module.RunSafeCoroutine(this.RegenerateAmmo(player), "custommtf_medicgun_ability");
        }

        private IEnumerator<float> RegenerateAmmo(Player player)
        {
            while (this.Players.Contains(player))
            {
                yield return Timing.WaitForSeconds(5f);
                var item = (Firearm)player.Items.FirstOrDefault(x => MistakenCustomItems.MEDIC_GUN.TryGet(out var ci) && ci.Check(x));
                while (!(item is null) && item.Ammo < 4)
                {
                    yield return Timing.WaitForSeconds(PluginHandler.Instance.Config.MedicGunBulletRecoveryTime);
                    if (!player.IsConnected)
                    {
                        if (this.Players.Contains(player))
                            this.Players.Remove(player);
                        break;
                    }

                    if (!player.Items.Contains(item))
                        item = null;
                    if (item is null)
                        break;
                    item.Ammo++;
                    RLogger.Log("MTF MEDIC", "ABILITY", $"Regenerated 1 ammo for {player.Nickname}");
                }
            }
        }
    }
}
