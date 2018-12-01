﻿using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Jint.Native.Array;
using Jint.Native.Function;
using Jint.Native.Object;
using Jint.Native.RegExp;
using Jint.Runtime;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Interop;

namespace Jint.Native.String
{
    /// <summary>
    /// http://www.ecma-international.org/ecma-262/5.1/#sec-15.5.4
    /// </summary>
    public sealed class StringPrototype : StringInstance
    {
        private StringPrototype(Engine engine)
            : base(engine)
        {
        }

        public static StringPrototype CreatePrototypeObject(Engine engine, StringConstructor stringConstructor)
        {
            var obj = new StringPrototype(engine);
            obj.Prototype = engine.Object.PrototypeObject;
            obj.PrimitiveValue = JsString.Empty;
            obj.Extensible = true;
            obj.SetOwnProperty("length", new PropertyDescriptor(0, PropertyFlag.AllForbidden));
            obj.SetOwnProperty("constructor", new PropertyDescriptor(stringConstructor, PropertyFlag.NonEnumerable));

            return obj;
        }

        public void Configure()
        {
            FastAddProperty("toString", new ClrFunctionInstance(Engine, "toString", ToStringString), true, false, true);
            FastAddProperty("valueOf", new ClrFunctionInstance(Engine, "valueOF", ValueOf), true, false, true);
            FastAddProperty("charAt", new ClrFunctionInstance(Engine, "charAt", CharAt, 1), true, false, true);
            FastAddProperty("charCodeAt", new ClrFunctionInstance(Engine, "charCodeAt", CharCodeAt, 1), true, false, true);
            FastAddProperty("concat", new ClrFunctionInstance(Engine, "concat", Concat, 1), true, false, true);
            FastAddProperty("indexOf", new ClrFunctionInstance(Engine, "indexOf", IndexOf, 1), true, false, true);
            FastAddProperty("startsWith", new ClrFunctionInstance(Engine, "startsWith", StartsWith, 1), true, false, true);
            FastAddProperty("lastIndexOf", new ClrFunctionInstance(Engine, "lastIndexOf", LastIndexOf, 1), true, false, true);
            FastAddProperty("localeCompare", new ClrFunctionInstance(Engine, "localeCompare", LocaleCompare, 1), true, false, true);
            FastAddProperty("match", new ClrFunctionInstance(Engine, "match", Match, 1), true, false, true);
            FastAddProperty("replace", new ClrFunctionInstance(Engine, "replace", Replace, 2), true, false, true);
            FastAddProperty("search", new ClrFunctionInstance(Engine, "search", Search, 1), true, false, true);
            FastAddProperty("slice", new ClrFunctionInstance(Engine, "slice", Slice, 2), true, false, true);
            FastAddProperty("split", new ClrFunctionInstance(Engine, "split", Split, 2), true, false, true);
            FastAddProperty("substr", new ClrFunctionInstance(Engine, "substr", Substr, 2), true, false, true);
            FastAddProperty("substring", new ClrFunctionInstance(Engine, "substring", Substring, 2), true, false, true);
            FastAddProperty("toLowerCase", new ClrFunctionInstance(Engine, "toLowerCase", ToLowerCase), true, false, true);
            FastAddProperty("toLocaleLowerCase", new ClrFunctionInstance(Engine, "toLocaleLowerCase", ToLocaleLowerCase), true, false, true);
            FastAddProperty("toUpperCase", new ClrFunctionInstance(Engine, "toUpperCase", ToUpperCase), true, false, true);
            FastAddProperty("toLocaleUpperCase", new ClrFunctionInstance(Engine, "toLocaleUpperCase", ToLocaleUpperCase), true, false, true);
            FastAddProperty("trim", new ClrFunctionInstance(Engine, "trim", Trim), true, false, true);
            FastAddProperty("padStart", new ClrFunctionInstance(Engine, "padStart", PadStart), true, false, true);
            FastAddProperty("padEnd", new ClrFunctionInstance(Engine, "padEnd", PadEnd), true, false, true);
        }

        private JsValue ToStringString(JsValue thisObj, JsValue[] arguments)
        {
            var s = TypeConverter.ToObject(Engine, thisObj) as StringInstance;
            if (ReferenceEquals(s, null))
            {
                ExceptionHelper.ThrowTypeError(Engine);
            }

            return s.PrimitiveValue;
        }

