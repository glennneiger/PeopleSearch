using System;

namespace PeopleSearch.Extentions
{
    public static class GuardExtensions
    {
        /// <summary>
        /// Guards against null values otherwise throws an exception
        /// </summary>
        /// <param name="value">The value to guard</param>
        /// <param name="paramName">The paramName to guard</param>
        /// <param name="message">An optional message that will be surfaced in the exception</param>
        public static void GuardNull(this object value, string paramName, string message = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName, message);
            }
        }

        /// <summary>
        /// Guards against empty strings otherwise throws an exception
        /// </summary>
        /// <param name="value">The value to guard</param>
        /// <param name="paramName">The paramName to guard</param>
        /// <param name="message">An optional message that will be surfaced in the exception</param>
        public static void GuardEmpty(this string value, string paramName, string message = null)
        {
            GuardNull(value, paramName, message);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(message ?? $"{paramName} cannot be empty.", paramName);
            }
        }
    }
}
