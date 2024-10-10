using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
	private CharacterController characterController;
    private InputAction moveAction;
    private InputAction lookAction;

	private float horizontalAngle;
    private float verticalAngle = 0f;
	private float verticalSpeed = 0f;

    private bool isGrounded;
    private float groundedTimer;

	[SerializeField]
	private float WalkingSpeed = 7f;
	[SerializeField]
	private float mouseSens = 0.2f;
	[SerializeField]
	private float gravity = 10f;
	[SerializeField]
	private float terminalSpeed = 20f;
	[SerializeField]
	private float jumpSpeed = 10f;
	[SerializeField]
	private Transform cameraTransform;

	void Start()
    {
        // ���콺 Ŀ�� ����
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

		// ���� Action �ҷ�����
		InputActionAsset inputActions = GetComponent<PlayerInput>().actions;
        moveAction = inputActions.FindAction("Move");
        lookAction = inputActions.FindAction("Look");

        // ĳ���� ��Ʈ�ѷ� ã��
        characterController = GetComponent<CharacterController>();

        horizontalAngle = transform.localEulerAngles.y;
    }

    private void UpdateMoving()
    {
		// Move �����̵� (x�� y)
		Vector2 moveVector = moveAction.ReadValue<Vector2>();

		// ���� �����϶��� xz������� �����δ�. ���� ���� x�� z�� �ٲ��־����
		Vector3 move = new Vector3(moveVector.x, 0f, moveVector.y);

		// �̵� ���Ͱ� 1���� ũ�� 1�� ����ȭ
		if (move.magnitude > 1)
        {
            move.Normalize();
        }
        move = Time.deltaTime * WalkingSpeed * move;
        move = transform.TransformDirection(move);
        characterController.Move(move);
    }

    private void UpdateLooking()
    {
        // �¿� ȸ�� ���� (�÷��̾� ����)
        Vector2 look = lookAction.ReadValue<Vector2>();
        float turnPlayer = look.x * mouseSens;
        horizontalAngle += turnPlayer;

        if (horizontalAngle < 0f) { horizontalAngle += 360f; }
        if (horizontalAngle >= 360f) { horizontalAngle -= 360f; }

        Vector3 currentAngle = transform.localEulerAngles;
        currentAngle.y = horizontalAngle;
        transform.localEulerAngles = currentAngle;
        
        // ���� ȸ�� ���� (ī�޶� ����)
        float turnCamera = look.y * mouseSens;
        verticalAngle -= turnCamera;
        verticalAngle = Mathf.Clamp(verticalAngle, -89f, 89f);

        currentAngle = cameraTransform.localEulerAngles;
        currentAngle.x = verticalAngle;
        cameraTransform.localEulerAngles = currentAngle; ;
    }

    private void UpdateGravity()
    {
        verticalSpeed -= gravity * 5f * Time.deltaTime;
        if (verticalSpeed < -terminalSpeed) { verticalSpeed = -terminalSpeed; }
        Vector3 verticalMove = new Vector3(0f, verticalSpeed, 0f);
        verticalMove *= Time.deltaTime;

        CollisionFlags flag = characterController.Move(verticalMove);

        if ((flag & (CollisionFlags.CollidedBelow | CollisionFlags.CollidedAbove)) != 0) 
        { 
            verticalSpeed = 0; 
        }

        if (!characterController.isGrounded)
        {
            if (isGrounded)
            {
                groundedTimer += Time.deltaTime;
                if (groundedTimer > 0.3f)
                {
                    isGrounded = false;
                }
            }
        }
        else
        {
            isGrounded = true;
            groundedTimer = 0;
        }
    }

    void Update()
    {
        UpdateMoving();
        UpdateLooking();
        UpdateGravity();
    }

    void OnJump()
    {
        if (isGrounded)
        {
            verticalSpeed = jumpSpeed;
            isGrounded = false;
        }
    }
}
