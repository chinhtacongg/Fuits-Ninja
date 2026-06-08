
using UnityEngine;

public class ParticlePool : MonoBehaviour
{
    public GameObject[] particlePrefabs;
    public GameObject[] floatPointPrefabs;
    
   

    public void PlayGameOBPool(Vector3 pos, GameObject[] gameObjectsPrefabs)
    {
        for (int i = 0; i < gameObjectsPrefabs.Length; i++)
        {
            if (!gameObjectsPrefabs[i].activeInHierarchy)
            {
                gameObjectsPrefabs[i].transform.position = pos;
                gameObjectsPrefabs[i].SetActive (true);
                return;
            }
        }
    }


}
