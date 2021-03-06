/// Copyright (c) 2012 Ecma International.  All rights reserved. 
/**
 * @path ch15/15.4/15.4.4/15.4.4.18/15.4.4.18-7-c-ii-21.js
 * @description Array.prototype.forEach - callbackfn called with correct parameters (kValue is correct)
 */


function testcase() {

        var resultOne = false;
        var resultTwo = false;

        function callbackfn(val, idx, obj) {
            if (idx === 0) {
                resultOne = (val === 11);
            }

            if (idx === 1) {
                resultTwo = (val === 12);
            }

        }

        var obj = { 0: 11, 1: 12, length: 2 };

        Array.prototype.forEach.call(obj, callbackfn);
        return resultOne && resultTwo;
    }
runTestCase(testcase);
