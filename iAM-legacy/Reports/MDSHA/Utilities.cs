using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DatabaseManager;
using RoadCareDatabaseOperations;

namespace Reports.MDSHA
{
    /// <summary>
    ///     This static class gathers some useful data and behavior for easier
    ///     implementation of MDSHA RoadCare reports.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        ///     This is a message box caption to show relatedness to the
        ///     reports.
        /// </summary>
        public const string Caption = "RoadCare Reports";

        private const string QueryTemplatePerformanceAttributes =
@"
SELECT DISTINCT a.attribute_,
  level1,
  level2,
  level3,
  level4,
  level5
FROM performance p
JOIN attributes_ a
ON p.attribute_    = a.attribute_
WHERE simulationid = {0}
AND type_          = 'NUMBER'
ORDER BY a.attribute_
";

        /// <summary>
        ///     Retrieves the analysis years for the given simulation.
        /// </summary>
        /// <param name="simulationId"></param>
        /// <returns>a list of analysis years</returns>
        public static List<string> GetAnalysisYears(string simulationId) =>
            DBOp.QueryBudgetYears(simulationId).Tables[0].AsEnumerable()
            .Select(r => r[0].ToString()).ToList();

        /// <summary>
        ///     Retrieves the analysis years and most recent data collection year for the given simulation.
        /// </summary>
        /// <param name="simulationId"></param>
        /// <returns>a list of analysis years and most recent data collection year</returns>
        public static List<string> GetAnalysisYearsInclMostRecent(string simulationId) =>
            DBOp.QueryBudgetYearsinclmostrecent(simulationId).Tables[0].AsEnumerable()
            .Select(r => r[0].ToString()).ToList();

        /// <summary>
        ///     Retrieves a map from the set of a simulation's numeric
        ///     attributes to the associated bound values of each attribute.
        /// </summary>
        /// <param name="simulationId"></param>
        /// <returns>
        ///     a dictionary mapping attributes to arrays of bound values
        /// </returns>
        public static Dictionary<string, double?[]> GetDefaultAttributeBounds(string simulationId)
        {
            var queryString = string.Format(
                QueryTemplatePerformanceAttributes,
                simulationId);

            var queryResultSet = DBMgr.ExecuteQuery(queryString);

            return
                queryResultSet.Tables[0].Rows.Cast<DataRow>().ToDictionary(
                dr => dr.Field<string>(0).Trim(),
                dr => dr.ItemArray.Skip(1).Select(o =>
                    Convert.IsDBNull(o) ?
                    (double?) null :
                    Convert.ToDouble(o)).ToArray());
        }

        /// <summary>
        ///     Shows a basic message box with RoadCare Reports captioning.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>the result of the message dialog</returns>
        public static DialogResult Show(string message) =>
            MessageBox.Show(message, Caption);

        /// <summary>
        ///     Shows a Reports-captioned message box with custom buttons.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <returns>the result of the message dialog</returns>
        public static DialogResult Show(string message, MessageBoxButtons buttons) =>
            MessageBox.Show(message, Caption, buttons);

        /// <summary>
        ///     Shows a Reports-captioned message box with custom buttons and
        ///     icon.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <param name="icon"></param>
        /// <returns>the result of the message dialog</returns>
        public static DialogResult Show(string message, MessageBoxButtons buttons, MessageBoxIcon icon) =>
            MessageBox.Show(message, Caption, buttons, icon);

        /// <summary>
        ///     This simply wraps Enumerable.Zip so that one can pass an Action
        ///     (void-returning delegate) rather than a Func.
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="action"></param>
        /// <remarks>
        ///     This was originally a custom extension method on generic
        ///     IEnumerable. Then the compiler got confused between this Zip and
        ///     the standard one. So now it's a plain static method. Rule of
        ///     thumb: Only define extension methods on types that you own.
        /// </remarks>
        public static void Zip<TFirst, TSecond>(
            IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            Action<TFirst, TSecond> action)
        {
            first.Zip<TFirst, TSecond, object>(
                second,
                (s, t) => { action(s, t); return null; })
                .ToArray(); // to force evaluations of action
        }

        /// <summary>
        ///     Iterator over the outer product of two sequences, combining
        ///     elements using the given function.
        /// </summary>
        /// <typeparam name="TA"></typeparam>
        /// <typeparam name="TB"></typeparam>
        /// <typeparam name="TC"></typeparam>
        /// <param name="f"></param>
        /// <param name="seqA"></param>
        /// <param name="seqB"></param>
        /// <returns>a sequence of combined elements</returns>
        /// <remarks>
        ///     It could be useful to generalize this to a method accepting an
        ///     arbitrary number of input sequences. I think it'd have to accept
        ///     a Delegate and do dynamic invocation, so no strong typing on the
        ///     arguments of the combiner, but it could still be handy.
        /// </remarks>
        public static IEnumerable<TC> FlatOuter<TA, TB, TC>(
            Func<TA, TB, TC> f,
            IEnumerable<TA> seqA,
            IEnumerable<TB> seqB)
        {
            foreach (var a in seqA)
            {
                foreach (var b in seqB)
                {
                    yield return f(a, b);
                }
            }
        }

        /// <summary>
        ///     Allows a textbox's contents to be safely upper-cased while
        ///     preserving the selection location.
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="tbTextChangedHandler"></param>
        public static void InertTextBoxToUpper(
            TextBox tb,
            EventHandler tbTextChangedHandler)
        {
            var prevSelStart = tb.SelectionStart;

            tb.TextChanged -= tbTextChangedHandler;
            tb.Text = tb.Text.ToUpper();
            tb.TextChanged += tbTextChangedHandler;

            tb.SelectionStart = prevSelStart;
        }
    }
}
