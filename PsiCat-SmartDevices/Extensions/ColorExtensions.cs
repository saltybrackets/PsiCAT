namespace PsiCat.SmartDevices
{
    using System;
    using System.Drawing;


    public static class ColorExtensions
    {
        public static float GetRealSaturation(this Color color)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            return (max == 0) ? 0 : 1f - (1f * min / max);
        }


        public static float GetValue(this Color color)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            return max / 255f;
        }
    }
}