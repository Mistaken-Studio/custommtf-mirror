// -----------------------------------------------------------------------
// <copyright file="MTFContainmentEnginner.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Features;
using Mistaken.API;
using Mistaken.API.Extensions;

namespace Mistaken.CustomMTF.Classes
{
    /// <inheritdoc/>
    public class MTFContainmentEnginner : CustomClasses.CustomClass
    {
        /// <inheritdoc cref="CustomClasses.CustomClass.CustomClass()"/>
        public MTFContainmentEnginner()
        {
            this.Register();
            Instance = this;
        }

        /// <inheritdoc/>
        public override SessionVarType ClassSessionVarType => SessionVarType.CC_MTF_CONTAINMENT_ENGINNER;

        /// <inheritdoc/>
        public override string ClassName => "MTF Containment Enginner";

        /// <inheritdoc/>
        public override string ClassDescription => "MTF Containment Enginner";

        /// <inheritdoc/>
        public override RoleType Role => RoleType.NtfCadet;

        /// <inheritdoc/>
        public override string Color => "#70C3FF";

        /// <inheritdoc/>
        public override void Spawn(Player player)
        {
            base.Spawn(player);
            player.ClearInventory();
            player.AddItem(ItemType.GunE11SR);
            player.AddItem(ItemType.KeycardContainmentEngineer);
            player.AddItem(ItemType.WeaponManagerTablet);
            player.AddItem(ItemType.GrenadeFrag);
            player.AddItem(ItemType.Radio);
            player.AddItem(ItemType.Disarmer);
            player.AddItem(ItemType.Medkit);
            player.Ammo[(int)AmmoType.Nato556] = 80;
            player.Ammo[(int)AmmoType.Nato9] = 50;
            player.SetGUI("cc_mtf_ce", API.GUI.PseudoGUIPosition.BOTTOM, $"You are <color=yellow>playing</color> as <color={this.Color}>{this.ClassName}</color>");
        }

        /// <inheritdoc/>
        public override void OnDie(Player player)
        {
            player.SetGUI("cc_mtf_ce", API.GUI.PseudoGUIPosition.BOTTOM, null);
            base.OnDie(player);
        }

        internal static MTFContainmentEnginner Instance { get; private set; }
    }
}
