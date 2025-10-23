using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PyramidBakeSettings", menuName = "Customs/PBS")]
public class PyramidBakeSettings : ScriptableObject
{
    public Mesh sourceMesh;
    public int subMeshIndex;
    public Vector3 scale;
    public Vector3 rotation;
    public float height;
}
