using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField, Min(0.01f)]
    private float _speed = 1;
    [SerializeField, Min(0.01f)]
    private float mouseSensitivity = 10f; // Sensibilité de la souris

    private bool _freeLook = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (CameraManager.Instance.cinematicStart > 0)
            return;
        // Changing Camera
        if(Input.anyKeyDown)
        {
            string input = Input.inputString.ToString();
            if(input.Length > 0 && int.TryParse(input.Last().ToString(), out int id) && CameraManager.Instance.ContainsCamera(id))
            {
                CameraManager.Instance.ChangeCamera(id);
            }
        }

        // Enable/Disable Free Look
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _freeLook = !_freeLook;
        }

        // Reset camera position
        if(Input.GetKeyDown(KeyCode.R))
            CameraManager.Instance.ResetCurrentCamera();

        // Moving Pos/Rot
        if (_freeLook)
            Look();
        Move();
    }

    Vector2 rotation = Vector2.zero;
    void Look()
    {
        rotation.y -= Input.GetAxis("Mouse X");
        rotation.x += Input.GetAxis("Mouse Y");

        // Applique la rotation de la caméra
        CameraManager.Instance.CurrentCamera().eulerAngles = rotation * mouseSensitivity;
    }

    void Move()
    {
        float movX = Input.GetAxis("Horizontal");
        float movY = Input.GetAxis("Jump");
        float movZ = Input.GetAxis("Vertical");
        CameraManager.Instance.MoveCamera(new Vector3(movX, movY, movZ) * _speed * Time.deltaTime);        
    }
}
