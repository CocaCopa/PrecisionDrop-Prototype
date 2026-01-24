using System;
using UnityEngine;

namespace PrecisionDrop.Input.Contracts {
    public interface IInputSource {
        /// <summary>True while the user is actively dragging/pressing.</summary>
        bool IsHolding { get; }
        /// <summary>Mouse drag delta while interacting.</summary>
        Vector2 MouseDragDelta { get; }
    }
}
