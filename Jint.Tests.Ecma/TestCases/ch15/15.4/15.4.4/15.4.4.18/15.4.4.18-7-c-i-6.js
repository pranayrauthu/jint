/// Copyright (c) 2012 Ecma International.  All rights reserved. 
/**
 * @path ch15/15.4/15.4.4/15.4.4.18/15.4.4.18-7-c-i-6.js
 * @description Array.prototype.forEach - element to be retrieved is own data property that overrides an inherited accessor property on an Array
 */


function testcase() {

        var testResult = false;

        function callbackfn(val, idx, obj) {
            if (idx === 0) {
                testResult = (val === 11);
            }
        }

        try {
            Object.defineProperty(Array.prototype, "0", {
                get: function () {
                    return 9;
                },
                configurable: true
            });

            [11].forEach(callbackfn);

            return testResult;
        } finally {
            delete Array.prototype[0];
        }
    }
runTestCase(testcase);
