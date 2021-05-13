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
    public string RuEn = "";
}

[System.Serializable]
public class CharacterInfo {
    public string Nickname = "";
    public string Password = "";
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

    HttpRequest httpRequest;
    private string statusOfWaiting = "";

    private string Language;


    // Errors:
    private GameObject errInputs; // Row with error at menu of inputs

    private void Awake() {
        gameObject.AddComponent<HttpRequest>(); // Добавляем скрипт к объекту.
        httpRequest = Camera.main.gameObject.GetComponent<HttpRequest>();
        httpRequest.Emit_answer += Checking;
    }

    void Start()
    {
        Canvas = GameObject.Find("Canvas");
        clickBtn = GameObject.Find("ClickBtn").GetComponent<AudioSource>();

        RuEn();
    }

    // Update is called once per frame
    void Update(){}

    protected void RuEn() {

        RecordAndReadFile file = new RecordAndReadFile(RecordAndReadFile.fileName);
        ArrayCharacterInfo obj = null;
        bool bf = false;
        if (file.FileExists()) {
            // Читаем файл:
            obj = file.ReadFromFile();

            string chInfo = obj.RuEn.ToString();

            Language = (chInfo == "En" || chInfo == "Ru") ? chInfo : "";

            if(Language != "") {
                startPageUI();
                print(Language);
            }
            else {
                bf = true;
            }
        }
        else { // Если же не существует, тогда переходим к выбору языка:
            bf = true;
        }

        if (bf) { // Выбор языка:
            GameObject RuEn_prefab = Instantiate(Resources.Load("Prefabs/UIPrefabs/RuEn/Prefab_RuEn"), Canvas.transform) as GameObject;
            RuEn_prefab.name = "RuEn";

            GameObject Ru = RuEn_prefab.transform.Find("RuBtn").gameObject;
            GameObject En = RuEn_prefab.transform.Find("EnBtn").gameObject;

            Ru.GetComponent<Button>().onClick.AddListener(() => {
                clickBtn.Play();
                Language = "Ru";
                Destroy(RuEn_prefab);
                startPageUI();
                
                if (obj == null) {
                    obj = new ArrayCharacterInfo();
                    obj.RuEn = Language;
                }
                else {
                    obj.RuEn = Language;
                    file.RecordInFile(obj);
                }
            });

            En.GetComponent<Button>().onClick.AddListener(() => {
                clickBtn.Play();
                Language = "En";
                Destroy(RuEn_prefab);
                startPageUI();

                if (obj == null) {
                    obj = new ArrayCharacterInfo();
                    obj.RuEn = Language;
                }
                else {
                    obj.RuEn = Language;
                    file.RecordInFile(obj);
                }
            });
        }
    }

