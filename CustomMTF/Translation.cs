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
        public string MTFContainmentEnginner { get; set; } = "MTF Containment Enginner";

        public string MTFExplosivesSpecialist { get; set; } = "MTF Explosives Specialist";

        public string MTFMedic { get; set; } = "MTF Medic";

        public string MTFSergantColor { get; set; } = "#0095FF";

        public string MTFPrivateColor { get; set; } = "#70C3FF";

        public string PlayingAs { get; set; } = "You're playing as <color={0}>{1}</color>";
    }
}
