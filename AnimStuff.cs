using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimStateController : MonoBehaviour
{
    public Animator animator;
    public float playerVelocity;
    public bool playerCrouch = false;
    public bool playerGrounded = true;
    public bool playerJump = false;
    public bool playerThrowGrenade = false;
    public bool playerShoot = false;
    public bool playerDie = false;
    public bool idle = true;
    public float idleTime = 0.0f;
    public Vector3 lastPosistion = new Vector3(0,0,0);
    public float timeStep;
    public Vector3 newPosistion;
    public float newPositionTime;
    public Transform weaponLocation;
    public GameObject weapon;
    public List<GameObject> weaponPrefabs = new List<GameObject>();
    public int currentWeapon = 0;
    public bool playingAnim = false;

    private void Start()
    {
        newPosistion = this.transform.position;
        timeStep = Time.deltaTime;
        changeGun(0);
        currentWeapon = 0;
        animator.SetInteger("WeaponType_int", 1);
    }
    private IEnumerator coroutine;
    public void changeGun(int i)
    {
        if (currentWeapon == i || playingAnim)
        {
            return;
        }
        Quaternion startRotation = weapon.transform.rotation;
        if (i == 0)
        {
            currentWeapon = 0;
            animator.SetInteger("WeaponType_int", 1);
            
            if(weapon != null)
            {
                startRotation = weapon.transform.rotation;
                Destroy(weapon.gameObject);         
            }
            GameObject newWeapon = Instantiate(weaponPrefabs[0], weaponLocation.position, startRotation);
            newWeapon.transform.SetParent(weaponLocation);
            newWeapon.transform.rotation = startRotation;
            newWeapon.transform.localScale = weaponLocation.localScale;
            weapon = newWeapon;
        }
        if (i == 1)
        {
            currentWeapon = 1;
            animator.SetInteger("WeaponType_int", 10);
            if (weapon != null)
            {
                Destroy(weapon.gameObject);
            }

            GameObject newWeapon = Instantiate(weaponPrefabs[1], weaponLocation.position, startRotation);
            newWeapon.transform.SetParent(weaponLocation);
            newWeapon.transform.rotation = startRotation;
            newWeapon.transform.localScale = weaponLocation.localScale;
            weapon = newWeapon;
        }
    }
    public void throwGrenade()
    {
        changeGun(1);
        playingAnim = true;
        playerVelocity = 0;
        currentWeapon = 1;
        
        animator.SetInteger("WeaponType_int", 10);
        animator.SetFloat("Body_Horivontal_f", 0);
        animator.SetFloat("Body_Vertical_f", 0);
        animator.SetBool("Shoot_b", true);
        coroutine = grenadeDelay(2.0f);
        StartCoroutine(coroutine);
    }
    
    private IEnumerator grenadeDelay(float delay)
    {

            yield return new WaitForSeconds(delay);
            animator.SetBool("Shoot_b", false);           
            animator.SetInteger("WeaponType_int", 1);
            playingAnim = false;          
            StopCoroutine(coroutine);       
            changeGun(0);
            currentWeapon = 0;
            yield return false;
     
    }
    private void FixedUpdate()
    {
        if (!playingAnim)
        {
            newPositionTime = timeStep;///I Really can't explain the order but this and the next two lines must happen as written to get a valid velocity each fram.
            timeStep += Time.deltaTime;
            newPosistion = new Vector3(this.transform.position.x, 0, this.transform.position.z);//Nuet the y axis so we don't factor jumping or falling into our velocity. We may change this later for a falling animation when not grounded.

            if (lastPosistion != newPosistion)
            {
                float stepTime = timeStep - newPositionTime;
                //Debug.Log("Time Passage: " + stepTime);
                playerVelocity = Vector3.Distance(lastPosistion, newPosistion) / stepTime;
                animator.SetFloat("Speed_f", playerVelocity);
                //Debug.Log(playerVelocity + " : Player Velocity.");
            }
            else
            {
                playerVelocity = 0;
                animator.SetFloat("Speed_f", playerVelocity);
            }

            if (playerThrowGrenade)
            {
                throwGrenade();
            }

        }
        animator.SetBool("Shoot_b", playerShoot);
        animator.SetBool("Crouch_b", playerCrouch);
        animator.SetBool("Grounded", playerGrounded);
        animator.SetBool("Jump_b", playerJump);

        //send packets to all clients to update this players anim state numbers.
        //reset bools so the animator will also be reset.
        playerCrouch = false;
        playerJump = false;
        playerGrounded = true;
        playerThrowGrenade = false;
        playerShoot = false;
        playerDie = false;
        //animator.SetInteger("WeaponType_int", 1);
        //Set the last position to the new position of this frame. so we have it for velocity calculation on next frame.
        lastPosistion = newPosistion;

    }
}
