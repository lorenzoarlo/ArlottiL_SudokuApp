using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ArlottiL_SudokuApp
{
    public class Utilities
    {
        public static double GetProportionalCoordinate(double coordinate, double size) => coordinate / (1 - size);

        
        private static List<bool> activePulses = new List<bool>();

        public static int NewPulseAnimation()
        {
            int myID = activePulses.Count;
            activePulses.Add(true);
            return myID;
        }

        public async static void RunPulseAnimation(int animationID, VisualElement element, double initial, double final, uint duration)
        {
            while (activePulses[animationID])
            {
                await element.ScaleTo(final, duration / 2);
                await element.ScaleTo(initial, duration / 2);
            }
        }

        public static void DeletePulseAnimation(int animationID) => activePulses[animationID] = false;
        
        
    
    }
}