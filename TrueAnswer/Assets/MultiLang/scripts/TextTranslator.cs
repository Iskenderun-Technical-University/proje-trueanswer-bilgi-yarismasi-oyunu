using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextTranslator : MonoBehaviour
{
    [SerializeField] private string key = null;

    public static TextTranslator Instance = null;

    private void Awake() { if (Instance == null) Instance = this; }

    private void Start() { GetComponent<Text>().text = GameMultiLang.GetTraduction(key); }
}
