using System.ComponentModel;

namespace ShootingInteractions.Configuration.Bases
{
    public class PedestalsInteraction : LockerInteraction
    {
        [Description("Should it only work when shooting on the keypad (false = whole interactable)")]
        public bool OnlyKeypad { get; set; } = true;
    }
}
