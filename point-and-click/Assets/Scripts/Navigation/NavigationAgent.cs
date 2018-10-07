using UnityEngine;
using Satsuma;

namespace Navigation
{
    // Этот компонент необходимо прикрепить к игровому объекту, который будет осуществлять перемещение по
    // навигационном графу.
    // Часть фасада над библиотекой Satsuma.

    public class NavigationAgent : MonoBehaviour
    {
        public NavigationNode FrontNavigationNode;
        public NavigationNode RareNavigationNode;

        public Node FrontNode;
        public Node RareNode;    

        private void Awake()
        {
            FrontNode = FrontNavigationNode.Node;
            RareNode = RareNavigationNode.Node;
        }
    }
}