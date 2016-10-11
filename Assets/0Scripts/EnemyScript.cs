using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EnemyScript : MonoBehaviour {
    public Transform player;
    public Transform enemy;
    public float speed = 5.0f;

    bool isOffScreen = false;
    public float offscreenDotRange = 0.7f;

    bool isVisible = false;
    public float visibleDotRange = 0.8f; // ** between 0.75 and 0.85 (originally 0.8172719) 

    bool isInRange = false;

    public float followDistance = 24.0f;
    public float maxVisibleDistance = 25.0f;

    public float reduceDistAmt = 3.1f;

    private float sqrDist = 0.0f;
    [SerializeField]
    public float health = 100.0f;
    public float damage = 20.0f;

    public AudioClip enemySightedSFX;

    private bool hasPlayedSeenSound = false;

    private float colDist = 5.0f; // raycast distance in front of enemy when checking for obstacles

	// Use this for initialization
	void Start () {
	    if (!player)
        {
            player = GameObject.Find("Player").transform;
        }
        enemy = transform;
	}
	
	// Update is called once per frame
	void Update () {
        // Movement : check if out-of-view, then move
        CheckIfOffScreen();

        // if is Off Screen, move
        if (isOffScreen)
        {
            MoveEnemy();

            // restore health
            RestoreHealth();
        }
        else
        {
            // check if Player is seen
            CheckIfVisible();

            if (isVisible)
            {
                // deduct health
                DeductHealth();

                // stop moving
                StopEnemy();

                // play sound only when the Man is first sighted
                if (!hasPlayedSeenSound)
                {
                    AudioSource.PlayClipAtPoint(enemySightedSFX, player.position);
                }
                hasPlayedSeenSound = true; // sound has now played
            }
            else
            {
                // check max range
                CheckMaxVisibleRange();

                // if far away then move, else stop
                if (!isInRange)
                {
                    MoveEnemy();
                }
                else
                {
                    StopEnemy();
                }

                // reset hasPlayedSeenSound for next time isVisible first occurs
                hasPlayedSeenSound = false;
            }
        }
    }
    void DeductHealth()
    {
        // deduct health
        health -= damage * Time.deltaTime;

        // check if no health left
        if (health <= 0.0f)
        {
            health = 0.0f;
            Debug.Log("YOU ARE OUT OF HEALTH !");

            // Restart game here!
            SceneManager.LoadScene("die");
        }
    }

    protected void RestoreHealth()
    {
        // deduct health
        health += damage * Time.deltaTime;

        // check if no health left
        if (health >= 100.0f)
        {
            health = 100.0f;
            Debug.Log( "HEALTH is FULL" );
        }
    }


    protected void CheckIfOffScreen()
    {
        Vector3 fwd = player.forward.normalized;
        Vector3 other = (enemy.position - player.position).normalized;

        float theProduct = Vector3.Dot(fwd, other);

        if (theProduct < offscreenDotRange)
        {
            isOffScreen = true;
        }
        else
        {
            isOffScreen = false;
        }
    }


    protected void MoveEnemy()
    {
        // Check the Follow Distance
        CheckDistance();

        // if not too close, move
        if (!isInRange)
        {
            GetComponent<Rigidbody>().velocity = new Vector3(0.0f, GetComponent<Rigidbody>().velocity.y, 0.0f); // maintain gravity

            // New Movement - with obstacle avoidance
            Vector3 dir = (player.position - enemy.position).normalized;
            RaycastHit hit;

            if (Physics.Raycast(enemy.position, enemy.forward, out hit, colDist))
            {
                Debug.Log( " obstacle ray hit " + hit.collider.gameObject.name );
                if (hit.collider.gameObject.name != "Player" && hit.collider.gameObject.name != "Terrain")
                {
                    dir += hit.normal * 20;
                }
            }

            Quaternion rot = Quaternion.LookRotation(dir);

            enemy.rotation = Quaternion.Slerp(enemy.rotation, rot, Time.deltaTime);
            enemy.position += enemy.forward * speed * Time.deltaTime;
        }
        else
        {
            StopEnemy();
        }
    }


    protected void StopEnemy()
    {
        transform.LookAt(player);

        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }


    protected void CheckIfVisible()
    {
        Vector3 fwd = player.forward.normalized;
        Vector3 other = (enemy.position - player.position).normalized;

        float theProduct = Vector3.Dot(fwd, other);

        if (theProduct > visibleDotRange)
        {
            // Check the Max Distance
            CheckMaxVisibleRange();

            if (isInRange)
            {
                // Linecast to check for occlusion
                RaycastHit hit;

                if (Physics.Linecast(enemy.position + (Vector3.up * 1.75f) + enemy.forward, player.position, out hit))
                {
                    Debug.Log("Enemy sees " + hit.collider.gameObject.name);

                    if (hit.collider.gameObject.name == "Player")
                    {
                        isVisible = true;
                    }
                }
            }
            else
            {
                isVisible = false;
            }
        }
        else
        {
            isVisible = false;
        }
    }


    protected void CheckDistance()
    {
        float sqrDist = (enemy.position - player.position).sqrMagnitude;
        float sqrFollowDist = followDistance * followDistance;

        if (sqrDist < sqrFollowDist)
        {
            isInRange = true;
        }
        else
        {
            isInRange = false;
        }
    }


    public void ReduceDistance()
    {
        followDistance -= reduceDistAmt;
        damage += 20.0f;
    }


    protected void CheckMaxVisibleRange()
    {
        float sqrDist = (enemy.position - player.position).sqrMagnitude;
        float sqrMaxDist = maxVisibleDistance * maxVisibleDistance;

        if (sqrDist < sqrMaxDist)
        {
            isInRange = true;
        }
        else
        {
            isInRange = false;
        }
    }

}
