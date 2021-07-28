// -----------------------------------------------------------------------
// <copyright file="StickyGrenadeHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs;
using Grenades;
using MEC;
using Mistaken.API.Diagnostics;
using UnityEngine;

namespace Mistaken.CustomMTF.Items
{
    /// <summary>
    /// Grenade that attaches to surfaces/players.
    /// </summary>
    public partial class StickyGrenadeHandler : Module
    {
        /// <summary>
        /// Gets instance of the class.
        /// </summary>
        public static StickyGrenadeHandler Instance { get; private set; }

        /// <inheritdoc cref="Module.Module(IPlugin{IConfig})"/>
        public StickyGrenadeHandler(IPlugin<IConfig> plugin)
            : base(plugin)
        {
            Instance = this;
            Grenades = new HashSet<GameObject>();
            new StickyGrenadeItem();
        }

        /// <inheritdoc/>
        public override string Name => "StickyGrenadeHandler";

        /// <inheritdoc/>
        public override void OnEnable()
        {
            Exiled.Events.Handlers.Map.ExplodingGrenade += this.Handle<ExplodingGrenadeEventArgs>((ev) => this.Map_ExplodingGrenade(ev));
            Exiled.Events.Handlers.Server.RoundStarted += this.Handle(() => this.Server_RoundStarted(), "RoundStart");
            Exiled.Events.Handlers.Map.ChangingIntoGrenade += this.Handle<ChangingIntoGrenadeEventArgs>((ev) => this.Map_ChangingIntoGrenade(ev));
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Map.ExplodingGrenade -= this.Handle<ExplodingGrenadeEventArgs>((ev) => this.Map_ExplodingGrenade(ev));
            Exiled.Events.Handlers.Server.RoundStarted -= this.Handle(() => this.Server_RoundStarted(), "RoundStart");
            Exiled.Events.Handlers.Map.ChangingIntoGrenade += this.Handle<ChangingIntoGrenadeEventArgs>((ev) => this.Map_ChangingIntoGrenade(ev));
        }

        internal static readonly float DamageMultiplayer = 0.08f;

        internal static HashSet<GameObject> Grenades { get; private set; }

        private GrenadeManager lastThrower;

        private void Server_RoundStarted()
        {
            Grenades.Clear();
        }

        private void Map_ExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            if (!Grenades.Contains(ev.Grenade))
            {
                return;
            }

            var tmp = ev.Grenade.GetComponent<FragGrenade>().thrower;
            this.lastThrower = tmp;
            Action action = () =>
            {
                if (this.lastThrower == tmp)
                {
                    this.lastThrower = null;
                }
            };
            foreach (Player player in ev.TargetToDamages.Keys.ToArray())
            {
                ev.TargetToDamages[player] *= DamageMultiplayer;
            }

            this.CallDelayed(1, action, "MapExploadingGrenade");
        }

        private void Map_ChangingIntoGrenade(ChangingIntoGrenadeEventArgs ev)
        {
            if (ev.Pickup.durability != 2000f)
            {
                return;
            }

            ev.IsAllowed = false;
            Grenade grenade = UnityEngine.Object.Instantiate(Server.Host.GrenadeManager.availableGrenades[0].grenadeInstance).GetComponent<Grenade>();
            Grenades.Add(grenade.gameObject);
            grenade.InitData(this.lastThrower ?? Server.Host.GrenadeManager, Vector3.zero, Vector3.zero, 0f);
            grenade.transform.position = ev.Pickup.position;
            Mirror.NetworkServer.Spawn(grenade.gameObject);
            ev.Pickup.Delete();
        }
    }
}
