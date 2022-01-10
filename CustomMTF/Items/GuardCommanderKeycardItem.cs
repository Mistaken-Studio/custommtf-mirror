// -----------------------------------------------------------------------
// <copyright file="GuardCommanderKeycardItem.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs;
using MEC;
using Mistaken.API.CustomItems;
using Mistaken.API.CustomRoles;
using Mistaken.API.Extensions;
using Mistaken.API.GUI;
using Mistaken.RoundLogger;
using UnityEngine;

namespace Mistaken.CustomMTF.Items
{
    /// <summary>
    /// Keycard that Guard commander uses.
    /// </summary>
    public class GuardCommanderKeycardItem : MistakenCustomItem
    {
        /// <summary>
        /// Gets the guard commander keycard instance.
        /// </summary>
        public static GuardCommanderKeycardItem Instance { get; private set; }

        /// <summary>
        /// Gets or sets owner of the item.
        /// </summary>
        public Player CurrentOwner { get; set; }

        /// <inheritdoc/>
        public override MistakenCustomItems CustomItem => MistakenCustomItems.GUARD_COMMANDER_KEYCARD;

        /// <inheritdoc/>
        public override ItemType Type { get; set; } = ItemType.KeycardNTFOfficer;

        /// <inheritdoc/>
        public override string Name { get; set; } = "Karta Dowódcy Ochrony";

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
        public override Pickup Spawn(Vector3 position)
        {
            Item keycard = new Item(this.Type);
            RLogger.Log("GUARD COMMANDER KEYCARD", "SPAWN", $"{this.Name} spawned");
            return this.Spawn(position, keycard);
        }

        /// <inheritdoc/>
        public override Pickup Spawn(Vector3 position, Item item)
        {
            item.Scale = Handlers.GuardCommanderHandler.Size;
            return item.Spawn(position);
        }

        /// <inheritdoc/>
        protected override void ShowSelectedMessage(Player player)
        {
            Handlers.GuardCommanderHandler.Instance.RunCoroutine(this.UpdateInterface(player));
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

        private IEnumerator<float> UpdateInterface(Player player)
        {
            yield return Timing.WaitForSeconds(0.1f);

            while (this.Check(player.CurrentItem))
            {
                if (Classes.GuardCommander.Instance.Check(player) || player == this.CurrentOwner)
                    player.SetGUI("GC_Keycard", PseudoGUIPosition.BOTTOM, "<color=yellow>Trzymasz</color> kartę <color=blue>Dowódcy Ochrony</color>");
                else
                    player.SetGUI("GC_Keycard", PseudoGUIPosition.BOTTOM, "<color=yellow>Trzymasz</color> kartę <color=blue>Dowódcy Ochrony</color>, ale chyba <color=yellow>nie</color> możesz jej używać");

                yield return Timing.WaitForSeconds(1f);
            }

            player.SetGUI("GC_Keycard", PseudoGUIPosition.BOTTOM, null);
        }
    }
}
