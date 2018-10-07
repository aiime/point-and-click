using UnityEngine;
using UnityEditor;

namespace Service.NamesOverObjects
{
    // GUI и внутренняя логика компонента NamesOverObjects.

    [CustomEditor(typeof(NamesOverObjects))]
    public class NamesOverObjectsToggle : Editor
    {
        private static string gameObjectsTag;
        private static bool new_showNames = true;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            bool old_showNames = new_showNames;
            new_showNames = GUILayout.Toggle(new_showNames, "Show object names");
            if (TogglePressed(old_showNames, new_showNames))
            {
                // Насчёт следующих двух строчек.
                // Методу ShowNameOverGameObject (он ниже) для отрисовки имён объектов в gizmo необходимо быть
                // статичным, поэтому он не может напрямую брать тег объектов для отрисовки из компонента
                // NamesOverObjects. Чтобы обойти это, было создано статичное поле gameObjectsTag (внутри этого
                // класса), в которое при включении тогла копируется значение поля GameObjectTag из компонента
                // NamesOverObjects. В дальнейшем, когда начнёт работать метод ShowNameOverGameObject, то значение
                // тега он будет брать именно из этого статичного поля.
                NamesOverObjects namesOverObjects = target as NamesOverObjects;
                gameObjectsTag = namesOverObjects.GameObjectTag;

                SceneView.RepaintAll();
            }
        }

        [DrawGizmo(GizmoType.NotInSelectionHierarchy)]
        public static void ShowNameOverGameObject(Transform objectTransform, GizmoType gizmoType)
        {
            if (new_showNames == false) return;

            if (objectTransform.tag == gameObjectsTag)
            {
                // Проверяем, есть ли у объекта рендерер.
                Renderer renderer = objectTransform.gameObject.GetComponent<Renderer>();
                if (renderer == null) return;

                // Далее необходима проверка на то, что объект находится в поле зрения камеры, дабы
                // предотварить отображения имени, когда камера не видит объект.
                if (RendererVisibleFromCamera(renderer, Camera.current))
                {
                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    style.normal.textColor = new Color32(1, 1, 1, 255);

                    Handles.Label(objectTransform.position, objectTransform.name, style);
                }
            }
        }

        // Метод взят отсюда: http://wiki.unity3d.com/index.php/IsVisibleFrom
        private static bool RendererVisibleFromCamera(Renderer renderer, Camera camera)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
        }

        // Проверяет по старому и новому значению тогла, был ли он нажат.
        private bool TogglePressed(bool old_showNames, bool new_showNames)
        {
            if (old_showNames != new_showNames)
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