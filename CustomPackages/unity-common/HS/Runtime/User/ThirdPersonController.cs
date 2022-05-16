//===========================================================================//
//                       FreeFlyCamera (Version 1.2)                         //
//                        (c) 2019 Sergey Stafeyev                           //
//===========================================================================//
// adapted by naam 2020

using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HS
{
    public interface IThirdPersonController
    {
        public bool IsControlling { get; set; }
        public bool IsPaused { get; set; }
    }

    public class ThirdPersonController : MonoBehaviour, IThirdPersonController
    {
        public ControllerSettings Settings { get; set; }

        #region UI

        [Header("Cinematic parameters")]
        public bool CinematicMode;
        [SerializeField] float _lookForce = 8;
        [SerializeField] float _motionForce = 4;

        [Space]

        [SerializeField] Transform AvatarSlot;

        [SerializeField]
        [Tooltip("The script is currently active")]
        private bool _active = true;

        [Space]

        [SerializeField] float _spawnRandomness = 30;

        [Space]

        [SerializeField]
        [Tooltip("Camera rotation by mouse movement is active")]
        private bool _enableRotation = true;
        [SerializeField] bool _inParentSpace;
        [SerializeField] [Range(0, 89)] float _pitchLimit = 60;
        [SerializeField] bool _onlyRotateWithKeyhold = true;

        [SerializeField]
        [Tooltip("Sensitivity of gamepad rotation")]
        private float _gamepadSense = 5f;

        [Space]

        [SerializeField]
        [Tooltip("Camera movement by 'W','A','S','D','Q','E' keys is active")]
        private bool _enableMovement = true;

        [SerializeField] private float _cruiseSpeed = 32f;
        [SerializeField] private float _maxSpeed = 65f;

        [SerializeField]
        [Tooltip("Move up")]
        private KeyCode _moveUp = KeyCode.E;

        [SerializeField]
        [Tooltip("Move down")]
        private KeyCode _moveDown = KeyCode.Q;

        [Space]

        [SerializeField]
        [Tooltip("Acceleration at camera movement is active")]
        private bool _enableSpeedAcceleration = true;

        [SerializeField]
        [Tooltip("Rate which is applied during camera movement")]
        private float _speedAccelerationFactor = 1.5f;
        [SerializeField] private float _boostedSpeedAccelerationFactor = 1.5f;

        [Space]

        [SerializeField]
        [Tooltip("This keypress will move the camera to initialization position")]
        //private KeyCode _initPositonButton = KeyCode.R;

        #endregion UI

        // Cinematic parameters
        DampedFloat _mouseInX = new DampedFloat(0);
        DampedFloat _mouseInY = new DampedFloat(0);
        DampedVector3 _motionIn = new DampedVector3();


        public System.Func<Vector3, Vector3, Vector3> OnConstraint;
        public bool IsControlling { get { return _isControlling; } set { _isControlling = value; } }

        bool _isControlling;
        public static ThirdPersonController Instance;

        // READOUTS
        /// <summary> Velocity based on user control </summary>
        public Vector3 Velo { get; private set; }
        /// <summary> Speed based on user control </summary>
        public float Speed { get; private set; }
        Vector3 _lastPos;
        /// <summary> Velocity regardless of user control </summary>
        public Vector3 AbsVelo { get; private set; }
        /// <summary> Speed regardless of user control </summary>
        public float AbsSpeed { get; private set; }

        /// <summary> Pause the controller </summary>
        public bool IsPaused { get; set; }

        /// <summary> Slots in a new avatar GO. There's a default avatar in here that will
        /// probably not have the right name/role/colors, this function replaces that one. 
        /// Feed it null to destroy (!) the current avatar and replace it with the default
        /// one.</summary>
        public void SetAvatar(AvatarDriver avatar)
        {
            if (avatar == null)
            {
                if (_currentAvatar != null && _currentAvatar != _defaultAvatar)
                    Destroy(_currentAvatar.gameObject);
                _currentAvatar = _defaultAvatar;
                _defaultAvatar.gameObject.SetActive(true);
            }
            else
            {
                _defaultAvatar.gameObject.SetActive(false);
                _currentAvatar = avatar;
                _currentAvatar.transform.SetParent(AvatarSlot, true);
                _currentAvatar.transform.localPosition = Vector3.zero;
            }
        }
        public AvatarDriver CurrentAvatar => _currentAvatar;

        private AvatarDriver _defaultAvatar;
        private AvatarDriver _currentAvatar;



        private CursorLockMode _wantedMode;

        private float _currentIncrease = 1;
        private float _currentIncreaseMem = 0;
        private Vector3 _deltaPosMem = Vector3.zero;

        private Vector3 _initPosition;
        private Vector3 _initRotation;

        private void Awake()
        {
            // offset initial spawn randomly
            transform.position += //new Vector3(Random.Range(-30f, 30f), Random.Range(-30f, 30f), Random.Range(-30f, 30f));
                Random.insideUnitSphere * _spawnRandomness;

            _defaultAvatar = AvatarSlot.GetComponentInChildren<AvatarDriver>();
            _currentAvatar = _defaultAvatar;

            Settings = new ControllerSettings();
        }


        private void Start()
        {
            _initPosition = transform.position;
            _initRotation = transform.eulerAngles;
        }

        private void OnEnable()
        {
            Instance = this;
            _lastPos = transform.position;
            if (_active)
                _wantedMode = _onlyRotateWithKeyhold ? CursorLockMode.None : CursorLockMode.Locked;
        }

        private void OnDisable()
        {
            if (Instance == this) Instance = null;
        }


        // Apply requested cursor state
        // TODO: why have this be a separate function now we removed most of the stuff?
        private void SetCursorState(bool doLock = false)
        {
            if (doLock)
                _wantedMode = CursorLockMode.Locked;
            else
                _wantedMode = CursorLockMode.None;

            // Apply cursor state
            Cursor.lockState = _wantedMode;
            // Hide cursor when locking
            Cursor.visible = (CursorLockMode.Locked != _wantedMode);
        }

        private void CalculateCurrentIncrease(bool moving, float delta, bool boost)
        {
            _currentIncrease = delta;

            if (!_enableSpeedAcceleration)
            {
                _currentIncreaseMem = 0;
                return;
            }
            else if (!moving)
            {
                _currentIncreaseMem = Mathf.Lerp(_currentIncreaseMem, 0, delta * 20); // MAGIC
                _currentIncrease = delta + Mathf.Pow(_currentIncreaseMem, 3) * delta;
            }
            else
            {
                _currentIncreaseMem += delta * ((boost ? _boostedSpeedAccelerationFactor : _speedAccelerationFactor) - 1);
                _currentIncrease = delta + Mathf.Pow(_currentIncreaseMem, 3) * delta;
            }

        }

        private bool IsNaNf(Vector3 q)
        {
            return float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z);
        }

        private void Update()
        {
            const float DEADZONE = .1f;

            UpdateSpeed();

            if (!_active)
                return;

            bool motionInput = false;
            float delta = Mathf.Clamp(Time.deltaTime, 0.0001f, 1f);
            bool boost = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.JoystickButton0);

            // Movement
            if (_enableMovement)
            {
                Vector3 deltaPosition = Vector3.zero;
                float currentSpeed = Settings.MovementSpeed;

                if (!IsPaused)
                {
                    if (boost)
                        currentSpeed = Settings.BoostedSpeed;

                    float vertAxis = Input.GetAxis("Vertical");
                    if (Input.GetKey(Settings.ForwardKey) || Input.GetKey(Settings.ForwardKeyAlternative) || vertAxis > DEADZONE)
                    {
                        motionInput = true;
                        deltaPosition += transform.forward;
                    }
                    if (Input.GetKey(Settings.BackKey) || Input.GetKey(Settings.BackKeyAlternative) || vertAxis < -DEADZONE)
                    {
                        motionInput = true;
                        deltaPosition -= transform.forward;
                    }

                    float horzAxis = Input.GetAxis("Horizontal");
                    if (Input.GetKey(Settings.LeftKey) || Input.GetKey(Settings.LeftKeyAlternative) || horzAxis < -DEADZONE)
                    {
                        motionInput = true;
                        deltaPosition -= transform.right;
                    }
                    if (Input.GetKey(Settings.RightKey) || Input.GetKey(Settings.RightKeyAlternative) || horzAxis > DEADZONE)
                    {
                        motionInput = true;
                        deltaPosition += transform.right;
                    }

                    float gamepadGoUp = Input.GetAxis("GamepadGoUp");
                    float gamepadGoDown = Input.GetAxis("GamepadGoDown");
                    if (Input.GetKey(_moveUp) || gamepadGoUp > DEADZONE)
                    {
                        motionInput = true;
                        deltaPosition += transform.up;
                    }
                    if (Input.GetKey(_moveDown) || gamepadGoDown > DEADZONE)
                    {
                        motionInput = true;
                        deltaPosition -= transform.up;
                    }
                }

                // Calc acceleration
                CalculateCurrentIncrease(deltaPosition != Vector3.zero, delta, boost);

                _deltaPosMem = Vector3.Lerp(_deltaPosMem, deltaPosition, delta * 4); // MAGIC
                if (deltaPosition.sqrMagnitude == 0) deltaPosition = _deltaPosMem;

                Velo = (deltaPosition * currentSpeed * _currentIncrease) / delta;
                if (CinematicMode)
                {
                    _motionIn.Update(Velo, Time.deltaTime, _motionForce * (motionInput ? 1 : 3));
                    Velo = _motionIn.Value;
                }
                Velo = Vector3.ClampMagnitude(Velo, (boost ? _maxSpeed : _cruiseSpeed));
                Speed = Mathf.Clamp(Velo.magnitude, 0, 1000);

                Vector3 newPos = transform.position + Velo * Time.deltaTime;
                if (OnConstraint != null)
                    // WARNING: this will only work reliably with a single subscription!
                    newPos = OnConstraint.Invoke(transform.position, Velo * Time.deltaTime);

                transform.position = newPos;
                // transform.position += Velo * Time.deltaTime;
            }


            float gamepadLookY = Input.GetAxis("GamepadLookY");
            float gamepadLookX = Input.GetAxis("GamepadLookX");
            bool gamepadLook = Mathf.Abs(gamepadLookY) > DEADZONE || Mathf.Abs(gamepadLookX) > DEADZONE;
            // Looking around
            bool look = Input.GetKey(Settings.LookKey) || Input.GetKey(Settings.LookKeySecondary) || gamepadLook;
            look &= !IsPaused;

            _isControlling = motionInput || look;
            SetCursorState(motionInput || look);
            if (Cursor.visible && !motionInput)
                return;

            // Rotation
            if (_enableRotation)
            {
                // need to redo this to limit up/down
                var flatForw = transform.forward;
                if (_inParentSpace) flatForw = transform.parent.InverseTransformDirection(flatForw);
                flatForw.y = 0;
                flatForw.Normalize();
                var curHeading = -
                    Vector3.SignedAngle(
                        flatForw,
                        Vector3.forward,
                        Vector3.up
                    );
                var curPitch = -
                    Vector3.SignedAngle(
                        transform.forward,
                        flatForw,
                        transform.right
                    );
                if (_inParentSpace)
                    curPitch = -
                        Vector3.SignedAngle(
                            transform.parent.InverseTransformDirection(transform.forward),
                            flatForw,
                            transform.parent.InverseTransformDirection(transform.right)
                        );

                var lookX = Input.GetAxis("Mouse X");
                var lookY = Input.GetAxis("Mouse Y");
                if (CinematicMode)
                {
                    _mouseInX.Update(lookX, Time.deltaTime, _lookForce);
                    _mouseInY.Update(lookY, Time.deltaTime, _lookForce);
                    lookX = _mouseInX.Value;
                    lookY = _mouseInY.Value;
                }
                var pitchDeltaAngle = -lookY * Settings.MouseSense;
                var headDeltaAngle = lookX * Settings.MouseSense;

                if (gamepadLook)
                {
                    pitchDeltaAngle = gamepadLookY * _gamepadSense;
                    headDeltaAngle = gamepadLookX * _gamepadSense;
                }

                curHeading += headDeltaAngle;
                curPitch += pitchDeltaAngle;
                curPitch = Mathf.Clamp(curPitch, -_pitchLimit, _pitchLimit);

                if (_inParentSpace)
                    transform.localRotation =
                        Quaternion.AngleAxis(
                            curHeading,
                            Vector3.up
                        )
                        * Quaternion.AngleAxis(
                            curPitch,
                            Vector3.right
                        )
                        ;
                else
                    transform.rotation =
                        Quaternion.AngleAxis(
                            curHeading,
                            Vector3.up
                        )
                        * Quaternion.AngleAxis(
                            curPitch,
                            Vector3.right
                        );
            }

            // clear at the end of the update
            motionInput = false;
        }


        void UpdateSpeed()
        {
            float delta = Time.deltaTime;
            AbsVelo = Vector3.Lerp(AbsVelo, (transform.position - _lastPos) / delta, delta * 20);
            AbsVelo = AbsVelo.ZeroNan();
            AbsVelo = Vector3.ClampMagnitude(AbsVelo, _maxSpeed);
            AbsSpeed = Mathf.Clamp(AbsVelo.magnitude, 0, 100);
            _lastPos = transform.position;
            // Velo = (deltaPosition * currentSpeed * _currentIncrease) / delta;
            // Velo = Vector3.ClampMagnitude(Velo, (boost?_maxSpeed:_cruiseSpeed) );
            // Speed = Velo.magnitude;
        }


        class DampedVector3
        {
            DampedFloat _x = new DampedFloat(0);
            DampedFloat _y = new DampedFloat(0);
            DampedFloat _z = new DampedFloat(0);

            public Vector3 Value => new Vector3(_x.Value, _y.Value, _z.Value);

            public void Update(Vector3 newValue, float delta, float force)
            {
                _x.Update(newValue.x, delta, force);
                _y.Update(newValue.y, delta, force);
                _z.Update(newValue.z, delta, force);
            }
        }


        class DampedFloat
        {
            float _value;
            float _speed;
            float _acc;

            public float Value => _value;


            public DampedFloat(float value)
            {
                _value = value;
            }


            public void Update(float newValue, float delta, float force)
            {
                _value = Mathf.Lerp(_value, newValue, delta * force);
                /*
				return;
                				var newSpeed = Mathf.Lerp( _speed, (newValue-_value)/delta *0.9f, delta*8 );
				_acc = newSpeed-_speed;
				_speed = newSpeed;
				_value += _speed*delta;
				// _speed = Mathf.Lerp( _speed, _speed/2, Time.deltaTime*2 );
				Debug.Log( _value );
                */
            }
        }
    }
}