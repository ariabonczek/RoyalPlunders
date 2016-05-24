using UnityEngine;
using System.Collections;

using UnityStandardAssets.ImageEffects;
using UnityEngine.UI;

public class ShaderTestManager : MonoBehaviour
{
    [SerializeField]
    private GameObject mainObject;

    [SerializeField]
    private GameObject widthSliderObject;

    [SerializeField]
    private Slider distanceSlider;
    [SerializeField]
    private Slider thetaSlider;
    [SerializeField]
    private Slider heightSlider;

    [SerializeField]
    private Slider widthSlider;

    private const float THETA_MIN = 0.0f;
    private const float THETA_MAX = 360.0f;

    private const float HEIGHT_MIN = -1.0f;
    private const float HEIGHT_MAX = 1.0f;

    private const float DISTANCE_MIN = 0.4f;
    private const float DISTANCE_MAX = 1.4f;

    // Camera stuff
    [SerializeField]
    private Camera mainCamera;
    private float cameraTheta = 90;
    private float cameraHeight = 0;
    private float cameraDistance = 0.4f;

    [SerializeField]
    private float cameraMoveSpeed;
    [SerializeField]
    private float cameraRotationSpeed;
    [SerializeField]
    private float cameraHorizontalSpeed;
    [SerializeField]
    private float cameraVerticalSpeed;
    private Vector3 mousePosition;
    private bool draggingLeft;
    private bool draggingRight;

    [SerializeField]
    private Material[] materials;

    private int numMaterials;
    private int currentIndex;

	public void Start ()
    {
        currentIndex = 0;
        draggingLeft = false;
        draggingRight = false;
        numMaterials = materials.GetLength(0);

        if (numMaterials <= 0)
            return;

        mainObject.GetComponent<MeshRenderer>().material = materials[currentIndex];

        UpdatePositionAndAngle();
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            cameraDistance -= cameraMoveSpeed * Time.deltaTime;
            UpdatePositionAndAngle();
        }
        else if (Input.GetKey(KeyCode.S))
        {
            cameraDistance += cameraMoveSpeed * Time.deltaTime;
            UpdatePositionAndAngle();
        }

        if (Input.GetKey(KeyCode.A))
        {
            cameraTheta -= cameraRotationSpeed * Time.deltaTime;
            UpdatePositionAndAngle();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            cameraTheta += cameraRotationSpeed * Time.deltaTime;
            UpdatePositionAndAngle();
        }

        if (Input.GetKey(KeyCode.Q))
        {
            cameraHeight += cameraVerticalSpeed;
            UpdatePositionAndAngle();
        }
        else if (Input.GetKey(KeyCode.E))
        {
            cameraHeight -= cameraVerticalSpeed;
            UpdatePositionAndAngle();
        }

        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            // Left mouse (rotation)
            if (!draggingRight)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    mousePosition = Input.mousePosition;
                    draggingLeft = true;
                    Cursor.visible = false;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    draggingLeft = false;
                    Cursor.visible = true;
                }

                if (draggingLeft)
                {
                    Vector3 lastMousePosition = mousePosition;
                    mousePosition = Input.mousePosition;

                    Vector3 delta = lastMousePosition - mousePosition;

                    cameraTheta += delta.x * cameraHorizontalSpeed;
                    cameraHeight += delta.y * cameraVerticalSpeed;

                    UpdatePositionAndAngle();
                }
            }

            // Right Mouse (distance)
            if (!draggingLeft)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    mousePosition = Input.mousePosition;
                    draggingRight = true;
                    Cursor.visible = false;
                }
                if (Input.GetMouseButtonUp(1))
                {
                    draggingRight = false;
                    Cursor.visible = true;
                }

                if (draggingRight)
                {
                    Vector3 lastMousePosition = mousePosition;
                    mousePosition = Input.mousePosition;

                    Vector3 delta = lastMousePosition - mousePosition;

                    cameraDistance += delta.x * cameraMoveSpeed * 0.001f;

                    UpdatePositionAndAngle();
                }
            }     
        }
    }

    private void UpdatePositionAndAngle()
    {
        // Update values
        cameraDistance = Mathf.Clamp(cameraDistance, DISTANCE_MIN, DISTANCE_MAX);
        cameraHeight = Mathf.Clamp(cameraHeight, HEIGHT_MIN, HEIGHT_MAX);

        cameraTheta = Mathf.Repeat(cameraTheta, THETA_MAX);

        // Update sliders
        distanceSlider.value = cameraDistance;
        heightSlider.value = cameraHeight;
        thetaSlider.value = cameraTheta;

        float theta = cameraTheta * Mathf.Deg2Rad;
        float r = cameraDistance;

        mainCamera.transform.position = new Vector3(
            r * Mathf.Cos(theta),
            cameraHeight,
            r * Mathf.Sin(theta));

        mainCamera.transform.LookAt(mainObject.transform);
    }

    #region UI Functions
    public void ChangeCameraAngleTheta(float angleDegrees)
    {
        cameraTheta = angleDegrees;

        UpdatePositionAndAngle();
    }

    public void ChangeCameraHeight(float height)
    {
        cameraHeight = height;

        UpdatePositionAndAngle();
    }

    public void ChangeCameraDistance(float distance)
    {
        cameraDistance = distance;

        UpdatePositionAndAngle();
    }

    public void ChangeShader(int index)
    {
        if (index >= materials.Length || index < 0)
        {
            return;
        }

        currentIndex = index;

        // If an outlined shader, activate the width slider
        if(index == 1 || index == 3 || index == 5)
        {
            widthSliderObject.SetActive(true);
        }
        else
        {
            widthSliderObject.SetActive(false);
        }

        // If Standard Cel Outline, turn on edge detection
        if (index == 5)
        {
            mainCamera.GetComponent<EdgeDetection>().enabled = true;
        }
        else
        {
            mainCamera.GetComponent<EdgeDetection>().enabled = false;
        }

        mainObject.GetComponent<MeshRenderer>().material = materials[currentIndex];
        ChangeLineWidth(widthSlider.value);
    }

    public void ChangeLineWidth(float width)
    {
        mainObject.GetComponent<MeshRenderer>().material.SetFloat("_Outline", width);

        if(currentIndex == 5)
        {
            mainCamera.GetComponent<EdgeDetection>().sampleDist = RemapFloat(width, 0.002f, 0.03f, 0.5f, 10);
        }
    }
    #endregion

    #region Utility Functions
    private float RemapFloat(float value, float min1, float max1, float min2, float max2)
    {
        return (value - min1) / (max1 - min1) * (max2 - min2) + min2;
    }

    private float RepeatFloat(float value, float min, float max)
    {
        if(value < min)
        {
            value = max - Mathf.Abs((min - value));
        }
        else if(value > max)
        {
            value = Mathf.Abs(value - max) + min;
        }

        return value;
    }
    #endregion  
}
