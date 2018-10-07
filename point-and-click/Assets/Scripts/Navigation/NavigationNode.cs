using UnityEngine;
using Satsuma;

namespace Navigation
{
    // Этот компонент предназначен для "маркировки" игровых объектов, являющимися навигационными точками.
    // Часть фасада над библиотекой Satsuma.

    public class NavigationNode : MonoBehaviour
    { 
        public Node Node;
        //[SerializeField] private NavigationGraph navigationGraph;

        private void Awake()
        {
            //Node = new Node(transform.position);
            //if (navigationGraph.Graph == null) navigationGraph.Graph = new CustomGraph();
            //
            //Node = navigationGraph.Graph.AddNode(transform.position);
        }
    }
}