    // Обработчик событий на тикие простые события как проверка уникальности ника, существования учетки и создания уч.:
    public void Checking(string Answer) {
        if (statusOfWaiting == "CkeckOnUniqNickname") { // ++
            if (Answer == "true") {

                // Создаем нового персонажа:
                string request = "Create_character|" + chInfo.Nickname + "|" + chInfo.Password;
                httpRequest.POST(request);
                statusOfWaiting = "Create_character";

            }
            else if(Answer == "false") {
                Canvas.transform.Find("signInputNickname").gameObject.transform.Find("Border").gameObject.GetComponent<Image>().color = new Color(90f / 255, 20f / 255f, 20f / 255f, 1f);
                StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), GameObject.Find("signInputNickname").transform.Find("Border").gameObject));
                string mtext = (Language == "En") ? "The nickname you entered is not unique !" : "Никнейм не является уникальным !";
                MessageOfError(mtext, -120f);
            }
            else { // error
                Canvas.transform.Find("signInputNickname").gameObject.transform.Find("Border").gameObject.GetComponent<Image>().color = new Color(90f / 255, 20f / 255f, 20f / 255f, 1f);
                string mtext = (Language == "En") ? "An error occurred while sending your request. There may be a temporary problem with the server." : "Произошла ошибка при отправке запроса. Возможно есть временные проблемы с сервером.";
                MessageOfError(mtext, -120f);
            }
        }
        else if (statusOfWaiting == "CheckOnExistingSuchNickname") { // +++
            if (Answer == "true") {

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
                    //GameObject.Find("signInputNickname").GetComponent<InputField>().text = chInfoList[0].Nickname;
                    //GameObject.Find("signInputPassword").GetComponent<InputField>().text = chInfoList[0].Password;
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

                MainMenu();
            }
            else if (Answer == "false") {
                Canvas.transform.Find("signInputNickname").gameObject.transform.Find("Border").gameObject.GetComponent<Image>().color = new Color(90f / 255, 20f / 255f, 20f / 255f, 1f);
                StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), GameObject.Find("signInputNickname").transform.Find("Border").gameObject));
                string mtext = (Language == "En") ? "Account with nickname " + chInfo.Nickname + " does not exist !" 
                    :"Акаунта с никнеймом " + chInfo.Nickname + " не существует !";
                MessageOfError(mtext, -120f);
            }
            else { // error
                Canvas.transform.Find("signInputNickname").gameObject.transform.Find("Border").gameObject.GetComponent<Image>().color = new Color(90f / 255, 20f / 255f, 20f / 255f, 1f);
                string mtext = (Language == "En") ? "An error occurred while sending your request. There may be a temporary problem with the server." : "Произошла ошибка при отправке запроса. Возможно есть временные проблемы с сервером.";
                MessageOfError(mtext, -120f);
            }
        }
        else if(statusOfWaiting == "DeleteAccount") {
            if(Answer == "true") {

                // Удаляем запись из локального файла:
                RecordAndReadFile file = new RecordAndReadFile(RecordAndReadFile.fileName);
                if (file.FileExists()) {
                    // Читаем файл:
                    ArrayCharacterInfo obj = file.ReadFromFile();

                    // Поиск актуальной записи - та запись у которой isActive == true является актуальной:
                    List<CharacterInfo> chInfoList = (from rec in obj.list
                                                      select rec).ToList();
                    if (chInfo != null) {
                        for(int i=0; i < chInfoList.Count; ++i) {
                            if(chInfoList[i].Nickname == chInfo.Nickname && chInfoList[i].Password == chInfo.Password) {
                                chInfoList.RemoveAt(i);
                                break;
                            }
                        }

                        // Перезаписываем локальный файл:
                        file.RecordInFile(obj);

                    }
                }

                startPageUI();

                // Удаляем ненужные объекты текущей страницы:
                Destroy(GameObject.Find("signHeader"));
                Destroy(GameObject.Find("signDescription"));
                Destroy(GameObject.Find("signBack"));
                Destroy(GameObject.Find("signDone"));
                Destroy(GameObject.Find("signLook"));
                Destroy(GameObject.Find("signInputNickname"));
                Destroy(GameObject.Find("signInputPassword"));

            }
            else if (Answer == "false") {
                Canvas.transform.Find("signInputNickname").gameObject.transform.Find("Border").gameObject.GetComponent<Image>().color = new Color(90f / 255, 20f / 255f, 20f / 255f, 1f);
                StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), GameObject.Find("signInputNickname").transform.Find("Border").gameObject));
                string mtext = (Language == "En") ? "This account does not exist!" : "Такого аккаунта не существует !";
                MessageOfError(mtext, -120f);
            }
            else { // error
                Canvas.transform.Find("signInputNickname").gameObject.transform.Find("Border").gameObject.GetComponent<Image>().color = new Color(90f / 255, 20f / 255f, 20f / 255f, 1f);
                string mtext = (Language == "En") ? "An error occurred while sending your request. There may be a temporary problem with the server." : "Произошла ошибка при отправке запроса. Возможно есть временные проблемы с сервером.";
                MessageOfError(mtext, -120f);
            }
        }
        else if (statusOfWaiting == "Create_character") { // +++
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

                // Удаляем ненужные объекты текущей страницы:
                Destroy(GameObject.Find("signHeader"));
                Destroy(GameObject.Find("signDescription"));
                Destroy(GameObject.Find("signBack"));
                Destroy(GameObject.Find("signDone"));
                Destroy(GameObject.Find("signLook"));
                Destroy(GameObject.Find("signInputNickname"));
                Destroy(GameObject.Find("signInputPassword"));

                MainMenu();
            }
            else {
                Canvas.transform.Find("signInputNickname").gameObject.transform.Find("Border").gameObject.GetComponent<Image>().color = new Color(90f / 255, 20f / 255f, 20f / 255f, 1f);
                string mtext = (Language == "En") ? "An error occurred while sending your request. There may be a temporary problem with the server." : "Произошла ошибка при отправке запроса. Возможно есть временные проблемы с сервером.";
                MessageOfError(mtext, -120f);
            }
        }
        else if(statusOfWaiting == "PveIndex") { // Получаем индекс стартовой сцены:
            int IndexScene = Convert.ToInt32(Answer);

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
                textnewCharacter.text = (Language == "En") ? "Continue..." : "Продолжить...";
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
                textchange_02.text = (Language == "En") ? "Start tournament..." : "Начать турнир...";
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
        else if (statusOfWaiting == "PveTopDatas") { // Получить список топа pve:
            GameObject.Find("mainMenu").SetActive(false); // Делаем главное меню временно не активным.
            // Загружаем префаб с подменю:

            // ...
        }
        else if (statusOfWaiting == "PvpTopDatas") { // Получиться список топа pvp:
            GameObject.Find("mainMenu").SetActive(false); // Делаем главное меню временно не активным.
            // Загружаем префаб с подменю:

            // ...
        }
        else if (statusOfWaiting == "CharactersInformation") { // Получаем все достежения по персонажу:
            GameObject.Find("mainMenu").SetActive(false); // Делаем главное меню временно не активным.
            // Загружаем префаб с подменю:

            // ...
        }
        else if (statusOfWaiting == "StoreList") { // Получаем список всех продаваемых товаров:
            GameObject.Find("mainMenu").SetActive(false); // Делаем главное меню временно не активным.
            // Загружаем префаб с подменю:

            // ...
        }
        // ...


        statusOfWaiting = "";
    }

    private void startPageUI() {
        // Создание стартового UI меню:

        currentIndexPage = 0;

        GameObject startMenu = Instantiate(Resources.Load("Prefabs/UIPrefabs/StartMenu/Prefab_StartMenu") as GameObject);
        startMenu.name = "Prefab_StartMenu";
        startMenu.transform.SetParent(Canvas.transform);
        startMenu.transform.localScale = Vector3.one;

        RectTransform RTStarnMenu = startMenu.GetComponent<RectTransform>();
        RTStarnMenu.pivot = new Vector2(0.5f, 0.5f);
        RTStarnMenu.offsetMin = new Vector2(0f, 0);
        RTStarnMenu.offsetMax = new Vector2(1f, 1f);
        RTStarnMenu.transform.localPosition = Vector3.zero;

        GameObject newCharacter, haveCharacter, deleteCharacter;
        newCharacter = startMenu.transform.Find("CreateNewCharacter").gameObject;
        haveCharacter = startMenu.transform.Find("IAlreadyHaveTheCharacter").gameObject;
        deleteCharacter = startMenu.transform.Find("IWantToDeleteCharacter").gameObject;

        newCharacter.transform.Find("Text").GetComponent<Text>().text =
                                                (Language == "En") ? "Create a new character..." : "Создать персонажа...";

        haveCharacter.transform.Find("Text").GetComponent<Text>().text =
                                                (Language == "En") ? "I already have the character..." : "У меня уже есть персонаж...";

        deleteCharacter.transform.Find("Text").GetComponent<Text>().text =
                                                (Language == "En") ? "Delete character..." : "Удалить персонажа...";



        // Обработчики событий для кнопок:
        newCharacter.GetComponent<Button>().onClick.AddListener(() => {
            Destroy(newCharacter);
            Destroy(haveCharacter);
            Destroy(deleteCharacter);
            Destroy(startMenu);
            Sign_Slot(1);
        });
        haveCharacter.GetComponent<Button>().onClick.AddListener(() => {
            Destroy(newCharacter);
            Destroy(haveCharacter);
            Destroy(deleteCharacter);
            Destroy(startMenu);
            Sign_Slot(2);
        });
        deleteCharacter.GetComponent<Button>().onClick.AddListener(() => {
            Destroy(newCharacter);
            Destroy(haveCharacter);
            Destroy(deleteCharacter);
            Destroy(startMenu);

            Sign_Slot(-1);

            print("Данный блок требует реализации на сервере...");
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
        if (input.name == "signInputNickname") {
            if(currentIndexPage == 1) phText.text = (Language == "En") ? "Pick a nickname..." : "Придумайте никнейм...";
            else if(currentIndexPage == 2 || currentIndexPage == -1) phText.text = (Language == "En") ? "Enter nickname..." : "Введиде ваш никнейм...";
        }
        else {
            if(currentIndexPage == 1) phText.text = (Language == "En") ? "Pick a password..." : "Придумайте пароль...";
            else if (currentIndexPage == 2 || currentIndexPage == -1) phText.text = (Language == "En") ? "Enter password..." : "Введите пароль...";

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
                if(input.gameObject.name == "signInputNickname") text.text = (Language == "En") ? "Input your nickname..." : "Введиде ваш никнейм...";
                else text.text = (Language == "En") ? "Pick a password..." : "Придумайте пароль...";
            }
        });

        if (nameObj == "signInputNickname") {
            EventSystem.current.SetSelectedGameObject(input.gameObject, null); // Устанавливаем фокус.
        }
    }

    public void Sign_Slot(int nextIndex) { // Пользователь хочет создать нового персонажа или же продолжить играть уже существующим:

        clickBtn.Play();
        currentIndexPage = nextIndex; // 1, 2 или -1

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
        if (currentIndexPage == 1) header.GetComponent<Text>().text = (Language == "En") ? "Creating your character:" : "Создание персонажа:";
        else if (currentIndexPage == 2) header.GetComponent<Text>().text = (Language == "En") ? "Sign in to your account:" : "Войдите в свой акаунт:";
        else if(currentIndexPage == -1) header.GetComponent<Text>().text = (Language == "En") ? "Deleting a character:" : "Удаление персонажа:";
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
        if (currentIndexPage == 1) description.GetComponent<Text>().text = (Language == "En") ? "Come up with a nickname that will be unique in this game!" 
                : "Придумайте никнейм, который будет уникальным в этой игре!";
        else if (currentIndexPage == 2) description.GetComponent<Text>().text = (Language == "En") ? "Enter the nickname and password for the previously created account!" 
                : "Введите ник и пароль от ранее созданной учетной записи!";
        else if(currentIndexPage == -1) description.GetComponent<Text>().text = (Language == "En") ? "Enter your nickname and password to delete your account!"
                : "Введите ник и пароль для удаления учетной записи!";

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
        textDone.text = (Language == "En") ? "Done" : "Далее";
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
        textBack.text = (Language == "En") ? "Back" : "Назад";
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
        look.GetComponent<Image>().sprite = Resources.Load("Sprites/Buttons/iceSprite", typeof(Sprite)) as Sprite;
        look.GetComponent<Button>().transition = Selectable.Transition.ColorTint;
        look.layer = 5;

        

        if(currentIndexPage == 2) { // Если пытаемся зайти в одну из ранее созданных учеток, то имеет смысл
            // подставить в поля ввода данные из последней играбельной (далее по тексту актуальной) учетки:
            // Проверяем, есть ли файл CharacterInfo существует, то загружаем информацию о логине, пароле и т.д. и подставляем в поля:
            RecordAndReadFile file = new RecordAndReadFile(RecordAndReadFile.fileName);
            if (file.FileExists()) {
                // Читаем файл:
                ArrayCharacterInfo obj = file.ReadFromFile();
                if(obj != null) {
                    // Поиск актуальной записи - та запись у которой isActualCharacter == true является актуальной:
                    List<CharacterInfo> chin = (from rec in obj.list
                                                  where rec.isActualCharacter == true
                                                  select rec).ToList();

                    if (chin != null && chin.Count != 0) {
                        // Записываем данные в input поля:
                        GameObject.Find("signInputNickname").GetComponent<InputField>().text = chin[0].Nickname;
                        GameObject.Find("signInputPassword").GetComponent<InputField>().text = chin[0].Password;
                        chInfo.Nickname = chin[0].Nickname;
                        chInfo.Password = chin[0].Password;
                    }
                }
            }
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
            chInfo.Nickname = Canvas.transform.Find("signInputNickname").gameObject.GetComponent<InputField>().text;
            chInfo.Password = Canvas.transform.Find("signInputPassword").gameObject.GetComponent<InputField>().text;

            if (chInfo.Nickname != "" && chInfo.Password != "") {
                Regex regularForNickname = new Regex("^[A-Za-zА-Яа-я]{2}[A-Za-zА-Яа-я0-9 _+=*&$!@#%\",.?]*$");
                Regex regularForPassword = new Regex("^[A-Za-zА-Яа-я0-9 _+=*&$!@#%\",.?]*$");

                if (regularForNickname.IsMatch(chInfo.Nickname)) {
                    if (regularForPassword.IsMatch(chInfo.Password)) {

                        if (currentIndexPage == 1) { // Проверка логина на уникальность, перед тем как создать персонажа
                            httpRequest.POST("CkeckOnUniqNickname|" + chInfo.Nickname);
                            statusOfWaiting = "CkeckOnUniqNickname";
                        }
                        else if (currentIndexPage == 2) { // 
                            /* Здесь будем проверять существует ли такая запись в БД и если да, то проходим в основное меню игры:  */
                            httpRequest.POST("CheckOnExistingSuchNickname|" + chInfo.Nickname + "|" + chInfo.Password);
                            statusOfWaiting = "CheckOnExistingSuchNickname";
                        }
                        else if(currentIndexPage == -1) { // Удаление учетной записи.
                            httpRequest.POST("DeleteAccount|" + chInfo.Nickname + "|" + chInfo.Password);
                            statusOfWaiting = "DeleteAccount";
                        }
                    }
                    else {
                        Canvas.transform.Find("signInputPassword").gameObject.transform.Find("Border").gameObject.GetComponent<Image>().color = new Color(90f / 255, 20f / 255f, 20f / 255f, 1f);
                        StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), GameObject.Find("signInputNickname").transform.Find("Border").gameObject));
                        string mtext = (Language == "En") ? "Invalid input !" : "Не корректный ввод !";
                        MessageOfError(mtext, -120f);
                    }
                }
                else {
                    Canvas.transform.Find("signInputNickname").gameObject.transform.Find("Border").gameObject.GetComponent<Image>().color = new Color(90f / 255, 20f / 255f, 20f / 255f, 1f);
                    StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), GameObject.Find("signInputNickname").transform.Find("Border").gameObject));
                    string mtext = (Language == "En") ? "Invalid input !" : "Не корректный ввод !";
                    MessageOfError(mtext, -120f);
                }
            }
            else {
                if (chInfo.Nickname == "") {
                    Canvas.transform.Find("signInputNickname").gameObject.transform.Find("Border").gameObject.GetComponent<Image>().color = new Color(90f / 255, 20f / 255f, 20f / 255f, 1f);
                    StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), GameObject.Find("signInputNickname").transform.Find("Border").gameObject));
                }
                if (chInfo.Password == "") {
                    Canvas.transform.Find("signInputPassword").gameObject.transform.Find("Border").gameObject.GetComponent<Image>().color = new Color(90f/255, 20f/255f, 20f/255f, 1f);
                    StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), GameObject.Find("signInputPassword").transform.Find("Border").gameObject));
                }
                string mtext = (Language == "En") ? "One of the fields is empty !" : "Одно из полей не заполнено !";
                MessageOfError(mtext, -120f);
            }
        });

        // Обработчик события на нажатие кнопки Back:
        back.GetComponent<Button>().onClick.AddListener(() => {
            clickBtn.Play();

            // Удаляем ненужные объекты текущей страницы:
            if (errInputs != null) Destroy(errInputs);
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

        errInputs = new GameObject("errInputs", typeof(Text), typeof(LayoutElement));
        errInputs.transform.SetParent(Canvas.transform);
        RectTransform rterrInputs = errInputs.GetComponent<RectTransform>();
        rterrInputs.localScale = new Vector3(1f, 1f, 1f);
        rterrInputs.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        rterrInputs.anchorMin = new Vector2(0.5f, 1f);
        rterrInputs.anchorMax = new Vector2(0.5f, 1f);
        rterrInputs.sizeDelta = new Vector2(400f, 20f); // Set width and height
        rterrInputs.localPosition = new Vector3(0f, 0f, 0f);
        rterrInputs.anchoredPosition = new Vector3(0f, posY, 0f);
        errInputs.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        errInputs.GetComponent<Text>().fontStyle = FontStyle.Italic;
        errInputs.GetComponent<Text>().fontSize = 10;
        errInputs.GetComponent<Text>().text = str;
        errInputs.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        errInputs.layer = 5;
        Text errInputsText = errInputs.GetComponent<Text>();
        errInputsText.color = new Color(1f, 0f, 0f, 1f);
        StartCoroutine(SetTimeoutChangeColor(3f, new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f), GameObject.Find("signInputNickname").transform.Find("Border").gameObject));

        StartCoroutine(DestroyWithCheckingObject(3f, errInputs));
    }

    private void MainMenu() { // Главное меню игры:
        currentIndexPage = 3;
        
        // Создание UI главного меню путем создания копии из подготовленного префаба:
        GameObject mainManu_prefab = Instantiate(Resources.Load("Prefabs/UIPrefabs/ScrollViewOfMainMenu"), Canvas.transform) as GameObject;
        mainManu_prefab.name = "mainMenu";
        // Идентификация всех необходимых объектов вложенных в данный префаб:
        GameObject Nickname = GameObject.Find("Nickname_Text");
        GameObject pve = GameObject.Find("pve");
        GameObject pvp = GameObject.Find("pvp");
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
            httpRequest.POST("PvpTopDatas|" + chInfo.Nickname + "|" + chInfo.Password);
            statusOfWaiting = "PvpTopDatas";
        });

        character.GetComponent<Button>().onClick.AddListener(() => {
            clickBtn.Play();

            // Запрос на получение данных об:
                // -- Максимальное здоровье
                // -- Максимальная мана *
                // -- Сила
                // -- Текущий пве скор
                // -- Максимальный пве скор
                // -- Место в пве // Отдельным запросом
                // -- Место в пвп
                // -- Золото
                // -- Наличие всех имеющихся предметов
                // -- Наличие оружия взятого в руке // из локального источника

            httpRequest.POST("CharactersInformation|" + chInfo.Nickname + "|" + chInfo.Password);
            statusOfWaiting = "CharactersInformation";
        });

        store.GetComponent<Button>().onClick.AddListener(() => {
            // Отправляем запрос на получение списка всех товаров для продажи:
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

    IEnumerator DestroyWithCheckingObject(float sec, GameObject obj) {
        yield return new WaitForSeconds(sec);
        if (obj != null) Destroy(obj);
    }
    IEnumerator SetTimeoutChangeColor(float sec, Vector4 vectorColor, GameObject refObj) {
        yield return new WaitForSeconds(sec);
        
        if(refObj != null) {
            refObj.GetComponent<Image>().color = new Color(vectorColor[0], vectorColor[1], vectorColor[2], vectorColor[3]);
        }
    }

}
