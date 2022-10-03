using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    public enum InputSystem
    {
        PC,
        Mobile
    }
    [SerializeField] InputSystem inputSystem;
    public enum PlayerPhase
    {
        Idle,
        Walking,
        WalkingBack,
        Running,
    }
    public PlayerPhase playerPhase;

    [Space]
    [SerializeField] Joystick joystick;

    [Header("Components")]
    [SerializeField] Transform player;
    [SerializeField] Animator playerAnimation;

    [Space]
    [Header("Animation tags")]
    [SerializeField] string animationParameter;
    [SerializeField] string turnOnIdle;

    [Header ("User interface")]
    [SerializeField] TMP_Text T_speed;
    [SerializeField] TMP_Text T_phase;

    [Header("Player stats")]
    [SerializeField] float animationSpeed;
    [SerializeField] float playerDesiredSpeed;

    [Space]
    [SerializeField] float walkingSpeed;
    [SerializeField] float runningSpeed = 8;
    [Range (0, 10)]
    [SerializeField] float acceleration;
    [SerializeField] float rotationValue;

    [Header("Mouse input")]
    [SerializeField] float mouseInput;
    [SerializeField] float touchX
;

    float lerpRotationInput;
    int direction;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        if (inputSystem == InputSystem.PC)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // Start is called before the first frame update
    private void Update()
    {
        ChangePlayerDirection();

        animationSpeed = Mathf.MoveTowards(animationSpeed, playerDesiredSpeed, acceleration * Time.deltaTime);
        ChangeAnimation();

        // update user interface
        T_speed.text = ($"SPEED: {animationSpeed.ToString("0.00")}");
        T_phase.text = ($"PHASE: {playerPhase}");
    }

    private void FixedUpdate() => PlayerTurn();

    void ChangeAnimation()
    {
        playerAnimation.SetFloat(animationParameter, animationSpeed);
    }

    void ChangePlayerDirection()
    {
        if (inputSystem == InputSystem.PC)
        {
            direction = (int)Input.GetAxisRaw("Vertical");
            if (direction == 1)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    playerPhase = PlayerPhase.Running;
                    playerDesiredSpeed = runningSpeed;
                }
                else
                {
                    playerPhase = PlayerPhase.Walking;
                    playerDesiredSpeed = walkingSpeed;
                }
            }

            else if (direction == -1)
            {
                playerPhase = PlayerPhase.WalkingBack;
                playerDesiredSpeed = -walkingSpeed;
            }

            else if (direction == 0)
            {
                playerPhase = PlayerPhase.Idle;
                playerDesiredSpeed = 0;
            }
        }

        else if (inputSystem == InputSystem.Mobile)
        {
            if (joystick.Vertical > 0 && joystick.Vertical < 1)
            {
                playerPhase = PlayerPhase.Walking;
                playerDesiredSpeed = walkingSpeed;
            }
            else if (joystick.Vertical == 1)
            {
                playerPhase = PlayerPhase.Running;
                playerDesiredSpeed = runningSpeed;
            }
            else if (joystick.Vertical < 0)
            {
                playerPhase = PlayerPhase.WalkingBack;
                playerDesiredSpeed = -walkingSpeed;
            }
            else if (joystick.Vertical == 0)
            {
                playerPhase = PlayerPhase.Idle;
                playerDesiredSpeed = 0;
            }
        }
    }

    void PlayerTurn()
    {
        if (inputSystem == InputSystem.PC)
        {
            mouseInput = Input.GetAxis("Mouse X");
        }

        else if (inputSystem == InputSystem.Mobile)
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                if (touch.position.x > Screen.width / 2)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Moved)
                    {
                        // Get movement of the finger since last frame
                        Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
                        mouseInput = touchDeltaPosition.x;
                    }
                    else if (Input.GetTouch(0).phase == TouchPhase.Ended)
                    {
                        return;
                    }
                }
            }
        }

        mouseInput = Mathf.Clamp(mouseInput, -1, 1);
        lerpRotationInput = Mathf.MoveTowards(lerpRotationInput, mouseInput, 2 * Time.fixedDeltaTime);
        player.Rotate(0, lerpRotationInput * rotationValue * Time.fixedDeltaTime, 0);
        if (mouseInput == 0)
        {
            playerAnimation.SetBool(turnOnIdle, false);
            return;
        }

        if (playerPhase == PlayerPhase.Idle)
        {
            playerAnimation.SetBool(turnOnIdle, true);
        }
        else
        {
            playerAnimation.SetBool(turnOnIdle, false);
        }
    }
}
