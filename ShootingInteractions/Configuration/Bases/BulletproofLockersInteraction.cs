using System.ComponentModel;

namespace ShootingInteractions.Configuration.Bases
{
    public class BulletproofLockersInteraction : LockersInteraction
    {
        [Description("Should it only work when shooting on the keypad (false = whole interactable)")]
        public bool OnlyKeypad { get; set; } = true;
    }
}
