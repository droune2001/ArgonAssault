using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    [Tooltip("In ms^-1")][SerializeField] float speed = 15f;
    [SerializeField] float xRange = 9f;
    [SerializeField] Vector2 yRange = new Vector2(-5f, 5f);

    [SerializeField] float positionPitchFactor = -6.0f;
    [SerializeField] float controlPitchFactor = -18.0f;

    [SerializeField] float positionYawFactor = 5.0f;
    [SerializeField] float controlYawFactor = 18.0f;

    [SerializeField] float controlRollFactor = -40.0f;

    private float xThrow, yThrow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ProcessTranslation();

        ProcessRotation();
    }

    void ProcessTranslation()
    {
        xThrow = CrossPlatformInputManager.GetAxis("Horizontal");
        float xOffset = xThrow * speed * Time.deltaTime;

        yThrow = CrossPlatformInputManager.GetAxis("Vertical");
        float yOffset = yThrow * speed * Time.deltaTime;

        float rawXPos = transform.localPosition.x + xOffset;
        float clampedXPos = Mathf.Clamp(rawXPos, -xRange, xRange);

        float rawYPos = transform.localPosition.y + yOffset;
        float clampedYPos = Mathf.Clamp(rawYPos, yRange.x, yRange.y);

        transform.localPosition = new Vector3(clampedXPos, clampedYPos, transform.localPosition.z);
    }

    void ProcessRotation()
    {
        float pitchDueToPosition = transform.localPosition.y * positionPitchFactor;
        float pitchDueToControlThrow = controlPitchFactor * yThrow;
        float pitch = pitchDueToPosition + pitchDueToControlThrow;

        float yawDueToPosition = transform.localPosition.x * positionYawFactor;
        float yawDueToControlThrow = controlYawFactor * xThrow;
        float yaw = yawDueToPosition + yawDueToControlThrow;

        float roll = controlRollFactor * xThrow;

        transform.localRotation = Quaternion.Euler(pitch, yaw, roll);
    }
}
