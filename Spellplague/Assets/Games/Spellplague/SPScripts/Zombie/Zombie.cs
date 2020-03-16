using Spellplague.Characters;
using Spellplague.Utility;
using UnityEngine;
using UnityEngine.AI;

namespace Spellplague.AI
{
    public class Zombie : Character
    {
        public enum WanderType { Random, Waypoint };

        public WanderType wanderType = WanderType.Random;
        public float wanderSpeed = 1.2f;
        public float chaseSpeed = 2.88f;
        public float fov = 95f;
        public float viewDistance = 9.5f;
        public float wanderRadius = 8.5f; //Area where next random wanderpoint is selected
        public float loseThreshold = 4f; //Time in seconds until losing the player after not detecting it
        public Transform[] waypoints; //Array of waypoints is only used when waypoint wandering is selected
        public PlayerStateVariable playerState;
        public float zombieOriginalDetectionRadius = 5;
        public float jumpRadius = 7;
        public float sprintRadius = 9;
        public float radiusViewDistanceMultiplier = 1.25f;
        public float playerDamageDistance = 1.6f;
        public float damage = 10;
        public float destinationOffset = 1.5f;

        private bool isAware = false;
        private bool isDetecting = false;
        private Vector3 wanderPoint;
        private NavMeshAgent agent;
        private int waypointIndex = 0;
        private float loseTimer = 0;
        private float zombieDetectionRadius = 5;
        private IDamageable playerDamageComponent;
        private GameObject target;

        private AudioSource zombieAudSrc;
        [SerializeField]
        private float attackAudSrcPitch = 1.5f;
        private float originalAudSrcPitch;
        [SerializeField]
        private float attackAudSrcVolume = 0.375f;
        private float originalAudSrcVolume;

        private void Start()
        {
            target = GameObject.FindGameObjectWithTag("Player");
            agent = GetComponent<NavMeshAgent>();
            playerDamageComponent = target.GetComponent<IDamageable>();
            zombieAudSrc = GetComponent<AudioSource>();
            originalAudSrcPitch = zombieAudSrc.pitch;
            originalAudSrcVolume = zombieAudSrc.volume;
            wanderPoint = RandomWanderPoint();
        }

        private void Update()
        {
            if (isAware)
            {
                Vector3 targetVector = target.transform.position - transform.position;
                agent.SetDestination(target.transform.position - (targetVector.normalized * destinationOffset));
                agent.speed = chaseSpeed;

                float targetDistance = targetVector.magnitude;
                if (targetDistance < playerDamageDistance)
                {
                    playerDamageComponent.TakeDamage(damage * Time.deltaTime);
                }
                else if (!isDetecting)
                {
                    loseTimer += Time.deltaTime;
                    if (loseTimer >= loseThreshold)
                    {
                        isAware = false;
                        loseTimer = 0;
                    }
                }
            }
            else
            {
                Wander();
                agent.speed = wanderSpeed;
            }

            SearchForPlayer();
        }

        public void SearchForPlayer()
        {
            Physics.Raycast(transform.position, (target.transform.position - transform.position).normalized,
                out RaycastHit hitRay, viewDistance * radiusViewDistanceMultiplier);
            if (hitRay.collider != null
                && !hitRay.transform.CompareTag("Player"))
            {
                return;
            }

            RadiusChanger();
            if (ZombieDetection())
            {
                OnAware();
            }
            else if (Vector3.Angle(Vector3.forward, transform.InverseTransformPoint(target.transform.position)) < fov / 2f)
            {
                if (Vector3.Distance(target.transform.position, transform.position) < viewDistance)
                {
                    if (Physics.Linecast(transform.position, target.transform.position, out RaycastHit hitLine, -1))
                    {
                        if (hitLine.transform.CompareTag("Player"))
                        {
                            OnAware();
                        }
                        else
                        {
                            isDetecting = false;
                        }
                    }
                    else
                    {
                        isDetecting = false;
                    }
                }
                else
                {
                    isDetecting = false;
                }
            }
            else
            {
                isDetecting = false;
            }
        }

