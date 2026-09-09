using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Fox_Save : MonoBehaviour
{
    [SerializeField] TMP_Text saveWarning;
    // Zapisywanie pozycji postaci gracza
    public void SavePosition(Vector3 playerPos)
    {
        // Zapisywanie pozycji postaci gracza na wszystkich osiach w ró¿nych miejscach 
        PlayerPrefs.SetFloat("posX", playerPos.x);
        PlayerPrefs.SetFloat("posY", playerPos.y);
        PlayerPrefs.SetFloat("posZ", playerPos.z);
        // Zapis danych
        PlayerPrefs.Save();
        saveWarning.text = "The save was successful!";
        Invoke("DeleteText", 2f);
    }

    private void DeleteText()
    {
        saveWarning.text = "";
    }

    private void OnTriggerEnter(Collider other)
    {
        // Jeœli spust portalu przekroczy³ obiekt z tagiem Gracza, wtedy
        if (other.CompareTag("Player"))
        {
            // Pobieranie pozycji obiektu i przekazanie jej do metody SavePosition
            Vector3 pos = other.transform.position;
            SavePosition(pos);
        }
    }
}
