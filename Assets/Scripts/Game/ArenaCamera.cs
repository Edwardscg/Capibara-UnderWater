using UnityEngine;

namespace UnderwaterAdventure
{
    [RequireComponent(typeof(Camera))]
    public class ArenaCamera : MonoBehaviour
    {
        Camera view;
        void Awake() => view = GetComponent<Camera>();
        void LateUpdate()
        {
            // Encuadre 16:9 estable: las ruinas no se estiran al redimensionar Game.
            float windowAspect = (float)Screen.width / Mathf.Max(1, Screen.height);
            const float targetAspect = 16f / 9f;
            if (windowAspect > targetAspect)
            {
                float width = targetAspect / windowAspect;
                view.rect = new Rect((1 - width) * 0.5f, 0, width, 1);
            }
            else
            {
                float height = windowAspect / targetAspect;
                view.rect = new Rect(0, (1 - height) * 0.5f, 1, height);
            }
            view.orthographicSize = 8.2f;
        }
    }
}
