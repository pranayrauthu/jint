// Copyright 2018 Mathias Bynens. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
author: Mathias Bynens
description: >
  Unicode property escapes for `Script=Hangul`
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
    [0x001100, 0x0011FF],
    [0x00302E, 0x00302F],
    [0x003131, 0x00318E],
    [0x003200, 0x00321E],
    [0x003260, 0x00327E],
    [0x00A960, 0x00A97C],
    [0x00AC00, 0x00D7A3],
    [0x00D7B0, 0x00D7C6],
    [0x00D7CB, 0x00D7FB],
    [0x00FFA0, 0x00FFBE],
    [0x00FFC2, 0x00FFC7],
    [0x00FFCA, 0x00FFCF],
    [0x00FFD2, 0x00FFD7],
    [0x00FFDA, 0x00FFDC]
  ]
});
testPropertyEscapes(
  /^\p{Script=Hangul}+$/u,
  matchSymbols,
  "\\p{Script=Hangul}"
);
testPropertyEscapes(
  /^\p{Script=Hang}+$/u,
  matchSymbols,
  "\\p{Script=Hang}"
);
testPropertyEscapes(
  /^\p{sc=Hangul}+$/u,
  matchSymbols,
  "\\p{sc=Hangul}"
);
testPropertyEscapes(
  /^\p{sc=Hang}+$/u,
  matchSymbols,
  "\\p{sc=Hang}"
);

const nonMatchSymbols = buildString({
  loneCodePoints: [],
  ranges: [
    [0x00DC00, 0x00DFFF],
    [0x000000, 0x0010FF],
    [0x001200, 0x00302D],
    [0x003030, 0x003130],
    [0x00318F, 0x0031FF],
    [0x00321F, 0x00325F],
    [0x00327F, 0x00A95F],
    [0x00A97D, 0x00ABFF],
    [0x00D7A4, 0x00D7AF],
    [0x00D7C7, 0x00D7CA],
    [0x00D7FC, 0x00DBFF],
    [0x00E000, 0x00FF9F],
    [0x00FFBF, 0x00FFC1],
    [0x00FFC8, 0x00FFC9],
    [0x00FFD0, 0x00FFD1],
    [0x00FFD8, 0x00FFD9],
    [0x00FFDD, 0x10FFFF]
  ]
});
testPropertyEscapes(
  /^\P{Script=Hangul}+$/u,
  nonMatchSymbols,
  "\\P{Script=Hangul}"
);
testPropertyEscapes(
  /^\P{Script=Hang}+$/u,
  nonMatchSymbols,
  "\\P{Script=Hang}"
);
testPropertyEscapes(
  /^\P{sc=Hangul}+$/u,
  nonMatchSymbols,
  "\\P{sc=Hangul}"
);
testPropertyEscapes(
  /^\P{sc=Hang}+$/u,
  nonMatchSymbols,
  "\\P{sc=Hang}"
);
