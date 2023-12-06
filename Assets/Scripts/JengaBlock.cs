using System;
using UnityEngine;

public class JengaBlock : MonoBehaviour
{
    public static event Action<StudentInfo> OnRightClick = delegate { };
    public static event Action<string> OnBlockMoved = delegate { };

    Vector3 diff;
    Vector3 originalPosition;   
    float currentHeight;
    bool isMouseDown = false;
    bool hasBlockMoved = false;

    Rigidbody rb;
    StudentInfo studentInfo;

    public StudentInfo StudentInfo { get => studentInfo; set => studentInfo = value; }

    void Awake()
    {
        diff = Vector3.zero;
        rb = GetComponent<Rigidbody>(); 
        rb.isKinematic = true;
    }

    void Update()
    {
        if (isMouseDown)
        {
            transform.position = MousePosition() + diff;
            originalPosition = transform.position;
            originalPosition.y = currentHeight;
            transform.position = originalPosition;
        }
    }

    private void OnMouseDown()
    {
        hasBlockMoved = true;
        isMouseDown = true;
        diff = transform.position - MousePosition();
        currentHeight = transform.position.y;
        rb.isKinematic = true;
    }

    private void OnMouseUp()
    {
        if(studentInfo.mastery == 0)
        {
            OnBlockMoved?.Invoke(studentInfo.grade);
        }      
        rb.isKinematic = false;
        isMouseDown = false;
    }

    private Vector3 MousePosition()
    {
        Vector3 mousePosition2d = Input.mousePosition;
        mousePosition2d.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePosition2d);

    }
    void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(1))
        {
            OnRightClick?.Invoke(studentInfo);
        }
    }

    internal void EnabledRigidBody()
    {
        rb.isKinematic = false;
    }

    internal bool IsTowerReady()
    {
        if (studentInfo.mastery == 0 &&
            !hasBlockMoved)
        {
            return false;
        }
        return true;
    }
}