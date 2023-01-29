using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    /// <summary>
    /// Набор методов для генерации/настройки цвета.
    /// Помимо обычных статических методов, добавляет методы расширения для класса Color (Negate, Lighten, Darken)
    /// </summary>
    static class ColorUtils
    {
        /// <summary>
        /// Создаёт RGB цвет, принимая на вход цвет в формате HSL
        /// </summary>
        /// <param name="h">Цвет (0-360)</param>
        /// <param name="s">Насыщенность (0-1)</param>
        /// <param name="l">Освещенность (0-1)</param>
        /// <returns></returns>
        public static Color HslToRgb(double h, double s, double l)
        {
            double r, g, b;

            if (s == 0)
            {
                r = g = b = l; // achromatic
            }
            else
            {
                double q = l < 0.5 ? l * (1 + s) : l + s - l * s;
                double p = 2 * l - q;
                r = HueToRgb(p, q, h + 1 / 3);
                g = HueToRgb(p, q, h);
                b = HueToRgb(p, q, h - 1 / 3);
            }

            return new Color((int)(r * 255), (int)(g * 255), (int)(b * 255), 1);
        }

        private static double HueToRgb(double p, double q, double t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1 / 6) return p + (q - p) * 6 * t;
            if (t < 1 / 2) return q;
            if (t < 2 / 3) return p + (q - p) * (2 / 3 - t) * 6;
            return p;
        }

        /// <summary>
        /// Создаёт RGB цвет, принимая на вход цвет в формате HSV
        /// </summary>
        /// <param name="hue">Цвет (0-360)</param>
        /// <param name="sturatrion">Насыщенность (0-1)</param>
        /// <param name="value">Значение (0-1)</param>
        /// <returns></returns>
        public static Color HsvToRgb(double hue, double sturatrion, double value)
        {
            double c = value * sturatrion;
            double x = c * (1 - Math.Abs((hue / 60) % 2 - 1));
            double m = value - c;

            int r, g, b;
            if (hue < 60)
            {
                r = (int)((c + m) * 255);
                g = (int)((x + m) * 255);
                b = (int)((0 + m) * 255);
            }
            else if (hue < 120)
            {
                r = (int)((x + m) * 255);
                g = (int)((c + m) * 255);
                b = (int)((0 + m) * 255);
            }
            else if (hue < 180)
            {
                r = (int)((0 + m) * 255);
                g = (int)((c + m) * 255);
                b = (int)((x + m) * 255);
            }
            else if (hue < 240)
            {
                r = (int)((0 + m) * 255);
                g = (int)((x + m) * 255);
                b = (int)((c + m) * 255);
            }
            else if (hue < 300)
            {
                r = (int)((x + m) * 255);
                g = (int)((0 + m) * 255);
                b = (int)((c + m) * 255);
            }
            else
            {
                r = (int)((c + m) * 255);
                g = (int)((0 + m) * 255);
                b = (int)((x + m) * 255);
            }

            return new Color(r, g, b, 255);
        }

        /// <summary>
        /// Преобразует RGB цвет в HSL.
        /// </summary>
        /// <returns>Массив из трех HSL значений</returns>
        public static double[] RgbToHsl(this Color color)
        {
            double r = (double) color.R / 255;
            double g = (double) color.G / 255;
            double b = (double) color.B / 255;
            double min = Math.Min(Math.Min(r, g), b);
            double max = Math.Max(Math.Max(r, g), b);
            double delta = max - min;
            double h, s, l = (max + min) / 2;

            if (delta == 0)
            {
                h = 0;
                s = 0;
            }
            else
            {
                s = l > 0.5 ? delta / (2 - max - min) : delta / (max + min);
                if (r == max) h = (g - b) / delta + (g < b ? 6 : 0);
                else if (g == max) h = (b - r) / delta + 2;
                else h = (r - g) / delta + 4;
                h /= 6;
            }

            return new double[] { h * 360, s, l };
        }


        /// <summary>
        /// Преобразует RGB цвет в HSV.
        /// </summary>
        /// <param name="color"></param>
        /// <returns>Массив из трех HSV значений</returns>
        public static double[] RgbToHsv(this Color color)
        {
            double r = (double) color.R / 255;
            double g = (double) color.G / 255;
            double b = (double) color.B / 255;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));

            double h, s, v;
            v = max;

            double d = max - min;
            s = max == 0 ? 0 : d / max;

            if (max == min)
            {
                h = 0;
            }
            else
            {
                if (max == r)
                {
                    h = (g - b) / d + (g < b ? 6 : 0);
                }
                else if (max == g)
                {
                    h = (b - r) / d + 2;
                }
                else
                {
                    h = (r - g) / d + 4;
                }

                h /= 6;
            }

            return new double[] { h * 360, s, v };
        }

        /// <summary>
        /// Возвращает негатив текущего цвета
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color Negate(this Color color)
        {
            return new Color(255 - color.R, 255 - color.G, 255 - color.B, color.A);
        }

        /// <summary>
        /// Затеняет текущий цвет
        /// </summary>
        /// <param name="color"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static Color Darken(this Color color, double amount)
        {
            return new Color((int)(color.R * (1 - amount)),
                (int)(color.G * (1 - amount)),
                (int)(color.B * (1 - amount)),
                color.A);
        }

        /// <summary>
        /// Осветляет текущий цвет
        /// </summary>
        /// <param name="color"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static Color Lighten(this Color color, double amount)
        {
            return new Color((int)(color.R + (255 - color.R) * amount),
                (int)(color.G + (255 - color.G) * amount),
                (int)(color.B + (255 - color.B) * amount),
                color.A);
        }
    }


}
