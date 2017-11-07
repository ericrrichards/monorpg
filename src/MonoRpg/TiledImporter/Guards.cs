// ReSharper disable once CheckNamespace
namespace Ardalis.GuardClauses {
    using System;
    using System.Xml.Linq;
    /// <summary>
    /// Simple interface to provide a generic mechanism to build guard clause extension methods from.
    /// </summary>
    public interface IGuardClause {
    }

    /// <summary>
    /// An entry point to a set of Guard Clauses defined as extension methods on IGuardClause.
    /// </summary>
    /// <remarks>See http://www.weeklydevtips.com/004 on Guard Clauses</remarks>
    public class Guard : IGuardClause {
        public static IGuardClause Against { get; } = new Guard();

        private Guard() { }
    }

    public static class Guards {
        public static void WrongXmlElement(this IGuardClause guardClause, XElement element, string parameterName, string expectedType) {
            if (element.Name != expectedType) {
                throw new ArgumentOutOfRangeException(parameterName, element.Value, $"Must be a <{expectedType}> element");
            }
        }
        /// <summary>
        /// Throws an ArgumentNullException if input is null.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Null(this IGuardClause guardClause, object input, string parameterName) {
            if (null == input) {
                throw new ArgumentNullException(parameterName);
            }
        }

        /// <summary>
        /// Throws an ArgumentNullException if input is null.
        /// Throws an ArgumentException if input is an empty string.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void NullOrEmpty(this IGuardClause guardClause, string input, string parameterName) {
            Guard.Against.Null(input, parameterName);
            if (input == String.Empty) {
                throw new ArgumentException($"Required input {parameterName} was empty.", parameterName);
            }
        }

        /// <summary>
        /// Throws an ArgumentOutOfRange if input is less than <see cref="rangeFrom"/> or greater than <see cref="rangeTo"/>
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="rangeFrom"></param>
        /// <param name="rangeTo"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void OutOfRange(this IGuardClause guardClause, int input, string parameterName,
                                      int rangeFrom, int rangeTo) {
            if (rangeFrom > rangeTo) {
                throw new ArgumentException($"{nameof(rangeFrom)} should be less or equal than {nameof(rangeTo)}");
            }

            if (input < rangeFrom || input > rangeTo) {
                throw new ArgumentOutOfRangeException($"Input {parameterName} was out of range", parameterName);
            }
        }
    }
}
