using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Класс SceneLoader представляет загрузчик сцен.
/// </summary>
/// <remarks>
/// Он используется для асинхронной загрузки сцен в Unity и обратного вызова, когда загрузка завершена.
/// </remarks>
public class SceneLoader
{
  private readonly ICoroutineRunner _coroutineRunner;

  public SceneLoader(ICoroutineRunner coroutineRunner) => 
    _coroutineRunner = coroutineRunner;

  public void Load(string name, Action onLoaded = null) =>
    _coroutineRunner.StartCoroutine(LoadScene(name, onLoaded));

  public IEnumerator LoadScene(string nextScene, Action onLoaded = null)
  {
    if (SceneManager.GetActiveScene().name == nextScene)
    {
      onLoaded?.Invoke();
      yield break;
    }

    AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(nextScene);

    while (!waitNextScene.isDone)
    yield return null;

    onLoaded?.Invoke();
  }
}
