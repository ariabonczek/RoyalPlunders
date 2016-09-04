using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    // target transform, can be anything!
    public Transform targetTransform;

    // Y offset from target position
    public float camHeight = 5;
    // pitch offset from target UP to camera in degrees
    // 0 = straight down
    // 90 = straight forward (infinity distance)
    public float camOffsetTilt = 52.5f;
    // pitch delta from camera Offset in degrees
    // 0 = look straight at the target
    // + = look up
    // - = look down
    public float camLookTilt = 10f;

    // free look range of movement. in degrees
    public float freeLookRangeX = 22.5f;
    public float freeLookRangeY = 13;
    // free look X, Y
    public float freeLookX = 0;
    public float freeLookY = 0;

    // number of angle snaps on the vertical axis rotation
    public uint angleSnapCount = 4;
    // current snap point
    public uint angleSnapIndex = 0;

    // Lerp factors
    public float positionLerp = 5;
    public float rotationLerp = 10;

    // experimental feature
    public bool immersiveLook = true;

    // whatever this script is attached to
    private Transform self;

	void Start ()
    {
        // simple caching, the transform is a getter, this makes it faster to run my calculations
        self = transform;
	}
	
	void Update ()
    {
        // manage improper angle snapping values
        uint oldAngleSnapCount = angleSnapCount;
        // ensure that snapping reamins on cardinal directions
        angleSnapCount = (uint)Mathf.NextPowerOfTwo((int)angleSnapCount);
        // ensure that your snap index maps to the new cardinal points properly
        if (angleSnapCount != oldAngleSnapCount)
            angleSnapIndex = (uint)(((float)angleSnapIndex).Remap(0, oldAngleSnapCount, 0, angleSnapCount));
        // ensure that the index never leaves count bounds
        angleSnapIndex %= angleSnapCount;

        // the yaw of the currently snapped to position
        float snapHeading = Mathf.PI * 2 / angleSnapCount * angleSnapIndex;
        // the forward heading of the currently snapped to position
        Vector3 snapForward = new Vector3(Mathf.Cos(snapHeading), 0, Mathf.Sin(snapHeading));
        // the snap position
        Vector3 snapOffset = Vector3.up * camHeight;
        // the desired camera radius
        float camRadius = Mathf.Tan(Mathf.Deg2Rad * camOffsetTilt) * camHeight;
        // the final desired snap position
        snapOffset -= snapForward * camRadius;

        // Lerp to the desired position
        self.position = Vector3.Lerp(self.position, targetTransform.position + snapOffset, Time.deltaTime * positionLerp);

        // the current forward relative to the target, as of moving to the snap point
        Vector3 currentForward = targetTransform.position - self.position;
        // project to XZ to test radius
        currentForward.y = 0;
        // the amount of error in the radius
        float camRadiusCorrection = camRadius - currentForward.magnitude;
        // to be used for rotation/translation correction
        currentForward.Normalize();

        // correct the error in the radius, positioning is done!
        self.position -= currentForward * camRadiusCorrection;

        // used for free look and pitch
        Vector3 currentRight = Vector3.Cross(-currentForward, Vector3.up);

        // yaw and pitch the camera to face the target appropriately
        Quaternion currentRotation = Quaternion.AngleAxis(90-camOffsetTilt-camLookTilt, currentRight) * Quaternion.LookRotation(currentForward, Vector3.up);
        // rotate the up vector for future use
        Vector3 currentUp = currentRotation * Vector3.up;

        // immersive look rotates the camera by it's own up rather than the world's
        if (immersiveLook)
            currentRotation = Quaternion.AngleAxis(freeLookY * freeLookRangeY, currentRight) * Quaternion.AngleAxis(freeLookX * freeLookRangeX, currentUp) * currentRotation;
        else
            currentRotation = Quaternion.AngleAxis(freeLookY * freeLookRangeY, currentRight) * Quaternion.AngleAxis(freeLookX * freeLookRangeX, Vector3.up) * currentRotation;

        // Lerp to the desired rotation, rotating is done!
        self.rotation = Quaternion.Lerp(self.rotation, currentRotation, Time.deltaTime * rotationLerp);
	}
}

// TODO: move to a utilities script
// http://forum.unity3d.com/threads/re-map-a-number-from-one-range-to-another.119437/
public static class ExtensionMethods
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
