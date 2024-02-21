using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomLanguages
{
    public static class Assets
    {
        static Texture2D runeBackTemplateTexture;
        
        public static Texture2D GetRuneBackTemplateTexture() {
            if (runeBackTemplateTexture == null)
            {
                runeBackTemplateTexture = TextureUtils.LoadTextureAtPath(Path.GetFullPath(Path.Combine(GetAssetsPath(), "Runes_Custom_Journal+Fond_Blank_Single.png")));
            }
            return runeBackTemplateTexture;
        }

        public static string GetAssetsPath()
            => Path.GetFullPath(Path.Combine(Application.dataPath, "Managed", "Mods", Polyglot.GetInstance().GetName(), "assets"));
    }
}
