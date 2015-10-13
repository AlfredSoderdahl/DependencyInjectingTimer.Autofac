using System;

namespace DependencyInjectingTimer.Autofac {
    public static class TimeExtensions {
        public static int FromHoursToMilliseconds(this int hours) {
            return (int) TimeSpan.FromHours(hours).TotalMilliseconds;
        }

        public static int FromSecondsToMilliseconds(this int seconds) {
            return (int) TimeSpan.FromSeconds(seconds).TotalMilliseconds;
        }

        public static int AssertGreaterThanZero(this int value, string nameOfValue = null) {
            if (value < 1) {
                throw new ArgumentOutOfRangeException(nameof(value),
                                                      $"{nameOfValue ?? "Value"} is not greater than zero");
            }

            return value;
        }
    }
}