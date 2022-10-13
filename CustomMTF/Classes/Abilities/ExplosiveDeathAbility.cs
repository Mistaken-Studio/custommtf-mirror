// -----------------------------------------------------------------------
// <copyright file="ExplosiveDeathAbility.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.CustomRoles.API.Features;

namespace Mistaken.CustomMTF.Classes.Abilities
{
    /// <summary>
    /// Spawns grenade on death.
    /// </summary>
    [CustomAbility]
    public sealed class ExplosiveDeathAbility : PassiveAbility
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = nameof(ExplosiveDeathAbility);

        /// <inheritdoc/>
        public override string Description { get; set; } = "Let's you explode on death";

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Dying += this.Player_Dying;
            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Dying -= this.Player_Dying;
            base.UnsubscribeEvents();
        }

        private void Player_Dying(Exiled.Events.EventArgs.DyingEventArgs ev)
        {
            if (ev.IsAllowed && this.Check(ev.Target))
            {
                var grenade = Item.Create(ItemType.GrenadeHE, ev.Target) as ExplosiveGrenade;
                grenade.FuseTime = 5f;
                grenade.SpawnActive(ev.Target.Position);
            }
        }
    }
}
