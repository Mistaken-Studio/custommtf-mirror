﻿// -----------------------------------------------------------------------
// <copyright file="Translation.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Interfaces;

namespace Mistaken.CustomMTF
{
    internal class Translation : ITranslation
    {
        public string GuardCommander { get; set; } = "Guard Commander";

        public string MtfMedic { get; set; } = "MTF Medic";

        public string MtfContainmentEnginner { get; set; } = "MTF Containment Enginner";

        public string MtfExplosivesSpecialist { get; set; } = "MTF Explosives Specialist";

        public string GuardCommanderDescription { get; set; } = "Twoim zadaniem jest <color=yellow>dowodzenie</color> <color=#7795a9>ochroną placówki</color>.<br>Twoja karta <color=yellow>pozwala</color> ci otworzyć Gate A i Gate B, ale tylko gdy:<br>- Obok jest <color=#f1e96e>Naukowiec</color><br>- Obok jest skuta <color=#ff8400>Klasa D</color><br>- Obok jest skuty <color=#1d6f00>Rebeliant Chaosu</color>";

        public string MtfMedicDescription { get; set; } = "MTF Medic";

        public string MtfContainmentEnginnerDescription { get; set; } = "MTF Containment Enginner";

        public string MtfExplosivesSpecialistDescription { get; set; } = "MTF Explosives Specialist";

        public string GuardCommanderAccess { get; set; } = "Dostałeś <color=yellow>informację</color> przez pager: Aktywowano protokuł <color=yellow>GB-12</color>, od teraz jesteś <color=yellow>autoryzowany</color> do otwierania Gatów bez kogoś obok oraz do otwierania <color=yellow>generatorów</color>.";

        public string GuardCommanderEscort { get; set; } = "Dostałeś <color=yellow>informację</color> przez pager: W związu z <color=yellow>eskortą personelu</color>, od teraz jesteś <color=yellow>autoryzowany</color> do otwierania Gatów bez kogoś obok oraz do otwierania <color=yellow>generatorów</color>.";
    }
}
