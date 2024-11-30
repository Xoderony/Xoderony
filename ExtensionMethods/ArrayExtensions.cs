using System;
using System.Collections.Generic;

namespace GuestUnion.ArrayExtensions {

    public static class ArrayExtensions {

        public static void Resize<T>(this T[] array, int newSize, out T[] newArray) {
            Array.Resize(ref array, newSize);
            newArray = array;
        }

        public static bool TrueForAll<T>(this T[] array, Predicate<T> match) => Array.TrueForAll(array, match);

        public static int FindIndex<T>(this T[] array, Predicate<T> match) => Array.FindIndex(array, match);

        public static int FindLastIndex<T>(this T[] array, Predicate<T> match) => Array.FindLastIndex(array, match);

        public static int IndexOf<T>(this T[] array, T value) => Array.IndexOf(array, value);

        public static int LastIndexOf<T>(this T[] array, T value) => Array.LastIndexOf(array, value);

        public static T Find<T>(this T[] array, Predicate<T> match) => Array.Find(array, match);

        public static T FindLast<T>(this T[] array, Predicate<T> match) => Array.FindLast(array, match);

        public static T[] FindAll<T>(this T[] array, Predicate<T> match) => Array.FindAll(array, match);

        public static TOutput[] ConvertAll<TInput, TOutput>(this TInput[] array, Converter<TInput, TOutput> converter) => Array.ConvertAll(array, converter);

        public static void BinarySearch<T>(this T[] array, T value) => Array.BinarySearch(array, value);

        public static void BinarySearch<T>(this T[] array, T value, IComparer<T> comparer) => Array.BinarySearch(array, value, comparer);

        public static void Clear<T>(this T[] array) => Array.Clear(array, 0, array.Length);

        public static void Fill<T>(this T[] array, T value) => Array.Fill(array, value);

        public static void ForEach<T>(this T[] array, Action<T> action) => Array.ForEach(array, action);

        public static void Reverse<T>(this T[] array) => Array.Reverse(array);

        public static void Sort<T>(this T[] array) => Array.Sort(array);

        public static void Sort<T>(this T[] array, IComparer<T> comparer) => Array.Sort(array, comparer);
    }
}