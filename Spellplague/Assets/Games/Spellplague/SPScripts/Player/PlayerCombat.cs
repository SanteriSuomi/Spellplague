using Spellplague.Characters;
using Spellplague.Utility;
using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Spellplague.Player
{
#pragma warning disable S3168 // Fire and forget method does not need to return task
    public class PlayerCombat : MonoBehaviour
    {
        [SerializeField]
        private InputSystemVariable inputSystem = default;
        [SerializeField]
        private PlayerStateVariable playerState = default;
        [SerializeField]
        private Transform meleeAreaBase = default;
        [SerializeField]
        private Transform weaponParent = default;
        [SerializeField]
        private TextMeshProUGUI damagePopupPrefab = default;
        [SerializeField]
        private RectTransform damagePopupParent = default;
        private Stopwatch damageTimer;
        [SerializeField]
        private LayerMask hitLayerMask = default;
        [SerializeField]
        private Vector2 damageRandomMultiplierRange = new Vector2(0.7f, 1.3f);

        [SerializeField]
        private float hitCooldownSeconds = 0.425f;
        [SerializeField]
        private float hitSphereRadius = 0.45f;
        [SerializeField]
        private float playerDamage = 21;
        [SerializeField]
        private float backstabDotProductMin = 0.4f;
        [SerializeField]
        private float backstabDamageMultiplier = 5;
        [SerializeField]
        private float popupMoveVerticalOffset = 90;
        [SerializeField]
        private float popupMoveSpeed = 7.5f;
        [SerializeField]
        private float popupPositionMinimum = 200;
        [SerializeField]
        private float stabAnimationLength = 0.2f;
        [SerializeField]
        private float stabAnimationSpeedMultiplier = 3f;
        [SerializeField]
        private float damagePopupDestroyTime = 2.5f;

        private bool runTasks;

        private void Awake() => damageTimer = new Stopwatch();

        private void OnEnable()
        {
            runTasks = true;
            inputSystem.Value.Player.Hit.Enable();
            inputSystem.Value.Player.Hit.performed += HitPerformed;
            damageTimer.Start();
        }

        private void HitPerformed(InputAction.CallbackContext callback)
        {
            if (damageTimer.ElapsedMilliseconds < hitCooldownSeconds * 1000
                || playerState.CurrentInventoryState != InventoryState.Closed) { return; }
            damageTimer.Restart();

            playerState.CurrentPlayerCombatState = PlayerCombatState.Attacking;

            WeaponStabAnimation();
            Collider[] hits = Physics.OverlapSphere(meleeAreaBase.position, hitSphereRadius, hitLayerMask);
            if (hits.Length > 0)
            {
                CheckHits(hits);
            }

            StartCoroutine(PlayerCombatStateOffDelay());
        }

        private void CheckHits(Collider[] hits)
        {
            playerState.CurrentPlayerCombatState = PlayerCombatState.Hit;
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].TryGetComponent(out IDamageable enemy))
                {
                    if (playerState.CurrentPlayerStance == PlayerStance.Crouch
                        && Vector3.Dot(transform.forward, enemy.GetTransform().forward) >= backstabDotProductMin)
                    {
                        Damage(enemy, playerDamage * backstabDamageMultiplier);
                    }
                    else
                    {
                        Damage(enemy, playerDamage);
                    }
                }
            }

            void Damage(IDamageable enemy, float amount)
            {
                float randomDamageMultiplier = Random.Range(damageRandomMultiplierRange.x, damageRandomMultiplierRange.y);
                enemy.TakeDamage(amount * randomDamageMultiplier);
                DamagePopup(enemy, (int)(amount * randomDamageMultiplier));
            }
        }

        private IEnumerator PlayerCombatStateOffDelay()
        {
            int index = 0;
            while (index < 2) // Wait for 2 frames to make sure the sound has time to get played.
            {
                index++;
                yield return null;
            }

            playerState.CurrentPlayerCombatState = PlayerCombatState.None;
        }

        private async void WeaponStabAnimation()
        {
            Vector3 original = weaponParent.localPosition;
            Vector3 goal = weaponParent.localPosition + new Vector3(0, 0, stabAnimationLength);
            while (runTasks && CheckPosition(weaponParent.localPosition, goal))
            {
                weaponParent.localPosition = Vector3.MoveTowards(weaponParent.localPosition, goal,
                    stabAnimationSpeedMultiplier * Time.deltaTime);
                await Task.Delay(TimeSpan.FromMilliseconds(Time.deltaTime * 1000));
            }

            while (runTasks && CheckPosition(weaponParent.localPosition, original))
            {
                weaponParent.localPosition = Vector3.MoveTowards(weaponParent.localPosition, original,
                    stabAnimationSpeedMultiplier * Time.deltaTime);
                await Task.Delay(TimeSpan.FromMilliseconds(Time.deltaTime * 1000));
            }
        }

        private void DamagePopup(IDamageable enemy, float damageAmount)
        {
            TextMeshProUGUI damagePopup = Instantiate(damagePopupPrefab, damagePopupParent);
            Invoke(nameof(DestroyPopup), damagePopupDestroyTime); // Make sure popup gets destroyed eventually.
            damagePopup.text = $"{enemy.GetName()} -{damageAmount}";
            damagePopup.rectTransform.position = new Vector2(Random.Range(popupPositionMinimum, Screen.width),
                Random.Range(popupPositionMinimum, Screen.height - popupPositionMinimum));
            damagePopup.gameObject.SetActive(true);
            MovePopup(damagePopup.rectTransform, damagePopup.rectTransform.position
                + new Vector3(0, popupMoveVerticalOffset, 0));
        }

        private async void MovePopup(RectTransform popup, Vector2 towards)
        {
            while (runTasks && CheckPosition(new Vector2(popup.position.x, popup.position.y), towards))
            {
                popup.position = Vector2.MoveTowards(popup.position, towards, popupMoveSpeed * Time.deltaTime);
                await Task.Delay(TimeSpan.FromMilliseconds(Time.deltaTime * 1000));
            }

            DestroyPopup(popup.gameObject);
        }

        private void DestroyPopup(GameObject popup)
        {
            if (popup != null)
            {
                Destroy(popup.gameObject);
            }
        }

        private bool CheckPosition(Vector2 position, Vector2 goal)
        {
            return !(Mathf.Approximately(position.x, goal.x)
                && Mathf.Approximately(position.y, goal.y));
        }

        private bool CheckPosition(Vector3 position, Vector3 goal)
        {
            return !(Mathf.Approximately(position.x, goal.x)
                && Mathf.Approximately(position.y, goal.y)
                && Mathf.Approximately(position.z, goal.z));
        }

        private void OnDisable()
        {
            runTasks = false;
            inputSystem.Value.Player.Hit.Disable();
            inputSystem.Value.Player.Hit.performed -= HitPerformed;
            CancelInvoke();
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos() 
            => Gizmos.DrawWireSphere(meleeAreaBase.position, hitSphereRadius);
        #endif
    }
}