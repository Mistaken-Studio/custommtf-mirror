// -----------------------------------------------------------------------
// <copyright file="Translation.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Interfaces;

namespace Mistaken.CustomMTF
{
    internal sealed class Translation : ITranslation
    {
        public string GuardCommander { get; set; } = "Dowódca Ochrony";

        public string MtfMedic { get; set; } = "Medyk NTF";

        public string MtfContainmentEnginner { get; set; } = "Technik Zabezpieczeń NTF";

        public string MtfExplosivesSpecialist { get; set; } = "Specjalista od Ładunków Wybuchowych NTF";

        public string MTFTauSoldier { get; set; } = "Żołnierz Samsara";

        public string GuardCommanderDescription { get; set; } = "Twoim zadaniem jest <color=yellow>dowodzenie</color> <color=#7795a9>ochroną placówki</color>.<br>Twoja karta <color=yellow>pozwala</color> ci otworzyć Gate A i Gate B, ale tylko gdy:<br>- Obok jest <color=#f1e96e>Naukowiec</color><br>- Obok jest skuta <color=#ff8400>Klasa D</color><br>- Obok jest skuty <color=#1d6f00>Rebeliant Chaosu</color>";

        public string MtfMedicDescription { get; set; } = "Medyk NTF";

        public string MtfContainmentEnginnerDescription { get; set; } = "Technik Zabezpieczeń NTF";

        public string MtfExplosivesSpecialistDescription { get; set; } = "Specjalista od Ładunków Wybuchowych NTF";

        public string MTFTauSoldierDescription { get; set; } = "Twoje zadanie: <color=red>Zneutralizować wszystko poza personelem fundacji</color><br><b>Karta O5 jest wbudowana w twoją rękę</b>, więc <color=yellow>możesz otwierać <b>wszystkie</b> drzwi nie używając karty</color>";

        public string GuardCommanderAccess { get; set; } = "Dostałeś <color=yellow>informację</color> przez pager: Aktywowano protokuł <color=yellow>GB-12</color>, od teraz jesteś <color=yellow>autoryzowany</color> do otwierania Gatów bez kogoś obok oraz do otwierania <color=yellow>generatorów</color>.";

        public string GuardCommanderEscort { get; set; } = "Dostałeś <color=yellow>informację</color> przez pager: W związu z <color=yellow>eskortą personelu</color>, od teraz jesteś <color=yellow>autoryzowany</color> do otwierania Gatów bez kogoś obok oraz do otwierania <color=yellow>generatorów</color>.";
    }
}
