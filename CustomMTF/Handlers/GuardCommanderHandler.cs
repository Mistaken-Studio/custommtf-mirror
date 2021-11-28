// -----------------------------------------------------------------------
// <copyright file="GuardCommanderHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Interfaces;
using Mistaken.API;
using Mistaken.API.CustomItems;
using Mistaken.API.CustomRoles;
using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;
using Mistaken.API.GUI;
using UnityEngine;

namespace Mistaken.CustomMTF.Handlers
{
    /// <summary>
    /// Guard Commander handler.
    /// </summary>
    public class GuardCommanderHandler : Module
    {
        /// <summary>
        /// Gets the size of the keycard.
        /// </summary>
        public static Vector3 Size => new Vector3(1, 5, 1);

        /// <inheritdoc cref="Module.Module(IPlugin{IConfig})"/>
        public GuardCommanderHandler(IPlugin<IConfig> plugin)
            : base(plugin)
        {
            Instance = this;
            new Classes.GuardCommander().TryRegister();
            new Items.GuardCommanderKeycardItem().TryRegister();
        }

        /// <inheritdoc/>
        public override string Name => nameof(GuardCommanderHandler);

        /// <inheritdoc/>
        public override void OnEnable()
        {
            Exiled.Events.Handlers.Server.RoundStarted += this.Server_RoundStarted;
            Exiled.Events.Handlers.Player.InteractingDoor += this.Player_InteractingDoor;
            Exiled.Events.Handlers.Player.ChangingRole += this.Player_ChangingRole;
            Exiled.Events.Handlers.Player.UnlockingGenerator += this.Player_UnlockingGenerator;
            Exiled.Events.Handlers.Scp914.UpgradingPlayer += this.Scp914_UpgradingPlayer;
            Exiled.Events.Handlers.Map.Decontaminating += this.Map_Decontaminating;
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= this.Server_RoundStarted;
            Exiled.Events.Handlers.Player.InteractingDoor -= this.Player_InteractingDoor;
            Exiled.Events.Handlers.Player.ChangingRole -= this.Player_ChangingRole;
            Exiled.Events.Handlers.Player.UnlockingGenerator -= this.Player_UnlockingGenerator;
            Exiled.Events.Handlers.Scp914.UpgradingPlayer -= this.Scp914_UpgradingPlayer;
            Exiled.Events.Handlers.Map.Decontaminating -= this.Map_Decontaminating;
        }

        internal static GuardCommanderHandler Instance { get; private set; }

        private bool hasCommanderEscorted = false;
        private bool isCommanderNow;

        private void Map_Decontaminating(Exiled.Events.EventArgs.DecontaminatingEventArgs ev)
        {
            this.isCommanderNow = true;
        }

        private void Player_ChangingRole(Exiled.Events.EventArgs.ChangingRoleEventArgs ev)
        {
            if (ev.Reason != SpawnReason.Escaped)
                return;
            if ((byte)ev.NewRole != 4 || (byte)ev.NewRole < 11 || (byte)ev.NewRole > 13)
                return;
            if (!this.hasCommanderEscorted)
            {
                foreach (var item in Classes.GuardCommander.Instance.TrackedPlayers)
                    item.SetGUI("GuardCommander_Escort", PseudoGUIPosition.TOP, "Dostałeś <color=yellow>informację</color> przez pager: W związu z <color=yellow>eskortą personelu</color>, od teraz jesteś <color=yellow>autoryzowany</color> do otwierania Gatów bez kogoś obok oraz do otwierania <color=yellow>generatorów</color>.", 10);
            }

            this.hasCommanderEscorted = true;
        }

        private void Player_UnlockingGenerator(Exiled.Events.EventArgs.UnlockingGeneratorEventArgs ev)
        {
            if (!Items.GuardCommanderKeycardItem.Instance.Check(ev.Player.CurrentItem))
                return;
            if (this.hasCommanderEscorted)
                return;
            if (Classes.GuardCommander.Instance.TrackedPlayers.Contains(ev.Player) || Items.GuardCommanderKeycardItem.Instance.CurrentOwner == ev.Player)
            {
                ev.IsAllowed = false;
                return;
            }
        }

