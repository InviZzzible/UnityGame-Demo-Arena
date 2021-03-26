using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    private GameObject Canvas;
    private AudioSource clickBtn;

    private int currentIndexPage; // Текущий индекс текущей страницы в меню.

    void Start()
    {
        Canvas = GameObject.Find("Canvas");
        clickBtn = GameObject.Find("ClickBtn").GetComponent<AudioSource>();
        startPageUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void startPageUI() {
        // Создание стартового UI меню:
        // Создание кнопоки для создания нового персонажа:
        currentIndexPage = 0;
        GameObject newCharacter = new GameObject("CreateNewCharacter", typeof(Image), typeof(Button), typeof(LayoutElement));
        newCharacter.transform.SetParent(Canvas.transform);
        RectTransform newCharacterRT = newCharacter.GetComponent<RectTransform>();
        newCharacterRT.localScale = new Vector3(1f, 1f, 1f);
        newCharacterRT.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        newCharacterRT.sizeDelta = new Vector2(250f, 100f);
        newCharacterRT.anchorMin = new Vector2(0.5f, 0.5f);
        newCharacterRT.anchorMax = new Vector2(0.5f, 0.5f);
        newCharacterRT.anchoredPosition = new Vector3(0f, 70f);
        newCharacter.GetComponent<Image>().color = new Color(10f / 255f, 10f / 255f, 10f / 255f, 200f / 255f);
        newCharacter.AddComponent<Outline>().effectColor = new Color(1f, 1f, 1f, 1f);
        newCharacter.AddComponent<Outline>().effectDistance = new Vector2(2f, -2f);
        newCharacter.GetComponent<Button>().transition = Selectable.Transition.ColorTint;
        newCharacter.layer = 5;

        GameObject txtnewCharacter = new GameObject(); // Создание текста для кнопки.
        txtnewCharacter.name = "Text";
        txtnewCharacter.transform.SetParent(newCharacter.transform);
        RectTransform RTtxtnewCharacter = txtnewCharacter.AddComponent<RectTransform>();
        RTtxtnewCharacter.localScale = new Vector3(1f, 1f, 1f);
        RTtxtnewCharacter.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        RTtxtnewCharacter.sizeDelta = new Vector2(250f, 20f);
        RTtxtnewCharacter.anchorMin = new Vector2(0.5f, 0.5f);
        RTtxtnewCharacter.anchorMax = new Vector2(0.5f, 0.5f);
        RTtxtnewCharacter.anchoredPosition = new Vector3(0f, 0f);
        Text textnewCharacter = txtnewCharacter.AddComponent<Text>();
        textnewCharacter.text = "Create a new character...";
        textnewCharacter.color = new Color(1f, 1f, 1f, 1f);
        textnewCharacter.fontSize = 16;
        textnewCharacter.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        textnewCharacter.alignment = TextAnchor.MiddleCenter;
        txtnewCharacter.layer = 5;

        // Создание кнопоки если пользователь хочет продолжить играть уже созданным персонажем:
        GameObject haveCharacter = new GameObject("IAlreadyHaveTheCharacter", typeof(Image), typeof(Button), typeof(LayoutElement));
        haveCharacter.transform.SetParent(Canvas.transform);
        RectTransform haveCharacterRT = haveCharacter.GetComponent<RectTransform>();
        haveCharacterRT.localScale = new Vector3(1f, 1f, 1f);
        haveCharacterRT.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        haveCharacterRT.sizeDelta = new Vector2(250f, 100f);
        haveCharacterRT.anchorMin = new Vector2(0.5f, 0.5f);
        haveCharacterRT.anchorMax = new Vector2(0.5f, 0.5f);
        haveCharacterRT.anchoredPosition = new Vector3(0f, -70f);
        haveCharacter.GetComponent<Image>().color = new Color(10f / 255f, 10f / 255f, 10f / 255f, 200f / 255f);
        haveCharacter.AddComponent<Outline>().effectColor = new Color(1f, 1f, 1f, 1f);
        haveCharacter.AddComponent<Outline>().effectDistance = new Vector2(2f, -2f);
        haveCharacter.GetComponent<Button>().transition = Selectable.Transition.ColorTint;
        haveCharacter.layer = 5;

        GameObject txtHaveCharacter = new GameObject(); // Создание текста для кнопки.
        txtHaveCharacter.name = "Text";
        txtHaveCharacter.transform.SetParent(haveCharacter.transform);
        RectTransform RTtxtHaveCharacter = txtHaveCharacter.AddComponent<RectTransform>();
        RTtxtHaveCharacter.localScale = new Vector3(1f, 1f, 1f);
        RTtxtHaveCharacter.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        RTtxtHaveCharacter.sizeDelta = new Vector2(250f, 20f);
        RTtxtHaveCharacter.anchorMin = new Vector2(0.5f, 0.5f);
        RTtxtHaveCharacter.anchorMax = new Vector2(0.5f, 0.5f);
        RTtxtHaveCharacter.anchoredPosition = new Vector3(0f, 0f);
        Text textHaveCharacter = txtHaveCharacter.AddComponent<Text>();
        textHaveCharacter.text = "I already have the character...";
        textHaveCharacter.color = new Color(1f, 1f, 1f, 1f);
        textHaveCharacter.fontSize = 16;
        textHaveCharacter.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        textHaveCharacter.alignment = TextAnchor.MiddleCenter;
        txtHaveCharacter.layer = 5;

        // Обработчики событий для кнопок:
        newCharacter.GetComponent<Button>().onClick.AddListener(() => {
            Destroy(newCharacter);
            Destroy(haveCharacter);
            Sign_Slot(1);
        });
        haveCharacter.GetComponent<Button>().onClick.AddListener(() => {
            Destroy(newCharacter);
            Destroy(haveCharacter);
            Sign_Slot(3);
        });
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
        // imgBorder.sprite = Resources.Load("BorderSprite", typeof(Sprite)) as Sprite; // !!!
        imgBorder.color = new Color(50f/255f, 50f / 255f, 50f / 255f, 100f / 255f);
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
        if (input.name == "signInputNickname") phText.text = "Input your nickname...";
        else {
            phText.text = "Pick a password...";
            ifd.contentType = InputField.ContentType.Password;
        }
        phText.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        ifd.placeholder = phText;
        placeholderObj.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        placeholderObj.GetComponent<Text>().fontStyle = FontStyle.Italic;
        placeholderObj.GetComponent<Text>().color = new Color(100f/255f, 100f / 255f, 100f / 255f, 150f / 255f);


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
        cb.normalColor = new Color(140f/255f, 140f / 255f, 140f / 255f, 1f);
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
                if(input.gameObject.name == "signInputNickname") text.text = "Input your nickname...";
                else text.text = "Pick a password...";
            }
        });

        if (nameObj == "signInputNickname") {
            EventSystem.current.SetSelectedGameObject(input.gameObject, null); // Устанавливаем фокус.
        }
    }

    public void Sign_Slot(int nextIndex) { // Пользователь хочет создать нового персонажа или же продолжить играть уже существующим:

        clickBtn.Play();
        currentIndexPage = nextIndex; // 1 или 3

        GameObject CreateNewCharacterBtn = GameObject.Find("CreateNewCharacter");
        GameObject IAlreadyHaveTheCharacterBtn = GameObject.Find("IAlreadyHaveTheCharacter");
        CreateNewCharacterBtn.SetActive(false);
        IAlreadyHaveTheCharacterBtn.SetActive(false);

        // Создание вложенного интерфейса:
        // Создание Заголовка:
        GameObject header = new GameObject("signHeader", typeof(Text), typeof(LayoutElement));
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
        if (currentIndexPage == 1) header.GetComponent<Text>().text = "Creating your character:";
        else if (currentIndexPage == 3) header.GetComponent<Text>().text = "Sign in to your account:";
        header.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        Outline outlineHeader = header.AddComponent<Outline>();
        outlineHeader.effectColor = new Color(255f, 255f, 255f, 100f);
        outlineHeader.effectDistance = new Vector2(0.2f, -0.2f);
        header.layer = 5;

        // Создание описания:
        GameObject description = new GameObject("signDescription", typeof(Text), typeof(LayoutElement));
        description.transform.SetParent(Canvas.transform);
        RectTransform rtDescription = description.GetComponent<RectTransform>();
        rtDescription.localScale = new Vector3(1f, 1f, 1f);
        rtDescription.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        rtDescription.anchorMin = new Vector2(0.5f, 1f);
        rtDescription.anchorMax = new Vector2(0.5f, 1f);
        rtDescription.sizeDelta = new Vector2(400f, 20f); // Set width and height
        rtDescription.localPosition = new Vector3(0f, 0f, 0f);
        rtDescription.anchoredPosition = new Vector3(0f, -100f, 0f);
        description.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        description.GetComponent<Text>().fontStyle = FontStyle.Italic;
        description.GetComponent<Text>().fontSize = 10;
        if (currentIndexPage == 1) description.GetComponent<Text>().text = "Come up with a nickname that will be unique in this game!";
        else if (currentIndexPage == 3) description.GetComponent<Text>().text = "Enter the nickname and password for the previously created account!";
        description.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        description.layer = 5;

        // Создаем поля Input Field - ы:
        CreateTheInputField("signInputNickname", -160f); // 1й
        CreateTheInputField("signInputPassword", -220f); // 2й

        // Создание кнопоки done:
        GameObject done = new GameObject("signDone", typeof(Image), typeof(Button), typeof(LayoutElement));
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
        GameObject back = new GameObject("signBack", typeof(Image), typeof(Button), typeof(LayoutElement));
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

        // Создание кнопки, чтобы просмотреть пароль:
        GameObject look = new GameObject("signLook", typeof(Image), typeof(Button), typeof(LayoutElement));
        look.transform.SetParent(Canvas.transform.Find("signInputPassword").gameObject.transform);
        RectTransform lookRT = look.GetComponent<RectTransform>();
        lookRT.localScale = new Vector3(1f, 1f, 1f);
        lookRT.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        lookRT.sizeDelta = new Vector2(40f, 40f);
        lookRT.anchorMin = new Vector2(1f, 0.5f);
        lookRT.anchorMax = new Vector2(1f, 0.5f);
        lookRT.anchoredPosition = new Vector3(25f, 2f);
        look.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        look.GetComponent<Image>().sprite = Resources.Load("iceSprite", typeof(Sprite)) as Sprite;
        look.GetComponent<Button>().transition = Selectable.Transition.ColorTint;
        look.layer = 5;


        // ОБРАБОТЧИКИ СОБЫТИЙ ДЛЯ КНОПОК:
        // Обработчик события на нажатие кнопки Look:
        look.GetComponent<Button>().onClick.AddListener(() => {
            clickBtn.Play();
            InputField inputF = Canvas.transform.Find("signInputPassword").gameObject.GetComponent<InputField>();
            if (inputF.contentType == InputField.ContentType.Password) {
                inputF.contentType = InputField.ContentType.Standard;
                look.GetComponent<Image>().color = new Color(22f / 255f, 22f / 255f, 22f / 255f, 1f);
            }
            else {
                inputF.contentType = InputField.ContentType.Password;
                look.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            }
            EventSystem.current.SetSelectedGameObject(inputF.gameObject, null); // Устанавливаем фокус.

        });

        // Обработчик события на нажатие кнопки Done:
        done.GetComponent<Button>().onClick.AddListener(() => {
            clickBtn.Play();

            string textNick = Canvas.transform.Find("signInputNickname").gameObject.GetComponent<InputField>().text;
            string textPass = Canvas.transform.Find("signInputPassword").gameObject.GetComponent<InputField>().text;

            if (textNick != "" && textPass != "") {
                if (currentIndexPage == 1) {
                    if (true /* Здесь будем проверять уникальность введенного ника в БД и если ник уникален тогда запускаем функцию:  */) {
                        currentIndexPage = 2;

                        // Удаляем ненужные объекты текущей страницы:
                        Destroy(header);
                        Destroy(description);
                        Destroy(back);
                        Destroy(done);
                        Destroy(look);
                        Destroy(GameObject.Find("signInputNickname"));
                        Destroy(GameObject.Find("signInputPassword"));

                        ChangeCharacterDone_Slot(textNick, textPass); // Переходим к выбору персонажа...
                    }
                    else { // Если нет, тогда остаемся на этой же странице, указывая пользователю, что его ник не уникален:
                        Canvas.transform.Find("signInputNickname").gameObject.transform.Find("Border").gameObject.GetComponent<Image>().color = new Color(90f / 255, 20f / 255f, 20f / 255f, 1f);
                        StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f/255f), "signInputNickname", "Border"));

                        MessageOfError("The nickname you entered is not unique !", -120f);
                        StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), "signInputNickname", "Border"));
                    }

                }
                else if (currentIndexPage == 3) {
                    if (true /* Здесь будем проверять существует ли такая запись в БД и если да, то проходим в основное меню игры  */) { // !!!!!!!!!!!!!!!!!!!!
                        currentIndexPage = 4;

                        // Удаляем ненужные объекты текущей страницы:
                        Destroy(header);
                        Destroy(description);
                        Destroy(back);
                        Destroy(done);
                        Destroy(look);
                        Destroy(GameObject.Find("signInputNickname"));
                        Destroy(GameObject.Find("signInputPassword"));

                        MainMenu(textNick, textPass);
                    }
                    else {
                        Canvas.transform.Find("signInputNickname").gameObject.transform.Find("Border").gameObject.GetComponent<Image>().color = new Color(90f / 255, 20f / 255f, 20f / 255f, 1f);
                        StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), "signInputNickname", "Border"));

                        MessageOfError("Account with nickname " + textNick + " does not exist !", -120f);
                        StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), "signInputNickname", "Border"));
                    }
                }
            }
            else {
                if (textNick == "") {
                    Canvas.transform.Find("signInputNickname").gameObject.transform.Find("Border").gameObject.GetComponent<Image>().color = new Color(90f / 255, 20f / 255f, 20f / 255f, 1f);
                    StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), "signInputNickname", "Border"));
                }
                if (textPass == "") {
                    Canvas.transform.Find("signInputPassword").gameObject.transform.Find("Border").gameObject.GetComponent<Image>().color = new Color(90f/255, 20f/255f, 20f/255f, 1f);
                    StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), "signInputPassword", "Border"));
                }
                MessageOfError("One of the fields is empty !", -120f);
            }
        });

        // Обработчик события на нажатие кнопки Back:
        back.GetComponent<Button>().onClick.AddListener(() => {
            clickBtn.Play();

            // Удаляем ненужные объекты текущей страницы:
            Destroy(header);
            Destroy(description);
            Destroy(back);
            Destroy(done);
            Destroy(look);
            Destroy(GameObject.Find("signInputNickname"));
            Destroy(GameObject.Find("signInputPassword"));

            startPageUI(); // Создаем интерфейс стартовой страницы.
        });
    }

    private void MessageOfError(string str, float posY) {
        // Создаем временное сообщение для пользователя:
        GameObject err = new GameObject("err", typeof(Text), typeof(LayoutElement));
        err.transform.SetParent(Canvas.transform);
        RectTransform rtErr = err.GetComponent<RectTransform>();
        rtErr.localScale = new Vector3(1f, 1f, 1f);
        rtErr.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        rtErr.anchorMin = new Vector2(0.5f, 1f);
        rtErr.anchorMax = new Vector2(0.5f, 1f);
        rtErr.sizeDelta = new Vector2(400f, 20f); // Set width and height
        rtErr.localPosition = new Vector3(0f, 0f, 0f);
        rtErr.anchoredPosition = new Vector3(0f, posY, 0f);
        err.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        err.GetComponent<Text>().fontStyle = FontStyle.Italic;
        err.GetComponent<Text>().fontSize = 10;
        err.GetComponent<Text>().text = str;
        err.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        err.layer = 5;
        Text errText = err.GetComponent<Text>();
        errText.color = new Color(1f, 0f, 0f, 1f);
        StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), "signInputNickname", "Border"));
        Destroy(err, 3f);
    }

    public void ChangeCharacterDone_Slot(string newNickname, string newPass) { // Выбор создаваемого персонажа:

        // ...

    }

    private void MainMenu(string nickname, string password) { // Главное меню игры:

        // ...

    }

    IEnumerator SetTimeoutChangeColor(float sec, Vector4 vectorColor, string name, string name2 = "") {
        yield return new WaitForSeconds(sec);
        
        Canvas.transform.Find(name).gameObject.transform.Find(name2).gameObject.GetComponent<Image>().color =
            new Color(vectorColor[0], vectorColor[1], vectorColor[2], vectorColor[3]);
    }

}
