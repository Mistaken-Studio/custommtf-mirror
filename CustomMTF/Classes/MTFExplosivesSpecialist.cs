// -----------------------------------------------------------------------
// <copyright file="MTFExplosivesSpecialist.cs" company="Mistaken">
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
    public class MTFExplosivesSpecialist : CustomClasses.CustomClass
    {
        /// <inheritdoc cref="CustomClasses.CustomClass.CustomClass()"/>
        public MTFExplosivesSpecialist()
        {
            this.Register();
            Instance = this;
        }

        /// <inheritdoc/>
        public override SessionVarType ClassSessionVarType => SessionVarType.CC_MTF_EXPLOSIVES_SPECIALIST;

        /// <inheritdoc/>
        public override string ClassName => "MTF Explosives Specialist";

        /// <inheritdoc/>
        public override string ClassDescription => "MTF Explosives Specialist";

        /// <inheritdoc/>
        public override RoleType Role => RoleType.NtfLieutenant;

        /// <inheritdoc/>
        public override string Color => "#0095FF";

        /// <inheritdoc/>
        public override void Spawn(Player player)
        {
            base.Spawn(player);
            player.ClearInventory();
            player.AddItem(ItemType.KeycardNTFLieutenant);
            player.AddItem(ItemType.GunProject90);
            player.AddItem(ItemType.WeaponManagerTablet);
            player.AddItem(ItemType.Radio);
            player.AddItem(ItemType.Disarmer);
            player.AddItem(new Inventory.SyncItemInfo
            {
                id = ItemType.GrenadeFrag,
                durability = 2000,
            });
            player.AddItem(new Inventory.SyncItemInfo
            {
                id = ItemType.GrenadeFrag,
                durability = 2000,
            });
            player.Ammo[(int)AmmoType.Nato556] = 40;
            player.Ammo[(int)AmmoType.Nato9] = 100;
            player.SetGUI("cc_mtf_es", API.GUI.PseudoGUIPosition.BOTTOM, $"You are <color=yellow>playing</color> as <color={this.Color}>{this.ClassName}</color>");
        }

        /// <inheritdoc/>
        public override void OnDie(Player player)
        {
            player.SetGUI("cc_mtf_es", API.GUI.PseudoGUIPosition.BOTTOM, null);
            base.OnDie(player);
        }

        internal static MTFExplosivesSpecialist Instance { get; private set; }
    }
}