        private void Scp914_UpgradingPlayer(Exiled.Events.EventArgs.UpgradingPlayerEventArgs ev)
        {
            if (ev.KnobSetting == Scp914.Scp914KnobSetting.OneToOne)
            {
                if (Items.GuardCommanderKeycardItem.Instance.Check(ev.Player.CurrentItem))
                    Items.GuardCommanderKeycardItem.Instance.CurrentOwner = ev.Player;
            }
        }

        private void Server_RoundStarted()
        {
            this.hasCommanderEscorted = false;
            this.isCommanderNow = false;
            var rid = RoundPlus.RoundId;
            this.CallDelayed(
                60 * 6,
                () =>
            {
                if (rid != RoundPlus.RoundId)
                    return;
                if (!this.hasCommanderEscorted)
                {
                    foreach (var item in Classes.GuardCommander.Instance.TrackedPlayers)
                        item.SetGUI("GuardCommander_Access", PseudoGUIPosition.TOP, "Dostałeś <color=yellow>informację</color> przez pager: Aktywowano protokuł <color=yellow>GB-12</color>, od teraz jesteś <color=yellow>autoryzowany</color> do otwierania Gatów bez kogoś obok oraz do otwierania <color=yellow>generatorów</color>.", 10);
                    this.hasCommanderEscorted = true;
                }
            }, "RoundStart1");
            this.CallDelayed(
                1.2f,
                () =>
            {
                try
                {
                    var guards = RealPlayers.Get(RoleType.FacilityGuard).ToArray();
                    if (guards.Length < 3)
                        return;
                    Classes.GuardCommander.Instance.AddRole(guards[UnityEngine.Random.Range(0, guards.Length)]);
                }
                catch (System.Exception ex)
                {
                    this.Log.Error(ex.Message);
                    this.Log.Error(ex.StackTrace);
                }
            }, "RoundStart2");
        }

        private void Player_InteractingDoor(Exiled.Events.EventArgs.InteractingDoorEventArgs ev)
        {
            if (ev.Player.CurrentItem.Type != ItemType.KeycardNTFOfficer)
                return;
            if (!Classes.GuardCommander.Instance.Check(ev.Player) && Items.GuardCommanderKeycardItem.Instance.CurrentOwner != ev.Player)
            {
                ev.IsAllowed = false;
                return;
            }

            if (ev.IsAllowed)
                return;
            var type = ev.Door.Type;
            if (type == DoorType.Intercom)
            {
                ev.IsAllowed = true;
                return;
            }
            else if (type == DoorType.NukeSurface)
            {
                foreach (var player in RealPlayers.List.Where(p => p.Id != ev.Player.Id && (p.Role != RoleType.FacilityGuard && p.Team == Team.MTF)))
                {
                    if (Vector3.Distance(player.Position, ev.Player.Position) < 10)
                    {
                        ev.IsAllowed = true;
                        return;
                    }
                }
            }
            else if (type == DoorType.Scp106Primary || type == DoorType.Scp106Secondary || type == DoorType.Scp106Bottom)
            {
                if (!this.isCommanderNow)
                    return;
                bool tmp = false;
                foreach (var player in RealPlayers.List.Where(p => p.Id != ev.Player.Id && (p.Role == RoleType.NtfCaptain || p.Role == RoleType.NtfSpecialist)))
                {
                    if (Vector3.Distance(player.Position, ev.Player.Position) < 10)
                    {
                        if (!tmp)
                            tmp = true;
                        else
                            ev.IsAllowed = true;
                    }
                }
            }
            else if (type == DoorType.GateA || type == DoorType.GateB)
            {
                if (this.hasCommanderEscorted)
                {
                    ev.IsAllowed = true;
                    return;
                }

                foreach (var player in RealPlayers.List.Where(p => (p.Id != ev.Player.Id && p.Team == Team.RSC) || ((p.Team == Team.CDP || p.Team == Team.CHI) && p.IsCuffed)))
                {
                    if (Vector3.Distance(player.Position, ev.Player.Position) < 10)
                    {
                        ev.IsAllowed = true;
                        return;
                    }
                }
            }
            else if (type == DoorType.HID || type == DoorType.Scp079First || type == DoorType.Scp079Second)
            {
                if (this.isCommanderNow)
                {
                    ev.IsAllowed = true;
                    return;
                }
            }
        }
    }
}
