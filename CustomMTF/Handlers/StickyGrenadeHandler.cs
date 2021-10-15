// -----------------------------------------------------------------------
// <copyright file="StickyGrenadeHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Interfaces;
using Mistaken.API.Diagnostics;

namespace Mistaken.CustomMTF.Handlers
{
    /// <summary>
    /// Grenade that attaches to surfaces/players.
    /// </summary>
    public class StickyGrenadeHandler : Module
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
            new Items.StickyGrenadeItem().TryRegister();
        }

        /// <inheritdoc/>
        public override string Name => nameof(StickyGrenadeHandler);

        /// <inheritdoc/>
        public override void OnEnable()
        {
            Exiled.Events.Handlers.Server.RoundStarted += this.Handle(() => this.Server_RoundStarted(), "RoundStart");
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= this.Handle(() => this.Server_RoundStarted(), "RoundStart");
        }

        private void Server_RoundStarted()
        {
            Patches.ExplodeDestructiblesPatch.Grenades.Clear();
            Patches.ServerThrowPatch.ThrowedItems.Clear();
        }
    }
}
