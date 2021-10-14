// -----------------------------------------------------------------------
// <copyright file="StickyComponent.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Features;
using InventorySystem.Items.ThrowableProjectiles;
using MEC;
using Mistaken.API;
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
        private Player grenadePlayer;
        private Action<Player> onEnter;
        private Vector3 positionDiff;
        private int owner;
        private bool ignoreOwner;

        private void Awake()
        {
            this.rigidbody = this.GetComponent<Rigidbody>();
            this.owner = this.GetComponent<ExplosionGrenade>().PreviousOwner.PlayerId;
            this.ignoreOwner = true;

            this.onEnter = (player) =>
            {
                if (this.ignoreOwner && player.Id == this.owner) return;
                this.grenadePlayer = player;
                this.onPlayerUsed = true;
                this.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                this.positionDiff = this.grenadePlayer.Position - this.transform.position;
            };

            Mistaken.API.Components.InRange.Spawn(this.transform, Vector3.zero, new Vector3(0.05f, 0.05f, 0.05f), this.onEnter);
            Timing.CallDelayed(0.5f, () => this.ignoreOwner = false);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!this.onSurfaceUsed && !this.onPlayerUsed)
            {
                var temp = collision.collider.transform;
                while (temp != null)
                {
                    Log.Debug(temp.name);
                    temp = temp.parent;
                }

                for (int i = 0; i < collision.contacts.Length; i++)
                    Log.Debug($"Contacts: {collision.contacts[i].point}| {collision.contacts[i].separation}");

                Log.Debug("Impulse" + collision.impulse);

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
