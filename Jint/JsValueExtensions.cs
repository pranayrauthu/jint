﻿using System.Runtime.CompilerServices;
using Jint.Native;
using Jint.Runtime;

namespace Jint
{
    public static class JsValueExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AsBoolean(this JsValue value)
        {
            if (value._type != Types.Boolean)
            {
                ExceptionHelper.ThrowArgumentException("The value is not a boolean");
            }

            return ((JsBoolean) value)._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double AsNumber(this JsValue value)
        {
            if (value._type != Types.Number)
            {
                ExceptionHelper.ThrowArgumentException("The value is not a number");
            }

            return ((JsNumber) value)._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string AsString(this JsValue value)
        {
            if (value._type != Types.String)
            {
                ExceptionHelper.ThrowArgumentException("The value is not a string");
            }

            return AsStringWithoutTypeCheck(value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string AsStringWithoutTypeCheck(this JsValue value)
        {
            return value.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string AsSymbol(this JsValue value)
        {
            if (value._type != Types.Symbol)
            {
                ExceptionHelper.ThrowArgumentException("The value is not a symbol");
            }

            return ((JsSymbol) value)._value;
        }
    }
}