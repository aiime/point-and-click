using System.Collections.Generic;
using UnityEngine;
using Satsuma;

namespace Navigation
{
    // Класс создан для поиска кратчайшего пути в навигационном графе.
    // Часть фасада над библиотекой Satsuma.

    public static class Navigator
    {
        // Публичные методы
        public static List<Node> FindPath(NavigationGraph navigationGraph,
                                          NavigationAgent navigationAgent,
                                          NavigationNode destination)
        {
            //foreach (Node n in navigationGraph.Graph.Nodes())
            //{
            //    Debug.Log(n.Coordinate);
            //}
            Node temporaryPathOriginNode = navigationGraph.Graph.AddNode(navigationAgent.transform.position);

            Arc  temporaryBackArc = navigationGraph.Graph.AddArc(temporaryPathOriginNode,
                                                                 navigationAgent.RareNode,
                                                                 Directedness.Undirected);

            Arc  temporaryForwardArc = navigationGraph.Graph.AddArc(temporaryPathOriginNode,
                                                                    navigationAgent.FrontNode,
                                                                    Directedness.Undirected);

            IPath path = FindGraphPath(navigationGraph, navigationAgent, destination);

            List<Node> pathAsNodeList = PathToNodeList(path);

            DeleteTemporaryNodeAndArcs(navigationGraph, 
                                       temporaryPathOriginNode, 
                                       temporaryForwardArc, 
                                       temporaryBackArc);

            return pathAsNodeList;
        }


        // Приватные методы
        private static IPath FindGraphPath(NavigationGraph navigationGraph,
                                           NavigationAgent navigationAgent,
                                           NavigationNode destination)
        {
            Debug.Log("DEST:" + destination.Node.Coordinate);
            IPath pathBack = navigationGraph.Graph.FindPath(navigationAgent.RareNode,
                                                            destination.Node,
                                                            Dfs.Direction.Undirected);

            IPath pathForward = navigationGraph.Graph.FindPath(navigationAgent.FrontNode,
                                                               destination.Node,
                                                               Dfs.Direction.Undirected);
            
            IPath shortestPath = pathBack.NodeCount() > pathForward.NodeCount() ? pathForward : pathBack;

            return shortestPath;
        }


        private static List<Node> PathToNodeList(IPath path)
        {
            List<Node> pathAsNodeList = new List<Node>();

            foreach (Node node in path.Nodes())
            {
                pathAsNodeList.Add(node);
            }

            return pathAsNodeList;
        }


        private static void DeleteTemporaryNodeAndArcs(NavigationGraph navigationGraph, 
                                                       Node originNode, 
                                                       Arc forwardArc, 
                                                       Arc backArk)
        {
            navigationGraph.Graph.DeleteArc(backArk);
            navigationGraph.Graph.DeleteArc(forwardArc);
            navigationGraph.Graph.DeleteNode(originNode);
        }
    }
}