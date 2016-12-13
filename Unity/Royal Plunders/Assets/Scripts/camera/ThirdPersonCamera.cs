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
    float wallPitch;
    float yaw; // current yaw

    public float rotationLerp = 10; // rotation rate of the camera
    public LayerMask raycastMask;
    public float clippingRadius = 0.3f;
    public float wallPitchFactor = 0.4f;

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
            pitch -= inputYaw < pitchYawTolerance ? inputPitch * turnSpeed : 0; // cull bad pitching on the joystick
            pitch = Mathf.Clamp(pitch, pitchRangeDown, pitchRangeUp); // keep it in range

            if (changeState) // if the player just stopped sprinting/sneaking
            {
                pitch = defaultPitch; // fix the pitch
                changeState = false; // default state
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

        yaw += inputYaw * turnSpeed; // rotate the yaw

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
        Vector3 posAbove = targetTransform.position + Vector3.up * (camHeightPercent * targetTransform.localScale.y * 2); // above player
        float angle = Mathf.Deg2Rad * (-yaw - 90); // direction to go
        Vector3 pos = posAbove + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * camDistance; // done! (pre-raycast)

        Vector3 rayPos = targetTransform.position + Vector3.up * (targetTransform.localScale.y * 2 + clippingRadius); // where to look for occlusion from
        RaycastHit camCast;
        Ray raycast = new Ray(rayPos, (pos - rayPos).normalized); // the occlusion ray
        if (Physics.SphereCast(raycast, clippingRadius, out camCast, (pos - rayPos).magnitude, raycastMask)) // spherecast is used for corner detection
        {
            if (Vector3.Dot(camCast.normal, Vector3.up) > 0) // hot-fix for held items blocking the camera cast.
            {
                pos = camCast.point + camCast.normal * clippingRadius; // push the camera off of the wall
                wallPitch = Vector3.Angle(pos - rayPos, pos + Vector3.up * (posAbove.y - pos.y) - rayPos) * wallPitchFactor; // look down properly
            }
        }
        else
            wallPitch = 0;

        return pos;
    }

    // camera rot get
    Quaternion getRot()
    {
        Quaternion ang = Quaternion.LookRotation(Vector3.forward, Vector3.up); // nuetral
        ang = Quaternion.AngleAxis(yaw, Vector3.up) * ang; // yaw
        ang = Quaternion.AngleAxis(-(pitch - wallPitch), ang * Vector3.right) * ang; // pitch

        return ang;
    }
}
