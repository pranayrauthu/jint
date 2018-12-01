﻿using Esprima;
using Jint.Runtime;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Environments;

namespace Jint.Native.Function
{
    public sealed class EvalFunctionInstance : FunctionInstance
    {
        private static readonly ParserOptions ParserOptions = new ParserOptions { AdaptRegexp = true, Tolerant = false };

        public EvalFunctionInstance(Engine engine, string[] parameters, LexicalEnvironment scope, bool strict) 
            : base(engine, "eval", parameters, scope, strict)
        {
            Prototype = Engine.Function.PrototypeObject;
            SetOwnProperty("length", new PropertyDescriptor(1, PropertyFlag.AllForbidden));
        }

        public override JsValue Call(JsValue thisObject, JsValue[] arguments)
        {
            return Call(thisObject, arguments, false);
        }

        public JsValue Call(JsValue thisObject, JsValue[] arguments, bool directCall)
        {
            var arg = arguments.At(0);
            if (arg.Type != Types.String)
            {
                return arg;
            }

            var code = TypeConverter.ToString(arg);

            try
            {
                var parser = new JavaScriptParser(code, ParserOptions);
                var program = parser.ParseProgram(StrictModeScope.IsStrictModeCode);
                using (new StrictModeScope(program.Strict))
                {
                    using (new EvalCodeScope())
                    {
                        LexicalEnvironment strictVarEnv = null;

                        try
                        {
                            if (!directCall)
                            {
                                Engine.EnterExecutionContext(Engine.GlobalEnvironment, Engine.GlobalEnvironment, Engine.Global);
                            }

                            var lexicalEnvironment = _engine.ExecutionContext.LexicalEnvironment;
                            if (StrictModeScope.IsStrictModeCode)
                            {
                                strictVarEnv = LexicalEnvironment.NewDeclarativeEnvironment(Engine, lexicalEnvironment);
                                Engine.EnterExecutionContext(strictVarEnv, strictVarEnv, Engine.ExecutionContext.ThisBinding);
                            }

                            bool argumentInstanceRented = Engine.DeclarationBindingInstantiation(
                                DeclarationBindingType.EvalCode,
                                program.HoistingScope.FunctionDeclarations,
                                program.HoistingScope.VariableDeclarations,
                                this, 
                                arguments);

                            var result = _engine.ExecuteStatement(program);
                            var value = result.GetValueOrDefault();

                            if (argumentInstanceRented)
                            {
                                lexicalEnvironment?._record?.FunctionWasCalled();
                                _engine.ExecutionContext.VariableEnvironment?._record?.FunctionWasCalled();
                            }

                            if (result.Type == CompletionType.Throw)
                            {
                                var ex = new JavaScriptException(value).SetCallstack(_engine, result.Location);
                                throw ex;
                            }
                            else
                            {
                                return value;
                            }
                        }
                        finally
                        {
                            if (strictVarEnv != null)
                            {
                                Engine.LeaveExecutionContext();
                            }

                            if (!directCall)
                            {
                                Engine.LeaveExecutionContext();
                            }
                        }
                    }
                }
            }
            catch (ParserException e)
            {
                if (e.Description == Messages.InvalidLHSInAssignment)
                {
                    ExceptionHelper.ThrowReferenceError(_engine);
                }

                ExceptionHelper.ThrowSyntaxError(_engine);
                return null;
            }
        }
    }
}
