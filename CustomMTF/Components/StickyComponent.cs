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
        private bool onPlayerUsed;

        private bool onSurfaceUsed;

        private Rigidbody rigidbody;

        private void Awake()
        {
            this.rigidbody = this.GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!this.onSurfaceUsed && !this.onPlayerUsed)
            {
                this.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                this.onSurfaceUsed = true;
            }
        }

        private void FixedUpdate()
        {
            if (Items.StickyGrenadeHandler.StickyGrenadeItem.GrenadePlayer != null && !this.onSurfaceUsed)
            {
                this.onPlayerUsed = true;
                this.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                var hitposition = Items.StickyGrenadeHandler.StickyGrenadeItem.GrenadePlayer.Position - Items.StickyGrenadeHandler.StickyGrenadeItem.GrenadeGo.transform.position;
                Items.StickyGrenadeHandler.StickyGrenadeItem.GrenadeGo.transform.position = Items.StickyGrenadeHandler.StickyGrenadeItem.GrenadePlayer.Position + hitposition;
            }
        }

        private void OnDestroy()
        {
            Items.StickyGrenadeHandler.StickyGrenadeItem.GrenadePlayer = null;
        }
    }
}
