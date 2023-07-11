using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.IO;
using System;



    [CreateAssetMenu(fileName = "ViralSettings", menuName = "Viral/Settings", order = 1)]
    public class ViralSettings : ScriptableObject
     {
        public BoolReactiveProperty TeleporationActive = new BoolReactiveProperty(true);
        public BoolReactiveProperty MimotionActive = new BoolReactiveProperty(true);
        public BoolReactiveProperty MenuFollowPlayer = new BoolReactiveProperty(false);

        public BoolReactiveProperty EngineerModeActive = new BoolReactiveProperty(false);

    public void Save(string path)
        {

           string json = JsonUtility.ToJson(new SerializableSettings
           {
                TeleporationActive = TeleporationActive.Value,
                MimotionActive = MimotionActive.Value,
                MenuFollowPlayer = MenuFollowPlayer.Value,

               EngineerModeActive = EngineerModeActive.Value


           });
           File.WriteAllText(path, json);
        }
        public void Load(string path)
        {
            if (!File.Exists(path)) return;

           SerializableSettings settings = JsonUtility.FromJson<SerializableSettings>(File.ReadAllText(path));

            TeleporationActive.Value = settings.TeleporationActive;
            MimotionActive.Value = settings.MimotionActive;
            MenuFollowPlayer.Value = settings.MenuFollowPlayer;
        EngineerModeActive.Value = settings.EngineerModeActive;
        }

        [Serializable]
        private class SerializableSettings
        {
            public bool VignetteActive;
            public bool TeleporationActive;
            public bool MenuFollowPlayer;
            public bool MimotionActive;
            public bool EngineerModeActive;
        }

    }

