﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Jint.Native;
using Jint.Runtime.Interop;

namespace Jint
{
    public sealed class Options
    {
        private bool _discardGlobal;
        private bool _strict;
        private bool _allowDebuggerStatement;
        private bool _allowClr;
        private readonly List<IObjectConverter> _objectConverters = new List<IObjectConverter>();
        private int _maxStatements;
        private long _memoryLimit;
        private int _maxRecursionDepth = -1;
        private TimeSpan _timeoutInterval;
        private CultureInfo _culture = CultureInfo.CurrentCulture;
        private TimeZoneInfo _localTimeZone = TimeZoneInfo.Local;
        private List<Assembly> _lookupAssemblies = new List<Assembly>();
        private Predicate<Exception> _clrExceptionsHandler;
        private IReferenceResolver _referenceResolver;

        /// <summary>
        /// When called, doesn't initialize the global scope.
        /// Can be useful in lightweight scripts for performance reason.
        /// </summary>
        public Options DiscardGlobal(bool discard = true)
        {
            _discardGlobal = discard;
            return this;
        }

        /// <summary>
        /// Run the script in strict mode.
        /// </summary>
        public Options Strict(bool strict = true)
        {
            _strict = strict;
            return this;
        }

        /// <summary>
        /// Allow the <code>debugger</code> statement to be called in a script.
        /// </summary>
        /// <remarks>
        /// Because the <code>debugger</code> statement can start the
        /// Visual Studio debugger, is it disabled by default
        /// </remarks>
        public Options AllowDebuggerStatement(bool allowDebuggerStatement = true)
        {
            _allowDebuggerStatement = allowDebuggerStatement;
            return this;
        }

        /// <summary>
        /// Allow to run the script in debug mode.
        /// </summary>
        public Options DebugMode(bool debugMode = true)
        {
            IsDebugMode = debugMode;
            return this;
        }

        /// <summary>
         /// Adds a <see cref="IObjectConverter"/> instance to convert CLR types to <see cref="JsValue"/>
        /// </summary>
        public Options AddObjectConverter(IObjectConverter objectConverter)
        {
            _objectConverters.Add(objectConverter);
            return this;
        }

        /// <summary>
        /// Allows scripts to call CLR types directly like <example>System.IO.File</example>
        /// </summary>
        public Options AllowClr(params Assembly[] assemblies)
        {
            _allowClr = true;
            _lookupAssemblies.AddRange(assemblies);
            _lookupAssemblies = _lookupAssemblies.Distinct().ToList();
            return this;
        }

        /// <summary>
        /// Exceptions thrown from CLR code are converted to JavaScript errors and
        /// can be used in at try/catch statement. By default these exceptions are bubbled
        /// to the CLR host and interrupt the script execution.
        /// </summary>
        public Options CatchClrExceptions()
        {
            CatchClrExceptions(_ => true);
            return this;
        }

        /// <summary>
        /// Exceptions that thrown from CLR code are converted to JavaScript errors and
        /// can be used in at try/catch statement. By default these exceptions are bubbled
        /// to the CLR host and interrupt the script execution.
        /// </summary>
        public Options CatchClrExceptions(Predicate<Exception> handler)
        {
            _clrExceptionsHandler = handler;
            return this;
        }

        public Options MaxStatements(int maxStatements = 0)
        {
            _maxStatements = maxStatements;
            return this;
        }
        public Options LimitMemory(long memoryLimit)
        {
            _memoryLimit = memoryLimit;
            return this;
        }

        public Options TimeoutInterval(TimeSpan timeoutInterval)
        {
            _timeoutInterval = timeoutInterval;
            return this;
        }

        /// <summary>
        /// Sets maximum allowed depth of recursion.
        /// </summary>
        /// <param name="maxRecursionDepth">
        /// The allowed depth.
        /// a) In case max depth is zero no recursion is allowed.
        /// b) In case max depth is equal to n it means that in one scope function can be called no more than n times.
        /// </param>
        /// <returns>Options instance for fluent syntax</returns>
        public Options LimitRecursion(int maxRecursionDepth = 0)
        {
            _maxRecursionDepth = maxRecursionDepth;
            return this;
        }

        public Options Culture(CultureInfo cultureInfo)
        {
            _culture = cultureInfo;
            return this;
        }

        public Options LocalTimeZone(TimeZoneInfo timeZoneInfo)
        {
            _localTimeZone = timeZoneInfo;
            return this;
        }

        public Options SetReferencesResolver(IReferenceResolver resolver)
        {
            _referenceResolver = resolver;
            return this;
        }

        internal bool _IsGlobalDiscarded => _discardGlobal;

        internal bool IsStrict => _strict;

        internal bool _IsDebuggerStatementAllowed => _allowDebuggerStatement;

        internal bool IsDebugMode { get; private set; }

        internal bool _IsClrAllowed => _allowClr;

        internal Predicate<Exception> _ClrExceptionsHandler => _clrExceptionsHandler;

        internal List<Assembly> _LookupAssemblies => _lookupAssemblies;

        internal List<IObjectConverter> _ObjectConverters => _objectConverters;

        internal long _MemoryLimit => _memoryLimit;

        internal int _MaxStatements => _maxStatements;

        internal int MaxRecursionDepth => _maxRecursionDepth;

        internal TimeSpan _TimeoutInterval => _timeoutInterval;

        internal CultureInfo _Culture => _culture;

        internal TimeZoneInfo _LocalTimeZone => _localTimeZone;

        internal IReferenceResolver  ReferenceResolver => _referenceResolver;

    }
}
