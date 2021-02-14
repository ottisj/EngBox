using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace EngBox
{
    public static class Collaborator
    {
        /// <summary>
        /// String format container
        /// </summary>
        public static string NumericFormat = "G3";
        /// <summary>
        /// Last valid value 
        /// </summary>
        public static double OldValue;

        /// <summary>
        /// Extension to loop through all controls. From CodeProject
        /// </summary>
        /// <typeparam name="T">ControlType</typeparam>
        /// <param name="depObj">Where to find</param>
        /// <returns>Objects found</returns>
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
