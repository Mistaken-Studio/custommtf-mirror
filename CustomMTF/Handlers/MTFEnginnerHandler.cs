// -----------------------------------------------------------------------
// <copyright file="MTFEnginnerHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Interfaces;
using Mistaken.API.Diagnostics;

namespace Mistaken.CustomMTF.Handlers
{
    internal class MTFEnginnerHandler : Module
    {
        public MTFEnginnerHandler(IPlugin<IConfig> plugin)
            : base(plugin)
        {
        }

        public override string Name => nameof(MTFEnginnerHandler);

        public override void OnEnable()
        {
        }

        public override void OnDisable()
        {
        }
    }
}
