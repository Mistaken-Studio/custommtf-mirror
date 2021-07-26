using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs;
using Grenades;
using MEC;
using Mistaken.API;
using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;
using Mistaken.CustomItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Mistaken.CustomMTF.Items
{
    /// <summary>
    /// Grenade that attaches to surfaces/players.
    /// </summary>
    public class StickyGrenadeHandler : Module
    {
        internal static HashSet<GameObject> grenades = new HashSet<GameObject>();
        internal static readonly float Damage_multiplayer = 0.08f;
        /// <inheritdoc/>
        public StickyGrenadeHandler(IPlugin<IConfig> plugin) : base(plugin)
        {
            Instance = this;
            new StickyGrenadeItem();
        }
        /// <inheritdoc/>
        public override string Name => "StickyGrenadeHandler";
        /// <inheritdoc/>
        public override void OnEnable()
        {
            Exiled.Events.Handlers.Map.ExplodingGrenade += this.Handle<ExplodingGrenadeEventArgs>((ev) => Map_ExplodingGrenade(ev));
            Exiled.Events.Handlers.Server.RoundStarted += this.Handle(() => Server_RoundStarted(), "RoundStart");
            Exiled.Events.Handlers.Map.ChangingIntoGrenade += this.Handle<ChangingIntoGrenadeEventArgs>((ev) => Map_ChangingIntoGrenade(ev));
        }
        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Map.ExplodingGrenade -= this.Handle<ExplodingGrenadeEventArgs>((ev) => Map_ExplodingGrenade(ev));
            Exiled.Events.Handlers.Server.RoundStarted -= this.Handle(() => Server_RoundStarted(), "RoundStart");
            Exiled.Events.Handlers.Map.ChangingIntoGrenade += this.Handle<ChangingIntoGrenadeEventArgs>((ev) => Map_ChangingIntoGrenade(ev));
        }
        /// <inheritdoc/>
        public static IEnumerator<float> UpdatePosition(Player player, GameObject grenade)
        {
            while (grenade != null)
            {
                yield return Timing.WaitForSeconds(0.2f);
                grenade.transform.position = player.Position + Vector3.up * 2;
            }
        }
        public static StickyGrenadeHandler Instance;
        /// <inheritdoc/>
        public class StickyGrenadeItem : CustomItem
        {
            /// <inheritdoc/>
            public StickyGrenadeItem() => base.Register();
            /// <inheritdoc/>
            public override string ItemName => "Sticky Grenade";
            /// <inheritdoc/>
            public override SessionVarType SessionVarType => SessionVarType.CI_STICKY_GRENADE;
            /// <inheritdoc/>
            public override ItemType Item => ItemType.GrenadeFrag;
            /// <inheritdoc/>
            public override int Durability => 002;
            /// <inheritdoc/>
            public override Vector3 Size => base.Size;
            /// <inheritdoc/>
            public override void OnStartHolding(Player player, Inventory.SyncItemInfo item)
            {
                player.SetGUI("sticky", API.GUI.PseudoGUIPosition.BOTTOM, "Trzymasz <color=yellow>Granat Samoprzylepny</color>");
            }
            /// <inheritdoc/>
            public override void OnStopHolding(Player player, Inventory.SyncItemInfo item)
            {
                player.SetGUI("sticky", API.GUI.PseudoGUIPosition.BOTTOM, null);
            }
            /// <inheritdoc/>
            public override void OnForceclass(Player player)
            {
                player.SetGUI("sticky", API.GUI.PseudoGUIPosition.BOTTOM, null);
            }
            /// <inheritdoc/>
            public override bool OnThrow(Player player, Inventory.SyncItemInfo item, bool slow)
            {
                Instance.CallDelayed(1f, () =>
                {
                    if (player.GetEffectActive<CustomPlayerEffects.Scp268>())
                        player.DisableEffect<CustomPlayerEffects.Scp268>();
                    Grenade grenade = UnityEngine.Object.Instantiate(player.GrenadeManager.availableGrenades[0].grenadeInstance).GetComponent<Grenade>();
                    grenade.InitData(player.GrenadeManager, Vector3.zero, player.CameraTransform.forward, slow ? 0.5f : 1f);
                    grenades.Add(grenade.gameObject);
                    Mirror.NetworkServer.Spawn(grenade.gameObject);
                    grenade.GetComponent<Rigidbody>().AddForce(new Vector3(grenade.NetworkserverVelocities.linear.x * 1.5f, grenade.NetworkserverVelocities.linear.y / 2f, grenade.NetworkserverVelocities.linear.z * 1.5f), ForceMode.VelocityChange);
                    player.RemoveItem(item);
                    grenade.gameObject.AddComponent<StickyComponent>();
                    OnStopHolding(player, item);
                }, "OnThrow");
                return false;
            }
        }
        private void Server_RoundStarted()
        {
            grenades.Clear();
        }
        private GrenadeManager lastThrower;
        private void Map_ExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            if (!grenades.Contains(ev.Grenade)) return;
            var tmp = (ev.Grenade.GetComponent<FragGrenade>()).thrower;
            lastThrower = tmp;
            this.CallDelayed(1, () =>
            {
                if (lastThrower == tmp)
                    lastThrower = null;
            }, "MapExploadingGrenade");
            foreach (Player player in ev.TargetToDamages.Keys.ToArray())
            {
                ev.TargetToDamages[player] *= Damage_multiplayer;
            }
        }
        private void Map_ChangingIntoGrenade(ChangingIntoGrenadeEventArgs ev)
        {
            if (ev.Pickup.durability != 2000f) return;
            ev.IsAllowed = false;
            Grenade grenade = UnityEngine.Object.Instantiate(Server.Host.GrenadeManager.availableGrenades[0].grenadeInstance).GetComponent<Grenade>();
            grenades.Add(grenade.gameObject);
            grenade.InitData(lastThrower ?? Server.Host.GrenadeManager, Vector3.zero, Vector3.zero, 0f);
            grenade.transform.position = ev.Pickup.position;
            Mirror.NetworkServer.Spawn(grenade.gameObject);
            ev.Pickup.Delete();
        }
    }
    /// <summary>
    /// Handles freeze on impact.
    /// </summary>
    public class StickyComponent : MonoBehaviour
    {
        private bool used;
        private void OnCollisionEnter(Collision collision)
        {
            if (!used && TryGetComponent<Rigidbody>(out Rigidbody component))
            {
                component.constraints = RigidbodyConstraints.FreezeAll;
                var player = Player.Get(collision?.gameObject);
                if (player != null)
                {
                    player.SendConsoleMessage("Grenade collided with you", "blue");
                    StickyGrenadeHandler.Instance.RunCoroutine(StickyGrenadeHandler.UpdatePosition(player, this.gameObject));
                }
            }
            used = true;
        }
    }
}
