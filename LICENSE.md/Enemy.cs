﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
public class Enemy : MonoBehaviour {
    public float lookRadius = 10f;
    Transform target;
    NavMeshAgent agent;
	void Start ()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
	}
	void Update ()
    {
        float distance = Vector3.Distance(target.position, transform.position);
        if (distance <= lookRadius)
        {
            agent.SetDestination(target.position);
        }
	}
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
