// -----------------------------------------------------------------------
// <copyright file="StickyComponent2.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Mistaken.API;
using UnityEngine;

namespace Mistaken.CustomMTF.Components
{
    /// <summary>
    /// Handles collision with player and updates position.
    /// </summary>
    public class StickyComponent2 : MonoBehaviour
    {
        private bool used;
        private Player player;
        private Vector3 hitposition;

        private void OnTriggerEnter(Collider other)
        {
            if (this.used) return;
            foreach (Player player in RealPlayers.List)
                player.SendConsoleMessage($"collider name: {other?.name}; grenade? {this?.name}", "blue");
            this.player = Player.Get(other?.gameObject);
            if (this.player != null)
            {
                if (this.TryGetComponent<Rigidbody>(out Rigidbody component))
                    component.constraints = RigidbodyConstraints.FreezeAll;
                this.hitposition = this.player.Position - this.transform.position;
                this.player.SendConsoleMessage("Grenade collided with you", "blue");
            }
        }

        private void FixedUpdate()
        {
            if (this.player == null) return;
            this.transform.position = this.hitposition + this.player.Position;
        }
    }
}
