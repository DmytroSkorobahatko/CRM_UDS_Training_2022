using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpTest
{
    public class WorkDayCalculator : IWorkDayCalculator
    {
        // implement method Calculate to find last work day
        public DateTime Calculate(DateTime startDate, int dayCount, WeekEnd[] weekEnds)
        {
            DateTime lastWorkDate = startDate.AddDays(dayCount - 1);

            if (weekEnds == null)
                return lastWorkDate;

            int weekendIndex = FindFirstWeekendIndex(ref startDate, ref weekEnds);

            /// if first weekend ends befor startDate -> there are no weekends after startDate
            if (startDate > weekEnds[weekendIndex].EndDate)
                return lastWorkDate;

            /// if startDate is a weekend -> minus odd weekend days to algo works properly 
            if (startDate > weekEnds[weekendIndex].StartDate)
            {
                int weekendsCount = weekEnds[weekendIndex].StartDate.Subtract(startDate).Days;
                lastWorkDate = lastWorkDate.AddDays(weekendsCount);
            }

            while (weekendIndex < weekEnds.Length && weekEnds[weekendIndex].StartDate <= lastWorkDate)
            {
                int weekendsCount = weekEnds[weekendIndex].EndDate.Subtract(weekEnds[weekendIndex].StartDate).Days + 1;
                lastWorkDate = lastWorkDate.AddDays(weekendsCount);
                weekendIndex++;
            }
            return lastWorkDate;
        }

        // returns the index of the first weekend ends after startDate, otherwise the weekend befor startDate
        private static int FindFirstWeekendIndex(ref DateTime firstDate, ref WeekEnd[] weekEnds)
        {
            int leftIndex = 0;
            int rightIndex = weekEnds.Length-1;
            while(leftIndex < rightIndex)
            {
                int mid_index = (leftIndex+rightIndex)/2;
                if( (weekEnds[mid_index].StartDate <= firstDate) && (firstDate <= weekEnds[mid_index].EndDate) ) 
                    return mid_index;
                else 
                    if (weekEnds[mid_index].StartDate > firstDate) 
                        rightIndex = mid_index;
                    else 
                        leftIndex = mid_index + 1;
            }
            return leftIndex;
        }
    }
}
