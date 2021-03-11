using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameObject player;
    public GameObject model; // game object that represents enemy
    public ParticleSystem effects; // effects for when enemy reaches player
    public AudioSource soundEffect; // used for explosion sound when enemy reaches player
    public NavMeshAgent enemyNavMeshAgent;

    private Vector3 startPos;
    public float playerDistance;

    private int timesRespawnRun = 0; // changes when respawn function has run
    private State currentState;

    enum State
    {
        idle, // start position
        chasing, // chasing player
        returning, // transition between chasing and idle
        caughtPlayer, // when agent reaches player
        dead
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
                timesRespawnRun = 0;
                if (playerDistance < 10f) currentState = State.chasing; // Chases player if the distance is less than 10
                break;

            case State.chasing:
                enemyNavMeshAgent.SetDestination(player.transform.position);
                if (playerDistance > 15f) currentState = State.returning; // loses player when distance is greater than 15
                if (playerDistance <= 1.6f) currentState = State.caughtPlayer;
                break;

            case State.returning:
                enemyNavMeshAgent.SetDestination(startPos);
                if (playerDistance < 10f) currentState = State.chasing;
                else if (enemyNavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete) currentState = State.idle;
                break;

            case State.caughtPlayer:
                enemyNavMeshAgent.isStopped = true;
                model.SetActive(false);
                effects.Play();
                soundEffect.Play();
                currentState = State.dead;
                break;

            case State.dead:
                Invoke("Respawn", 1.5f); // respawns enemy after about 1.5 seconds
                break;
        }
    }

    private void Respawn()
    {
        timesRespawnRun++;
        if (timesRespawnRun > 1) return;
        gameObject.transform.position = startPos;
        currentState = State.idle;
        enemyNavMeshAgent.isStopped = false;
        model.SetActive(true);
    }
}
