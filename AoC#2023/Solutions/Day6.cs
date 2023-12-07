using AoC_2023.Utilities;

namespace AoC_2023.Solutions
{
    internal class Day6 : ISolution
    {
        public string Part1(ReadOnlySpan<char> input)
        {
            var total = 1L;
            input.ReadTo(out var timeLine, '\n');
            timeLine.ReadTo(out _, ": ");
            input.ReadTo(out _, ": ");
            while (timeLine.Length > 0)
            {
                timeLine = timeLine.Trim();
                input = input.Trim();
                timeLine.ReadTo(out var timeChars, ' ');
                input.ReadTo(out var distanceChars, ' ');
                var recordTime = int.Parse(timeChars);
                var recordDistance = int.Parse(distanceChars);
                total *= CountWaysToWin(recordTime, recordDistance);
            }
            return total.ToString();
        }

        public string Part2(ReadOnlySpan<char> input)
        {
            input.ReadTo(out var timeLine, '\n');
            timeLine.ReadTo(out _, ": ");
            input.ReadTo(out _, ": ");
            var recordTime = ReadNumber(timeLine);
            var recordDistance = ReadNumber(input);
            return CountWaysToWin(recordTime, recordDistance).ToString();
        }

        private static long ReadNumber(ReadOnlySpan<char> numbersWithSpaces)
        {
            var digitsObserved = 0;
            var number = 0l;
            while (numbersWithSpaces.ReadFromEnd(out var numberChars, ' '))
            {
                numbersWithSpaces = numbersWithSpaces.Trim();
                var currentNum = long.Parse(numberChars);
                number += currentNum * (long)Math.Pow(10, digitsObserved);
                digitsObserved += numberChars.Trim().Length;
            }
            return number;
        }

        private static int CountWaysToWin(long recordTime, long recordDistance)
        {
            if (recordDistance == 0)
                return 0;

            //d = v * (tMoving) 
            //where v = tPressed, tMoving = (tTotal - tPressed)
            //∴ d = tPressed * (tTotal - tPressed)
            //d = tPressed * tTotal - tPressed^2
            //d/dtPressed = -2tPressed + tTotal
            //Solve for the maximum
            //0 = -2tPressed + tTotal
            //∴ tPressed = tTotal/2
            var tPressedMax = GetPressDurationForMaxDistance(recordTime);
            var dMax = tPressedMax * (recordTime - tPressedMax);

            //Walk up and determine when we fall below dRecord
            var i = 0;
            var dTest = dMax;
            var waysToWin = 0;
            do
            {
                waysToWin++;
                i++;
                dTest = (tPressedMax + i) * (recordTime - (tPressedMax + i));
            }
            while (dTest > recordDistance);

            //Walk down and determine when we fall below dRecord
            i = -1;
            dTest = (tPressedMax + i) * (recordTime - (tPressedMax + i));
            while (dTest > recordDistance)
            {
                waysToWin++;
                i--;
                dTest = (tPressedMax + i) * (recordTime - (tPressedMax + i));
            }

            return waysToWin;
        }

        private static long GetPressDurationForMaxDistance(long recordTime)
        {
            var tPressedMax = recordTime / 2f;
            if (tPressedMax % 1 == 0) return (long)tPressedMax;

            var tPressedMaxUpper = Math.Ceiling(recordTime / 2f);
            var dMaxUpper = Math.Floor(tPressedMaxUpper * (recordTime - tPressedMaxUpper));

            var tPressedMaxLower = Math.Floor(recordTime / 2f);
            var dMaxLower = Math.Floor(tPressedMaxLower * (recordTime - tPressedMaxLower));

            return (dMaxUpper > dMaxLower) ? (long)tPressedMaxUpper : (long)tPressedMaxLower;
        }
    }
}
