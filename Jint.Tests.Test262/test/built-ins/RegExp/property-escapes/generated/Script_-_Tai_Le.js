// Copyright 2018 Mathias Bynens. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
author: Mathias Bynens
description: >
  Unicode property escapes for `Script=Tai_Le`
info: |
  Generated by https://github.com/mathiasbynens/unicode-property-escapes-tests
  Unicode v11.0.0
esid: sec-static-semantics-unicodematchproperty-p
features: [regexp-unicode-property-escapes]
includes: [regExpUtils.js]
---*/

const matchSymbols = buildString({
  loneCodePoints: [],
  ranges: [
    [0x001950, 0x00196D],
    [0x001970, 0x001974]
  ]
});
testPropertyEscapes(
  /^\p{Script=Tai_Le}+$/u,
  matchSymbols,
  "\\p{Script=Tai_Le}"
);
testPropertyEscapes(
  /^\p{Script=Tale}+$/u,
  matchSymbols,
  "\\p{Script=Tale}"
);
testPropertyEscapes(
  /^\p{sc=Tai_Le}+$/u,
  matchSymbols,
  "\\p{sc=Tai_Le}"
);
testPropertyEscapes(
  /^\p{sc=Tale}+$/u,
  matchSymbols,
  "\\p{sc=Tale}"
);

const nonMatchSymbols = buildString({
  loneCodePoints: [],
  ranges: [
    [0x00DC00, 0x00DFFF],
    [0x000000, 0x00194F],
    [0x00196E, 0x00196F],
    [0x001975, 0x00DBFF],
    [0x00E000, 0x10FFFF]
  ]
});
testPropertyEscapes(
  /^\P{Script=Tai_Le}+$/u,
  nonMatchSymbols,
  "\\P{Script=Tai_Le}"
);
testPropertyEscapes(
  /^\P{Script=Tale}+$/u,
  nonMatchSymbols,
  "\\P{Script=Tale}"
);
testPropertyEscapes(
  /^\P{sc=Tai_Le}+$/u,
  nonMatchSymbols,
  "\\P{sc=Tai_Le}"
);
testPropertyEscapes(
  /^\P{sc=Tale}+$/u,
  nonMatchSymbols,
  "\\P{sc=Tale}"
);
