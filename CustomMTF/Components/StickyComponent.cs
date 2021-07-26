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

namespace Mistaken.CustomMTF.Items
{
    /// <summary>
    /// Handles freeze on impact and updates position.
    /// </summary>
    public class StickyComponent : MonoBehaviour
    {
        private bool used;
        private Player player;
        private Vector3 hitposition;

        private void OnCollisionEnter(Collision collision)
        {
            if (!this.used && this.TryGetComponent<Rigidbody>(out Rigidbody component))
            {
                component.constraints = RigidbodyConstraints.FreezeAll;
                this.player = Player.Get(collision?.gameObject);
                if (this.player != null)
                {
                    this.hitposition = this.player.Position - this.transform.position;
                    this.player.SendConsoleMessage("Grenade collided with you", "blue");
                }
            }

            this.used = true;
        }

        private void FixedUpdate()
        {
            if (this.player == null) return;
            this.transform.position = this.hitposition + this.player.Position;
        }
    }
}
