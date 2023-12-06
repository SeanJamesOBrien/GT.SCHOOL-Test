using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float cameraSpeed;

    [SerializeField] Transform[] towerPositions;
    private void Start()
    {
        UIController.OnGradeSelected += UIController_OnGradeSelected;
    }

    private void OnDestroy()
    {
        UIController.OnGradeSelected -= UIController_OnGradeSelected;
    }

    private void UIController_OnGradeSelected(int index)
    {
        transform.position = towerPositions[index].position;
    }

    void Update()
    {
        if(Input.GetMouseButton(1))
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * cameraSpeed, 0);
        }           
    }
}