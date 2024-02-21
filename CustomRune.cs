using System;
using UnityEngine;

namespace CustomLanguages
{
    [Serializable]
    public class CustomRune
    {
        public string name;
        public CustomLanguage language;

        Sprite chatBubbleSprite;
        Sprite journalSprite;
        Sprite journalBackSprite;
        RuneDefinition runeDefinition;

        RuneData runeData;

        public void Init(string name, CustomLanguage language, string runePath)
        {
            this.name = name;
            this.language = language;

            var tex = TextureUtils.LoadTextureAtPath(runePath);
            var chatBubbleRatio = (float)tex.width / tex.height;
            var chatBubbleScale = 60f / 76f;

            var pad = -10f;
            var chatPad = chatBubbleScale * pad;

            var chatBubbleBaseTex = TextureUtils.CreateSolidTexture(Mathf.CeilToInt(chatBubbleRatio * 100), 100, new Color(0f, 0f, 0f, 0f));
            var chatBubbleTex = TextureUtils.CombineTextures(chatBubbleBaseTex, tex, new Rect(20f + chatPad, 20f + chatPad, chatBubbleBaseTex.width - 40f - chatPad * 2f, chatBubbleBaseTex.height - 40f - chatPad * 2f));
            chatBubbleSprite = Sprite.Create(chatBubbleTex, new Rect(0f, 0f, chatBubbleRatio * 100f, 100f), new Vector2(chatBubbleRatio * 50f, 50f), chatBubbleScale * 100f);
            
            var journalBaseTex = TextureUtils.CreateSolidTexture(252, 180, new Color(0f, 0f, 0f, 0f));
            var journalTex = TextureUtils.CombineTextures(journalBaseTex, tex, new Rect(88f + pad, 52f + pad, 76f - pad * 2f, 76f - pad * 2f));
            journalSprite = Sprite.Create(journalTex, new Rect(0f, 0f, 252f, 180f), new Vector2(126f, 90f), 100f);
            
            var journalBackBaseTex = Assets.GetRuneBackTemplateTexture();
            var journalBackTex = TextureUtils.CombineTextures(journalBackBaseTex, tex, new Rect(88f + pad, 52f + pad, 76f - pad * 2f, 76f - pad * 2f));
            journalBackSprite = Sprite.Create(journalBackTex, new Rect(0f, 0f, 252f, 180f), new Vector2(126f, 90f), 100f);

            // AssignatedRune image = Journal+Fond texture (252x180 sprites in a 2048x2048 atlas; 90px vertical padding above each row)
            // LargeRune image = Journal texture (252x180 sprites in a 2048x2048 atlas; 90px vertical padding above each row)
            // SmallRune image = Bulles texture (140x100 sprites in a 1024x1024 atlas; 50px vertical padding above each row, variable width and x-offset per sprite)

            runeDefinition = new RuneDefinition
            {
                runeI18n = new I18nEntry
                {
                    key = $"custom_{language.name.ToLower()}.{name.ToLower()}",
                    englishTranslation = name,
                },
                smallRune = chatBubbleSprite,
                largeRune = journalSprite,
                assignatedRune = journalBackSprite,
                // Don't understand how this works yet, so leaving it unimplemented
                isLinkedRune = false,
                linkedRuneId = 0,
                linkedLanguageType = LanguageType.DEVOTS
            };
        }

        public void BuildRuneData()
        {
            runeData = language.GetLanguageData().GetRuneData(GetRuneID());
        }

        public int GetRuneID()
            => language.runes.IndexOf(this);

        public RuneDefinition GetRuneDefinition()
            => runeDefinition;

        public RuneData GetRuneData()
            => runeData;
    }
}
