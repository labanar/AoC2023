using AoC_2023.Utilities;

namespace AoC_2023.Solutions
{
    internal class Day6 : ISolution
    {
        public string Part1(ReadOnlySpan<char> input)
        {
            var total = 1;
            Helpers.ReadLine(ref input, out var timeLine);
            //Discard everything up to and including ':' for both lines
            timeLine.ReadTo(out _, ": ");
            input.ReadTo(out _, ": ");
            while (timeLine.Length > 0)
            {
                timeLine = timeLine.Trim();
                input = input.Trim();
                timeLine.ReadTo(out var timeChars, ' ');
                input.ReadTo(out var distanceChars, ' ');
                var tTotal = int.Parse(timeChars);
                var dRecord = int.Parse(distanceChars);

                if (dRecord == 0)
                {
                    total = 0;
                    break;
                }

                //d = v * (tMoving) 
                //where v = tPressed, tMoving = (tTotal - tPressed)
                //∴ d = tPressed * (tTotal - tPressed)
                //d = tPressed * tTotal - tPressed^2
                //d/dtPressed = -2tPressed + tTotal
                //Solve for the maximum
                //0 = -2tPressed + tTotal
                //∴ tPressed = tTotal/2

                var tPressedMax = GetTPressedMax(tTotal, dRecord);
                var dMax = tPressedMax * (tTotal - tPressedMax);

                //Walk up and deterine when we fall below dRecord
                var i = 0;
                var dTest = dMax;
                var waysToWin = 0;
                do
                {
                    waysToWin++;
                    i++;
                    dTest = (tPressedMax + i) * (tTotal - (tPressedMax + i));
                }
                while (dTest > dRecord);


                //Walk down and deterine when we fall below dRecord
                i = -1;
                dTest = (tPressedMax + i) * (tTotal - (tPressedMax + i));
                while (dTest > dRecord)
                {
                    waysToWin++;
                    i--;
                    dTest = (tPressedMax + i) * (tTotal - (tPressedMax + i));
                }

                total *= waysToWin;
            }

            return total.ToString();
        }

        private static int GetTPressedMax(int tTotal, int distanceRecord)
        {
            var tPressedMax = tTotal / 2f;
            if (tPressedMax % 1 == 0) return (int)tPressedMax;

            var tPressedMaxUpper = Math.Ceiling(tTotal / 2f);
            var dMaxUpper = Math.Floor(tPressedMaxUpper * (tTotal - tPressedMaxUpper));

            var tPressedMaxLower = Math.Floor(tTotal / 2f);
            var dMaxLower = Math.Floor(tPressedMaxLower * (tTotal - tPressedMaxLower));

            return (dMaxUpper > dMaxLower) ? (int)tPressedMaxUpper : (int)tPressedMaxLower;

        }

        public string Part2(ReadOnlySpan<char> input)
        {
            return "X";
        }
    }
}
