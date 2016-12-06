///////////////////////////////////////////////////////////////////////////////
//  COPYRIGHT (c) 2011 Schweitzer Engineering Laboratories, Pullman, WA
//////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Windows.Media;

namespace SEL
{
    public class DistinguishableColors
    {
        public DistinguishableColors(bool useDarkBackground = true)
        {
            UseDarkBackground = useDarkBackground;
        }

        public readonly bool UseDarkBackground;

        private static Color ColorFromPercentRgb(double percentRed, double percentGreen, double percentBlue)
        {
            if ((percentRed > 1) || (percentGreen > 1) || (percentBlue > 1) ||
                (percentRed < 0) || (percentGreen < 0) || (percentBlue < 0))
            {
                throw new System.ArgumentOutOfRangeException("Percent must be between 0 and 1");
            }

            return Color.FromArgb(
                byte.MaxValue,
                (byte)(percentRed * byte.MaxValue),
                (byte)(percentGreen * byte.MaxValue),
                (byte)(percentBlue * byte.MaxValue));
        }

        private IEnumerable<Color> CreateColorSet(double[] PercentArray)
        {
            for (int blueIndex = 0; blueIndex < PercentArray.Length; ++blueIndex)
            {
                for (int greenIndex = 0; greenIndex < PercentArray.Length; ++greenIndex)
                {
                    for (int redIndex = 0; redIndex < PercentArray.Length; ++redIndex)
                    {
                        if (UseDarkBackground)
                        {
                            // Colors that have a combined color percentage of .5 or less are too dark 
                            // to be seen on a dark background
                            if ((PercentArray[redIndex] + PercentArray[greenIndex] + PercentArray[blueIndex] <= .5) ||
                                (PercentArray[redIndex] + PercentArray[greenIndex] < .1))
                                continue;
                        }
                        else
                        {
                            // Colors that have a combined color percentage of 2.5 or greater are too light 
                            // to be seen on a light background
                            if (PercentArray[redIndex] + PercentArray[greenIndex] + PercentArray[blueIndex] >= 2.5)
                                continue;
                        }

                        yield return ColorFromPercentRgb(
                            PercentArray[redIndex], PercentArray[greenIndex], PercentArray[blueIndex]);
                    }
                }
            }
        }

        public IEnumerable<Color> AllColors()
        {
            var set = new HashSet<Color>();

            // Place the colors that have the most contrast at the beginning of the set
            // and continue adding gradients.  Somewhere in the 3rd color set the colors begin
            // to be hard to differentiate.
            set.UnionWith(CreateColorSet(new double[] { 0, 1 }));
            if (UseDarkBackground)
            {
                set.Add(ColorFromPercentRgb(0, .5, 1)); // Move a good blue to the front
            }
            set.UnionWith(CreateColorSet(new double[] { 0, .5, 1 }));
            set.UnionWith(CreateColorSet(new double[] { 0, .5, .75, 1 }));
            set.UnionWith(CreateColorSet(new double[] { 0, .25, .5, .75, 1 }));
            set.UnionWith(CreateColorSet(new double[] { 0, .2, .4, .6, .8, 1 }));
            set.UnionWith(CreateColorSet(new double[] { 0, .1, .2, .3, .4, .5, .6, .7, .8, .9, 1 }));

            return set;
        }
    }
}
