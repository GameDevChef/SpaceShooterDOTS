using Unity.Entities;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    [HideInInspector]
    public float gravityConstant;
    public float simulationScale;
    public Entity playerEntity;

    private void Awake()
    {
        Cursor.visible = false;
        gravityConstant = 6.67408f * Mathf.Pow(10, -11);
        instance = this;
    }
}
