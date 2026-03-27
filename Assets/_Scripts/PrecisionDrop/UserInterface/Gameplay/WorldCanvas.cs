using System;
using UnityEngine;

namespace PrecisionDrop.UserInterface.Gameplay {
    [RequireComponent(typeof(Canvas))]
    public class WorldCanvas : MonoBehaviour {
        private void Start() {
            var canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            Camera mainCam = Camera.main;
            if (!mainCam) { throw new NullReferenceException($"[{nameof(WorldCanvas)}] Could not find main camera through 'Camera.main'"); }
            canvas.worldCamera = mainCam;
        }
    }
}