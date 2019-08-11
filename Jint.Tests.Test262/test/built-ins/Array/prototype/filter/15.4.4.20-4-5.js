// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-array.prototype.filter
es5id: 15.4.4.20-4-5
description: Array.prototype.filter throws TypeError if callbackfn is number
---*/

var arr = new Array(10);
assert.throws(TypeError, function() {
  arr.filter(5);
});
