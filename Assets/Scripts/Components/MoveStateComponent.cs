using System;
using UnityEngine;

namespace SampleProject {
    [Serializable]
    public struct MoveStateComponent
    {
        public bool moveRequired;
        public Vector3 direction;
    }
}
