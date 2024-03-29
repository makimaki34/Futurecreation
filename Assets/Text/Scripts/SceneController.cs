﻿using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public Actions Actions;
    public List<CharacterIcon> CharaIcons = new List<CharacterIcon>();//このように書きながらも結局インスタンス一つしか生成してないです(簡単に複数にも出来ます)
    public List<Character> Characters = new List<Character>();    //このように書きながらも結局インスタンス一つしか生成してないです(簡単に複数にも出来ます)
    public List<Background> Backgrounds = new List<Background>();   //このように書きながらも結局インスタンス一つしか生成してないです(簡単に複数にも出来ます)
    public List<Score> Scores = new List<Score>();
    public List<Fade> _Fade = new List<Fade>();
    public Fade fade;
    private Sound _Sound;
    public string sceneTxtname { private get; set; }
    public string sceneLoadName {  get; set; }//ロードで必要なもの
    public int loadnum = 0;
    private GameController _gc;
    private GUIManager _gui;
    private SceneHolder _sh;
    private SceneReader _sr;
    private Sequence _textSeq  = DOTween.Sequence();
    private Sequence _imageSeq = DOTween.Sequence();
    private Scene _currentScene;
    public  bool _isOptionsShowed;
    private bool _isSaveShowed;
    private string _tempText;
    private float _messageSpeed = 0.1f;
    private bool _charaAction = false;
    private GameObject canvas;
    private GameObject targetGameObject;
    private string _name="";
    public string ID = "";
    public string Save_options;

    public SceneController(GameController _gc)
    {
        this._gc = _gc;
        _gui = GameObject.Find("GUI").GetComponent<GUIManager>();
        
        Actions = new Actions(_gc);
        _sh = new SceneHolder(this);
        _sr = new SceneReader(this);
        _textSeq.Complete();
        sceneTxtname = "";
        //ここはセーブのウィンドウが開いているか開いていないかでテキストのクリックを止める処理
        canvas = GameObject.Find("Canvas");
        foreach (Transform child in canvas.transform)
        {
            if (child.name == "SaveLoadWindow")
            {
                targetGameObject = child.gameObject;
            }
        }
        targetGameObject.SetActive(false);
    }

    public void WaitClick()
    {
        if (_currentScene != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //  キャラの座標戻し
                //if (_charaAction)
                //{
                //    var character = Characters.Find(c => c.Name == "Characters");
                //    character.action = false;
                //    _charaAction = false;
                //}

                //ここでタップ判定(テキストを進めていいのかどうか)
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    Vector2 tapPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Collider2D collition2d = Physics2D.OverlapPoint(tapPoint);
                    if (collition2d != null || targetGameObject.activeSelf)
                    {
                        loadnum = _currentScene.Index - 1;
                        sceneLoadName = _name;
                        ID = sceneTxtname;
                        
                        return;
                    }
                }

                if (!_isOptionsShowed && !_imageSeq.IsPlaying() && !_isSaveShowed)
                {
                    Save_options = "";
                    SetNextProcess();      
                }
            }
            if(Input.GetKey(KeyCode.LeftControl))
                SetNextProcess();
            if (_Fade.Find(c => c.GetActive()) && fade._playInfade)
            {
                if (!fade._playOutfade)
                {
                    SetNextProcess();
                    fade._playOutfade = true;
                }
            }
        }
    }

    public void SetComponents()
    {
        _gui.ButtonPanel.gameObject.SetActive(_isOptionsShowed);
        _gui.Delta.gameObject.SetActive
            (!_textSeq.IsPlaying() && !_isOptionsShowed && !_imageSeq.IsPlaying());
    }

    public void SetNextProcess()
    {
        if (_textSeq.IsPlaying())
        {
            SetText(_tempText);
        }
        else
        {
            _sr.ReadLines(_currentScene);
        }
    }

    public void SetScene(string id)
    {
        _currentScene = _sh.Scenes.Find(s => s.ID == id + sceneTxtname);
        _name = id + sceneTxtname;
        _currentScene = _currentScene.Clone();
        if (_currentScene == null) Debug.LogError("scenario not found");
            SetNextProcess();
    }
    // 文字の出現回数をカウント
    public static int CountChar(string s, char c)
    {
        return s.Length - s.Replace(c.ToString(), "").Length;
    }

    public void LoadScene(string LoadName, int num, string id,string line)
    {
        _currentScene = _sh.Scenes.Find(s => s.ID == LoadName);
        _currentScene = _currentScene.Clone();
        if (_currentScene == null) Debug.LogError("scenario not found");
        _currentScene.LoadLine(num);
        sceneTxtname = id;
        if (line != ""&& line !=null)
        {
            int i = 0;
            var options = new List<(string, string)>();
            while (true)
            {
                if (i< CountChar(line, ';'))
                {
                    var splitted = line.Split(';');

                    var splitted2 = splitted[i].Split(':');
                   
                    options.Add((splitted2[0], splitted2[1]));          
                    i++;
                }
                else
                {
                    _gui.ButtonPanel.gameObject.SetActive(true);
                    SetOptions(options);
                    SetNextProcess();
                    break;
                }
            }          
        }
        else
        {
            _isOptionsShowed = false;
            SetNextProcess();
        }
        targetGameObject.SetActive(false);
    }

    public void SetText(string text)
    {
        _tempText = text;
        if (_textSeq.IsPlaying())
        {
            _textSeq.Complete();
        }
        else
        {
            _gui.Text.text = "";
            _textSeq = DOTween.Sequence();
            _textSeq.Append
                (_gui.Text.DOText
                (
                    text,
                    text.Length * _messageSpeed
                ).SetEase(Ease.Linear));
        }
    }

    public void SetOptionsPanel()
    {
        _gui.ButtonPanel.gameObject.SetActive(_isOptionsShowed);
    }

    public void SetSpeaker(string name = "")
    {
        _gui.Speaker.text = name;
    }


    //特別なアクション処理//////////////////////////////////////////////////
    public void Action(string num)
    {
        int num2 = int.Parse(num);
        Actions.SelectAction(num2);
    }

    public void ActionMove()
    {
        var character = Characters.Find(c => c.Name == "Characters");
        _charaAction = true;
        character.Move();
    }

    /////////////////////////////////////////////////////////////////////


    //キャラクター処理//////////////////////////////////////////////////
    public void SetCharacter(string name)
    {
        Characters.ForEach(c => c.Destroy());
        Characters = new List<Character>();
        AddCharacter(name);
    }
    public void AddCharacter(string name)
    {
        if (Characters.Exists(c => c.Name == name)) return;

        var prefab = Resources.Load("Prefabs/Character") as GameObject;
        var charactorObject = Object.Instantiate(prefab);
        var character = charactorObject.GetComponent<Character>();

        character.Init(name);
        Characters.Add(character);
        _imageSeq = DOTween.Sequence();

        var cpos = new Vector3(0, _gui.MainCamera.transform.position.y, 0);
        _imageSeq.Append(Characters[0].transform.DOMove(cpos, 0f))
            .OnComplete(() => character.Appear());
    }

    public void SetCharaImage(string name, string ID,float x,float y,float scale)
    {
        var character = Characters.Find(c => c.Name == name);
        character.SetImage(ID,x,y,scale);

    }
    //////////////////////////////////////////////////////////////////////

    //キャラアイコン処理//////////////////////////////////////////////////
    public void SetCharaIcon(string name)
    {
        CharaIcons.ForEach(c => c.Destroy());
        CharaIcons = new List<CharacterIcon>();
        AddCharaIcon(name);
    }
    public void AddCharaIcon(string name)
    {
        if (CharaIcons.Exists(c => c.Name == name)) return;

        var prefab = Resources.Load("Prefabs/CharacterIcon") as GameObject;
        var charactorObject = Object.Instantiate(prefab);
        var character = charactorObject.GetComponent<CharacterIcon>();

        character.Init(name);
        CharaIcons.Add(character);
    }
    public void SetIcoImage(string name, string ID)
    {
        var character = CharaIcons.Find(c => c.Name == name);
        character.SetImage(ID);
    }
    //////////////////////////////////////////////////////////////////////

    //フェード処理//////////////////////////////////////////////////
    public void SetFade(string name)
    {
        _Fade.ForEach(c => c.Destroy());
        _Fade = new List<Fade>();
        AddFade(name);
    }
    public void AddFade(string name)
    {
        if (_Fade.Exists(c => c.Name == name)) return;

        var prefab = Resources.Load("Prefabs/Fade") as GameObject;
        var fadeObject = Object.Instantiate(prefab);
        var fade = fadeObject.GetComponent<Fade>();

        fade.Init(name);
        _Fade.Add(fade);
    }
    public void SetFadeInImage(string name, string ID, int COLOR)
    {
        fade = _Fade.Find(c => c.Name == name);
        fade.Appear();
        fade.SetInImage(ID, COLOR);
    }
    public void SetFadeOutImage(string name, string ID, int COLOR)
    {
        fade = _Fade.Find(c => c.Name == name);
        fade.Appear();
        fade._playOutfade = false;
        fade.SetOutImage(ID, COLOR);
    }
    //////////////////////////////////////////////////////////////////////

    //背景処理////////////////////////////////////////////////////////////
    public void SetBackground(string name)
    {
        Backgrounds.ForEach(c => c.Destroy());
        Backgrounds = new List<Background>();
        AddBackground(name);
    }
    public void AddBackground(string name)
    {
        if (Backgrounds.Exists(c => c.Name == name)) return;

        var prefab = Resources.Load("Prefabs/Background") as GameObject;
        var backgroundObject = Object.Instantiate(prefab);
        var background = backgroundObject.GetComponent<Background>();

        background.Init(name);
        Backgrounds.Add(background);
    }
    public void SetBgImage(string name, string ID)
    {
        var background = Backgrounds.Find(c => c.Name == name);
        background.SetImage(ID);
    }
    //////////////////////////////////////////////////////////////////////

    //スコア処理////////////////////////////////////////////////////////////
    public void AddScore(string name)
    {
        if (Scores.Exists(c => c.Name == name)) return;
        var prefab = Resources.Load("Prefabs/CharaScore") as GameObject;
        var scoreobject = Object.Instantiate(prefab);
        var score = scoreobject.GetComponent<Score>();
        score.Init(name);
        Scores.Add(score);
    }
    public void SetScore(string name, string num)
    {
        var score = Scores.Find(c => c.Name == name);
        int num2 = int.Parse(num);
        score.ChangeScore(num2);
    }
    //////////////////////////////////////////////////////////////////////

    //サウンド処理////////////////////////////////////////////////////////////
    public void AddSound()
    {
        var prefab = Resources.Load("Prefabs/SoundManager") as GameObject;
        var soundobject = Object.Instantiate(prefab);
        var sound = soundobject.GetComponent<Sound>();
        _Sound = sound;
        _Sound.Init();
    }
    public void ChangeBGM(string num)
    {
        _Sound.ChangeBGM(num);
    }
    public void ChangeSE(string num, string flag)
    {
        int flag2 = int.Parse(flag);
        _Sound.ChangeSE(num, flag2);
    }
    //////////////////////////////////////////////////////////////////////
    public void SetOptions(List<(string text, string nextScene)> options)
    {
        if (!_isOptionsShowed)
        {
            _isOptionsShowed = true;
            foreach (var o in options)
            {
                Button b = Object.Instantiate(_gui.OptionButton);
                Text text = b.GetComponentInChildren<Text>();
                text.text = o.text;
                b.onClick.AddListener(() => onClickedOption(o.nextScene));
                b.transform.SetParent(_gui.ButtonPanel, false);
            }
        }
    }

    public void Save_Options(string data)
    {
        
        Save_options += data;
        Debug.Log(Save_options);
    }

    public void onClickedOption(string nextID = "")
    {
        SetScene(nextID);
        _isOptionsShowed = false;

        foreach (Transform t in _gui.ButtonPanel)
        {
            UnityEngine.Object.Destroy(t.gameObject);
        }
    }
}