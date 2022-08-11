using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;

    [Header("----- Player Stats -----")]
    [Range(1, 10)][SerializeField] float playerSpeed;
    [Range(1, 4)][SerializeField] float sprintMultiplyer;
    [Range(8, 18)][SerializeField] float jumpHeight;
    [Range(15, 30)][SerializeField] float gravity;
    [Range(1, 3)][SerializeField] int jumpsMax;
    [Range(1, 20)][SerializeField] public int HP;

    [Header("----- Weapon Stats -----")]
    [Range(1, 200)][SerializeField] int shootingDist;
    [Range(.01f, 200)][SerializeField] float shootRate;
    [Range(.01f, 200)][SerializeField] int shootDamage;
    //[Range(.01f, 200)][SerializeField] int bulletPershot;
    public List<gunStats> gunStat = new List<gunStats>();

    private Vector3 playerVelocity;
    Vector3 move = Vector3.zero;
    int timesJumped;
    float playerSpeedOriginal;
    bool isSprinting = false;
    bool isShooting = false;


    // Start is called before the first frame update
    void Start()
    {
        playerSpeedOriginal = playerSpeed;

    }

    // Update is called once per frame
    void Update()
    {
        playerMovement();
        Sprint();

        if (Input.GetKeyDown(KeyCode.K))
        {
            takeDamage(5);
        }


        playerMovement();
        Sprint();

        StartCoroutine(shoot());

    }

    void playerMovement()
    {
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            timesJumped = 0;
        }

        //get input from Unity input system
        move = (transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"));

        // add our move vector to character controller
        controller.Move(move * Time.deltaTime * playerSpeed);

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && timesJumped < jumpsMax)
        {
            playerVelocity.y = jumpHeight;
            timesJumped++;
        }

        playerVelocity.y -= gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            isSprinting = true;
            playerSpeed = playerSpeed * sprintMultiplyer;
        }
        if (Input.GetButtonUp("Sprint"))
        {
            isSprinting = false;
            playerSpeed = playerSpeedOriginal;
        }
    }
    IEnumerator shoot()
    {

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootingDist, Color.red, 0.00001f);
        if (gunStat.Count != 0 && Input.GetButton("Shoot") && isShooting == false)
        {
            isShooting = true;

            // do something
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootingDist))
            {
                if (hit.collider.GetComponent<IDamageable>() != null)
                {
                    IDamageable isDamageable = hit.collider.GetComponent<IDamageable>();

                    if (hit.collider is SphereCollider)
                        isDamageable.takeDamage(shootDamage * 2);
                    else
                        isDamageable.takeDamage(shootDamage);
                }
            }

            yield return new WaitForSeconds(shootRate);
            isShooting = false;
        }
    }

    public void gunPickup(float shtRate, int shtingDist, int shtDamage, gunStats _gstats)
    {
        shootRate = shtRate;
        shootingDist = shtingDist;
        shootDamage = shtDamage;
        //bulletPershot = bulletCount;
        gunStat.Add(_gstats);
    }


    public void takeDamage(int damage)
    {
        HP -= damage;
        //StartCoroutine(damageFlash());

        if (HP <= 0)
        {
            // death
        }
    }
}
