using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameObject player;
    public NavMeshAgent enemyNavMeshAgent;

    private Vector3 startPos;
    public float playerDistance;

    private State currentState;

    enum State
    {
        idle, // start position
        chasing, // chasing player
        returning, // transition between chasing and idle
        caughtPlayer // when agent reaches player
    }

    private void Start()
    {
        startPos = gameObject.transform.position;
        currentState = State.idle;
    }

    private void Update()
    {
        playerDistance = Vector3.Distance(player.transform.position, gameObject.transform.position);

        switch(currentState)
        {
            case State.idle:
                if (playerDistance < 10f) currentState = State.chasing; // Chases player if the distance is less than 10
                break;

            case State.chasing:
                enemyNavMeshAgent.SetDestination(player.transform.position);
                if (playerDistance > 15f) currentState = State.returning; // loses player when distance is greater than 15
                if (playerDistance <= 1.5f) currentState = State.caughtPlayer;
                break;

            case State.returning:
                Debug.Log("LOST PLAYER");
                enemyNavMeshAgent.SetDestination(startPos);
                if (playerDistance < 10f) currentState = State.chasing;
                else if (enemyNavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete) currentState = State.idle;
                break;

            case State.caughtPlayer:
                Debug.Log("Reached Player");
                enemyNavMeshAgent.isStopped = true;
                break;
        }
    }
}
