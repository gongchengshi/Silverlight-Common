///////////////////////////////////////////////////////////////////////////////
//  COPYRIGHT (c) 2011 Schweitzer Engineering Laboratories, Pullman, WA
//////////////////////////////////////////////////////////////////////////////

using System.Windows;
using System.Windows.Controls;
using DeathToXAML;

namespace InfragisticsTest
{
    public class GridElement : Element
    {
        public GridElement(UIElement element, int row, int col) : base(element)
        {
            Grid.SetRow(element as FrameworkElement, row);
            Grid.SetColumn(element as FrameworkElement, col);
        }
    }
}
