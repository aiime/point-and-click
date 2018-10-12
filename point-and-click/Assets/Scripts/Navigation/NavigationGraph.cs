using UnityEngine;
using Satsuma;

namespace Navigation
{
    // Этот компонент предназначен для создания навигационного графа уровня и его дальнейшего хранения.
    // Часть фасада над библиотекой Satsuma.

    public class NavigationGraph : MonoBehaviour
    {
        public NavigationNode[] NavigationNodes;
        public NavigationNode[] NavigationTransitions;

        [HideInInspector] public CustomGraph Graph = new CustomGraph();

        private void Awake()
        {
            CreateGraph(NavigationNodes);
            CreateGraphArcs(NavigationTransitions);
        }

        private void CreateGraph(NavigationNode[] navigationNodes)
        {
            Graph = new CustomGraph();
        
            foreach (NavigationNode navigationNode in navigationNodes)
            {
                navigationNode.Node = Graph.AddNode(navigationNode.transform.position);
            } 
        }

        private void CreateGraphArcs(NavigationNode[] navigationTransitions)
        {
            for (int i = 0; i < navigationTransitions.Length; i += 2)
            {
                Graph.AddArc(navigationTransitions[i].Node,
                             navigationTransitions[i + 1].Node,
                             Directedness.Undirected);
            }
        }
    }
}