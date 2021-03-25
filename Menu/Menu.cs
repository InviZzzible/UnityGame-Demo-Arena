using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    private GameObject Canvas;
    private AudioSource clickBtn;

    // private int currentIndexPage = 0; // Текущий индекс текущей страницы в меню.

    // Start is called before the first frame update
    void Start()
    {
        Canvas = GameObject.Find("Canvas");
        clickBtn = GameObject.Find("ClickBtn").GetComponent<AudioSource>();

        // Создание стартового UI меню:

        // ... Перенести сюда создание нулевой (стартовой) страницы меню...

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private Vector4 ConvertFromRGBA255toRGBA01(float R, float G, float B, float A) {
        // Конвертируем представление цвета из диапазона до 255 в диапазон до 1.0:
        R /= 255;
        G /= 255;
        B /= 255;
        A /= 255;
        return new Vector4(R, G, B, A);
    }

    private void CreateTheInputField(string nameObj, float y) {
        //GameObject input = new GameObject(nameObj, typeof(InputField), typeof(LayoutElement));
        GameObject input = new GameObject();
        input.transform.SetParent(Canvas.transform);
        input.name = nameObj;
        RectTransform RT = input.AddComponent<RectTransform>();
        RT.localScale = new Vector3(1f, 1f, 1f);
        RT.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        RT.anchorMin = new Vector2(0.5f, 1f);
        RT.anchorMax = new Vector2(0.5f, 1f);
        RT.sizeDelta = new Vector2(300f, 40f); // Set width and height
        RT.localPosition = new Vector3(0f, 0f, 0f);
        RT.anchoredPosition = new Vector3(0f, y, 0f);
        
        InputField ifd = input.AddComponent<InputField>();
        ifd.transition = Selectable.Transition.ColorTint;


        GameObject border = new GameObject("Border", typeof(Image), typeof(LayoutElement));
        border.transform.SetParent(input.transform);
        RectTransform RTborder = border.GetComponent<RectTransform>();
        RTborder.localScale = new Vector3(1f, 1f, 1f);
        RTborder.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        RTborder.anchorMin = new Vector2(0.5f, 0.5f);
        RTborder.anchorMax = new Vector2(0.5f, 0.5f);
        RTborder.sizeDelta = new Vector2(300f, 40f); // Set width and height
        RTborder.localPosition = new Vector3(0f, 0f, 0f);
        RTborder.anchoredPosition = new Vector3(0f, 0f, 0f);

        Image imgBorder = border.GetComponent<Image>();
        imgBorder.sprite = Resources.Load<Sprite>("Square"); // Почему то не работает... ???
        Vector4 vColor = ConvertFromRGBA255toRGBA01(50f, 50f, 50f, 100f);
        imgBorder.color = new Color(vColor[0], vColor[1], vColor[2], vColor[3]);
        imgBorder.type = Image.Type.Tiled;


        GameObject placeholderObj = new GameObject();
        placeholderObj.transform.SetParent(input.transform);
        placeholderObj.name = "Placeholder";

        RectTransform rtPlaceholder = placeholderObj.AddComponent<RectTransform>();
        rtPlaceholder.localScale = new Vector3(1f, 1f, 1f);
        rtPlaceholder.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        rtPlaceholder.anchorMin = new Vector2(0f, 0.5f);
        rtPlaceholder.anchorMax = new Vector2(1f, 0.5f);
        rtPlaceholder.sizeDelta = new Vector2(200f, 40f);
        rtPlaceholder.localPosition = new Vector3(0f, 0f, 0f);
        rtPlaceholder.anchoredPosition = new Vector3(110f, -20f, 0f);

        Text phText = placeholderObj.AddComponent<Text>();
        if (input.name == "signupInputNickname") phText.text = "Input your nickname...";
        else {
            phText.text = "Pick a password...";
            ifd.contentType = InputField.ContentType.Password;
        }
        phText.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        ifd.placeholder = phText;
        placeholderObj.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        placeholderObj.GetComponent<Text>().fontStyle = FontStyle.Italic;
        vColor = ConvertFromRGBA255toRGBA01(100f, 100f, 100f, 150f);
        placeholderObj.GetComponent<Text>().color = new Color(vColor[0], vColor[1], vColor[2], vColor[3]);


        Graphic myGraphic = input.GetComponent<Graphic>();
        ifd.placeholder = myGraphic;
        ifd.transition = Selectable.Transition.None;
        
        GameObject textObj = new GameObject();
        textObj.name = "Text";
        textObj.transform.parent = input.transform;
        RectTransform textRT = textObj.AddComponent<RectTransform>();
        textRT.localScale = new Vector3(1f, 1f, 1f);
        textRT.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        textRT.anchorMin = new Vector2(0.5f, 0.5f);
        textRT.anchorMax = new Vector2(0.5f, 0.5f);
        textRT.sizeDelta = new Vector2(280f, 40f);
        textRT.localPosition = new Vector3(0f, 0f, 0f);
        textRT.anchoredPosition = new Vector3(0f, 0f, 0f);


        Text textscript = textObj.AddComponent<Text>();
        textscript.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        
        ifd.textComponent = textscript;
        textscript.supportRichText = false;
        textscript.resizeTextForBestFit = true;
        textscript.resizeTextMaxSize = 14;
        textscript.resizeTextMinSize = 10;

        ColorBlock cb = input.GetComponent<InputField>().colors;
        Vector4 v = ConvertFromRGBA255toRGBA01(140f, 140f, 140f, 255f);
        cb.normalColor = new Color(v[0], v[1], v[2], v[3]);
        input.GetComponent<InputField>().colors = cb;

        input.layer = 5;
        
        input.GetComponent<InputField>().onValueChanged.AddListener((string str) => {
            GameObject placeholder = input.transform.Find("Placeholder").gameObject;
            Text text = placeholder.GetComponent<Text>();
            if(str != "") {
                text.text = "";
                if (ifd.preferredWidth > 210f) { // Ограничение ввода по ширине border - а
                    ifd.text = str.Substring(0, str.Length - 1);
                }
            }
            else {
                if(input.gameObject.name == "signupInputNickname") text.text = "Input your nickname...";
                else text.text = "Pick a password...";
            }
        });

        if (nameObj == "signupInputNickname") {
            EventSystem.current.SetSelectedGameObject(input.gameObject, null); // Устанавливаем фокус.
        }
    }

    public void SignUp_Slot() { // Пользователь хочет создать нового персонажа

        clickBtn.Play();

        // Создание вложенного интерфейса:
        GameObject CreateNewCharacterBtn = GameObject.Find("CreateNewCharacter");
        GameObject IAlreadyHaveTheCharacterBtn = GameObject.Find("IAlreadyHaveTheCharacter");
        CreateNewCharacterBtn.SetActive(false);
        IAlreadyHaveTheCharacterBtn.SetActive(false);

        // Создание Заголовка:
        GameObject header = new GameObject("signupHeader", typeof(Text), typeof(LayoutElement));
        header.transform.SetParent(Canvas.transform);
        RectTransform rtHeader = header.GetComponent<RectTransform>();
        rtHeader.localScale = new Vector3(1f, 1f, 1f);
        rtHeader.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        rtHeader.anchorMin = new Vector2(0.5f, 1f);
        rtHeader.anchorMax = new Vector2(0.5f, 1f);
        rtHeader.sizeDelta = new Vector2(200f, 30f); // Set width and height
        rtHeader.localPosition = new Vector3(0f, 0f, 0f); // Обнуляем позиции по всем осям относительно анкеров.
        rtHeader.anchoredPosition = new Vector3(0f, -75f, 0f); // Устанавливаем позицию относительно анкеров.
        header.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        header.GetComponent<Text>().fontSize = 18;
        header.GetComponent<Text>().text = "Creating your character:";
        header.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        Outline outlineHeader = header.AddComponent<Outline>();
        outlineHeader.effectColor = new Color(255f, 255f, 255f, 100f);
        outlineHeader.effectDistance = new Vector2(0.2f, -0.2f);
        header.layer = 5;

        // Создание описания:
        GameObject description = new GameObject("signupDescription", typeof(Text), typeof(LayoutElement));
        description.transform.SetParent(Canvas.transform);
        RectTransform rtDescription = description.GetComponent<RectTransform>();
        rtDescription.localScale = new Vector3(1f, 1f, 1f);
        rtDescription.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        rtDescription.anchorMin = new Vector2(0.5f, 1f);
        rtDescription.anchorMax = new Vector2(0.5f, 1f);
        rtDescription.sizeDelta = new Vector2(300f, 20f); // Set width and height
        rtDescription.localPosition = new Vector3(0f, 0f, 0f);
        rtDescription.anchoredPosition = new Vector3(0f, -100f, 0f);
        description.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        description.GetComponent<Text>().fontStyle = FontStyle.Italic;
        description.GetComponent<Text>().fontSize = 10;
        description.GetComponent<Text>().text = "Come up with a nickname that will be unique in this game!";
        description.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        description.layer = 5;

        // Создаем поля Input Field - ы:
        CreateTheInputField("signupInputNickname", -160f); // 1й
        CreateTheInputField("signupInputPassword", -220f); // 2й

        // Создание кнопоки done:
        GameObject done = new GameObject("signupDone", typeof(Image), typeof(Button), typeof(LayoutElement));
        done.transform.SetParent(Canvas.transform);
        RectTransform doneRT = done.GetComponent<RectTransform>();
        doneRT.localScale = new Vector3(1f, 1f, 1f);
        doneRT.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        doneRT.sizeDelta = new Vector2(200f, 50f);
        doneRT.anchorMin = new Vector2(0.5f, 1f);
        doneRT.anchorMax = new Vector2(0.5f, 1f);
        doneRT.anchoredPosition = new Vector3(50f, -285f);
        done.GetComponent<Image>().color = new Color(85f / 255f, 114f / 255f, 50f / 255f, 255f / 255f);
        done.GetComponent<Button>().transition = Selectable.Transition.ColorTint;
        done.layer = 5;
        
        GameObject txtDone = new GameObject(); // Создание текста для кнопки.
        txtDone.name = "txtDone";
        txtDone.transform.SetParent(done.transform);
        RectTransform RTtxtDone = txtDone.AddComponent<RectTransform>();
        RTtxtDone.localScale = new Vector3(1f, 1f, 1f);
        RTtxtDone.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        RTtxtDone.sizeDelta = new Vector2(RTtxtDone.sizeDelta[0], 20f);
        RTtxtDone.anchorMin = new Vector2(0f, 0.5f);
        RTtxtDone.anchorMax = new Vector2(0f, 0.5f);
        RTtxtDone.anchoredPosition = new Vector3(100f, 0f);
        Text textDone = txtDone.AddComponent<Text>();
        textDone.text = "Done";
        textDone.color = new Color(0f, 0f, 0f, 1f);
        textDone.fontSize = 18;
        textDone.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        textDone.alignment = TextAnchor.MiddleCenter;
        txtDone.layer = 5;

        // Создание кнопоки back:
        GameObject back = new GameObject("signupBack", typeof(Image), typeof(Button), typeof(LayoutElement));
        back.transform.SetParent(Canvas.transform);
        RectTransform backRT = back.GetComponent<RectTransform>();
        backRT.localScale = new Vector3(1f, 1f, 1f);
        backRT.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        backRT.sizeDelta = new Vector2(backRT.sizeDelta[0] - 5f, 50f);
        backRT.anchorMin = new Vector2(0.5f, 1f);
        backRT.anchorMax = new Vector2(0.5f, 1f);
        backRT.anchoredPosition = new Vector3(-102f, -285f);
        back.GetComponent<Image>().color = new Color(145f / 255f, 75f / 255f, 67f / 255f, 1f);
        back.GetComponent<Button>().transition = Selectable.Transition.ColorTint;
        back.layer = 5;
        
        GameObject txtBack = new GameObject(); // Создание текста для кнопки.
        txtBack.name = "txtBack";
        txtBack.transform.SetParent(back.transform);
        RectTransform RTtxtBack = txtBack.AddComponent<RectTransform>();
        RTtxtBack.localScale = new Vector3(1f, 1f, 1f);
        RTtxtBack.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        RTtxtBack.sizeDelta = new Vector2(RTtxtBack.sizeDelta[0], 20f);
        RTtxtBack.anchorMin = new Vector2(0f, 0.5f);
        RTtxtBack.anchorMax = new Vector2(0f, 0.5f);
        RTtxtBack.anchoredPosition = new Vector3(50f, 0f);
        Text textBack = txtBack.AddComponent<Text>();
        textBack.text = "Back";
        textBack.color = new Color(0f, 0f, 0f, 1f);
        textBack.fontSize = 18;
        textBack.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        textBack.alignment = TextAnchor.MiddleCenter;
        txtBack.layer = 5;

        
        

        // Обработчик события на нажатие кнопки Done:
        done.GetComponent<Button>().onClick.AddListener(() => CreateNewCharacterWithUniqNicknameAndPassword_Slot(
            Canvas.transform.Find("signupInputNickname").gameObject.GetComponent<InputField>().text,
            Canvas.transform.Find("signupInputPassword").gameObject.GetComponent<InputField>().text)
        );

        // Обработчик события на нажатие кнопки Back:
        back.GetComponent<Button>().onClick.AddListener(() => {
            clickBtn.Play();
            /*
            inputUniqNickname.SetActive(false);
            inputPassword.SetActive(false);
            done.SetActive(false);
            back.SetActive(false);

            CreateNewCharacterBtn.SetActive(false);
            IAlreadyHaveTheCharacterBtn.SetActive(false);
            */
        });
    }

    public void SignIn_Slot() { // У пользователя уже есть перс и он хочет продолжить играть с ним.

    }

    public void InputsTheNicknameAndPasswordDone_Slot(string nickname, string pass) { // Персонаж уже есть, пользователь входит в свою уч. запись.

    }

    private void CreateNewCharacterWithUniqNicknameAndPassword_Slot(string newNickname, string newPass) { // Пользователь создает нового персонажа.
        clickBtn.Play();
        // Здесь будем проверять существование такого ника в бд и если Ник является уникальным а пароль не является пустым полем, то переходим к следующей вкладке...

        // ...
        print(newNickname + "   " + newPass);
    }

    public void ChangeCharacterDone_Slot(int num) { // Выбор создаваемого персонажа

    }

    private void LoadMainMenuScene() { // Загрузка сцены с главным меню игры.

    }

    IEnumerator SetTimeout(float sec) {
        yield return new WaitForSeconds(sec);
        
    }

}
