using COSML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CustomLanguages
{
    public static class Patches
    {
        public static void Apply()
        {
            On.GameSession.SaveJournal += GameSession_SaveJournal;
            On.LanguageController.Init += LanguageController_Init;
            On.LanguageController.Load += LanguageController_Load;
            On.LanguageController.GetLanguageData += LanguageController_GetLanguageData;
            On.LanguageController.GetFirstKnownLanguage += LanguageController_GetFirstKnownLanguage;
            On.LanguageController.HasNewRunes += LanguageController_HasNewRunes;
            On.LanguageController.HasKnownLanguage += LanguageController_HasKnownLanguage;
            On.LanguageController.HasAvailablePage += LanguageController_HasAvailablePage;
            On.LanguageController.HasValidatedRunes += LanguageController_HasValidatedRunes;
            On.LanguageController.HasValidatedLinkedRunes += LanguageController_HasValidatedLinkedRunes;
            On.LanguageController.NeedSave += LanguageController_NeedSave;
            On.LanguageController.GetNbValidedRunes += LanguageController_GetNbValidedRunes;
            On.UIController.Init += UIController_Init;
            On.JournalLanguageUI.Resume += JournalLanguageUI_Resume;
            On.Journal.Init += Journal_Init;
            On.Journal.SelectOnglet += Journal_SelectOnglet;
            On.Journal.UpdateRunes += Journal_UpdateRunes;
            On.Journal.GetJournalLanguageUI += Journal_GetJournalLanguageUI;
            On.Journal.TryLaunchOngletValidation += Journal_TryLaunchOngletValidation;
            On.Journal.HasNextOnglet += Journal_HasNextOnglet;
            On.Journal.HasFoundLinkedPage += Journal_HasFoundLinkedPage;
            On.MouseController.CheckJournalOngletShortcuts += MouseController_CheckJournalOngletShortcuts;
            On.PadController.CheckJournalOngletShortcuts += PadController_CheckJournalOngletShortcuts;
            On.PlayerTatoo.InitTatoo += PlayerTatoo_InitTatoo;
            On.PlayerTatoo.Loop += PlayerTatoo_Loop;
            On.PlayerTatoo.TrySetNeedUpgradeTatoos += PlayerTatoo_TrySetNeedUpgradeTatoos;
            On.LanguageValidationUI.Init += LanguageValidationUI_Init;
            On.LanguageValidationUI.ShowLink += LanguageValidationUI_ShowLink;
            On.TerminalKeyboardsUI.Init += TerminalKeyboardsUI_Init;
            On.TerminalKeyboardsUI.OnOpen += TerminalKeyboardsUI_OnOpen;
            On.TerminalKeyboardsUI.Swap += TerminalKeyboardsUI_Swap;
            On.TerminalKeyboardsUI.HideEnd += TerminalKeyboardsUI_HideEnd;
            On.TerminalKeyboardsUI.Loop += TerminalKeyboardsUI_Loop;
            On.TerminalKeyboardsUI.GetCurrentTerminalLanguage += TerminalKeyboardsUI_GetCurrentTerminalLanguage;
        }

        private static void GameSession_SaveJournal(On.GameSession.orig_SaveJournal orig, GameSession self, string save, bool isDemo)
        {
            orig(self, save, isDemo);
            foreach (var lang in Polyglot.GetCustomLanguages()) lang.SaveJournalSaveData();
        }

        private static void LanguageController_Init(On.LanguageController.orig_Init orig, LanguageController self)
        {
            orig(self);
            var allLanguages = ReflectionHelper.GetField<LanguageController, LanguageData[]>(self, "allLanguages").ToList();
            foreach (var lang in Polyglot.GetCustomLanguages())
            {
                lang.BuildLanguageData();
                allLanguages.Add(lang.GetLanguageData());
            }
            ReflectionHelper.SetField(self, "allLanguages", allLanguages.ToArray());
        }

        private static void LanguageController_Load(On.LanguageController.orig_Load orig, LanguageController self, Journal journal)
        {
            orig(self, journal);
            foreach (var lang in Polyglot.GetCustomLanguages())
            {
                lang.GetLanguageData().Load(lang.LoadJournalSaveData(), self.defaultRuneState, journal);
                lang.GetJournalLanguageUI().SelectPage(-1);
            }
        }

        private static LanguageData LanguageController_GetLanguageData(On.LanguageController.orig_GetLanguageData orig, LanguageController self, LanguageType type)
        {
            var lang = Polyglot.GetCustomLanguages().FirstOrDefault(l => l.languageType == type);
            if (lang != null) return lang.GetLanguageData();
            return orig(self, type);
        }

        private static LanguageType LanguageController_GetFirstKnownLanguage(On.LanguageController.orig_GetFirstKnownLanguage orig, LanguageController self)
        {
            var lang = Polyglot.GetCustomLanguages().FirstOrDefault(l => l.GetLanguageData().GetState() != LanguageData.LanguageDataState.UNKNOW);
            if (lang != null) return lang.languageType;
            return orig(self);
        }

        private static bool LanguageController_HasNewRunes(On.LanguageController.orig_HasNewRunes orig, LanguageController self)
        {
            return orig(self) || (Polyglot.GetCustomLanguages().Any() && Polyglot.GetCustomLanguages().Any(l => l.GetLanguageData().GetState() == LanguageData.LanguageDataState.NEW));
        }

        private static bool LanguageController_HasKnownLanguage(On.LanguageController.orig_HasKnownLanguage orig, LanguageController self)
        {
            return orig(self) || (Polyglot.GetCustomLanguages().Any() && Polyglot.GetCustomLanguages().Any(l => l.GetLanguageData().GetState() != LanguageData.LanguageDataState.UNKNOW));
        }

        private static bool LanguageController_HasAvailablePage(On.LanguageController.orig_HasAvailablePage orig, LanguageController self)
        {
            return orig(self) || (Polyglot.GetCustomLanguages().Any() && Polyglot.GetCustomLanguages().Any(l => l.GetLanguageData().HasAvailablePage()));
        }

        private static bool LanguageController_HasValidatedRunes(On.LanguageController.orig_HasValidatedRunes orig, LanguageController self)
        {
            return orig(self) || (Polyglot.GetCustomLanguages().Any() && Polyglot.GetCustomLanguages().Any(l => l.GetLanguageData().HasValidatedRunes()));
        }

        private static bool LanguageController_HasValidatedLinkedRunes(On.LanguageController.orig_HasValidatedLinkedRunes orig, LanguageController self)
        {
            return orig(self) || (Polyglot.GetCustomLanguages().Any() && Polyglot.GetCustomLanguages().Any(l => l.GetLanguageData().HasValidatedLinkedRunes()));
        }

        private static bool LanguageController_NeedSave(On.LanguageController.orig_NeedSave orig, LanguageController self)
        {
            return orig(self) || (Polyglot.GetCustomLanguages().Any() && Polyglot.GetCustomLanguages().Any(l => l.GetLanguageData().NeedSave()));
        }

        private static int LanguageController_GetNbValidedRunes(On.LanguageController.orig_GetNbValidedRunes orig, LanguageController self)
        {
            var count = orig(self);
            foreach (var lang in Polyglot.GetCustomLanguages())
            {
                for (int j = 0; j < lang.GetLanguageData().GetNbRunes(); j++)
                {
                    if (lang.GetLanguageData().GetRuneData(j).GetState() == RuneStateData.VALIDATED)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private static void UIController_Init(On.UIController.orig_Init orig, UIController self, Journal journal, TerminalUI newTerminalUI, VisiocodeUI newVisiocodeUI, bool startOnMainMenu)
        {
            foreach (var lang in Polyglot.GetCustomLanguages())
            {
                lang.BuildJournalLanguageUI();
            }
            orig(self, journal, newTerminalUI, newVisiocodeUI, startOnMainMenu);
        }

        private static void JournalLanguageUI_Resume(On.JournalLanguageUI.orig_Resume orig, JournalLanguageUI self, LanguageType targetLanguage, bool newHasSeveralLanguages, bool hasAvailablePage)
        {
            orig(self, targetLanguage, newHasSeveralLanguages, hasAvailablePage);
            // Bit of a hack; this method is called deep in Journal.Loop(), so it's easier to piggyback off a known call instead of patching Loop()
            if (self.language == LanguageType.RECLUS)
            {
                foreach (var lang in Polyglot.GetCustomLanguages()) lang.GetJournalLanguageUI().Resume(targetLanguage, newHasSeveralLanguages, hasAvailablePage);
            }
        }

        private static void Journal_Init(On.Journal.orig_Init orig, Journal self)
        {
            orig(self);
            foreach (var lang in Polyglot.GetCustomLanguages()) lang.GetJournalLanguageUI().Init(self);
        }

        private static bool Journal_SelectOnglet(On.Journal.orig_SelectOnglet orig, Journal self, LanguageType newLanguage)
        {
            var result = orig(self, newLanguage);
            if (result)
            {
                foreach (var lang in Polyglot.GetCustomLanguages()) lang.GetJournalLanguageUI().SelectOnglet(newLanguage);
            }
            return result;
        }

        private static void Journal_UpdateRunes(On.Journal.orig_UpdateRunes orig, Journal self)
        {
            orig(self);
            foreach (var lang in Polyglot.GetCustomLanguages()) lang.GetJournalLanguageUI().UpdateRunes(self.GetLanguage());
        }

        private static JournalLanguageUI Journal_GetJournalLanguageUI(On.Journal.orig_GetJournalLanguageUI orig, Journal self, LanguageType searchLanguage)
        {
            var lang = Polyglot.GetCustomLanguages().FirstOrDefault(l => l.languageType == searchLanguage);
            if (lang != null) return lang.GetJournalLanguageUI();
            return orig(self, searchLanguage);
        }

        private static bool Journal_TryLaunchOngletValidation(On.Journal.orig_TryLaunchOngletValidation orig, Journal self)
        {
            foreach (var lang in Polyglot.GetCustomLanguages())
            {
                if (self.GetJournalLanguageUI(lang.languageType).ValidOnglet())
                {
                    ReflectionHelper.SetField(self, "waitAnimation", true);
                    return true;
                }
            }
            return orig(self);
        }

        private static bool Journal_HasNextOnglet(On.Journal.orig_HasNextOnglet orig, Journal self)
        {
            var languageController = GameController.GetInstance().GetLanguageController();
            for (int i = (int)(self.GetLanguage() + 1); i < 5 + Polyglot.GetCustomLanguages().Count(); i++)
            {
                if (languageController.GetLanguageData((LanguageType)i).GetState() != LanguageData.LanguageDataState.UNKNOW)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool Journal_HasFoundLinkedPage(On.Journal.orig_HasFoundLinkedPage orig, Journal self)
        {
            return orig(self) || (Polyglot.GetCustomLanguages().Any() && Polyglot.GetCustomLanguages().Any(l => l.GetJournalLanguageUI().HasFoundLinkedPage()));
        }

        private static void MouseController_CheckJournalOngletShortcuts(On.MouseController.orig_CheckJournalOngletShortcuts orig, MouseController self, Journal journal)
        {
            orig(self, journal);
            for (int i = 0; i < 4; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha6 + i) || Input.GetKeyDown(KeyCode.Keypad6 + i))
                {
                    var lang = Polyglot.GetCustomLanguages().FirstOrDefault(l => l.languageType == (LanguageType)(5 + i));
                    if (lang != null)
                    {
                        journal.SelectOnglet(lang.languageType);
                        self.OnShortcuts();
                        break;
                    }
                }
            }
        }

        private static void PadController_CheckJournalOngletShortcuts(On.PadController.orig_CheckJournalOngletShortcuts orig, PadController self, Journal journal)
        {
            var player = ReflectionHelper.GetField<PadController, Rewired.Player>(self, "player");
            if (player.GetButtonDown("Next2"))
            {
                int nextIndex = (int)(journal.GetLanguage() + 1);
                while (nextIndex < 5 + Polyglot.GetCustomLanguages().Count() && !journal.SelectOnglet((LanguageType)nextIndex))
                {
                    nextIndex++;
                }
                self.OnShortcuts();
            }
            else
            {
                orig(self, journal);
            }
        }

        private static void PlayerTatoo_InitTatoo(On.PlayerTatoo.orig_InitTatoo orig, PlayerTatoo self)
        {
            orig(self);
            var hasTatoos = ReflectionHelper.GetField<PlayerTatoo, bool[]>(self, "hasTatoos").ToList();
            var tatooParticles = ReflectionHelper.GetField<PlayerTatoo, ParticleSystem[]>(self, "tatooParticles").ToList();
            foreach (var lang in Polyglot.GetCustomLanguages())
            {
                hasTatoos.Add(lang.GetLanguageData().IsTranslated());
                tatooParticles.Add(tatooParticles[0]);
            }
            ReflectionHelper.SetField(self, "hasTatoos", hasTatoos.ToArray());
            ReflectionHelper.SetField(self, "tatooParticles", tatooParticles.ToArray());
        }

        private static void PlayerTatoo_Loop(On.PlayerTatoo.orig_Loop orig, PlayerTatoo self)
        {
            var hasTatoos = ReflectionHelper.GetField<PlayerTatoo, bool[]>(self, "hasTatoos");
            var playerController = GameController.GetInstance().GetPlayerController();
            if (ReflectionHelper.GetField<PlayerTatoo, bool>(self, "needUpgradeTatoos"))
            {
                var languageController = GameController.GetInstance().GetLanguageController();
                var humanoid = playerController.GetHumanoid();
                var currentSpot = humanoid.GetCurrentSpot();
                if (currentSpot.IsFinish() || currentSpot.GetType().Equals(typeof(PadSpot)))
                {
                    humanoid.ChangeSpot(new TatooSpot(), false);
                    for (int i = 0; i < 5 + Polyglot.GetCustomLanguages().Count(); i++)
                    {
                        if (!hasTatoos[i] && languageController.GetLanguageData((LanguageType)i).IsTranslated())
                        {
                            hasTatoos[i] = true;
                            ReflectionHelper.SetField(self, "tatooToFlash", i);
                            break;
                        }
                    }
                    ReflectionHelper.SetField(self, "needUpgradeTatoos", false);
                    for (int j = 0; j < 5 + Polyglot.GetCustomLanguages().Count(); j++)
                    {
                        if (!hasTatoos[j] && languageController.GetLanguageData((LanguageType)j).IsTranslated())
                        {
                            ReflectionHelper.SetField(self, "needUpgradeTatoos", true);
                            return;
                        }
                    }
                    return;
                }
                if (currentSpot.CanBeEscaped())
                {
                    currentSpot.Quit(humanoid);
                    if (currentSpot.GetType().Equals(typeof(InteractionSpot)))
                    {
                        var interactionSpot = (InteractionSpot)currentSpot;
                        if (interactionSpot.GetInteractive().GetType().Equals(typeof(PNJGhost)))
                        {
                            ((PNJGhost)interactionSpot.GetInteractive()).ForceQuit();
                        }
                    }
                }
            }
        }

        private static void PlayerTatoo_TrySetNeedUpgradeTatoos(On.PlayerTatoo.orig_TrySetNeedUpgradeTatoos orig, PlayerTatoo self)
        {
            var hasTatoos = ReflectionHelper.GetField<PlayerTatoo, bool[]>(self, "hasTatoos");
            if (!ReflectionHelper.GetField<PlayerTatoo, bool>(self, "needUpgradeTatoos"))
            {
                GameController instance = GameController.GetInstance();
                LanguageController languageController = instance.GetLanguageController();
                Journal journal = instance.GetJournal();
                bool flag = false;
                for (int i = 0; i < 5 + Polyglot.GetCustomLanguages().Count(); i++)
                {
                    if (!hasTatoos[i] && languageController.GetLanguageData((LanguageType)i).IsTranslated())
                    {
                        flag = true;
                        journal.GetJournalLanguageUI((LanguageType)i).FlagOngletToValid();
                    }
                }
                if (flag)
                {
                    ReflectionHelper.SetField(self, "needUpgradeTatoos", true);
                }
                journal.TryLaunchOngletValidation();
            }
        }

        private static void LanguageValidationUI_Init(On.LanguageValidationUI.orig_Init orig, LanguageValidationUI self)
        {
            orig(self);
            AbstractPlateform plateformController = GameController.GetInstance().GetPlateformController();
            I18nType i18n = plateformController.GetOptions().i18n;
            var i18nTexts = self.i18nTexts.ToList();
            var langText = i18nTexts[0];
            var linkText = i18nTexts[5];
            i18nTexts.Remove(linkText);
            var pictos = self.pictos.ToList();
            foreach (var lang in Polyglot.GetCustomLanguages())
            {
                var newLangText = langText.Clone($"Text_Custom_{lang.name}");
                newLangText.i18n = lang.GetCompletedPopupI18NEntry();
                newLangText.Init(i18n, plateformController.GetI18nPlateformType());
                i18nTexts.Add(newLangText);
                pictos.Add(lang.GetCompletedPopupSprite());
            }
            i18nTexts.Add(linkText);
            self.i18nTexts = i18nTexts.ToArray();
            self.pictos = pictos.ToArray();
        }

        private static void LanguageValidationUI_ShowLink(On.LanguageValidationUI.orig_ShowLink orig, LanguageValidationUI self)
        {
            orig(self);
            // Base code assumes the "linking" text is always index 5, but we've extended the array
            self.i18nTexts[5].gameObject.SetActive(false);
            self.i18nTexts[self.i18nTexts.Length - 1].gameObject.SetActive(true);
        }

        private static void TerminalKeyboardsUI_Init(On.TerminalKeyboardsUI.orig_Init orig, TerminalKeyboardsUI self, Journal journal)
        {
            orig(self, journal);
            foreach (var lang in Polyglot.GetCustomLanguages())
            {
                lang.BuildTerminalLanguageUI();
            }
        }

        private static void TerminalKeyboardsUI_OnOpen(On.TerminalKeyboardsUI.orig_OnOpen orig, TerminalKeyboardsUI self)
        {
            orig(self);
            foreach (var lang in Polyglot.GetCustomLanguages()) lang.GetTerminalLanguageUI().OnOpen();
        }

        private static void TerminalKeyboardsUI_Swap(On.TerminalKeyboardsUI.orig_Swap orig, TerminalKeyboardsUI self)
        {
            orig(self);
            var currentLanguage = ReflectionHelper.GetField<TerminalKeyboardsUI, LanguageType>(self, "currentLanguage");
            var lang = Polyglot.GetCustomLanguages().FirstOrDefault(l => l.languageType == currentLanguage);
            if (lang != null)
            {
                ReflectionHelper.GetField<TerminalKeyboardsUI, TerminalLanguageUI>("devotLanguage").Hide();
                lang.GetTerminalLanguageUI().Show();
            }
        }

        private static void TerminalKeyboardsUI_HideEnd(On.TerminalKeyboardsUI.orig_HideEnd orig, TerminalKeyboardsUI self)
        {
            foreach (var lang in Polyglot.GetCustomLanguages()) lang.GetTerminalLanguageUI().Hide();
            orig(self);
        }

        private static void TerminalKeyboardsUI_Loop(On.TerminalKeyboardsUI.orig_Loop orig, TerminalKeyboardsUI self)
        {
            orig(self);
            foreach (var lang in Polyglot.GetCustomLanguages()) lang.GetTerminalLanguageUI().Loop();
        }

        private static TerminalLanguageUI TerminalKeyboardsUI_GetCurrentTerminalLanguage(On.TerminalKeyboardsUI.orig_GetCurrentTerminalLanguage orig, TerminalKeyboardsUI self)
        {
            var currentLanguage = ReflectionHelper.GetField<TerminalKeyboardsUI, LanguageType>(self, "currentLanguage");
            var lang = Polyglot.GetCustomLanguages().FirstOrDefault(l => l.languageType == currentLanguage);
            if (lang != null) return lang.GetTerminalLanguageUI();
            return orig(self);
        }
    }
}
