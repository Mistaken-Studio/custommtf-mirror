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
        internal float DamageMultiplayer { get; set; } = 0.08f;

        private bool onPlayerUsed;
        private bool onSurfaceUsed;
        private Rigidbody rigidbody;
        private Player grenadePlayer;
        private Action<Player> onEnter;
        private Vector3 positionDiff;

        private void Awake()
        {
            this.rigidbody = this.GetComponent<Rigidbody>();

            this.onEnter = (player) =>
            {
                this.grenadePlayer = player;
                this.onPlayerUsed = true;
                this.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                this.positionDiff = this.grenadePlayer.Position - this.transform.position;
            };
            Handlers.StickyGrenadeHandler.Instance.CallDelayed(0.2f, () => Mistaken.API.Components.InRange.Spawn(this.transform, Vector3.zero, new Vector3(0.5f, 0.5f, 0.5f), this.onEnter));
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
            if (this.onPlayerUsed)
                this.transform.position = this.grenadePlayer.Position + this.positionDiff;
        }
    }
}
