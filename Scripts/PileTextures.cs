using UnityEngine;

namespace SheetAnimationGenerator
{
    public class PileTextures
    {
        public static bool Pile(Texture2D baseTex, Texture2D overTex, out Texture2D resultTex)
        {
            resultTex = null;
            if (baseTex.width != overTex.width || baseTex.height != overTex.height)
            {
                Debug.LogError($"テクスチャのサイズが一致しません. {baseTex.width} x {baseTex.height} ↔ {overTex.width} x {overTex.height}");
                return false;
            }

            resultTex = new Texture2D(baseTex.width, baseTex.height, baseTex.format, false);
            for (int y = 0; y < baseTex.height; y++)
            {
                for (int x = 0; x < baseTex.width; x++)
                {
                    var baseCol = baseTex.GetPixel(x, y).gamma;
                    var overCol = overTex.GetPixel(x, y).gamma;
                    var c = NormalAlphaBlend(baseCol, overCol);
                    resultTex.SetPixel(x, y, c);
                }
            }

            return resultTex;
        }

        static Color NormalAlphaBlend(Color cb, Color cf)
        {
            return new Color(
                NormalAlphaBlend(cb.r, cb.a, cf.r, cf.a),
                NormalAlphaBlend(cb.g, cb.a, cf.g, cf.a),
                NormalAlphaBlend(cb.b, cb.a, cf.b, cf.a),
                NormalAlphaBlend(cb.a, cf.a));
        }

        static float NormalAlphaBlend(float cb, float ab, float cf, float af)
        {
            return Mathf.Clamp01(ab * (1 - af) * cb + af * cf);
        }

        static float NormalAlphaBlend(float ab, float af)
        {
            return Mathf.Clamp01(ab + af - ab * af);
        }
    }
}
