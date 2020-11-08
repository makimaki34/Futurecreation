﻿
using System.Collections.Generic;
using System;


namespace Tag
{
    public class Taglist
    {
        public interface ITag
        {
            void Do(SceneController _sc, string line, Scene s);
        }

        public ITag CreateTag(string className)
        {

            Type type = TagJudge(className);
            if (type == null)
            {
                throw new ArgumentException();
            }
            return (ITag)Activator.CreateInstance(type);
        }

        public class Speaker : ITag
        {
            public void Do(SceneController _sc, string line, Scene s)
            {
                line = line.Replace("Speaker=", "");
                _sc.SetSpeaker(line);
            }
        }
        public class Chara : ITag
        {
            public void Do(SceneController _sc, string line, Scene s)
            {
                if (line.Contains("("))
                {
                    line = line.Replace("Chara=", "").Replace(")", "");
                    var splitted = line.Split('(');
                    var splitted2 = splitted[1].Split(':');
                    _sc.SetCharaImage("Characters", splitted[0], float.Parse(splitted2[0]), float.Parse(splitted2[1]), float.Parse(splitted2[2]));
                }
                else
                {
                    line = line.Replace("Chara=", "");
                    _sc.SetCharaImage("Characters", line, 0f, 0f, 1f);
                }
            }
        }
        public class Ico : ITag
        {
            public void Do(SceneController _sc, string line, Scene s)
            {
                line = line.Replace("Ico=", "");
                _sc.SetIcoImage("CharaIcons", line);
            }
        }
        public class Background : ITag
        {
            public void Do(SceneController _sc, string line, Scene s)
            {
                line = line.Replace("Background=", "");
                _sc.SetBgImage("BackGrounds", line);
            }
        }
        public class Score : ITag
        {
            public void Do(SceneController _sc, string line, Scene s)
            {
                line = line.Replace("Score=", "");
                _sc.AddScore(line);
            }
        }
        public class Action : ITag
        {
            public void Do(SceneController _sc, string line, Scene s)
            {
                line = line.Replace("Action=", "");
                _sc.Action(line);
            }
        }
        public class SE : ITag
        {
            public void Do(SceneController _sc, string line, Scene s)
            {
                if (line.Contains("("))
                {
                    line = line.Replace("SE=", "").Replace(")", "");
                    var splitted = line.Split('(');
                    _sc.ChangeSE(splitted[0], splitted[1]);
                }
                else
                {
                    line = line.Replace("SE=", "");
                    _sc.ChangeSE(line, "0");
                }
            }
        }
        public class BGM : ITag
        {
            public void Do(SceneController _sc, string line, Scene s)
            {
                line = line.Replace("BGM=", "");
                _sc.ChangeBGM(line);
            }
        }
        public class Add : ITag
        {
            public void Do(SceneController _sc, string line, Scene s)
            {
                line = line.Replace("Add=", "");
                var splitted = line.Split(':');
                _sc.SetScore(splitted[0], splitted[1]);
            }
        }
        public class Next : ITag
        {
            public void Do(SceneController _sc, string line, Scene s)
            {
                line = line.Replace("Next=", "");
                _sc.SetScene(line);
            }
        }
        public class Options : ITag
        {
            public void Do(SceneController _sc, string line, Scene s)
            {
                var options = new List<(string, string)>();
                while (true)
                {
                    s.GoNextLine();
                    line = line = s.Lines[s.Index];
                    if (line.Contains("["))
                    {
                        line = line.Replace("[", "").Replace("]", "");
                        var splitted = line.Split(':');
                        options.Add((splitted[0], splitted[1]));
                    }
                    else
                    {
                        _sc.SetOptions(options);
                        break;
                    }
                }
            }
        }
        public Type TagJudge(string className)
        {
            Type type = null;
            switch (className)
            {
                case "Speaker":
                    type = typeof(Speaker);
                    break;
                case "Chara":
                    type = typeof(Chara);
                    break;
                case "Ico":
                    type = typeof(Ico);
                    break;
                case "Background":
                    type = typeof(Background);
                    break;
                case "Score":
                    type = typeof(Score);
                    break;
                case "Action":
                    type = typeof(Action);
                    break;
                case "SE":
                    type = typeof(SE);
                    break;
                case "BGM":
                    type = typeof(BGM);
                    break;
                case "Add":
                    type = typeof(Add);
                    break;
                case "Next":
                    type = typeof(Next);
                    break;
                case "Options":
                    type = typeof(Options);
                    break;
            }
            return type;
        }
    }
}