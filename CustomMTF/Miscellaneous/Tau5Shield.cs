// -----------------------------------------------------------------------
// <copyright file="Tau5Shield.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Mistaken.API.Shield;

namespace Mistaken.CustomMTF.Miscellaneous
{
    internal sealed class Tau5Shield : Shield
    {
        protected override float MaxShield => 150f;

        protected override float ShieldRechargeRate => 5f;

        protected override float ShieldEffectivnes => 0.8f;

        protected override float TimeUntilShieldRecharge => 30f;

        protected override float ShieldDropRateOnOverflow => 0f;
    }
}
