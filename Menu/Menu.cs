using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using System;


// Классы, необходимый для создания объектов, которые будут конвертироваться в json и 
// обратно для записи/чтения данных в локальном пространстве устройства:
[System.Serializable]
public class ArrayCharacterInfo {               
    public List<CharacterInfo> list = new List<CharacterInfo>();
}

[System.Serializable]
public class CharacterInfo {
    public string Nickname = "";
    public string Password = "";
    public string LeftHand = "";
    public string RightHand = "";
    public string LeftFoot = "";
    public string RightFoot = "";
    public string ActionSlot_01 = "";
    public string ActionSlot_02 = "";
    public string ActionSlot_03 = "";
    public string ActionSlot_04 = "";
    public string ActionSlot_05 = "";
    public bool isActualCharacter = false;
}

// ---------------------------------------------------------------------------

// Класс для проверки существования файла, для записи и для чтения файла:
public class RecordAndReadFile {
    public static string fileName = "CharacterInfo.info";
    private string path;
    public RecordAndReadFile(string path) {
#if !UNITY_EDITOR
    this.path = Path.Combine(Application.persistentDataPath, path);
#else
        this.path = Path.Combine(Application.dataPath, path);
#endif
    }

    public bool FileExists() {
        return File.Exists(path);
    }

    public void RecordInFile(ArrayCharacterInfo obj) {
        string toJson = JsonUtility.ToJson(obj, true);
        File.WriteAllText(path, toJson);
    }

    public ArrayCharacterInfo ReadFromFile() {
        string fromjson = File.ReadAllText(path);
        return JsonUtility.FromJson<ArrayCharacterInfo>(fromjson);
    }
}
// ---------------------------------------------------------------------------


public class Menu : MonoBehaviour
{

    private GameObject Canvas;
    private AudioSource clickBtn;

    CharacterInfo chInfo = new CharacterInfo();
    private int currentIndexPage; // Текущий индекс текущей страницы в меню.
    private int currentNumberOfChangedModel2D = -1; // Номер модели 2D персонажа.

    public HttpRequest httpRequest = new HttpRequest();
    private string statusOfWaiting = "";