        public void OnAware()
        {
            isAware = true;
            isDetecting = true;
            loseTimer = 0;
        }

        private bool ZombieDetection()
        {
            if (playerState.CurrentPlayerStance == PlayerStance.Crouch)
            {
                return false;
            }

            Collider[] hits = Physics.OverlapSphere(transform.position, zombieDetectionRadius);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.CompareTag("Player"))
                {
                    zombieAudSrc.pitch = attackAudSrcPitch;
                    zombieAudSrc.volume = attackAudSrcVolume;
                    return true;
                }
            }

            zombieAudSrc.pitch = originalAudSrcPitch;
            zombieAudSrc.volume = originalAudSrcVolume;
            return false;
        }

        private void RadiusChanger()
        {
            if (playerState.CurrentPlayerStance == PlayerStance.Jump)
            {
                zombieDetectionRadius = jumpRadius;
            }
            else if (playerState.CurrentPlayerMoveState == PlayerMove.Sprint)
            {
                zombieDetectionRadius = sprintRadius;
            }
            else
            {
                zombieDetectionRadius = zombieOriginalDetectionRadius;
            }
        }

        private void Wander()
        {
            if (wanderType == WanderType.Random)
            {
                if (Vector3.Distance(transform.position, wanderPoint) < 2f)
                {
                    wanderPoint = RandomWanderPoint();
                }

                agent.SetDestination(wanderPoint);
            }
            else
            {
                if (waypoints.Length >= 2)
                {

                    if (Vector3.Distance(waypoints[waypointIndex].position, transform.position) < 2f)
                    {
                        if (waypointIndex == waypoints.Length - 1)
                        {
                            waypointIndex = 0;
                        }
                        else
                        {
                            waypointIndex++;
                        }
                    }
                    else
                    {
                        agent.SetDestination(waypoints[waypointIndex].position);
                    }
                }
                else
                {
                    #if UNITY_EDITOR
                    Debug.LogWarning("Please assign more than 1 waypoint to the AI" + gameObject.name);
                    #endif
                }
            }
        }

        private Vector3 RandomWanderPoint()
        {
            Vector3 newWanderPosition = GetRandomPosition();
            NavMeshPath path = TestPathValidity(newWanderPosition);
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                return newWanderPosition;
            }

            return RandomWanderPoint(); // Continue until we find a valid path.
        }

        private Vector3 GetRandomPosition()
        {
            Vector3 randomPoint = (Random.insideUnitSphere * wanderRadius) + transform.position;
            NavMesh.SamplePosition(randomPoint, out NavMeshHit navHit, wanderRadius, NavMesh.AllAreas);
            Vector3 newPosition = new Vector3(navHit.position.x, transform.position.y, navHit.position.z);
            return newPosition;
        }

        private NavMeshPath TestPathValidity(Vector3 newPosition)
        {
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, newPosition, NavMesh.AllAreas, path);
            return path;
        }

        public override void DeathEvent() 
            => Destroy(gameObject);

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (target != null)
            {
                Gizmos.DrawWireSphere(transform.position, zombieDetectionRadius);
                float halfFov = fov / 2f;
                Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFov, Vector3.up);
                Quaternion rightRayRotation = Quaternion.AngleAxis(halfFov, Vector3.up);
                Vector3 leftRayDirection = leftRayRotation * transform.forward;
                Vector3 rightRayDirection = rightRayRotation * transform.forward;
                Gizmos.DrawRay(transform.position, leftRayDirection * viewDistance);
                Gizmos.DrawRay(transform.position, rightRayDirection * viewDistance);
                Gizmos.DrawRay(transform.position, (target.transform.position - transform.position).normalized 
                    * viewDistance * radiusViewDistanceMultiplier);
            }
        }
        #endif
    }
}