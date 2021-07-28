// -----------------------------------------------------------------------
// <copyright file="MTFMedic.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;
using Mistaken.API;
using Mistaken.API.Extensions;

namespace Mistaken.CustomMTF.Classes
{
    /// <inheritdoc/>
    public class MTFMedic : CustomClasses.CustomClass
    {
        /// <inheritdoc cref="CustomClasses.CustomClass.CustomClass()"/>
        public MTFMedic()
        {
            this.Register();
            Instance = this;
        }

        /// <inheritdoc/>
        public override SessionVarType ClassSessionVarType => SessionVarType.CC_MTF_MEDIC;

        /// <inheritdoc/>
        public override string ClassName => "MTF Medic";

        /// <inheritdoc/>
        public override string ClassDescription => "MTF Medic";

        /// <inheritdoc/>
        public override RoleType Role => RoleType.NtfLieutenant;

        /// <inheritdoc/>
        public override string Color => "#0095FF"; // cadet color #70C3FF

        /// <inheritdoc/>
        public override void Spawn(Player player)
        {
            base.Spawn(player);
            player.ClearInventory();
            player.AddItem(ItemType.GunE11SR);
            player.AddItem(ItemType.KeycardNTFLieutenant);
            player.AddItem(ItemType.Disarmer);
            player.AddItem(ItemType.WeaponManagerTablet);
            player.AddItem(ItemType.Adrenaline);
            player.AddItem(ItemType.Medkit);
            player.AddItem(ItemType.Medkit);
            player.AddItem(new Inventory.SyncItemInfo
            {
                id = ItemType.GunCOM15,
                durability = 1003,
            });
            player.SetGUI("cc_mtf_medic", API.GUI.PseudoGUIPosition.BOTTOM, $"You are <color=yellow>playing</color> as <color={this.Color}>{this.ClassName}</color>");
        }

        /// <inheritdoc/>
        public override void OnDie(Player player)
        {
            player.SetGUI("cc_mtf_medic", API.GUI.PseudoGUIPosition.BOTTOM, null);
            base.OnDie(player);
        }

        internal static MTFMedic Instance { get; private set; }
    }
}
