// -----------------------------------------------------------------------
// <copyright file="Translation.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Interfaces;

namespace Mistaken.CustomMTF
{
    internal class Translation : ITranslation
    {
        public string MtfContainmentEnginner { get; set; } = "MTF Containment Enginner";

        public string MtfExplosivesSpecialist { get; set; } = "MTF Explosives Specialist";

        public string MtfMedic { get; set; } = "MTF Medic";

        public string MtfSergantColor { get; set; } = "#0095FF";

        public string MtfPrivateColor { get; set; } = "#70C3FF";

        public string PlayingAs { get; set; } = "You are playing as <color={0}>{1}</color>";
    }
}
