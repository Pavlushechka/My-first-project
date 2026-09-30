using UnityEngine;
using UnityEngine.AI;

public class NPCAnimator : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public NavMeshAgent navAgent;

    [Header("Settings")]
    public float walkSpeedThreshold = 0.1f;

    void Update()
    {
        if (animator == null || navAgent == null) return;

        // Скорость NavMeshAgent
        float speed = navAgent.velocity.magnitude;

        // Передаём в Animator
        animator.SetFloat("Speed", speed);
    }
}