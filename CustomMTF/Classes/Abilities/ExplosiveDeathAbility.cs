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
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Dying += this.Player_Dying;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Dying -= this.Player_Dying;
        }

        private void Player_Dying(Exiled.Events.EventArgs.DyingEventArgs ev)
        {
            if (ev.IsAllowed && this.Players.Contains(ev.Target))
            {
                ExplosiveGrenade grenade = Item.Create(ItemType.GrenadeHE) as ExplosiveGrenade;
                grenade.FuseTime = 5f;
                grenade.SpawnActive(ev.Target.Position);
            }
        }
    }
}
