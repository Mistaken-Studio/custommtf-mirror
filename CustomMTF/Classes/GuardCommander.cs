// -----------------------------------------------------------------------
// <copyright file="GuardCommander.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Exiled.API.Features;
using Mistaken.API;
using Mistaken.API.CustomRoles;
using Mistaken.API.Extensions;
using Mistaken.API.GUI;
using Mistaken.RoundLogger;

namespace Mistaken.CustomMTF.Classes
{
    /// <inheritdoc/>
    public class GuardCommander : MistakenCustomRole
    {
        /// <inheritdoc/>
        public override MistakenCustomRoles CustomRole => MistakenCustomRoles.GUARD_COMMANDER;

        /// <inheritdoc/>
        public override RoleType Role { get; set; } = RoleType.NtfCaptain;

        /// <inheritdoc/>
        public override int MaxHealth { get; set; } = 100;

        /// <inheritdoc/>
        public override string Name { get; set; } = "Guard Commander";

        /// <inheritdoc/>
        public override string Description { get; set; } = "Twoim zadaniem jest <color=yellow>dowodzenie</color> <color=#7795a9>ochroną placówki</color>.<br>Twoja karta <color=yellow>pozwala</color> ci otworzyć Gate A i Gate B, ale tylko gdy:<br>- Obok jest <color=#f1e96e>Naukowiec</color><br>- Obok jest skuta <color=#ff8400>Klasa D</color><br>- Obok jest skuty <color=#1d6f00>Rebeliant Chaosu</color>";

        /// <inheritdoc/>
        public override void AddRole(Player player)
        {
            base.AddRole(player);
            MEC.Timing.CallDelayed(2, () =>
            {
                player.Ammo[ItemType.Ammo556x45] = 80;
                player.Ammo[ItemType.Ammo9x19] = 50;
            });
            player.InfoArea = ~PlayerInfoArea.Role;
            CustomInfoHandler.Set(player, "Guard_Commander", "<color=blue><b>Dowódca Ochrony</b></color>");
            player.SetGUI("Guard_Commander", PseudoGUIPosition.MIDDLE, $"<size=150%>Jesteś <color=blue>Dowódcą Ochrony</color></size><br>{this.Description}", 20);
            player.SetGUI("Guard_Commander_Info", PseudoGUIPosition.BOTTOM, "<color=yellow>Grasz</color> jako <color=blue>Dowódca Ochrony</color>");
            RLogger.Log("GUARD COMMANDER", "SPAWN", $"Player {player.PlayerToString()} is now a {this.Name}");
        }

        /// <inheritdoc/>
        public override void RemoveRole(Player player)
        {
            base.RemoveRole(player);
            CustomInfoHandler.Set(player, "Guard_Commander", null);
            player.SetGUI("Guard_Commander_Info", PseudoGUIPosition.BOTTOM, null);
            RLogger.Log("GUARD COMMANDER", "DEATH", $"Player {player.PlayerToString()} is no longer a {this.Name}");
        }

        /// <inheritdoc/>
        protected override bool KeepInventoryOnSpawn { get; set; } = false;

        /// <inheritdoc/>
        protected override bool KeepRoleOnDeath { get; set; } = false;

        /// <inheritdoc/>
        protected override List<string> Inventory { get; set; } = new List<string>
        {
            "Karta Dowódcy Ochrony",
            ItemType.GunCrossvec.ToString(),
            ItemType.Radio.ToString(),
            "Taser",
            "Impact Grenade",
            ItemType.ArmorCombat.ToString(),
            ItemType.Medkit.ToString(),
        };
    }
}
