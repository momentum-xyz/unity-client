using UnityEngine;

namespace HS
{
    [System.Serializable]
    public class ControllerSettings
    {
        public KeyCode ForwardKey = KeyCode.W;
        public KeyCode ForwardKeyAlternative = KeyCode.UpArrow;
        public KeyCode BackKey = KeyCode.S;
        public KeyCode BackKeyAlternative = KeyCode.DownArrow;
        public KeyCode LeftKey = KeyCode.A;
        public KeyCode LeftKeyAlternative = KeyCode.LeftArrow;
        public KeyCode RightKey = KeyCode.D;
        public KeyCode RightKeyAlternative = KeyCode.RightArrow;
        public KeyCode LookKey = KeyCode.Space;
        public KeyCode LookKeySecondary = KeyCode.R;
        public KeyCode MoveUpKey = KeyCode.E;
        public KeyCode MoveDownKey = KeyCode.Q;

        public float MouseSense = 1.8f;
        public float MovementSpeed = 3f;
        [Tooltip("BoostedSpeed should be bigger than MovementSpeed")]
        public float BoostedSpeed = 8f;
        public float CruiseSpeed = 32f;
        public float MaxSpeed = 65f;
        public float SpeedAccelerationFactor = 1.2f;
        public float BoostedSpeedAccelerationFactor = 1.5f;
    }

    [CreateAssetMenu(fileName = "ControllerSettings", menuName = "ScriptableObjects/Controller Settings")]
    public class ThirdPersonControllerSettings : ScriptableObject
    {
        public ControllerSettings Settings;

        private void OnValidate()
        {
            if (Settings.BoostedSpeed < Settings.MovementSpeed)
                Settings.BoostedSpeed = Settings.MovementSpeed;
        }
    }
}