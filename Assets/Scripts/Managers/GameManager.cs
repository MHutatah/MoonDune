using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject flag;
    public AudioSource mainMusic;
    public AudioClip winningsfxclip;

    public void GameOver()
    {
        SceneManager.LoadScene("DeathScene");
        Debug.Log("Game Over!");
    }
    public void Victory()
    {
        flag.SetActive(true);
        mainMusic.PlayOneShot(winningsfxclip);
        //winningsfx.Play();
        //WaitUntil wait = new WaitUntil(() => !winningsfx.isPlaying);
        SceneManager.LoadScene("WinningScene");
        Debug.Log("Victory!");
    }
}


