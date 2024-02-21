using COSML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CustomLanguages
{
    [Serializable]
    public class CustomLanguage
    {
        public string name;
        public LanguageType languageType;
        public List<CustomRune> runes;
        public int journalRuneRows = 6;

        Sprite journalSprite;
        Sprite journalCompletedSprite;
        Sprite completedPopupSprite;
        I18nEntry completedPopupI18nEntry;
        LanguageDefinition languageDefinition;

        LanguageData languageData;
        JournalLanguageUI journalLanguageUI;
        TerminalLanguageUI terminalLanguageUI;

        public void Init(string name, LanguageType languageType, string languageDir)
        {
            this.name = name;
            this.languageType = languageType;

            var journalTexPath = Path.GetFullPath(Path.Combine(languageDir, "icons", "Journal.png"));
            journalSprite = TextureUtils.LoadSpriteAtPath(journalTexPath, 50f);
            var journalCompletedTexPath = Path.GetFullPath(Path.Combine(languageDir, "icons", "Journal_Completed.png"));
            journalCompletedSprite = TextureUtils.LoadSpriteAtPath(journalCompletedTexPath, 50f);
            var completedPopupTexPath = Path.GetFullPath(Path.Combine(languageDir, "icons", "Completed_Popup.png"));
            completedPopupSprite = TextureUtils.LoadSpriteAtPath(completedPopupTexPath, 50f);

            completedPopupI18nEntry = new I18nEntry
            {
                key = $"menu.popup.complete.custom_{name.ToLower()}",
                chineseSimplifiedTranslation = "语言破译完毕！",
                chineseTraditionalTranslation = "您已完成語言翻譯！",
                czeshTranslation = "Úspěšně jste dokončili překlad jazyka!",
                englishTranslation = "You've completed the translation of the language!",
                frenchTranslation = "Vous avez entièrement traduit la langue !",
                germanTranslation = "Sie haben die Sprache vollständig übersetzt!",
                italianTranslation = "Hai completato la traduzione della lingua!",
                japaneseTranslation = "言葉をすべて解読した！",
                koreanTranslation = "우리는 언어 전체를 해독했습니다!",
                polishTranslation = "Odszyfrowałeś język!",
                portugueseTranslation = "Você concluiu a tradução do idioma!",
                russianTranslation = "Вы расшифровали все символы языка!",
                spanishEuropeanTranslation = "¡Has completado la traducción del idioma!",
                spanishLatinAmericanTranslation = "¡Completaste la traducción del idioma!",
            };

            runes = new List<CustomRune>();

            var runesDir = Path.GetFullPath(Path.Combine(languageDir, "runes"));
            foreach (var runePath in Directory.EnumerateFiles(runesDir))
            {
                var ext = Path.GetExtension(runePath);
                if (ext.ToLower() != ".png") continue;
                var runeName = Path.GetFileNameWithoutExtension(runePath);
                var rune = new CustomRune();
                rune.Init(runeName, this, runePath);
                runes.Add(rune);
            }

            languageDefinition = ScriptableObject.CreateInstance<LanguageDefinition>();
            languageDefinition.i18nPrefix = $"custom_{name.ToLower()}";
            languageDefinition.nbRunes = runes.Count;
            languageDefinition.runes = runes.Select(r => r.GetRuneDefinition()).ToArray();
        }

        public void BuildLanguageData()
        {
            languageData = new LanguageData(languageDefinition, languageType);
            foreach (var rune in runes) rune.BuildRuneData();
        }

        public void BuildJournalLanguageUI()
        {
            var journal = GameController.GetInstance().GetJournal();
            var devotUI = journal.GetJournalLanguageUI(LanguageType.DEVOTS);
            var langUI = devotUI.Clone($"Touches_Custom_{name}");
            langUI.language = languageType;
            langUI.languageTrigger = $"Custom_{name}";
            langUI.ongletActiveSprite = journalSprite;
            langUI.ongletValidatedSprite = journalCompletedSprite;
            langUI.nbLine = journalRuneRows; // Number of rows
            var calculatedColumns = Mathf.CeilToInt((float)runes.Count / journalRuneRows);
            langUI.nbRaw = calculatedColumns; // Number of columns
            langUI.firstRawUp = true; // Whether the first column should start higher than the second column
            langUI.addOnLine2 = false; // Adds one extra rune every other column; only used by Reclus

            foreach (var key in langUI.keys)
            {
                UnityEngine.Object.Destroy(key.gameObject);
            }

            var keys = new List<JournalKeyUI>();
            var keyIndex = 0;
            foreach (var rune in runes)
            {
                var keyUI = devotUI.keys[0].Clone($"Touche {keyIndex + 1}");
                keyUI.transform.SetParent(langUI.transform, false);
                var row = keyIndex % langUI.nbLine;
                var col = keyIndex / langUI.nbLine;
                var x = (-2 + col) * 140f;
                var y = 40f + (2 - row) * 160f;
                if (col % 2 == 1) y += (langUI.firstRawUp ? -0.5f : 0.5f) * 160f;
                keyUI.transform.localPosition = new Vector3(x, y, 0f);
                keys.Add(keyUI);
                keyIndex++;
            }
            langUI.keys = keys.ToArray();

            // These values were selected somewhat arbitrarily based on the vanilla ones since there's no clear formula
            // First 5 values in each array are the values for the vanilla languages
            /*
            var newGaucheX = new int[] { -698, -611, -524, -439, -355, -272, -190, -109, -29, 50 }[(int)languageType];
            var newGaucheY = new int[] { 450, 450, 449, 448, 447, 446, 445, 443, 442, 441 }[(int)languageType];
            var newDroiteX = new int[] { -206, -125, -45, 35, 115, 195, 275, 355, 435, 515 }[(int)languageType];
            var newDroiteY = new int[] { 453, 460, 465, 472, 478, 484, 490, 496, 502, 508 }[(int)languageType];
            var boxRightX = new int[] { -370, -200, -40, 120, 280, 440, 600, 760, 920, 1080 }[(int)languageType];

            var ongletLeft3DX = 2f - (int)languageType;
            var ongletLeft3DY = 0f;
            var ongletRight3DX = 2f - (int)languageType;
            var ongletRight3DY = 0f;
            var ongletActive3DX = 2f - (int)languageType;
            var ongletActive3DY = 0f;

            var ongletLeftUIX = -600f + 300f * (int)languageType;
            var ongletLeftUIY = 0f;
            var ongletRightUIX = -600f + 300f * (int)languageType;
            var ongletRightUIY = 0f;
            var ongletActiveUIX = -600f + 300f * (int)languageType;
            var ongletActiveUIY = -40f;

            var ongletLeftBoxX = -1370f + 180f * (int)languageType;
            var ongletLeftBoxY = 760f;
            var ongletRightBoxX = boxRightX;
            var ongletRightBoxY = 790f + 10f * (int)languageType;
            */

            // Scrapped the above approach, now laying out the journal tabs vertically on the outsides

            var index = (int)languageType - 5;
            var ongletLeft3DPos = new Vector3(2.75f, -1f - index, -0.015f);
            var ongletLeft3DRot = new Vector3(0f, 0f, 270f);
            var ongletActive3DPos = new Vector3(2.6f, -1f - index, -0.015f);
            var ongletActive3DRot = new Vector3(0f, 0f, 270f);
            var ongletRight3DPos = new Vector3(-2.75f, -1f - index, -0.015f);
            var ongletRight3DRot = new Vector3(0f, 0f, 90f);
            var ongletLeftUIPos = new Vector3(-950f, -425f - 300f * index, 0f);
            var ongletActiveUIPos = new Vector3(-910f, -425f - 300f * index, 0f);
            var ongletRightUIPos = new Vector3(950f, -425f - 300f * index, 0f);

            // These values are definitely wrong since they're in flat space instead of journal surface space
            // Likely the only good way of setting these is empirically
            var ongletLeftNewPos = new Vector3(-800f, 350f - 75f * index, 0f);
            var ongletRightNewPos = new Vector3(235f, 395f - 75f * index, 0f);
            var ongletLeftBoxPos = new Vector3();
            var ongletRightBoxPos = new Vector3();

            var suffix = $"Custom_{name}";

            langUI.ongletLeft.onglet3D = langUI.ongletLeft.onglet3D.Clone($"onglet_gauche_{suffix}");
            langUI.ongletLeft.onglet3D.localPosition = ongletLeft3DPos;
            langUI.ongletLeft.onglet3D.localEulerAngles = ongletLeft3DRot;
            langUI.ongletLeft.ongletUI = langUI.ongletLeft.ongletUI.Clone($"Onglet_gauche_{suffix}");
            langUI.ongletLeft.ongletUI.localPosition = ongletLeftUIPos;
            langUI.ongletLeft.ongletUI.GetComponent<JournalOngletImageUI>().language = languageType;
            langUI.ongletLeft.ongletAnimator = langUI.ongletLeft.ongletUI.GetComponent<Animator>();
            langUI.ongletLeft.ongletImg = langUI.ongletLeft.ongletUI.Find("Picto").GetComponent<Image>();
            langUI.ongletLeft.ongletMaskImg = langUI.ongletLeft.ongletUI.Find("Swap").GetComponent<Image>();
            langUI.ongletLeft.ongletNew = langUI.ongletLeft.ongletNew.Clone($"New_gauche_{suffix}");
            langUI.ongletLeft.ongletNew.transform.localPosition = ongletLeftNewPos;
            langUI.ongletLeft.ongletBox = langUI.ongletLeft.ongletBox.Clone($"OngletBoxLeft{suffix}");
            langUI.ongletLeft.ongletBox.transform.localPosition = ongletLeftBoxPos;
            langUI.ongletLeft.ongletBox.language = languageType;

            langUI.ongletRight.onglet3D = langUI.ongletRight.onglet3D.Clone($"onglet_droite_{suffix}");
            langUI.ongletRight.onglet3D.localPosition = ongletRight3DPos;
            langUI.ongletRight.onglet3D.localEulerAngles = ongletRight3DRot;
            langUI.ongletRight.ongletUI = langUI.ongletRight.ongletUI.Clone($"Onglet_droite_{suffix}");
            langUI.ongletRight.ongletUI.localPosition = ongletRightUIPos;
            langUI.ongletRight.ongletUI.GetComponent<JournalOngletImageUI>().language = languageType;
            langUI.ongletRight.ongletAnimator = langUI.ongletRight.ongletUI.GetComponent<Animator>();
            langUI.ongletRight.ongletImg = langUI.ongletRight.ongletUI.Find("Picto").GetComponent<Image>();
            langUI.ongletRight.ongletMaskImg = langUI.ongletRight.ongletUI.Find("Swap").GetComponent<Image>();
            langUI.ongletRight.ongletNew = langUI.ongletRight.ongletNew.Clone($"New_droite_{suffix}");
            langUI.ongletRight.ongletNew.transform.localPosition = ongletRightNewPos;
            langUI.ongletRight.ongletBox = langUI.ongletRight.ongletBox.Clone($"OngletBoxLeft{suffix}");
            langUI.ongletRight.ongletBox.transform.localPosition = ongletRightBoxPos;
            langUI.ongletRight.ongletBox.language = languageType;

            langUI.ongletActive.onglet3D = langUI.ongletActive.onglet3D.Clone($"onglet_actif_{suffix}");
            langUI.ongletActive.onglet3D.localPosition = ongletActive3DPos;
            langUI.ongletActive.onglet3D.localEulerAngles = ongletActive3DRot;
            langUI.ongletActive.ongletUI = langUI.ongletActive.ongletUI.Clone($"Onglet_actif_{suffix}");
            langUI.ongletActive.ongletUI.localPosition = ongletActiveUIPos;
            langUI.ongletActive.ongletAnimator = langUI.ongletActive.ongletUI.GetComponent<Animator>();
            langUI.ongletActive.ongletImg = langUI.ongletActive.ongletUI.Find("Picto").GetComponent<Image>();
            langUI.ongletActive.ongletMaskImg = langUI.ongletActive.ongletUI.Find("Swap").GetComponent<Image>();

            //TODO: create journal page UI objects
            langUI.pages = new JournalLanguageUI.JournalPage[] { };
            langUI.availablePages = new JournalLanguageUI.JournalPage[] { };

            journalLanguageUI = langUI;
        }

        public void UpdateJournalLanguageUI()
        {
            var leftScreenPos = Camera.main.WorldToScreenPoint(journalLanguageUI.ongletLeft.ongletUI.position);
            journalLanguageUI.ongletLeft.ongletBox.transform.position = leftScreenPos;
            journalLanguageUI.ongletLeft.ongletNew.transform.position = leftScreenPos;
            journalLanguageUI.ongletLeft.ongletNew.transform.localPosition += Vector3.up * 70f;
            var rightScreenPos = Camera.main.WorldToScreenPoint(journalLanguageUI.ongletRight.ongletUI.position);
            journalLanguageUI.ongletRight.ongletBox.transform.position = rightScreenPos;
            journalLanguageUI.ongletRight.ongletNew.transform.position = rightScreenPos;
            journalLanguageUI.ongletRight.ongletNew.transform.localPosition += Vector3.up * 70f;
            //journalLanguageUI.ongletLeft.Select(true, true);
            //journalLanguageUI.ongletLeft.ongletBox.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
            //journalLanguageUI.ongletRight.Select(true, true);
            //journalLanguageUI.ongletRight.ongletBox.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
            //journalLanguageUI.ongletActive.Select(true, true);
        }

        public void BuildTerminalLanguageUI()
        {
            var journal = GameController.GetInstance().GetJournal();
            var ui = GameController.GetInstance().GetUIController().terminalKeyboardsUI;
            terminalLanguageUI = ReflectionHelper.CallMethod<TerminalKeyboardsUI, TerminalLanguageUI>(ui, "CreateTerminalLanguageUI", journal, languageType);
        }

        public void SaveJournalSaveData()
        {
            var data = languageData.GetSerializedData();
        }

        public string LoadJournalSaveData()
        {
            return string.Empty;
        }

        public LanguageData GetLanguageData()
            => languageData;

        public JournalLanguageUI GetJournalLanguageUI()
            => journalLanguageUI;

        public TerminalLanguageUI GetTerminalLanguageUI()
            => terminalLanguageUI;

        public I18nEntry GetCompletedPopupI18NEntry()
            => completedPopupI18nEntry;

        public Sprite GetCompletedPopupSprite()
            => completedPopupSprite;
    }
}
