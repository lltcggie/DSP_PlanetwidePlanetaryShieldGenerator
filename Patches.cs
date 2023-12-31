using HarmonyLib;
using System;
using UnityEngine;

namespace PlanetaryShieldGenerator
{
    [HarmonyPatch]
    internal class PlanetaryShieldGeneratorPatches
    {
        [HarmonyPostfix, HarmonyPatch(typeof(PlanetATField), "UpdateGeneratorMatrix")]
        public static void PlanetATField_UpdateGeneratorMatrix_Postfix(PlanetATField __instance)
        {
            int count = __instance.generatorCount;

            if (count <= 0)
            {
                return;
            }

            // 惑星内の惑星シールドジェネレータで最も高いサポート率をサポート率として採用
            float maxW = 0.0f;
            for (int i = 0; i < count; i++)
            {
                maxW = Math.Max(__instance.generatorMatrix[i].w, maxW);
            }

            __instance.generatorCount = 0;
            Array.Clear(__instance.generatorMatrix, 0, PlanetATField.MAX_GENERATOR_COUNT);

            // 惑星全体を包めるようにジェネレータを配置したことにする
            // 配置位置はシールドの半径をshieldRadiusと仮定して隙間がないようかつ最小限の数に抑える
            double shieldRadius = 80.0; // 実験した範囲だとこの値で大丈夫そう
            double realRadius = __instance.planet.realRadius; // 惑星の半径
            int rCount = (int)Math.Ceiling((Math.PI * realRadius) / (2.0 * shieldRadius));

            // 緯度方向に等間隔に配置する
            Vector4 vec;
            vec.x = 0.0f;
            vec.y = (float)realRadius;
            vec.z = 0.0f;
            vec.w = maxW;
            __instance.generatorMatrix[__instance.generatorCount] = vec;
            __instance.generatorCount++;

            for (int i = 1; i <= rCount; i++)
            {
                double sita = (double)i * Math.PI / (double)rCount;
                vec.x = 0.0f;
                vec.y = (float)(realRadius * Math.Cos(sita));
                vec.z = (float)(realRadius * Math.Sin(sita));
                vec.w = maxW;
                __instance.generatorMatrix[__instance.generatorCount] = vec;
                __instance.generatorCount++;

                // 緯度方向に配置したのを原点として経度方向に等間隔に配置する
                double r2 = realRadius * Math.Sin(sita);
                int r2Count = (int)Math.Ceiling((Math.PI * r2 * 2.0) / (2.0 * shieldRadius));
                for (int j = 1; j < r2Count; j++)
                {
                    double sita2 = (double)j * 2.0 * Math.PI / (double)r2Count;
                    vec.x = (float)(r2 * Math.Sin(sita2));
                    vec.z = (float)(r2 * Math.Cos(sita2));
                    __instance.generatorMatrix[__instance.generatorCount] = vec;
                    __instance.generatorCount++;
                }
            }
        }
    }
}
