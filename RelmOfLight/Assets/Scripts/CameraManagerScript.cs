using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class CameraManagerScript : MonoBehaviour
{
    public static CameraManagerScript Instance;
    public static CameraShakeManager Shaker;
    public static PostProcessingManager PostProlone;
    public Animator TransitionAnimController;


    public Camera Cam;

    private void Awake()
    {
        Instance = this;
        Shaker = GetComponentInChildren<CameraShakeManager>();
        PostProlone = GetComponentInChildren<PostProcessingManager>();
    }

    public void CameraJumpInOut(int jumpInOut)
    {
        if (jumpInOut != 1 && jumpInOut != 2)
        {
            Shaker.HaltShaking();
            return;
        }
        Shaker.PlayShake(jumpInOut == 1 ? "Jump_In" : "Jump_Out");
    }

    public void SetWindTransitionAnim(bool value, float rotation)
    {
        TransitionAnimController.SetBool("UIState", value);
        TransitionAnimController.transform.parent.localEulerAngles = new Vector3(0, 0, rotation); 
    }

    public void CameraFocusSequence(float duration, float endOrtho, AnimationCurve animCurveZoom, AnimationCurve animCurveMovement, Vector3 playerPos)
    {
        if (playerPos != Vector3.zero)
        {
            if (CameraMoveSequencer != null) StopCoroutine(CameraMoveSequencer);
            CameraMoveSequencer = CameraMoveSequence_Co(duration, playerPos, animCurveMovement);
            StartCoroutine(CameraMoveSequencer);
        }

        if (endOrtho > 0)
        {
            if (CameraFocusSequencer != null) StopCoroutine(CameraFocusSequencer);
            CameraFocusSequencer = CameraFocusSequence_Co(duration, endOrtho, animCurveZoom);
            StartCoroutine(CameraFocusSequencer);
        }
    }

    public IEnumerator WaitCameraFocusSequence(float duration, float endOrtho, AnimationCurve animCurveZoom, AnimationCurve animCurveMovement, Vector3 playerPos, float delay)
    {
        if (playerPos != Vector3.zero && endOrtho > 0)
        {
            if (CameraMoveSequencer != null) StopCoroutine(CameraMoveSequencer);
            CameraMoveSequencer = CameraMoveSequence_Co(duration, playerPos, animCurveMovement);
            yield return CameraMoveSequence_Co(duration, playerPos, animCurveMovement);
            if (CameraFocusSequencer != null) StopCoroutine(CameraFocusSequencer);
            CameraFocusSequencer = CameraFocusSequence_Co(duration, endOrtho, animCurveZoom);
            yield return CameraFocusSequencer;
            yield break;

        }

        if (playerPos != Vector3.zero)
        {
            if (CameraMoveSequencer != null) StopCoroutine(CameraMoveSequencer);
            CameraMoveSequencer = CameraMoveSequence_Co(duration, playerPos, animCurveMovement);
            yield return CameraMoveSequence_Co(duration, playerPos, animCurveMovement);
            yield break;
        }

        if (endOrtho > 0)
        {
            if (CameraFocusSequencer != null) StopCoroutine(CameraFocusSequencer);
            CameraFocusSequencer = CameraFocusSequence_Co(duration, endOrtho, animCurveZoom);
            yield return CameraFocusSequencer;
            yield break;
        }
    }

    float destinationOrtho = 0f;
    bool isFocusing = false;
    IEnumerator CameraFocusSequencer = null;
    public IEnumerator CameraFocusSequence_Co(float duration, float endOrtho, AnimationCurve animCurve)
    {
        destinationOrtho = endOrtho;
        isFocusing = true;
        bool hasStarted = false;
        float startingOrtho = Cam.orthographicSize;
        float progress = 0f;
        while (progress < 1 || !hasStarted)
        {
            hasStarted = true;
            progress += BattleManagerScript.Instance.FixedDeltaTime / duration;
            Cam.orthographicSize = Mathf.Lerp(startingOrtho, endOrtho, animCurve == null ? progress : animCurve.Evaluate(progress));
			yield return null;
        }
        Cam.orthographicSize = endOrtho;
        isFocusing = false;
    }

    Vector3 destinationPos = Vector3.zero;
    bool isMoving = false;
    IEnumerator CameraMoveSequencer = null;
    public IEnumerator CameraMoveSequence_Co(float duration, Vector3 playerPos, AnimationCurve animCurve)
    {
        destinationPos = playerPos;
        isMoving = true;
        bool hasStarted = false;
        Vector3 cameraStartingPosition = transform.position;
        Vector3 finalPos = new Vector3(playerPos.x, playerPos.y, cameraStartingPosition.z);
        float progress = 0f;
        while (progress < 1 || !hasStarted)
        {
            hasStarted = true;
            progress += BattleManagerScript.Instance.FixedDeltaTime / duration;
            transform.position = Vector3.Lerp(cameraStartingPosition, finalPos, animCurve == null ? progress : animCurve.Evaluate(progress));
			yield return null;
        }
        transform.position = finalPos;
        isMoving = false;
    }
  
}

