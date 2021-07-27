// -----------------------------------------------------------------------
// <copyright file="StickyComponent.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs;
using Grenades;
using MEC;
using Mistaken.API;
using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;
using Mistaken.CustomItems;
using UnityEngine;

namespace Mistaken.CustomMTF.Components
{
    /// <summary>
    /// Handles freeze on impact with surfaces.
    /// </summary>
    public class StickyComponent : MonoBehaviour
    {
        private bool used;

        private void OnCollisionEnter(Collision collision)
        {
            if (!this.used && this.TryGetComponent<Rigidbody>(out Rigidbody component))
                component.constraints = RigidbodyConstraints.FreezeAll;

            this.used = true;
        }

        private void FixedUpdate()
        {
            if (Items.StickyGrenadeItem.GrenadePlayer == null) return;
            var hitposition = Items.StickyGrenadeItem.GrenadePlayer.Position - Items.StickyGrenadeItem.GrenadeGo.transform.position;
            Items.StickyGrenadeItem.GrenadeGo.transform.position = Items.StickyGrenadeItem.GrenadePlayer.Position + hitposition;
            Items.StickyGrenadeItem.GrenadePlayer.SendConsoleMessage("works", "blue");
        }
    }
}