    void Start()
    {
        Canvas = GameObject.Find("Canvas");
        clickBtn = GameObject.Find("ClickBtn").GetComponent<AudioSource>();

        startPageUI();
        httpRequest.Emit_answer += Checking;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Обработчик событий на тикие простые события как проверка уникальности ника, существования учетки и создания уч.:
    protected void Checking(string json) {
        if (statusOfWaiting == "CkeckOnUniqNickname") {

            string Answer = hidden.SH.Decipher(json); // true / false

            if (Answer == "true") {
                currentIndexPage = 2;

                // Создаем объект с актуальной информацией об учетной записи и персонаже:
                CharacterInfo newRecord = new CharacterInfo();
                newRecord.Nickname = GameObject.Find("signInputNickname").GetComponent<InputField>().text;
                newRecord.Password = GameObject.Find("signInputPassword").GetComponent<InputField>().text;
                newRecord.isActualCharacter = true;

                // Обнуляем актуальность последней сессии и делаем актуальной текущую:
                RecordAndReadFile file = new RecordAndReadFile(RecordAndReadFile.fileName);
                if (file.FileExists()) {
                    // Читаем файл:
                    ArrayCharacterInfo obj = file.ReadFromFile();

                    // Поиск актуальной записи - та запись у которой isActive == true является актуальной:
                    List<CharacterInfo> chInfo = (from rec in obj.list
                                                  select rec).ToList();

                    if (chInfo != null) {
                        foreach (var iterator in chInfo) { // Обнуляем актуальность предудущих сессии:
                            iterator.isActualCharacter = false;
                        }
                    }
                }
                else { // Если же не существует, то создаем новый файл:

                    ArrayCharacterInfo newArrayObj = new ArrayCharacterInfo();
                    newArrayObj.list.Add(newRecord);
                }



                // Удаляем ненужные объекты текущей страницы:
                Destroy(GameObject.Find("signHeader"));
                Destroy(GameObject.Find("signDescription"));
                Destroy(GameObject.Find("signBack"));
                Destroy(GameObject.Find("signDone"));
                Destroy(GameObject.Find("signLook"));
                Destroy(GameObject.Find("signInputNickname"));
                Destroy(GameObject.Find("signInputPassword"));

                ChangeCharacterDone_Slot(); // Переходим к выбору персонажа...
            }
            else {
                Canvas.transform.Find("signInputNickname").gameObject.transform.Find("Border").gameObject.GetComponent<Image>().color = new Color(90f / 255, 20f / 255f, 20f / 255f, 1f);
                StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), "signInputNickname", "Border"));
                MessageOfError("The nickname you entered is not unique !", -120f);
                StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), "signInputNickname", "Border"));
            }
        }
        else if (statusOfWaiting == "CheckOnExistingSuchNickname") {

            string Answer = hidden.SH.Decipher(json); // true / false

            if (Answer == "true") {
                currentIndexPage = 4;

                // Создаем объект с актуальной информацией об учетной записи и персонаже:
                chInfo.Nickname = GameObject.Find("signInputNickname").GetComponent<InputField>().text;
                chInfo.Password = GameObject.Find("signInputPassword").GetComponent<InputField>().text;
                chInfo.isActualCharacter = true;

                // Обнуляем актуальность последней сессии и делаем актуальной текущую:
                RecordAndReadFile file = new RecordAndReadFile(RecordAndReadFile.fileName);
                if (file.FileExists()) {
                    // Читаем файл:
                    ArrayCharacterInfo obj = file.ReadFromFile();

                    // Поиск актуальной записи - та запись у которой isActive == true является актуальной:
                    List<CharacterInfo> chInfoList = (from rec in obj.list
                                                  select rec).ToList();
                    if (chInfo != null) {
                        foreach (var iterator in chInfoList) { // Обнуляем актуальность предудущей сессии:
                            if (iterator.Nickname == chInfo.Nickname) iterator.isActualCharacter = true;
                            else iterator.isActualCharacter = false;
                        }
                    }


                    // Перезаписываем локальный файл:
                    file.RecordInFile(obj);


                    // Записываем данные в input поля:
                    GameObject.Find("signInputNickname").GetComponent<InputField>().text = chInfoList[0].Nickname;
                    GameObject.Find("signInputPassword").GetComponent<InputField>().text = chInfoList[0].Password;
                }
                else { // Если же не существует, то создаем новый файл: (На случай случайного его удаления)

                    ArrayCharacterInfo newArrayObj = new ArrayCharacterInfo();
                    newArrayObj.list.Add(chInfo);

                    // Перезаписываем локальный файл:
                    file.RecordInFile(newArrayObj);
                }

                // Удаляем ненужные объекты текущей страницы:
                Destroy(GameObject.Find("signHeader"));
                Destroy(GameObject.Find("signDescription"));
                Destroy(GameObject.Find("signBack"));
                Destroy(GameObject.Find("signDone"));
                Destroy(GameObject.Find("signLook"));
                Destroy(GameObject.Find("signInputNickname"));
                Destroy(GameObject.Find("signInputPassword"));

                

                // Также здесь будем получать из БД значение модели по данному персонажу:
                // currentNumberOfChangedModel2D = ...; 

                MainMenu();
            }
            else {
                Canvas.transform.Find("signInputNickname").gameObject.transform.Find("Border").gameObject.GetComponent<Image>().color = new Color(90f / 255, 20f / 255f, 20f / 255f, 1f);
                StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), "signInputNickname", "Border"));
                MessageOfError("Account with nickname " + chInfo.Nickname + " does not exist !", -120f);
                StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), "signInputNickname", "Border"));
            }
        }
        else if (statusOfWaiting == "Create_character") {

            // Просто ожидаем ответ от сервера, для продолжения работы...
            string Answer = hidden.SH.Decipher(json); // true

            if(Answer == "true") {
                // Проверяем, есть ли файл CharacterInfo существует, то необходимо обнулить актуальность последней сессии:
                RecordAndReadFile file = new RecordAndReadFile(RecordAndReadFile.fileName);
                if (file.FileExists()) {
                    // Читаем файл:
                    ArrayCharacterInfo obj = file.ReadFromFile();

                    // Поиск актуальной записи - та запись у которой isActive == true является актуальной:
                    List<CharacterInfo> chInfoList = (from rec in obj.list
                                                      select rec).ToList();
                    if (chInfo != null) {
                        foreach (var iterator in chInfoList) { // Обнуляем актуальность предудущей сессии:
                            iterator.isActualCharacter = false;
                        }
                    }



                    // Так как мы находимся на стадии создания персонажа, то добавляем новый элемент к списку всех учеток:
                    obj.list.Add(chInfo);

                    // Перезаписываем локальный файл:
                    file.RecordInFile(obj);
                }
                else { // Если же не существует, то создаем новый файл:
                    ArrayCharacterInfo newArrayObj = new ArrayCharacterInfo();
                    newArrayObj.list.Add(chInfo);

                    // Перезаписываем локальный файл:
                    file.RecordInFile(newArrayObj);
                }

                Destroy(GameObject.Find("changeHeader"));
                Destroy(GameObject.Find("woman"));
                Destroy(GameObject.Find("man"));
                Destroy(GameObject.Find("pictureBtn"));
                Destroy(GameObject.Find("Back"));
                Destroy(GameObject.Find("Done"));
                Destroy(GameObject.Find("leftArrow"));
                Destroy(GameObject.Find("rightArrow"));

                MainMenu();
            }
        }
        else if(statusOfWaiting == "PveIndex") {
            int IndexScene = Convert.ToInt32(hidden.SH.Decipher(json));

            if(IndexScene == 1) {
                GameObject.Find("mainMenu").SetActive(false); // Делаем главное меню временно не активным.
                
                // Запускаем нулевую сцену:
                SceneManager.LoadScene(1);
            }
            else {
                // Если индекс текущей сцены не равен 1, тогда переходим в подменю с выбором о
                // продолжении прохождения турнира или же о прохождении его с самого начала:

                GameObject.Find("mainMenu").SetActive(false); // Делаем главное меню временно не активным.

                // Создание кнопок выбора:
                GameObject change_01 = new GameObject("change_01", typeof(Image), typeof(Button), typeof(LayoutElement));
                change_01.transform.SetParent(Canvas.transform);
                RectTransform change_01RT = change_01.GetComponent<RectTransform>();
                change_01RT.localScale = new Vector3(1f, 1f, 1f);
                change_01RT.localRotation = new Quaternion(0f, 0f, 0f, 0f);
                change_01RT.sizeDelta = new Vector2(250f, 100f);
                change_01RT.anchorMin = new Vector2(0.5f, 0.5f);
                change_01RT.anchorMax = new Vector2(0.5f, 0.5f);
                change_01RT.anchoredPosition = new Vector3(0f, 70f);
                change_01.GetComponent<Image>().color = new Color(10f / 255f, 10f / 255f, 10f / 255f, 200f / 255f);
                change_01.AddComponent<Outline>().effectColor = new Color(1f, 1f, 1f, 1f);
                change_01.AddComponent<Outline>().effectDistance = new Vector2(2f, -2f);
                change_01.GetComponent<Button>().transition = Selectable.Transition.ColorTint;
                change_01.layer = 5;

                GameObject txtChange_01 = new GameObject(); // Создание текста для кнопки.
                txtChange_01.name = "Text";
                txtChange_01.transform.SetParent(change_01.transform);
                RectTransform RTtxtChange_01 = txtChange_01.AddComponent<RectTransform>();
                RTtxtChange_01.localScale = new Vector3(1f, 1f, 1f);
                RTtxtChange_01.localRotation = new Quaternion(0f, 0f, 0f, 0f);
                RTtxtChange_01.sizeDelta = new Vector2(250f, 20f);
                RTtxtChange_01.anchorMin = new Vector2(0.5f, 0.5f);
                RTtxtChange_01.anchorMax = new Vector2(0.5f, 0.5f);
                RTtxtChange_01.anchoredPosition = new Vector3(0f, 0f);
                Text textnewCharacter = txtChange_01.AddComponent<Text>();
                textnewCharacter.text = "Continue tournament...";
                textnewCharacter.color = new Color(1f, 1f, 1f, 1f);
                textnewCharacter.fontSize = 16;
                textnewCharacter.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                textnewCharacter.alignment = TextAnchor.MiddleCenter;
                txtChange_01.layer = 5;

                GameObject change_02 = new GameObject("change_02", typeof(Image), typeof(Button), typeof(LayoutElement));
                change_02.transform.SetParent(Canvas.transform);
                RectTransform change_02RT = change_02.GetComponent<RectTransform>();
                change_02RT.localScale = new Vector3(1f, 1f, 1f);
                change_02RT.localRotation = new Quaternion(0f, 0f, 0f, 0f);
                change_02RT.sizeDelta = new Vector2(250f, 100f);
                change_02RT.anchorMin = new Vector2(0.5f, 0.5f);
                change_02RT.anchorMax = new Vector2(0.5f, 0.5f);
                change_02RT.anchoredPosition = new Vector3(0f, -70f);
                change_02.GetComponent<Image>().color = new Color(10f / 255f, 10f / 255f, 10f / 255f, 200f / 255f);
                change_02.AddComponent<Outline>().effectColor = new Color(1f, 1f, 1f, 1f);
                change_02.AddComponent<Outline>().effectDistance = new Vector2(2f, -2f);
                change_02.GetComponent<Button>().transition = Selectable.Transition.ColorTint;
                change_02.layer = 5;

                GameObject txtchange_02 = new GameObject(); // Создание текста для кнопки.
                txtchange_02.name = "Text";
                txtchange_02.transform.SetParent(change_02.transform);
                RectTransform RTtxtchange_02 = txtchange_02.AddComponent<RectTransform>();
                RTtxtchange_02.localScale = new Vector3(1f, 1f, 1f);
                RTtxtchange_02.localRotation = new Quaternion(0f, 0f, 0f, 0f);
                RTtxtchange_02.sizeDelta = new Vector2(250f, 20f);
                RTtxtchange_02.anchorMin = new Vector2(0.5f, 0.5f);
                RTtxtchange_02.anchorMax = new Vector2(0.5f, 0.5f);
                RTtxtchange_02.anchoredPosition = new Vector3(0f, 0f);
                Text textchange_02 = txtchange_02.AddComponent<Text>();
                textchange_02.text = "Start tournament...";
                textchange_02.color = new Color(1f, 1f, 1f, 1f);
                textchange_02.fontSize = 16;
                textchange_02.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                textchange_02.alignment = TextAnchor.MiddleCenter;
                txtchange_02.layer = 5;

                // Обработчики событий для кнопок:
                change_01.GetComponent<Button>().onClick.AddListener(() => { // Продолжить турнир !!!
                    clickBtn.Play();

                    Destroy(change_01);
                    Destroy(change_02);

                    // Запускаем актуальную сцену:
                    SceneManager.LoadScene(IndexScene);
                });
                change_02.GetComponent<Button>().onClick.AddListener(() => { // Начать турнир сначала !!!
                    clickBtn.Play();

                    Destroy(change_01);
                    Destroy(change_02);

                    StaticClasses.TransitsData.transitPveCurrentLevel = 1; // Сброс индекса сцен.
                    StaticClasses.TransitsData.transitPveCurrentScore = 0; // Сброс счета.
                    // Запускаем нулевую сцену:
                    SceneManager.LoadScene(1);
                });

            }
        }
        else if (statusOfWaiting == "PveTopDatas") {
            GameObject.Find("mainMenu").SetActive(false); // Делаем главное меню временно не активным.
            // Загружаем префаб с подменю:

            // ...
        }
        else if (statusOfWaiting == "PveTopDatas") {
            GameObject.Find("mainMenu").SetActive(false); // Делаем главное меню временно не активным.
            // Загружаем префаб с подменю:

            // ...
        }
        else if (statusOfWaiting == "CharactersInformation") {
            GameObject.Find("mainMenu").SetActive(false); // Делаем главное меню временно не активным.
            // Загружаем префаб с подменю:

            // ...
        }
        else if (statusOfWaiting == "StoreList") {
            GameObject.Find("mainMenu").SetActive(false); // Делаем главное меню временно не активным.
            // Загружаем префаб с подменю:

            // ...
        }
        // ...


        statusOfWaiting = "";
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

    public void Sign_Slot(int nextIndex, bool comeback = false, string nicknameComeBack = "", string passwordComeBack = "") { // Пользователь хочет создать нового персонажа или же продолжить играть уже существующим:

        clickBtn.Play();
        currentIndexPage = nextIndex; // 1 или 3

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
        txtDone.name = "Text";
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
        txtBack.name = "Text";
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

        if(currentIndexPage == 3) { // Если пытаемся зайти в одну из ранее созданных учеток, то имеет смысл
            // подставить в поля ввода данные из последней играбельной (далее по тексту актуальной) учетки:
            // Проверяем, есть ли файл CharacterInfo существует, то загружаем информацию о логине, пароле и т.д. и подставляем в поля:
            RecordAndReadFile file = new RecordAndReadFile(RecordAndReadFile.fileName);
            if (file.FileExists()) {
                // Читаем файл:
                ArrayCharacterInfo obj = file.ReadFromFile();
                if(obj != null) {
                    // Поиск актуальной записи - та запись у которой isActualCharacter == true является актуальной:
                    List<CharacterInfo> chInfo = (from rec in obj.list
                                                  where rec.isActualCharacter == true
                                                  select rec).ToList();

                    if (chInfo != null && chInfo.Count != 0) {
                        // Записываем данные в input поля:
                        GameObject.Find("signInputNickname").GetComponent<InputField>().text = chInfo[0].Nickname;
                        GameObject.Find("signInputPassword").GetComponent<InputField>().text = chInfo[0].Password;
                    }
                }
            }
        }
        else if(currentIndexPage == 1 && comeback) { // Есл мы возвращаемся в это меню для создания перса из меню выбора перса, то тоже есть смысл
                                                     // показать логин и пароль пользователю:
            // Записываем данные в input поля:
            GameObject.Find("signInputNickname").GetComponent<InputField>().text = nicknameComeBack;
            GameObject.Find("signInputPassword").GetComponent<InputField>().text = passwordComeBack;
        }

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
                Regex regularForNickname = new Regex("^[A-Za-zА-Яа-я]{2}[A-Za-zА-Яа-я0-9 _+=*&$!@#%\",.?]*$");
                Regex regularForPassword = new Regex("^[A-Za-zА-Яа-я0-9 _+=*&$!@#%\",.?]*$");

                if (regularForNickname.IsMatch(textNick)) {
                    if (regularForPassword.IsMatch(textPass)) {

                        if (currentIndexPage == 1) {

                            // Чтобы не задрачивать удаленный сервер, сначала проверим является ли придуманный ник
                            // уникальным в локальных данных пользователя:

                            bool uniqFile = false;
                            RecordAndReadFile file = new RecordAndReadFile(RecordAndReadFile.fileName);
                            if (file.FileExists()) {
                                // Читаем файл:
                                ArrayCharacterInfo obj = file.ReadFromFile();

                                int count = obj.list.Where(x => x.Nickname == textNick).Select(x => x).Count();

                                if (count == 0) { // Ник является уникальным в локальной среде
                                    uniqFile = true;
                                }
                            }
                            else uniqFile = true;


                            if (uniqFile) {

                                httpRequest.POST("CkeckOnUniqNickname|" + chInfo.Nickname);
                                statusOfWaiting = "CkeckOnUniqNickname";

                            }
                            else { // Если нет, тогда остаемся на этой же странице, указывая пользователю, что такой ник не является уникальным:
                                Canvas.transform.Find("signInputNickname").gameObject.transform.Find("Border").gameObject.GetComponent<Image>().color = new Color(90f / 255, 20f / 255f, 20f / 255f, 1f);
                                StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), "signInputNickname", "Border"));
                                MessageOfError("The nickname you entered is not unique !", -120f);
                                StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), "signInputNickname", "Border"));
                            }

                        }
                        else if (currentIndexPage == 3) {

                            // Чтобы не задрачивать удаленный сервер, сначала проверим есть ли такой никнейм в локальном пространстве пользователя:
                            bool uniqFile = false;
                            RecordAndReadFile file = new RecordAndReadFile(RecordAndReadFile.fileName);
                            if (file.FileExists()) {
                                // Читаем файл:
                                ArrayCharacterInfo obj = file.ReadFromFile();

                                int count = obj.list.Where(x => x.Nickname == textNick).Select(x => x).Count();

                                if (count == 0) { // Ник является уникальным в локальной среде, что не допустимо
                                    uniqFile = true;
                                }
                            }
                            else uniqFile = true;

                            if (!uniqFile) {
                                /* Здесь будем проверять существует ли такая запись в БД и если да, то проходим в основное меню игры:  */
                                httpRequest.POST("CheckOnExistingSuchNickname|" + chInfo.Nickname + "|" + chInfo.Password);
                                statusOfWaiting = "CheckOnExistingSuchNickname";

                            }
                            else {
                                Canvas.transform.Find("signInputNickname").gameObject.transform.Find("Border").gameObject.GetComponent<Image>().color = new Color(90f / 255, 20f / 255f, 20f / 255f, 1f);
                                StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), "signInputNickname", "Border"));
                                MessageOfError("You have not created a post with this nickname before !", -120f);
                                StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), "signInputNickname", "Border"));
                            }
                        }
                    }
                    else {
                        Canvas.transform.Find("signInputPassword").gameObject.transform.Find("Border").gameObject.GetComponent<Image>().color = new Color(90f / 255, 20f / 255f, 20f / 255f, 1f);
                        StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), "signInputPassword", "Border"));
                        MessageOfError("Invalid input !", -120f);
                        StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), "signInputPassword", "Border"));
                    }
                }
                else {
                    Canvas.transform.Find("signInputNickname").gameObject.transform.Find("Border").gameObject.GetComponent<Image>().color = new Color(90f / 255, 20f / 255f, 20f / 255f, 1f);
                    StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), "signInputNickname", "Border"));
                    MessageOfError("Invalid input !", -120f);
                    StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), "signInputNickname", "Border"));
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

    public void ChangeCharacterDone_Slot() { // Выбор создаваемого персонажа:

        bool isManBtnActive = false;
        bool isWomanBtnActive = false;

        // Создание Заголовка:
        GameObject header = new GameObject("changeHeader", typeof(Text), typeof(LayoutElement));
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
        header.GetComponent<Text>().text = "Choose a character:";
        header.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        Outline outlineHeader = header.AddComponent<Outline>();
        outlineHeader.effectColor = new Color(255f, 255f, 255f, 100f);
        outlineHeader.effectDistance = new Vector2(0.2f, -0.2f);
        header.layer = 5;

        // Создание кнопок выбора мужского или женского персонажа:
        GameObject woman = new GameObject("woman", typeof(Image), typeof(Button), typeof(LayoutElement));
        woman.transform.SetParent(Canvas.transform);
        RectTransform womanRT = woman.GetComponent<RectTransform>();
        womanRT.localScale = new Vector3(1f, 1f, 1f);
        womanRT.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        womanRT.sizeDelta = new Vector2(40f, 40f);
        womanRT.anchorMin = new Vector2(0.5f, 1f);
        womanRT.anchorMax = new Vector2(0.5f, 1f);
        womanRT.anchoredPosition = new Vector3(-25f, -110f);
        woman.GetComponent<Button>().transition = Selectable.Transition.ColorTint;
        woman.layer = 5;
        Image imgWoman = woman.GetComponent<Image>();
        imgWoman.sprite = Resources.Load("ManAndWomanButtons/SignOfWomanSprite", typeof(Sprite)) as Sprite;
        imgWoman.color = new Color(60f / 255f, 60f / 255f, 60f / 255f, 1f);

        GameObject man = new GameObject("man", typeof(Image), typeof(Button), typeof(LayoutElement));
        man.transform.SetParent(Canvas.transform);
        RectTransform manRT = man.GetComponent<RectTransform>();
        manRT.localScale = new Vector3(1f, 1f, 1f);
        manRT.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        manRT.sizeDelta = new Vector2(40f, 40f);
        manRT.anchorMin = new Vector2(0.5f, 1f);
        manRT.anchorMax = new Vector2(0.5f, 1f);
        manRT.anchoredPosition = new Vector3(25f, -110f);
        man.GetComponent<Button>().transition = Selectable.Transition.ColorTint;
        man.layer = 5;
        Image imgMan = man.GetComponent<Image>();
        imgMan.sprite = Resources.Load("ManAndWomanButtons/SignOfManSprite", typeof(Sprite)) as Sprite;
        imgMan.color = new Color(60f / 255f, 60f / 255f, 60f / 255f, 1f);


        // Кликабельная кнопка с картинкой:
        GameObject pictureBtn = new GameObject("pictureBtn", typeof(Image), typeof(Button), typeof(LayoutElement));
        pictureBtn.transform.SetParent(Canvas.transform);
        RectTransform pictureBtnRT = pictureBtn.GetComponent<RectTransform>();
        pictureBtnRT.localScale = new Vector3(1f, 1f, 1f);
        pictureBtnRT.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        pictureBtnRT.sizeDelta = new Vector2(300f, 300f);
        pictureBtnRT.anchorMin = new Vector2(0.5f, 1f);
        pictureBtnRT.anchorMax = new Vector2(0.5f, 1f);
        pictureBtnRT.anchoredPosition = new Vector3(0f, -280f);
        pictureBtn.GetComponent<Image>().color = new Color(145f / 255f, 75f / 255f, 67f / 255f, 1f);
        pictureBtn.GetComponent<Button>().transition = Selectable.Transition.ColorTint;
        pictureBtn.layer = 5;

        Image imgPictureBtn = pictureBtn.GetComponent<Image>();
        imgPictureBtn.color = new Color(0f, 0f, 0f, 1f);
        pictureBtn.AddComponent<Outline>().effectColor = new Color(1f, 1f, 1f, 1f);
        pictureBtn.GetComponent<Outline>().effectDistance = new Vector2(2f, -2f);

        // Создание кнопоки done:
        GameObject done = new GameObject("Done", typeof(Image), typeof(Button), typeof(LayoutElement));
        done.transform.SetParent(Canvas.transform);
        RectTransform doneRT = done.GetComponent<RectTransform>();
        doneRT.localScale = new Vector3(1f, 1f, 1f);
        doneRT.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        doneRT.sizeDelta = new Vector2(200f, 50f);
        doneRT.anchorMin = new Vector2(0.5f, 1f);
        doneRT.anchorMax = new Vector2(0.5f, 1f);
        doneRT.anchoredPosition = new Vector3(50f, -405f);
        done.GetComponent<Image>().color = new Color(85f / 255f, 114f / 255f, 50f / 255f, 150f / 255f);
        done.GetComponent<Button>().transition = Selectable.Transition.ColorTint;
        done.layer = 5;

        GameObject txtDone = new GameObject(); // Создание текста для кнопки.
        txtDone.name = "Text";
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
        GameObject back = new GameObject("Back", typeof(Image), typeof(Button), typeof(LayoutElement));
        back.transform.SetParent(Canvas.transform);
        RectTransform backRT = back.GetComponent<RectTransform>();
        backRT.localScale = new Vector3(1f, 1f, 1f);
        backRT.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        backRT.sizeDelta = new Vector2(backRT.sizeDelta[0] - 5f, 50f);
        backRT.anchorMin = new Vector2(0.5f, 1f);
        backRT.anchorMax = new Vector2(0.5f, 1f);
        backRT.anchoredPosition = new Vector3(-102f, -405f);
        back.GetComponent<Image>().color = new Color(145f / 255f, 75f / 255f, 67f / 255f, 150f / 255f);
        back.GetComponent<Button>().transition = Selectable.Transition.ColorTint;
        back.layer = 5;

        GameObject txtBack = new GameObject(); // Создание текста для кнопки.
        txtBack.name = "Text";
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

        // Стрелка влево:
        GameObject leftArrow = new GameObject("leftArrow", typeof(Image), typeof(Button), typeof(LayoutElement));
        leftArrow.transform.SetParent(Canvas.transform);
        RectTransform leftArrowRT = leftArrow.GetComponent<RectTransform>();
        leftArrowRT.localScale = new Vector3(1f, 1f, 1f);
        leftArrowRT.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        leftArrowRT.sizeDelta = new Vector2(50f, 50f);
        leftArrowRT.anchorMin = new Vector2(0.5f, 1f);
        leftArrowRT.anchorMax = new Vector2(0.5f, 1f);
        leftArrowRT.anchoredPosition = new Vector3(-190f, -280f);
        leftArrow.GetComponent<Button>().transition = Selectable.Transition.ColorTint;
        leftArrow.layer = 5;
        Image imgleftArrow = leftArrow.GetComponent<Image>();
        imgleftArrow.sprite = Resources.Load("LeftArrowSprite", typeof(Sprite)) as Sprite;
        imgleftArrow.color = new Color(60f / 255f, 60f / 255f, 60f / 255f, 1f);

        // Стрелка вправо:
        GameObject rightArrow = new GameObject("rightArrow", typeof(Image), typeof(Button), typeof(LayoutElement));
        rightArrow.transform.SetParent(Canvas.transform);
        RectTransform rightArrowRT = rightArrow.GetComponent<RectTransform>();
        rightArrowRT.localScale = new Vector3(1f, 1f, 1f);
        rightArrowRT.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        rightArrowRT.sizeDelta = new Vector2(50f, 50f);
        rightArrowRT.anchorMin = new Vector2(0.5f, 1f);
        rightArrowRT.anchorMax = new Vector2(0.5f, 1f);
        rightArrowRT.anchoredPosition = new Vector3(190f, -280f);
        rightArrow.GetComponent<Button>().transition = Selectable.Transition.ColorTint;
        rightArrow.layer = 5;
        Image imgrightArrow = rightArrow.GetComponent<Image>();
        imgrightArrow.sprite = Resources.Load("RightArrowSprite", typeof(Sprite)) as Sprite;
        imgrightArrow.color = new Color(60f / 255f, 60f / 255f, 60f / 255f, 1f);

        // Создаем массив с ресурсами:
        List<Sprite> list = new List<Sprite>();
        for (int i = 0; i < 20; ++i) {
            if (i < 10) list.Add(Resources.Load("ManAndWomanButtons/he/heSprite_" + (i+1).ToString(), typeof(Sprite)) as Sprite);
            else list.Add(Resources.Load("ManAndWomanButtons/she/sheSprite_" + (i+1).ToString(), typeof(Sprite)) as Sprite);
        }


        // ОБРАБОТЧИКИ СОБЫТИЙ:
        man.GetComponent<Button>().onClick.AddListener(() => {
            if (!isManBtnActive && !isWomanBtnActive) {
                clickBtn.Play();
                currentNumberOfChangedModel2D = 0;
                imgMan.color = new Color(1f, 1f, 1f, 1f);

                // Активируем область просмотра:
                pictureBtn.GetComponent<Image>().sprite = list[currentNumberOfChangedModel2D] as Sprite;
                pictureBtn.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

            }
            else if (isManBtnActive && !isWomanBtnActive) {
                // Nothing doing.
            }
            else if (!isManBtnActive && isWomanBtnActive) {
                clickBtn.Play();
                currentNumberOfChangedModel2D = 0;
                imgWoman.color = new Color(60f / 255f, 60f / 255f, 60f / 255f, 1f);
                imgMan.color = new Color(1f, 1f, 1f, 1f);
                pictureBtn.GetComponent<Image>().sprite = list[currentNumberOfChangedModel2D] as Sprite;
            }
            isManBtnActive = true;
            isWomanBtnActive = false;
            imgrightArrow.color = new Color(1f, 1f, 1f, 1f);
            imgleftArrow.color = new Color(1f, 1f, 1f, 1f);
        });

        woman.GetComponent<Button>().onClick.AddListener(() => {
            if (!isManBtnActive && !isWomanBtnActive) {
                clickBtn.Play();
                currentNumberOfChangedModel2D = 10;
                imgWoman.color = new Color(1f, 1f, 1f, 1f);

                // Активируем область просмотра:
                pictureBtn.GetComponent<Image>().sprite = list[currentNumberOfChangedModel2D];
                pictureBtn.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            }
            else if (isManBtnActive && !isWomanBtnActive) {
                clickBtn.Play();
                currentNumberOfChangedModel2D = 10;
                imgMan.color = new Color(60f / 255f, 60f / 255f, 60f / 255f, 1f);
                imgWoman.color = new Color(1f, 1f, 1f, 1f);
                pictureBtn.GetComponent<Image>().sprite = list[currentNumberOfChangedModel2D] as Sprite;
            }
            else if (!isManBtnActive && isWomanBtnActive) {
                // Nothing doing.
            }
            isManBtnActive = false;
            isWomanBtnActive = true;
            imgrightArrow.color = new Color(1f, 1f, 1f, 1f);
            imgleftArrow.color = new Color(1f, 1f, 1f, 1f);
        });

        leftArrow.GetComponent<Button>().onClick.AddListener(() => {
            clickBtn.Play();
            if (isManBtnActive) { // 1 - 10
                switch (currentNumberOfChangedModel2D) {
                    case 0:
                    currentNumberOfChangedModel2D = 9;
                    pictureBtn.GetComponent<Image>().sprite = list[currentNumberOfChangedModel2D];
                    break;
                    default:
                    currentNumberOfChangedModel2D--;
                    pictureBtn.GetComponent<Image>().sprite = list[currentNumberOfChangedModel2D];
                    break;
                }
            }
            else if (isWomanBtnActive) { // 11 - 20
                switch (currentNumberOfChangedModel2D) {
                    case 10:
                    currentNumberOfChangedModel2D = 19;
                    pictureBtn.GetComponent<Image>().sprite = list[currentNumberOfChangedModel2D];
                    break;
                    default:
                    currentNumberOfChangedModel2D--;
                    pictureBtn.GetComponent<Image>().sprite = list[currentNumberOfChangedModel2D];
                    break;
                }
            }
        });

        rightArrow.GetComponent<Button>().onClick.AddListener(() => {
            clickBtn.Play();
            if (isManBtnActive) { // 1 - 10
                switch (currentNumberOfChangedModel2D) {
                    case 9:
                    currentNumberOfChangedModel2D = 0;
                    pictureBtn.GetComponent<Image>().sprite = list[currentNumberOfChangedModel2D];
                    break;
                    default:
                    currentNumberOfChangedModel2D++;
                    pictureBtn.GetComponent<Image>().sprite = list[currentNumberOfChangedModel2D];
                    break;
                }
            }
            else if (isWomanBtnActive) { // 11 - 20
                switch (currentNumberOfChangedModel2D) {
                    case 19:
                    currentNumberOfChangedModel2D = 10;
                    pictureBtn.GetComponent<Image>().sprite = list[currentNumberOfChangedModel2D];
                    break;
                    default:
                    currentNumberOfChangedModel2D++;
                    pictureBtn.GetComponent<Image>().sprite = list[currentNumberOfChangedModel2D];
                    break;
                }
            }
        });

        back.GetComponent<Button>().onClick.AddListener(() => {
            clickBtn.Play();
            currentIndexPage--;

            Destroy(header);
            Destroy(woman);
            Destroy(man);
            Destroy(pictureBtn);
            Destroy(leftArrow);
            Destroy(rightArrow);
            Destroy(back);
            Destroy(done);

            Sign_Slot(1, true, chInfo.Nickname, chInfo.Password);
        });

        done.GetComponent<Button>().onClick.AddListener(() => {
            clickBtn.Play();

            if (isManBtnActive || isWomanBtnActive) {
                // Тут будем записывать нового пользователя в БД и переходить на главное игровое меню.
                StaticClasses.TransitsData.transitNumberOfModel2D = currentNumberOfChangedModel2D;
                string request = "Create_character|" + chInfo.Nickname + "|" + chInfo.Password + "|" + StaticClasses.TransitsData.transitNumberOfModel2D.ToString();
                request = hidden.SH.Encrypt(request);
                statusOfWaiting = "Create_character";
                httpRequest.POST(request);
            }
        });
    }

    private void MainMenu() { // Главное меню игры:
        currentIndexPage = 4;
        
        // Создание UI главного меню путем создания копии из подготовленного префаба:
        GameObject mainManu_prefab = Instantiate(Resources.Load("UIPrefabs/ScrollViewOfMainMenu") as Object, Canvas.transform) as GameObject;
        mainManu_prefab.name = "mainMenu";
        // Идентификация всех необходимых объектов вложенных в данный префаб:
        GameObject Nickname = GameObject.Find("Nickname_Text");
        GameObject pve = GameObject.Find("pveBtn");
        GameObject pvp = GameObject.Find("pvpBtn");
        GameObject pveTop = GameObject.Find("pveTopBtn");
        GameObject pvpTop = GameObject.Find("pvpTopBtn");
        GameObject character = GameObject.Find("characterBtn"); // Характеристики и вещи.
        GameObject store = GameObject.Find("storeBtn");
        GameObject about = GameObject.Find("aboutBtn");


        // Инициализация записи с никнеймом:
        Nickname.GetComponent<Text>().text = chInfo.Nickname;
        
        // ОБРАБОТЧИКИ СОБЫТИЙ ДЛЯ ВСЕХ КНОПОК В ГЛАВНОМ МЕНЮ:
        pve.GetComponent<Button>().onClick.AddListener(() => {
            clickBtn.Play();

            // Получаем индекс стартовой сцены:
            httpRequest.POST("PveIndex|" + chInfo.Nickname + "|" + chInfo.Password);
            statusOfWaiting = "PveIndex";
        });

        pvp.GetComponent<Button>().onClick.AddListener(() => {
            clickBtn.Play();

            mainManu_prefab.SetActive(false); // Делаем главное меню временно не активным.

            // Загружаем префаб с подменю:

            // ...

        });

        pveTop.GetComponent<Button>().onClick.AddListener(() => {
            clickBtn.Play();

            // Запрос на получение данных о списке лучших игроков в режиме PVE:
            httpRequest.POST("PveTopDatas|" + chInfo.Nickname + "|" + chInfo.Password);
            statusOfWaiting = "PveTopDatas";
        });

        pvpTop.GetComponent<Button>().onClick.AddListener(() => {
            clickBtn.Play();

            // Запрос на получение данных о списке лучших игроков в режиме PVP:
            httpRequest.POST("PveTopDatas|" + chInfo.Nickname + "|" + chInfo.Password);
            statusOfWaiting = "PveTopDatas";
        });

        character.GetComponent<Button>().onClick.AddListener(() => {
            clickBtn.Play();

            // Запрос на получение данных об:
                // -- Максимальное здоровье
                // -- Максимальная мана *
                // -- Сила
                // -- Наличие всех имеющихся предметов включая золото
                // -- Наличие оружия взятого в руке
                // -- Достижения персонажа: текущий скор, максимальный, уровень в пве, кол-во побед и поражений.
            httpRequest.POST("CharactersInformation|" + chInfo.Nickname + "|" + chInfo.Password);
            statusOfWaiting = "CharactersInformation";
        });

        store.GetComponent<Button>().onClick.AddListener(() => {
            clickBtn.Play();

            httpRequest.POST("StoreList|" + chInfo.Nickname + "|" + chInfo.Password);
            statusOfWaiting = "StoreList";
        });

        about.GetComponent<Button>().onClick.AddListener(() => {
            clickBtn.Play();

            mainManu_prefab.SetActive(false); // Делаем главное меню временно не активным.

            // Загружаем префаб с подменю:

            // ...

        });
    }

    IEnumerator SetTimeoutChangeColor(float sec, Vector4 vectorColor, string name, string name2 = "") {
        yield return new WaitForSeconds(sec);
        
        Canvas.transform.Find(name).Find(name2).gameObject.GetComponent<Image>().color =
            new Color(vectorColor[0], vectorColor[1], vectorColor[2], vectorColor[3]);
    }

}
