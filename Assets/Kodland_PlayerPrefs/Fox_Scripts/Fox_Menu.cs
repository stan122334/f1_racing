using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fox_Menu : MonoBehaviour
{
    // Referencja do przycisku Wczytaj Grê
    [SerializeField] Button przyciskWczytaj;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        // Sprawdzanie, czy mamy gniazdo zapisu. Jeœli tak, aktywujemy przycisk Wczytaj
        if (PlayerPrefs.HasKey("posX"))
        {
            przyciskWczytaj.interactable = true;
        }
    }
    // Funkcja nowej gry
    public void RozpocznijNowaGre()
    {
        // Sprawdzanie, czy mamy gniazdo zapisu. Jeœli tak, czyœcimy wszystkie gniazda zapisu i rozpoczynamy now¹ grê
        if (PlayerPrefs.HasKey("posX"))
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene("Game");
        }
        // W przeciwnym razie po prostu rozpoczynamy grê
        else
        {
            SceneManager.LoadScene("Game");
        }
    }
    // Funkcja, która uruchamia grê z uwzglêdnieniem wszystkich zapisanych danych
    public void WczytajGre()
    {
        // Uruchamiamy grê, jeœli mamy jakiekolwiek gniazda zapisu
        if (PlayerPrefs.HasKey("posX"))
        {
            SceneManager.LoadScene("Game");
        }
    }
}