        // http://msdn.microsoft.com/en-us/library/system.char.iswhitespace(v=vs.110).aspx
        // http://en.wikipedia.org/wiki/Byte_order_mark
        const char BOM_CHAR = '\uFEFF';
        const char MONGOLIAN_VOWEL_SEPARATOR = '\u180E';

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsWhiteSpaceEx(char c, bool acceptMongolianVowelSeparator = true)
        {
            return
                char.IsWhiteSpace(c) ||
                c == BOM_CHAR ||
                // In .NET 4.6 this was removed from WS based on Unicode 6.3 changes
                (acceptMongolianVowelSeparator && c == MONGOLIAN_VOWEL_SEPARATOR);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string TrimEndEx(string s)
        {
            if (s.Length == 0)
                return string.Empty;

            if (!IsWhiteSpaceEx(s[s.Length - 1]))
                return s;

            return TrimEnd(s);
        }

        private static string TrimEnd(string s)
        {
            var i = s.Length - 1;
            while (i >= 0)
            {
                if (IsWhiteSpaceEx(s[i]))
                    i--;
                else
                    break;
            }

            return i >= 0 ? s.Substring(0, i + 1) : string.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string TrimStartEx(string s, bool acceptMongolianVowelSeparator = true)
        {
            if (s.Length == 0)
                return string.Empty;

            if (!IsWhiteSpaceEx(s[0], acceptMongolianVowelSeparator))
                return s;

            return TrimStart(s);
        }

        private static string TrimStart(string s)
        {
            var i = 0;
            while (i < s.Length)
            {
                if (IsWhiteSpaceEx(s[i]))
                    i++;
                else
                    break;
            }

            return i >= s.Length ? string.Empty : s.Substring(i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string TrimEx(string s, bool acceptMongolianVowelSeparator = true)
        {
            return TrimEndEx(TrimStartEx(s, acceptMongolianVowelSeparator));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsValue Trim(JsValue thisObj, JsValue[] arguments)
        {
            TypeConverter.CheckObjectCoercible(Engine, thisObj);
            var s = TypeConverter.ToString(thisObj);
            return TrimEx(s);
        }

        private static JsValue ToLocaleUpperCase(JsValue thisObj, JsValue[] arguments)
        {
            var s = TypeConverter.ToString(thisObj);
            return s.ToUpper();
        }

        private static JsValue ToUpperCase(JsValue thisObj, JsValue[] arguments)
        {
            var s = TypeConverter.ToString(thisObj);
            return s.ToUpperInvariant();
        }

        private static JsValue ToLocaleLowerCase(JsValue thisObj, JsValue[] arguments)
        {
            var s = TypeConverter.ToString(thisObj);
            return s.ToLower();
        }

        private static JsValue ToLowerCase(JsValue thisObj, JsValue[] arguments)
        {
            var s = TypeConverter.ToString(thisObj);
            return s.ToLowerInvariant();
        }

        private static int ToIntegerSupportInfinity(JsValue numberVal)
        {
            var doubleVal = TypeConverter.ToInteger(numberVal);
            int intVal;
            if (double.IsPositiveInfinity(doubleVal))
                intVal = int.MaxValue;
            else if (double.IsNegativeInfinity(doubleVal))
                intVal = int.MinValue;
            else
                intVal = (int) doubleVal;
            return intVal;
        }

        private JsValue Substring(JsValue thisObj, JsValue[] arguments)
        {
            TypeConverter.CheckObjectCoercible(Engine, thisObj);

            var s = TypeConverter.ToString(thisObj);
            var start = TypeConverter.ToNumber(arguments.At(0));
            var end = TypeConverter.ToNumber(arguments.At(1));

            if (double.IsNaN(start) || start < 0)
            {
                start = 0;
            }

            if (double.IsNaN(end) || end < 0)
            {
                end = 0;
            }

            var len = s.Length;
            var intStart = ToIntegerSupportInfinity(start);

            var intEnd = arguments.At(1).IsUndefined() ? len : ToIntegerSupportInfinity(end);
            var finalStart = System.Math.Min(len, System.Math.Max(intStart, 0));
            var finalEnd = System.Math.Min(len, System.Math.Max(intEnd, 0));
            // Swap value if finalStart < finalEnd
            var from = System.Math.Min(finalStart, finalEnd);
            var to = System.Math.Max(finalStart, finalEnd);
            var length = to - from;

            if (length == 0)
            {
                return string.Empty;
            }

            if (length == 1)
            {
                return TypeConverter.ToString(s[from]);
            }

            return s.Substring(from, length);
        }

        private static JsValue Substr(JsValue thisObj, JsValue[] arguments)
        {
            var s = TypeConverter.ToString(thisObj);
            var start = TypeConverter.ToInteger(arguments.At(0));
            var length = arguments.At(1).IsUndefined()
                ? double.PositiveInfinity
                : TypeConverter.ToInteger(arguments.At(1));

            start = start >= 0 ? start : System.Math.Max(s.Length + start, 0);
            length = System.Math.Min(System.Math.Max(length, 0), s.Length - start);
            if (length <= 0)
            {
                return "";
            }

            var startIndex = TypeConverter.ToInt32(start);
            var l = TypeConverter.ToInt32(length);
            if (l == 1)
            {
                return TypeConverter.ToString(s[startIndex]);
            }
            return s.Substring(startIndex, l);
        }

        private JsValue Split(JsValue thisObj, JsValue[] arguments)
        {
            TypeConverter.CheckObjectCoercible(Engine, thisObj);
            var s = TypeConverter.ToString(thisObj);

            var separator = arguments.At(0);

            // Coerce into a number, true will become 1
            var l = arguments.At(1);
            var limit = l.IsUndefined() ? uint.MaxValue : TypeConverter.ToUint32(l);
            var len = s.Length;

            if (limit == 0)
            {
                return Engine.Array.Construct(Arguments.Empty);
            }

            if (separator.IsNull())
            {
                separator = Native.Null.Text;
            }
            else if (separator.IsUndefined())
            {
                var jsValues = _engine._jsValueArrayPool.RentArray(1);
                jsValues[0] = s;
                var arrayInstance = (ArrayInstance)Engine.Array.Construct(jsValues);
                _engine._jsValueArrayPool.ReturnArray(jsValues);
                return arrayInstance;
            }
            else
            {
                if (!separator.IsRegExp())
                {
                    separator = TypeConverter.ToString(separator); // Coerce into a string, for an object call toString()
                }
            }

            var rx = TypeConverter.ToObject(Engine, separator) as RegExpInstance;

            const string regExpForMatchingAllCharactere = "(?:)";

            if (!ReferenceEquals(rx, null) &&
                rx.Source != regExpForMatchingAllCharactere // We need pattern to be defined -> for s.split(new RegExp)
                )
            {
                var a = (ArrayInstance) Engine.Array.Construct(Arguments.Empty);
                var match = rx.Value.Match(s, 0);

                if (!match.Success) // No match at all return the string in an array
                {
                    a.SetIndexValue(0, s, updateLength: true);
                    return a;
                }

                int lastIndex = 0;
                uint index = 0;
                while (match.Success && index < limit)
                {
                    if (match.Length == 0 && (match.Index == 0 || match.Index == len || match.Index == lastIndex))
                    {
                        match = match.NextMatch();
                        continue;
                    }

                    // Add the match results to the array.
                    a.SetIndexValue(index++, s.Substring(lastIndex, match.Index - lastIndex), updateLength: true);

                    if (index >= limit)
                    {
                        return a;
                    }

                    lastIndex = match.Index + match.Length;
                    for (int i = 1; i < match.Groups.Count; i++)
                    {
                        var group = match.Groups[i];
                        var item = Undefined;
                        if (group.Captures.Count > 0)
                        {
                            item = match.Groups[i].Value;
                        }

                        a.SetIndexValue(index++, item, updateLength: true);

                        if (index >= limit)
                        {
                            return a;
                        }
                    }

                    match = match.NextMatch();
                    if (!match.Success) // Add the last part of the split
                    {
                        a.SetIndexValue(index++, s.Substring(lastIndex), updateLength: true);
                    }
                }

                return a;
            }
            else
            {
                var segments = StringExecutionContext.Current.SplitSegmentList;
                segments.Clear();
                var sep = TypeConverter.ToString(separator);

                if (sep == string.Empty || (!ReferenceEquals(rx, null) && rx.Source == regExpForMatchingAllCharactere)) // for s.split(new RegExp)
                {
                    if (s.Length > segments.Capacity)
                    {
                        segments.Capacity = s.Length;
                    }

                    for (var i = 0; i < s.Length; i++)
                    {
                        segments.Add(TypeConverter.ToString(s[i]));
                    }
                }
                else
                {
                    var array = StringExecutionContext.Current.SplitArray1;
                    array[0] = sep;
                    segments.AddRange(s.Split(array, StringSplitOptions.None));
                }

                var length = (uint) System.Math.Min(segments.Count, limit);
                var a = Engine.Array.ConstructFast(length);
                for (int i = 0; i < length; i++)
                {
                    a.SetIndexValue((uint) i, segments[i], updateLength: false);
                }
                a.SetLength(length);
                return a;
            }
        }

        private JsValue Slice(JsValue thisObj, JsValue[] arguments)
        {
            TypeConverter.CheckObjectCoercible(Engine, thisObj);

            var start = TypeConverter.ToNumber(arguments.At(0));
            if (double.IsNegativeInfinity(start))
            {
                start = 0;
            }
            if (double.IsPositiveInfinity(start))
            {
                return string.Empty;
            }

            var s = TypeConverter.ToString(thisObj);
            var end = TypeConverter.ToNumber(arguments.At(1));
            if (double.IsPositiveInfinity(end))
            {
                end = s.Length;
            }

            var len = s.Length;
            var intStart = (int)TypeConverter.ToInteger(start);
            var intEnd = arguments.At(1).IsUndefined() ? len : (int)TypeConverter.ToInteger(end);
            var from = intStart < 0 ? System.Math.Max(len + intStart, 0) : System.Math.Min(intStart, len);
            var to = intEnd < 0 ? System.Math.Max(len + intEnd, 0) : System.Math.Min(intEnd, len);
            var span = System.Math.Max(to - from, 0);

            if (span == 0)
            {
                return string.Empty;
            }

            if (span == 1)
            {
                return TypeConverter.ToString(s[from]);
            }

            return s.Substring(from, span);
        }

        private JsValue Search(JsValue thisObj, JsValue[] arguments)
        {
            TypeConverter.CheckObjectCoercible(Engine, thisObj);

            var s = TypeConverter.ToString(thisObj);

            var regex = arguments.At(0);

            if (regex.IsUndefined())
            {
                regex = string.Empty;
            }
            else if (regex.IsNull())
            {
                regex = Native.Null.Text;
            }

            var rx = TypeConverter.ToObject(Engine, regex) as RegExpInstance ?? (RegExpInstance)Engine.RegExp.Construct(new[] { regex });
            var match = rx.Value.Match(s);
            if (!match.Success)
            {
                return -1;
            }

            return match.Index;
        }

        private JsValue Replace(JsValue thisObj, JsValue[] arguments)
        {
            TypeConverter.CheckObjectCoercible(Engine, thisObj);

            var thisString = TypeConverter.ToString(thisObj);
            var searchValue = arguments.At(0);
            var replaceValue = arguments.At(1);

            // If the second parameter is not a function we create one
            var replaceFunction = replaceValue.TryCast<FunctionInstance>();
            if (ReferenceEquals(replaceFunction, null))
            {
                replaceFunction = new ClrFunctionInstance(Engine, "anonymous", (self, args) =>
                {
                    var replaceString = TypeConverter.ToString(replaceValue);
                    var matchValue = TypeConverter.ToString(args.At(0));
                    var matchIndex = (int)TypeConverter.ToInteger(args.At(args.Length - 2));

                    // Check if the replacement string contains any patterns.
                    bool replaceTextContainsPattern = replaceString.IndexOf('$') >= 0;

                    // If there is no pattern, replace the pattern as is.
                    if (replaceTextContainsPattern == false)
                        return replaceString;

                    // Patterns
                    // $$	Inserts a "$".
                    // $&	Inserts the matched substring.
                    // $`	Inserts the portion of the string that precedes the matched substring.
                    // $'	Inserts the portion of the string that follows the matched substring.
                    // $n or $nn	Where n or nn are decimal digits, inserts the nth parenthesized submatch string, provided the first argument was a RegExp object.
                    var replacementBuilder = StringExecutionContext.Current.GetStringBuilder(0);
                    replacementBuilder.Clear();
                    for (int i = 0; i < replaceString.Length; i++)
                    {
                        char c = replaceString[i];
                        if (c == '$' && i < replaceString.Length - 1)
                        {
                            c = replaceString[++i];
                            if (c == '$')
                                replacementBuilder.Append('$');
                            else if (c == '&')
                                replacementBuilder.Append(matchValue);
                            else if (c == '`')
                                replacementBuilder.Append(thisString.Substring(0, matchIndex));
                            else if (c == '\'')
                                replacementBuilder.Append(thisString.Substring(matchIndex + matchValue.Length));
                            else if (c >= '0' && c <= '9')
                            {
                                int matchNumber1 = c - '0';

                                // The match number can be one or two digits long.
                                int matchNumber2 = 0;
                                if (i < replaceString.Length - 1 && replaceString[i + 1] >= '0' && replaceString[i + 1] <= '9')
                                    matchNumber2 = matchNumber1 * 10 + (replaceString[i + 1] - '0');

                                // Try the two digit capture first.
                                if (matchNumber2 > 0 && matchNumber2 < args.Length - 2)
                                {
                                    // Two digit capture replacement.
                                    replacementBuilder.Append(TypeConverter.ToString(args[matchNumber2]));
                                    i++;
                                }
                                else if (matchNumber1 > 0 && matchNumber1 < args.Length - 2)
                                {
                                    // Single digit capture replacement.
                                    replacementBuilder.Append(TypeConverter.ToString(args[matchNumber1]));
                                }
                                else
                                {
                                    // Capture does not exist.
                                    replacementBuilder.Append('$');
                                    i--;
                                }
                            }
                            else
                            {
                                // Unknown replacement pattern.
                                replacementBuilder.Append('$');
                                replacementBuilder.Append(c);
                            }
                        }
                        else
                            replacementBuilder.Append(c);
                    }

                    return replacementBuilder.ToString();
                });
            }

            // searchValue is a regular expression

            if (searchValue.IsNull())
            {
                searchValue = Native.Null.Text;
            }
            if (searchValue.IsUndefined())
            {
                searchValue = Native.Undefined.Text;
            }

            var rx = TypeConverter.ToObject(Engine, searchValue) as RegExpInstance;
            if (!ReferenceEquals(rx, null))
            {
                // Replace the input string with replaceText, recording the last match found.
                string result = rx.Value.Replace(thisString, match =>
                {
                    var args = new JsValue[match.Groups.Count + 2];
                    for (var k = 0; k < match.Groups.Count; k++)
                    {
                        var group = match.Groups[k];
                        args[k] = @group.Value;
                    }

                    args[match.Groups.Count] = match.Index;
                    args[match.Groups.Count + 1] = thisString;

                    var v = TypeConverter.ToString(replaceFunction.Call(Undefined, args));
                    return v;
                }, rx.Global == true ? -1 : 1);

                // Set the deprecated RegExp properties if at least one match was found.
                //if (lastMatch != null)
                //    this.Engine.RegExp.SetDeprecatedProperties(input, lastMatch);

                return result;
            }

            // searchValue is a string
            else
            {
                var substr = TypeConverter.ToString(searchValue);

                // Find the first occurrance of substr.
                int start = thisString.IndexOf(substr, StringComparison.Ordinal);
                if (start == -1)
                    return thisString;
                int end = start + substr.Length;

                var args = _engine._jsValueArrayPool.RentArray(3);
                args[0] = substr;
                args[1] = start;
                args[2] = thisString;

                var replaceString = TypeConverter.ToString(replaceFunction.Call(Undefined, args));

                _engine._jsValueArrayPool.ReturnArray(args);

                // Replace only the first match.
                var result = StringExecutionContext.Current.GetStringBuilder(thisString.Length + (substr.Length - substr.Length));
                result.Clear();
                result.Append(thisString, 0, start);
                result.Append(replaceString);
                result.Append(thisString, end, thisString.Length - end);
                return result.ToString();
            }
        }

        private JsValue Match(JsValue thisObj, JsValue[] arguments)
        {
            TypeConverter.CheckObjectCoercible(Engine, thisObj);

            var s = TypeConverter.ToString(thisObj);

            var regex = arguments.At(0);
            var rx = regex.TryCast<RegExpInstance>();

            rx = rx ?? (RegExpInstance) Engine.RegExp.Construct(new[] {regex});

            var global = ((JsBoolean) rx.Get("global"))._value;
            if (!global)
            {
                return Engine.RegExp.PrototypeObject.Exec(rx, Arguments.From(s));
            }
            else
            {
                rx.Put("lastIndex", 0, false);
                var a = (ArrayInstance) Engine.Array.Construct(Arguments.Empty);
                double previousLastIndex = 0;
                uint n = 0;
                var lastMatch = true;
                while (lastMatch)
                {
                    var result = Engine.RegExp.PrototypeObject.Exec(rx, Arguments.From(s)).TryCast<ObjectInstance>();
                    if (ReferenceEquals(result, null))
                    {
                        lastMatch = false;
                    }
                    else
                    {
                        var thisIndex = ((JsNumber) rx.Get("lastIndex"))._value;
                        if (thisIndex == previousLastIndex)
                        {
                            rx.Put("lastIndex", thisIndex + 1, false);
                            previousLastIndex = thisIndex;
                        }

                        var matchStr = result.Get("0");
                        a.SetIndexValue(n, matchStr, updateLength: false);
                        n++;
                    }
                }
                if (n == 0)
                {
                    return Null;
                }
                a.SetLength(n);
                return a;
            }

        }

        private JsValue LocaleCompare(JsValue thisObj, JsValue[] arguments)
        {
            TypeConverter.CheckObjectCoercible(Engine, thisObj);

            var s = TypeConverter.ToString(thisObj);
            var that = TypeConverter.ToString(arguments.At(0));

            return string.CompareOrdinal(s, that);
        }

        private JsValue LastIndexOf(JsValue thisObj, JsValue[] arguments)
        {
            TypeConverter.CheckObjectCoercible(Engine, thisObj);

            var s = TypeConverter.ToString(thisObj);
            var searchStr = TypeConverter.ToString(arguments.At(0));
            double numPos = double.NaN;
            if (arguments.Length > 1 && !arguments[1].IsUndefined())
            {
                numPos = TypeConverter.ToNumber(arguments[1]);
            }

            var pos = double.IsNaN(numPos) ? double.PositiveInfinity : TypeConverter.ToInteger(numPos);

            var len = s.Length;
            var start = (int)System.Math.Min(System.Math.Max(pos, 0), len);
            var searchLen = searchStr.Length;

            var i = start;
            bool found;

            do
            {
                found = true;
                var j = 0;

                while (found && j < searchLen)
                {
                    if ((i + searchLen > len) || (s[i + j] != searchStr[j]))
                    {
                        found = false;
                    }
                    else
                    {
                        j++;
                    }
                }
                if (!found)
                {
                    i--;
                }

            } while (!found && i >= 0);

            return i;
        }

        private JsValue IndexOf(JsValue thisObj, JsValue[] arguments)
        {
            TypeConverter.CheckObjectCoercible(Engine, thisObj);

            var s = TypeConverter.ToString(thisObj);
            var searchStr = TypeConverter.ToString(arguments.At(0));
            double pos = 0;
            if (arguments.Length > 1 && !arguments[1].IsUndefined())
            {
                pos = TypeConverter.ToInteger(arguments[1]);
            }

            if (pos >= s.Length)
            {
                return -1;
            }

            if (pos < 0)
            {
                pos = 0;
            }

            return s.IndexOf(searchStr, (int) pos, StringComparison.Ordinal);
        }

        private JsValue Concat(JsValue thisObj, JsValue[] arguments)
        {
            TypeConverter.CheckObjectCoercible(Engine, thisObj);

            // try to hint capacity if possible
            int capacity = 0;
            for (int i = 0; i < arguments.Length; ++i)
            {
                if (arguments[i].Type == Types.String)
                {
                    capacity += arguments[i].AsStringWithoutTypeCheck().Length;
                }
            }

            var value = TypeConverter.ToString(thisObj);
            capacity += value.Length;
            if (!(thisObj is JsString jsString))
            {
                jsString = new JsString.ConcatenatedString(value, capacity);
            }
            else
            {
                jsString = jsString.EnsureCapacity(capacity);
            }

            for (int i = 0; i < arguments.Length; i++)
            {
                jsString = jsString.Append(arguments[i]);
            }

            return jsString;
        }

        private JsValue CharCodeAt(JsValue thisObj, JsValue[] arguments)
        {
            TypeConverter.CheckObjectCoercible(Engine, thisObj);

            JsValue pos = arguments.Length > 0 ? arguments[0] : 0;
            var s = TypeConverter.ToString(thisObj);
            var position = (int)TypeConverter.ToInteger(pos);
            if (position < 0 || position >= s.Length)
            {
                return double.NaN;
            }
            return (double) s[position];
        }

        private JsValue CharAt(JsValue thisObj, JsValue[] arguments)
        {
            TypeConverter.CheckObjectCoercible(Engine, thisObj);
            var s = TypeConverter.ToString(thisObj);
            var position = TypeConverter.ToInteger(arguments.At(0));
            var size = s.Length;
            if (position >= size || position < 0)
            {
                return "";
            }
            return TypeConverter.ToString(s[(int) position]);

        }

        private JsValue ValueOf(JsValue thisObj, JsValue[] arguments)
        {
            if (thisObj is StringInstance si)
            {
                return si.PrimitiveValue;
            }

            if (thisObj is JsString)
            {
                return thisObj;
            }

            return ExceptionHelper.ThrowTypeError<JsValue>(Engine);
        }

        /// <summary>
        /// https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/String/padStart
        /// </summary>
        /// <param name="thisObj">The original string object</param>
        /// <param name="arguments">
        ///     argument[0] is the target length of the output string
        ///     argument[1] is the string to pad with
        /// </param>
        /// <returns></returns>
        private JsValue PadStart(JsValue thisObj, JsValue[] arguments)
        {
            return Pad(thisObj, arguments, true);
        }

        /// <summary>
        /// https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/String/padEnd
        /// </summary>
        /// <param name="thisObj">The original string object</param>
        /// <param name="arguments">
        ///     argument[0] is the target length of the output string
        ///     argument[1] is the string to pad with
        /// </param>
        /// <returns></returns>
        private JsValue PadEnd(JsValue thisObj, JsValue[] arguments)
        {
            return Pad(thisObj, arguments, false);
        }

        private JsValue Pad(JsValue thisObj, JsValue[] arguments, bool padStart)
        {
            TypeConverter.CheckObjectCoercible(Engine, thisObj);
            var targetLength = TypeConverter.ToInt32(arguments.At(0));
            var padString = TypeConverter.ToString(arguments.At(1, " "));

            var s = TypeConverter.ToString(thisObj);
            if (s.Length > targetLength)
            {
                return s;
            }

            targetLength = targetLength - s.Length;
            if (targetLength > padString.Length)
            {
                padString = string.Join("", Enumerable.Repeat(padString, (targetLength / padString.Length) + 1));
            }

            return padStart ? $"{padString.Substring(0, targetLength)}{s}" : $"{s}{padString.Substring(0, targetLength)}";
        }

        /// <summary>
        /// https://www.ecma-international.org/ecma-262/6.0/#sec-string.prototype.startswith
        /// </summary>
        /// <param name="thisObj"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        private JsValue StartsWith(JsValue thisObj, JsValue[] arguments)
        {
            TypeConverter.CheckObjectCoercible(Engine, thisObj);

            var s = TypeConverter.ToString(thisObj);

            var searchString = arguments.At(0);
            if (ReferenceEquals(searchString, Null))
            {
                searchString = Native.Null.Text;
            }
            else
            {
                if (searchString.IsRegExp())
                {
                    ExceptionHelper.ThrowTypeError(Engine);
                }
            }

            var searchStr = TypeConverter.ToString(searchString);

            var pos = TypeConverter.ToInt32(arguments.At(1));

            var len = s.Length;
            var start = System.Math.Min(System.Math.Max(pos, 0), len);
            var searchLength = searchStr.Length;
            if (searchLength + start > len)
            {
                return false;
            }

            for (var i = 0; i < searchLength; i++)
            {
                if (s[start + i] != searchStr[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
