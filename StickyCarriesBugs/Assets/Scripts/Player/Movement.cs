using System.Collections;
using UnityEngine;
using TMPro;

public class Movement : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpForce = 5.0f;
    public float fallMultiplier = 2.5f;
    public float fallThreshold = 1.5f;
    public float jumpForceDecreasePerObject = 0.1f;
    private bool isJumping = false;
    private bool isPicking = false;
    private bool isFalling = false;
    private bool canPlayRunSound = true;
    private Rigidbody2D rb;
    public int pickedUpObjects = 0;

    [SerializeField] private GameObject DropObject;
    [SerializeField] private TextMeshProUGUI indicator;

    [SerializeField] private Animator anim;
    
    //AudioSources
    [SerializeField] private AudioSource playerJump;
    [SerializeField] private AudioSource playerImpact;
    [SerializeField] private AudioSource playerPickup;
    [SerializeField] private AudioSource playerWalk;
    
    
    //Object to pickup
    private GameObject currentPickableObject = null;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        
        if (!isPicking)
        {
            rb.velocity = new Vector2(moveX * speed, rb.velocity.y);
            
            if (Input.GetButtonDown("Jump") && !isJumping)
            {
                anim.SetTrigger("isJumping");
                playerJump.Play();
                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
                isJumping = true;
                anim.SetBool("isGrounded", false);
            }

            if (Input.GetKeyDown(KeyCode.E) && currentPickableObject != null)
            {
                StartCoroutine(PickUpObject(currentPickableObject));
            } 
            
            if(Input.GetKeyDown(KeyCode.Q) && pickedUpObjects > 0)
            {
                pickedUpObjects--;
            }
                    
            if (moveX < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (moveX > 0)
            {
                //Is Walking right
                transform.localScale = new Vector3(1, 1, 1);
            }

            if (moveX < 0 || moveX > 0)
            {
                if (!isJumping)
                {
                    if (canPlayRunSound)
                    {
                        anim.SetBool("isWalking", true);
                        playerWalk.Play();
                    } 
                    canPlayRunSound = false;
                }
                else
                {
                    playerWalk.Stop();
                    canPlayRunSound = true;
                }
            }
            else
            {
                anim.SetBool("isWalking", false);
                playerWalk.Stop();
                canPlayRunSound = true;
            }
            
        }
        
        if (rb.velocity.y < -fallThreshold)
        {
            //paste falling anim
            isFalling = true;
        }
        else if (rb.velocity.y > fallThreshold)
        {
            //paste jumping anim
            isFalling = false;
        }

        if (isFalling)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        indicator.text = "Extra Weight: " + pickedUpObjects.ToString() + "kg";
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pickable"))
        {
            currentPickableObject = collision.gameObject;
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pickable"))
        {
            currentPickableObject = null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            anim.SetBool("isGrounded", true);
            playerImpact.Play();
        }
    }
    IEnumerator PickUpObject(GameObject obj)
    {
        playerPickup.Play();
        anim.SetTrigger("isPickingUp");
        isPicking = true;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(1f);
        //Add to list of picked up Objects here.
        pickedUpObjects += 1;
        jumpForce -= jumpForceDecreasePerObject;
        obj.SetActive(false);
        isPicking = false;
    }
}
