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
    /// Handles freeze on impact.
    /// </summary>
    public class StickyComponent : MonoBehaviour
    {
        private bool used;

        private void OnCollisionEnter(Collision collision)
        {
            if (!this.used && this.TryGetComponent<Rigidbody>(out Rigidbody component))
            {
                component.constraints = RigidbodyConstraints.FreezeAll;
                var player = Player.Get(collision?.gameObject);
                if (player != null)
                {
                    player.SendConsoleMessage("Grenade collided with you", "blue");
                    StickyGrenadeHandler.Instance.RunCoroutine(StickyGrenadeHandler.UpdatePosition(player, this.gameObject));
                }
            }

            this.used = true;
        }
    }
}
