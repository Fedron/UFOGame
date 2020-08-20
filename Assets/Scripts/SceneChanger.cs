using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour {
    public static SceneChanger Instance = null;
    [SerializeField] Animator animator = default;
    [SerializeField] float animationLength = 0.2f;

    private void Awake() {
        Instance = this;
    }

    public void ChangeScene(int index) {
        StartCoroutine(Transition(index));
    }

    private IEnumerator Transition(int index) {
        animator.SetTrigger("Out");
        yield return new WaitForSeconds(animationLength);
        SceneManager.LoadScene(index);
    }
}
