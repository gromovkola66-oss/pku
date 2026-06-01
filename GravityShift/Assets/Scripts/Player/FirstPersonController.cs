using UnityEngine;

/// <summary>
/// Контроллер от первого лица.
/// Управление: WASD - движение, мышь - осмотр, Space - прыжок, Shift - бег.
/// Работает с системой переключения гравитации.
/// </summary>
public class FirstPersonController : MonoBehaviour
{
    [Header("Движение")]
    [Tooltip("Скорость ходьбы")]
    [SerializeField] private float walkSpeed = 6f;

    [Tooltip("Скорость бега")]
    [SerializeField] private float sprintSpeed = 10f;

    [Tooltip("Сила прыжка")]
    [SerializeField] private float jumpForce = 7f;

    [Tooltip("Множитель гравитации")]
    [SerializeField] private float gravityMultiplier = 2f;

    [Header("Камера")]
    [Tooltip("Чувствительность мыши")]
    [SerializeField] private float mouseSensitivity = 2f;

    [Tooltip("Максимальный угол обзора вверх/вниз")]
    [SerializeField] private float maxLookAngle = 85f;

    [Header("Проверка земли")]
    [Tooltip("Расстояние рейкаста для проверки земли")]
    [SerializeField] private float groundCheckDistance = 0.3f;

    [Tooltip("Маска слоёв земли")]
    [SerializeField] private LayerMask groundMask = ~0;

    // Компоненты
    private Rigidbody rb;
    private Transform cameraTransform;
    private GravityController gravityController;

    // Состояние
    private float rotationX = 0f;
    private bool isGrounded = false;
    private Vector3 currentUp = Vector3.up;

    /// <summary>
    /// Инициализация компонентов
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cameraTransform = GetComponentInChildren<Camera>().transform;
        gravityController = GetComponent<GravityController>();

        // Блокируем и скрываем курсор
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Настраиваем Rigidbody
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    /// <summary>
    /// Обновление каждый кадр: обработка ввода камеры
    /// </summary>
    private void Update()
    {
        // Не обрабатывать ввод если игра на паузе
        if (Time.timeScale == 0f) return;

        HandleMouseLook();
        CheckGround();
    }

    /// <summary>
    /// Физическое обновление: движение и прыжок
    /// </summary>
    private void FixedUpdate()
    {
        if (Time.timeScale == 0f) return;

        HandleMovement();
    }

    /// <summary>
    /// Обработка вращения камеры мышью
    /// </summary>
    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Вертикальный поворот камеры (ограничен)
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -maxLookAngle, maxLookAngle);

        // Применяем вращение камеры (локальное)
        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // Горизонтальный поворот тела игрока (вокруг текущей оси "вверх")
        currentUp = gravityController != null ? gravityController.GetCurrentUp() : Vector3.up;
        transform.Rotate(currentUp, mouseX, Space.World);
    }

    /// <summary>
    /// Проверка, стоит ли игрок на земле (рейкаст вниз относительно текущей гравитации)
    /// </summary>
    private void CheckGround()
    {
        currentUp = gravityController != null ? gravityController.GetCurrentUp() : Vector3.up;
        Vector3 down = -currentUp;

        isGrounded = Physics.Raycast(transform.position, down, groundCheckDistance + 0.1f, groundMask);

        // Прыжок
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(currentUp * jumpForce, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// Обработка движения WASD
    /// </summary>
    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Определяем скорость (бег или ходьба)
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // Направление движения относительно поворота игрока
        Vector3 moveDirection = transform.forward * vertical + transform.right * horizontal;
        moveDirection = moveDirection.normalized * currentSpeed;

        // Сохраняем вертикальную скорость (по оси гравитации)
        currentUp = gravityController != null ? gravityController.GetCurrentUp() : Vector3.up;
        float verticalVelocity = Vector3.Dot(rb.linearVelocity, currentUp);

        // Применяем движение
        Vector3 newVelocity = moveDirection + currentUp * verticalVelocity;
        rb.linearVelocity = newVelocity;
    }

    /// <summary>
    /// Возвращает true если игрок на земле
    /// </summary>
    public bool IsGrounded()
    {
        return isGrounded;
    }

    /// <summary>
    /// Устанавливает чувствительность мыши (для настроек)
    /// </summary>
    public void SetSensitivity(float value)
    {
        mouseSensitivity = value;
    }
}
