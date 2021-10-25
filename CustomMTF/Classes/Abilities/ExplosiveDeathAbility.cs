// -----------------------------------------------------------------------
// <copyright file="ExplosiveDeathAbility.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomRoles.API.Features;

namespace Mistaken.CustomMTF.Classes.Abilities
{
    /// <summary>
    /// Spawns grenade on death.
    /// </summary>
    public class ExplosiveDeathAbility : PassiveAbility
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = nameof(ExplosiveDeathAbility);

        /// <inheritdoc/>
        public override string Description { get; set; } = "Let's you explode on death";

        /// <inheritdoc/>
        protected override void AbilityAdded(Player player)
        {
            this.player = player;
            Exiled.Events.Handlers.Player.Dying += this.Player_Dying;
        }

        /// <inheritdoc/>
        protected override void AbilityRemoved(Player player)
        {
            Exiled.Events.Handlers.Player.Dying -= this.Player_Dying;
        }

        private Player player;

        private void Player_Dying(Exiled.Events.EventArgs.DyingEventArgs ev)
        {
            if (ev.IsAllowed && this.player == ev.Target)
            {
                ExplosiveGrenade grenade = new ExplosiveGrenade(ItemType.GrenadeHE);
                grenade.FuseTime = 5f;
                grenade.SpawnActive(ev.Target.Position, ev.Target);
            }
        }
    }
}
