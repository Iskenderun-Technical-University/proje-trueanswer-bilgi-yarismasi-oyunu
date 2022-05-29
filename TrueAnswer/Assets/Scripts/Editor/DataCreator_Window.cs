using UnityEditor;
using UnityEngine;
using System.IO;

public class DataCreator_Window : EditorWindow
{
    #region Variables

    [SerializeField] Data data = new Data();
    [SerializeField] string path = string.Empty;

    private Vector2 scrollPosition = Vector2.zero;

    private SerializedObject serializedObject = null;
    private SerializedProperty questionsProp = null;

    #endregion

    private void OnEnable()
    {
        serializedObject = new SerializedObject(this);
        data.Sorular = new Soru[0];
        questionsProp = serializedObject.FindProperty("data").FindPropertyRelative("Sorular");
    }

    [MenuItem("Oyun/Soru Oluşturucu")]
    public static void OpenWindow()
    {
        var window = EditorWindow.GetWindow<DataCreator_Window>("Oluşturucu");

        window.minSize = new Vector2(510.0f, 344.0f);
        window.Show();
    }

    private void OnGUI()
    {
        #region Header Section

        Rect headerRect = new Rect(15, 15, this.position.width - 30, 65);
        GUI.Box(headerRect, GUIContent.none);

        GUIStyle headerStyle = new GUIStyle(EditorStyles.largeLabel)
        {
            fontSize = 26,
            alignment = TextAnchor.UpperLeft
        };
        headerRect.x += 5;
        headerRect.width -= 10;
        headerRect.y += 5;
        headerRect.height -= 10;
        GUI.Label(headerRect, "XML verisi oluşturma", headerStyle);

        Rect summaryRect = new Rect(headerRect.x + 25, (headerRect.y + headerRect.height) - 20, headerRect.width - 50, 15);
        GUI.Label(summaryRect, "XML dosyasına eklenmesi gereken verileri oluşturun");

        #endregion

        #region Body Section

        Rect bodyRect = new Rect(15, (headerRect.y + headerRect.height) + 20, this.position.width - 30, this.position.height - (headerRect.y + headerRect.height) - 80);
        GUI.Box(bodyRect, GUIContent.none);

        var arraySize = data.Sorular.Length;
        
        Rect viewRect = new Rect(bodyRect.x + 10, bodyRect.y + 10, bodyRect.width - 20, EditorGUI.GetPropertyHeight(questionsProp));
        Rect scrollPosRect = new Rect(viewRect)
        {
            height = bodyRect.height - 20
        };
        scrollPosition = GUI.BeginScrollView(scrollPosRect, scrollPosition, viewRect, false, false, GUIStyle.none, GUI.skin.verticalScrollbar);

        var drawSlider = viewRect.height > scrollPosRect.height;

        Rect propertyRect = new Rect(bodyRect.x + 10, bodyRect.y + 10, bodyRect.width - (drawSlider ? 40 : 20), 17);
        EditorGUI.PropertyField(propertyRect, questionsProp, true);

        serializedObject.ApplyModifiedProperties();        

        GUI.EndScrollView();

        #endregion

        #region Navigation

        Rect buttonRect = new Rect(bodyRect.x + bodyRect.width - 85, bodyRect.y + bodyRect.height + 15, 85, 30);
        bool pressed = GUI.Button(buttonRect, string.IsNullOrEmpty(path) ? "Oluştur" : "Kaydet", EditorStyles.miniButtonRight);
        if (pressed)
        {
            if (string.IsNullOrEmpty(path) == true)
            {
                path = EditorUtility.SaveFilePanel("Create", "Asset", GameRecords.FileName, "xml");
                
            }
            if (string.IsNullOrEmpty(path) == false)
            {
                Data.Write(data, path);
                AssetDatabase.Refresh();
            }
        }
        buttonRect.x -= buttonRect.width;
        pressed = GUI.Button(buttonRect, "Aç", EditorStyles.miniButtonLeft);
        if (pressed)
        {
            path = EditorUtility.OpenFilePanel("Select", "Assets/Resources/Sorular", "xml");
            if (string.IsNullOrEmpty(path) == false)
            {
                var path2 = Path.Combine(GameRecords.FileDir, Path.GetFileNameWithoutExtension(path));
                var d = Data.Fetch(out bool result, path2);
                if (result)
                {
                    data = d;
                }
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }
        }
        buttonRect.x = bodyRect.x;
        pressed = GUI.Button(buttonRect, "Yeni", EditorStyles.miniButton);
        if (pressed)
        {
            path = string.Empty;
            data = new Data
            {
                Sorular = new Soru[0]
            };
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        #endregion
    }
}