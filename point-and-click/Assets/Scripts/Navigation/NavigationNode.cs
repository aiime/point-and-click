using UnityEngine;
using Satsuma;

namespace Navigation
{
    // Этот компонент предназначен для "маркировки" игровых объектов, являющимися навигационными точками.
    // Часть фасада над библиотекой Satsuma.

    public class NavigationNode : MonoBehaviour
    { 
        public Node Node;
    }
}