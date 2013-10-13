using UnityEngine;
using System.Collections;

public class WireframeRenderer : MonoBehaviour {
    void OnPreRender() {
        GL.wireframe = true;
    }
    void OnPostRender() {
        GL.wireframe = false;
    }
}


