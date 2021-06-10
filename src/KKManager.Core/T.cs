﻿using NGettext;
using System.IO;

namespace KKManager
{
    //
    // Usage:
    //		T._("Hello, World!"); // GetString
    //		T._n("You have {0} apple.", "You have {0} apples.", count, count); // GetPluralString
    //		T._p("Context", "Hello, World!"); // GetParticularString
    //		T._pn("Context", "You have {0} apple.", "You have {0} apples.", count, count); // GetParticularPluralString
    //
    //  https://habr.com/ru/post/432786/
    //  https://github.com/VitaliiTsilnyk/NGettext
    public class T
    {
        static Catalog GetCatalog()
        {
            return new Catalog("kkmanager", Path.Combine(Directory.GetCurrentDirectory(), "l10n"));
        }

        //private static readonly ICatalog _Catalog = new Catalog("Example", "./locale");
        private static readonly ICatalog _Catalog = GetCatalog();

        public static string _(string text) => _Catalog.GetString(text);

        public static string _(string text, params object[] args) => _Catalog.GetString(text, args);

#pragma warning disable IDE1006 // Стили именования
        public static string _n(string text, string pluralText, long n) => _Catalog.GetPluralString(text, pluralText, n);


        public static string _n(string text, string pluralText, long n, params object[] args) => _Catalog.GetPluralString(text, pluralText, n, args);

        public static string _p(string context, string text) => _Catalog.GetParticularString(context, text);

        public static string _p(string context, string text, params object[] args) => _Catalog.GetParticularString(context, text, args);

        public static string _pn(string context, string text, string pluralText, long n) => _Catalog.GetParticularPluralString(context, text, pluralText, n);

        public static string _pn(string context, string text, string pluralText, long n, params object[] args) => _Catalog.GetParticularPluralString(context, text, pluralText, n, args);
#pragma warning restore IDE1006 // Стили именования
    }
}
