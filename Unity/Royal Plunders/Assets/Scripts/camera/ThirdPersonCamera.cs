using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour
{
    public float inputPitch; // joystick up/down
    public float inputYaw; // joystick left/right
    public bool sprinting; // X
    public bool sneaking; // B
    bool changeState = false; // X/B let go of

    public float pitchYawTolerance = 0.4f; // amount of left/right that is tolerable to apply up/down
    public float defaultPitch = -20; // default pitch
    public float pitchRangeUp = -10; // max pitch
    public float pitchRangeDown = -30; // min pitch
    public float autoPitchPercent = 0.85f; // amount of min/max to apply when the camera takes over the pitch

    public float camHeightPercent = 1.35f; // camera height relative to the player model
    public float camDistance = 3; // distance from the player

    public float turnSpeed = 2.5f; // turn rate of the camera (via joystick)
    float pitch; // current pitch
    float yaw; // current yaw

    public float rotationLerp = 10; // rotation rate of the camera

    GameObject target; // target object
    Transform targetTransform; // target object transform (its faster)

    Transform self; // camera transform (its faster)

    void Start()
    {
        self = transform; // cache the transform
    }
	
    // update after the player does, stops the jitterbug
	void LateUpdate()
    {
        if (!target) // why bother updating?
            return;

        if (!sprinting && !sneaking) // if walking like a normal person
        {
            yaw += inputYaw * turnSpeed; // rotate the yaw

            if (changeState) // if the player just stopped sprinting/sneaking
            {
                pitch = defaultPitch; // fix the pitch
                changeState = false; // default state
            }
            else // allow the player to change the pitch
            {
                pitch -= inputYaw < pitchYawTolerance ? inputPitch * turnSpeed : 0; // cull bad pitching on the joystick
                pitch = Mathf.Clamp(pitch, pitchRangeDown, pitchRangeUp); // keep it in range
            }
        }
        else
        {
            changeState = true; // the player is sprinting/sneaking, he aint normal!

            yaw = Quaternion.LookRotation(targetTransform.position - self.position).eulerAngles.y; // look at the player

            if (sprinting) // pitch up
                pitch = defaultPitch + (pitchRangeUp - defaultPitch) * autoPitchPercent;
            else if (sneaking) // pitch down
                pitch = defaultPitch - (defaultPitch - pitchRangeDown) * autoPitchPercent;
        }

        self.position = getPos(); // move the cam
        self.rotation = Quaternion.Lerp(self.rotation, getRot(), Time.smoothDeltaTime * rotationLerp); // rotate the cam
    }

    // tell the camera to follow something
    public void setTarget(GameObject targetObj)
    {
        target = targetObj;
        targetTransform = targetObj.GetComponent<Transform>();

        // default
        pitch = defaultPitch;
        yaw = 0;

        if (!self) // stupid out of order Start thing
            self = transform;

        // move to starting position
        self.position = getPos();
        self.rotation = getRot();
    }

    // camera pos get
    Vector3 getPos()
    {
        Vector3 pos = targetTransform.position + Vector3.up * (camHeightPercent * targetTransform.localScale.y * 2); // above player
        float angle = Mathf.Deg2Rad * (-yaw - 90); // direction to go
        pos += new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * camDistance; // done!

        return pos;
    }

    // camera rot get
    Quaternion getRot()
    {
        Quaternion ang = Quaternion.LookRotation(Vector3.forward, Vector3.up); // nuetral
        ang = Quaternion.AngleAxis(yaw, Vector3.up) * ang; // yaw
        ang = Quaternion.AngleAxis(-pitch, ang * Vector3.right) * ang; // pitch

        return ang;
    }
}
