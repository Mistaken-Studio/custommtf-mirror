// -----------------------------------------------------------------------
// <copyright file="MedicGunHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Interfaces;
using Exiled.Events.EventArgs;
using Grenades;
using MEC;
using Mistaken.API.Diagnostics;

namespace Mistaken.CustomMTF.Handlers
{
    /// <summary>
    /// Gun that heals hit players.
    /// </summary>
    public class MedicGunHandler : Module
    {
        /// <inheritdoc cref="Module.Module(IPlugin{IConfig})"/>
        public MedicGunHandler(IPlugin<IConfig> plugin)
            : base(plugin)
        {
            Instance = this;
            new Items.MedicGunItem();
        }

        /// <inheritdoc/>
        public override string Name => "MedicGunHandler";

        /// <inheritdoc/>
        public override void OnEnable()
        {
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
        }

        internal static MedicGunHandler Instance { get; private set; }

        internal const float HealAmount = 35;
    }
}
