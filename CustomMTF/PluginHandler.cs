// -----------------------------------------------------------------------
// <copyright file="PluginHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Enums;
using Exiled.API.Features;

namespace Mistaken.CustomMTF
{
    /// <inheritdoc/>
    internal class PluginHandler : Plugin<Config, Translation>
    {
        /// <inheritdoc/>
        public override string Author => "Mistaken Devs";

        /// <inheritdoc/>
        public override string Name => "CustomMTF";

        /// <inheritdoc/>
        public override string Prefix => "MCMTF";

        /// <inheritdoc/>
        public override PluginPriority Priority => PluginPriority.Low;

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => new Version(5, 0, 0);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            base.OnDisabled();
        }

        internal static PluginHandler Instance { get; private set; }
    }
}
