using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Satsuma;
using Service;

namespace Navigation
{
    // Компонент предназначен для перемещения героя по навигационному графу с помощью мыши.

    public class PointAndClickController : MonoBehaviour
    {
        // Поля для инспектора.
        [SerializeField] private Camera mainCamera;
        [SerializeField] private NavigationGraph navigationGraph;
        [SerializeField] private NavigationAgent navigationAgent;
        [SerializeField] private Transform heroDragPoint;
        [SerializeField] private float heroMovementSpeed; // 4 - обычная скорость перемещения
        [SerializeField] private float heroRotationSpeed; // 400 - обычная скорость вращения

        // Публичные поля.
        public Action MovementStarted;
        public Action MovementStopped;
        
        // Приватные поля.
        private Coroutine movementCoroutine;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject.tag == "NavigationNode")
                    {
                        StopMovement();

                        NavigationNode newDestination = hit.transform.gameObject.GetComponent<NavigationNode>();
                        List<Node> newPath = Navigator.FindPath(navigationGraph, 
                                                                navigationAgent, 
                                                                newDestination);

                        movementCoroutine = StartCoroutine(MoveHero(newPath));
                    }
                }
            }
        }

        private IEnumerator MoveHero(List<Node> path)
        {
            MovementStarted.SafeInvoke();

            for (int i = 0; i < path.Count; i++)
            {
                navigationAgent.FrontNode = path[i];

                while (Vector3.Distance(heroDragPoint.position, path[i].Coordinate) > 0.1f)
                {
                    heroDragPoint.position = Vector3.MoveTowards(heroDragPoint.position,
                                                                 navigationAgent.FrontNode.Coordinate, 
                                                                 Time.deltaTime * heroMovementSpeed);

                    Quaternion targetRotationQ = 
                        Quaternion.LookRotation(navigationAgent.FrontNode.Coordinate - heroDragPoint.position);

                    targetRotationQ = Quaternion.Euler(0, 
                                                       targetRotationQ.eulerAngles.y, 
                                                       targetRotationQ.eulerAngles.z);

                    heroDragPoint.rotation = 
                        Quaternion.RotateTowards(heroDragPoint.rotation, 
                                                 targetRotationQ, 
                                                 Time.deltaTime * heroRotationSpeed);

                    yield return null;
                }

                navigationAgent.RareNode = path[i];
            }

            MovementStopped.SafeInvoke();
        }

        private void StopMovement()
        {
            if (movementCoroutine != null)
            {
                StopCoroutine(movementCoroutine);
                MovementStopped.SafeInvoke();
            }
        }
    }
}