// -----------------------------------------------------------------------
// <copyright file="GuardCommanderKeycardItem.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs;
using MEC;
using Mistaken.API.CustomItems;
using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;
using Mistaken.API.GUI;
using Mistaken.RoundLogger;
using UnityEngine;

namespace Mistaken.CustomMTF.Miscellaneous
{
    /// <summary>
    /// Keycard that Guard commander uses.
    /// </summary>
    [CustomItem(ItemType.KeycardNTFOfficer)]
    public sealed class GuardCommanderKeycardItem : MistakenCustomItem
    {
        /// <inheritdoc/>
        public override MistakenCustomItems CustomItem => MistakenCustomItems.GUARD_COMMANDER_KEYCARD;

        /// <inheritdoc/>
        public override ItemType Type { get; set; } = ItemType.KeycardNTFOfficer;

        /// <inheritdoc/>
        public override string Name { get; set; } = "Karta Dowódcy Ochrony";

        /// <inheritdoc/>
        public override string DisplayName => "Karta Dowódcy Ochrony";

        /// <inheritdoc/>
        public override string Description { get; set; } = "Guard Commander's Keycard";

        /// <inheritdoc/>
        public override float Weight { get; set; } = 0.01f;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; }

        /// <inheritdoc/>
        public override void Init()
        {
            Instance = this;
            base.Init();
        }

        /// <inheritdoc/>
        public override Pickup Spawn(Vector3 position, Player previousOwner = null)
        {
            return this.Spawn(position, this.CreateCorrectItem(), previousOwner);
        }

        /// <inheritdoc/>
        public override Pickup Spawn(Vector3 position, Item item, Player previousOwner = null)
        {
            var pickup = base.Spawn(position, item, previousOwner);
            RLogger.Log("GUARD COMMANDER KEYCARD", "SPAWN", $"{this.Name} spawned");
            pickup.Scale = KeycardSize;
            return pickup;
        }

        internal static GuardCommanderKeycardItem Instance { get; private set; }

        internal Player CurrentOwner { get; set; }

        /// <inheritdoc/>
        protected override void ShowSelectedMessage(Player player)
        {
            Module.RunSafeCoroutine(this.UpdateInterface(player), nameof(this.UpdateInterface));
        }

        /// <inheritdoc/>
        protected override void OnDropping(DroppingItemEventArgs ev)
        {
            if (this.CurrentOwner is null)
                return;

            this.CurrentOwner = null;
        }

        /// <inheritdoc/>
        protected override void OnWaitingForPlayers()
        {
            this.CurrentOwner = null;
        }

        private static readonly Vector3 KeycardSize = new(1, 5, 1);

        private IEnumerator<float> UpdateInterface(Player player)
        {
            yield return Timing.WaitForSeconds(0.1f);

            while (this.Check(player.CurrentItem))
            {
                if (!(Classes.GuardCommander.Instance.Check(player) || player == this.CurrentOwner))
                    player.SetGUI("GC_Keycard", PseudoGUIPosition.BOTTOM, "<color=yellow>Trzymasz</color> kartę <color=blue>Dowódcy Ochrony</color>, ale chyba <color=yellow>nie</color> możesz jej używać");

                yield return Timing.WaitForSeconds(1f);
            }

            player.SetGUI("GC_Keycard", PseudoGUIPosition.BOTTOM, null);
        }
    }
}
