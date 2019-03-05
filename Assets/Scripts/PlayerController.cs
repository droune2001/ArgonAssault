using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour
{
    [Header("General")]
    [Tooltip("In ms^-1")][SerializeField] float controlSpeed = 15f;
    [SerializeField] float xRange = 9f;
    [SerializeField] Vector2 yRange = new Vector2(-5f, 5f);
    [SerializeField] GameObject[] guns;

    [Header("Screen-position Based")]
    [SerializeField] float positionPitchFactor = -6.0f;
    [SerializeField] float positionYawFactor = 5.0f;

    [Header("Control-throw Based")]
    [SerializeField] float controlPitchFactor = -18.0f;
    [SerializeField] float controlYawFactor = 18.0f;
    [SerializeField] float controlRollFactor = -40.0f;


    private float xThrow, yThrow;
    private bool areControlEnabled = true;

    // Update is called once per frame
    void Update()
    {
        if (areControlEnabled)
        {
            ProcessTranslation();
            ProcessRotation();
            ProcessFiring();
        }
    }

    // Called by CollisionHandler
    void OnPlayerDeath()
    {
        areControlEnabled = false;
        ActivateGuns(false);
    }

    void ProcessTranslation()
    {
        xThrow = CrossPlatformInputManager.GetAxis("Horizontal");
        float xOffset = xThrow * controlSpeed * Time.deltaTime;

        yThrow = CrossPlatformInputManager.GetAxis("Vertical");
        float yOffset = yThrow * controlSpeed * Time.deltaTime;

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

    void ProcessFiring()
    {
        if (CrossPlatformInputManager.GetButton("Fire"))
        {
            ActivateGuns(true);
        }
        else if (CrossPlatformInputManager.GetButtonUp("Fire"))
        {
            ActivateGuns(false);
        }
    }

    private void ActivateGuns(bool isActive)
    {
        foreach (var gun in guns)
        {
            if (isActive)
            {
                gun.SetActive(isActive);
            }
            var emissionModule = gun.GetComponent<ParticleSystem>().emission;
            emissionModule.enabled = isActive;
        }
    }
}
