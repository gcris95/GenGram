using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    public bool inViewer = false;

    Vector3 origin;
    Vector3 difference;
    public Camera myCamera;
    bool isDragging;
    

    public void onDrag(InputAction.CallbackContext ctx)
    {
        if(ctx.started)        
            origin = GetMousePosition();
        isDragging = ctx.started || ctx.performed;
    }

    private void LateUpdate()
    {
        if (!isDragging || !inViewer)
            return;

        difference = GetMousePosition() - transform.position;
        transform.position = origin - difference;
    }


    private Vector3 GetMousePosition()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = -187;

        return myCamera.ScreenToWorldPoint(mousePos);

    }

}
