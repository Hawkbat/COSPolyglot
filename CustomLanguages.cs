using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COSML;
using COSML.Modding;
using UnityEngine;

namespace CustomLanguages
{
    public class CustomLanguages : Mod
    {
        public static CustomLanguages instance;

        readonly List<CustomLanguage> customLanguages = new List<CustomLanguage>();

        public override string GetVersion() => "1.0.0";

        public CustomLanguages() : base("Custom Languages") { }

        public override void Init()
        {
            Info($"Loaded {nameof(CustomLanguages)}");

            instance = this;

            Patches.Apply();

            var languageTypeIndex = (int)LanguageType.RECLUS + 1;

            var modsDir = Path.GetFullPath(Path.Combine(Application.dataPath, "Managed", "Mods"));
            foreach (var modDir in Directory.EnumerateDirectories(modsDir))
            {
                var languagesDir = Path.GetFullPath(Path.Combine(modDir, "languages"));
                if (!Directory.Exists(languagesDir)) continue;
                foreach (var languageDir in Directory.EnumerateDirectories(languagesDir))
                {
                    var languageName = Path.GetFileName(languageDir);
                    var language = new CustomLanguage();
                    language.Init(languageName, (LanguageType)languageTypeIndex, languageDir);
                    Info($"Loaded custom language {languageName} with {language.runes.Count} runes");
                    customLanguages.Add(language);
                    languageTypeIndex++;
                }
            }

            ModHooks.PlaceChanged += ModHooks_PlaceChanged;
        }

        private void ModHooks_PlaceChanged(Place from, Place to)
        {
            RevealLanguages();
        }

        public static CustomLanguages GetInstance()
            => instance;

        public static IEnumerable<CustomLanguage> GetCustomLanguages()
            => instance.customLanguages;

        public static void RevealLanguages()
        {
            var game = GameController.GetInstance();
            foreach (var lang in GetCustomLanguages())
            {
                var runeIDs = lang.runes.Select(r => r.GetRuneData().GetId()).ToArray();
                game.GetLanguageController().GetLanguageData(lang.languageType).AddRunes(runeIDs);
                foreach (var rune in lang.runes)
                {
                    if (UnityEngine.Random.value > 0.5f) continue;
                    rune.GetRuneData().SetState(RuneStateData.VALIDATED);
                }
            }
        }
    }
}
