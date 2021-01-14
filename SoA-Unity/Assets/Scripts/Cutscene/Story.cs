using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace story
{
    [System.Serializable]
    public class Story
    {
        public List<Scene> scenes;

        [System.Serializable]
        public class Scene
        {
            public List<Action> actions;

            [System.Serializable]
            public class Action
            {
                public string type;
                public string metadata;
                public string data;
            };
        };

        public Story(string text)
        {
            Deserialize(text);
        }
        public void Deserialize(string text)
        {
            JsonUtility.FromJsonOverwrite(text, this);
        }
    }
}
