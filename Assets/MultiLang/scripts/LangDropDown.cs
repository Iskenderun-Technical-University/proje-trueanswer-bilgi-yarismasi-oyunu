using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LangDropDown : MonoBehaviour
{
    //[SerializeField] private string[] myLangs;
    private Dropdown drp;
    //private int index;

    private void Awake()
    {
        drp = this.GetComponent<Dropdown>();
        //int v = PlayerPrefs.GetInt("_language_index", 0);
        //drp.value = v;
        string defLang = PlayerPrefs.GetString(GameRecords.SaveDefaultLanguage);

        drp.value = (defLang == "tr") ? 0 : (defLang == "en") ? 1 : 
					(defLang == "de") ? 2 : (defLang == "sp") ? 3 : (defLang == "fr") ? 4 : 1;

        drp.onValueChanged.AddListener(delegate
        {
            int index = drp.value;
			string str = (index == 0) ? "tr" : (index == 1) ? "en" : 
						(index == 2) ? "de" : (index == 3) ? "sp" : (index == 4) ? "fr" : "en";

            PlayerPrefs.SetString(GameRecords.SaveDefaultLanguage, str);
            //PlayerPrefs.SetInt("_language_index", index);
            //PlayerPrefs.SetString("_language", myLangs[index]);
            Debug.Log("language changed to " +str);
            ApplyLanguageChanges();
        });
    }

    private void ApplyLanguageChanges()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDestroy()
    {
        drp.onValueChanged.RemoveAllListeners();
    }
}
