// WARNING: This file was automatically generated from a T4 text template at the
// following moment in time: 04/21/2020 15:11:32 -05:00. Any changes you make to
// this file will be lost when this file is regenerated from the template
// source.

using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates
{
    partial class Static
    {
        public static IList<TElement>[] Channel<TElement>(this IEnumerable<TElement> elements, params Predicate<TElement>[] predicates) => elements.Channel(Identity, predicates);

        public static IList<TElement>[] Channel<TElement, TSelection>(this IEnumerable<TElement> elements, Func<TElement, TSelection> selector, params Predicate<TSelection>[] predicates)
        {
            var destinations = predicates.Select(_ => new List<TElement>()).ToArray();

            foreach (var element in elements)
            {
                var selection = selector(element);

                foreach (var (predicate, destination) in Zip.Strict(predicates, destinations))
                {
                    if (predicate(selection))
                    {
                        destination.Add(element);
                    }
                }
            }

            return destinations;
        }

        public static void Channel<TElement>(this IEnumerable<TElement> elements, Predicate<TElement> predicate1, Predicate<TElement> predicate2, out IList<TElement> channel1, out IList<TElement> channel2) => elements.Channel(Identity, predicate1, predicate2, out channel1, out channel2);

        public static void Channel<TElement, TSelection>(this IEnumerable<TElement> elements, Func<TElement, TSelection> selector, Predicate<TSelection> predicate1, Predicate<TSelection> predicate2, out IList<TElement> channel1, out IList<TElement> channel2)
        {
            channel1 = new List<TElement>();
            channel2 = new List<TElement>();

            foreach (var element in elements)
            {
                var selection = selector(element);

                if (predicate1(selection)) channel1.Add(element);
                if (predicate2(selection)) channel2.Add(element);
            }
        }

        public static void Channel<TElement>(this IEnumerable<TElement> elements, Predicate<TElement> predicate1, Predicate<TElement> predicate2, Predicate<TElement> predicate3, out IList<TElement> channel1, out IList<TElement> channel2, out IList<TElement> channel3) => elements.Channel(Identity, predicate1, predicate2, predicate3, out channel1, out channel2, out channel3);

        public static void Channel<TElement, TSelection>(this IEnumerable<TElement> elements, Func<TElement, TSelection> selector, Predicate<TSelection> predicate1, Predicate<TSelection> predicate2, Predicate<TSelection> predicate3, out IList<TElement> channel1, out IList<TElement> channel2, out IList<TElement> channel3)
        {
            channel1 = new List<TElement>();
            channel2 = new List<TElement>();
            channel3 = new List<TElement>();

            foreach (var element in elements)
            {
                var selection = selector(element);

                if (predicate1(selection)) channel1.Add(element);
                if (predicate2(selection)) channel2.Add(element);
                if (predicate3(selection)) channel3.Add(element);
            }
        }

        public static void Channel<TElement>(this IEnumerable<TElement> elements, Predicate<TElement> predicate1, Predicate<TElement> predicate2, Predicate<TElement> predicate3, Predicate<TElement> predicate4, out IList<TElement> channel1, out IList<TElement> channel2, out IList<TElement> channel3, out IList<TElement> channel4) => elements.Channel(Identity, predicate1, predicate2, predicate3, predicate4, out channel1, out channel2, out channel3, out channel4);

        public static void Channel<TElement, TSelection>(this IEnumerable<TElement> elements, Func<TElement, TSelection> selector, Predicate<TSelection> predicate1, Predicate<TSelection> predicate2, Predicate<TSelection> predicate3, Predicate<TSelection> predicate4, out IList<TElement> channel1, out IList<TElement> channel2, out IList<TElement> channel3, out IList<TElement> channel4)
        {
            channel1 = new List<TElement>();
            channel2 = new List<TElement>();
            channel3 = new List<TElement>();
            channel4 = new List<TElement>();

            foreach (var element in elements)
            {
                var selection = selector(element);

                if (predicate1(selection)) channel1.Add(element);
                if (predicate2(selection)) channel2.Add(element);
                if (predicate3(selection)) channel3.Add(element);
                if (predicate4(selection)) channel4.Add(element);
            }
        }

        public static void Channel<TElement>(this IEnumerable<TElement> elements, Predicate<TElement> predicate1, Predicate<TElement> predicate2, Predicate<TElement> predicate3, Predicate<TElement> predicate4, Predicate<TElement> predicate5, out IList<TElement> channel1, out IList<TElement> channel2, out IList<TElement> channel3, out IList<TElement> channel4, out IList<TElement> channel5) => elements.Channel(Identity, predicate1, predicate2, predicate3, predicate4, predicate5, out channel1, out channel2, out channel3, out channel4, out channel5);

        public static void Channel<TElement, TSelection>(this IEnumerable<TElement> elements, Func<TElement, TSelection> selector, Predicate<TSelection> predicate1, Predicate<TSelection> predicate2, Predicate<TSelection> predicate3, Predicate<TSelection> predicate4, Predicate<TSelection> predicate5, out IList<TElement> channel1, out IList<TElement> channel2, out IList<TElement> channel3, out IList<TElement> channel4, out IList<TElement> channel5)
        {
            channel1 = new List<TElement>();
            channel2 = new List<TElement>();
            channel3 = new List<TElement>();
            channel4 = new List<TElement>();
            channel5 = new List<TElement>();

            foreach (var element in elements)
            {
                var selection = selector(element);

                if (predicate1(selection)) channel1.Add(element);
                if (predicate2(selection)) channel2.Add(element);
                if (predicate3(selection)) channel3.Add(element);
                if (predicate4(selection)) channel4.Add(element);
                if (predicate5(selection)) channel5.Add(element);
            }
        }

        public static void Channel<TElement>(this IEnumerable<TElement> elements, Predicate<TElement> predicate1, Predicate<TElement> predicate2, Predicate<TElement> predicate3, Predicate<TElement> predicate4, Predicate<TElement> predicate5, Predicate<TElement> predicate6, out IList<TElement> channel1, out IList<TElement> channel2, out IList<TElement> channel3, out IList<TElement> channel4, out IList<TElement> channel5, out IList<TElement> channel6) => elements.Channel(Identity, predicate1, predicate2, predicate3, predicate4, predicate5, predicate6, out channel1, out channel2, out channel3, out channel4, out channel5, out channel6);

        public static void Channel<TElement, TSelection>(this IEnumerable<TElement> elements, Func<TElement, TSelection> selector, Predicate<TSelection> predicate1, Predicate<TSelection> predicate2, Predicate<TSelection> predicate3, Predicate<TSelection> predicate4, Predicate<TSelection> predicate5, Predicate<TSelection> predicate6, out IList<TElement> channel1, out IList<TElement> channel2, out IList<TElement> channel3, out IList<TElement> channel4, out IList<TElement> channel5, out IList<TElement> channel6)
        {
            channel1 = new List<TElement>();
            channel2 = new List<TElement>();
            channel3 = new List<TElement>();
            channel4 = new List<TElement>();
            channel5 = new List<TElement>();
            channel6 = new List<TElement>();

            foreach (var element in elements)
            {
                var selection = selector(element);

                if (predicate1(selection)) channel1.Add(element);
                if (predicate2(selection)) channel2.Add(element);
                if (predicate3(selection)) channel3.Add(element);
                if (predicate4(selection)) channel4.Add(element);
                if (predicate5(selection)) channel5.Add(element);
                if (predicate6(selection)) channel6.Add(element);
            }
        }

        public static void Channel<TElement>(this IEnumerable<TElement> elements, Predicate<TElement> predicate1, Predicate<TElement> predicate2, Predicate<TElement> predicate3, Predicate<TElement> predicate4, Predicate<TElement> predicate5, Predicate<TElement> predicate6, Predicate<TElement> predicate7, out IList<TElement> channel1, out IList<TElement> channel2, out IList<TElement> channel3, out IList<TElement> channel4, out IList<TElement> channel5, out IList<TElement> channel6, out IList<TElement> channel7) => elements.Channel(Identity, predicate1, predicate2, predicate3, predicate4, predicate5, predicate6, predicate7, out channel1, out channel2, out channel3, out channel4, out channel5, out channel6, out channel7);

        public static void Channel<TElement, TSelection>(this IEnumerable<TElement> elements, Func<TElement, TSelection> selector, Predicate<TSelection> predicate1, Predicate<TSelection> predicate2, Predicate<TSelection> predicate3, Predicate<TSelection> predicate4, Predicate<TSelection> predicate5, Predicate<TSelection> predicate6, Predicate<TSelection> predicate7, out IList<TElement> channel1, out IList<TElement> channel2, out IList<TElement> channel3, out IList<TElement> channel4, out IList<TElement> channel5, out IList<TElement> channel6, out IList<TElement> channel7)
        {
            channel1 = new List<TElement>();
            channel2 = new List<TElement>();
            channel3 = new List<TElement>();
            channel4 = new List<TElement>();
            channel5 = new List<TElement>();
            channel6 = new List<TElement>();
            channel7 = new List<TElement>();

            foreach (var element in elements)
            {
                var selection = selector(element);

                if (predicate1(selection)) channel1.Add(element);
                if (predicate2(selection)) channel2.Add(element);
                if (predicate3(selection)) channel3.Add(element);
                if (predicate4(selection)) channel4.Add(element);
                if (predicate5(selection)) channel5.Add(element);
                if (predicate6(selection)) channel6.Add(element);
                if (predicate7(selection)) channel7.Add(element);
            }
        }

        public static void Channel<TElement>(this IEnumerable<TElement> elements, Predicate<TElement> predicate1, Predicate<TElement> predicate2, Predicate<TElement> predicate3, Predicate<TElement> predicate4, Predicate<TElement> predicate5, Predicate<TElement> predicate6, Predicate<TElement> predicate7, Predicate<TElement> predicate8, out IList<TElement> channel1, out IList<TElement> channel2, out IList<TElement> channel3, out IList<TElement> channel4, out IList<TElement> channel5, out IList<TElement> channel6, out IList<TElement> channel7, out IList<TElement> channel8) => elements.Channel(Identity, predicate1, predicate2, predicate3, predicate4, predicate5, predicate6, predicate7, predicate8, out channel1, out channel2, out channel3, out channel4, out channel5, out channel6, out channel7, out channel8);

        public static void Channel<TElement, TSelection>(this IEnumerable<TElement> elements, Func<TElement, TSelection> selector, Predicate<TSelection> predicate1, Predicate<TSelection> predicate2, Predicate<TSelection> predicate3, Predicate<TSelection> predicate4, Predicate<TSelection> predicate5, Predicate<TSelection> predicate6, Predicate<TSelection> predicate7, Predicate<TSelection> predicate8, out IList<TElement> channel1, out IList<TElement> channel2, out IList<TElement> channel3, out IList<TElement> channel4, out IList<TElement> channel5, out IList<TElement> channel6, out IList<TElement> channel7, out IList<TElement> channel8)
        {
            channel1 = new List<TElement>();
            channel2 = new List<TElement>();
            channel3 = new List<TElement>();
            channel4 = new List<TElement>();
            channel5 = new List<TElement>();
            channel6 = new List<TElement>();
            channel7 = new List<TElement>();
            channel8 = new List<TElement>();

            foreach (var element in elements)
            {
                var selection = selector(element);

                if (predicate1(selection)) channel1.Add(element);
                if (predicate2(selection)) channel2.Add(element);
                if (predicate3(selection)) channel3.Add(element);
                if (predicate4(selection)) channel4.Add(element);
                if (predicate5(selection)) channel5.Add(element);
                if (predicate6(selection)) channel6.Add(element);
                if (predicate7(selection)) channel7.Add(element);
                if (predicate8(selection)) channel8.Add(element);
            }
        }
    }
}
