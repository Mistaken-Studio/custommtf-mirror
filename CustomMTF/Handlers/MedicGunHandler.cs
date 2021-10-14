﻿// -----------------------------------------------------------------------
// <copyright file="MedicGunHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Interfaces;
using Mistaken.API.Diagnostics;
using UnityEngine;

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
            new Items.MedicGunItem().TryRegister();
        }

        /// <inheritdoc/>
        public override string Name => nameof(MedicGunHandler);

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

        internal const float BulletRecoveryTime = 90;

        internal static readonly Vector3 Size = new Vector3(2, 2, 2);
    }
}
