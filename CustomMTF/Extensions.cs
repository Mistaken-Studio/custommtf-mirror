// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;

namespace Mistaken.CustomMTF
{
    internal static class Extensions
    {
        public static void SpawnPlayerWithRole(this List<Player> players, CustomRole cr, float spawnChance)
        {
            var count = Math.Floor(players.Count * (spawnChance / 100f));

            for (int i = 0; i < count; i++)
                cr.AddRole(players[i]);
        }
    }
}
