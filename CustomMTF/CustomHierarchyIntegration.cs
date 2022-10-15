// -----------------------------------------------------------------------
// <copyright file="CustomHierarchyIntegration.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;
using Mistaken.CustomMTF.Classes;
using static Mistaken.CustomHierarchy.HierarchyHandler;

#pragma warning disable SA1118 // Parameter should not span multiple lines

namespace Mistaken.CustomMTF
{
    internal static class CustomHierarchyIntegration
    {
        public static void Init()
        {
            Log.Debug("Enabling CustomHierarchy integration.", PluginHandler.Instance.Config.VerboseOutput);

            CustomPlayerComperers.Add(
                "tau5_comparer",
                (
                    int.MaxValue,
                    (Player p1, Player p2) =>
                    {
                        if (!(p1.Role.Team == Team.MTF && p2.Role.Team == Team.MTF))
                            return CompareResult.NO_ACTION;

                        if (Tau5Soldier.Instance.Check(p1) || Tau5Soldier.Instance.Check(p2))
                            return CompareResult.SAME_RANK;

                        return CompareResult.NO_ACTION;
                    }));

            Log.Debug("Enabled CustomHierarchy integration.", PluginHandler.Instance.Config.VerboseOutput);
        }
    }
}
