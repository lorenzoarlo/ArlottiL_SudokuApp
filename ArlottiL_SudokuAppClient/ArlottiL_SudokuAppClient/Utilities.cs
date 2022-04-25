using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ArlottiL_SudokuAppClient
{
    public class Utilities
    {
        public static double GetProportionalCoordinate(double coordinate, double size) => coordinate / (1 - size);


        private static List<bool> _pulsations = new List<bool>();

        public static int NewPulseAnimation()
        {
            int myID = _pulsations.Count;
            _pulsations.Add(true);
            return myID;
        }

        public async static void RunPulseAnimation(int animationID, VisualElement element, double initial, double final, uint duration)
        {
            _pulsations[animationID] = true;
            while (_pulsations[animationID])
            {
                await element.ScaleTo(final, duration / 2);
                await element.ScaleTo(initial, duration / 2);
            }
        }

        public static void StopPulseAnimation(int animationID) => _pulsations[animationID] = false;

        public async static void ShakeAnimation(VisualElement element)
        {
            double offset = element.Width * 0.1;
            await element.TranslateTo(offset, 0, 100);
            await element.TranslateTo(-offset, 0, 100);
            await element.TranslateTo(0, 0, 100);
        }

    }
}
