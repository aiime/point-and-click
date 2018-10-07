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
        [SerializeField] private Transform heroTransform;
        [SerializeField] private float heroMovementSpeed; // 4
        [SerializeField] private float heroRotationSpeed; // 400

        // Публичные поля.
        public Action MovementStarted;
        public Action MovementStoped;
        
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
                        //print(hit.transform.gameObject.name);
                        StopMovement();

                        NavigationNode newDestination = hit.transform.gameObject.GetComponent<NavigationNode>();
                        print(hit.transform.gameObject.name);
                        print(hit.transform.position);
                        print(hit.transform.gameObject.GetComponent<NavigationNode>().Node.Coordinate);
                        print("DEST_I:" + newDestination.Node.Coordinate);
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
            foreach (Node n in path)
            {
                print(n.Coordinate);
            }

            MovementStarted.SafeInvoke();
            //heroAnimator.SetFloat("Speed", 0.4f);

            for (int i = 0; i < path.Count; i++)
            {
                navigationAgent.FrontNode = path[i];

                while (Vector3.Distance(heroTransform.position, path[i].Coordinate) > 0.1f)
                {
                    heroTransform.position = Vector3.MoveTowards(heroTransform.position,
                                                                 navigationAgent.FrontNode.Coordinate, 
                                                                 Time.deltaTime * heroMovementSpeed);

                    Quaternion q = 
                        Quaternion.LookRotation(navigationAgent.FrontNode.Coordinate - heroTransform.position);

                    heroTransform.rotation = 
                        Quaternion.RotateTowards(heroTransform.rotation, q, Time.deltaTime * heroRotationSpeed);

                    //yield return new WaitForEndOfFrame();
                    yield return null;
                }

                navigationAgent.RareNode = path[i];
            }

            MovementStoped.SafeInvoke();
            //animator.SetFloat("Speed", 0);
        }

        private void StopMovement()
        {
            if (movementCoroutine != null)
            {
                StopCoroutine(movementCoroutine);
                MovementStoped.SafeInvoke();
                //heroAnimator.SetFloat("Speed", 0);
            }
        }
    }
}