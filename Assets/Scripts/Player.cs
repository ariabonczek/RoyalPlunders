using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour, IAttackable
{
    public const float KNOCKOUT_RANGE = 5.0f;
    public static Player instance;

    // Use this for initialization
    void Start()
    {
        if (instance != null)
        {
            Debug.LogWarning("MULTIPLE PLAYERS DETECTED");
        }
        instance = this;

        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Capsule = GetComponent<CapsuleCollider>();

        m_Animator.SetBool("Crouch", false);
        m_Animator.SetBool("OnGround", true);
        m_Animator.SetFloat("Jump", 0);

        inventory = new List<Loot>();
        abracadavers = new List<Abracadaver>();
        cakes = new List<Cake>();


        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        if (Camera.main != null)
        {
            m_Cam = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning(
                "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.LeftControl)) m_Crouching = !m_Crouching;

        if (m_Cam != null)
        {
            // calculate camera relative direction to move:
            m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
            m_Move = v * m_CamForward + h * m_Cam.right;
        }
        else
        {
            // we use world-relative directions in the case of no main camera
            m_Move = v * Vector3.forward + h * Vector3.right;
        }

        // walk speed multiplier
        if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;

        if(pickedUp != null)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {   
                pickedUp.Throw(this);
                pickedUp = null;
            }
        }
        else
        {
            // Interaction
            if (!isInteracting)
            {
                CheckForInteractables();

                if (nearestInteractable != null && Input.GetKeyDown(KeyCode.Q))
                {
                    isInteracting = true;
                    nearestInteractable.StartInteraction(this);
                }

            }
            else
            {
                nearestInteractable.UpdateInteraction(this);

                if (nearestInteractable.ShouldEndInteraction)
                {
                    nearestInteractable.EndInteraction(this);
                    nearestInteractable = null;
                    isInteracting = false;
                }
            }

            // Combat
            if (!isInteracting && !isAttacking)
            {
                CheckForEnemies();

                if (nearestEnemy != null && Input.GetKeyDown(KeyCode.Q))
                {
                    isAttacking = true;
                    StartAttacking(nearestEnemy);
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {

                if (abracadaverReticleActive)
                {
                    gadgetText.text = "Current Gadget: None";
                    abracadaverReticleActive = false;
                }
                else
                {
                    gadgetText.text = "Current Gadget: Abracadaver";
                    abracadaverReticleActive = true;
                    cakeReticleActive = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (cakeReticleActive)
                {
                    gadgetText.text = "Current Gadget: None";
                    cakeReticleActive = false;
                }
                else
                {
                    gadgetText.text = "Current Gadget: Piece of Cake";
                    abracadaverReticleActive = false;
                    cakeReticleActive = true;
                }
            }

            if (abracadaverReticleActive)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit);

                reticlePoint = hit.point;

                RaycastHit hit2;
                Physics.Raycast(transform.position + new Vector3(0.0f, 5.0f, 0.0f), (hit.point - (transform.position + new Vector3(0.0f, 5.0f, 0.0f))).normalized, out hit2);

                if (Vector3.Distance(transform.position, hit.point) <= ABRACADAVER_RANGE &&
                   hit.point == hit2.point)
                {
                    abracadaverReticleValid = true;
                }
                else
                {
                    abracadaverReticleValid = false;
                }

                if (abracadaverReticleValid && Input.GetMouseButtonDown(0))
                {
                    ThrowAbracadaver(reticlePoint + new Vector3(0.0f, 2.0f, 0.0f));
                }
            }
            else if (cakeReticleActive)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit);// LayerMask.NameToLayer("Floor"));

                reticlePoint = hit.point;
                reticlePoint.y = 0.0f;

                Vector3 reticleDirection = (reticlePoint - transform.position).normalized;

                reticlePoint = transform.position + reticleDirection * CAKE_RANGE;

                RaycastHit hit2;
                Physics.Raycast(transform.position + new Vector3(0.0f, 5.0f, 0.0f), (reticlePoint - (transform.position + new Vector3(0.0f, 5.0f, 0.0f))).normalized, out hit2);

                if (reticlePoint == hit2.point)
                {
                    cakeReticleValid = true;
                }
                else
                {
                    cakeReticleValid = false;
                }

                if (cakeReticleValid && Input.GetMouseButtonDown(0))
                {
                    PlaceCake(reticlePoint + new Vector3(0.0f, 1.0f, 0.0f));
                }
            }
        }

     
        if (!isAttacking)
        {
            Move(m_Move);
        }
    }

    #region Components
    Rigidbody m_Rigidbody;
    Animator m_Animator;
    CapsuleCollider m_Capsule;
    #endregion

    #region External Objects
    [SerializeField]
    UnityEngine.UI.Text healthText;
    [SerializeField]
    UnityEngine.UI.Text gadgetText;
    [SerializeField]
    UnityEngine.UI.Text valueText;
    #endregion

    #region Movement
    [SerializeField]
    float m_MovingTurnSpeed = 360;
    [SerializeField]
    float m_StationaryTurnSpeed = 180;

    [SerializeField]
    float m_MoveSpeedMultiplier = 1f;
    [SerializeField]
    float m_AnimSpeedMultiplier = 1f;

    Transform m_Cam;
    Vector3 m_CamForward;
    Vector3 m_Move;

    float m_TurnAmount;
    float m_ForwardAmount;
    bool m_Crouching;

    void Move(Vector3 move)
    {
        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);
        move = Vector3.ProjectOnPlane(move, Vector3.up);
        m_TurnAmount = Mathf.Atan2(move.x, move.z);
        m_ForwardAmount = move.z;

        ApplyExtraTurnRotation();

        Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

        // we preserve the existing y part of the current velocity.
        v.y = m_Rigidbody.velocity.y;
        m_Rigidbody.velocity = v;


        // send input and other state parameters to the animator
        UpdateAnimator(move);
    }

    public void ClipMoveToAxis(Vector3 v)
    {
        m_Move.Scale(v);
    }

    void ApplyExtraTurnRotation()
    {
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
        transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
    }

    void UpdateAnimator(Vector3 move)
    {
        // update the animator parameters
        m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
        m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
        m_Animator.SetBool("Crouch", m_Crouching);

        m_Animator.speed = m_AnimSpeedMultiplier;
    }

    #endregion

    #region Interaction
    [SerializeField]
    private float interactionRadius;

    private Interactable nearestInteractable;

    private List<Loot> inventory;
    private int totalValue = 0;
    private bool isInteracting = false;

    void CheckForInteractables()
    {
        Collider[] interactablesInRange = Physics.OverlapSphere(transform.position, interactionRadius);

        Interactable temp = null;
        float tempDist = float.MaxValue;
        foreach (Collider c in interactablesInRange)
        {
            if (c.tag != "Interactable")
                continue;

            float d = Vector3.Distance(transform.position, c.transform.position);
            if (d < tempDist)
            {
                temp = c.gameObject.GetComponent<Interactable>();
                tempDist = d;
            }
        }

        nearestInteractable = temp;
    }

    public void AddToInventory(Loot loot)
    {
        inventory.Add(loot);
        totalValue += loot.value;
        valueText.text = "Total Value: " + totalValue;
    }
    #endregion

    #region Combat

    public const int MAXIMUM_HEALTH = 6;
    private int health = 6;
    private Guard nearestEnemy;
    private bool isAttacking = false;

    [SerializeField]
    private float throwForce = 1.0f;

    public float ThrowForce
    {
        get { return throwForce; }
    }

    private IPickup pickedUp = null;

    void CheckForEnemies()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, KNOCKOUT_RANGE);

        Guard temp = null;
        float tempDist = float.MaxValue;
        foreach (Collider c in enemiesInRange)
        {
            if (!(c.tag == "Enemy" || c.tag == "Target"))
                continue;

            float d = Vector3.Distance(transform.position, c.transform.position);
            if (d < tempDist)
            {
                temp = c.gameObject.GetComponent<Guard>();
                tempDist = d;
            }
        }

        nearestEnemy = temp;
    }

    bool IsInAttackCone(Guard guard)
    {
        float a = Vector3.Angle(transform.forward, (transform.position - guard.transform.position).normalized);
        if (a > 180 - guard.KnockoutAngle * 0.5)
        {
            return true;
        }

        return false;
    }

    void StartAttacking(Guard guard)
    {
        Debug.Log("Start Attacking");
        if (guard.IsKnockedOut)
        {
            (guard as IPickup).Pickup(this);
            pickedUp = guard;
        }
        else
        {
            guard.TransitionToKnockoutState();
        }
        isAttacking = false;
    }

    void UpdateAttacking(Guard guard)
    {

    }

    void EndAttacking(Guard guard)
    {

    }

    bool IAttackable.TakeDamage()
    {
        --health;
        healthText.text = "Health: " + health;
        if (health <= 0)
        {
            return false;
        }
        return true;
    }
    #endregion

    #region Gadgets

    [SerializeField]
    private GameObject abracadaverPrefab;

    [SerializeField]
    private GameObject cakePrefab;

    public List<Abracadaver> abracadavers;
    public List<Cake> cakes;

    private bool abracadaverReticleActive = false;
    private bool abracadaverReticleValid = false;

    private bool cakeReticleActive = false;
    private bool cakeReticleValid = false;

    public const float ABRACADAVER_RANGE = 18.0f;
    public const float CAKE_RANGE = 5.0f;

    public const int ABRACADAVER_MAX = 2;
    public const int CAKE_MAX = 1;

    private Vector3 reticlePoint;

    void OnDrawGizmos()
    {
        if (abracadaverReticleActive)
        {
            if (abracadaverReticleValid)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawSphere(reticlePoint, 2.0f);
        }

        if (cakeReticleActive)
        {
            if (cakeReticleValid)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawSphere(reticlePoint, 2.0f);
        }
    }

    void ThrowAbracadaver(Vector3 point)
    {
        if (abracadavers.Count == ABRACADAVER_MAX)
        {
            Destroy(abracadavers[0].gameObject);
            abracadavers.RemoveAt(0);
            //return;
        }
        GameObject a = (Instantiate(abracadaverPrefab, point, Quaternion.identity) as GameObject);
        abracadavers.Add(a.GetComponent<Abracadaver>());
    }

    void PlaceCake(Vector3 point)
    {
        if (cakes.Count == CAKE_MAX)
        {
            Destroy(cakes[0].gameObject);
            cakes.RemoveAt(0);
            //return;
        }
        GameObject c = (Instantiate(cakePrefab, point, Quaternion.identity) as GameObject);
        cakes.Add(c.GetComponent<Cake>());
    }

    #endregion

}
