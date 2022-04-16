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

        public static void SwapGridDefinitions(Grid grid)
        {
            Stack<int> initialRows = new Stack<int>();
            Stack<int> initialColumns = new Stack<int>();

            foreach (View gChild in grid.Children)
            {
                initialRows.Push(Grid.GetRow(gChild));
                initialColumns.Push(Grid.GetColumn(gChild));

                Grid.SetRow(gChild, 0);
                Grid.SetColumn(gChild, 0);
            }


            //Swap definitions collection

            RowDefinitionCollectionTypeConverter rowDefinitionsConverter = new RowDefinitionCollectionTypeConverter();
            string rowDefinition = rowDefinitionsConverter.ConvertToInvariantString(grid.RowDefinitions);

            ColumnDefinitionCollectionTypeConverter columnsDefinitionsConverter = new ColumnDefinitionCollectionTypeConverter();
            string columnDefinition = columnsDefinitionsConverter.ConvertToInvariantString(grid.ColumnDefinitions);

            grid.ColumnDefinitions = (ColumnDefinitionCollection) columnsDefinitionsConverter.ConvertFromInvariantString(rowDefinition);
            grid.RowDefinitions = (RowDefinitionCollection)rowDefinitionsConverter.ConvertFromInvariantString(columnDefinition);

            foreach (View gChild in grid.Children)
            {
                int row = initialRows.Pop();
                int column = initialColumns.Pop();

                Grid.SetRow(gChild, column);
                Grid.SetColumn(gChild, row);
            }
        }

    }
}