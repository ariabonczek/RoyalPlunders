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
    private uint angleSnapCount = 4;
    // current snap point
    private uint angleSnapIndex = 0;

    // goal quat/vec
    Quaternion goalQuat;
    Vector3 goalVec;

    // whatever this script is attached to
    private Transform self;

	void Start ()
    {
        self = transform;
	}
	
	void Update ()
    {
        // angle to the target from self
        float ang = Mathf.PI * 2 / angleSnapCount * angleSnapIndex;

        // height
        Vector3 offset = Vector3.up * camHeight;
        // snapped forward
        Vector3 snappedForward = new Vector3(Mathf.Cos(ang), 0, Mathf.Sin(ang));
        // lateral
        offset -= snappedForward * (Mathf.Tan(Mathf.Deg2Rad * camOffsetTilt) * camHeight);

        // look at the target and tilt up
        goalQuat = Quaternion.AngleAxis(90-camOffsetTilt-camLookTilt, self.right) * Quaternion.LookRotation(snappedForward, Vector3.up);

        // adjust "look"
        goalQuat = Quaternion.AngleAxis(freeLookX * freeLookRangeX, self.up) * Quaternion.AngleAxis(freeLookY * freeLookRangeY, self.right) * goalQuat;
        
        // apply the offset
        goalVec = targetTransform.position + offset;

        self.position = Vector3.Slerp(self.position, goalVec, Time.deltaTime * 5f);
        self.rotation = Quaternion.Slerp(self.rotation, goalQuat, Time.deltaTime * 5f);
	}

    // set the number of snapping points for rotating the camera
    public void setSnapCount(uint count)
    {
        angleSnapCount = (uint)Mathf.Clamp(count, 1, 128);
        angleSnapIndex = (uint)Mathf.Clamp(angleSnapIndex, 0, angleSnapCount - 1);
    }

    // snap the camera to the right
    public void snapRight()
    {
        angleSnapIndex = angleSnapIndex == angleSnapCount - 1 ? 0 : angleSnapIndex + 1;
    }

    // snap the camera to the left
    public void snapLeft()
    {
        angleSnapIndex = angleSnapIndex == 0 ? angleSnapCount - 1 : angleSnapIndex - 1;
    }
}
