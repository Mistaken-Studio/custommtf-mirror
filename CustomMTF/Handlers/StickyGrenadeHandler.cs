// -----------------------------------------------------------------------
// <copyright file="StickyGrenadeHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs;
using Mistaken.API.Diagnostics;
using UnityEngine;

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
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
        }
    }
}
