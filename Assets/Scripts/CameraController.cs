using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public InputAction swapCamera, goForward, goBack;
    public Camera camera;
    public Camera deskView, bookView;
    public float cameraMoveTime;
    private Camera targetCam;
    private bool moving;

    private void OnEnable()
    {
        swapCamera.performed += SwapCamera;
        swapCamera.Enable();
        goForward.performed += Book;
        goForward.Enable();
        goBack.performed += Desk;
        goBack.Enable();
    }

    private void OnDisable()
    {
        swapCamera.performed -= SwapCamera;
        swapCamera.Disable();
        goForward.performed -= Book;
        goForward.Disable();
        goBack.performed -= Desk;
        goBack.Disable();
    }

    private void SwapCamera(InputAction.CallbackContext input)
    {
        if (moving) return;
        if (targetCam == bookView) targetCam = deskView;
        else targetCam = bookView;
        StartCoroutine(MoveCamera());
    }

    private void Desk(InputAction.CallbackContext input)
    {
        if (moving) return;
        targetCam = deskView;
        StartCoroutine(MoveCamera());
    }

    private void Book(InputAction.CallbackContext input)
    {
        if (moving) return;
        targetCam = bookView;
        StartCoroutine(MoveCamera());
    }

    private IEnumerator MoveCamera()
    {
        moving = true;
        float time = 0;
        float initialFoV = camera.fieldOfView;
        Vector3 initialPosition = camera.transform.position;
        Quaternion initialRotation = camera.transform.rotation;
        while(time < cameraMoveTime)
        {
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
            camera.transform.position = Vector3.Lerp(initialPosition, targetCam.transform.position, time / cameraMoveTime);
            camera.transform.rotation = Quaternion.Slerp(initialRotation, targetCam.transform.rotation, time / cameraMoveTime);
            camera.fieldOfView = Mathf.Lerp(initialFoV, targetCam.fieldOfView, time / cameraMoveTime);
        }
        moving = false;
    }
}
