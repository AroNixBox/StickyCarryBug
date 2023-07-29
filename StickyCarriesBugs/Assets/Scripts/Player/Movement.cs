using System.Collections;
using UnityEngine;

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
    private int pickedUpObjects = 0;
    
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
                playerJump.Play();
                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
                isJumping = true;
            }

            if (Input.GetKeyDown(KeyCode.E) && currentPickableObject != null)
            {
                StartCoroutine(PickUpObject(currentPickableObject));
            } 
            
                    
            if (moveX < 0)
            {
                //Is Walking left
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
            playerImpact.Play();
        }
    }
    IEnumerator PickUpObject(GameObject obj)
    {
        playerPickup.Play();
        isPicking = true;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(3f);
        //Add to list of picked up Objects here.
        pickedUpObjects += 1;
        jumpForce -= jumpForceDecreasePerObject;
        obj.SetActive(false);
        isPicking = false;
    }
}
