using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomLanguages
{
    public static class TextureUtils
    {
        public static Texture2D LoadTextureAtPath(string path)
        {
            var bytes = File.ReadAllBytes(path);
            var tex = new Texture2D(2, 2);
            ImageConversion.LoadImage(tex, bytes);
            tex.name = Path.GetFileNameWithoutExtension(path);
            return tex;
        }

        public static Sprite LoadSpriteAtPath(string path, float pixelsPerUnit)
        {
            var tex = LoadTextureAtPath(path);
            var rect = new Rect(0f, 0f, tex.width, tex.height);
            var pivot = rect.center;
            var sprite = Sprite.Create(tex, rect, pivot, pixelsPerUnit);
            sprite.name = tex.name;
            return sprite;
        }

        public static Texture2D CreateSolidTexture(int width, int height, Color color)
        {
            var tex = new Texture2D(width, height);
            Color[] colors = new Color[width * height];
            for (int i = 0; i < width * height; i++) colors[i] = color;
            tex.SetPixels(colors);
            tex.Apply();
            return tex;
        }

        public static Texture2D CombineTextures(Texture2D bottom, Texture2D top, Rect dstRect)
        {
            var mat = new Material(Shader.Find("Sprites/Default"));
            mat.color = Color.white;
            mat.mainTexture = top;

            var scaledRect = new Rect(dstRect.x / bottom.width, dstRect.y / bottom.height, dstRect.width / bottom.width, dstRect.height / bottom.height);
            scaledRect.x = scaledRect.x * 2f - 1f;
            scaledRect.y = scaledRect.y * 2f - 1f;
            scaledRect.width *= 2f;
            scaledRect.height *= 2f;

            var rt = new RenderTexture(bottom.width, bottom.height, 0);
            var previousRenderTexture = RenderTexture.active;
            RenderTexture.active = rt;
            Graphics.Blit(bottom, rt);
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, rt.width, rt.height, 0);
            Graphics.DrawTexture(dstRect, top);
            GL.PopMatrix();
            /*GL.PushMatrix();
            GL.LoadOrtho();
            mat.SetPass(0);
            GL.Begin(GL.QUADS);
            GL.TexCoord2(0f, 0f);
            GL.Vertex3(scaledRect.xMin, scaledRect.yMin, 0f);
            GL.TexCoord2(0f, 1f);
            GL.Vertex3(scaledRect.xMin, scaledRect.yMax, 0f);
            GL.TexCoord2(1f, 1f);
            GL.Vertex3(scaledRect.xMax, scaledRect.yMax, 0f);
            GL.TexCoord2(1f, 0f);
            GL.Vertex3(scaledRect.xMax, scaledRect.yMin, 0f);
            GL.End();
            GL.PopMatrix();*/

            var result = new Texture2D(rt.width, rt.height);
            result.ReadPixels(new Rect(0f, 0f, rt.width, rt.height), 0, 0);
            result.Apply();

            RenderTexture.active = previousRenderTexture;

            UnityEngine.Object.Destroy(mat);
            UnityEngine.Object.Destroy(rt);

            return result;
        }
    }
}
