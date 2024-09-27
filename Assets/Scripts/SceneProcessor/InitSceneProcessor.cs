using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameCore
{
    public class InitSceneProcessor : MonoBehaviour
    {
        private void Start()
        {
            SceneManager.LoadScene("GameScene", LoadSceneMode.Additive);
        }
    }
}