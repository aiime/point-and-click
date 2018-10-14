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
        private const float MINIMUM_REACH_DISTANCE = 0.1f;

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

                while (HeroNotCloseEnoughToNode(heroDragPoint, path[i]))
                {
                    ChangeHeroPosition(heroDragPoint, navigationAgent);
                    ChangeHeroRotation(heroDragPoint, navigationAgent);
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

        private void ChangeHeroPosition(Transform heroDragPoint, NavigationAgent heroNavigationAgent)
        {
            heroDragPoint.position = Vector3.MoveTowards(heroDragPoint.position,
                                                         heroNavigationAgent.FrontNode.Coordinate,
                                                         Time.deltaTime * heroMovementSpeed);
        }

        private void ChangeHeroRotation(Transform heroDragPoint, NavigationAgent heroNavigationAgent)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(heroNavigationAgent.FrontNode.Coordinate - heroDragPoint.position);

            // Обнуляем x-компоненту, дабы модель героя не наклонялась на горках.
            targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, targetRotation.eulerAngles.z);

            heroDragPoint.rotation = Quaternion.RotateTowards(heroDragPoint.rotation,
                                                              targetRotation,
                                                              Time.deltaTime * heroRotationSpeed);
        }

        private bool HeroNotCloseEnoughToNode(Transform heroPosition, Node targetNode)
        {
            if (Vector3.Distance(heroDragPoint.position, targetNode.Coordinate) > MINIMUM_REACH_DISTANCE)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}