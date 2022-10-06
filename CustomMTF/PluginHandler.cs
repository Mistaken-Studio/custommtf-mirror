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
    internal sealed class PluginHandler : Plugin<Config, Translation>
    {
        public override string Author => "Mistaken Devs";

        public override string Name => "CustomMTF";

        public override string Prefix => "MCMTF";

        public override PluginPriority Priority => PluginPriority.Low;

        public override Version RequiredExiledVersion => new (5, 2, 2);

        public override void OnEnabled()
        {
            Instance = this;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            base.OnDisabled();
        }

        internal static PluginHandler Instance { get; private set; }
    }
}
