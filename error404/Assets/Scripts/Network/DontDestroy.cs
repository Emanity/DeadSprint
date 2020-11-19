using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*This class will prevent an object to be destroyed
 * when loading to other scenes
 */
public class DontDestroy : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        Scene currentScene = SceneManager.GetSceneByBuildIndex(0);
        if (currentScene.buildIndex == 0)
        {
            //when scene returned to menu then destroy all objects that have been kept
            Destroy(this);
        }
    }
}
