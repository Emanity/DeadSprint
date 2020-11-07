using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This class will prevent an object to be destroyed
 * when loading to other scenes
 */
public class DontDestroy : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
