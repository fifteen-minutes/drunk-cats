// https://github.com/yasirkula/UnityIngameDebugConsole/issues/77
// Disable Canvas Scaler in the IngameDebugConsole object/prefab and add the following script to it.

using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(1)]
public class IngameDebugConsoleFix : MonoBehaviour
{
    private void Start()
    {
        GetComponent<CanvasScaler>().enabled = true;
    }
}
