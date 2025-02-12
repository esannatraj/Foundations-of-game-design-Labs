using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10;
    public float maxSpeed = 20;
    public float upSpeed = 10;
    private bool onGroundState = true;
    private Rigidbody2D marioBody;
    
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;

    public TextMeshProUGUI scoreText;
    public GameObject enemies;

    public Animator marioAnimator;

    public AudioSource marioAudio;
    public AudioClip marioDeath;
    public float deathImpulse = 15;


    public GameOverScreen gameOverScreen; // Reference to the GameOverScreen script

    // state
    [System.NonSerialized]
    public bool alive = true;


    // Start is called before the first frame update
    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        // update animator state
        marioAnimator.SetBool("onGround", onGroundState);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("a") && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
            if (marioBody.velocity.x > 0.1f)
                marioAnimator.SetTrigger("onSkid");
        }

        if (Input.GetKeyDown("d") && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
            if (marioBody.velocity.x < -0.1f)
                marioAnimator.SetTrigger("onSkid");
        }

        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.velocity.x));

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground") && !onGroundState)
        {
            onGroundState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
        }
    }

    // FixedUpdate is called 50 times a second
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(moveHorizontal) > 0)
        {
            Vector2 movement = new Vector2(moveHorizontal, 0);
            // check if it doesn't go beyond maxSpeed
            if (marioBody.velocity.magnitude < maxSpeed)
                marioBody.AddForce(movement * speed);
        }

        // stop
        if (Input.GetKeyUp("a") || Input.GetKeyUp("d"))
        {
            // stop
            marioBody.velocity = Vector2.zero;
        }

        if (Input.GetKeyDown("space") && onGroundState)
        {
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
        }
    }

    void PlayJumpSound()
    {
        // play jump sound
        marioAudio.PlayOneShot(marioAudio.clip);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collided with goomba!");

            if (gameOverScreen != null)
            {
                gameOverScreen.ShowGameOverScreen();
            }
            else
            {
                Debug.LogError("GameOverScreen reference is missing in PlayerMovement!");
            }
        }
    }

    void PlayDeathImpulse()
    {
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }


    public void RestartButtonCallback(int input)
    {
        Debug.Log("Restart!");
        // reset everything
        ResetGame();
        // resume time
        Time.timeScale = 1.0f;
    }

    private void ResetGame()
    {
        // Reset player position
        marioBody.transform.position = new Vector3(0.0f, 0.0f, 0.0f);

        // Reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;

        // Reset score
        scoreText.text = "Score: 0";

        // Reset the actual score in JumpOverGoomba script
        JumpOverGoomba jumpOverGoomba = GetComponent<JumpOverGoomba>();
        if (jumpOverGoomba != null)
        {
            jumpOverGoomba.score = 0;
        }

        // Reset Goomba
        foreach (Transform eachChild in enemies.transform)
        {
            EnemyMovement enemyMovement = eachChild.GetComponent<EnemyMovement>();
            if (enemyMovement != null)
            {
                eachChild.transform.position = enemyMovement.startPosition; // Use dynamically stored position
            }
        }

    }

}