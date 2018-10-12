using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Navigation;

public class UnityChanAnimation : MonoBehaviour
{
    [SerializeField] private Animator unityChanAnimator;
    [SerializeField] private PointAndClickController pointAndClickController;

    private void Awake()
    {
        pointAndClickController.MovementStarted += OnMovementStarted;
        pointAndClickController.MovementStopped += OnMovementStopped;
    }

    private void OnMovementStarted()
    {
        unityChanAnimator.SetFloat("Speed", 0.4f);
    }

    private void OnMovementStopped()
    {
        unityChanAnimator.SetFloat("Speed", 0f);
    }
}