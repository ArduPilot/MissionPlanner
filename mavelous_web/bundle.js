(function e(t,n,r){function s(o,u){if(!n[o]){if(!t[o]){var a=typeof require=="function"&&require;if(!u&&a)return a(o,!0);if(i)return i(o,!0);var f=new Error("Cannot find module '"+o+"'");throw f.code="MODULE_NOT_FOUND",f}var l=n[o]={exports:{}};t[o][0].call(l.exports,function(e){var n=t[o][1][e];return s(n?n:e)},l,l.exports,e,t,n,r)}return n[o].exports}var i=typeof require=="function"&&require;for(var o=0;o<r.length;o++)s(r[o]);return s})({1:[function(require,module,exports){
'use strict'

exports.byteLength = byteLength
exports.toByteArray = toByteArray
exports.fromByteArray = fromByteArray

var lookup = []
var revLookup = []
var Arr = typeof Uint8Array !== 'undefined' ? Uint8Array : Array

var code = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/'
for (var i = 0, len = code.length; i < len; ++i) {
  lookup[i] = code[i]
  revLookup[code.charCodeAt(i)] = i
}

revLookup['-'.charCodeAt(0)] = 62
revLookup['_'.charCodeAt(0)] = 63

function placeHoldersCount (b64) {
  var len = b64.length
  if (len % 4 > 0) {
    throw new Error('Invalid string. Length must be a multiple of 4')
  }

  // the number of equal signs (place holders)
  // if there are two placeholders, than the two characters before it
  // represent one byte
  // if there is only one, then the three characters before it represent 2 bytes
  // this is just a cheap hack to not do indexOf twice
  return b64[len - 2] === '=' ? 2 : b64[len - 1] === '=' ? 1 : 0
}

function byteLength (b64) {
  // base64 is 4/3 + up to two characters of the original data
  return (b64.length * 3 / 4) - placeHoldersCount(b64)
}

function toByteArray (b64) {
  var i, l, tmp, placeHolders, arr
  var len = b64.length
  placeHolders = placeHoldersCount(b64)

  arr = new Arr((len * 3 / 4) - placeHolders)

  // if there are placeholders, only get up to the last complete 4 chars
  l = placeHolders > 0 ? len - 4 : len

  var L = 0

  for (i = 0; i < l; i += 4) {
    tmp = (revLookup[b64.charCodeAt(i)] << 18) | (revLookup[b64.charCodeAt(i + 1)] << 12) | (revLookup[b64.charCodeAt(i + 2)] << 6) | revLookup[b64.charCodeAt(i + 3)]
    arr[L++] = (tmp >> 16) & 0xFF
    arr[L++] = (tmp >> 8) & 0xFF
    arr[L++] = tmp & 0xFF
  }

  if (placeHolders === 2) {
    tmp = (revLookup[b64.charCodeAt(i)] << 2) | (revLookup[b64.charCodeAt(i + 1)] >> 4)
    arr[L++] = tmp & 0xFF
  } else if (placeHolders === 1) {
    tmp = (revLookup[b64.charCodeAt(i)] << 10) | (revLookup[b64.charCodeAt(i + 1)] << 4) | (revLookup[b64.charCodeAt(i + 2)] >> 2)
    arr[L++] = (tmp >> 8) & 0xFF
    arr[L++] = tmp & 0xFF
  }

  return arr
}

function tripletToBase64 (num) {
  return lookup[num >> 18 & 0x3F] + lookup[num >> 12 & 0x3F] + lookup[num >> 6 & 0x3F] + lookup[num & 0x3F]
}

function encodeChunk (uint8, start, end) {
  var tmp
  var output = []
  for (var i = start; i < end; i += 3) {
    tmp = (uint8[i] << 16) + (uint8[i + 1] << 8) + (uint8[i + 2])
    output.push(tripletToBase64(tmp))
  }
  return output.join('')
}

function fromByteArray (uint8) {
  var tmp
  var len = uint8.length
  var extraBytes = len % 3 // if we have 1 byte left, pad 2 bytes
  var output = ''
  var parts = []
  var maxChunkLength = 16383 // must be multiple of 3

  // go through the array every three bytes, we'll deal with trailing stuff later
  for (var i = 0, len2 = len - extraBytes; i < len2; i += maxChunkLength) {
    parts.push(encodeChunk(uint8, i, (i + maxChunkLength) > len2 ? len2 : (i + maxChunkLength)))
  }

  // pad the end with zeros, but make sure to not forget the extra bytes
  if (extraBytes === 1) {
    tmp = uint8[len - 1]
    output += lookup[tmp >> 2]
    output += lookup[(tmp << 4) & 0x3F]
    output += '=='
  } else if (extraBytes === 2) {
    tmp = (uint8[len - 2] << 8) + (uint8[len - 1])
    output += lookup[tmp >> 10]
    output += lookup[(tmp >> 4) & 0x3F]
    output += lookup[(tmp << 2) & 0x3F]
    output += '='
  }

  parts.push(output)

  return parts.join('')
}

},{}],2:[function(require,module,exports){
/*!
 * The buffer module from node.js, for the browser.
 *
 * @author   Feross Aboukhadijeh <https://feross.org>
 * @license  MIT
 */
/* eslint-disable no-proto */

'use strict'

var base64 = require('base64-js')
var ieee754 = require('ieee754')

exports.Buffer = Buffer
exports.SlowBuffer = SlowBuffer
exports.INSPECT_MAX_BYTES = 50

var K_MAX_LENGTH = 0x7fffffff
exports.kMaxLength = K_MAX_LENGTH

/**
 * If `Buffer.TYPED_ARRAY_SUPPORT`:
 *   === true    Use Uint8Array implementation (fastest)
 *   === false   Print warning and recommend using `buffer` v4.x which has an Object
 *               implementation (most compatible, even IE6)
 *
 * Browsers that support typed arrays are IE 10+, Firefox 4+, Chrome 7+, Safari 5.1+,
 * Opera 11.6+, iOS 4.2+.
 *
 * We report that the browser does not support typed arrays if the are not subclassable
 * using __proto__. Firefox 4-29 lacks support for adding new properties to `Uint8Array`
 * (See: https://bugzilla.mozilla.org/show_bug.cgi?id=695438). IE 10 lacks support
 * for __proto__ and has a buggy typed array implementation.
 */
Buffer.TYPED_ARRAY_SUPPORT = typedArraySupport()

if (!Buffer.TYPED_ARRAY_SUPPORT && typeof console !== 'undefined' &&
    typeof console.error === 'function') {
  console.error(
    'This browser lacks typed array (Uint8Array) support which is required by ' +
    '`buffer` v5.x. Use `buffer` v4.x if you require old browser support.'
  )
}

function typedArraySupport () {
  // Can typed array instances can be augmented?
  try {
    var arr = new Uint8Array(1)
    arr.__proto__ = {__proto__: Uint8Array.prototype, foo: function () { return 42 }}
    return arr.foo() === 42
  } catch (e) {
    return false
  }
}

function createBuffer (length) {
  if (length > K_MAX_LENGTH) {
    throw new RangeError('Invalid typed array length')
  }
  // Return an augmented `Uint8Array` instance
  var buf = new Uint8Array(length)
  buf.__proto__ = Buffer.prototype
  return buf
}

/**
 * The Buffer constructor returns instances of `Uint8Array` that have their
 * prototype changed to `Buffer.prototype`. Furthermore, `Buffer` is a subclass of
 * `Uint8Array`, so the returned instances will have all the node `Buffer` methods
 * and the `Uint8Array` methods. Square bracket notation works as expected -- it
 * returns a single octet.
 *
 * The `Uint8Array` prototype remains unmodified.
 */

function Buffer (arg, encodingOrOffset, length) {
  // Common case.
  if (typeof arg === 'number') {
    if (typeof encodingOrOffset === 'string') {
      throw new Error(
        'If encoding is specified then the first argument must be a string'
      )
    }
    return allocUnsafe(arg)
  }
  return from(arg, encodingOrOffset, length)
}

// Fix subarray() in ES2016. See: https://github.com/feross/buffer/pull/97
if (typeof Symbol !== 'undefined' && Symbol.species &&
    Buffer[Symbol.species] === Buffer) {
  Object.defineProperty(Buffer, Symbol.species, {
    value: null,
    configurable: true,
    enumerable: false,
    writable: false
  })
}

Buffer.poolSize = 8192 // not used by this implementation

function from (value, encodingOrOffset, length) {
  if (typeof value === 'number') {
    throw new TypeError('"value" argument must not be a number')
  }

  if (isArrayBuffer(value)) {
    return fromArrayBuffer(value, encodingOrOffset, length)
  }

  if (typeof value === 'string') {
    return fromString(value, encodingOrOffset)
  }

  return fromObject(value)
}

/**
 * Functionally equivalent to Buffer(arg, encoding) but throws a TypeError
 * if value is a number.
 * Buffer.from(str[, encoding])
 * Buffer.from(array)
 * Buffer.from(buffer)
 * Buffer.from(arrayBuffer[, byteOffset[, length]])
 **/
Buffer.from = function (value, encodingOrOffset, length) {
  return from(value, encodingOrOffset, length)
}

// Note: Change prototype *after* Buffer.from is defined to workaround Chrome bug:
// https://github.com/feross/buffer/pull/148
Buffer.prototype.__proto__ = Uint8Array.prototype
Buffer.__proto__ = Uint8Array

function assertSize (size) {
  if (typeof size !== 'number') {
    throw new TypeError('"size" argument must be a number')
  } else if (size < 0) {
    throw new RangeError('"size" argument must not be negative')
  }
}

function alloc (size, fill, encoding) {
  assertSize(size)
  if (size <= 0) {
    return createBuffer(size)
  }
  if (fill !== undefined) {
    // Only pay attention to encoding if it's a string. This
    // prevents accidentally sending in a number that would
    // be interpretted as a start offset.
    return typeof encoding === 'string'
      ? createBuffer(size).fill(fill, encoding)
      : createBuffer(size).fill(fill)
  }
  return createBuffer(size)
}

/**
 * Creates a new filled Buffer instance.
 * alloc(size[, fill[, encoding]])
 **/
Buffer.alloc = function (size, fill, encoding) {
  return alloc(size, fill, encoding)
}

function allocUnsafe (size) {
  assertSize(size)
  return createBuffer(size < 0 ? 0 : checked(size) | 0)
}

/**
 * Equivalent to Buffer(num), by default creates a non-zero-filled Buffer instance.
 * */
Buffer.allocUnsafe = function (size) {
  return allocUnsafe(size)
}
/**
 * Equivalent to SlowBuffer(num), by default creates a non-zero-filled Buffer instance.
 */
Buffer.allocUnsafeSlow = function (size) {
  return allocUnsafe(size)
}

function fromString (string, encoding) {
  if (typeof encoding !== 'string' || encoding === '') {
    encoding = 'utf8'
  }

  if (!Buffer.isEncoding(encoding)) {
    throw new TypeError('"encoding" must be a valid string encoding')
  }

  var length = byteLength(string, encoding) | 0
  var buf = createBuffer(length)

  var actual = buf.write(string, encoding)

  if (actual !== length) {
    // Writing a hex string, for example, that contains invalid characters will
    // cause everything after the first invalid character to be ignored. (e.g.
    // 'abxxcd' will be treated as 'ab')
    buf = buf.slice(0, actual)
  }

  return buf
}

function fromArrayLike (array) {
  var length = array.length < 0 ? 0 : checked(array.length) | 0
  var buf = createBuffer(length)
  for (var i = 0; i < length; i += 1) {
    buf[i] = array[i] & 255
  }
  return buf
}

function fromArrayBuffer (array, byteOffset, length) {
  if (byteOffset < 0 || array.byteLength < byteOffset) {
    throw new RangeError('\'offset\' is out of bounds')
  }

  if (array.byteLength < byteOffset + (length || 0)) {
    throw new RangeError('\'length\' is out of bounds')
  }

  var buf
  if (byteOffset === undefined && length === undefined) {
    buf = new Uint8Array(array)
  } else if (length === undefined) {
    buf = new Uint8Array(array, byteOffset)
  } else {
    buf = new Uint8Array(array, byteOffset, length)
  }

  // Return an augmented `Uint8Array` instance
  buf.__proto__ = Buffer.prototype
  return buf
}

function fromObject (obj) {
  if (Buffer.isBuffer(obj)) {
    var len = checked(obj.length) | 0
    var buf = createBuffer(len)

    if (buf.length === 0) {
      return buf
    }

    obj.copy(buf, 0, 0, len)
    return buf
  }

  if (obj) {
    if (isArrayBufferView(obj) || 'length' in obj) {
      if (typeof obj.length !== 'number' || numberIsNaN(obj.length)) {
        return createBuffer(0)
      }
      return fromArrayLike(obj)
    }

    if (obj.type === 'Buffer' && Array.isArray(obj.data)) {
      return fromArrayLike(obj.data)
    }
  }

  throw new TypeError('First argument must be a string, Buffer, ArrayBuffer, Array, or array-like object.')
}

function checked (length) {
  // Note: cannot use `length < K_MAX_LENGTH` here because that fails when
  // length is NaN (which is otherwise coerced to zero.)
  if (length >= K_MAX_LENGTH) {
    throw new RangeError('Attempt to allocate Buffer larger than maximum ' +
                         'size: 0x' + K_MAX_LENGTH.toString(16) + ' bytes')
  }
  return length | 0
}

function SlowBuffer (length) {
  if (+length != length) { // eslint-disable-line eqeqeq
    length = 0
  }
  return Buffer.alloc(+length)
}

Buffer.isBuffer = function isBuffer (b) {
  return b != null && b._isBuffer === true
}

Buffer.compare = function compare (a, b) {
  if (!Buffer.isBuffer(a) || !Buffer.isBuffer(b)) {
    throw new TypeError('Arguments must be Buffers')
  }

  if (a === b) return 0

  var x = a.length
  var y = b.length

  for (var i = 0, len = Math.min(x, y); i < len; ++i) {
    if (a[i] !== b[i]) {
      x = a[i]
      y = b[i]
      break
    }
  }

  if (x < y) return -1
  if (y < x) return 1
  return 0
}

Buffer.isEncoding = function isEncoding (encoding) {
  switch (String(encoding).toLowerCase()) {
    case 'hex':
    case 'utf8':
    case 'utf-8':
    case 'ascii':
    case 'latin1':
    case 'binary':
    case 'base64':
    case 'ucs2':
    case 'ucs-2':
    case 'utf16le':
    case 'utf-16le':
      return true
    default:
      return false
  }
}

Buffer.concat = function concat (list, length) {
  if (!Array.isArray(list)) {
    throw new TypeError('"list" argument must be an Array of Buffers')
  }

  if (list.length === 0) {
    return Buffer.alloc(0)
  }

  var i
  if (length === undefined) {
    length = 0
    for (i = 0; i < list.length; ++i) {
      length += list[i].length
    }
  }

  var buffer = Buffer.allocUnsafe(length)
  var pos = 0
  for (i = 0; i < list.length; ++i) {
    var buf = list[i]
    if (!Buffer.isBuffer(buf)) {
      throw new TypeError('"list" argument must be an Array of Buffers')
    }
    buf.copy(buffer, pos)
    pos += buf.length
  }
  return buffer
}

function byteLength (string, encoding) {
  if (Buffer.isBuffer(string)) {
    return string.length
  }
  if (isArrayBufferView(string) || isArrayBuffer(string)) {
    return string.byteLength
  }
  if (typeof string !== 'string') {
    string = '' + string
  }

  var len = string.length
  if (len === 0) return 0

  // Use a for loop to avoid recursion
  var loweredCase = false
  for (;;) {
    switch (encoding) {
      case 'ascii':
      case 'latin1':
      case 'binary':
        return len
      case 'utf8':
      case 'utf-8':
      case undefined:
        return utf8ToBytes(string).length
      case 'ucs2':
      case 'ucs-2':
      case 'utf16le':
      case 'utf-16le':
        return len * 2
      case 'hex':
        return len >>> 1
      case 'base64':
        return base64ToBytes(string).length
      default:
        if (loweredCase) return utf8ToBytes(string).length // assume utf8
        encoding = ('' + encoding).toLowerCase()
        loweredCase = true
    }
  }
}
Buffer.byteLength = byteLength

function slowToString (encoding, start, end) {
  var loweredCase = false

  // No need to verify that "this.length <= MAX_UINT32" since it's a read-only
  // property of a typed array.

  // This behaves neither like String nor Uint8Array in that we set start/end
  // to their upper/lower bounds if the value passed is out of range.
  // undefined is handled specially as per ECMA-262 6th Edition,
  // Section 13.3.3.7 Runtime Semantics: KeyedBindingInitialization.
  if (start === undefined || start < 0) {
    start = 0
  }
  // Return early if start > this.length. Done here to prevent potential uint32
  // coercion fail below.
  if (start > this.length) {
    return ''
  }

  if (end === undefined || end > this.length) {
    end = this.length
  }

  if (end <= 0) {
    return ''
  }

  // Force coersion to uint32. This will also coerce falsey/NaN values to 0.
  end >>>= 0
  start >>>= 0

  if (end <= start) {
    return ''
  }

  if (!encoding) encoding = 'utf8'

  while (true) {
    switch (encoding) {
      case 'hex':
        return hexSlice(this, start, end)

      case 'utf8':
      case 'utf-8':
        return utf8Slice(this, start, end)

      case 'ascii':
        return asciiSlice(this, start, end)

      case 'latin1':
      case 'binary':
        return latin1Slice(this, start, end)

      case 'base64':
        return base64Slice(this, start, end)

      case 'ucs2':
      case 'ucs-2':
      case 'utf16le':
      case 'utf-16le':
        return utf16leSlice(this, start, end)

      default:
        if (loweredCase) throw new TypeError('Unknown encoding: ' + encoding)
        encoding = (encoding + '').toLowerCase()
        loweredCase = true
    }
  }
}

// This property is used by `Buffer.isBuffer` (and the `is-buffer` npm package)
// to detect a Buffer instance. It's not possible to use `instanceof Buffer`
// reliably in a browserify context because there could be multiple different
// copies of the 'buffer' package in use. This method works even for Buffer
// instances that were created from another copy of the `buffer` package.
// See: https://github.com/feross/buffer/issues/154
Buffer.prototype._isBuffer = true

function swap (b, n, m) {
  var i = b[n]
  b[n] = b[m]
  b[m] = i
}

Buffer.prototype.swap16 = function swap16 () {
  var len = this.length
  if (len % 2 !== 0) {
    throw new RangeError('Buffer size must be a multiple of 16-bits')
  }
  for (var i = 0; i < len; i += 2) {
    swap(this, i, i + 1)
  }
  return this
}

Buffer.prototype.swap32 = function swap32 () {
  var len = this.length
  if (len % 4 !== 0) {
    throw new RangeError('Buffer size must be a multiple of 32-bits')
  }
  for (var i = 0; i < len; i += 4) {
    swap(this, i, i + 3)
    swap(this, i + 1, i + 2)
  }
  return this
}

Buffer.prototype.swap64 = function swap64 () {
  var len = this.length
  if (len % 8 !== 0) {
    throw new RangeError('Buffer size must be a multiple of 64-bits')
  }
  for (var i = 0; i < len; i += 8) {
    swap(this, i, i + 7)
    swap(this, i + 1, i + 6)
    swap(this, i + 2, i + 5)
    swap(this, i + 3, i + 4)
  }
  return this
}

Buffer.prototype.toString = function toString () {
  var length = this.length
  if (length === 0) return ''
  if (arguments.length === 0) return utf8Slice(this, 0, length)
  return slowToString.apply(this, arguments)
}

Buffer.prototype.equals = function equals (b) {
  if (!Buffer.isBuffer(b)) throw new TypeError('Argument must be a Buffer')
  if (this === b) return true
  return Buffer.compare(this, b) === 0
}

Buffer.prototype.inspect = function inspect () {
  var str = ''
  var max = exports.INSPECT_MAX_BYTES
  if (this.length > 0) {
    str = this.toString('hex', 0, max).match(/.{2}/g).join(' ')
    if (this.length > max) str += ' ... '
  }
  return '<Buffer ' + str + '>'
}

Buffer.prototype.compare = function compare (target, start, end, thisStart, thisEnd) {
  if (!Buffer.isBuffer(target)) {
    throw new TypeError('Argument must be a Buffer')
  }

  if (start === undefined) {
    start = 0
  }
  if (end === undefined) {
    end = target ? target.length : 0
  }
  if (thisStart === undefined) {
    thisStart = 0
  }
  if (thisEnd === undefined) {
    thisEnd = this.length
  }

  if (start < 0 || end > target.length || thisStart < 0 || thisEnd > this.length) {
    throw new RangeError('out of range index')
  }

  if (thisStart >= thisEnd && start >= end) {
    return 0
  }
  if (thisStart >= thisEnd) {
    return -1
  }
  if (start >= end) {
    return 1
  }

  start >>>= 0
  end >>>= 0
  thisStart >>>= 0
  thisEnd >>>= 0

  if (this === target) return 0

  var x = thisEnd - thisStart
  var y = end - start
  var len = Math.min(x, y)

  var thisCopy = this.slice(thisStart, thisEnd)
  var targetCopy = target.slice(start, end)

  for (var i = 0; i < len; ++i) {
    if (thisCopy[i] !== targetCopy[i]) {
      x = thisCopy[i]
      y = targetCopy[i]
      break
    }
  }

  if (x < y) return -1
  if (y < x) return 1
  return 0
}

// Finds either the first index of `val` in `buffer` at offset >= `byteOffset`,
// OR the last index of `val` in `buffer` at offset <= `byteOffset`.
//
// Arguments:
// - buffer - a Buffer to search
// - val - a string, Buffer, or number
// - byteOffset - an index into `buffer`; will be clamped to an int32
// - encoding - an optional encoding, relevant is val is a string
// - dir - true for indexOf, false for lastIndexOf
function bidirectionalIndexOf (buffer, val, byteOffset, encoding, dir) {
  // Empty buffer means no match
  if (buffer.length === 0) return -1

  // Normalize byteOffset
  if (typeof byteOffset === 'string') {
    encoding = byteOffset
    byteOffset = 0
  } else if (byteOffset > 0x7fffffff) {
    byteOffset = 0x7fffffff
  } else if (byteOffset < -0x80000000) {
    byteOffset = -0x80000000
  }
  byteOffset = +byteOffset  // Coerce to Number.
  if (numberIsNaN(byteOffset)) {
    // byteOffset: it it's undefined, null, NaN, "foo", etc, search whole buffer
    byteOffset = dir ? 0 : (buffer.length - 1)
  }

  // Normalize byteOffset: negative offsets start from the end of the buffer
  if (byteOffset < 0) byteOffset = buffer.length + byteOffset
  if (byteOffset >= buffer.length) {
    if (dir) return -1
    else byteOffset = buffer.length - 1
  } else if (byteOffset < 0) {
    if (dir) byteOffset = 0
    else return -1
  }

  // Normalize val
  if (typeof val === 'string') {
    val = Buffer.from(val, encoding)
  }

  // Finally, search either indexOf (if dir is true) or lastIndexOf
  if (Buffer.isBuffer(val)) {
    // Special case: looking for empty string/buffer always fails
    if (val.length === 0) {
      return -1
    }
    return arrayIndexOf(buffer, val, byteOffset, encoding, dir)
  } else if (typeof val === 'number') {
    val = val & 0xFF // Search for a byte value [0-255]
    if (typeof Uint8Array.prototype.indexOf === 'function') {
      if (dir) {
        return Uint8Array.prototype.indexOf.call(buffer, val, byteOffset)
      } else {
        return Uint8Array.prototype.lastIndexOf.call(buffer, val, byteOffset)
      }
    }
    return arrayIndexOf(buffer, [ val ], byteOffset, encoding, dir)
  }

  throw new TypeError('val must be string, number or Buffer')
}

function arrayIndexOf (arr, val, byteOffset, encoding, dir) {
  var indexSize = 1
  var arrLength = arr.length
  var valLength = val.length

  if (encoding !== undefined) {
    encoding = String(encoding).toLowerCase()
    if (encoding === 'ucs2' || encoding === 'ucs-2' ||
        encoding === 'utf16le' || encoding === 'utf-16le') {
      if (arr.length < 2 || val.length < 2) {
        return -1
      }
      indexSize = 2
      arrLength /= 2
      valLength /= 2
      byteOffset /= 2
    }
  }

  function read (buf, i) {
    if (indexSize === 1) {
      return buf[i]
    } else {
      return buf.readUInt16BE(i * indexSize)
    }
  }

  var i
  if (dir) {
    var foundIndex = -1
    for (i = byteOffset; i < arrLength; i++) {
      if (read(arr, i) === read(val, foundIndex === -1 ? 0 : i - foundIndex)) {
        if (foundIndex === -1) foundIndex = i
        if (i - foundIndex + 1 === valLength) return foundIndex * indexSize
      } else {
        if (foundIndex !== -1) i -= i - foundIndex
        foundIndex = -1
      }
    }
  } else {
    if (byteOffset + valLength > arrLength) byteOffset = arrLength - valLength
    for (i = byteOffset; i >= 0; i--) {
      var found = true
      for (var j = 0; j < valLength; j++) {
        if (read(arr, i + j) !== read(val, j)) {
          found = false
          break
        }
      }
      if (found) return i
    }
  }

  return -1
}

Buffer.prototype.includes = function includes (val, byteOffset, encoding) {
  return this.indexOf(val, byteOffset, encoding) !== -1
}

Buffer.prototype.indexOf = function indexOf (val, byteOffset, encoding) {
  return bidirectionalIndexOf(this, val, byteOffset, encoding, true)
}

Buffer.prototype.lastIndexOf = function lastIndexOf (val, byteOffset, encoding) {
  return bidirectionalIndexOf(this, val, byteOffset, encoding, false)
}

function hexWrite (buf, string, offset, length) {
  offset = Number(offset) || 0
  var remaining = buf.length - offset
  if (!length) {
    length = remaining
  } else {
    length = Number(length)
    if (length > remaining) {
      length = remaining
    }
  }

  // must be an even number of digits
  var strLen = string.length
  if (strLen % 2 !== 0) throw new TypeError('Invalid hex string')

  if (length > strLen / 2) {
    length = strLen / 2
  }
  for (var i = 0; i < length; ++i) {
    var parsed = parseInt(string.substr(i * 2, 2), 16)
    if (numberIsNaN(parsed)) return i
    buf[offset + i] = parsed
  }
  return i
}

function utf8Write (buf, string, offset, length) {
  return blitBuffer(utf8ToBytes(string, buf.length - offset), buf, offset, length)
}

function asciiWrite (buf, string, offset, length) {
  return blitBuffer(asciiToBytes(string), buf, offset, length)
}

function latin1Write (buf, string, offset, length) {
  return asciiWrite(buf, string, offset, length)
}

function base64Write (buf, string, offset, length) {
  return blitBuffer(base64ToBytes(string), buf, offset, length)
}

function ucs2Write (buf, string, offset, length) {
  return blitBuffer(utf16leToBytes(string, buf.length - offset), buf, offset, length)
}

Buffer.prototype.write = function write (string, offset, length, encoding) {
  // Buffer#write(string)
  if (offset === undefined) {
    encoding = 'utf8'
    length = this.length
    offset = 0
  // Buffer#write(string, encoding)
  } else if (length === undefined && typeof offset === 'string') {
    encoding = offset
    length = this.length
    offset = 0
  // Buffer#write(string, offset[, length][, encoding])
  } else if (isFinite(offset)) {
    offset = offset >>> 0
    if (isFinite(length)) {
      length = length >>> 0
      if (encoding === undefined) encoding = 'utf8'
    } else {
      encoding = length
      length = undefined
    }
  } else {
    throw new Error(
      'Buffer.write(string, encoding, offset[, length]) is no longer supported'
    )
  }

  var remaining = this.length - offset
  if (length === undefined || length > remaining) length = remaining

  if ((string.length > 0 && (length < 0 || offset < 0)) || offset > this.length) {
    throw new RangeError('Attempt to write outside buffer bounds')
  }

  if (!encoding) encoding = 'utf8'

  var loweredCase = false
  for (;;) {
    switch (encoding) {
      case 'hex':
        return hexWrite(this, string, offset, length)

      case 'utf8':
      case 'utf-8':
        return utf8Write(this, string, offset, length)

      case 'ascii':
        return asciiWrite(this, string, offset, length)

      case 'latin1':
      case 'binary':
        return latin1Write(this, string, offset, length)

      case 'base64':
        // Warning: maxLength not taken into account in base64Write
        return base64Write(this, string, offset, length)

      case 'ucs2':
      case 'ucs-2':
      case 'utf16le':
      case 'utf-16le':
        return ucs2Write(this, string, offset, length)

      default:
        if (loweredCase) throw new TypeError('Unknown encoding: ' + encoding)
        encoding = ('' + encoding).toLowerCase()
        loweredCase = true
    }
  }
}

Buffer.prototype.toJSON = function toJSON () {
  return {
    type: 'Buffer',
    data: Array.prototype.slice.call(this._arr || this, 0)
  }
}

function base64Slice (buf, start, end) {
  if (start === 0 && end === buf.length) {
    return base64.fromByteArray(buf)
  } else {
    return base64.fromByteArray(buf.slice(start, end))
  }
}

function utf8Slice (buf, start, end) {
  end = Math.min(buf.length, end)
  var res = []

  var i = start
  while (i < end) {
    var firstByte = buf[i]
    var codePoint = null
    var bytesPerSequence = (firstByte > 0xEF) ? 4
      : (firstByte > 0xDF) ? 3
      : (firstByte > 0xBF) ? 2
      : 1

    if (i + bytesPerSequence <= end) {
      var secondByte, thirdByte, fourthByte, tempCodePoint

      switch (bytesPerSequence) {
        case 1:
          if (firstByte < 0x80) {
            codePoint = firstByte
          }
          break
        case 2:
          secondByte = buf[i + 1]
          if ((secondByte & 0xC0) === 0x80) {
            tempCodePoint = (firstByte & 0x1F) << 0x6 | (secondByte & 0x3F)
            if (tempCodePoint > 0x7F) {
              codePoint = tempCodePoint
            }
          }
          break
        case 3:
          secondByte = buf[i + 1]
          thirdByte = buf[i + 2]
          if ((secondByte & 0xC0) === 0x80 && (thirdByte & 0xC0) === 0x80) {
            tempCodePoint = (firstByte & 0xF) << 0xC | (secondByte & 0x3F) << 0x6 | (thirdByte & 0x3F)
            if (tempCodePoint > 0x7FF && (tempCodePoint < 0xD800 || tempCodePoint > 0xDFFF)) {
              codePoint = tempCodePoint
            }
          }
          break
        case 4:
          secondByte = buf[i + 1]
          thirdByte = buf[i + 2]
          fourthByte = buf[i + 3]
          if ((secondByte & 0xC0) === 0x80 && (thirdByte & 0xC0) === 0x80 && (fourthByte & 0xC0) === 0x80) {
            tempCodePoint = (firstByte & 0xF) << 0x12 | (secondByte & 0x3F) << 0xC | (thirdByte & 0x3F) << 0x6 | (fourthByte & 0x3F)
            if (tempCodePoint > 0xFFFF && tempCodePoint < 0x110000) {
              codePoint = tempCodePoint
            }
          }
      }
    }

    if (codePoint === null) {
      // we did not generate a valid codePoint so insert a
      // replacement char (U+FFFD) and advance only 1 byte
      codePoint = 0xFFFD
      bytesPerSequence = 1
    } else if (codePoint > 0xFFFF) {
      // encode to utf16 (surrogate pair dance)
      codePoint -= 0x10000
      res.push(codePoint >>> 10 & 0x3FF | 0xD800)
      codePoint = 0xDC00 | codePoint & 0x3FF
    }

    res.push(codePoint)
    i += bytesPerSequence
  }

  return decodeCodePointsArray(res)
}

// Based on http://stackoverflow.com/a/22747272/680742, the browser with
// the lowest limit is Chrome, with 0x10000 args.
// We go 1 magnitude less, for safety
var MAX_ARGUMENTS_LENGTH = 0x1000

function decodeCodePointsArray (codePoints) {
  var len = codePoints.length
  if (len <= MAX_ARGUMENTS_LENGTH) {
    return String.fromCharCode.apply(String, codePoints) // avoid extra slice()
  }

  // Decode in chunks to avoid "call stack size exceeded".
  var res = ''
  var i = 0
  while (i < len) {
    res += String.fromCharCode.apply(
      String,
      codePoints.slice(i, i += MAX_ARGUMENTS_LENGTH)
    )
  }
  return res
}

function asciiSlice (buf, start, end) {
  var ret = ''
  end = Math.min(buf.length, end)

  for (var i = start; i < end; ++i) {
    ret += String.fromCharCode(buf[i] & 0x7F)
  }
  return ret
}

function latin1Slice (buf, start, end) {
  var ret = ''
  end = Math.min(buf.length, end)

  for (var i = start; i < end; ++i) {
    ret += String.fromCharCode(buf[i])
  }
  return ret
}

function hexSlice (buf, start, end) {
  var len = buf.length

  if (!start || start < 0) start = 0
  if (!end || end < 0 || end > len) end = len

  var out = ''
  for (var i = start; i < end; ++i) {
    out += toHex(buf[i])
  }
  return out
}

function utf16leSlice (buf, start, end) {
  var bytes = buf.slice(start, end)
  var res = ''
  for (var i = 0; i < bytes.length; i += 2) {
    res += String.fromCharCode(bytes[i] + (bytes[i + 1] * 256))
  }
  return res
}

Buffer.prototype.slice = function slice (start, end) {
  var len = this.length
  start = ~~start
  end = end === undefined ? len : ~~end

  if (start < 0) {
    start += len
    if (start < 0) start = 0
  } else if (start > len) {
    start = len
  }

  if (end < 0) {
    end += len
    if (end < 0) end = 0
  } else if (end > len) {
    end = len
  }

  if (end < start) end = start

  var newBuf = this.subarray(start, end)
  // Return an augmented `Uint8Array` instance
  newBuf.__proto__ = Buffer.prototype
  return newBuf
}

/*
 * Need to make sure that buffer isn't trying to write out of bounds.
 */
function checkOffset (offset, ext, length) {
  if ((offset % 1) !== 0 || offset < 0) throw new RangeError('offset is not uint')
  if (offset + ext > length) throw new RangeError('Trying to access beyond buffer length')
}

Buffer.prototype.readUIntLE = function readUIntLE (offset, byteLength, noAssert) {
  offset = offset >>> 0
  byteLength = byteLength >>> 0
  if (!noAssert) checkOffset(offset, byteLength, this.length)

  var val = this[offset]
  var mul = 1
  var i = 0
  while (++i < byteLength && (mul *= 0x100)) {
    val += this[offset + i] * mul
  }

  return val
}

Buffer.prototype.readUIntBE = function readUIntBE (offset, byteLength, noAssert) {
  offset = offset >>> 0
  byteLength = byteLength >>> 0
  if (!noAssert) {
    checkOffset(offset, byteLength, this.length)
  }

  var val = this[offset + --byteLength]
  var mul = 1
  while (byteLength > 0 && (mul *= 0x100)) {
    val += this[offset + --byteLength] * mul
  }

  return val
}

Buffer.prototype.readUInt8 = function readUInt8 (offset, noAssert) {
  offset = offset >>> 0
  if (!noAssert) checkOffset(offset, 1, this.length)
  return this[offset]
}

Buffer.prototype.readUInt16LE = function readUInt16LE (offset, noAssert) {
  offset = offset >>> 0
  if (!noAssert) checkOffset(offset, 2, this.length)
  return this[offset] | (this[offset + 1] << 8)
}

Buffer.prototype.readUInt16BE = function readUInt16BE (offset, noAssert) {
  offset = offset >>> 0
  if (!noAssert) checkOffset(offset, 2, this.length)
  return (this[offset] << 8) | this[offset + 1]
}

Buffer.prototype.readUInt32LE = function readUInt32LE (offset, noAssert) {
  offset = offset >>> 0
  if (!noAssert) checkOffset(offset, 4, this.length)

  return ((this[offset]) |
      (this[offset + 1] << 8) |
      (this[offset + 2] << 16)) +
      (this[offset + 3] * 0x1000000)
}

Buffer.prototype.readUInt32BE = function readUInt32BE (offset, noAssert) {
  offset = offset >>> 0
  if (!noAssert) checkOffset(offset, 4, this.length)

  return (this[offset] * 0x1000000) +
    ((this[offset + 1] << 16) |
    (this[offset + 2] << 8) |
    this[offset + 3])
}

Buffer.prototype.readIntLE = function readIntLE (offset, byteLength, noAssert) {
  offset = offset >>> 0
  byteLength = byteLength >>> 0
  if (!noAssert) checkOffset(offset, byteLength, this.length)

  var val = this[offset]
  var mul = 1
  var i = 0
  while (++i < byteLength && (mul *= 0x100)) {
    val += this[offset + i] * mul
  }
  mul *= 0x80

  if (val >= mul) val -= Math.pow(2, 8 * byteLength)

  return val
}

Buffer.prototype.readIntBE = function readIntBE (offset, byteLength, noAssert) {
  offset = offset >>> 0
  byteLength = byteLength >>> 0
  if (!noAssert) checkOffset(offset, byteLength, this.length)

  var i = byteLength
  var mul = 1
  var val = this[offset + --i]
  while (i > 0 && (mul *= 0x100)) {
    val += this[offset + --i] * mul
  }
  mul *= 0x80

  if (val >= mul) val -= Math.pow(2, 8 * byteLength)

  return val
}

Buffer.prototype.readInt8 = function readInt8 (offset, noAssert) {
  offset = offset >>> 0
  if (!noAssert) checkOffset(offset, 1, this.length)
  if (!(this[offset] & 0x80)) return (this[offset])
  return ((0xff - this[offset] + 1) * -1)
}

Buffer.prototype.readInt16LE = function readInt16LE (offset, noAssert) {
  offset = offset >>> 0
  if (!noAssert) checkOffset(offset, 2, this.length)
  var val = this[offset] | (this[offset + 1] << 8)
  return (val & 0x8000) ? val | 0xFFFF0000 : val
}

Buffer.prototype.readInt16BE = function readInt16BE (offset, noAssert) {
  offset = offset >>> 0
  if (!noAssert) checkOffset(offset, 2, this.length)
  var val = this[offset + 1] | (this[offset] << 8)
  return (val & 0x8000) ? val | 0xFFFF0000 : val
}

Buffer.prototype.readInt32LE = function readInt32LE (offset, noAssert) {
  offset = offset >>> 0
  if (!noAssert) checkOffset(offset, 4, this.length)

  return (this[offset]) |
    (this[offset + 1] << 8) |
    (this[offset + 2] << 16) |
    (this[offset + 3] << 24)
}

Buffer.prototype.readInt32BE = function readInt32BE (offset, noAssert) {
  offset = offset >>> 0
  if (!noAssert) checkOffset(offset, 4, this.length)

  return (this[offset] << 24) |
    (this[offset + 1] << 16) |
    (this[offset + 2] << 8) |
    (this[offset + 3])
}

Buffer.prototype.readFloatLE = function readFloatLE (offset, noAssert) {
  offset = offset >>> 0
  if (!noAssert) checkOffset(offset, 4, this.length)
  return ieee754.read(this, offset, true, 23, 4)
}

Buffer.prototype.readFloatBE = function readFloatBE (offset, noAssert) {
  offset = offset >>> 0
  if (!noAssert) checkOffset(offset, 4, this.length)
  return ieee754.read(this, offset, false, 23, 4)
}

Buffer.prototype.readDoubleLE = function readDoubleLE (offset, noAssert) {
  offset = offset >>> 0
  if (!noAssert) checkOffset(offset, 8, this.length)
  return ieee754.read(this, offset, true, 52, 8)
}

Buffer.prototype.readDoubleBE = function readDoubleBE (offset, noAssert) {
  offset = offset >>> 0
  if (!noAssert) checkOffset(offset, 8, this.length)
  return ieee754.read(this, offset, false, 52, 8)
}

function checkInt (buf, value, offset, ext, max, min) {
  if (!Buffer.isBuffer(buf)) throw new TypeError('"buffer" argument must be a Buffer instance')
  if (value > max || value < min) throw new RangeError('"value" argument is out of bounds')
  if (offset + ext > buf.length) throw new RangeError('Index out of range')
}

Buffer.prototype.writeUIntLE = function writeUIntLE (value, offset, byteLength, noAssert) {
  value = +value
  offset = offset >>> 0
  byteLength = byteLength >>> 0
  if (!noAssert) {
    var maxBytes = Math.pow(2, 8 * byteLength) - 1
    checkInt(this, value, offset, byteLength, maxBytes, 0)
  }

  var mul = 1
  var i = 0
  this[offset] = value & 0xFF
  while (++i < byteLength && (mul *= 0x100)) {
    this[offset + i] = (value / mul) & 0xFF
  }

  return offset + byteLength
}

Buffer.prototype.writeUIntBE = function writeUIntBE (value, offset, byteLength, noAssert) {
  value = +value
  offset = offset >>> 0
  byteLength = byteLength >>> 0
  if (!noAssert) {
    var maxBytes = Math.pow(2, 8 * byteLength) - 1
    checkInt(this, value, offset, byteLength, maxBytes, 0)
  }

  var i = byteLength - 1
  var mul = 1
  this[offset + i] = value & 0xFF
  while (--i >= 0 && (mul *= 0x100)) {
    this[offset + i] = (value / mul) & 0xFF
  }

  return offset + byteLength
}

Buffer.prototype.writeUInt8 = function writeUInt8 (value, offset, noAssert) {
  value = +value
  offset = offset >>> 0
  if (!noAssert) checkInt(this, value, offset, 1, 0xff, 0)
  this[offset] = (value & 0xff)
  return offset + 1
}

Buffer.prototype.writeUInt16LE = function writeUInt16LE (value, offset, noAssert) {
  value = +value
  offset = offset >>> 0
  if (!noAssert) checkInt(this, value, offset, 2, 0xffff, 0)
  this[offset] = (value & 0xff)
  this[offset + 1] = (value >>> 8)
  return offset + 2
}

Buffer.prototype.writeUInt16BE = function writeUInt16BE (value, offset, noAssert) {
  value = +value
  offset = offset >>> 0
  if (!noAssert) checkInt(this, value, offset, 2, 0xffff, 0)
  this[offset] = (value >>> 8)
  this[offset + 1] = (value & 0xff)
  return offset + 2
}

Buffer.prototype.writeUInt32LE = function writeUInt32LE (value, offset, noAssert) {
  value = +value
  offset = offset >>> 0
  if (!noAssert) checkInt(this, value, offset, 4, 0xffffffff, 0)
  this[offset + 3] = (value >>> 24)
  this[offset + 2] = (value >>> 16)
  this[offset + 1] = (value >>> 8)
  this[offset] = (value & 0xff)
  return offset + 4
}

Buffer.prototype.writeUInt32BE = function writeUInt32BE (value, offset, noAssert) {
  value = +value
  offset = offset >>> 0
  if (!noAssert) checkInt(this, value, offset, 4, 0xffffffff, 0)
  this[offset] = (value >>> 24)
  this[offset + 1] = (value >>> 16)
  this[offset + 2] = (value >>> 8)
  this[offset + 3] = (value & 0xff)
  return offset + 4
}

Buffer.prototype.writeIntLE = function writeIntLE (value, offset, byteLength, noAssert) {
  value = +value
  offset = offset >>> 0
  if (!noAssert) {
    var limit = Math.pow(2, (8 * byteLength) - 1)

    checkInt(this, value, offset, byteLength, limit - 1, -limit)
  }

  var i = 0
  var mul = 1
  var sub = 0
  this[offset] = value & 0xFF
  while (++i < byteLength && (mul *= 0x100)) {
    if (value < 0 && sub === 0 && this[offset + i - 1] !== 0) {
      sub = 1
    }
    this[offset + i] = ((value / mul) >> 0) - sub & 0xFF
  }

  return offset + byteLength
}

Buffer.prototype.writeIntBE = function writeIntBE (value, offset, byteLength, noAssert) {
  value = +value
  offset = offset >>> 0
  if (!noAssert) {
    var limit = Math.pow(2, (8 * byteLength) - 1)

    checkInt(this, value, offset, byteLength, limit - 1, -limit)
  }

  var i = byteLength - 1
  var mul = 1
  var sub = 0
  this[offset + i] = value & 0xFF
  while (--i >= 0 && (mul *= 0x100)) {
    if (value < 0 && sub === 0 && this[offset + i + 1] !== 0) {
      sub = 1
    }
    this[offset + i] = ((value / mul) >> 0) - sub & 0xFF
  }

  return offset + byteLength
}

Buffer.prototype.writeInt8 = function writeInt8 (value, offset, noAssert) {
  value = +value
  offset = offset >>> 0
  if (!noAssert) checkInt(this, value, offset, 1, 0x7f, -0x80)
  if (value < 0) value = 0xff + value + 1
  this[offset] = (value & 0xff)
  return offset + 1
}

Buffer.prototype.writeInt16LE = function writeInt16LE (value, offset, noAssert) {
  value = +value
  offset = offset >>> 0
  if (!noAssert) checkInt(this, value, offset, 2, 0x7fff, -0x8000)
  this[offset] = (value & 0xff)
  this[offset + 1] = (value >>> 8)
  return offset + 2
}

Buffer.prototype.writeInt16BE = function writeInt16BE (value, offset, noAssert) {
  value = +value
  offset = offset >>> 0
  if (!noAssert) checkInt(this, value, offset, 2, 0x7fff, -0x8000)
  this[offset] = (value >>> 8)
  this[offset + 1] = (value & 0xff)
  return offset + 2
}

Buffer.prototype.writeInt32LE = function writeInt32LE (value, offset, noAssert) {
  value = +value
  offset = offset >>> 0
  if (!noAssert) checkInt(this, value, offset, 4, 0x7fffffff, -0x80000000)
  this[offset] = (value & 0xff)
  this[offset + 1] = (value >>> 8)
  this[offset + 2] = (value >>> 16)
  this[offset + 3] = (value >>> 24)
  return offset + 4
}

Buffer.prototype.writeInt32BE = function writeInt32BE (value, offset, noAssert) {
  value = +value
  offset = offset >>> 0
  if (!noAssert) checkInt(this, value, offset, 4, 0x7fffffff, -0x80000000)
  if (value < 0) value = 0xffffffff + value + 1
  this[offset] = (value >>> 24)
  this[offset + 1] = (value >>> 16)
  this[offset + 2] = (value >>> 8)
  this[offset + 3] = (value & 0xff)
  return offset + 4
}

function checkIEEE754 (buf, value, offset, ext, max, min) {
  if (offset + ext > buf.length) throw new RangeError('Index out of range')
  if (offset < 0) throw new RangeError('Index out of range')
}

function writeFloat (buf, value, offset, littleEndian, noAssert) {
  value = +value
  offset = offset >>> 0
  if (!noAssert) {
    checkIEEE754(buf, value, offset, 4, 3.4028234663852886e+38, -3.4028234663852886e+38)
  }
  ieee754.write(buf, value, offset, littleEndian, 23, 4)
  return offset + 4
}

Buffer.prototype.writeFloatLE = function writeFloatLE (value, offset, noAssert) {
  return writeFloat(this, value, offset, true, noAssert)
}

Buffer.prototype.writeFloatBE = function writeFloatBE (value, offset, noAssert) {
  return writeFloat(this, value, offset, false, noAssert)
}

function writeDouble (buf, value, offset, littleEndian, noAssert) {
  value = +value
  offset = offset >>> 0
  if (!noAssert) {
    checkIEEE754(buf, value, offset, 8, 1.7976931348623157E+308, -1.7976931348623157E+308)
  }
  ieee754.write(buf, value, offset, littleEndian, 52, 8)
  return offset + 8
}

Buffer.prototype.writeDoubleLE = function writeDoubleLE (value, offset, noAssert) {
  return writeDouble(this, value, offset, true, noAssert)
}

Buffer.prototype.writeDoubleBE = function writeDoubleBE (value, offset, noAssert) {
  return writeDouble(this, value, offset, false, noAssert)
}

// copy(targetBuffer, targetStart=0, sourceStart=0, sourceEnd=buffer.length)
Buffer.prototype.copy = function copy (target, targetStart, start, end) {
  if (!start) start = 0
  if (!end && end !== 0) end = this.length
  if (targetStart >= target.length) targetStart = target.length
  if (!targetStart) targetStart = 0
  if (end > 0 && end < start) end = start

  // Copy 0 bytes; we're done
  if (end === start) return 0
  if (target.length === 0 || this.length === 0) return 0

  // Fatal error conditions
  if (targetStart < 0) {
    throw new RangeError('targetStart out of bounds')
  }
  if (start < 0 || start >= this.length) throw new RangeError('sourceStart out of bounds')
  if (end < 0) throw new RangeError('sourceEnd out of bounds')

  // Are we oob?
  if (end > this.length) end = this.length
  if (target.length - targetStart < end - start) {
    end = target.length - targetStart + start
  }

  var len = end - start
  var i

  if (this === target && start < targetStart && targetStart < end) {
    // descending copy from end
    for (i = len - 1; i >= 0; --i) {
      target[i + targetStart] = this[i + start]
    }
  } else if (len < 1000) {
    // ascending copy from start
    for (i = 0; i < len; ++i) {
      target[i + targetStart] = this[i + start]
    }
  } else {
    Uint8Array.prototype.set.call(
      target,
      this.subarray(start, start + len),
      targetStart
    )
  }

  return len
}

// Usage:
//    buffer.fill(number[, offset[, end]])
//    buffer.fill(buffer[, offset[, end]])
//    buffer.fill(string[, offset[, end]][, encoding])
Buffer.prototype.fill = function fill (val, start, end, encoding) {
  // Handle string cases:
  if (typeof val === 'string') {
    if (typeof start === 'string') {
      encoding = start
      start = 0
      end = this.length
    } else if (typeof end === 'string') {
      encoding = end
      end = this.length
    }
    if (val.length === 1) {
      var code = val.charCodeAt(0)
      if (code < 256) {
        val = code
      }
    }
    if (encoding !== undefined && typeof encoding !== 'string') {
      throw new TypeError('encoding must be a string')
    }
    if (typeof encoding === 'string' && !Buffer.isEncoding(encoding)) {
      throw new TypeError('Unknown encoding: ' + encoding)
    }
  } else if (typeof val === 'number') {
    val = val & 255
  }

  // Invalid ranges are not set to a default, so can range check early.
  if (start < 0 || this.length < start || this.length < end) {
    throw new RangeError('Out of range index')
  }

  if (end <= start) {
    return this
  }

  start = start >>> 0
  end = end === undefined ? this.length : end >>> 0

  if (!val) val = 0

  var i
  if (typeof val === 'number') {
    for (i = start; i < end; ++i) {
      this[i] = val
    }
  } else {
    var bytes = Buffer.isBuffer(val)
      ? val
      : new Buffer(val, encoding)
    var len = bytes.length
    for (i = 0; i < end - start; ++i) {
      this[i + start] = bytes[i % len]
    }
  }

  return this
}

// HELPER FUNCTIONS
// ================

var INVALID_BASE64_RE = /[^+/0-9A-Za-z-_]/g

function base64clean (str) {
  // Node strips out invalid characters like \n and \t from the string, base64-js does not
  str = str.trim().replace(INVALID_BASE64_RE, '')
  // Node converts strings with length < 2 to ''
  if (str.length < 2) return ''
  // Node allows for non-padded base64 strings (missing trailing ===), base64-js does not
  while (str.length % 4 !== 0) {
    str = str + '='
  }
  return str
}

function toHex (n) {
  if (n < 16) return '0' + n.toString(16)
  return n.toString(16)
}

function utf8ToBytes (string, units) {
  units = units || Infinity
  var codePoint
  var length = string.length
  var leadSurrogate = null
  var bytes = []

  for (var i = 0; i < length; ++i) {
    codePoint = string.charCodeAt(i)

    // is surrogate component
    if (codePoint > 0xD7FF && codePoint < 0xE000) {
      // last char was a lead
      if (!leadSurrogate) {
        // no lead yet
        if (codePoint > 0xDBFF) {
          // unexpected trail
          if ((units -= 3) > -1) bytes.push(0xEF, 0xBF, 0xBD)
          continue
        } else if (i + 1 === length) {
          // unpaired lead
          if ((units -= 3) > -1) bytes.push(0xEF, 0xBF, 0xBD)
          continue
        }

        // valid lead
        leadSurrogate = codePoint

        continue
      }

      // 2 leads in a row
      if (codePoint < 0xDC00) {
        if ((units -= 3) > -1) bytes.push(0xEF, 0xBF, 0xBD)
        leadSurrogate = codePoint
        continue
      }

      // valid surrogate pair
      codePoint = (leadSurrogate - 0xD800 << 10 | codePoint - 0xDC00) + 0x10000
    } else if (leadSurrogate) {
      // valid bmp char, but last char was a lead
      if ((units -= 3) > -1) bytes.push(0xEF, 0xBF, 0xBD)
    }

    leadSurrogate = null

    // encode utf8
    if (codePoint < 0x80) {
      if ((units -= 1) < 0) break
      bytes.push(codePoint)
    } else if (codePoint < 0x800) {
      if ((units -= 2) < 0) break
      bytes.push(
        codePoint >> 0x6 | 0xC0,
        codePoint & 0x3F | 0x80
      )
    } else if (codePoint < 0x10000) {
      if ((units -= 3) < 0) break
      bytes.push(
        codePoint >> 0xC | 0xE0,
        codePoint >> 0x6 & 0x3F | 0x80,
        codePoint & 0x3F | 0x80
      )
    } else if (codePoint < 0x110000) {
      if ((units -= 4) < 0) break
      bytes.push(
        codePoint >> 0x12 | 0xF0,
        codePoint >> 0xC & 0x3F | 0x80,
        codePoint >> 0x6 & 0x3F | 0x80,
        codePoint & 0x3F | 0x80
      )
    } else {
      throw new Error('Invalid code point')
    }
  }

  return bytes
}

function asciiToBytes (str) {
  var byteArray = []
  for (var i = 0; i < str.length; ++i) {
    // Node's code seems to be doing this and not & 0x7F..
    byteArray.push(str.charCodeAt(i) & 0xFF)
  }
  return byteArray
}

function utf16leToBytes (str, units) {
  var c, hi, lo
  var byteArray = []
  for (var i = 0; i < str.length; ++i) {
    if ((units -= 2) < 0) break

    c = str.charCodeAt(i)
    hi = c >> 8
    lo = c % 256
    byteArray.push(lo)
    byteArray.push(hi)
  }

  return byteArray
}

function base64ToBytes (str) {
  return base64.toByteArray(base64clean(str))
}

function blitBuffer (src, dst, offset, length) {
  for (var i = 0; i < length; ++i) {
    if ((i + offset >= dst.length) || (i >= src.length)) break
    dst[i + offset] = src[i]
  }
  return i
}

// ArrayBuffers from another context (i.e. an iframe) do not pass the `instanceof` check
// but they should be treated as valid. See: https://github.com/feross/buffer/issues/166
function isArrayBuffer (obj) {
  return obj instanceof ArrayBuffer ||
    (obj != null && obj.constructor != null && obj.constructor.name === 'ArrayBuffer' &&
      typeof obj.byteLength === 'number')
}

// Node 0.10 supports `ArrayBuffer` but lacks `ArrayBuffer.isView`
function isArrayBufferView (obj) {
  return (typeof ArrayBuffer.isView === 'function') && ArrayBuffer.isView(obj)
}

function numberIsNaN (obj) {
  return obj !== obj // eslint-disable-line no-self-compare
}

},{"base64-js":1,"ieee754":4}],3:[function(require,module,exports){
// Copyright Joyent, Inc. and other Node contributors.
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the
// following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.

function EventEmitter() {
  this._events = this._events || {};
  this._maxListeners = this._maxListeners || undefined;
}
module.exports = EventEmitter;

// Backwards-compat with node 0.10.x
EventEmitter.EventEmitter = EventEmitter;

EventEmitter.prototype._events = undefined;
EventEmitter.prototype._maxListeners = undefined;

// By default EventEmitters will print a warning if more than 10 listeners are
// added to it. This is a useful default which helps finding memory leaks.
EventEmitter.defaultMaxListeners = 10;

// Obviously not all Emitters should be limited to 10. This function allows
// that to be increased. Set to zero for unlimited.
EventEmitter.prototype.setMaxListeners = function(n) {
  if (!isNumber(n) || n < 0 || isNaN(n))
    throw TypeError('n must be a positive number');
  this._maxListeners = n;
  return this;
};

EventEmitter.prototype.emit = function(type) {
  var er, handler, len, args, i, listeners;

  if (!this._events)
    this._events = {};

  // If there is no 'error' event listener then throw.
  if (type === 'error') {
    if (!this._events.error ||
        (isObject(this._events.error) && !this._events.error.length)) {
      er = arguments[1];
      if (er instanceof Error) {
        throw er; // Unhandled 'error' event
      } else {
        // At least give some kind of context to the user
        var err = new Error('Uncaught, unspecified "error" event. (' + er + ')');
        err.context = er;
        throw err;
      }
    }
  }

  handler = this._events[type];

  if (isUndefined(handler))
    return false;

  if (isFunction(handler)) {
    switch (arguments.length) {
      // fast cases
      case 1:
        handler.call(this);
        break;
      case 2:
        handler.call(this, arguments[1]);
        break;
      case 3:
        handler.call(this, arguments[1], arguments[2]);
        break;
      // slower
      default:
        args = Array.prototype.slice.call(arguments, 1);
        handler.apply(this, args);
    }
  } else if (isObject(handler)) {
    args = Array.prototype.slice.call(arguments, 1);
    listeners = handler.slice();
    len = listeners.length;
    for (i = 0; i < len; i++)
      listeners[i].apply(this, args);
  }

  return true;
};

EventEmitter.prototype.addListener = function(type, listener) {
  var m;

  if (!isFunction(listener))
    throw TypeError('listener must be a function');

  if (!this._events)
    this._events = {};

  // To avoid recursion in the case that type === "newListener"! Before
  // adding it to the listeners, first emit "newListener".
  if (this._events.newListener)
    this.emit('newListener', type,
              isFunction(listener.listener) ?
              listener.listener : listener);

  if (!this._events[type])
    // Optimize the case of one listener. Don't need the extra array object.
    this._events[type] = listener;
  else if (isObject(this._events[type]))
    // If we've already got an array, just append.
    this._events[type].push(listener);
  else
    // Adding the second element, need to change to array.
    this._events[type] = [this._events[type], listener];

  // Check for listener leak
  if (isObject(this._events[type]) && !this._events[type].warned) {
    if (!isUndefined(this._maxListeners)) {
      m = this._maxListeners;
    } else {
      m = EventEmitter.defaultMaxListeners;
    }

    if (m && m > 0 && this._events[type].length > m) {
      this._events[type].warned = true;
      console.error('(node) warning: possible EventEmitter memory ' +
                    'leak detected. %d listeners added. ' +
                    'Use emitter.setMaxListeners() to increase limit.',
                    this._events[type].length);
      if (typeof console.trace === 'function') {
        // not supported in IE 10
        console.trace();
      }
    }
  }

  return this;
};

EventEmitter.prototype.on = EventEmitter.prototype.addListener;

EventEmitter.prototype.once = function(type, listener) {
  if (!isFunction(listener))
    throw TypeError('listener must be a function');

  var fired = false;

  function g() {
    this.removeListener(type, g);

    if (!fired) {
      fired = true;
      listener.apply(this, arguments);
    }
  }

  g.listener = listener;
  this.on(type, g);

  return this;
};

// emits a 'removeListener' event iff the listener was removed
EventEmitter.prototype.removeListener = function(type, listener) {
  var list, position, length, i;

  if (!isFunction(listener))
    throw TypeError('listener must be a function');

  if (!this._events || !this._events[type])
    return this;

  list = this._events[type];
  length = list.length;
  position = -1;

  if (list === listener ||
      (isFunction(list.listener) && list.listener === listener)) {
    delete this._events[type];
    if (this._events.removeListener)
      this.emit('removeListener', type, listener);

  } else if (isObject(list)) {
    for (i = length; i-- > 0;) {
      if (list[i] === listener ||
          (list[i].listener && list[i].listener === listener)) {
        position = i;
        break;
      }
    }

    if (position < 0)
      return this;

    if (list.length === 1) {
      list.length = 0;
      delete this._events[type];
    } else {
      list.splice(position, 1);
    }

    if (this._events.removeListener)
      this.emit('removeListener', type, listener);
  }

  return this;
};

EventEmitter.prototype.removeAllListeners = function(type) {
  var key, listeners;

  if (!this._events)
    return this;

  // not listening for removeListener, no need to emit
  if (!this._events.removeListener) {
    if (arguments.length === 0)
      this._events = {};
    else if (this._events[type])
      delete this._events[type];
    return this;
  }

  // emit removeListener for all listeners on all events
  if (arguments.length === 0) {
    for (key in this._events) {
      if (key === 'removeListener') continue;
      this.removeAllListeners(key);
    }
    this.removeAllListeners('removeListener');
    this._events = {};
    return this;
  }

  listeners = this._events[type];

  if (isFunction(listeners)) {
    this.removeListener(type, listeners);
  } else if (listeners) {
    // LIFO order
    while (listeners.length)
      this.removeListener(type, listeners[listeners.length - 1]);
  }
  delete this._events[type];

  return this;
};

EventEmitter.prototype.listeners = function(type) {
  var ret;
  if (!this._events || !this._events[type])
    ret = [];
  else if (isFunction(this._events[type]))
    ret = [this._events[type]];
  else
    ret = this._events[type].slice();
  return ret;
};

EventEmitter.prototype.listenerCount = function(type) {
  if (this._events) {
    var evlistener = this._events[type];

    if (isFunction(evlistener))
      return 1;
    else if (evlistener)
      return evlistener.length;
  }
  return 0;
};

EventEmitter.listenerCount = function(emitter, type) {
  return emitter.listenerCount(type);
};

function isFunction(arg) {
  return typeof arg === 'function';
}

function isNumber(arg) {
  return typeof arg === 'number';
}

function isObject(arg) {
  return typeof arg === 'object' && arg !== null;
}

function isUndefined(arg) {
  return arg === void 0;
}

},{}],4:[function(require,module,exports){
exports.read = function (buffer, offset, isLE, mLen, nBytes) {
  var e, m
  var eLen = nBytes * 8 - mLen - 1
  var eMax = (1 << eLen) - 1
  var eBias = eMax >> 1
  var nBits = -7
  var i = isLE ? (nBytes - 1) : 0
  var d = isLE ? -1 : 1
  var s = buffer[offset + i]

  i += d

  e = s & ((1 << (-nBits)) - 1)
  s >>= (-nBits)
  nBits += eLen
  for (; nBits > 0; e = e * 256 + buffer[offset + i], i += d, nBits -= 8) {}

  m = e & ((1 << (-nBits)) - 1)
  e >>= (-nBits)
  nBits += mLen
  for (; nBits > 0; m = m * 256 + buffer[offset + i], i += d, nBits -= 8) {}

  if (e === 0) {
    e = 1 - eBias
  } else if (e === eMax) {
    return m ? NaN : ((s ? -1 : 1) * Infinity)
  } else {
    m = m + Math.pow(2, mLen)
    e = e - eBias
  }
  return (s ? -1 : 1) * m * Math.pow(2, e - mLen)
}

exports.write = function (buffer, value, offset, isLE, mLen, nBytes) {
  var e, m, c
  var eLen = nBytes * 8 - mLen - 1
  var eMax = (1 << eLen) - 1
  var eBias = eMax >> 1
  var rt = (mLen === 23 ? Math.pow(2, -24) - Math.pow(2, -77) : 0)
  var i = isLE ? 0 : (nBytes - 1)
  var d = isLE ? 1 : -1
  var s = value < 0 || (value === 0 && 1 / value < 0) ? 1 : 0

  value = Math.abs(value)

  if (isNaN(value) || value === Infinity) {
    m = isNaN(value) ? 1 : 0
    e = eMax
  } else {
    e = Math.floor(Math.log(value) / Math.LN2)
    if (value * (c = Math.pow(2, -e)) < 1) {
      e--
      c *= 2
    }
    if (e + eBias >= 1) {
      value += rt / c
    } else {
      value += rt * Math.pow(2, 1 - eBias)
    }
    if (value * c >= 2) {
      e++
      c /= 2
    }

    if (e + eBias >= eMax) {
      m = 0
      e = eMax
    } else if (e + eBias >= 1) {
      m = (value * c - 1) * Math.pow(2, mLen)
      e = e + eBias
    } else {
      m = value * Math.pow(2, eBias - 1) * Math.pow(2, mLen)
      e = 0
    }
  }

  for (; mLen >= 8; buffer[offset + i] = m & 0xff, i += d, m /= 256, mLen -= 8) {}

  e = (e << mLen) | m
  eLen += mLen
  for (; eLen > 0; buffer[offset + i] = e & 0xff, i += d, e /= 256, eLen -= 8) {}

  buffer[offset + i - d] |= s * 128
}

},{}],5:[function(require,module,exports){
// shim for using process in browser
var process = module.exports = {};

// cached from whatever global is present so that test runners that stub it
// don't break things.  But we need to wrap it in a try catch in case it is
// wrapped in strict mode code which doesn't define any globals.  It's inside a
// function because try/catches deoptimize in certain engines.

var cachedSetTimeout;
var cachedClearTimeout;

function defaultSetTimout() {
    throw new Error('setTimeout has not been defined');
}
function defaultClearTimeout () {
    throw new Error('clearTimeout has not been defined');
}
(function () {
    try {
        if (typeof setTimeout === 'function') {
            cachedSetTimeout = setTimeout;
        } else {
            cachedSetTimeout = defaultSetTimout;
        }
    } catch (e) {
        cachedSetTimeout = defaultSetTimout;
    }
    try {
        if (typeof clearTimeout === 'function') {
            cachedClearTimeout = clearTimeout;
        } else {
            cachedClearTimeout = defaultClearTimeout;
        }
    } catch (e) {
        cachedClearTimeout = defaultClearTimeout;
    }
} ())
function runTimeout(fun) {
    if (cachedSetTimeout === setTimeout) {
        //normal enviroments in sane situations
        return setTimeout(fun, 0);
    }
    // if setTimeout wasn't available but was latter defined
    if ((cachedSetTimeout === defaultSetTimout || !cachedSetTimeout) && setTimeout) {
        cachedSetTimeout = setTimeout;
        return setTimeout(fun, 0);
    }
    try {
        // when when somebody has screwed with setTimeout but no I.E. maddness
        return cachedSetTimeout(fun, 0);
    } catch(e){
        try {
            // When we are in I.E. but the script has been evaled so I.E. doesn't trust the global object when called normally
            return cachedSetTimeout.call(null, fun, 0);
        } catch(e){
            // same as above but when it's a version of I.E. that must have the global object for 'this', hopfully our context correct otherwise it will throw a global error
            return cachedSetTimeout.call(this, fun, 0);
        }
    }


}
function runClearTimeout(marker) {
    if (cachedClearTimeout === clearTimeout) {
        //normal enviroments in sane situations
        return clearTimeout(marker);
    }
    // if clearTimeout wasn't available but was latter defined
    if ((cachedClearTimeout === defaultClearTimeout || !cachedClearTimeout) && clearTimeout) {
        cachedClearTimeout = clearTimeout;
        return clearTimeout(marker);
    }
    try {
        // when when somebody has screwed with setTimeout but no I.E. maddness
        return cachedClearTimeout(marker);
    } catch (e){
        try {
            // When we are in I.E. but the script has been evaled so I.E. doesn't  trust the global object when called normally
            return cachedClearTimeout.call(null, marker);
        } catch (e){
            // same as above but when it's a version of I.E. that must have the global object for 'this', hopfully our context correct otherwise it will throw a global error.
            // Some versions of I.E. have different rules for clearTimeout vs setTimeout
            return cachedClearTimeout.call(this, marker);
        }
    }



}
var queue = [];
var draining = false;
var currentQueue;
var queueIndex = -1;

function cleanUpNextTick() {
    if (!draining || !currentQueue) {
        return;
    }
    draining = false;
    if (currentQueue.length) {
        queue = currentQueue.concat(queue);
    } else {
        queueIndex = -1;
    }
    if (queue.length) {
        drainQueue();
    }
}

function drainQueue() {
    if (draining) {
        return;
    }
    var timeout = runTimeout(cleanUpNextTick);
    draining = true;

    var len = queue.length;
    while(len) {
        currentQueue = queue;
        queue = [];
        while (++queueIndex < len) {
            if (currentQueue) {
                currentQueue[queueIndex].run();
            }
        }
        queueIndex = -1;
        len = queue.length;
    }
    currentQueue = null;
    draining = false;
    runClearTimeout(timeout);
}

process.nextTick = function (fun) {
    var args = new Array(arguments.length - 1);
    if (arguments.length > 1) {
        for (var i = 1; i < arguments.length; i++) {
            args[i - 1] = arguments[i];
        }
    }
    queue.push(new Item(fun, args));
    if (queue.length === 1 && !draining) {
        runTimeout(drainQueue);
    }
};

// v8 likes predictible objects
function Item(fun, array) {
    this.fun = fun;
    this.array = array;
}
Item.prototype.run = function () {
    this.fun.apply(null, this.array);
};
process.title = 'browser';
process.browser = true;
process.env = {};
process.argv = [];
process.version = ''; // empty string to avoid regexp issues
process.versions = {};

function noop() {}

process.on = noop;
process.addListener = noop;
process.once = noop;
process.off = noop;
process.removeListener = noop;
process.removeAllListeners = noop;
process.emit = noop;
process.prependListener = noop;
process.prependOnceListener = noop;

process.listeners = function (name) { return [] }

process.binding = function (name) {
    throw new Error('process.binding is not supported');
};

process.cwd = function () { return '/' };
process.chdir = function (dir) {
    throw new Error('process.chdir is not supported');
};
process.umask = function() { return 0; };

},{}],6:[function(require,module,exports){
if (typeof Object.create === 'function') {
  // implementation from standard node.js 'util' module
  module.exports = function inherits(ctor, superCtor) {
    ctor.super_ = superCtor
    ctor.prototype = Object.create(superCtor.prototype, {
      constructor: {
        value: ctor,
        enumerable: false,
        writable: true,
        configurable: true
      }
    });
  };
} else {
  // old school shim for old browsers
  module.exports = function inherits(ctor, superCtor) {
    ctor.super_ = superCtor
    var TempCtor = function () {}
    TempCtor.prototype = superCtor.prototype
    ctor.prototype = new TempCtor()
    ctor.prototype.constructor = ctor
  }
}

},{}],7:[function(require,module,exports){
module.exports = function isBuffer(arg) {
  return arg && typeof arg === 'object'
    && typeof arg.copy === 'function'
    && typeof arg.fill === 'function'
    && typeof arg.readUInt8 === 'function';
}
},{}],8:[function(require,module,exports){
(function (process,global){
// Copyright Joyent, Inc. and other Node contributors.
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the
// following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.

var formatRegExp = /%[sdj%]/g;
exports.format = function(f) {
  if (!isString(f)) {
    var objects = [];
    for (var i = 0; i < arguments.length; i++) {
      objects.push(inspect(arguments[i]));
    }
    return objects.join(' ');
  }

  var i = 1;
  var args = arguments;
  var len = args.length;
  var str = String(f).replace(formatRegExp, function(x) {
    if (x === '%%') return '%';
    if (i >= len) return x;
    switch (x) {
      case '%s': return String(args[i++]);
      case '%d': return Number(args[i++]);
      case '%j':
        try {
          return JSON.stringify(args[i++]);
        } catch (_) {
          return '[Circular]';
        }
      default:
        return x;
    }
  });
  for (var x = args[i]; i < len; x = args[++i]) {
    if (isNull(x) || !isObject(x)) {
      str += ' ' + x;
    } else {
      str += ' ' + inspect(x);
    }
  }
  return str;
};


// Mark that a method should not be used.
// Returns a modified function which warns once by default.
// If --no-deprecation is set, then it is a no-op.
exports.deprecate = function(fn, msg) {
  // Allow for deprecating things in the process of starting up.
  if (isUndefined(global.process)) {
    return function() {
      return exports.deprecate(fn, msg).apply(this, arguments);
    };
  }

  if (process.noDeprecation === true) {
    return fn;
  }

  var warned = false;
  function deprecated() {
    if (!warned) {
      if (process.throwDeprecation) {
        throw new Error(msg);
      } else if (process.traceDeprecation) {
        console.trace(msg);
      } else {
        console.error(msg);
      }
      warned = true;
    }
    return fn.apply(this, arguments);
  }

  return deprecated;
};


var debugs = {};
var debugEnviron;
exports.debuglog = function(set) {
  if (isUndefined(debugEnviron))
    debugEnviron = process.env.NODE_DEBUG || '';
  set = set.toUpperCase();
  if (!debugs[set]) {
    if (new RegExp('\\b' + set + '\\b', 'i').test(debugEnviron)) {
      var pid = process.pid;
      debugs[set] = function() {
        var msg = exports.format.apply(exports, arguments);
        console.error('%s %d: %s', set, pid, msg);
      };
    } else {
      debugs[set] = function() {};
    }
  }
  return debugs[set];
};


/**
 * Echos the value of a value. Trys to print the value out
 * in the best way possible given the different types.
 *
 * @param {Object} obj The object to print out.
 * @param {Object} opts Optional options object that alters the output.
 */
/* legacy: obj, showHidden, depth, colors*/
function inspect(obj, opts) {
  // default options
  var ctx = {
    seen: [],
    stylize: stylizeNoColor
  };
  // legacy...
  if (arguments.length >= 3) ctx.depth = arguments[2];
  if (arguments.length >= 4) ctx.colors = arguments[3];
  if (isBoolean(opts)) {
    // legacy...
    ctx.showHidden = opts;
  } else if (opts) {
    // got an "options" object
    exports._extend(ctx, opts);
  }
  // set default options
  if (isUndefined(ctx.showHidden)) ctx.showHidden = false;
  if (isUndefined(ctx.depth)) ctx.depth = 2;
  if (isUndefined(ctx.colors)) ctx.colors = false;
  if (isUndefined(ctx.customInspect)) ctx.customInspect = true;
  if (ctx.colors) ctx.stylize = stylizeWithColor;
  return formatValue(ctx, obj, ctx.depth);
}
exports.inspect = inspect;


// http://en.wikipedia.org/wiki/ANSI_escape_code#graphics
inspect.colors = {
  'bold' : [1, 22],
  'italic' : [3, 23],
  'underline' : [4, 24],
  'inverse' : [7, 27],
  'white' : [37, 39],
  'grey' : [90, 39],
  'black' : [30, 39],
  'blue' : [34, 39],
  'cyan' : [36, 39],
  'green' : [32, 39],
  'magenta' : [35, 39],
  'red' : [31, 39],
  'yellow' : [33, 39]
};

// Don't use 'blue' not visible on cmd.exe
inspect.styles = {
  'special': 'cyan',
  'number': 'yellow',
  'boolean': 'yellow',
  'undefined': 'grey',
  'null': 'bold',
  'string': 'green',
  'date': 'magenta',
  // "name": intentionally not styling
  'regexp': 'red'
};


function stylizeWithColor(str, styleType) {
  var style = inspect.styles[styleType];

  if (style) {
    return '\u001b[' + inspect.colors[style][0] + 'm' + str +
           '\u001b[' + inspect.colors[style][1] + 'm';
  } else {
    return str;
  }
}


function stylizeNoColor(str, styleType) {
  return str;
}


function arrayToHash(array) {
  var hash = {};

  array.forEach(function(val, idx) {
    hash[val] = true;
  });

  return hash;
}


function formatValue(ctx, value, recurseTimes) {
  // Provide a hook for user-specified inspect functions.
  // Check that value is an object with an inspect function on it
  if (ctx.customInspect &&
      value &&
      isFunction(value.inspect) &&
      // Filter out the util module, it's inspect function is special
      value.inspect !== exports.inspect &&
      // Also filter out any prototype objects using the circular check.
      !(value.constructor && value.constructor.prototype === value)) {
    var ret = value.inspect(recurseTimes, ctx);
    if (!isString(ret)) {
      ret = formatValue(ctx, ret, recurseTimes);
    }
    return ret;
  }

  // Primitive types cannot have properties
  var primitive = formatPrimitive(ctx, value);
  if (primitive) {
    return primitive;
  }

  // Look up the keys of the object.
  var keys = Object.keys(value);
  var visibleKeys = arrayToHash(keys);

  if (ctx.showHidden) {
    keys = Object.getOwnPropertyNames(value);
  }

  // IE doesn't make error fields non-enumerable
  // http://msdn.microsoft.com/en-us/library/ie/dww52sbt(v=vs.94).aspx
  if (isError(value)
      && (keys.indexOf('message') >= 0 || keys.indexOf('description') >= 0)) {
    return formatError(value);
  }

  // Some type of object without properties can be shortcutted.
  if (keys.length === 0) {
    if (isFunction(value)) {
      var name = value.name ? ': ' + value.name : '';
      return ctx.stylize('[Function' + name + ']', 'special');
    }
    if (isRegExp(value)) {
      return ctx.stylize(RegExp.prototype.toString.call(value), 'regexp');
    }
    if (isDate(value)) {
      return ctx.stylize(Date.prototype.toString.call(value), 'date');
    }
    if (isError(value)) {
      return formatError(value);
    }
  }

  var base = '', array = false, braces = ['{', '}'];

  // Make Array say that they are Array
  if (isArray(value)) {
    array = true;
    braces = ['[', ']'];
  }

  // Make functions say that they are functions
  if (isFunction(value)) {
    var n = value.name ? ': ' + value.name : '';
    base = ' [Function' + n + ']';
  }

  // Make RegExps say that they are RegExps
  if (isRegExp(value)) {
    base = ' ' + RegExp.prototype.toString.call(value);
  }

  // Make dates with properties first say the date
  if (isDate(value)) {
    base = ' ' + Date.prototype.toUTCString.call(value);
  }

  // Make error with message first say the error
  if (isError(value)) {
    base = ' ' + formatError(value);
  }

  if (keys.length === 0 && (!array || value.length == 0)) {
    return braces[0] + base + braces[1];
  }

  if (recurseTimes < 0) {
    if (isRegExp(value)) {
      return ctx.stylize(RegExp.prototype.toString.call(value), 'regexp');
    } else {
      return ctx.stylize('[Object]', 'special');
    }
  }

  ctx.seen.push(value);

  var output;
  if (array) {
    output = formatArray(ctx, value, recurseTimes, visibleKeys, keys);
  } else {
    output = keys.map(function(key) {
      return formatProperty(ctx, value, recurseTimes, visibleKeys, key, array);
    });
  }

  ctx.seen.pop();

  return reduceToSingleString(output, base, braces);
}


function formatPrimitive(ctx, value) {
  if (isUndefined(value))
    return ctx.stylize('undefined', 'undefined');
  if (isString(value)) {
    var simple = '\'' + JSON.stringify(value).replace(/^"|"$/g, '')
                                             .replace(/'/g, "\\'")
                                             .replace(/\\"/g, '"') + '\'';
    return ctx.stylize(simple, 'string');
  }
  if (isNumber(value))
    return ctx.stylize('' + value, 'number');
  if (isBoolean(value))
    return ctx.stylize('' + value, 'boolean');
  // For some reason typeof null is "object", so special case here.
  if (isNull(value))
    return ctx.stylize('null', 'null');
}


function formatError(value) {
  return '[' + Error.prototype.toString.call(value) + ']';
}


function formatArray(ctx, value, recurseTimes, visibleKeys, keys) {
  var output = [];
  for (var i = 0, l = value.length; i < l; ++i) {
    if (hasOwnProperty(value, String(i))) {
      output.push(formatProperty(ctx, value, recurseTimes, visibleKeys,
          String(i), true));
    } else {
      output.push('');
    }
  }
  keys.forEach(function(key) {
    if (!key.match(/^\d+$/)) {
      output.push(formatProperty(ctx, value, recurseTimes, visibleKeys,
          key, true));
    }
  });
  return output;
}


function formatProperty(ctx, value, recurseTimes, visibleKeys, key, array) {
  var name, str, desc;
  desc = Object.getOwnPropertyDescriptor(value, key) || { value: value[key] };
  if (desc.get) {
    if (desc.set) {
      str = ctx.stylize('[Getter/Setter]', 'special');
    } else {
      str = ctx.stylize('[Getter]', 'special');
    }
  } else {
    if (desc.set) {
      str = ctx.stylize('[Setter]', 'special');
    }
  }
  if (!hasOwnProperty(visibleKeys, key)) {
    name = '[' + key + ']';
  }
  if (!str) {
    if (ctx.seen.indexOf(desc.value) < 0) {
      if (isNull(recurseTimes)) {
        str = formatValue(ctx, desc.value, null);
      } else {
        str = formatValue(ctx, desc.value, recurseTimes - 1);
      }
      if (str.indexOf('\n') > -1) {
        if (array) {
          str = str.split('\n').map(function(line) {
            return '  ' + line;
          }).join('\n').substr(2);
        } else {
          str = '\n' + str.split('\n').map(function(line) {
            return '   ' + line;
          }).join('\n');
        }
      }
    } else {
      str = ctx.stylize('[Circular]', 'special');
    }
  }
  if (isUndefined(name)) {
    if (array && key.match(/^\d+$/)) {
      return str;
    }
    name = JSON.stringify('' + key);
    if (name.match(/^"([a-zA-Z_][a-zA-Z_0-9]*)"$/)) {
      name = name.substr(1, name.length - 2);
      name = ctx.stylize(name, 'name');
    } else {
      name = name.replace(/'/g, "\\'")
                 .replace(/\\"/g, '"')
                 .replace(/(^"|"$)/g, "'");
      name = ctx.stylize(name, 'string');
    }
  }

  return name + ': ' + str;
}


function reduceToSingleString(output, base, braces) {
  var numLinesEst = 0;
  var length = output.reduce(function(prev, cur) {
    numLinesEst++;
    if (cur.indexOf('\n') >= 0) numLinesEst++;
    return prev + cur.replace(/\u001b\[\d\d?m/g, '').length + 1;
  }, 0);

  if (length > 60) {
    return braces[0] +
           (base === '' ? '' : base + '\n ') +
           ' ' +
           output.join(',\n  ') +
           ' ' +
           braces[1];
  }

  return braces[0] + base + ' ' + output.join(', ') + ' ' + braces[1];
}


// NOTE: These type checking functions intentionally don't use `instanceof`
// because it is fragile and can be easily faked with `Object.create()`.
function isArray(ar) {
  return Array.isArray(ar);
}
exports.isArray = isArray;

function isBoolean(arg) {
  return typeof arg === 'boolean';
}
exports.isBoolean = isBoolean;

function isNull(arg) {
  return arg === null;
}
exports.isNull = isNull;

function isNullOrUndefined(arg) {
  return arg == null;
}
exports.isNullOrUndefined = isNullOrUndefined;

function isNumber(arg) {
  return typeof arg === 'number';
}
exports.isNumber = isNumber;

function isString(arg) {
  return typeof arg === 'string';
}
exports.isString = isString;

function isSymbol(arg) {
  return typeof arg === 'symbol';
}
exports.isSymbol = isSymbol;

function isUndefined(arg) {
  return arg === void 0;
}
exports.isUndefined = isUndefined;

function isRegExp(re) {
  return isObject(re) && objectToString(re) === '[object RegExp]';
}
exports.isRegExp = isRegExp;

function isObject(arg) {
  return typeof arg === 'object' && arg !== null;
}
exports.isObject = isObject;

function isDate(d) {
  return isObject(d) && objectToString(d) === '[object Date]';
}
exports.isDate = isDate;

function isError(e) {
  return isObject(e) &&
      (objectToString(e) === '[object Error]' || e instanceof Error);
}
exports.isError = isError;

function isFunction(arg) {
  return typeof arg === 'function';
}
exports.isFunction = isFunction;

function isPrimitive(arg) {
  return arg === null ||
         typeof arg === 'boolean' ||
         typeof arg === 'number' ||
         typeof arg === 'string' ||
         typeof arg === 'symbol' ||  // ES6 symbol
         typeof arg === 'undefined';
}
exports.isPrimitive = isPrimitive;

exports.isBuffer = require('./support/isBuffer');

function objectToString(o) {
  return Object.prototype.toString.call(o);
}


function pad(n) {
  return n < 10 ? '0' + n.toString(10) : n.toString(10);
}


var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep',
              'Oct', 'Nov', 'Dec'];

// 26 Feb 16:19:34
function timestamp() {
  var d = new Date();
  var time = [pad(d.getHours()),
              pad(d.getMinutes()),
              pad(d.getSeconds())].join(':');
  return [d.getDate(), months[d.getMonth()], time].join(' ');
}


// log is just a thin wrapper to console.log that prepends a timestamp
exports.log = function() {
  console.log('%s - %s', timestamp(), exports.format.apply(exports, arguments));
};


/**
 * Inherit the prototype methods from one constructor into another.
 *
 * The Function.prototype.inherits from lang.js rewritten as a standalone
 * function (not on Function.prototype). NOTE: If this file is to be loaded
 * during bootstrapping this function needs to be rewritten using some native
 * functions as prototype setup using normal JavaScript does not work as
 * expected during bootstrapping (see mirror.js in r114903).
 *
 * @param {function} ctor Constructor function which needs to inherit the
 *     prototype.
 * @param {function} superCtor Constructor function to inherit prototype from.
 */
exports.inherits = require('inherits');

exports._extend = function(origin, add) {
  // Don't do anything if add isn't an object
  if (!add || !isObject(add)) return origin;

  var keys = Object.keys(add);
  var i = keys.length;
  while (i--) {
    origin[keys[i]] = add[keys[i]];
  }
  return origin;
};

function hasOwnProperty(obj, prop) {
  return Object.prototype.hasOwnProperty.call(obj, prop);
}

}).call(this,require('_process'),typeof global !== "undefined" ? global : typeof self !== "undefined" ? self : typeof window !== "undefined" ? window : {})
},{"./support/isBuffer":7,"_process":5,"inherits":6}],9:[function(require,module,exports){
/*
MAVLink protocol implementation for node.js (auto-generated by mavgen_javascript.py)

Generated from: ardupilotmega.xml,common.xml

Note: this file has been auto-generated. DO NOT EDIT
*/

jspack = require("jspack").jspack;
    _ = require("underscore");
    events = require("events");
    util = require("util");

Buffer = require('buffer').Buffer;

// Add a convenience method to Buffer
Buffer.prototype.toByteArray = function () {
  return Array.prototype.slice.call(this, 0)
}

mavlink = function(){};

// Implement the X25CRC function (present in the Python version through the mavutil.py package)
mavlink.x25Crc = function(buffer, crc) {

    var bytes = buffer;
    var crc = crc || 0xffff;
    _.each(bytes, function(e) {
        var tmp = e ^ (crc & 0xff);
        tmp = (tmp ^ (tmp << 4)) & 0xff;
        crc = (crc >> 8) ^ (tmp << 8) ^ (tmp << 3) ^ (tmp >> 4);
        crc = crc & 0xffff;
    });
    return crc;

}

mavlink.WIRE_PROTOCOL_VERSION = "2.0";

mavlink.MAVLINK_TYPE_CHAR     = 0
mavlink.MAVLINK_TYPE_UINT8_T  = 1
mavlink.MAVLINK_TYPE_INT8_T   = 2
mavlink.MAVLINK_TYPE_UINT16_T = 3
mavlink.MAVLINK_TYPE_INT16_T  = 4
mavlink.MAVLINK_TYPE_UINT32_T = 5
mavlink.MAVLINK_TYPE_INT32_T  = 6
mavlink.MAVLINK_TYPE_UINT64_T = 7
mavlink.MAVLINK_TYPE_INT64_T  = 8
mavlink.MAVLINK_TYPE_FLOAT    = 9
mavlink.MAVLINK_TYPE_DOUBLE   = 10

// Mavlink headers incorporate sequence, source system (platform) and source component. 
mavlink.header = function(msgId, mlen, seq, srcSystem, srcComponent) {

    this.mlen = ( typeof mlen === 'undefined' ) ? 0 : mlen;
    this.seq = ( typeof seq === 'undefined' ) ? 0 : seq;
    this.srcSystem = ( typeof srcSystem === 'undefined' ) ? 0 : srcSystem;
    this.srcComponent = ( typeof srcComponent === 'undefined' ) ? 0 : srcComponent;
    this.msgId = msgId

}

mavlink.header.prototype.pack = function() {
    return jspack.Pack('BBBBBB', [253, this.mlen, this.seq, this.srcSystem, this.srcComponent, this.msgId]);
}

// Base class declaration: mavlink.message will be the parent class for each
// concrete implementation in mavlink.messages.
mavlink.message = function() {};

// Convenience setter to facilitate turning the unpacked array of data into member properties
mavlink.message.prototype.set = function(args) {
    _.each(this.fieldnames, function(e, i) {
        this[e] = args[i];
    }, this);
};

// This pack function builds the header and produces a complete MAVLink message,
// including header and message CRC.
mavlink.message.prototype.pack = function(mav, crc_extra, payload) {

    this.payload = payload;
    this.header = new mavlink.header(this.id, payload.length, mav.seq, mav.srcSystem, mav.srcComponent);    
    this.msgbuf = this.header.pack().concat(payload);
    var crc = mavlink.x25Crc(this.msgbuf.slice(1));

    // For now, assume always using crc_extra = True.  TODO: check/fix this.
    crc = mavlink.x25Crc([crc_extra], crc);
    this.msgbuf = this.msgbuf.concat(jspack.Pack('<H', [crc] ) );
    return this.msgbuf;

}


// enums

// MAV_CMD
mavlink.MAV_CMD_NAV_WAYPOINT = 16 // Navigate to MISSION.
mavlink.MAV_CMD_NAV_LOITER_UNLIM = 17 // Loiter around this MISSION an unlimited amount of time
mavlink.MAV_CMD_NAV_LOITER_TURNS = 18 // Loiter around this MISSION for X turns
mavlink.MAV_CMD_NAV_LOITER_TIME = 19 // Loiter around this MISSION for X seconds
mavlink.MAV_CMD_NAV_RETURN_TO_LAUNCH = 20 // Return to launch location
mavlink.MAV_CMD_NAV_LAND = 21 // Land at location
mavlink.MAV_CMD_NAV_TAKEOFF = 22 // Takeoff from ground / hand
mavlink.MAV_CMD_NAV_CONTINUE_AND_CHANGE_ALT = 30 // Continue on the current course and climb/descend to specified
                        // altitude.  When the altitude is reached
                        // continue to the next command (i.e., don't
                        // proceed to the next command until the
                        // desired altitude is reached.
mavlink.MAV_CMD_NAV_ROI = 80 // Sets the region of interest (ROI) for a sensor set or the vehicle
                        // itself. This can then be used by the
                        // vehicles control system to control the
                        // vehicle attitude and the attitude of
                        // various sensors such as cameras.
mavlink.MAV_CMD_NAV_PATHPLANNING = 81 // Control autonomous path planning on the MAV.
mavlink.MAV_CMD_NAV_SPLINE_WAYPOINT = 82 // Navigate to MISSION using a spline path.
mavlink.MAV_CMD_NAV_GUIDED_ENABLE = 92 // hand control over to an external controller
mavlink.MAV_CMD_NAV_LAST = 95 // NOP - This command is only used to mark the upper limit of the
                        // NAV/ACTION commands in the enumeration
mavlink.MAV_CMD_CONDITION_DELAY = 112 // Delay mission state machine.
mavlink.MAV_CMD_CONDITION_CHANGE_ALT = 113 // Ascend/descend at rate.  Delay mission state machine until desired
                        // altitude reached.
mavlink.MAV_CMD_CONDITION_DISTANCE = 114 // Delay mission state machine until within desired distance of next NAV
                        // point.
mavlink.MAV_CMD_CONDITION_YAW = 115 // Reach a certain target angle.
mavlink.MAV_CMD_CONDITION_LAST = 159 // NOP - This command is only used to mark the upper limit of the
                        // CONDITION commands in the enumeration
mavlink.MAV_CMD_DO_SET_MODE = 176 // Set system mode.
mavlink.MAV_CMD_DO_JUMP = 177 // Jump to the desired command in the mission list.  Repeat this action
                        // only the specified number of times
mavlink.MAV_CMD_DO_CHANGE_SPEED = 178 // Change speed and/or throttle set points.
mavlink.MAV_CMD_DO_SET_HOME = 179 // Changes the home location either to the current location or a
                        // specified location.
mavlink.MAV_CMD_DO_SET_PARAMETER = 180 // Set a system parameter.  Caution!  Use of this command requires
                        // knowledge of the numeric enumeration value
                        // of the parameter.
mavlink.MAV_CMD_DO_SET_RELAY = 181 // Set a relay to a condition.
mavlink.MAV_CMD_DO_REPEAT_RELAY = 182 // Cycle a relay on and off for a desired number of cyles with a desired
                        // period.
mavlink.MAV_CMD_DO_SET_SERVO = 183 // Set a servo to a desired PWM value.
mavlink.MAV_CMD_DO_REPEAT_SERVO = 184 // Cycle a between its nominal setting and a desired PWM for a desired
                        // number of cycles with a desired period.
mavlink.MAV_CMD_DO_FLIGHTTERMINATION = 185 // Terminate flight immediately
mavlink.MAV_CMD_DO_LAND_START = 189 // Mission command to perform a landing. This is used as a marker in a
                        // mission to tell the autopilot where a
                        // sequence of mission items that represents a
                        // landing starts. It may also be sent via a
                        // COMMAND_LONG to trigger a landing, in which
                        // case the nearest (geographically) landing
                        // sequence in the mission will be used. The
                        // Latitude/Longitude is optional, and may be
                        // set to 0/0 if not needed. If specified then
                        // it will be used to help find the closest
                        // landing sequence.
mavlink.MAV_CMD_DO_RALLY_LAND = 190 // Mission command to perform a landing from a rally point.
mavlink.MAV_CMD_DO_GO_AROUND = 191 // Mission command to safely abort an autonmous landing.
mavlink.MAV_CMD_DO_CONTROL_VIDEO = 200 // Control onboard camera system.
mavlink.MAV_CMD_DO_SET_ROI = 201 // Sets the region of interest (ROI) for a sensor set or the vehicle
                        // itself. This can then be used by the
                        // vehicles control system to control the
                        // vehicle attitude and the attitude of
                        // various sensors such as cameras.
mavlink.MAV_CMD_DO_DIGICAM_CONFIGURE = 202 // Mission command to configure an on-board camera controller system.
mavlink.MAV_CMD_DO_DIGICAM_CONTROL = 203 // Mission command to control an on-board camera controller system.
mavlink.MAV_CMD_DO_MOUNT_CONFIGURE = 204 // Mission command to configure a camera or antenna mount
mavlink.MAV_CMD_DO_MOUNT_CONTROL = 205 // Mission command to control a camera or antenna mount
mavlink.MAV_CMD_DO_SET_CAM_TRIGG_DIST = 206 // Mission command to set CAM_TRIGG_DIST for this flight
mavlink.MAV_CMD_DO_FENCE_ENABLE = 207 // Mission command to enable the geofence
mavlink.MAV_CMD_DO_PARACHUTE = 208 // Mission command to trigger a parachute
mavlink.MAV_CMD_DO_MOTOR_TEST = 209 // Mission command to perform motor test
mavlink.MAV_CMD_DO_INVERTED_FLIGHT = 210 // Change to/from inverted flight
mavlink.MAV_CMD_DO_GRIPPER = 211 // Mission command to operate EPM gripper
mavlink.MAV_CMD_DO_MOUNT_CONTROL_QUAT = 220 // Mission command to control a camera or antenna mount, using a
                        // quaternion as reference.
mavlink.MAV_CMD_DO_GUIDED_MASTER = 221 // set id of master controller
mavlink.MAV_CMD_DO_GUIDED_LIMITS = 222 // set limits for external control
mavlink.MAV_CMD_DO_LAST = 240 // NOP - This command is only used to mark the upper limit of the DO
                        // commands in the enumeration
mavlink.MAV_CMD_PREFLIGHT_CALIBRATION = 241 // Trigger calibration. This command will be only accepted if in pre-
                        // flight mode.
mavlink.MAV_CMD_PREFLIGHT_SET_SENSOR_OFFSETS = 242 // Set sensor offsets. This command will be only accepted if in pre-
                        // flight mode.
mavlink.MAV_CMD_PREFLIGHT_STORAGE = 245 // Request storage of different parameter values and logs. This command
                        // will be only accepted if in pre-flight
                        // mode.
mavlink.MAV_CMD_PREFLIGHT_REBOOT_SHUTDOWN = 246 // Request the reboot or shutdown of system components.
mavlink.MAV_CMD_OVERRIDE_GOTO = 252 // Hold / continue the current action
mavlink.MAV_CMD_MISSION_START = 300 // start running a mission
mavlink.MAV_CMD_COMPONENT_ARM_DISARM = 400 // Arms / Disarms a component
mavlink.MAV_CMD_START_RX_PAIR = 500 // Starts receiver pairing
mavlink.MAV_CMD_REQUEST_AUTOPILOT_CAPABILITIES = 520 // Request autopilot capabilities
mavlink.MAV_CMD_IMAGE_START_CAPTURE = 2000 // Start image capture sequence
mavlink.MAV_CMD_IMAGE_STOP_CAPTURE = 2001 // Stop image capture sequence
mavlink.MAV_CMD_VIDEO_START_CAPTURE = 2500 // Starts video capture
mavlink.MAV_CMD_VIDEO_STOP_CAPTURE = 2501 // Stop the current video capture
mavlink.MAV_CMD_PANORAMA_CREATE = 2800 // Create a panorama at the current position
mavlink.MAV_CMD_PAYLOAD_PREPARE_DEPLOY = 30001 // Deploy payload on a Lat / Lon / Alt position. This includes the
                        // navigation to reach the required release
                        // position and velocity.
mavlink.MAV_CMD_PAYLOAD_CONTROL_DEPLOY = 30002 // Control the payload deployment.
mavlink.MAV_CMD_DO_START_MAG_CAL = 42424 // Initiate a magnetometer calibration
mavlink.MAV_CMD_DO_ACCEPT_MAG_CAL = 42425 // Initiate a magnetometer calibration
mavlink.MAV_CMD_DO_CANCEL_MAG_CAL = 42426 // Cancel a running magnetometer calibration
mavlink.MAV_CMD_ENUM_END = 42427 // 

// LIMITS_STATE
mavlink.LIMITS_INIT = 0 //  pre-initialization
mavlink.LIMITS_DISABLED = 1 //  disabled
mavlink.LIMITS_ENABLED = 2 //  checking limits
mavlink.LIMITS_TRIGGERED = 3 //  a limit has been breached
mavlink.LIMITS_RECOVERING = 4 //  taking action eg. RTL
mavlink.LIMITS_RECOVERED = 5 //  we're no longer in breach of a limit
mavlink.LIMITS_STATE_ENUM_END = 6 // 

// LIMIT_MODULE
mavlink.LIMIT_GPSLOCK = 1 //  pre-initialization
mavlink.LIMIT_GEOFENCE = 2 //  disabled
mavlink.LIMIT_ALTITUDE = 4 //  checking limits
mavlink.LIMIT_MODULE_ENUM_END = 5 // 

// RALLY_FLAGS
mavlink.FAVORABLE_WIND = 1 // Flag set when requiring favorable winds for landing.
mavlink.LAND_IMMEDIATELY = 2 // Flag set when plane is to immediately descend to break altitude and
                        // land without GCS intervention.  Flag not
                        // set when plane is to loiter at Rally point
                        // until commanded to land.
mavlink.RALLY_FLAGS_ENUM_END = 3 // 

// PARACHUTE_ACTION
mavlink.PARACHUTE_DISABLE = 0 // Disable parachute release
mavlink.PARACHUTE_ENABLE = 1 // Enable parachute release
mavlink.PARACHUTE_RELEASE = 2 // Release parachute
mavlink.PARACHUTE_ACTION_ENUM_END = 3 // 

// MOTOR_TEST_THROTTLE_TYPE
mavlink.MOTOR_TEST_THROTTLE_PERCENT = 0 // throttle as a percentage from 0 ~ 100
mavlink.MOTOR_TEST_THROTTLE_PWM = 1 // throttle as an absolute PWM value (normally in range of 1000~2000)
mavlink.MOTOR_TEST_THROTTLE_PILOT = 2 // throttle pass-through from pilot's transmitter
mavlink.MOTOR_TEST_THROTTLE_TYPE_ENUM_END = 3 // 

// GRIPPER_ACTIONS
mavlink.GRIPPER_ACTION_RELEASE = 0 // gripper release of cargo
mavlink.GRIPPER_ACTION_GRAB = 1 // gripper grabs onto cargo
mavlink.GRIPPER_ACTIONS_ENUM_END = 2 // 

// CAMERA_STATUS_TYPES
mavlink.CAMERA_STATUS_TYPE_HEARTBEAT = 0 // Camera heartbeat, announce camera component ID at 1hz
mavlink.CAMERA_STATUS_TYPE_TRIGGER = 1 // Camera image triggered
mavlink.CAMERA_STATUS_TYPE_DISCONNECT = 2 // Camera connection lost
mavlink.CAMERA_STATUS_TYPE_ERROR = 3 // Camera unknown error
mavlink.CAMERA_STATUS_TYPE_LOWBATT = 4 // Camera battery low. Parameter p1 shows reported voltage
mavlink.CAMERA_STATUS_TYPE_LOWSTORE = 5 // Camera storage low. Parameter p1 shows reported shots remaining
mavlink.CAMERA_STATUS_TYPE_LOWSTOREV = 6 // Camera storage low. Parameter p1 shows reported video minutes
                        // remaining
mavlink.CAMERA_STATUS_TYPES_ENUM_END = 7 // 

// CAMERA_FEEDBACK_FLAGS
mavlink.VIDEO = 1 // Shooting video, not stills
mavlink.BADEXPOSURE = 2 // Unable to achieve requested exposure (e.g. shutter speed too low)
mavlink.CLOSEDLOOP = 3 // Closed loop feedback from camera, we know for sure it has successfully
                        // taken a picture
mavlink.OPENLOOP = 4 // Open loop camera, an image trigger has been requested but we can't
                        // know for sure it has successfully taken a
                        // picture
mavlink.CAMERA_FEEDBACK_FLAGS_ENUM_END = 5 // 

// MAV_MODE_GIMBAL
mavlink.MAV_MODE_GIMBAL_UNINITIALIZED = 0 // Gimbal is powered on but has not started initializing yet
mavlink.MAV_MODE_GIMBAL_CALIBRATING_PITCH = 1 // Gimbal is currently running calibration on the pitch axis
mavlink.MAV_MODE_GIMBAL_CALIBRATING_ROLL = 2 // Gimbal is currently running calibration on the roll axis
mavlink.MAV_MODE_GIMBAL_CALIBRATING_YAW = 3 // Gimbal is currently running calibration on the yaw axis
mavlink.MAV_MODE_GIMBAL_INITIALIZED = 4 // Gimbal has finished calibrating and initializing, but is relaxed
                        // pending reception of first rate command
                        // from copter
mavlink.MAV_MODE_GIMBAL_ACTIVE = 5 // Gimbal is actively stabilizing
mavlink.MAV_MODE_GIMBAL_RATE_CMD_TIMEOUT = 6 // Gimbal is relaxed because it missed more than 10 expected rate command
                        // messages in a row.  Gimbal will move back
                        // to active mode when it receives a new rate
                        // command
mavlink.MAV_MODE_GIMBAL_ENUM_END = 7 // 

// GIMBAL_AXIS
mavlink.GIMBAL_AXIS_YAW = 0 // Gimbal yaw axis
mavlink.GIMBAL_AXIS_PITCH = 1 // Gimbal pitch axis
mavlink.GIMBAL_AXIS_ROLL = 2 // Gimbal roll axis
mavlink.GIMBAL_AXIS_ENUM_END = 3 // 

// GIMBAL_AXIS_CALIBRATION_STATUS
mavlink.GIMBAL_AXIS_CALIBRATION_STATUS_IN_PROGRESS = 0 // Axis calibration is in progress
mavlink.GIMBAL_AXIS_CALIBRATION_STATUS_SUCCEEDED = 1 // Axis calibration succeeded
mavlink.GIMBAL_AXIS_CALIBRATION_STATUS_FAILED = 2 // Axis calibration failed
mavlink.GIMBAL_AXIS_CALIBRATION_STATUS_ENUM_END = 3 // 

// FACTORY_TEST
mavlink.FACTORY_TEST_AXIS_RANGE_LIMITS = 0 // Tests to make sure each axis can move to its mechanical limits
mavlink.FACTORY_TEST_ENUM_END = 1 // 

// GOPRO_CMD_RESULT
mavlink.GOPRO_CMD_RESULT_UNKNOWN = 0 // The result of the command is unknown
mavlink.GOPRO_CMD_RESULT_SUCCESSFUL = 1 // The command was successfully sent, and a response was successfully
                        // received
mavlink.GOPRO_CMD_RESULT_SEND_CMD_START_TIMEOUT = 2 // Timed out waiting for the GoPro to acknowledge our request to send a
                        // command
mavlink.GOPRO_CMD_RESULT_SEND_CMD_COMPLETE_TIMEOUT = 3 // Timed out waiting for the GoPro to read the command
mavlink.GOPRO_CMD_RESULT_GET_RESPONSE_START_TIMEOUT = 4 // Timed out waiting for the GoPro to begin transmitting a response to
                        // the command
mavlink.GOPRO_CMD_RESULT_GET_RESPONSE_COMPLETE_TIMEOUT = 5 // Timed out waiting for the GoPro to finish transmitting a response to
                        // the command
mavlink.GOPRO_CMD_RESULT_GET_CMD_COMPLETE_TIMEOUT = 6 // Timed out waiting for the GoPro to finish transmitting its own command
mavlink.GOPRO_CMD_RESULT_SEND_RESPONSE_START_TIMEOUT = 7 // Timed out waiting for the GoPro to start reading a response to its own
                        // command
mavlink.GOPRO_CMD_RESULT_SEND_RESPONSE_COMPLETE_TIMEOUT = 8 // Timed out waiting for the GoPro to finish reading a response to its
                        // own command
mavlink.GOPRO_CMD_RESULT_PREEMPTED = 9 // Command to the GoPro was preempted by the GoPro sending its own
                        // command
mavlink.GOPRO_CMD_RECEIVED_DATA_OVERFLOW = 10 // More data than expected received in response to the command
mavlink.GOPRO_CMD_RECEIVED_DATA_UNDERFLOW = 11 // Less data than expected received in response to the command
mavlink.GOPRO_CMD_RESULT_ENUM_END = 12 // 

// LED_CONTROL_PATTERN
mavlink.LED_CONTROL_PATTERN_OFF = 0 // LED patterns off (return control to regular vehicle control)
mavlink.LED_CONTROL_PATTERN_FIRMWAREUPDATE = 1 // LEDs show pattern during firmware update
mavlink.LED_CONTROL_PATTERN_CUSTOM = 255 // Custom Pattern using custom bytes fields
mavlink.LED_CONTROL_PATTERN_ENUM_END = 256 // 

// EKF_STATUS_FLAGS
mavlink.EKF_ATTITUDE = 1 // set if EKF's attitude estimate is good
mavlink.EKF_VELOCITY_HORIZ = 2 // set if EKF's horizontal velocity estimate is good
mavlink.EKF_VELOCITY_VERT = 4 // set if EKF's vertical velocity estimate is good
mavlink.EKF_POS_HORIZ_REL = 8 // set if EKF's horizontal position (relative) estimate is good
mavlink.EKF_POS_HORIZ_ABS = 16 // set if EKF's horizontal position (absolute) estimate is good
mavlink.EKF_POS_VERT_ABS = 32 // set if EKF's vertical position (absolute) estimate is good
mavlink.EKF_POS_VERT_AGL = 64 // set if EKF's vertical position (above ground) estimate is good
mavlink.EKF_CONST_POS_MODE = 128 // EKF is in constant position mode and does not know it's absolute or
                        // relative position
mavlink.EKF_PRED_POS_HORIZ_REL = 256 // set if EKF's predicted horizontal position (relative) estimate is good
mavlink.EKF_PRED_POS_HORIZ_ABS = 512 // set if EKF's predicted horizontal position (absolute) estimate is good
mavlink.EKF_STATUS_FLAGS_ENUM_END = 513 // 

// MAG_CAL_STATUS
mavlink.MAG_CAL_NOT_STARTED = 0 // 
mavlink.MAG_CAL_WAITING_TO_START = 1 // 
mavlink.MAG_CAL_RUNNING_STEP_ONE = 2 // 
mavlink.MAG_CAL_RUNNING_STEP_TWO = 3 // 
mavlink.MAG_CAL_SUCCESS = 4 // 
mavlink.MAG_CAL_FAILED = 5 // 
mavlink.MAG_CAL_STATUS_ENUM_END = 6 // 

// MAV_AUTOPILOT
mavlink.MAV_AUTOPILOT_GENERIC = 0 // Generic autopilot, full support for everything
mavlink.MAV_AUTOPILOT_PIXHAWK = 1 // PIXHAWK autopilot, http://pixhawk.ethz.ch
mavlink.MAV_AUTOPILOT_SLUGS = 2 // SLUGS autopilot, http://slugsuav.soe.ucsc.edu
mavlink.MAV_AUTOPILOT_ARDUPILOTMEGA = 3 // ArduPilotMega / ArduCopter, http://diydrones.com
mavlink.MAV_AUTOPILOT_OPENPILOT = 4 // OpenPilot, http://openpilot.org
mavlink.MAV_AUTOPILOT_GENERIC_WAYPOINTS_ONLY = 5 // Generic autopilot only supporting simple waypoints
mavlink.MAV_AUTOPILOT_GENERIC_WAYPOINTS_AND_SIMPLE_NAVIGATION_ONLY = 6 // Generic autopilot supporting waypoints and other simple navigation
                        // commands
mavlink.MAV_AUTOPILOT_GENERIC_MISSION_FULL = 7 // Generic autopilot supporting the full mission command set
mavlink.MAV_AUTOPILOT_INVALID = 8 // No valid autopilot, e.g. a GCS or other MAVLink component
mavlink.MAV_AUTOPILOT_PPZ = 9 // PPZ UAV - http://nongnu.org/paparazzi
mavlink.MAV_AUTOPILOT_UDB = 10 // UAV Dev Board
mavlink.MAV_AUTOPILOT_FP = 11 // FlexiPilot
mavlink.MAV_AUTOPILOT_PX4 = 12 // PX4 Autopilot - http://pixhawk.ethz.ch/px4/
mavlink.MAV_AUTOPILOT_SMACCMPILOT = 13 // SMACCMPilot - http://smaccmpilot.org
mavlink.MAV_AUTOPILOT_AUTOQUAD = 14 // AutoQuad -- http://autoquad.org
mavlink.MAV_AUTOPILOT_ARMAZILA = 15 // Armazila -- http://armazila.com
mavlink.MAV_AUTOPILOT_AEROB = 16 // Aerob -- http://aerob.ru
mavlink.MAV_AUTOPILOT_ASLUAV = 17 // ASLUAV autopilot -- http://www.asl.ethz.ch
mavlink.MAV_AUTOPILOT_ENUM_END = 18 // 

// MAV_TYPE
mavlink.MAV_TYPE_GENERIC = 0 // Generic micro air vehicle.
mavlink.MAV_TYPE_FIXED_WING = 1 // Fixed wing aircraft.
mavlink.MAV_TYPE_QUADROTOR = 2 // Quadrotor
mavlink.MAV_TYPE_COAXIAL = 3 // Coaxial helicopter
mavlink.MAV_TYPE_HELICOPTER = 4 // Normal helicopter with tail rotor.
mavlink.MAV_TYPE_ANTENNA_TRACKER = 5 // Ground installation
mavlink.MAV_TYPE_GCS = 6 // Operator control unit / ground control station
mavlink.MAV_TYPE_AIRSHIP = 7 // Airship, controlled
mavlink.MAV_TYPE_FREE_BALLOON = 8 // Free balloon, uncontrolled
mavlink.MAV_TYPE_ROCKET = 9 // Rocket
mavlink.MAV_TYPE_GROUND_ROVER = 10 // Ground rover
mavlink.MAV_TYPE_SURFACE_BOAT = 11 // Surface vessel, boat, ship
mavlink.MAV_TYPE_SUBMARINE = 12 // Submarine
mavlink.MAV_TYPE_HEXAROTOR = 13 // Hexarotor
mavlink.MAV_TYPE_OCTOROTOR = 14 // Octorotor
mavlink.MAV_TYPE_TRICOPTER = 15 // Octorotor
mavlink.MAV_TYPE_FLAPPING_WING = 16 // Flapping wing
mavlink.MAV_TYPE_KITE = 17 // Flapping wing
mavlink.MAV_TYPE_ONBOARD_CONTROLLER = 18 // Onboard companion controller
mavlink.MAV_TYPE_VTOL_DUOROTOR = 19 // Two-rotor VTOL using control surfaces in vertical operation in
                        // addition. Tailsitter.
mavlink.MAV_TYPE_VTOL_QUADROTOR = 20 // Quad-rotor VTOL using a V-shaped quad config in vertical operation.
                        // Tailsitter.
mavlink.MAV_TYPE_VTOL_RESERVED1 = 21 // VTOL reserved 1
mavlink.MAV_TYPE_VTOL_RESERVED2 = 22 // VTOL reserved 2
mavlink.MAV_TYPE_VTOL_RESERVED3 = 23 // VTOL reserved 3
mavlink.MAV_TYPE_VTOL_RESERVED4 = 24 // VTOL reserved 4
mavlink.MAV_TYPE_VTOL_RESERVED5 = 25 // VTOL reserved 5
mavlink.MAV_TYPE_GIMBAL = 26 // Onboard gimbal
mavlink.MAV_TYPE_ENUM_END = 27 // 

// MAV_MODE_FLAG
mavlink.MAV_MODE_FLAG_CUSTOM_MODE_ENABLED = 1 // 0b00000001 Reserved for future use.
mavlink.MAV_MODE_FLAG_TEST_ENABLED = 2 // 0b00000010 system has a test mode enabled. This flag is intended for
                        // temporary system tests and should not be
                        // used for stable implementations.
mavlink.MAV_MODE_FLAG_AUTO_ENABLED = 4 // 0b00000100 autonomous mode enabled, system finds its own goal
                        // positions. Guided flag can be set or not,
                        // depends on the actual implementation.
mavlink.MAV_MODE_FLAG_GUIDED_ENABLED = 8 // 0b00001000 guided mode enabled, system flies MISSIONs / mission items.
mavlink.MAV_MODE_FLAG_STABILIZE_ENABLED = 16 // 0b00010000 system stabilizes electronically its attitude (and
                        // optionally position). It needs however
                        // further control inputs to move around.
mavlink.MAV_MODE_FLAG_HIL_ENABLED = 32 // 0b00100000 hardware in the loop simulation. All motors / actuators are
                        // blocked, but internal software is full
                        // operational.
mavlink.MAV_MODE_FLAG_MANUAL_INPUT_ENABLED = 64 // 0b01000000 remote control input is enabled.
mavlink.MAV_MODE_FLAG_SAFETY_ARMED = 128 // 0b10000000 MAV safety set to armed. Motors are enabled / running / can
                        // start. Ready to fly.
mavlink.MAV_MODE_FLAG_ENUM_END = 129 // 

// MAV_MODE_FLAG_DECODE_POSITION
mavlink.MAV_MODE_FLAG_DECODE_POSITION_CUSTOM_MODE = 1 // Eighth bit: 00000001
mavlink.MAV_MODE_FLAG_DECODE_POSITION_TEST = 2 // Seventh bit: 00000010
mavlink.MAV_MODE_FLAG_DECODE_POSITION_AUTO = 4 // Sixt bit:   00000100
mavlink.MAV_MODE_FLAG_DECODE_POSITION_GUIDED = 8 // Fifth bit:  00001000
mavlink.MAV_MODE_FLAG_DECODE_POSITION_STABILIZE = 16 // Fourth bit: 00010000
mavlink.MAV_MODE_FLAG_DECODE_POSITION_HIL = 32 // Third bit:  00100000
mavlink.MAV_MODE_FLAG_DECODE_POSITION_MANUAL = 64 // Second bit: 01000000
mavlink.MAV_MODE_FLAG_DECODE_POSITION_SAFETY = 128 // First bit:  10000000
mavlink.MAV_MODE_FLAG_DECODE_POSITION_ENUM_END = 129 // 

// MAV_GOTO
mavlink.MAV_GOTO_DO_HOLD = 0 // Hold at the current position.
mavlink.MAV_GOTO_DO_CONTINUE = 1 // Continue with the next item in mission execution.
mavlink.MAV_GOTO_HOLD_AT_CURRENT_POSITION = 2 // Hold at the current position of the system
mavlink.MAV_GOTO_HOLD_AT_SPECIFIED_POSITION = 3 // Hold at the position specified in the parameters of the DO_HOLD action
mavlink.MAV_GOTO_ENUM_END = 4 // 

// MAV_MODE
mavlink.MAV_MODE_PREFLIGHT = 0 // System is not ready to fly, booting, calibrating, etc. No flag is set.
mavlink.MAV_MODE_MANUAL_DISARMED = 64 // System is allowed to be active, under manual (RC) control, no
                        // stabilization
mavlink.MAV_MODE_TEST_DISARMED = 66 // UNDEFINED mode. This solely depends on the autopilot - use with
                        // caution, intended for developers only.
mavlink.MAV_MODE_STABILIZE_DISARMED = 80 // System is allowed to be active, under assisted RC control.
mavlink.MAV_MODE_GUIDED_DISARMED = 88 // System is allowed to be active, under autonomous control, manual
                        // setpoint
mavlink.MAV_MODE_AUTO_DISARMED = 92 // System is allowed to be active, under autonomous control and
                        // navigation (the trajectory is decided
                        // onboard and not pre-programmed by MISSIONs)
mavlink.MAV_MODE_MANUAL_ARMED = 192 // System is allowed to be active, under manual (RC) control, no
                        // stabilization
mavlink.MAV_MODE_TEST_ARMED = 194 // UNDEFINED mode. This solely depends on the autopilot - use with
                        // caution, intended for developers only.
mavlink.MAV_MODE_STABILIZE_ARMED = 208 // System is allowed to be active, under assisted RC control.
mavlink.MAV_MODE_GUIDED_ARMED = 216 // System is allowed to be active, under autonomous control, manual
                        // setpoint
mavlink.MAV_MODE_AUTO_ARMED = 220 // System is allowed to be active, under autonomous control and
                        // navigation (the trajectory is decided
                        // onboard and not pre-programmed by MISSIONs)
mavlink.MAV_MODE_ENUM_END = 221 // 

// MAV_STATE
mavlink.MAV_STATE_UNINIT = 0 // Uninitialized system, state is unknown.
mavlink.MAV_STATE_BOOT = 1 // System is booting up.
mavlink.MAV_STATE_CALIBRATING = 2 // System is calibrating and not flight-ready.
mavlink.MAV_STATE_STANDBY = 3 // System is grounded and on standby. It can be launched any time.
mavlink.MAV_STATE_ACTIVE = 4 // System is active and might be already airborne. Motors are engaged.
mavlink.MAV_STATE_CRITICAL = 5 // System is in a non-normal flight mode. It can however still navigate.
mavlink.MAV_STATE_EMERGENCY = 6 // System is in a non-normal flight mode. It lost control over parts or
                        // over the whole airframe. It is in mayday
                        // and going down.
mavlink.MAV_STATE_POWEROFF = 7 // System just initialized its power-down sequence, will shut down now.
mavlink.MAV_STATE_ENUM_END = 8 // 

// MAV_COMPONENT
mavlink.MAV_COMP_ID_ALL = 0 // 
mavlink.MAV_COMP_ID_CAMERA = 100 // 
mavlink.MAV_COMP_ID_SERVO1 = 140 // 
mavlink.MAV_COMP_ID_SERVO2 = 141 // 
mavlink.MAV_COMP_ID_SERVO3 = 142 // 
mavlink.MAV_COMP_ID_SERVO4 = 143 // 
mavlink.MAV_COMP_ID_SERVO5 = 144 // 
mavlink.MAV_COMP_ID_SERVO6 = 145 // 
mavlink.MAV_COMP_ID_SERVO7 = 146 // 
mavlink.MAV_COMP_ID_SERVO8 = 147 // 
mavlink.MAV_COMP_ID_SERVO9 = 148 // 
mavlink.MAV_COMP_ID_SERVO10 = 149 // 
mavlink.MAV_COMP_ID_SERVO11 = 150 // 
mavlink.MAV_COMP_ID_SERVO12 = 151 // 
mavlink.MAV_COMP_ID_SERVO13 = 152 // 
mavlink.MAV_COMP_ID_SERVO14 = 153 // 
mavlink.MAV_COMP_ID_GIMBAL = 154 // 
mavlink.MAV_COMP_ID_MAPPER = 180 // 
mavlink.MAV_COMP_ID_MISSIONPLANNER = 190 // 
mavlink.MAV_COMP_ID_PATHPLANNER = 195 // 
mavlink.MAV_COMP_ID_IMU = 200 // 
mavlink.MAV_COMP_ID_IMU_2 = 201 // 
mavlink.MAV_COMP_ID_IMU_3 = 202 // 
mavlink.MAV_COMP_ID_GPS = 220 // 
mavlink.MAV_COMP_ID_UDP_BRIDGE = 240 // 
mavlink.MAV_COMP_ID_UART_BRIDGE = 241 // 
mavlink.MAV_COMP_ID_SYSTEM_CONTROL = 250 // 
mavlink.MAV_COMPONENT_ENUM_END = 251 // 

// MAV_SYS_STATUS_SENSOR
mavlink.MAV_SYS_STATUS_SENSOR_3D_GYRO = 1 // 0x01 3D gyro
mavlink.MAV_SYS_STATUS_SENSOR_3D_ACCEL = 2 // 0x02 3D accelerometer
mavlink.MAV_SYS_STATUS_SENSOR_3D_MAG = 4 // 0x04 3D magnetometer
mavlink.MAV_SYS_STATUS_SENSOR_ABSOLUTE_PRESSURE = 8 // 0x08 absolute pressure
mavlink.MAV_SYS_STATUS_SENSOR_DIFFERENTIAL_PRESSURE = 16 // 0x10 differential pressure
mavlink.MAV_SYS_STATUS_SENSOR_GPS = 32 // 0x20 GPS
mavlink.MAV_SYS_STATUS_SENSOR_OPTICAL_FLOW = 64 // 0x40 optical flow
mavlink.MAV_SYS_STATUS_SENSOR_VISION_POSITION = 128 // 0x80 computer vision position
mavlink.MAV_SYS_STATUS_SENSOR_LASER_POSITION = 256 // 0x100 laser based position
mavlink.MAV_SYS_STATUS_SENSOR_EXTERNAL_GROUND_TRUTH = 512 // 0x200 external ground truth (Vicon or Leica)
mavlink.MAV_SYS_STATUS_SENSOR_ANGULAR_RATE_CONTROL = 1024 // 0x400 3D angular rate control
mavlink.MAV_SYS_STATUS_SENSOR_ATTITUDE_STABILIZATION = 2048 // 0x800 attitude stabilization
mavlink.MAV_SYS_STATUS_SENSOR_YAW_POSITION = 4096 // 0x1000 yaw position
mavlink.MAV_SYS_STATUS_SENSOR_Z_ALTITUDE_CONTROL = 8192 // 0x2000 z/altitude control
mavlink.MAV_SYS_STATUS_SENSOR_XY_POSITION_CONTROL = 16384 // 0x4000 x/y position control
mavlink.MAV_SYS_STATUS_SENSOR_MOTOR_OUTPUTS = 32768 // 0x8000 motor outputs / control
mavlink.MAV_SYS_STATUS_SENSOR_RC_RECEIVER = 65536 // 0x10000 rc receiver
mavlink.MAV_SYS_STATUS_SENSOR_3D_GYRO2 = 131072 // 0x20000 2nd 3D gyro
mavlink.MAV_SYS_STATUS_SENSOR_3D_ACCEL2 = 262144 // 0x40000 2nd 3D accelerometer
mavlink.MAV_SYS_STATUS_SENSOR_3D_MAG2 = 524288 // 0x80000 2nd 3D magnetometer
mavlink.MAV_SYS_STATUS_GEOFENCE = 1048576 // 0x100000 geofence
mavlink.MAV_SYS_STATUS_AHRS = 2097152 // 0x200000 AHRS subsystem health
mavlink.MAV_SYS_STATUS_TERRAIN = 4194304 // 0x400000 Terrain subsystem health
mavlink.MAV_SYS_STATUS_SENSOR_ENUM_END = 4194305 // 

// MAV_FRAME
mavlink.MAV_FRAME_GLOBAL = 0 // Global coordinate frame, WGS84 coordinate system. First value / x:
                        // latitude, second value / y: longitude,
                        // third value / z: positive altitude over
                        // mean sea level (MSL)
mavlink.MAV_FRAME_LOCAL_NED = 1 // Local coordinate frame, Z-up (x: north, y: east, z: down).
mavlink.MAV_FRAME_MISSION = 2 // NOT a coordinate frame, indicates a mission command.
mavlink.MAV_FRAME_GLOBAL_RELATIVE_ALT = 3 // Global coordinate frame, WGS84 coordinate system, relative altitude
                        // over ground with respect to the home
                        // position. First value / x: latitude, second
                        // value / y: longitude, third value / z:
                        // positive altitude with 0 being at the
                        // altitude of the home location.
mavlink.MAV_FRAME_LOCAL_ENU = 4 // Local coordinate frame, Z-down (x: east, y: north, z: up)
mavlink.MAV_FRAME_GLOBAL_INT = 5 // Global coordinate frame, WGS84 coordinate system. First value / x:
                        // latitude in degrees*1.0e-7, second value /
                        // y: longitude in degrees*1.0e-7, third value
                        // / z: positive altitude over mean sea level
                        // (MSL)
mavlink.MAV_FRAME_GLOBAL_RELATIVE_ALT_INT = 6 // Global coordinate frame, WGS84 coordinate system, relative altitude
                        // over ground with respect to the home
                        // position. First value / x: latitude in
                        // degrees*10e-7, second value / y: longitude
                        // in degrees*10e-7, third value / z: positive
                        // altitude with 0 being at the altitude of
                        // the home location.
mavlink.MAV_FRAME_LOCAL_OFFSET_NED = 7 // Offset to the current local frame. Anything expressed in this frame
                        // should be added to the current local frame
                        // position.
mavlink.MAV_FRAME_BODY_NED = 8 // Setpoint in body NED frame. This makes sense if all position control
                        // is externalized - e.g. useful to command 2
                        // m/s^2 acceleration to the right.
mavlink.MAV_FRAME_BODY_OFFSET_NED = 9 // Offset in body NED frame. This makes sense if adding setpoints to the
                        // current flight path, to avoid an obstacle -
                        // e.g. useful to command 2 m/s^2 acceleration
                        // to the east.
mavlink.MAV_FRAME_GLOBAL_TERRAIN_ALT = 10 // Global coordinate frame with above terrain level altitude. WGS84
                        // coordinate system, relative altitude over
                        // terrain with respect to the waypoint
                        // coordinate. First value / x: latitude in
                        // degrees, second value / y: longitude in
                        // degrees, third value / z: positive altitude
                        // in meters with 0 being at ground level in
                        // terrain model.
mavlink.MAV_FRAME_GLOBAL_TERRAIN_ALT_INT = 11 // Global coordinate frame with above terrain level altitude. WGS84
                        // coordinate system, relative altitude over
                        // terrain with respect to the waypoint
                        // coordinate. First value / x: latitude in
                        // degrees*10e-7, second value / y: longitude
                        // in degrees*10e-7, third value / z: positive
                        // altitude in meters with 0 being at ground
                        // level in terrain model.
mavlink.MAV_FRAME_ENUM_END = 12 // 

// MAVLINK_DATA_STREAM_TYPE
mavlink.MAVLINK_DATA_STREAM_IMG_JPEG = 1 // 
mavlink.MAVLINK_DATA_STREAM_IMG_BMP = 2 // 
mavlink.MAVLINK_DATA_STREAM_IMG_RAW8U = 3 // 
mavlink.MAVLINK_DATA_STREAM_IMG_RAW32U = 4 // 
mavlink.MAVLINK_DATA_STREAM_IMG_PGM = 5 // 
mavlink.MAVLINK_DATA_STREAM_IMG_PNG = 6 // 
mavlink.MAVLINK_DATA_STREAM_TYPE_ENUM_END = 7 // 

// FENCE_ACTION
mavlink.FENCE_ACTION_NONE = 0 // Disable fenced mode
mavlink.FENCE_ACTION_GUIDED = 1 // Switched to guided mode to return point (fence point 0)
mavlink.FENCE_ACTION_REPORT = 2 // Report fence breach, but don't take action
mavlink.FENCE_ACTION_GUIDED_THR_PASS = 3 // Switched to guided mode to return point (fence point 0) with manual
                        // throttle control
mavlink.FENCE_ACTION_ENUM_END = 4 // 

// FENCE_BREACH
mavlink.FENCE_BREACH_NONE = 0 // No last fence breach
mavlink.FENCE_BREACH_MINALT = 1 // Breached minimum altitude
mavlink.FENCE_BREACH_MAXALT = 2 // Breached maximum altitude
mavlink.FENCE_BREACH_BOUNDARY = 3 // Breached fence boundary
mavlink.FENCE_BREACH_ENUM_END = 4 // 

// MAV_MOUNT_MODE
mavlink.MAV_MOUNT_MODE_RETRACT = 0 // Load and keep safe position (Roll,Pitch,Yaw) from permant memory and
                        // stop stabilization
mavlink.MAV_MOUNT_MODE_NEUTRAL = 1 // Load and keep neutral position (Roll,Pitch,Yaw) from permanent memory.
mavlink.MAV_MOUNT_MODE_MAVLINK_TARGETING = 2 // Load neutral position and start MAVLink Roll,Pitch,Yaw control with
                        // stabilization
mavlink.MAV_MOUNT_MODE_RC_TARGETING = 3 // Load neutral position and start RC Roll,Pitch,Yaw control with
                        // stabilization
mavlink.MAV_MOUNT_MODE_GPS_POINT = 4 // Load neutral position and start to point to Lat,Lon,Alt
mavlink.MAV_MOUNT_MODE_ENUM_END = 5 // 

// MAV_DATA_STREAM
mavlink.MAV_DATA_STREAM_ALL = 0 // Enable all data streams
mavlink.MAV_DATA_STREAM_RAW_SENSORS = 1 // Enable IMU_RAW, GPS_RAW, GPS_STATUS packets.
mavlink.MAV_DATA_STREAM_EXTENDED_STATUS = 2 // Enable GPS_STATUS, CONTROL_STATUS, AUX_STATUS
mavlink.MAV_DATA_STREAM_RC_CHANNELS = 3 // Enable RC_CHANNELS_SCALED, RC_CHANNELS_RAW, SERVO_OUTPUT_RAW
mavlink.MAV_DATA_STREAM_RAW_CONTROLLER = 4 // Enable ATTITUDE_CONTROLLER_OUTPUT, POSITION_CONTROLLER_OUTPUT,
                        // NAV_CONTROLLER_OUTPUT.
mavlink.MAV_DATA_STREAM_POSITION = 6 // Enable LOCAL_POSITION, GLOBAL_POSITION/GLOBAL_POSITION_INT messages.
mavlink.MAV_DATA_STREAM_EXTRA1 = 10 // Dependent on the autopilot
mavlink.MAV_DATA_STREAM_EXTRA2 = 11 // Dependent on the autopilot
mavlink.MAV_DATA_STREAM_EXTRA3 = 12 // Dependent on the autopilot
mavlink.MAV_DATA_STREAM_ENUM_END = 13 // 

// MAV_ROI
mavlink.MAV_ROI_NONE = 0 // No region of interest.
mavlink.MAV_ROI_WPNEXT = 1 // Point toward next MISSION.
mavlink.MAV_ROI_WPINDEX = 2 // Point toward given MISSION.
mavlink.MAV_ROI_LOCATION = 3 // Point toward fixed location.
mavlink.MAV_ROI_TARGET = 4 // Point toward of given id.
mavlink.MAV_ROI_ENUM_END = 5 // 

// MAV_CMD_ACK
mavlink.MAV_CMD_ACK_OK = 1 // Command / mission item is ok.
mavlink.MAV_CMD_ACK_ERR_FAIL = 2 // Generic error message if none of the other reasons fails or if no
                        // detailed error reporting is implemented.
mavlink.MAV_CMD_ACK_ERR_ACCESS_DENIED = 3 // The system is refusing to accept this command from this source /
                        // communication partner.
mavlink.MAV_CMD_ACK_ERR_NOT_SUPPORTED = 4 // Command or mission item is not supported, other commands would be
                        // accepted.
mavlink.MAV_CMD_ACK_ERR_COORDINATE_FRAME_NOT_SUPPORTED = 5 // The coordinate frame of this command / mission item is not supported.
mavlink.MAV_CMD_ACK_ERR_COORDINATES_OUT_OF_RANGE = 6 // The coordinate frame of this command is ok, but he coordinate values
                        // exceed the safety limits of this system.
                        // This is a generic error, please use the
                        // more specific error messages below if
                        // possible.
mavlink.MAV_CMD_ACK_ERR_X_LAT_OUT_OF_RANGE = 7 // The X or latitude value is out of range.
mavlink.MAV_CMD_ACK_ERR_Y_LON_OUT_OF_RANGE = 8 // The Y or longitude value is out of range.
mavlink.MAV_CMD_ACK_ERR_Z_ALT_OUT_OF_RANGE = 9 // The Z or altitude value is out of range.
mavlink.MAV_CMD_ACK_ENUM_END = 10 // 

// MAV_PARAM_TYPE
mavlink.MAV_PARAM_TYPE_UINT8 = 1 // 8-bit unsigned integer
mavlink.MAV_PARAM_TYPE_INT8 = 2 // 8-bit signed integer
mavlink.MAV_PARAM_TYPE_UINT16 = 3 // 16-bit unsigned integer
mavlink.MAV_PARAM_TYPE_INT16 = 4 // 16-bit signed integer
mavlink.MAV_PARAM_TYPE_UINT32 = 5 // 32-bit unsigned integer
mavlink.MAV_PARAM_TYPE_INT32 = 6 // 32-bit signed integer
mavlink.MAV_PARAM_TYPE_UINT64 = 7 // 64-bit unsigned integer
mavlink.MAV_PARAM_TYPE_INT64 = 8 // 64-bit signed integer
mavlink.MAV_PARAM_TYPE_REAL32 = 9 // 32-bit floating-point
mavlink.MAV_PARAM_TYPE_REAL64 = 10 // 64-bit floating-point
mavlink.MAV_PARAM_TYPE_ENUM_END = 11 // 

// MAV_RESULT
mavlink.MAV_RESULT_ACCEPTED = 0 // Command ACCEPTED and EXECUTED
mavlink.MAV_RESULT_TEMPORARILY_REJECTED = 1 // Command TEMPORARY REJECTED/DENIED
mavlink.MAV_RESULT_DENIED = 2 // Command PERMANENTLY DENIED
mavlink.MAV_RESULT_UNSUPPORTED = 3 // Command UNKNOWN/UNSUPPORTED
mavlink.MAV_RESULT_FAILED = 4 // Command executed, but failed
mavlink.MAV_RESULT_ENUM_END = 5 // 

// MAV_MISSION_RESULT
mavlink.MAV_MISSION_ACCEPTED = 0 // mission accepted OK
mavlink.MAV_MISSION_ERROR = 1 // generic error / not accepting mission commands at all right now
mavlink.MAV_MISSION_UNSUPPORTED_FRAME = 2 // coordinate frame is not supported
mavlink.MAV_MISSION_UNSUPPORTED = 3 // command is not supported
mavlink.MAV_MISSION_NO_SPACE = 4 // mission item exceeds storage space
mavlink.MAV_MISSION_INVALID = 5 // one of the parameters has an invalid value
mavlink.MAV_MISSION_INVALID_PARAM1 = 6 // param1 has an invalid value
mavlink.MAV_MISSION_INVALID_PARAM2 = 7 // param2 has an invalid value
mavlink.MAV_MISSION_INVALID_PARAM3 = 8 // param3 has an invalid value
mavlink.MAV_MISSION_INVALID_PARAM4 = 9 // param4 has an invalid value
mavlink.MAV_MISSION_INVALID_PARAM5_X = 10 // x/param5 has an invalid value
mavlink.MAV_MISSION_INVALID_PARAM6_Y = 11 // y/param6 has an invalid value
mavlink.MAV_MISSION_INVALID_PARAM7 = 12 // param7 has an invalid value
mavlink.MAV_MISSION_INVALID_SEQUENCE = 13 // received waypoint out of sequence
mavlink.MAV_MISSION_DENIED = 14 // not accepting any mission commands from this communication partner
mavlink.MAV_MISSION_RESULT_ENUM_END = 15 // 

// MAV_SEVERITY
mavlink.MAV_SEVERITY_EMERGENCY = 0 // System is unusable. This is a "panic" condition.
mavlink.MAV_SEVERITY_ALERT = 1 // Action should be taken immediately. Indicates error in non-critical
                        // systems.
mavlink.MAV_SEVERITY_CRITICAL = 2 // Action must be taken immediately. Indicates failure in a primary
                        // system.
mavlink.MAV_SEVERITY_ERROR = 3 // Indicates an error in secondary/redundant systems.
mavlink.MAV_SEVERITY_WARNING = 4 // Indicates about a possible future error if this is not resolved within
                        // a given timeframe. Example would be a low
                        // battery warning.
mavlink.MAV_SEVERITY_NOTICE = 5 // An unusual event has occured, though not an error condition. This
                        // should be investigated for the root cause.
mavlink.MAV_SEVERITY_INFO = 6 // Normal operational messages. Useful for logging. No action is required
                        // for these messages.
mavlink.MAV_SEVERITY_DEBUG = 7 // Useful non-operational messages that can assist in debugging. These
                        // should not occur during normal operation.
mavlink.MAV_SEVERITY_ENUM_END = 8 // 

// MAV_POWER_STATUS
mavlink.MAV_POWER_STATUS_BRICK_VALID = 1 // main brick power supply valid
mavlink.MAV_POWER_STATUS_SERVO_VALID = 2 // main servo power supply valid for FMU
mavlink.MAV_POWER_STATUS_USB_CONNECTED = 4 // USB power is connected
mavlink.MAV_POWER_STATUS_PERIPH_OVERCURRENT = 8 // peripheral supply is in over-current state
mavlink.MAV_POWER_STATUS_PERIPH_HIPOWER_OVERCURRENT = 16 // hi-power peripheral supply is in over-current state
mavlink.MAV_POWER_STATUS_CHANGED = 32 // Power status has changed since boot
mavlink.MAV_POWER_STATUS_ENUM_END = 33 // 

// SERIAL_CONTROL_DEV
mavlink.SERIAL_CONTROL_DEV_TELEM1 = 0 // First telemetry port
mavlink.SERIAL_CONTROL_DEV_TELEM2 = 1 // Second telemetry port
mavlink.SERIAL_CONTROL_DEV_GPS1 = 2 // First GPS port
mavlink.SERIAL_CONTROL_DEV_GPS2 = 3 // Second GPS port
mavlink.SERIAL_CONTROL_DEV_ENUM_END = 4 // 

// SERIAL_CONTROL_FLAG
mavlink.SERIAL_CONTROL_FLAG_REPLY = 1 // Set if this is a reply
mavlink.SERIAL_CONTROL_FLAG_RESPOND = 2 // Set if the sender wants the receiver to send a response as another
                        // SERIAL_CONTROL message
mavlink.SERIAL_CONTROL_FLAG_EXCLUSIVE = 4 // Set if access to the serial port should be removed from whatever
                        // driver is currently using it, giving
                        // exclusive access to the SERIAL_CONTROL
                        // protocol. The port can be handed back by
                        // sending a request without this flag set
mavlink.SERIAL_CONTROL_FLAG_BLOCKING = 8 // Block on writes to the serial port
mavlink.SERIAL_CONTROL_FLAG_MULTI = 16 // Send multiple replies until port is drained
mavlink.SERIAL_CONTROL_FLAG_ENUM_END = 17 // 

// MAV_DISTANCE_SENSOR
mavlink.MAV_DISTANCE_SENSOR_LASER = 0 // Laser altimeter, e.g. LightWare SF02/F or PulsedLight units
mavlink.MAV_DISTANCE_SENSOR_ULTRASOUND = 1 // Ultrasound altimeter, e.g. MaxBotix units
mavlink.MAV_DISTANCE_SENSOR_ENUM_END = 2 // 

// MAV_PROTOCOL_CAPABILITY
mavlink.MAV_PROTOCOL_CAPABILITY_MISSION_FLOAT = 1 // Autopilot supports MISSION float message type.
mavlink.MAV_PROTOCOL_CAPABILITY_PARAM_FLOAT = 2 // Autopilot supports the new param float message type.
mavlink.MAV_PROTOCOL_CAPABILITY_MISSION_INT = 4 // Autopilot supports MISSION_INT scaled integer message type.
mavlink.MAV_PROTOCOL_CAPABILITY_COMMAND_INT = 8 // Autopilot supports COMMAND_INT scaled integer message type.
mavlink.MAV_PROTOCOL_CAPABILITY_PARAM_UNION = 16 // Autopilot supports the new param union message type.
mavlink.MAV_PROTOCOL_CAPABILITY_FTP = 32 // Autopilot supports the new param union message type.
mavlink.MAV_PROTOCOL_CAPABILITY_SET_ATTITUDE_TARGET = 64 // Autopilot supports commanding attitude offboard.
mavlink.MAV_PROTOCOL_CAPABILITY_SET_POSITION_TARGET_LOCAL_NED = 128 // Autopilot supports commanding position and velocity targets in local
                        // NED frame.
mavlink.MAV_PROTOCOL_CAPABILITY_SET_POSITION_TARGET_GLOBAL_INT = 256 // Autopilot supports commanding position and velocity targets in global
                        // scaled integers.
mavlink.MAV_PROTOCOL_CAPABILITY_TERRAIN = 512 // Autopilot supports terrain protocol / data handling.
mavlink.MAV_PROTOCOL_CAPABILITY_SET_ACTUATOR_TARGET = 1024 // Autopilot supports direct actuator control.
mavlink.MAV_PROTOCOL_CAPABILITY_ENUM_END = 1025 // 

// MAV_ESTIMATOR_TYPE
mavlink.MAV_ESTIMATOR_TYPE_NAIVE = 1 // This is a naive estimator without any real covariance feedback.
mavlink.MAV_ESTIMATOR_TYPE_VISION = 2 // Computer vision based estimate. Might be up to scale.
mavlink.MAV_ESTIMATOR_TYPE_VIO = 3 // Visual-inertial estimate.
mavlink.MAV_ESTIMATOR_TYPE_GPS = 4 // Plain GPS estimate.
mavlink.MAV_ESTIMATOR_TYPE_GPS_INS = 5 // Estimator integrating GPS and inertial sensing.
mavlink.MAV_ESTIMATOR_TYPE_ENUM_END = 6 // 

// MAV_BATTERY_TYPE
mavlink.MAV_BATTERY_TYPE_UNKNOWN = 0 // Not specified.
mavlink.MAV_BATTERY_TYPE_LIPO = 1 // Lithium polymere battery
mavlink.MAV_BATTERY_TYPE_LIFE = 2 // Lithium ferrite battery
mavlink.MAV_BATTERY_TYPE_LION = 3 // Lithium-ION battery
mavlink.MAV_BATTERY_TYPE_NIMH = 4 // Nickel metal hydride battery
mavlink.MAV_BATTERY_TYPE_ENUM_END = 5 // 

// MAV_BATTERY_FUNCTION
mavlink.MAV_BATTERY_FUNCTION_UNKNOWN = 0 // Lithium polymere battery
mavlink.MAV_BATTERY_FUNCTION_ALL = 1 // Battery supports all flight systems
mavlink.MAV_BATTERY_FUNCTION_PROPULSION = 2 // Battery for the propulsion system
mavlink.MAV_BATTERY_FUNCTION_AVIONICS = 3 // Avionics battery
mavlink.MAV_BATTERY_TYPE_PAYLOAD = 4 // Payload battery
mavlink.MAV_BATTERY_FUNCTION_ENUM_END = 5 // 

// message IDs
mavlink.MAVLINK_MSG_ID_BAD_DATA = -1
mavlink.MAVLINK_MSG_ID_SENSOR_OFFSETS = 150
mavlink.MAVLINK_MSG_ID_SET_MAG_OFFSETS = 151
mavlink.MAVLINK_MSG_ID_MEMINFO = 152
mavlink.MAVLINK_MSG_ID_AP_ADC = 153
mavlink.MAVLINK_MSG_ID_DIGICAM_CONFIGURE = 154
mavlink.MAVLINK_MSG_ID_DIGICAM_CONTROL = 155
mavlink.MAVLINK_MSG_ID_MOUNT_CONFIGURE = 156
mavlink.MAVLINK_MSG_ID_MOUNT_CONTROL = 157
mavlink.MAVLINK_MSG_ID_MOUNT_STATUS = 158
mavlink.MAVLINK_MSG_ID_FENCE_POINT = 160
mavlink.MAVLINK_MSG_ID_FENCE_FETCH_POINT = 161
mavlink.MAVLINK_MSG_ID_FENCE_STATUS = 162
mavlink.MAVLINK_MSG_ID_AHRS = 163
mavlink.MAVLINK_MSG_ID_SIMSTATE = 164
mavlink.MAVLINK_MSG_ID_HWSTATUS = 165
mavlink.MAVLINK_MSG_ID_RADIO = 166
mavlink.MAVLINK_MSG_ID_LIMITS_STATUS = 167
mavlink.MAVLINK_MSG_ID_WIND = 168
mavlink.MAVLINK_MSG_ID_DATA16 = 169
mavlink.MAVLINK_MSG_ID_DATA32 = 170
mavlink.MAVLINK_MSG_ID_DATA64 = 171
mavlink.MAVLINK_MSG_ID_DATA96 = 172
mavlink.MAVLINK_MSG_ID_RANGEFINDER = 173
mavlink.MAVLINK_MSG_ID_AIRSPEED_AUTOCAL = 174
mavlink.MAVLINK_MSG_ID_RALLY_POINT = 175
mavlink.MAVLINK_MSG_ID_RALLY_FETCH_POINT = 176
mavlink.MAVLINK_MSG_ID_COMPASSMOT_STATUS = 177
mavlink.MAVLINK_MSG_ID_AHRS2 = 178
mavlink.MAVLINK_MSG_ID_CAMERA_STATUS = 179
mavlink.MAVLINK_MSG_ID_CAMERA_FEEDBACK = 180
mavlink.MAVLINK_MSG_ID_BATTERY2 = 181
mavlink.MAVLINK_MSG_ID_AHRS3 = 182
mavlink.MAVLINK_MSG_ID_AUTOPILOT_VERSION_REQUEST = 183
mavlink.MAVLINK_MSG_ID_LED_CONTROL = 186
mavlink.MAVLINK_MSG_ID_MAG_CAL_PROGRESS = 191
mavlink.MAVLINK_MSG_ID_MAG_CAL_REPORT = 192
mavlink.MAVLINK_MSG_ID_EKF_STATUS_REPORT = 193
mavlink.MAVLINK_MSG_ID_GIMBAL_REPORT = 200
mavlink.MAVLINK_MSG_ID_GIMBAL_CONTROL = 201
mavlink.MAVLINK_MSG_ID_GIMBAL_RESET = 202
mavlink.MAVLINK_MSG_ID_GIMBAL_AXIS_CALIBRATION_PROGRESS = 203
mavlink.MAVLINK_MSG_ID_GIMBAL_SET_HOME_OFFSETS = 204
mavlink.MAVLINK_MSG_ID_GIMBAL_HOME_OFFSET_CALIBRATION_RESULT = 205
mavlink.MAVLINK_MSG_ID_GIMBAL_SET_FACTORY_PARAMETERS = 206
mavlink.MAVLINK_MSG_ID_GIMBAL_FACTORY_PARAMETERS_LOADED = 207
mavlink.MAVLINK_MSG_ID_GIMBAL_ERASE_FIRMWARE_AND_CONFIG = 208
mavlink.MAVLINK_MSG_ID_GIMBAL_PERFORM_FACTORY_TESTS = 209
mavlink.MAVLINK_MSG_ID_GIMBAL_REPORT_FACTORY_TESTS_PROGRESS = 210
mavlink.MAVLINK_MSG_ID_GOPRO_POWER_ON = 215
mavlink.MAVLINK_MSG_ID_GOPRO_POWER_OFF = 216
mavlink.MAVLINK_MSG_ID_GOPRO_COMMAND = 217
mavlink.MAVLINK_MSG_ID_GOPRO_RESPONSE = 218
mavlink.MAVLINK_MSG_ID_HEARTBEAT = 0
mavlink.MAVLINK_MSG_ID_SYS_STATUS = 1
mavlink.MAVLINK_MSG_ID_SYSTEM_TIME = 2
mavlink.MAVLINK_MSG_ID_PING = 4
mavlink.MAVLINK_MSG_ID_CHANGE_OPERATOR_CONTROL = 5
mavlink.MAVLINK_MSG_ID_CHANGE_OPERATOR_CONTROL_ACK = 6
mavlink.MAVLINK_MSG_ID_AUTH_KEY = 7
mavlink.MAVLINK_MSG_ID_SET_MODE = 11
mavlink.MAVLINK_MSG_ID_PARAM_REQUEST_READ = 20
mavlink.MAVLINK_MSG_ID_PARAM_REQUEST_LIST = 21
mavlink.MAVLINK_MSG_ID_PARAM_VALUE = 22
mavlink.MAVLINK_MSG_ID_PARAM_SET = 23
mavlink.MAVLINK_MSG_ID_GPS_RAW_INT = 24
mavlink.MAVLINK_MSG_ID_GPS_STATUS = 25
mavlink.MAVLINK_MSG_ID_SCALED_IMU = 26
mavlink.MAVLINK_MSG_ID_RAW_IMU = 27
mavlink.MAVLINK_MSG_ID_RAW_PRESSURE = 28
mavlink.MAVLINK_MSG_ID_SCALED_PRESSURE = 29
mavlink.MAVLINK_MSG_ID_ATTITUDE = 30
mavlink.MAVLINK_MSG_ID_ATTITUDE_QUATERNION = 31
mavlink.MAVLINK_MSG_ID_LOCAL_POSITION_NED = 32
mavlink.MAVLINK_MSG_ID_GLOBAL_POSITION_INT = 33
mavlink.MAVLINK_MSG_ID_RC_CHANNELS_SCALED = 34
mavlink.MAVLINK_MSG_ID_RC_CHANNELS_RAW = 35
mavlink.MAVLINK_MSG_ID_SERVO_OUTPUT_RAW = 36
mavlink.MAVLINK_MSG_ID_MISSION_REQUEST_PARTIAL_LIST = 37
mavlink.MAVLINK_MSG_ID_MISSION_WRITE_PARTIAL_LIST = 38
mavlink.MAVLINK_MSG_ID_MISSION_ITEM = 39
mavlink.MAVLINK_MSG_ID_MISSION_REQUEST = 40
mavlink.MAVLINK_MSG_ID_MISSION_SET_CURRENT = 41
mavlink.MAVLINK_MSG_ID_MISSION_CURRENT = 42
mavlink.MAVLINK_MSG_ID_MISSION_REQUEST_LIST = 43
mavlink.MAVLINK_MSG_ID_MISSION_COUNT = 44
mavlink.MAVLINK_MSG_ID_MISSION_CLEAR_ALL = 45
mavlink.MAVLINK_MSG_ID_MISSION_ITEM_REACHED = 46
mavlink.MAVLINK_MSG_ID_MISSION_ACK = 47
mavlink.MAVLINK_MSG_ID_SET_GPS_GLOBAL_ORIGIN = 48
mavlink.MAVLINK_MSG_ID_GPS_GLOBAL_ORIGIN = 49
mavlink.MAVLINK_MSG_ID_PARAM_MAP_RC = 50
mavlink.MAVLINK_MSG_ID_SAFETY_SET_ALLOWED_AREA = 54
mavlink.MAVLINK_MSG_ID_SAFETY_ALLOWED_AREA = 55
mavlink.MAVLINK_MSG_ID_ATTITUDE_QUATERNION_COV = 61
mavlink.MAVLINK_MSG_ID_NAV_CONTROLLER_OUTPUT = 62
mavlink.MAVLINK_MSG_ID_GLOBAL_POSITION_INT_COV = 63
mavlink.MAVLINK_MSG_ID_LOCAL_POSITION_NED_COV = 64
mavlink.MAVLINK_MSG_ID_RC_CHANNELS = 65
mavlink.MAVLINK_MSG_ID_REQUEST_DATA_STREAM = 66
mavlink.MAVLINK_MSG_ID_DATA_STREAM = 67
mavlink.MAVLINK_MSG_ID_MANUAL_CONTROL = 69
mavlink.MAVLINK_MSG_ID_RC_CHANNELS_OVERRIDE = 70
mavlink.MAVLINK_MSG_ID_MISSION_ITEM_INT = 73
mavlink.MAVLINK_MSG_ID_VFR_HUD = 74
mavlink.MAVLINK_MSG_ID_COMMAND_INT = 75
mavlink.MAVLINK_MSG_ID_COMMAND_LONG = 76
mavlink.MAVLINK_MSG_ID_COMMAND_ACK = 77
mavlink.MAVLINK_MSG_ID_MANUAL_SETPOINT = 81
mavlink.MAVLINK_MSG_ID_SET_ATTITUDE_TARGET = 82
mavlink.MAVLINK_MSG_ID_ATTITUDE_TARGET = 83
mavlink.MAVLINK_MSG_ID_SET_POSITION_TARGET_LOCAL_NED = 84
mavlink.MAVLINK_MSG_ID_POSITION_TARGET_LOCAL_NED = 85
mavlink.MAVLINK_MSG_ID_SET_POSITION_TARGET_GLOBAL_INT = 86
mavlink.MAVLINK_MSG_ID_POSITION_TARGET_GLOBAL_INT = 87
mavlink.MAVLINK_MSG_ID_LOCAL_POSITION_NED_SYSTEM_GLOBAL_OFFSET = 89
mavlink.MAVLINK_MSG_ID_HIL_STATE = 90
mavlink.MAVLINK_MSG_ID_HIL_CONTROLS = 91
mavlink.MAVLINK_MSG_ID_HIL_RC_INPUTS_RAW = 92
mavlink.MAVLINK_MSG_ID_OPTICAL_FLOW = 100
mavlink.MAVLINK_MSG_ID_GLOBAL_VISION_POSITION_ESTIMATE = 101
mavlink.MAVLINK_MSG_ID_VISION_POSITION_ESTIMATE = 102
mavlink.MAVLINK_MSG_ID_VISION_SPEED_ESTIMATE = 103
mavlink.MAVLINK_MSG_ID_VICON_POSITION_ESTIMATE = 104
mavlink.MAVLINK_MSG_ID_HIGHRES_IMU = 105
mavlink.MAVLINK_MSG_ID_OPTICAL_FLOW_RAD = 106
mavlink.MAVLINK_MSG_ID_HIL_SENSOR = 107
mavlink.MAVLINK_MSG_ID_SIM_STATE = 108
mavlink.MAVLINK_MSG_ID_RADIO_STATUS = 109
mavlink.MAVLINK_MSG_ID_FILE_TRANSFER_PROTOCOL = 110
mavlink.MAVLINK_MSG_ID_TIMESYNC = 111
mavlink.MAVLINK_MSG_ID_HIL_GPS = 113
mavlink.MAVLINK_MSG_ID_HIL_OPTICAL_FLOW = 114
mavlink.MAVLINK_MSG_ID_HIL_STATE_QUATERNION = 115
mavlink.MAVLINK_MSG_ID_SCALED_IMU2 = 116
mavlink.MAVLINK_MSG_ID_LOG_REQUEST_LIST = 117
mavlink.MAVLINK_MSG_ID_LOG_ENTRY = 118
mavlink.MAVLINK_MSG_ID_LOG_REQUEST_DATA = 119
mavlink.MAVLINK_MSG_ID_LOG_DATA = 120
mavlink.MAVLINK_MSG_ID_LOG_ERASE = 121
mavlink.MAVLINK_MSG_ID_LOG_REQUEST_END = 122
mavlink.MAVLINK_MSG_ID_GPS_INJECT_DATA = 123
mavlink.MAVLINK_MSG_ID_GPS2_RAW = 124
mavlink.MAVLINK_MSG_ID_POWER_STATUS = 125
mavlink.MAVLINK_MSG_ID_SERIAL_CONTROL = 126
mavlink.MAVLINK_MSG_ID_GPS_RTK = 127
mavlink.MAVLINK_MSG_ID_GPS2_RTK = 128
mavlink.MAVLINK_MSG_ID_SCALED_IMU3 = 129
mavlink.MAVLINK_MSG_ID_DATA_TRANSMISSION_HANDSHAKE = 130
mavlink.MAVLINK_MSG_ID_ENCAPSULATED_DATA = 131
mavlink.MAVLINK_MSG_ID_DISTANCE_SENSOR = 132
mavlink.MAVLINK_MSG_ID_TERRAIN_REQUEST = 133
mavlink.MAVLINK_MSG_ID_TERRAIN_DATA = 134
mavlink.MAVLINK_MSG_ID_TERRAIN_CHECK = 135
mavlink.MAVLINK_MSG_ID_TERRAIN_REPORT = 136
mavlink.MAVLINK_MSG_ID_SCALED_PRESSURE2 = 137
mavlink.MAVLINK_MSG_ID_ATT_POS_MOCAP = 138
mavlink.MAVLINK_MSG_ID_SET_ACTUATOR_CONTROL_TARGET = 139
mavlink.MAVLINK_MSG_ID_ACTUATOR_CONTROL_TARGET = 140
mavlink.MAVLINK_MSG_ID_BATTERY_STATUS = 147
mavlink.MAVLINK_MSG_ID_AUTOPILOT_VERSION = 148
mavlink.MAVLINK_MSG_ID_V2_EXTENSION = 248
mavlink.MAVLINK_MSG_ID_MEMORY_VECT = 249
mavlink.MAVLINK_MSG_ID_DEBUG_VECT = 250
mavlink.MAVLINK_MSG_ID_NAMED_VALUE_FLOAT = 251
mavlink.MAVLINK_MSG_ID_NAMED_VALUE_INT = 252
mavlink.MAVLINK_MSG_ID_STATUSTEXT = 253
mavlink.MAVLINK_MSG_ID_DEBUG = 254

mavlink.messages = {};

/* 
Offsets and calibrations values for hardware         sensors. This
makes it easier to debug the calibration process.

                mag_ofs_x                 : magnetometer X offset (int16_t)
                mag_ofs_y                 : magnetometer Y offset (int16_t)
                mag_ofs_z                 : magnetometer Z offset (int16_t)
                mag_declination           : magnetic declination (radians) (float)
                raw_press                 : raw pressure from barometer (int32_t)
                raw_temp                  : raw temperature from barometer (int32_t)
                gyro_cal_x                : gyro X calibration (float)
                gyro_cal_y                : gyro Y calibration (float)
                gyro_cal_z                : gyro Z calibration (float)
                accel_cal_x               : accel X calibration (float)
                accel_cal_y               : accel Y calibration (float)
                accel_cal_z               : accel Z calibration (float)

*/
mavlink.messages.sensor_offsets = function(mag_ofs_x, mag_ofs_y, mag_ofs_z, mag_declination, raw_press, raw_temp, gyro_cal_x, gyro_cal_y, gyro_cal_z, accel_cal_x, accel_cal_y, accel_cal_z) {

    this.format = '<fiiffffffhhh';
    this.id = mavlink.MAVLINK_MSG_ID_SENSOR_OFFSETS;
    this.order_map = [9, 10, 11, 0, 1, 2, 3, 4, 5, 6, 7, 8];
    this.crc_extra = 134;
    this.name = 'SENSOR_OFFSETS';

    this.fieldnames = ['mag_ofs_x', 'mag_ofs_y', 'mag_ofs_z', 'mag_declination', 'raw_press', 'raw_temp', 'gyro_cal_x', 'gyro_cal_y', 'gyro_cal_z', 'accel_cal_x', 'accel_cal_y', 'accel_cal_z'];


    this.set(arguments);

}
        
mavlink.messages.sensor_offsets.prototype = new mavlink.message;

mavlink.messages.sensor_offsets.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.mag_declination, this.raw_press, this.raw_temp, this.gyro_cal_x, this.gyro_cal_y, this.gyro_cal_z, this.accel_cal_x, this.accel_cal_y, this.accel_cal_z, this.mag_ofs_x, this.mag_ofs_y, this.mag_ofs_z]));
}

/* 
Deprecated. Use MAV_CMD_PREFLIGHT_SET_SENSOR_OFFSETS instead. Set the
magnetometer offsets

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                mag_ofs_x                 : magnetometer X offset (int16_t)
                mag_ofs_y                 : magnetometer Y offset (int16_t)
                mag_ofs_z                 : magnetometer Z offset (int16_t)

*/
mavlink.messages.set_mag_offsets = function(target_system, target_component, mag_ofs_x, mag_ofs_y, mag_ofs_z) {

    this.format = '<hhhBB';
    this.id = mavlink.MAVLINK_MSG_ID_SET_MAG_OFFSETS;
    this.order_map = [3, 4, 0, 1, 2];
    this.crc_extra = 219;
    this.name = 'SET_MAG_OFFSETS';

    this.fieldnames = ['target_system', 'target_component', 'mag_ofs_x', 'mag_ofs_y', 'mag_ofs_z'];


    this.set(arguments);

}
        
mavlink.messages.set_mag_offsets.prototype = new mavlink.message;

mavlink.messages.set_mag_offsets.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.mag_ofs_x, this.mag_ofs_y, this.mag_ofs_z, this.target_system, this.target_component]));
}

/* 
state of APM memory

                brkval                    : heap top (uint16_t)
                freemem                   : free memory (uint16_t)

*/
mavlink.messages.meminfo = function(brkval, freemem) {

    this.format = '<HH';
    this.id = mavlink.MAVLINK_MSG_ID_MEMINFO;
    this.order_map = [0, 1];
    this.crc_extra = 208;
    this.name = 'MEMINFO';

    this.fieldnames = ['brkval', 'freemem'];


    this.set(arguments);

}
        
mavlink.messages.meminfo.prototype = new mavlink.message;

mavlink.messages.meminfo.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.brkval, this.freemem]));
}

/* 
raw ADC output

                adc1                      : ADC output 1 (uint16_t)
                adc2                      : ADC output 2 (uint16_t)
                adc3                      : ADC output 3 (uint16_t)
                adc4                      : ADC output 4 (uint16_t)
                adc5                      : ADC output 5 (uint16_t)
                adc6                      : ADC output 6 (uint16_t)

*/
mavlink.messages.ap_adc = function(adc1, adc2, adc3, adc4, adc5, adc6) {

    this.format = '<HHHHHH';
    this.id = mavlink.MAVLINK_MSG_ID_AP_ADC;
    this.order_map = [0, 1, 2, 3, 4, 5];
    this.crc_extra = 188;
    this.name = 'AP_ADC';

    this.fieldnames = ['adc1', 'adc2', 'adc3', 'adc4', 'adc5', 'adc6'];


    this.set(arguments);

}
        
mavlink.messages.ap_adc.prototype = new mavlink.message;

mavlink.messages.ap_adc.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.adc1, this.adc2, this.adc3, this.adc4, this.adc5, this.adc6]));
}

/* 
Configure on-board Camera Control System.

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                mode                      : Mode enumeration from 1 to N //P, TV, AV, M, Etc (0 means ignore) (uint8_t)
                shutter_speed             : Divisor number //e.g. 1000 means 1/1000 (0 means ignore) (uint16_t)
                aperture                  : F stop number x 10 //e.g. 28 means 2.8 (0 means ignore) (uint8_t)
                iso                       : ISO enumeration from 1 to N //e.g. 80, 100, 200, Etc (0 means ignore) (uint8_t)
                exposure_type             : Exposure type enumeration from 1 to N (0 means ignore) (uint8_t)
                command_id                : Command Identity (incremental loop: 0 to 255)//A command sent multiple times will be executed or pooled just once (uint8_t)
                engine_cut_off            : Main engine cut-off time before camera trigger in seconds/10 (0 means no cut-off) (uint8_t)
                extra_param               : Extra parameters enumeration (0 means ignore) (uint8_t)
                extra_value               : Correspondent value to given extra_param (float)

*/
mavlink.messages.digicam_configure = function(target_system, target_component, mode, shutter_speed, aperture, iso, exposure_type, command_id, engine_cut_off, extra_param, extra_value) {

    this.format = '<fHBBBBBBBBB';
    this.id = mavlink.MAVLINK_MSG_ID_DIGICAM_CONFIGURE;
    this.order_map = [2, 3, 4, 1, 5, 6, 7, 8, 9, 10, 0];
    this.crc_extra = 84;
    this.name = 'DIGICAM_CONFIGURE';

    this.fieldnames = ['target_system', 'target_component', 'mode', 'shutter_speed', 'aperture', 'iso', 'exposure_type', 'command_id', 'engine_cut_off', 'extra_param', 'extra_value'];


    this.set(arguments);

}
        
mavlink.messages.digicam_configure.prototype = new mavlink.message;

mavlink.messages.digicam_configure.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.extra_value, this.shutter_speed, this.target_system, this.target_component, this.mode, this.aperture, this.iso, this.exposure_type, this.command_id, this.engine_cut_off, this.extra_param]));
}

/* 
Control on-board Camera Control System to take shots.

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                session                   : 0: stop, 1: start or keep it up //Session control e.g. show/hide lens (uint8_t)
                zoom_pos                  : 1 to N //Zoom's absolute position (0 means ignore) (uint8_t)
                zoom_step                 : -100 to 100 //Zooming step value to offset zoom from the current position (int8_t)
                focus_lock                : 0: unlock focus or keep unlocked, 1: lock focus or keep locked, 3: re-lock focus (uint8_t)
                shot                      : 0: ignore, 1: shot or start filming (uint8_t)
                command_id                : Command Identity (incremental loop: 0 to 255)//A command sent multiple times will be executed or pooled just once (uint8_t)
                extra_param               : Extra parameters enumeration (0 means ignore) (uint8_t)
                extra_value               : Correspondent value to given extra_param (float)

*/
mavlink.messages.digicam_control = function(target_system, target_component, session, zoom_pos, zoom_step, focus_lock, shot, command_id, extra_param, extra_value) {

    this.format = '<fBBBBbBBBB';
    this.id = mavlink.MAVLINK_MSG_ID_DIGICAM_CONTROL;
    this.order_map = [1, 2, 3, 4, 5, 6, 7, 8, 9, 0];
    this.crc_extra = 22;
    this.name = 'DIGICAM_CONTROL';

    this.fieldnames = ['target_system', 'target_component', 'session', 'zoom_pos', 'zoom_step', 'focus_lock', 'shot', 'command_id', 'extra_param', 'extra_value'];


    this.set(arguments);

}
        
mavlink.messages.digicam_control.prototype = new mavlink.message;

mavlink.messages.digicam_control.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.extra_value, this.target_system, this.target_component, this.session, this.zoom_pos, this.zoom_step, this.focus_lock, this.shot, this.command_id, this.extra_param]));
}

/* 
Message to configure a camera mount, directional antenna, etc.

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                mount_mode                : mount operating mode (see MAV_MOUNT_MODE enum) (uint8_t)
                stab_roll                 : (1 = yes, 0 = no) (uint8_t)
                stab_pitch                : (1 = yes, 0 = no) (uint8_t)
                stab_yaw                  : (1 = yes, 0 = no) (uint8_t)

*/
mavlink.messages.mount_configure = function(target_system, target_component, mount_mode, stab_roll, stab_pitch, stab_yaw) {

    this.format = '<BBBBBB';
    this.id = mavlink.MAVLINK_MSG_ID_MOUNT_CONFIGURE;
    this.order_map = [0, 1, 2, 3, 4, 5];
    this.crc_extra = 19;
    this.name = 'MOUNT_CONFIGURE';

    this.fieldnames = ['target_system', 'target_component', 'mount_mode', 'stab_roll', 'stab_pitch', 'stab_yaw'];


    this.set(arguments);

}
        
mavlink.messages.mount_configure.prototype = new mavlink.message;

mavlink.messages.mount_configure.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.target_system, this.target_component, this.mount_mode, this.stab_roll, this.stab_pitch, this.stab_yaw]));
}

/* 
Message to control a camera mount, directional antenna, etc.

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                input_a                   : pitch(deg*100) or lat, depending on mount mode (int32_t)
                input_b                   : roll(deg*100) or lon depending on mount mode (int32_t)
                input_c                   : yaw(deg*100) or alt (in cm) depending on mount mode (int32_t)
                save_position             : if "1" it will save current trimmed position on EEPROM (just valid for NEUTRAL and LANDING) (uint8_t)

*/
mavlink.messages.mount_control = function(target_system, target_component, input_a, input_b, input_c, save_position) {

    this.format = '<iiiBBB';
    this.id = mavlink.MAVLINK_MSG_ID_MOUNT_CONTROL;
    this.order_map = [3, 4, 0, 1, 2, 5];
    this.crc_extra = 21;
    this.name = 'MOUNT_CONTROL';

    this.fieldnames = ['target_system', 'target_component', 'input_a', 'input_b', 'input_c', 'save_position'];


    this.set(arguments);

}
        
mavlink.messages.mount_control.prototype = new mavlink.message;

mavlink.messages.mount_control.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.input_a, this.input_b, this.input_c, this.target_system, this.target_component, this.save_position]));
}

/* 
Message with some status from APM to GCS about camera or antenna mount

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                pointing_a                : pitch(deg*100) (int32_t)
                pointing_b                : roll(deg*100) (int32_t)
                pointing_c                : yaw(deg*100) (int32_t)

*/
mavlink.messages.mount_status = function(target_system, target_component, pointing_a, pointing_b, pointing_c) {

    this.format = '<iiiBB';
    this.id = mavlink.MAVLINK_MSG_ID_MOUNT_STATUS;
    this.order_map = [3, 4, 0, 1, 2];
    this.crc_extra = 134;
    this.name = 'MOUNT_STATUS';

    this.fieldnames = ['target_system', 'target_component', 'pointing_a', 'pointing_b', 'pointing_c'];


    this.set(arguments);

}
        
mavlink.messages.mount_status.prototype = new mavlink.message;

mavlink.messages.mount_status.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.pointing_a, this.pointing_b, this.pointing_c, this.target_system, this.target_component]));
}

/* 
A fence point. Used to set a point when from               GCS -> MAV.
Also used to return a point from MAV -> GCS

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                idx                       : point index (first point is 1, 0 is for return point) (uint8_t)
                count                     : total number of points (for sanity checking) (uint8_t)
                lat                       : Latitude of point (float)
                lng                       : Longitude of point (float)

*/
mavlink.messages.fence_point = function(target_system, target_component, idx, count, lat, lng) {

    this.format = '<ffBBBB';
    this.id = mavlink.MAVLINK_MSG_ID_FENCE_POINT;
    this.order_map = [2, 3, 4, 5, 0, 1];
    this.crc_extra = 78;
    this.name = 'FENCE_POINT';

    this.fieldnames = ['target_system', 'target_component', 'idx', 'count', 'lat', 'lng'];


    this.set(arguments);

}
        
mavlink.messages.fence_point.prototype = new mavlink.message;

mavlink.messages.fence_point.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.lat, this.lng, this.target_system, this.target_component, this.idx, this.count]));
}

/* 
Request a current fence point from MAV

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                idx                       : point index (first point is 1, 0 is for return point) (uint8_t)

*/
mavlink.messages.fence_fetch_point = function(target_system, target_component, idx) {

    this.format = '<BBB';
    this.id = mavlink.MAVLINK_MSG_ID_FENCE_FETCH_POINT;
    this.order_map = [0, 1, 2];
    this.crc_extra = 68;
    this.name = 'FENCE_FETCH_POINT';

    this.fieldnames = ['target_system', 'target_component', 'idx'];


    this.set(arguments);

}
        
mavlink.messages.fence_fetch_point.prototype = new mavlink.message;

mavlink.messages.fence_fetch_point.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.target_system, this.target_component, this.idx]));
}

/* 
Status of geo-fencing. Sent in extended             status stream when
fencing enabled

                breach_status             : 0 if currently inside fence, 1 if outside (uint8_t)
                breach_count              : number of fence breaches (uint16_t)
                breach_type               : last breach type (see FENCE_BREACH_* enum) (uint8_t)
                breach_time               : time of last breach in milliseconds since boot (uint32_t)

*/
mavlink.messages.fence_status = function(breach_status, breach_count, breach_type, breach_time) {

    this.format = '<IHBB';
    this.id = mavlink.MAVLINK_MSG_ID_FENCE_STATUS;
    this.order_map = [2, 1, 3, 0];
    this.crc_extra = 189;
    this.name = 'FENCE_STATUS';

    this.fieldnames = ['breach_status', 'breach_count', 'breach_type', 'breach_time'];


    this.set(arguments);

}
        
mavlink.messages.fence_status.prototype = new mavlink.message;

mavlink.messages.fence_status.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.breach_time, this.breach_count, this.breach_status, this.breach_type]));
}

/* 
Status of DCM attitude estimator

                omegaIx                   : X gyro drift estimate rad/s (float)
                omegaIy                   : Y gyro drift estimate rad/s (float)
                omegaIz                   : Z gyro drift estimate rad/s (float)
                accel_weight              : average accel_weight (float)
                renorm_val                : average renormalisation value (float)
                error_rp                  : average error_roll_pitch value (float)
                error_yaw                 : average error_yaw value (float)

*/
mavlink.messages.ahrs = function(omegaIx, omegaIy, omegaIz, accel_weight, renorm_val, error_rp, error_yaw) {

    this.format = '<fffffff';
    this.id = mavlink.MAVLINK_MSG_ID_AHRS;
    this.order_map = [0, 1, 2, 3, 4, 5, 6];
    this.crc_extra = 127;
    this.name = 'AHRS';

    this.fieldnames = ['omegaIx', 'omegaIy', 'omegaIz', 'accel_weight', 'renorm_val', 'error_rp', 'error_yaw'];


    this.set(arguments);

}
        
mavlink.messages.ahrs.prototype = new mavlink.message;

mavlink.messages.ahrs.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.omegaIx, this.omegaIy, this.omegaIz, this.accel_weight, this.renorm_val, this.error_rp, this.error_yaw]));
}

/* 
Status of simulation environment, if used

                roll                      : Roll angle (rad) (float)
                pitch                     : Pitch angle (rad) (float)
                yaw                       : Yaw angle (rad) (float)
                xacc                      : X acceleration m/s/s (float)
                yacc                      : Y acceleration m/s/s (float)
                zacc                      : Z acceleration m/s/s (float)
                xgyro                     : Angular speed around X axis rad/s (float)
                ygyro                     : Angular speed around Y axis rad/s (float)
                zgyro                     : Angular speed around Z axis rad/s (float)
                lat                       : Latitude in degrees * 1E7 (int32_t)
                lng                       : Longitude in degrees * 1E7 (int32_t)

*/
mavlink.messages.simstate = function(roll, pitch, yaw, xacc, yacc, zacc, xgyro, ygyro, zgyro, lat, lng) {

    this.format = '<fffffffffii';
    this.id = mavlink.MAVLINK_MSG_ID_SIMSTATE;
    this.order_map = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
    this.crc_extra = 154;
    this.name = 'SIMSTATE';

    this.fieldnames = ['roll', 'pitch', 'yaw', 'xacc', 'yacc', 'zacc', 'xgyro', 'ygyro', 'zgyro', 'lat', 'lng'];


    this.set(arguments);

}
        
mavlink.messages.simstate.prototype = new mavlink.message;

mavlink.messages.simstate.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.roll, this.pitch, this.yaw, this.xacc, this.yacc, this.zacc, this.xgyro, this.ygyro, this.zgyro, this.lat, this.lng]));
}

/* 
Status of key hardware

                Vcc                       : board voltage (mV) (uint16_t)
                I2Cerr                    : I2C error count (uint8_t)

*/
mavlink.messages.hwstatus = function(Vcc, I2Cerr) {

    this.format = '<HB';
    this.id = mavlink.MAVLINK_MSG_ID_HWSTATUS;
    this.order_map = [0, 1];
    this.crc_extra = 21;
    this.name = 'HWSTATUS';

    this.fieldnames = ['Vcc', 'I2Cerr'];


    this.set(arguments);

}
        
mavlink.messages.hwstatus.prototype = new mavlink.message;

mavlink.messages.hwstatus.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.Vcc, this.I2Cerr]));
}

/* 
Status generated by radio

                rssi                      : local signal strength (uint8_t)
                remrssi                   : remote signal strength (uint8_t)
                txbuf                     : how full the tx buffer is as a percentage (uint8_t)
                noise                     : background noise level (uint8_t)
                remnoise                  : remote background noise level (uint8_t)
                rxerrors                  : receive errors (uint16_t)
                fixed                     : count of error corrected packets (uint16_t)

*/
mavlink.messages.radio = function(rssi, remrssi, txbuf, noise, remnoise, rxerrors, fixed) {

    this.format = '<HHBBBBB';
    this.id = mavlink.MAVLINK_MSG_ID_RADIO;
    this.order_map = [2, 3, 4, 5, 6, 0, 1];
    this.crc_extra = 21;
    this.name = 'RADIO';

    this.fieldnames = ['rssi', 'remrssi', 'txbuf', 'noise', 'remnoise', 'rxerrors', 'fixed'];


    this.set(arguments);

}
        
mavlink.messages.radio.prototype = new mavlink.message;

mavlink.messages.radio.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.rxerrors, this.fixed, this.rssi, this.remrssi, this.txbuf, this.noise, this.remnoise]));
}

/* 
Status of AP_Limits. Sent in extended             status stream when
AP_Limits is enabled

                limits_state              : state of AP_Limits, (see enum LimitState, LIMITS_STATE) (uint8_t)
                last_trigger              : time of last breach in milliseconds since boot (uint32_t)
                last_action               : time of last recovery action in milliseconds since boot (uint32_t)
                last_recovery             : time of last successful recovery in milliseconds since boot (uint32_t)
                last_clear                : time of last all-clear in milliseconds since boot (uint32_t)
                breach_count              : number of fence breaches (uint16_t)
                mods_enabled              : AP_Limit_Module bitfield of enabled modules, (see enum moduleid or LIMIT_MODULE) (uint8_t)
                mods_required             : AP_Limit_Module bitfield of required modules, (see enum moduleid or LIMIT_MODULE) (uint8_t)
                mods_triggered            : AP_Limit_Module bitfield of triggered modules, (see enum moduleid or LIMIT_MODULE) (uint8_t)

*/
mavlink.messages.limits_status = function(limits_state, last_trigger, last_action, last_recovery, last_clear, breach_count, mods_enabled, mods_required, mods_triggered) {

    this.format = '<IIIIHBBBB';
    this.id = mavlink.MAVLINK_MSG_ID_LIMITS_STATUS;
    this.order_map = [5, 0, 1, 2, 3, 4, 6, 7, 8];
    this.crc_extra = 144;
    this.name = 'LIMITS_STATUS';

    this.fieldnames = ['limits_state', 'last_trigger', 'last_action', 'last_recovery', 'last_clear', 'breach_count', 'mods_enabled', 'mods_required', 'mods_triggered'];


    this.set(arguments);

}
        
mavlink.messages.limits_status.prototype = new mavlink.message;

mavlink.messages.limits_status.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.last_trigger, this.last_action, this.last_recovery, this.last_clear, this.breach_count, this.limits_state, this.mods_enabled, this.mods_required, this.mods_triggered]));
}

/* 
Wind estimation

                direction                 : wind direction that wind is coming from (degrees) (float)
                speed                     : wind speed in ground plane (m/s) (float)
                speed_z                   : vertical wind speed (m/s) (float)

*/
mavlink.messages.wind = function(direction, speed, speed_z) {

    this.format = '<fff';
    this.id = mavlink.MAVLINK_MSG_ID_WIND;
    this.order_map = [0, 1, 2];
    this.crc_extra = 1;
    this.name = 'WIND';

    this.fieldnames = ['direction', 'speed', 'speed_z'];


    this.set(arguments);

}
        
mavlink.messages.wind.prototype = new mavlink.message;

mavlink.messages.wind.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.direction, this.speed, this.speed_z]));
}

/* 
Data packet, size 16

                type                      : data type (uint8_t)
                len                       : data length (uint8_t)
                data                      : raw data (uint8_t)

*/
mavlink.messages.data16 = function(type, len, data) {

    this.format = '<BB16s';
    this.id = mavlink.MAVLINK_MSG_ID_DATA16;
    this.order_map = [0, 1, 2];
    this.crc_extra = 234;
    this.name = 'DATA16';

    this.fieldnames = ['type', 'len', 'data'];


    this.set(arguments);

}
        
mavlink.messages.data16.prototype = new mavlink.message;

mavlink.messages.data16.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.type, this.len, this.data]));
}

/* 
Data packet, size 32

                type                      : data type (uint8_t)
                len                       : data length (uint8_t)
                data                      : raw data (uint8_t)

*/
mavlink.messages.data32 = function(type, len, data) {

    this.format = '<BB32s';
    this.id = mavlink.MAVLINK_MSG_ID_DATA32;
    this.order_map = [0, 1, 2];
    this.crc_extra = 73;
    this.name = 'DATA32';

    this.fieldnames = ['type', 'len', 'data'];


    this.set(arguments);

}
        
mavlink.messages.data32.prototype = new mavlink.message;

mavlink.messages.data32.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.type, this.len, this.data]));
}

/* 
Data packet, size 64

                type                      : data type (uint8_t)
                len                       : data length (uint8_t)
                data                      : raw data (uint8_t)

*/
mavlink.messages.data64 = function(type, len, data) {

    this.format = '<BB64s';
    this.id = mavlink.MAVLINK_MSG_ID_DATA64;
    this.order_map = [0, 1, 2];
    this.crc_extra = 181;
    this.name = 'DATA64';

    this.fieldnames = ['type', 'len', 'data'];


    this.set(arguments);

}
        
mavlink.messages.data64.prototype = new mavlink.message;

mavlink.messages.data64.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.type, this.len, this.data]));
}

/* 
Data packet, size 96

                type                      : data type (uint8_t)
                len                       : data length (uint8_t)
                data                      : raw data (uint8_t)

*/
mavlink.messages.data96 = function(type, len, data) {

    this.format = '<BB96s';
    this.id = mavlink.MAVLINK_MSG_ID_DATA96;
    this.order_map = [0, 1, 2];
    this.crc_extra = 22;
    this.name = 'DATA96';

    this.fieldnames = ['type', 'len', 'data'];


    this.set(arguments);

}
        
mavlink.messages.data96.prototype = new mavlink.message;

mavlink.messages.data96.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.type, this.len, this.data]));
}

/* 
Rangefinder reporting

                distance                  : distance in meters (float)
                voltage                   : raw voltage if available, zero otherwise (float)

*/
mavlink.messages.rangefinder = function(distance, voltage) {

    this.format = '<ff';
    this.id = mavlink.MAVLINK_MSG_ID_RANGEFINDER;
    this.order_map = [0, 1];
    this.crc_extra = 83;
    this.name = 'RANGEFINDER';

    this.fieldnames = ['distance', 'voltage'];


    this.set(arguments);

}
        
mavlink.messages.rangefinder.prototype = new mavlink.message;

mavlink.messages.rangefinder.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.distance, this.voltage]));
}

/* 
Airspeed auto-calibration

                vx                        : GPS velocity north m/s (float)
                vy                        : GPS velocity east m/s (float)
                vz                        : GPS velocity down m/s (float)
                diff_pressure             : Differential pressure pascals (float)
                EAS2TAS                   : Estimated to true airspeed ratio (float)
                ratio                     : Airspeed ratio (float)
                state_x                   : EKF state x (float)
                state_y                   : EKF state y (float)
                state_z                   : EKF state z (float)
                Pax                       : EKF Pax (float)
                Pby                       : EKF Pby (float)
                Pcz                       : EKF Pcz (float)

*/
mavlink.messages.airspeed_autocal = function(vx, vy, vz, diff_pressure, EAS2TAS, ratio, state_x, state_y, state_z, Pax, Pby, Pcz) {

    this.format = '<ffffffffffff';
    this.id = mavlink.MAVLINK_MSG_ID_AIRSPEED_AUTOCAL;
    this.order_map = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];
    this.crc_extra = 167;
    this.name = 'AIRSPEED_AUTOCAL';

    this.fieldnames = ['vx', 'vy', 'vz', 'diff_pressure', 'EAS2TAS', 'ratio', 'state_x', 'state_y', 'state_z', 'Pax', 'Pby', 'Pcz'];


    this.set(arguments);

}
        
mavlink.messages.airspeed_autocal.prototype = new mavlink.message;

mavlink.messages.airspeed_autocal.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.vx, this.vy, this.vz, this.diff_pressure, this.EAS2TAS, this.ratio, this.state_x, this.state_y, this.state_z, this.Pax, this.Pby, this.Pcz]));
}

/* 
A rally point. Used to set a point when from GCS -> MAV. Also used to
return a point from MAV -> GCS

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                idx                       : point index (first point is 0) (uint8_t)
                count                     : total number of points (for sanity checking) (uint8_t)
                lat                       : Latitude of point in degrees * 1E7 (int32_t)
                lng                       : Longitude of point in degrees * 1E7 (int32_t)
                alt                       : Transit / loiter altitude in meters relative to home (int16_t)
                break_alt                 : Break altitude in meters relative to home (int16_t)
                land_dir                  : Heading to aim for when landing. In centi-degrees. (uint16_t)
                flags                     : See RALLY_FLAGS enum for definition of the bitmask. (uint8_t)

*/
mavlink.messages.rally_point = function(target_system, target_component, idx, count, lat, lng, alt, break_alt, land_dir, flags) {

    this.format = '<iihhHBBBBB';
    this.id = mavlink.MAVLINK_MSG_ID_RALLY_POINT;
    this.order_map = [5, 6, 7, 8, 0, 1, 2, 3, 4, 9];
    this.crc_extra = 138;
    this.name = 'RALLY_POINT';

    this.fieldnames = ['target_system', 'target_component', 'idx', 'count', 'lat', 'lng', 'alt', 'break_alt', 'land_dir', 'flags'];


    this.set(arguments);

}
        
mavlink.messages.rally_point.prototype = new mavlink.message;

mavlink.messages.rally_point.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.lat, this.lng, this.alt, this.break_alt, this.land_dir, this.target_system, this.target_component, this.idx, this.count, this.flags]));
}

/* 
Request a current rally point from MAV. MAV should respond with a
RALLY_POINT message. MAV should not respond if the request is invalid.

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                idx                       : point index (first point is 0) (uint8_t)

*/
mavlink.messages.rally_fetch_point = function(target_system, target_component, idx) {

    this.format = '<BBB';
    this.id = mavlink.MAVLINK_MSG_ID_RALLY_FETCH_POINT;
    this.order_map = [0, 1, 2];
    this.crc_extra = 234;
    this.name = 'RALLY_FETCH_POINT';

    this.fieldnames = ['target_system', 'target_component', 'idx'];


    this.set(arguments);

}
        
mavlink.messages.rally_fetch_point.prototype = new mavlink.message;

mavlink.messages.rally_fetch_point.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.target_system, this.target_component, this.idx]));
}

/* 
Status of compassmot calibration

                throttle                  : throttle (percent*10) (uint16_t)
                current                   : current (amps) (float)
                interference              : interference (percent) (uint16_t)
                CompensationX             : Motor Compensation X (float)
                CompensationY             : Motor Compensation Y (float)
                CompensationZ             : Motor Compensation Z (float)

*/
mavlink.messages.compassmot_status = function(throttle, current, interference, CompensationX, CompensationY, CompensationZ) {

    this.format = '<ffffHH';
    this.id = mavlink.MAVLINK_MSG_ID_COMPASSMOT_STATUS;
    this.order_map = [4, 0, 5, 1, 2, 3];
    this.crc_extra = 240;
    this.name = 'COMPASSMOT_STATUS';

    this.fieldnames = ['throttle', 'current', 'interference', 'CompensationX', 'CompensationY', 'CompensationZ'];


    this.set(arguments);

}
        
mavlink.messages.compassmot_status.prototype = new mavlink.message;

mavlink.messages.compassmot_status.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.current, this.CompensationX, this.CompensationY, this.CompensationZ, this.throttle, this.interference]));
}

/* 
Status of secondary AHRS filter if available

                roll                      : Roll angle (rad) (float)
                pitch                     : Pitch angle (rad) (float)
                yaw                       : Yaw angle (rad) (float)
                altitude                  : Altitude (MSL) (float)
                lat                       : Latitude in degrees * 1E7 (int32_t)
                lng                       : Longitude in degrees * 1E7 (int32_t)

*/
mavlink.messages.ahrs2 = function(roll, pitch, yaw, altitude, lat, lng) {

    this.format = '<ffffii';
    this.id = mavlink.MAVLINK_MSG_ID_AHRS2;
    this.order_map = [0, 1, 2, 3, 4, 5];
    this.crc_extra = 47;
    this.name = 'AHRS2';

    this.fieldnames = ['roll', 'pitch', 'yaw', 'altitude', 'lat', 'lng'];


    this.set(arguments);

}
        
mavlink.messages.ahrs2.prototype = new mavlink.message;

mavlink.messages.ahrs2.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.roll, this.pitch, this.yaw, this.altitude, this.lat, this.lng]));
}

/* 
Camera Event

                time_usec                 : Image timestamp (microseconds since UNIX epoch, according to camera clock) (uint64_t)
                target_system             : System ID (uint8_t)
                cam_idx                   : Camera ID (uint8_t)
                img_idx                   : Image index (uint16_t)
                event_id                  : See CAMERA_STATUS_TYPES enum for definition of the bitmask (uint8_t)
                p1                        : Parameter 1 (meaning depends on event, see CAMERA_STATUS_TYPES enum) (float)
                p2                        : Parameter 2 (meaning depends on event, see CAMERA_STATUS_TYPES enum) (float)
                p3                        : Parameter 3 (meaning depends on event, see CAMERA_STATUS_TYPES enum) (float)
                p4                        : Parameter 4 (meaning depends on event, see CAMERA_STATUS_TYPES enum) (float)

*/
mavlink.messages.camera_status = function(time_usec, target_system, cam_idx, img_idx, event_id, p1, p2, p3, p4) {

    this.format = '<QffffHBBB';
    this.id = mavlink.MAVLINK_MSG_ID_CAMERA_STATUS;
    this.order_map = [0, 6, 7, 5, 8, 1, 2, 3, 4];
    this.crc_extra = 189;
    this.name = 'CAMERA_STATUS';

    this.fieldnames = ['time_usec', 'target_system', 'cam_idx', 'img_idx', 'event_id', 'p1', 'p2', 'p3', 'p4'];


    this.set(arguments);

}
        
mavlink.messages.camera_status.prototype = new mavlink.message;

mavlink.messages.camera_status.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_usec, this.p1, this.p2, this.p3, this.p4, this.img_idx, this.target_system, this.cam_idx, this.event_id]));
}

/* 
Camera Capture Feedback

                time_usec                 : Image timestamp (microseconds since UNIX epoch), as passed in by CAMERA_STATUS message (or autopilot if no CCB) (uint64_t)
                target_system             : System ID (uint8_t)
                cam_idx                   : Camera ID (uint8_t)
                img_idx                   : Image index (uint16_t)
                lat                       : Latitude in (deg * 1E7) (int32_t)
                lng                       : Longitude in (deg * 1E7) (int32_t)
                alt_msl                   : Altitude Absolute (meters AMSL) (float)
                alt_rel                   : Altitude Relative (meters above HOME location) (float)
                roll                      : Camera Roll angle (earth frame, degrees, +-180) (float)
                pitch                     : Camera Pitch angle (earth frame, degrees, +-180) (float)
                yaw                       : Camera Yaw (earth frame, degrees, 0-360, true) (float)
                foc_len                   : Focal Length (mm) (float)
                flags                     : See CAMERA_FEEDBACK_FLAGS enum for definition of the bitmask (uint8_t)

*/
mavlink.messages.camera_feedback = function(time_usec, target_system, cam_idx, img_idx, lat, lng, alt_msl, alt_rel, roll, pitch, yaw, foc_len, flags) {

    this.format = '<QiiffffffHBBB';
    this.id = mavlink.MAVLINK_MSG_ID_CAMERA_FEEDBACK;
    this.order_map = [0, 10, 11, 9, 1, 2, 3, 4, 5, 6, 7, 8, 12];
    this.crc_extra = 52;
    this.name = 'CAMERA_FEEDBACK';

    this.fieldnames = ['time_usec', 'target_system', 'cam_idx', 'img_idx', 'lat', 'lng', 'alt_msl', 'alt_rel', 'roll', 'pitch', 'yaw', 'foc_len', 'flags'];


    this.set(arguments);

}
        
mavlink.messages.camera_feedback.prototype = new mavlink.message;

mavlink.messages.camera_feedback.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_usec, this.lat, this.lng, this.alt_msl, this.alt_rel, this.roll, this.pitch, this.yaw, this.foc_len, this.img_idx, this.target_system, this.cam_idx, this.flags]));
}

/* 
2nd Battery status

                voltage                   : voltage in millivolts (uint16_t)
                current_battery           : Battery current, in 10*milliamperes (1 = 10 milliampere), -1: autopilot does not measure the current (int16_t)

*/
mavlink.messages.battery2 = function(voltage, current_battery) {

    this.format = '<Hh';
    this.id = mavlink.MAVLINK_MSG_ID_BATTERY2;
    this.order_map = [0, 1];
    this.crc_extra = 174;
    this.name = 'BATTERY2';

    this.fieldnames = ['voltage', 'current_battery'];


    this.set(arguments);

}
        
mavlink.messages.battery2.prototype = new mavlink.message;

mavlink.messages.battery2.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.voltage, this.current_battery]));
}

/* 
Status of third AHRS filter if available. This is for ANU research
group (Ali and Sean)

                roll                      : Roll angle (rad) (float)
                pitch                     : Pitch angle (rad) (float)
                yaw                       : Yaw angle (rad) (float)
                altitude                  : Altitude (MSL) (float)
                lat                       : Latitude in degrees * 1E7 (int32_t)
                lng                       : Longitude in degrees * 1E7 (int32_t)
                v1                        : test variable1 (float)
                v2                        : test variable2 (float)
                v3                        : test variable3 (float)
                v4                        : test variable4 (float)

*/
mavlink.messages.ahrs3 = function(roll, pitch, yaw, altitude, lat, lng, v1, v2, v3, v4) {

    this.format = '<ffffiiffff';
    this.id = mavlink.MAVLINK_MSG_ID_AHRS3;
    this.order_map = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
    this.crc_extra = 229;
    this.name = 'AHRS3';

    this.fieldnames = ['roll', 'pitch', 'yaw', 'altitude', 'lat', 'lng', 'v1', 'v2', 'v3', 'v4'];


    this.set(arguments);

}
        
mavlink.messages.ahrs3.prototype = new mavlink.message;

mavlink.messages.ahrs3.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.roll, this.pitch, this.yaw, this.altitude, this.lat, this.lng, this.v1, this.v2, this.v3, this.v4]));
}

/* 
Request the autopilot version from the system/component.

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)

*/
mavlink.messages.autopilot_version_request = function(target_system, target_component) {

    this.format = '<BB';
    this.id = mavlink.MAVLINK_MSG_ID_AUTOPILOT_VERSION_REQUEST;
    this.order_map = [0, 1];
    this.crc_extra = 85;
    this.name = 'AUTOPILOT_VERSION_REQUEST';

    this.fieldnames = ['target_system', 'target_component'];


    this.set(arguments);

}
        
mavlink.messages.autopilot_version_request.prototype = new mavlink.message;

mavlink.messages.autopilot_version_request.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.target_system, this.target_component]));
}

/* 
Control vehicle LEDs

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                instance                  : Instance (LED instance to control or 255 for all LEDs) (uint8_t)
                pattern                   : Pattern (see LED_PATTERN_ENUM) (uint8_t)
                custom_len                : Custom Byte Length (uint8_t)
                custom_bytes              : Custom Bytes (uint8_t)

*/
mavlink.messages.led_control = function(target_system, target_component, instance, pattern, custom_len, custom_bytes) {

    this.format = '<BBBBB24s';
    this.id = mavlink.MAVLINK_MSG_ID_LED_CONTROL;
    this.order_map = [0, 1, 2, 3, 4, 5];
    this.crc_extra = 72;
    this.name = 'LED_CONTROL';

    this.fieldnames = ['target_system', 'target_component', 'instance', 'pattern', 'custom_len', 'custom_bytes'];


    this.set(arguments);

}
        
mavlink.messages.led_control.prototype = new mavlink.message;

mavlink.messages.led_control.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.target_system, this.target_component, this.instance, this.pattern, this.custom_len, this.custom_bytes]));
}

/* 
Reports progress of compass calibration.

                compass_id                : Compass being calibrated (uint8_t)
                cal_mask                  : Bitmask of compasses being calibrated (uint8_t)
                cal_status                : Status (see MAG_CAL_STATUS enum) (uint8_t)
                attempt                   : Attempt number (uint8_t)
                completion_pct            : Completion percentage (uint8_t)
                completion_mask           : Bitmask of sphere sections (see http://en.wikipedia.org/wiki/Geodesic_grid) (uint8_t)
                direction_x               : Body frame direction vector for display (float)
                direction_y               : Body frame direction vector for display (float)
                direction_z               : Body frame direction vector for display (float)

*/
mavlink.messages.mag_cal_progress = function(compass_id, cal_mask, cal_status, attempt, completion_pct, completion_mask, direction_x, direction_y, direction_z) {

    this.format = '<fffBBBBB10s';
    this.id = mavlink.MAVLINK_MSG_ID_MAG_CAL_PROGRESS;
    this.order_map = [3, 4, 5, 6, 7, 8, 0, 1, 2];
    this.crc_extra = 92;
    this.name = 'MAG_CAL_PROGRESS';

    this.fieldnames = ['compass_id', 'cal_mask', 'cal_status', 'attempt', 'completion_pct', 'completion_mask', 'direction_x', 'direction_y', 'direction_z'];


    this.set(arguments);

}
        
mavlink.messages.mag_cal_progress.prototype = new mavlink.message;

mavlink.messages.mag_cal_progress.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.direction_x, this.direction_y, this.direction_z, this.compass_id, this.cal_mask, this.cal_status, this.attempt, this.completion_pct, this.completion_mask]));
}

/* 
Reports results of completed compass calibration. Sent until
MAG_CAL_ACK received.

                compass_id                : Compass being calibrated (uint8_t)
                cal_mask                  : Bitmask of compasses being calibrated (uint8_t)
                cal_status                : Status (see MAG_CAL_STATUS enum) (uint8_t)
                autosaved                 : 0=requires a MAV_CMD_DO_ACCEPT_MAG_CAL, 1=saved to parameters (uint8_t)
                fitness                   : RMS milligauss residuals (float)
                ofs_x                     : X offset (float)
                ofs_y                     : Y offset (float)
                ofs_z                     : Z offset (float)
                diag_x                    : X diagonal (matrix 11) (float)
                diag_y                    : Y diagonal (matrix 22) (float)
                diag_z                    : Z diagonal (matrix 33) (float)
                offdiag_x                 : X off-diagonal (matrix 12 and 21) (float)
                offdiag_y                 : Y off-diagonal (matrix 13 and 31) (float)
                offdiag_z                 : Z off-diagonal (matrix 32 and 23) (float)

*/
mavlink.messages.mag_cal_report = function(compass_id, cal_mask, cal_status, autosaved, fitness, ofs_x, ofs_y, ofs_z, diag_x, diag_y, diag_z, offdiag_x, offdiag_y, offdiag_z) {

    this.format = '<ffffffffffBBBB';
    this.id = mavlink.MAVLINK_MSG_ID_MAG_CAL_REPORT;
    this.order_map = [10, 11, 12, 13, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
    this.crc_extra = 36;
    this.name = 'MAG_CAL_REPORT';

    this.fieldnames = ['compass_id', 'cal_mask', 'cal_status', 'autosaved', 'fitness', 'ofs_x', 'ofs_y', 'ofs_z', 'diag_x', 'diag_y', 'diag_z', 'offdiag_x', 'offdiag_y', 'offdiag_z'];


    this.set(arguments);

}
        
mavlink.messages.mag_cal_report.prototype = new mavlink.message;

mavlink.messages.mag_cal_report.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.fitness, this.ofs_x, this.ofs_y, this.ofs_z, this.diag_x, this.diag_y, this.diag_z, this.offdiag_x, this.offdiag_y, this.offdiag_z, this.compass_id, this.cal_mask, this.cal_status, this.autosaved]));
}

/* 
EKF Status message including flags and variances

                flags                     : Flags (uint16_t)
                velocity_variance         : Velocity variance (float)
                pos_horiz_variance        : Horizontal Position variance (float)
                pos_vert_variance         : Vertical Position variance (float)
                compass_variance          : Compass variance (float)
                terrain_alt_variance        : Terrain Altitude variance (float)

*/
mavlink.messages.ekf_status_report = function(flags, velocity_variance, pos_horiz_variance, pos_vert_variance, compass_variance, terrain_alt_variance) {

    this.format = '<fffffH';
    this.id = mavlink.MAVLINK_MSG_ID_EKF_STATUS_REPORT;
    this.order_map = [5, 0, 1, 2, 3, 4];
    this.crc_extra = 71;
    this.name = 'EKF_STATUS_REPORT';

    this.fieldnames = ['flags', 'velocity_variance', 'pos_horiz_variance', 'pos_vert_variance', 'compass_variance', 'terrain_alt_variance'];


    this.set(arguments);

}
        
mavlink.messages.ekf_status_report.prototype = new mavlink.message;

mavlink.messages.ekf_status_report.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.velocity_variance, this.pos_horiz_variance, this.pos_vert_variance, this.compass_variance, this.terrain_alt_variance, this.flags]));
}

/* 
3 axis gimbal mesuraments

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                delta_time                : Time since last update (seconds) (float)
                delta_angle_x             : Delta angle X (radians) (float)
                delta_angle_y             : Delta angle Y (radians) (float)
                delta_angle_z             : Delta angle X (radians) (float)
                delta_velocity_x          : Delta velocity X (m/s) (float)
                delta_velocity_y          : Delta velocity Y (m/s) (float)
                delta_velocity_z          : Delta velocity Z (m/s) (float)
                joint_roll                : Joint ROLL (radians) (float)
                joint_el                  : Joint EL (radians) (float)
                joint_az                  : Joint AZ (radians) (float)

*/
mavlink.messages.gimbal_report = function(target_system, target_component, delta_time, delta_angle_x, delta_angle_y, delta_angle_z, delta_velocity_x, delta_velocity_y, delta_velocity_z, joint_roll, joint_el, joint_az) {

    this.format = '<ffffffffffBB';
    this.id = mavlink.MAVLINK_MSG_ID_GIMBAL_REPORT;
    this.order_map = [10, 11, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
    this.crc_extra = 134;
    this.name = 'GIMBAL_REPORT';

    this.fieldnames = ['target_system', 'target_component', 'delta_time', 'delta_angle_x', 'delta_angle_y', 'delta_angle_z', 'delta_velocity_x', 'delta_velocity_y', 'delta_velocity_z', 'joint_roll', 'joint_el', 'joint_az'];


    this.set(arguments);

}
        
mavlink.messages.gimbal_report.prototype = new mavlink.message;

mavlink.messages.gimbal_report.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.delta_time, this.delta_angle_x, this.delta_angle_y, this.delta_angle_z, this.delta_velocity_x, this.delta_velocity_y, this.delta_velocity_z, this.joint_roll, this.joint_el, this.joint_az, this.target_system, this.target_component]));
}

/* 
Control message for rate gimbal

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                demanded_rate_x           : Demanded angular rate X (rad/s) (float)
                demanded_rate_y           : Demanded angular rate Y (rad/s) (float)
                demanded_rate_z           : Demanded angular rate Z (rad/s) (float)

*/
mavlink.messages.gimbal_control = function(target_system, target_component, demanded_rate_x, demanded_rate_y, demanded_rate_z) {

    this.format = '<fffBB';
    this.id = mavlink.MAVLINK_MSG_ID_GIMBAL_CONTROL;
    this.order_map = [3, 4, 0, 1, 2];
    this.crc_extra = 205;
    this.name = 'GIMBAL_CONTROL';

    this.fieldnames = ['target_system', 'target_component', 'demanded_rate_x', 'demanded_rate_y', 'demanded_rate_z'];


    this.set(arguments);

}
        
mavlink.messages.gimbal_control.prototype = new mavlink.message;

mavlink.messages.gimbal_control.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.demanded_rate_x, this.demanded_rate_y, this.demanded_rate_z, this.target_system, this.target_component]));
}

/* 
Causes the gimbal to reset and boot as if it was just powered on

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)

*/
mavlink.messages.gimbal_reset = function(target_system, target_component) {

    this.format = '<BB';
    this.id = mavlink.MAVLINK_MSG_ID_GIMBAL_RESET;
    this.order_map = [0, 1];
    this.crc_extra = 94;
    this.name = 'GIMBAL_RESET';

    this.fieldnames = ['target_system', 'target_component'];


    this.set(arguments);

}
        
mavlink.messages.gimbal_reset.prototype = new mavlink.message;

mavlink.messages.gimbal_reset.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.target_system, this.target_component]));
}

/* 
Reports progress and success or failure of gimbal axis calibration
procedure

                calibration_axis          : Which gimbal axis we're reporting calibration progress for (uint8_t)
                calibration_progress        : The current calibration progress for this axis, 0x64=100% (uint8_t)
                calibration_status        : The status of the running calibration (uint8_t)

*/
mavlink.messages.gimbal_axis_calibration_progress = function(calibration_axis, calibration_progress, calibration_status) {

    this.format = '<BBB';
    this.id = mavlink.MAVLINK_MSG_ID_GIMBAL_AXIS_CALIBRATION_PROGRESS;
    this.order_map = [0, 1, 2];
    this.crc_extra = 128;
    this.name = 'GIMBAL_AXIS_CALIBRATION_PROGRESS';

    this.fieldnames = ['calibration_axis', 'calibration_progress', 'calibration_status'];


    this.set(arguments);

}
        
mavlink.messages.gimbal_axis_calibration_progress.prototype = new mavlink.message;

mavlink.messages.gimbal_axis_calibration_progress.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.calibration_axis, this.calibration_progress, this.calibration_status]));
}

/* 
Instructs the gimbal to set its current position as its new home
position.  Will primarily be used for factory calibration

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)

*/
mavlink.messages.gimbal_set_home_offsets = function(target_system, target_component) {

    this.format = '<BB';
    this.id = mavlink.MAVLINK_MSG_ID_GIMBAL_SET_HOME_OFFSETS;
    this.order_map = [0, 1];
    this.crc_extra = 54;
    this.name = 'GIMBAL_SET_HOME_OFFSETS';

    this.fieldnames = ['target_system', 'target_component'];


    this.set(arguments);

}
        
mavlink.messages.gimbal_set_home_offsets.prototype = new mavlink.message;

mavlink.messages.gimbal_set_home_offsets.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.target_system, this.target_component]));
}

/* 
Sent by the gimbal after it receives a SET_HOME_OFFSETS message to
indicate the result of the home offset calibration

                calibration_result        : The result of the home offset calibration (uint8_t)

*/
mavlink.messages.gimbal_home_offset_calibration_result = function(calibration_result) {

    this.format = '<B';
    this.id = mavlink.MAVLINK_MSG_ID_GIMBAL_HOME_OFFSET_CALIBRATION_RESULT;
    this.order_map = [0];
    this.crc_extra = 63;
    this.name = 'GIMBAL_HOME_OFFSET_CALIBRATION_RESULT';

    this.fieldnames = ['calibration_result'];


    this.set(arguments);

}
        
mavlink.messages.gimbal_home_offset_calibration_result.prototype = new mavlink.message;

mavlink.messages.gimbal_home_offset_calibration_result.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.calibration_result]));
}

/* 
Set factory configuration parameters (such as assembly date and time,
and serial number).  This is only intended to be used
during manufacture, not by end users, so it is protected by a simple
checksum of sorts (this won't stop anybody determined,
it's mostly just to keep the average user from trying to modify these
values.  This will need to be revisited if that isn't
adequate.

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                magic_1                   : Magic number 1 for validation (uint32_t)
                magic_2                   : Magic number 2 for validation (uint32_t)
                magic_3                   : Magic number 3 for validation (uint32_t)
                assembly_year             : Assembly Date Year (uint16_t)
                assembly_month            : Assembly Date Month (uint8_t)
                assembly_day              : Assembly Date Day (uint8_t)
                assembly_hour             : Assembly Time Hour (uint8_t)
                assembly_minute           : Assembly Time Minute (uint8_t)
                assembly_second           : Assembly Time Second (uint8_t)
                serial_number_pt_1        : Unit Serial Number Part 1 (part code, design, language/country) (uint32_t)
                serial_number_pt_2        : Unit Serial Number Part 2 (option, year, month) (uint32_t)
                serial_number_pt_3        : Unit Serial Number Part 3 (incrementing serial number per month) (uint32_t)

*/
mavlink.messages.gimbal_set_factory_parameters = function(target_system, target_component, magic_1, magic_2, magic_3, assembly_year, assembly_month, assembly_day, assembly_hour, assembly_minute, assembly_second, serial_number_pt_1, serial_number_pt_2, serial_number_pt_3) {

    this.format = '<IIIIIIHBBBBBBB';
    this.id = mavlink.MAVLINK_MSG_ID_GIMBAL_SET_FACTORY_PARAMETERS;
    this.order_map = [7, 8, 0, 1, 2, 6, 9, 10, 11, 12, 13, 3, 4, 5];
    this.crc_extra = 112;
    this.name = 'GIMBAL_SET_FACTORY_PARAMETERS';

    this.fieldnames = ['target_system', 'target_component', 'magic_1', 'magic_2', 'magic_3', 'assembly_year', 'assembly_month', 'assembly_day', 'assembly_hour', 'assembly_minute', 'assembly_second', 'serial_number_pt_1', 'serial_number_pt_2', 'serial_number_pt_3'];


    this.set(arguments);

}
        
mavlink.messages.gimbal_set_factory_parameters.prototype = new mavlink.message;

mavlink.messages.gimbal_set_factory_parameters.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.magic_1, this.magic_2, this.magic_3, this.serial_number_pt_1, this.serial_number_pt_2, this.serial_number_pt_3, this.assembly_year, this.target_system, this.target_component, this.assembly_month, this.assembly_day, this.assembly_hour, this.assembly_minute, this.assembly_second]));
}

/* 
Sent by the gimbal after the factory parameters are successfully
loaded, to inform the factory software that the load is complete

                dummy                     : Dummy field because mavgen doesn't allow messages with no fields (uint8_t)

*/
mavlink.messages.gimbal_factory_parameters_loaded = function(dummy) {

    this.format = '<B';
    this.id = mavlink.MAVLINK_MSG_ID_GIMBAL_FACTORY_PARAMETERS_LOADED;
    this.order_map = [0];
    this.crc_extra = 201;
    this.name = 'GIMBAL_FACTORY_PARAMETERS_LOADED';

    this.fieldnames = ['dummy'];


    this.set(arguments);

}
        
mavlink.messages.gimbal_factory_parameters_loaded.prototype = new mavlink.message;

mavlink.messages.gimbal_factory_parameters_loaded.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.dummy]));
}

/* 
Commands the gimbal to erase its firmware image and flash
configuration, leaving only the bootloader.  The gimbal will then
reboot into the bootloader,             ready for the load of a new
application firmware image.  Erasing the flash configuration will
cause the gimbal to re-perform axis calibration when a             new
firmware image is loaded, and will cause all tuning parameters to
return to their factory defaults.  WARNING: sending this command will
render a             gimbal inoperable until a new firmware image is
loaded onto it.  For this reason, a particular "knock" value must be
sent for the command to take effect.             Use this command at
your own risk

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                knock                     : Knock value to confirm this is a valid request (uint32_t)

*/
mavlink.messages.gimbal_erase_firmware_and_config = function(target_system, target_component, knock) {

    this.format = '<IBB';
    this.id = mavlink.MAVLINK_MSG_ID_GIMBAL_ERASE_FIRMWARE_AND_CONFIG;
    this.order_map = [1, 2, 0];
    this.crc_extra = 221;
    this.name = 'GIMBAL_ERASE_FIRMWARE_AND_CONFIG';

    this.fieldnames = ['target_system', 'target_component', 'knock'];


    this.set(arguments);

}
        
mavlink.messages.gimbal_erase_firmware_and_config.prototype = new mavlink.message;

mavlink.messages.gimbal_erase_firmware_and_config.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.knock, this.target_system, this.target_component]));
}

/* 
Command the gimbal to perform a series of factory tests.  Should not
be needed by end users

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)

*/
mavlink.messages.gimbal_perform_factory_tests = function(target_system, target_component) {

    this.format = '<BB';
    this.id = mavlink.MAVLINK_MSG_ID_GIMBAL_PERFORM_FACTORY_TESTS;
    this.order_map = [0, 1];
    this.crc_extra = 226;
    this.name = 'GIMBAL_PERFORM_FACTORY_TESTS';

    this.fieldnames = ['target_system', 'target_component'];


    this.set(arguments);

}
        
mavlink.messages.gimbal_perform_factory_tests.prototype = new mavlink.message;

mavlink.messages.gimbal_perform_factory_tests.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.target_system, this.target_component]));
}

/* 
Reports the current status of a section of a running factory test

                test                      : Which factory test is currently running (uint8_t)
                test_section              : Which section of the test is currently running.  The meaning of this is test-dependent (uint8_t)
                test_section_progress        : The progress of the current test section, 0x64=100% (uint8_t)
                test_status               : The status of the currently executing test section.  The meaning of this is test and section-dependent (uint8_t)

*/
mavlink.messages.gimbal_report_factory_tests_progress = function(test, test_section, test_section_progress, test_status) {

    this.format = '<BBBB';
    this.id = mavlink.MAVLINK_MSG_ID_GIMBAL_REPORT_FACTORY_TESTS_PROGRESS;
    this.order_map = [0, 1, 2, 3];
    this.crc_extra = 238;
    this.name = 'GIMBAL_REPORT_FACTORY_TESTS_PROGRESS';

    this.fieldnames = ['test', 'test_section', 'test_section_progress', 'test_status'];


    this.set(arguments);

}
        
mavlink.messages.gimbal_report_factory_tests_progress.prototype = new mavlink.message;

mavlink.messages.gimbal_report_factory_tests_progress.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.test, this.test_section, this.test_section_progress, this.test_status]));
}

/* 
Instruct a HeroBus attached GoPro to power on

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)

*/
mavlink.messages.gopro_power_on = function(target_system, target_component) {

    this.format = '<BB';
    this.id = mavlink.MAVLINK_MSG_ID_GOPRO_POWER_ON;
    this.order_map = [0, 1];
    this.crc_extra = 241;
    this.name = 'GOPRO_POWER_ON';

    this.fieldnames = ['target_system', 'target_component'];


    this.set(arguments);

}
        
mavlink.messages.gopro_power_on.prototype = new mavlink.message;

mavlink.messages.gopro_power_on.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.target_system, this.target_component]));
}

/* 
Instruct a HeroBus attached GoPro to power off

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)

*/
mavlink.messages.gopro_power_off = function(target_system, target_component) {

    this.format = '<BB';
    this.id = mavlink.MAVLINK_MSG_ID_GOPRO_POWER_OFF;
    this.order_map = [0, 1];
    this.crc_extra = 155;
    this.name = 'GOPRO_POWER_OFF';

    this.fieldnames = ['target_system', 'target_component'];


    this.set(arguments);

}
        
mavlink.messages.gopro_power_off.prototype = new mavlink.message;

mavlink.messages.gopro_power_off.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.target_system, this.target_component]));
}

/* 
Send a command to a HeroBus attached GoPro.  Will generate a
GOPRO_RESPONSE message with results of the command

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                gp_cmd_name_1             : First character of the 2 character GoPro command (uint8_t)
                gp_cmd_name_2             : Second character of the 2 character GoPro command (uint8_t)
                gp_cmd_parm               : Parameter for the command (uint8_t)

*/
mavlink.messages.gopro_command = function(target_system, target_component, gp_cmd_name_1, gp_cmd_name_2, gp_cmd_parm) {

    this.format = '<BBBBB';
    this.id = mavlink.MAVLINK_MSG_ID_GOPRO_COMMAND;
    this.order_map = [0, 1, 2, 3, 4];
    this.crc_extra = 43;
    this.name = 'GOPRO_COMMAND';

    this.fieldnames = ['target_system', 'target_component', 'gp_cmd_name_1', 'gp_cmd_name_2', 'gp_cmd_parm'];


    this.set(arguments);

}
        
mavlink.messages.gopro_command.prototype = new mavlink.message;

mavlink.messages.gopro_command.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.target_system, this.target_component, this.gp_cmd_name_1, this.gp_cmd_name_2, this.gp_cmd_parm]));
}

/* 
Response to a command sent to a HeroBus attached GoPro with a
GOPRO_COMMAND message.  Contains response from the camera as well as
information about any errors encountered while attempting to
communicate with the camera

                gp_cmd_name_1             : First character of the 2 character GoPro command that generated this response (uint8_t)
                gp_cmd_name_2             : Second character of the 2 character GoPro command that generated this response (uint8_t)
                gp_cmd_response_status        : Response byte from the GoPro's response to the command.  0 = Success, 1 = Failure (uint8_t)
                gp_cmd_response_argument        : Response argument from the GoPro's response to the command (uint8_t)
                gp_cmd_result             : Result of the command attempt to the GoPro, as defined by GOPRO_CMD_RESULT enum. (uint16_t)

*/
mavlink.messages.gopro_response = function(gp_cmd_name_1, gp_cmd_name_2, gp_cmd_response_status, gp_cmd_response_argument, gp_cmd_result) {

    this.format = '<HBBBB';
    this.id = mavlink.MAVLINK_MSG_ID_GOPRO_RESPONSE;
    this.order_map = [1, 2, 3, 4, 0];
    this.crc_extra = 149;
    this.name = 'GOPRO_RESPONSE';

    this.fieldnames = ['gp_cmd_name_1', 'gp_cmd_name_2', 'gp_cmd_response_status', 'gp_cmd_response_argument', 'gp_cmd_result'];


    this.set(arguments);

}
        
mavlink.messages.gopro_response.prototype = new mavlink.message;

mavlink.messages.gopro_response.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.gp_cmd_result, this.gp_cmd_name_1, this.gp_cmd_name_2, this.gp_cmd_response_status, this.gp_cmd_response_argument]));
}

/* 
The heartbeat message shows that a system is present and responding.
The type of the MAV and Autopilot hardware allow the receiving system
to treat further messages from this system appropriate (e.g. by laying
out the user interface based on the autopilot).

                type                      : Type of the MAV (quadrotor, helicopter, etc., up to 15 types, defined in MAV_TYPE ENUM) (uint8_t)
                autopilot                 : Autopilot type / class. defined in MAV_AUTOPILOT ENUM (uint8_t)
                base_mode                 : System mode bitfield, see MAV_MODE_FLAG ENUM in mavlink/include/mavlink_types.h (uint8_t)
                custom_mode               : A bitfield for use for autopilot-specific flags. (uint32_t)
                system_status             : System status flag, see MAV_STATE ENUM (uint8_t)
                mavlink_version           : MAVLink version, not writable by user, gets added by protocol because of magic data type: uint8_t_mavlink_version (uint8_t)

*/
mavlink.messages.heartbeat = function(type, autopilot, base_mode, custom_mode, system_status, mavlink_version) {

    this.format = '<IBBBBB';
    this.id = mavlink.MAVLINK_MSG_ID_HEARTBEAT;
    this.order_map = [1, 2, 3, 0, 4, 5];
    this.crc_extra = 50;
    this.name = 'HEARTBEAT';

    this.fieldnames = ['type', 'autopilot', 'base_mode', 'custom_mode', 'system_status', 'mavlink_version'];


    this.set(arguments);

}
        
mavlink.messages.heartbeat.prototype = new mavlink.message;

mavlink.messages.heartbeat.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.custom_mode, this.type, this.autopilot, this.base_mode, this.system_status, this.mavlink_version]));
}

/* 
The general system state. If the system is following the MAVLink
standard, the system state is mainly defined by three orthogonal
states/modes: The system mode, which is either LOCKED (motors shut
down and locked), MANUAL (system under RC control), GUIDED (system
with autonomous position control, position setpoint controlled
manually) or AUTO (system guided by path/waypoint planner). The
NAV_MODE defined the current flight state: LIFTOFF (often an open-loop
maneuver), LANDING, WAYPOINTS or VECTOR. This represents the internal
navigation state machine. The system status shows wether the system is
currently active or not and if an emergency occured. During the
CRITICAL and EMERGENCY states the MAV is still considered to be
active, but should start emergency procedures autonomously. After a
failure occured it should first move from active to critical to allow
manual intervention and then move to emergency after a certain
timeout.

                onboard_control_sensors_present        : Bitmask showing which onboard controllers and sensors are present. Value of 0: not present. Value of 1: present. Indices defined by ENUM MAV_SYS_STATUS_SENSOR (uint32_t)
                onboard_control_sensors_enabled        : Bitmask showing which onboard controllers and sensors are enabled:  Value of 0: not enabled. Value of 1: enabled. Indices defined by ENUM MAV_SYS_STATUS_SENSOR (uint32_t)
                onboard_control_sensors_health        : Bitmask showing which onboard controllers and sensors are operational or have an error:  Value of 0: not enabled. Value of 1: enabled. Indices defined by ENUM MAV_SYS_STATUS_SENSOR (uint32_t)
                load                      : Maximum usage in percent of the mainloop time, (0%: 0, 100%: 1000) should be always below 1000 (uint16_t)
                voltage_battery           : Battery voltage, in millivolts (1 = 1 millivolt) (uint16_t)
                current_battery           : Battery current, in 10*milliamperes (1 = 10 milliampere), -1: autopilot does not measure the current (int16_t)
                battery_remaining         : Remaining battery energy: (0%: 0, 100%: 100), -1: autopilot estimate the remaining battery (int8_t)
                drop_rate_comm            : Communication drops in percent, (0%: 0, 100%: 10'000), (UART, I2C, SPI, CAN), dropped packets on all links (packets that were corrupted on reception on the MAV) (uint16_t)
                errors_comm               : Communication errors (UART, I2C, SPI, CAN), dropped packets on all links (packets that were corrupted on reception on the MAV) (uint16_t)
                errors_count1             : Autopilot-specific errors (uint16_t)
                errors_count2             : Autopilot-specific errors (uint16_t)
                errors_count3             : Autopilot-specific errors (uint16_t)
                errors_count4             : Autopilot-specific errors (uint16_t)

*/
mavlink.messages.sys_status = function(onboard_control_sensors_present, onboard_control_sensors_enabled, onboard_control_sensors_health, load, voltage_battery, current_battery, battery_remaining, drop_rate_comm, errors_comm, errors_count1, errors_count2, errors_count3, errors_count4) {

    this.format = '<IIIHHhHHHHHHb';
    this.id = mavlink.MAVLINK_MSG_ID_SYS_STATUS;
    this.order_map = [0, 1, 2, 3, 4, 5, 12, 6, 7, 8, 9, 10, 11];
    this.crc_extra = 124;
    this.name = 'SYS_STATUS';

    this.fieldnames = ['onboard_control_sensors_present', 'onboard_control_sensors_enabled', 'onboard_control_sensors_health', 'load', 'voltage_battery', 'current_battery', 'battery_remaining', 'drop_rate_comm', 'errors_comm', 'errors_count1', 'errors_count2', 'errors_count3', 'errors_count4'];


    this.set(arguments);

}
        
mavlink.messages.sys_status.prototype = new mavlink.message;

mavlink.messages.sys_status.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.onboard_control_sensors_present, this.onboard_control_sensors_enabled, this.onboard_control_sensors_health, this.load, this.voltage_battery, this.current_battery, this.drop_rate_comm, this.errors_comm, this.errors_count1, this.errors_count2, this.errors_count3, this.errors_count4, this.battery_remaining]));
}

/* 
The system time is the time of the master clock, typically the
computer clock of the main onboard computer.

                time_unix_usec            : Timestamp of the master clock in microseconds since UNIX epoch. (uint64_t)
                time_boot_ms              : Timestamp of the component clock since boot time in milliseconds. (uint32_t)

*/
mavlink.messages.system_time = function(time_unix_usec, time_boot_ms) {

    this.format = '<QI';
    this.id = mavlink.MAVLINK_MSG_ID_SYSTEM_TIME;
    this.order_map = [0, 1];
    this.crc_extra = 137;
    this.name = 'SYSTEM_TIME';

    this.fieldnames = ['time_unix_usec', 'time_boot_ms'];


    this.set(arguments);

}
        
mavlink.messages.system_time.prototype = new mavlink.message;

mavlink.messages.system_time.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_unix_usec, this.time_boot_ms]));
}

/* 
A ping message either requesting or responding to a ping. This allows
to measure the system latencies, including serial port, radio modem
and UDP connections.

                time_usec                 : Unix timestamp in microseconds or since system boot if smaller than MAVLink epoch (1.1.2009) (uint64_t)
                seq                       : PING sequence (uint32_t)
                target_system             : 0: request ping from all receiving systems, if greater than 0: message is a ping response and number is the system id of the requesting system (uint8_t)
                target_component          : 0: request ping from all receiving components, if greater than 0: message is a ping response and number is the system id of the requesting system (uint8_t)

*/
mavlink.messages.ping = function(time_usec, seq, target_system, target_component) {

    this.format = '<QIBB';
    this.id = mavlink.MAVLINK_MSG_ID_PING;
    this.order_map = [0, 1, 2, 3];
    this.crc_extra = 237;
    this.name = 'PING';

    this.fieldnames = ['time_usec', 'seq', 'target_system', 'target_component'];


    this.set(arguments);

}
        
mavlink.messages.ping.prototype = new mavlink.message;

mavlink.messages.ping.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_usec, this.seq, this.target_system, this.target_component]));
}

/* 
Request to control this MAV

                target_system             : System the GCS requests control for (uint8_t)
                control_request           : 0: request control of this MAV, 1: Release control of this MAV (uint8_t)
                version                   : 0: key as plaintext, 1-255: future, different hashing/encryption variants. The GCS should in general use the safest mode possible initially and then gradually move down the encryption level if it gets a NACK message indicating an encryption mismatch. (uint8_t)
                passkey                   : Password / Key, depending on version plaintext or encrypted. 25 or less characters, NULL terminated. The characters may involve A-Z, a-z, 0-9, and "!?,.-" (char)

*/
mavlink.messages.change_operator_control = function(target_system, control_request, version, passkey) {

    this.format = '<BBB25s';
    this.id = mavlink.MAVLINK_MSG_ID_CHANGE_OPERATOR_CONTROL;
    this.order_map = [0, 1, 2, 3];
    this.crc_extra = 217;
    this.name = 'CHANGE_OPERATOR_CONTROL';

    this.fieldnames = ['target_system', 'control_request', 'version', 'passkey'];


    this.set(arguments);

}
        
mavlink.messages.change_operator_control.prototype = new mavlink.message;

mavlink.messages.change_operator_control.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.target_system, this.control_request, this.version, this.passkey]));
}

/* 
Accept / deny control of this MAV

                gcs_system_id             : ID of the GCS this message (uint8_t)
                control_request           : 0: request control of this MAV, 1: Release control of this MAV (uint8_t)
                ack                       : 0: ACK, 1: NACK: Wrong passkey, 2: NACK: Unsupported passkey encryption method, 3: NACK: Already under control (uint8_t)

*/
mavlink.messages.change_operator_control_ack = function(gcs_system_id, control_request, ack) {

    this.format = '<BBB';
    this.id = mavlink.MAVLINK_MSG_ID_CHANGE_OPERATOR_CONTROL_ACK;
    this.order_map = [0, 1, 2];
    this.crc_extra = 104;
    this.name = 'CHANGE_OPERATOR_CONTROL_ACK';

    this.fieldnames = ['gcs_system_id', 'control_request', 'ack'];


    this.set(arguments);

}
        
mavlink.messages.change_operator_control_ack.prototype = new mavlink.message;

mavlink.messages.change_operator_control_ack.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.gcs_system_id, this.control_request, this.ack]));
}

/* 
Emit an encrypted signature / key identifying this system. PLEASE
NOTE: This protocol has been kept simple, so transmitting the key
requires an encrypted channel for true safety.

                key                       : key (char)

*/
mavlink.messages.auth_key = function(key) {

    this.format = '<32s';
    this.id = mavlink.MAVLINK_MSG_ID_AUTH_KEY;
    this.order_map = [0];
    this.crc_extra = 119;
    this.name = 'AUTH_KEY';

    this.fieldnames = ['key'];


    this.set(arguments);

}
        
mavlink.messages.auth_key.prototype = new mavlink.message;

mavlink.messages.auth_key.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.key]));
}

/* 
Set the system mode, as defined by enum MAV_MODE. There is no target
component id as the mode is by definition for the overall aircraft,
not only for one component.

                target_system             : The system setting the mode (uint8_t)
                base_mode                 : The new base mode (uint8_t)
                custom_mode               : The new autopilot-specific mode. This field can be ignored by an autopilot. (uint32_t)

*/
mavlink.messages.set_mode = function(target_system, base_mode, custom_mode) {

    this.format = '<IBB';
    this.id = mavlink.MAVLINK_MSG_ID_SET_MODE;
    this.order_map = [1, 2, 0];
    this.crc_extra = 89;
    this.name = 'SET_MODE';

    this.fieldnames = ['target_system', 'base_mode', 'custom_mode'];


    this.set(arguments);

}
        
mavlink.messages.set_mode.prototype = new mavlink.message;

mavlink.messages.set_mode.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.custom_mode, this.target_system, this.base_mode]));
}

/* 
Request to read the onboard parameter with the param_id string id.
Onboard parameters are stored as key[const char*] -> value[float].
This allows to send a parameter to any other component (such as the
GCS) without the need of previous knowledge of possible parameter
names. Thus the same GCS can store different parameters for different
autopilots. See also http://qgroundcontrol.org/parameter_interface for
a full documentation of QGroundControl and IMU code.

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                param_id                  : Onboard parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string (char)
                param_index               : Parameter index. Send -1 to use the param ID field as identifier (else the param id will be ignored) (int16_t)

*/
mavlink.messages.param_request_read = function(target_system, target_component, param_id, param_index) {

    this.format = '<hBB16s';
    this.id = mavlink.MAVLINK_MSG_ID_PARAM_REQUEST_READ;
    this.order_map = [1, 2, 3, 0];
    this.crc_extra = 214;
    this.name = 'PARAM_REQUEST_READ';

    this.fieldnames = ['target_system', 'target_component', 'param_id', 'param_index'];


    this.set(arguments);

}
        
mavlink.messages.param_request_read.prototype = new mavlink.message;

mavlink.messages.param_request_read.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.param_index, this.target_system, this.target_component, this.param_id]));
}

/* 
Request all parameters of this component. After his request, all
parameters are emitted.

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)

*/
mavlink.messages.param_request_list = function(target_system, target_component) {

    this.format = '<BB';
    this.id = mavlink.MAVLINK_MSG_ID_PARAM_REQUEST_LIST;
    this.order_map = [0, 1];
    this.crc_extra = 159;
    this.name = 'PARAM_REQUEST_LIST';

    this.fieldnames = ['target_system', 'target_component'];


    this.set(arguments);

}
        
mavlink.messages.param_request_list.prototype = new mavlink.message;

mavlink.messages.param_request_list.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.target_system, this.target_component]));
}

/* 
Emit the value of a onboard parameter. The inclusion of param_count
and param_index in the message allows the recipient to keep track of
received parameters and allows him to re-request missing parameters
after a loss or timeout.

                param_id                  : Onboard parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string (char)
                param_value               : Onboard parameter value (float)
                param_type                : Onboard parameter type: see the MAV_PARAM_TYPE enum for supported data types. (uint8_t)
                param_count               : Total number of onboard parameters (uint16_t)
                param_index               : Index of this onboard parameter (uint16_t)

*/
mavlink.messages.param_value = function(param_id, param_value, param_type, param_count, param_index) {

    this.format = '<fHH16sB';
    this.id = mavlink.MAVLINK_MSG_ID_PARAM_VALUE;
    this.order_map = [3, 0, 4, 1, 2];
    this.crc_extra = 220;
    this.name = 'PARAM_VALUE';

    this.fieldnames = ['param_id', 'param_value', 'param_type', 'param_count', 'param_index'];


    this.set(arguments);

}
        
mavlink.messages.param_value.prototype = new mavlink.message;

mavlink.messages.param_value.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.param_value, this.param_count, this.param_index, this.param_id, this.param_type]));
}

/* 
Set a parameter value TEMPORARILY to RAM. It will be reset to default
on system reboot. Send the ACTION MAV_ACTION_STORAGE_WRITE to
PERMANENTLY write the RAM contents to EEPROM. IMPORTANT: The receiving
component should acknowledge the new parameter value by sending a
param_value message to all communication partners. This will also
ensure that multiple GCS all have an up-to-date list of all
parameters. If the sending GCS did not receive a PARAM_VALUE message
within its timeout time, it should re-send the PARAM_SET message.

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                param_id                  : Onboard parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string (char)
                param_value               : Onboard parameter value (float)
                param_type                : Onboard parameter type: see the MAV_PARAM_TYPE enum for supported data types. (uint8_t)

*/
mavlink.messages.param_set = function(target_system, target_component, param_id, param_value, param_type) {

    this.format = '<fBB16sB';
    this.id = mavlink.MAVLINK_MSG_ID_PARAM_SET;
    this.order_map = [1, 2, 3, 0, 4];
    this.crc_extra = 168;
    this.name = 'PARAM_SET';

    this.fieldnames = ['target_system', 'target_component', 'param_id', 'param_value', 'param_type'];


    this.set(arguments);

}
        
mavlink.messages.param_set.prototype = new mavlink.message;

mavlink.messages.param_set.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.param_value, this.target_system, this.target_component, this.param_id, this.param_type]));
}

/* 
The global position, as returned by the Global Positioning System
(GPS). This is                 NOT the global position estimate of the
system, but rather a RAW sensor value. See message GLOBAL_POSITION for
the global position estimate. Coordinate frame is right-handed, Z-axis
up (GPS frame).

                time_usec                 : Timestamp (microseconds since UNIX epoch or microseconds since system boot) (uint64_t)
                fix_type                  : 0-1: no fix, 2: 2D fix, 3: 3D fix, 4: DGPS, 5: RTK. Some applications will not use the value of this field unless it is at least two, so always correctly fill in the fix. (uint8_t)
                lat                       : Latitude (WGS84), in degrees * 1E7 (int32_t)
                lon                       : Longitude (WGS84), in degrees * 1E7 (int32_t)
                alt                       : Altitude (AMSL, NOT WGS84), in meters * 1000 (positive for up). Note that virtually all GPS modules provide the AMSL altitude in addition to the WGS84 altitude. (int32_t)
                eph                       : GPS HDOP horizontal dilution of position in cm (m*100). If unknown, set to: UINT16_MAX (uint16_t)
                epv                       : GPS VDOP vertical dilution of position in cm (m*100). If unknown, set to: UINT16_MAX (uint16_t)
                vel                       : GPS ground speed (m/s * 100). If unknown, set to: UINT16_MAX (uint16_t)
                cog                       : Course over ground (NOT heading, but direction of movement) in degrees * 100, 0.0..359.99 degrees. If unknown, set to: UINT16_MAX (uint16_t)
                satellites_visible        : Number of satellites visible. If unknown, set to 255 (uint8_t)

*/
mavlink.messages.gps_raw_int = function(time_usec, fix_type, lat, lon, alt, eph, epv, vel, cog, satellites_visible) {

    this.format = '<QiiiHHHHBB';
    this.id = mavlink.MAVLINK_MSG_ID_GPS_RAW_INT;
    this.order_map = [0, 8, 1, 2, 3, 4, 5, 6, 7, 9];
    this.crc_extra = 24;
    this.name = 'GPS_RAW_INT';

    this.fieldnames = ['time_usec', 'fix_type', 'lat', 'lon', 'alt', 'eph', 'epv', 'vel', 'cog', 'satellites_visible'];


    this.set(arguments);

}
        
mavlink.messages.gps_raw_int.prototype = new mavlink.message;

mavlink.messages.gps_raw_int.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_usec, this.lat, this.lon, this.alt, this.eph, this.epv, this.vel, this.cog, this.fix_type, this.satellites_visible]));
}

/* 
The positioning status, as reported by GPS. This message is intended
to display status information about each satellite visible to the
receiver. See message GLOBAL_POSITION for the global position
estimate. This message can contain information for up to 20
satellites.

                satellites_visible        : Number of satellites visible (uint8_t)
                satellite_prn             : Global satellite ID (uint8_t)
                satellite_used            : 0: Satellite not used, 1: used for localization (uint8_t)
                satellite_elevation        : Elevation (0: right on top of receiver, 90: on the horizon) of satellite (uint8_t)
                satellite_azimuth         : Direction of satellite, 0: 0 deg, 255: 360 deg. (uint8_t)
                satellite_snr             : Signal to noise ratio of satellite (uint8_t)

*/
mavlink.messages.gps_status = function(satellites_visible, satellite_prn, satellite_used, satellite_elevation, satellite_azimuth, satellite_snr) {

    this.format = '<B20s20s20s20s20s';
    this.id = mavlink.MAVLINK_MSG_ID_GPS_STATUS;
    this.order_map = [0, 1, 2, 3, 4, 5];
    this.crc_extra = 23;
    this.name = 'GPS_STATUS';

    this.fieldnames = ['satellites_visible', 'satellite_prn', 'satellite_used', 'satellite_elevation', 'satellite_azimuth', 'satellite_snr'];


    this.set(arguments);

}
        
mavlink.messages.gps_status.prototype = new mavlink.message;

mavlink.messages.gps_status.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.satellites_visible, this.satellite_prn, this.satellite_used, this.satellite_elevation, this.satellite_azimuth, this.satellite_snr]));
}

/* 
The RAW IMU readings for the usual 9DOF sensor setup. This message
should contain the scaled values to the described units

                time_boot_ms              : Timestamp (milliseconds since system boot) (uint32_t)
                xacc                      : X acceleration (mg) (int16_t)
                yacc                      : Y acceleration (mg) (int16_t)
                zacc                      : Z acceleration (mg) (int16_t)
                xgyro                     : Angular speed around X axis (millirad /sec) (int16_t)
                ygyro                     : Angular speed around Y axis (millirad /sec) (int16_t)
                zgyro                     : Angular speed around Z axis (millirad /sec) (int16_t)
                xmag                      : X Magnetic field (milli tesla) (int16_t)
                ymag                      : Y Magnetic field (milli tesla) (int16_t)
                zmag                      : Z Magnetic field (milli tesla) (int16_t)

*/
mavlink.messages.scaled_imu = function(time_boot_ms, xacc, yacc, zacc, xgyro, ygyro, zgyro, xmag, ymag, zmag) {

    this.format = '<Ihhhhhhhhh';
    this.id = mavlink.MAVLINK_MSG_ID_SCALED_IMU;
    this.order_map = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
    this.crc_extra = 170;
    this.name = 'SCALED_IMU';

    this.fieldnames = ['time_boot_ms', 'xacc', 'yacc', 'zacc', 'xgyro', 'ygyro', 'zgyro', 'xmag', 'ymag', 'zmag'];


    this.set(arguments);

}
        
mavlink.messages.scaled_imu.prototype = new mavlink.message;

mavlink.messages.scaled_imu.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.xacc, this.yacc, this.zacc, this.xgyro, this.ygyro, this.zgyro, this.xmag, this.ymag, this.zmag]));
}

/* 
The RAW IMU readings for the usual 9DOF sensor setup. This message
should always contain the true raw values without any scaling to allow
data capture and system debugging.

                time_usec                 : Timestamp (microseconds since UNIX epoch or microseconds since system boot) (uint64_t)
                xacc                      : X acceleration (raw) (int16_t)
                yacc                      : Y acceleration (raw) (int16_t)
                zacc                      : Z acceleration (raw) (int16_t)
                xgyro                     : Angular speed around X axis (raw) (int16_t)
                ygyro                     : Angular speed around Y axis (raw) (int16_t)
                zgyro                     : Angular speed around Z axis (raw) (int16_t)
                xmag                      : X Magnetic field (raw) (int16_t)
                ymag                      : Y Magnetic field (raw) (int16_t)
                zmag                      : Z Magnetic field (raw) (int16_t)

*/
mavlink.messages.raw_imu = function(time_usec, xacc, yacc, zacc, xgyro, ygyro, zgyro, xmag, ymag, zmag) {

    this.format = '<Qhhhhhhhhh';
    this.id = mavlink.MAVLINK_MSG_ID_RAW_IMU;
    this.order_map = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
    this.crc_extra = 144;
    this.name = 'RAW_IMU';

    this.fieldnames = ['time_usec', 'xacc', 'yacc', 'zacc', 'xgyro', 'ygyro', 'zgyro', 'xmag', 'ymag', 'zmag'];


    this.set(arguments);

}
        
mavlink.messages.raw_imu.prototype = new mavlink.message;

mavlink.messages.raw_imu.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_usec, this.xacc, this.yacc, this.zacc, this.xgyro, this.ygyro, this.zgyro, this.xmag, this.ymag, this.zmag]));
}

/* 
The RAW pressure readings for the typical setup of one absolute
pressure and one differential pressure sensor. The sensor values
should be the raw, UNSCALED ADC values.

                time_usec                 : Timestamp (microseconds since UNIX epoch or microseconds since system boot) (uint64_t)
                press_abs                 : Absolute pressure (raw) (int16_t)
                press_diff1               : Differential pressure 1 (raw) (int16_t)
                press_diff2               : Differential pressure 2 (raw) (int16_t)
                temperature               : Raw Temperature measurement (raw) (int16_t)

*/
mavlink.messages.raw_pressure = function(time_usec, press_abs, press_diff1, press_diff2, temperature) {

    this.format = '<Qhhhh';
    this.id = mavlink.MAVLINK_MSG_ID_RAW_PRESSURE;
    this.order_map = [0, 1, 2, 3, 4];
    this.crc_extra = 67;
    this.name = 'RAW_PRESSURE';

    this.fieldnames = ['time_usec', 'press_abs', 'press_diff1', 'press_diff2', 'temperature'];


    this.set(arguments);

}
        
mavlink.messages.raw_pressure.prototype = new mavlink.message;

mavlink.messages.raw_pressure.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_usec, this.press_abs, this.press_diff1, this.press_diff2, this.temperature]));
}

/* 
The pressure readings for the typical setup of one absolute and
differential pressure sensor. The units are as specified in each
field.

                time_boot_ms              : Timestamp (milliseconds since system boot) (uint32_t)
                press_abs                 : Absolute pressure (hectopascal) (float)
                press_diff                : Differential pressure 1 (hectopascal) (float)
                temperature               : Temperature measurement (0.01 degrees celsius) (int16_t)

*/
mavlink.messages.scaled_pressure = function(time_boot_ms, press_abs, press_diff, temperature) {

    this.format = '<Iffh';
    this.id = mavlink.MAVLINK_MSG_ID_SCALED_PRESSURE;
    this.order_map = [0, 1, 2, 3];
    this.crc_extra = 115;
    this.name = 'SCALED_PRESSURE';

    this.fieldnames = ['time_boot_ms', 'press_abs', 'press_diff', 'temperature'];


    this.set(arguments);

}
        
mavlink.messages.scaled_pressure.prototype = new mavlink.message;

mavlink.messages.scaled_pressure.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.press_abs, this.press_diff, this.temperature]));
}

/* 
The attitude in the aeronautical frame (right-handed, Z-down, X-front,
Y-right).

                time_boot_ms              : Timestamp (milliseconds since system boot) (uint32_t)
                roll                      : Roll angle (rad, -pi..+pi) (float)
                pitch                     : Pitch angle (rad, -pi..+pi) (float)
                yaw                       : Yaw angle (rad, -pi..+pi) (float)
                rollspeed                 : Roll angular speed (rad/s) (float)
                pitchspeed                : Pitch angular speed (rad/s) (float)
                yawspeed                  : Yaw angular speed (rad/s) (float)

*/
mavlink.messages.attitude = function(time_boot_ms, roll, pitch, yaw, rollspeed, pitchspeed, yawspeed) {

    this.format = '<Iffffff';
    this.id = mavlink.MAVLINK_MSG_ID_ATTITUDE;
    this.order_map = [0, 1, 2, 3, 4, 5, 6];
    this.crc_extra = 39;
    this.name = 'ATTITUDE';

    this.fieldnames = ['time_boot_ms', 'roll', 'pitch', 'yaw', 'rollspeed', 'pitchspeed', 'yawspeed'];


    this.set(arguments);

}
        
mavlink.messages.attitude.prototype = new mavlink.message;

mavlink.messages.attitude.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.roll, this.pitch, this.yaw, this.rollspeed, this.pitchspeed, this.yawspeed]));
}

/* 
The attitude in the aeronautical frame (right-handed, Z-down, X-front,
Y-right), expressed as quaternion. Quaternion order is w, x, y, z and
a zero rotation would be expressed as (1 0 0 0).

                time_boot_ms              : Timestamp (milliseconds since system boot) (uint32_t)
                q1                        : Quaternion component 1, w (1 in null-rotation) (float)
                q2                        : Quaternion component 2, x (0 in null-rotation) (float)
                q3                        : Quaternion component 3, y (0 in null-rotation) (float)
                q4                        : Quaternion component 4, z (0 in null-rotation) (float)
                rollspeed                 : Roll angular speed (rad/s) (float)
                pitchspeed                : Pitch angular speed (rad/s) (float)
                yawspeed                  : Yaw angular speed (rad/s) (float)

*/
mavlink.messages.attitude_quaternion = function(time_boot_ms, q1, q2, q3, q4, rollspeed, pitchspeed, yawspeed) {

    this.format = '<Ifffffff';
    this.id = mavlink.MAVLINK_MSG_ID_ATTITUDE_QUATERNION;
    this.order_map = [0, 1, 2, 3, 4, 5, 6, 7];
    this.crc_extra = 246;
    this.name = 'ATTITUDE_QUATERNION';

    this.fieldnames = ['time_boot_ms', 'q1', 'q2', 'q3', 'q4', 'rollspeed', 'pitchspeed', 'yawspeed'];


    this.set(arguments);

}
        
mavlink.messages.attitude_quaternion.prototype = new mavlink.message;

mavlink.messages.attitude_quaternion.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.q1, this.q2, this.q3, this.q4, this.rollspeed, this.pitchspeed, this.yawspeed]));
}

/* 
The filtered local position (e.g. fused computer vision and
accelerometers). Coordinate frame is right-handed, Z-axis down
(aeronautical frame, NED / north-east-down convention)

                time_boot_ms              : Timestamp (milliseconds since system boot) (uint32_t)
                x                         : X Position (float)
                y                         : Y Position (float)
                z                         : Z Position (float)
                vx                        : X Speed (float)
                vy                        : Y Speed (float)
                vz                        : Z Speed (float)

*/
mavlink.messages.local_position_ned = function(time_boot_ms, x, y, z, vx, vy, vz) {

    this.format = '<Iffffff';
    this.id = mavlink.MAVLINK_MSG_ID_LOCAL_POSITION_NED;
    this.order_map = [0, 1, 2, 3, 4, 5, 6];
    this.crc_extra = 185;
    this.name = 'LOCAL_POSITION_NED';

    this.fieldnames = ['time_boot_ms', 'x', 'y', 'z', 'vx', 'vy', 'vz'];


    this.set(arguments);

}
        
mavlink.messages.local_position_ned.prototype = new mavlink.message;

mavlink.messages.local_position_ned.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.x, this.y, this.z, this.vx, this.vy, this.vz]));
}

/* 
The filtered global position (e.g. fused GPS and accelerometers). The
position is in GPS-frame (right-handed, Z-up). It                is
designed as scaled integer message since the resolution of float is
not sufficient.

                time_boot_ms              : Timestamp (milliseconds since system boot) (uint32_t)
                lat                       : Latitude, expressed as * 1E7 (int32_t)
                lon                       : Longitude, expressed as * 1E7 (int32_t)
                alt                       : Altitude in meters, expressed as * 1000 (millimeters), AMSL (not WGS84 - note that virtually all GPS modules provide the AMSL as well) (int32_t)
                relative_alt              : Altitude above ground in meters, expressed as * 1000 (millimeters) (int32_t)
                vx                        : Ground X Speed (Latitude), expressed as m/s * 100 (int16_t)
                vy                        : Ground Y Speed (Longitude), expressed as m/s * 100 (int16_t)
                vz                        : Ground Z Speed (Altitude), expressed as m/s * 100 (int16_t)
                hdg                       : Compass heading in degrees * 100, 0.0..359.99 degrees. If unknown, set to: UINT16_MAX (uint16_t)

*/
mavlink.messages.global_position_int = function(time_boot_ms, lat, lon, alt, relative_alt, vx, vy, vz, hdg) {

    this.format = '<IiiiihhhH';
    this.id = mavlink.MAVLINK_MSG_ID_GLOBAL_POSITION_INT;
    this.order_map = [0, 1, 2, 3, 4, 5, 6, 7, 8];
    this.crc_extra = 104;
    this.name = 'GLOBAL_POSITION_INT';

    this.fieldnames = ['time_boot_ms', 'lat', 'lon', 'alt', 'relative_alt', 'vx', 'vy', 'vz', 'hdg'];


    this.set(arguments);

}
        
mavlink.messages.global_position_int.prototype = new mavlink.message;

mavlink.messages.global_position_int.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.lat, this.lon, this.alt, this.relative_alt, this.vx, this.vy, this.vz, this.hdg]));
}

/* 
The scaled values of the RC channels received. (-100%) -10000, (0%) 0,
(100%) 10000. Channels that are inactive should be set to UINT16_MAX.

                time_boot_ms              : Timestamp (milliseconds since system boot) (uint32_t)
                port                      : Servo output port (set of 8 outputs = 1 port). Most MAVs will just use one, but this allows for more than 8 servos. (uint8_t)
                chan1_scaled              : RC channel 1 value scaled, (-100%) -10000, (0%) 0, (100%) 10000, (invalid) INT16_MAX. (int16_t)
                chan2_scaled              : RC channel 2 value scaled, (-100%) -10000, (0%) 0, (100%) 10000, (invalid) INT16_MAX. (int16_t)
                chan3_scaled              : RC channel 3 value scaled, (-100%) -10000, (0%) 0, (100%) 10000, (invalid) INT16_MAX. (int16_t)
                chan4_scaled              : RC channel 4 value scaled, (-100%) -10000, (0%) 0, (100%) 10000, (invalid) INT16_MAX. (int16_t)
                chan5_scaled              : RC channel 5 value scaled, (-100%) -10000, (0%) 0, (100%) 10000, (invalid) INT16_MAX. (int16_t)
                chan6_scaled              : RC channel 6 value scaled, (-100%) -10000, (0%) 0, (100%) 10000, (invalid) INT16_MAX. (int16_t)
                chan7_scaled              : RC channel 7 value scaled, (-100%) -10000, (0%) 0, (100%) 10000, (invalid) INT16_MAX. (int16_t)
                chan8_scaled              : RC channel 8 value scaled, (-100%) -10000, (0%) 0, (100%) 10000, (invalid) INT16_MAX. (int16_t)
                rssi                      : Receive signal strength indicator, 0: 0%, 100: 100%, 255: invalid/unknown. (uint8_t)

*/
mavlink.messages.rc_channels_scaled = function(time_boot_ms, port, chan1_scaled, chan2_scaled, chan3_scaled, chan4_scaled, chan5_scaled, chan6_scaled, chan7_scaled, chan8_scaled, rssi) {

    this.format = '<IhhhhhhhhBB';
    this.id = mavlink.MAVLINK_MSG_ID_RC_CHANNELS_SCALED;
    this.order_map = [0, 9, 1, 2, 3, 4, 5, 6, 7, 8, 10];
    this.crc_extra = 237;
    this.name = 'RC_CHANNELS_SCALED';

    this.fieldnames = ['time_boot_ms', 'port', 'chan1_scaled', 'chan2_scaled', 'chan3_scaled', 'chan4_scaled', 'chan5_scaled', 'chan6_scaled', 'chan7_scaled', 'chan8_scaled', 'rssi'];


    this.set(arguments);

}
        
mavlink.messages.rc_channels_scaled.prototype = new mavlink.message;

mavlink.messages.rc_channels_scaled.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.chan1_scaled, this.chan2_scaled, this.chan3_scaled, this.chan4_scaled, this.chan5_scaled, this.chan6_scaled, this.chan7_scaled, this.chan8_scaled, this.port, this.rssi]));
}

/* 
The RAW values of the RC channels received. The standard PPM
modulation is as follows: 1000 microseconds: 0%, 2000 microseconds:
100%. Individual receivers/transmitters might violate this
specification.

                time_boot_ms              : Timestamp (milliseconds since system boot) (uint32_t)
                port                      : Servo output port (set of 8 outputs = 1 port). Most MAVs will just use one, but this allows for more than 8 servos. (uint8_t)
                chan1_raw                 : RC channel 1 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                chan2_raw                 : RC channel 2 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                chan3_raw                 : RC channel 3 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                chan4_raw                 : RC channel 4 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                chan5_raw                 : RC channel 5 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                chan6_raw                 : RC channel 6 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                chan7_raw                 : RC channel 7 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                chan8_raw                 : RC channel 8 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                rssi                      : Receive signal strength indicator, 0: 0%, 100: 100%, 255: invalid/unknown. (uint8_t)

*/
mavlink.messages.rc_channels_raw = function(time_boot_ms, port, chan1_raw, chan2_raw, chan3_raw, chan4_raw, chan5_raw, chan6_raw, chan7_raw, chan8_raw, rssi) {

    this.format = '<IHHHHHHHHBB';
    this.id = mavlink.MAVLINK_MSG_ID_RC_CHANNELS_RAW;
    this.order_map = [0, 9, 1, 2, 3, 4, 5, 6, 7, 8, 10];
    this.crc_extra = 244;
    this.name = 'RC_CHANNELS_RAW';

    this.fieldnames = ['time_boot_ms', 'port', 'chan1_raw', 'chan2_raw', 'chan3_raw', 'chan4_raw', 'chan5_raw', 'chan6_raw', 'chan7_raw', 'chan8_raw', 'rssi'];


    this.set(arguments);

}
        
mavlink.messages.rc_channels_raw.prototype = new mavlink.message;

mavlink.messages.rc_channels_raw.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.chan1_raw, this.chan2_raw, this.chan3_raw, this.chan4_raw, this.chan5_raw, this.chan6_raw, this.chan7_raw, this.chan8_raw, this.port, this.rssi]));
}

/* 
The RAW values of the servo outputs (for RC input from the remote, use
the RC_CHANNELS messages). The standard PPM modulation is as follows:
1000 microseconds: 0%, 2000 microseconds: 100%.

                time_usec                 : Timestamp (microseconds since system boot) (uint32_t)
                port                      : Servo output port (set of 8 outputs = 1 port). Most MAVs will just use one, but this allows to encode more than 8 servos. (uint8_t)
                servo1_raw                : Servo output 1 value, in microseconds (uint16_t)
                servo2_raw                : Servo output 2 value, in microseconds (uint16_t)
                servo3_raw                : Servo output 3 value, in microseconds (uint16_t)
                servo4_raw                : Servo output 4 value, in microseconds (uint16_t)
                servo5_raw                : Servo output 5 value, in microseconds (uint16_t)
                servo6_raw                : Servo output 6 value, in microseconds (uint16_t)
                servo7_raw                : Servo output 7 value, in microseconds (uint16_t)
                servo8_raw                : Servo output 8 value, in microseconds (uint16_t)

*/
mavlink.messages.servo_output_raw = function(time_usec, port, servo1_raw, servo2_raw, servo3_raw, servo4_raw, servo5_raw, servo6_raw, servo7_raw, servo8_raw) {

    this.format = '<IHHHHHHHHB';
    this.id = mavlink.MAVLINK_MSG_ID_SERVO_OUTPUT_RAW;
    this.order_map = [0, 9, 1, 2, 3, 4, 5, 6, 7, 8];
    this.crc_extra = 222;
    this.name = 'SERVO_OUTPUT_RAW';

    this.fieldnames = ['time_usec', 'port', 'servo1_raw', 'servo2_raw', 'servo3_raw', 'servo4_raw', 'servo5_raw', 'servo6_raw', 'servo7_raw', 'servo8_raw'];


    this.set(arguments);

}
        
mavlink.messages.servo_output_raw.prototype = new mavlink.message;

mavlink.messages.servo_output_raw.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_usec, this.servo1_raw, this.servo2_raw, this.servo3_raw, this.servo4_raw, this.servo5_raw, this.servo6_raw, this.servo7_raw, this.servo8_raw, this.port]));
}

/* 
Request a partial list of mission items from the system/component.
http://qgroundcontrol.org/mavlink/waypoint_protocol. If start and end
index are the same, just send one waypoint.

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                start_index               : Start index, 0 by default (int16_t)
                end_index                 : End index, -1 by default (-1: send list to end). Else a valid index of the list (int16_t)

*/
mavlink.messages.mission_request_partial_list = function(target_system, target_component, start_index, end_index) {

    this.format = '<hhBB';
    this.id = mavlink.MAVLINK_MSG_ID_MISSION_REQUEST_PARTIAL_LIST;
    this.order_map = [2, 3, 0, 1];
    this.crc_extra = 212;
    this.name = 'MISSION_REQUEST_PARTIAL_LIST';

    this.fieldnames = ['target_system', 'target_component', 'start_index', 'end_index'];


    this.set(arguments);

}
        
mavlink.messages.mission_request_partial_list.prototype = new mavlink.message;

mavlink.messages.mission_request_partial_list.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.start_index, this.end_index, this.target_system, this.target_component]));
}

/* 
This message is sent to the MAV to write a partial list. If start
index == end index, only one item will be transmitted / updated. If
the start index is NOT 0 and above the current list size, this request
should be REJECTED!

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                start_index               : Start index, 0 by default and smaller / equal to the largest index of the current onboard list. (int16_t)
                end_index                 : End index, equal or greater than start index. (int16_t)

*/
mavlink.messages.mission_write_partial_list = function(target_system, target_component, start_index, end_index) {

    this.format = '<hhBB';
    this.id = mavlink.MAVLINK_MSG_ID_MISSION_WRITE_PARTIAL_LIST;
    this.order_map = [2, 3, 0, 1];
    this.crc_extra = 9;
    this.name = 'MISSION_WRITE_PARTIAL_LIST';

    this.fieldnames = ['target_system', 'target_component', 'start_index', 'end_index'];


    this.set(arguments);

}
        
mavlink.messages.mission_write_partial_list.prototype = new mavlink.message;

mavlink.messages.mission_write_partial_list.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.start_index, this.end_index, this.target_system, this.target_component]));
}

/* 
Message encoding a mission item. This message is emitted to announce
the presence of a mission item and to set a mission item on the
system. The mission item can be either in x, y, z meters (type: LOCAL)
or x:lat, y:lon, z:altitude. Local frame is Z-down, right handed
(NED), global frame is Z-up, right handed (ENU). See also
http://qgroundcontrol.org/mavlink/waypoint_protocol.

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                seq                       : Sequence (uint16_t)
                frame                     : The coordinate system of the MISSION. see MAV_FRAME in mavlink_types.h (uint8_t)
                command                   : The scheduled action for the MISSION. see MAV_CMD in common.xml MAVLink specs (uint16_t)
                current                   : false:0, true:1 (uint8_t)
                autocontinue              : autocontinue to next wp (uint8_t)
                param1                    : PARAM1, see MAV_CMD enum (float)
                param2                    : PARAM2, see MAV_CMD enum (float)
                param3                    : PARAM3, see MAV_CMD enum (float)
                param4                    : PARAM4, see MAV_CMD enum (float)
                x                         : PARAM5 / local: x position, global: latitude (float)
                y                         : PARAM6 / y position: global: longitude (float)
                z                         : PARAM7 / z position: global: altitude (relative or absolute, depending on frame. (float)

*/
mavlink.messages.mission_item = function(target_system, target_component, seq, frame, command, current, autocontinue, param1, param2, param3, param4, x, y, z) {

    this.format = '<fffffffHHBBBBB';
    this.id = mavlink.MAVLINK_MSG_ID_MISSION_ITEM;
    this.order_map = [9, 10, 7, 11, 8, 12, 13, 0, 1, 2, 3, 4, 5, 6];
    this.crc_extra = 254;
    this.name = 'MISSION_ITEM';

    this.fieldnames = ['target_system', 'target_component', 'seq', 'frame', 'command', 'current', 'autocontinue', 'param1', 'param2', 'param3', 'param4', 'x', 'y', 'z'];


    this.set(arguments);

}
        
mavlink.messages.mission_item.prototype = new mavlink.message;

mavlink.messages.mission_item.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.param1, this.param2, this.param3, this.param4, this.x, this.y, this.z, this.seq, this.command, this.target_system, this.target_component, this.frame, this.current, this.autocontinue]));
}

/* 
Request the information of the mission item with the sequence number
seq. The response of the system to this message should be a
MISSION_ITEM message.
http://qgroundcontrol.org/mavlink/waypoint_protocol

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                seq                       : Sequence (uint16_t)

*/
mavlink.messages.mission_request = function(target_system, target_component, seq) {

    this.format = '<HBB';
    this.id = mavlink.MAVLINK_MSG_ID_MISSION_REQUEST;
    this.order_map = [1, 2, 0];
    this.crc_extra = 230;
    this.name = 'MISSION_REQUEST';

    this.fieldnames = ['target_system', 'target_component', 'seq'];


    this.set(arguments);

}
        
mavlink.messages.mission_request.prototype = new mavlink.message;

mavlink.messages.mission_request.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.seq, this.target_system, this.target_component]));
}

/* 
Set the mission item with sequence number seq as current item. This
means that the MAV will continue to this mission item on the shortest
path (not following the mission items in-between).

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                seq                       : Sequence (uint16_t)

*/
mavlink.messages.mission_set_current = function(target_system, target_component, seq) {

    this.format = '<HBB';
    this.id = mavlink.MAVLINK_MSG_ID_MISSION_SET_CURRENT;
    this.order_map = [1, 2, 0];
    this.crc_extra = 28;
    this.name = 'MISSION_SET_CURRENT';

    this.fieldnames = ['target_system', 'target_component', 'seq'];


    this.set(arguments);

}
        
mavlink.messages.mission_set_current.prototype = new mavlink.message;

mavlink.messages.mission_set_current.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.seq, this.target_system, this.target_component]));
}

/* 
Message that announces the sequence number of the current active
mission item. The MAV will fly towards this mission item.

                seq                       : Sequence (uint16_t)

*/
mavlink.messages.mission_current = function(seq) {

    this.format = '<H';
    this.id = mavlink.MAVLINK_MSG_ID_MISSION_CURRENT;
    this.order_map = [0];
    this.crc_extra = 28;
    this.name = 'MISSION_CURRENT';

    this.fieldnames = ['seq'];


    this.set(arguments);

}
        
mavlink.messages.mission_current.prototype = new mavlink.message;

mavlink.messages.mission_current.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.seq]));
}

/* 
Request the overall list of mission items from the system/component.

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)

*/
mavlink.messages.mission_request_list = function(target_system, target_component) {

    this.format = '<BB';
    this.id = mavlink.MAVLINK_MSG_ID_MISSION_REQUEST_LIST;
    this.order_map = [0, 1];
    this.crc_extra = 132;
    this.name = 'MISSION_REQUEST_LIST';

    this.fieldnames = ['target_system', 'target_component'];


    this.set(arguments);

}
        
mavlink.messages.mission_request_list.prototype = new mavlink.message;

mavlink.messages.mission_request_list.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.target_system, this.target_component]));
}

/* 
This message is emitted as response to MISSION_REQUEST_LIST by the MAV
and to initiate a write transaction. The GCS can then request the
individual mission item based on the knowledge of the total number of
MISSIONs.

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                count                     : Number of mission items in the sequence (uint16_t)

*/
mavlink.messages.mission_count = function(target_system, target_component, count) {

    this.format = '<HBB';
    this.id = mavlink.MAVLINK_MSG_ID_MISSION_COUNT;
    this.order_map = [1, 2, 0];
    this.crc_extra = 221;
    this.name = 'MISSION_COUNT';

    this.fieldnames = ['target_system', 'target_component', 'count'];


    this.set(arguments);

}
        
mavlink.messages.mission_count.prototype = new mavlink.message;

mavlink.messages.mission_count.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.count, this.target_system, this.target_component]));
}

/* 
Delete all mission items at once.

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)

*/
mavlink.messages.mission_clear_all = function(target_system, target_component) {

    this.format = '<BB';
    this.id = mavlink.MAVLINK_MSG_ID_MISSION_CLEAR_ALL;
    this.order_map = [0, 1];
    this.crc_extra = 232;
    this.name = 'MISSION_CLEAR_ALL';

    this.fieldnames = ['target_system', 'target_component'];


    this.set(arguments);

}
        
mavlink.messages.mission_clear_all.prototype = new mavlink.message;

mavlink.messages.mission_clear_all.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.target_system, this.target_component]));
}

/* 
A certain mission item has been reached. The system will either hold
this position (or circle on the orbit) or (if the autocontinue on the
WP was set) continue to the next MISSION.

                seq                       : Sequence (uint16_t)

*/
mavlink.messages.mission_item_reached = function(seq) {

    this.format = '<H';
    this.id = mavlink.MAVLINK_MSG_ID_MISSION_ITEM_REACHED;
    this.order_map = [0];
    this.crc_extra = 11;
    this.name = 'MISSION_ITEM_REACHED';

    this.fieldnames = ['seq'];


    this.set(arguments);

}
        
mavlink.messages.mission_item_reached.prototype = new mavlink.message;

mavlink.messages.mission_item_reached.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.seq]));
}

/* 
Ack message during MISSION handling. The type field states if this
message is a positive ack (type=0) or if an error happened (type=non-
zero).

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                type                      : See MAV_MISSION_RESULT enum (uint8_t)

*/
mavlink.messages.mission_ack = function(target_system, target_component, type) {

    this.format = '<BBB';
    this.id = mavlink.MAVLINK_MSG_ID_MISSION_ACK;
    this.order_map = [0, 1, 2];
    this.crc_extra = 153;
    this.name = 'MISSION_ACK';

    this.fieldnames = ['target_system', 'target_component', 'type'];


    this.set(arguments);

}
        
mavlink.messages.mission_ack.prototype = new mavlink.message;

mavlink.messages.mission_ack.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.target_system, this.target_component, this.type]));
}

/* 
As local waypoints exist, the global MISSION reference allows to
transform between the local coordinate frame and the global (GPS)
coordinate frame. This can be necessary when e.g. in- and outdoor
settings are connected and the MAV should move from in- to outdoor.

                target_system             : System ID (uint8_t)
                latitude                  : Latitude (WGS84), in degrees * 1E7 (int32_t)
                longitude                 : Longitude (WGS84, in degrees * 1E7 (int32_t)
                altitude                  : Altitude (AMSL), in meters * 1000 (positive for up) (int32_t)

*/
mavlink.messages.set_gps_global_origin = function(target_system, latitude, longitude, altitude) {

    this.format = '<iiiB';
    this.id = mavlink.MAVLINK_MSG_ID_SET_GPS_GLOBAL_ORIGIN;
    this.order_map = [3, 0, 1, 2];
    this.crc_extra = 41;
    this.name = 'SET_GPS_GLOBAL_ORIGIN';

    this.fieldnames = ['target_system', 'latitude', 'longitude', 'altitude'];


    this.set(arguments);

}
        
mavlink.messages.set_gps_global_origin.prototype = new mavlink.message;

mavlink.messages.set_gps_global_origin.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.latitude, this.longitude, this.altitude, this.target_system]));
}

/* 
Once the MAV sets a new GPS-Local correspondence, this message
announces the origin (0,0,0) position

                latitude                  : Latitude (WGS84), in degrees * 1E7 (int32_t)
                longitude                 : Longitude (WGS84), in degrees * 1E7 (int32_t)
                altitude                  : Altitude (AMSL), in meters * 1000 (positive for up) (int32_t)

*/
mavlink.messages.gps_global_origin = function(latitude, longitude, altitude) {

    this.format = '<iii';
    this.id = mavlink.MAVLINK_MSG_ID_GPS_GLOBAL_ORIGIN;
    this.order_map = [0, 1, 2];
    this.crc_extra = 39;
    this.name = 'GPS_GLOBAL_ORIGIN';

    this.fieldnames = ['latitude', 'longitude', 'altitude'];


    this.set(arguments);

}
        
mavlink.messages.gps_global_origin.prototype = new mavlink.message;

mavlink.messages.gps_global_origin.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.latitude, this.longitude, this.altitude]));
}

/* 
Bind a RC channel to a parameter. The parameter should change accoding
to the RC channel value.

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                param_id                  : Onboard parameter id, terminated by NULL if the length is less than 16 human-readable chars and WITHOUT null termination (NULL) byte if the length is exactly 16 chars - applications have to provide 16+1 bytes storage if the ID is stored as string (char)
                param_index               : Parameter index. Send -1 to use the param ID field as identifier (else the param id will be ignored), send -2 to disable any existing map for this rc_channel_index. (int16_t)
                parameter_rc_channel_index        : Index of parameter RC channel. Not equal to the RC channel id. Typically correpsonds to a potentiometer-knob on the RC. (uint8_t)
                param_value0              : Initial parameter value (float)
                scale                     : Scale, maps the RC range [-1, 1] to a parameter value (float)
                param_value_min           : Minimum param value. The protocol does not define if this overwrites an onboard minimum value. (Depends on implementation) (float)
                param_value_max           : Maximum param value. The protocol does not define if this overwrites an onboard maximum value. (Depends on implementation) (float)

*/
mavlink.messages.param_map_rc = function(target_system, target_component, param_id, param_index, parameter_rc_channel_index, param_value0, scale, param_value_min, param_value_max) {

    this.format = '<ffffhBB16sB';
    this.id = mavlink.MAVLINK_MSG_ID_PARAM_MAP_RC;
    this.order_map = [5, 6, 7, 4, 8, 0, 1, 2, 3];
    this.crc_extra = 78;
    this.name = 'PARAM_MAP_RC';

    this.fieldnames = ['target_system', 'target_component', 'param_id', 'param_index', 'parameter_rc_channel_index', 'param_value0', 'scale', 'param_value_min', 'param_value_max'];


    this.set(arguments);

}
        
mavlink.messages.param_map_rc.prototype = new mavlink.message;

mavlink.messages.param_map_rc.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.param_value0, this.scale, this.param_value_min, this.param_value_max, this.param_index, this.target_system, this.target_component, this.param_id, this.parameter_rc_channel_index]));
}

/* 
Set a safety zone (volume), which is defined by two corners of a cube.
This message can be used to tell the MAV which setpoints/MISSIONs to
accept and which to reject. Safety areas are often enforced by
national or competition regulations.

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                frame                     : Coordinate frame, as defined by MAV_FRAME enum in mavlink_types.h. Can be either global, GPS, right-handed with Z axis up or local, right handed, Z axis down. (uint8_t)
                p1x                       : x position 1 / Latitude 1 (float)
                p1y                       : y position 1 / Longitude 1 (float)
                p1z                       : z position 1 / Altitude 1 (float)
                p2x                       : x position 2 / Latitude 2 (float)
                p2y                       : y position 2 / Longitude 2 (float)
                p2z                       : z position 2 / Altitude 2 (float)

*/
mavlink.messages.safety_set_allowed_area = function(target_system, target_component, frame, p1x, p1y, p1z, p2x, p2y, p2z) {

    this.format = '<ffffffBBB';
    this.id = mavlink.MAVLINK_MSG_ID_SAFETY_SET_ALLOWED_AREA;
    this.order_map = [6, 7, 8, 0, 1, 2, 3, 4, 5];
    this.crc_extra = 15;
    this.name = 'SAFETY_SET_ALLOWED_AREA';

    this.fieldnames = ['target_system', 'target_component', 'frame', 'p1x', 'p1y', 'p1z', 'p2x', 'p2y', 'p2z'];


    this.set(arguments);

}
        
mavlink.messages.safety_set_allowed_area.prototype = new mavlink.message;

mavlink.messages.safety_set_allowed_area.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.p1x, this.p1y, this.p1z, this.p2x, this.p2y, this.p2z, this.target_system, this.target_component, this.frame]));
}

/* 
Read out the safety zone the MAV currently assumes.

                frame                     : Coordinate frame, as defined by MAV_FRAME enum in mavlink_types.h. Can be either global, GPS, right-handed with Z axis up or local, right handed, Z axis down. (uint8_t)
                p1x                       : x position 1 / Latitude 1 (float)
                p1y                       : y position 1 / Longitude 1 (float)
                p1z                       : z position 1 / Altitude 1 (float)
                p2x                       : x position 2 / Latitude 2 (float)
                p2y                       : y position 2 / Longitude 2 (float)
                p2z                       : z position 2 / Altitude 2 (float)

*/
mavlink.messages.safety_allowed_area = function(frame, p1x, p1y, p1z, p2x, p2y, p2z) {

    this.format = '<ffffffB';
    this.id = mavlink.MAVLINK_MSG_ID_SAFETY_ALLOWED_AREA;
    this.order_map = [6, 0, 1, 2, 3, 4, 5];
    this.crc_extra = 3;
    this.name = 'SAFETY_ALLOWED_AREA';

    this.fieldnames = ['frame', 'p1x', 'p1y', 'p1z', 'p2x', 'p2y', 'p2z'];


    this.set(arguments);

}
        
mavlink.messages.safety_allowed_area.prototype = new mavlink.message;

mavlink.messages.safety_allowed_area.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.p1x, this.p1y, this.p1z, this.p2x, this.p2y, this.p2z, this.frame]));
}

/* 
The attitude in the aeronautical frame (right-handed, Z-down, X-front,
Y-right), expressed as quaternion. Quaternion order is w, x, y, z and
a zero rotation would be expressed as (1 0 0 0).

                time_boot_ms              : Timestamp (milliseconds since system boot) (uint32_t)
                q                         : Quaternion components, w, x, y, z (1 0 0 0 is the null-rotation) (float)
                rollspeed                 : Roll angular speed (rad/s) (float)
                pitchspeed                : Pitch angular speed (rad/s) (float)
                yawspeed                  : Yaw angular speed (rad/s) (float)
                covariance                : Attitude covariance (float)

*/
mavlink.messages.attitude_quaternion_cov = function(time_boot_ms, q, rollspeed, pitchspeed, yawspeed, covariance) {

    this.format = '<I4ffff9f';
    this.id = mavlink.MAVLINK_MSG_ID_ATTITUDE_QUATERNION_COV;
    this.order_map = [0, 1, 2, 3, 4, 5];
    this.crc_extra = 153;
    this.name = 'ATTITUDE_QUATERNION_COV';

    this.fieldnames = ['time_boot_ms', 'q', 'rollspeed', 'pitchspeed', 'yawspeed', 'covariance'];


    this.set(arguments);

}
        
mavlink.messages.attitude_quaternion_cov.prototype = new mavlink.message;

mavlink.messages.attitude_quaternion_cov.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.q, this.rollspeed, this.pitchspeed, this.yawspeed, this.covariance]));
}

/* 
Outputs of the APM navigation controller. The primary use of this
message is to check the response and signs of the controller before
actual flight and to assist with tuning controller parameters.

                nav_roll                  : Current desired roll in degrees (float)
                nav_pitch                 : Current desired pitch in degrees (float)
                nav_bearing               : Current desired heading in degrees (int16_t)
                target_bearing            : Bearing to current MISSION/target in degrees (int16_t)
                wp_dist                   : Distance to active MISSION in meters (uint16_t)
                alt_error                 : Current altitude error in meters (float)
                aspd_error                : Current airspeed error in meters/second (float)
                xtrack_error              : Current crosstrack error on x-y plane in meters (float)

*/
mavlink.messages.nav_controller_output = function(nav_roll, nav_pitch, nav_bearing, target_bearing, wp_dist, alt_error, aspd_error, xtrack_error) {

    this.format = '<fffffhhH';
    this.id = mavlink.MAVLINK_MSG_ID_NAV_CONTROLLER_OUTPUT;
    this.order_map = [0, 1, 5, 6, 7, 2, 3, 4];
    this.crc_extra = 183;
    this.name = 'NAV_CONTROLLER_OUTPUT';

    this.fieldnames = ['nav_roll', 'nav_pitch', 'nav_bearing', 'target_bearing', 'wp_dist', 'alt_error', 'aspd_error', 'xtrack_error'];


    this.set(arguments);

}
        
mavlink.messages.nav_controller_output.prototype = new mavlink.message;

mavlink.messages.nav_controller_output.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.nav_roll, this.nav_pitch, this.alt_error, this.aspd_error, this.xtrack_error, this.nav_bearing, this.target_bearing, this.wp_dist]));
}

/* 
The filtered global position (e.g. fused GPS and accelerometers). The
position is in GPS-frame (right-handed, Z-up). It  is designed as
scaled integer message since the resolution of float is not
sufficient. NOTE: This message is intended for onboard networks /
companion computers and higher-bandwidth links and optimized for
accuracy and completeness. Please use the GLOBAL_POSITION_INT message
for a minimal subset.

                time_boot_ms              : Timestamp (milliseconds since system boot) (uint32_t)
                time_utc                  : Timestamp (microseconds since UNIX epoch) in UTC. 0 for unknown. Commonly filled by the precision time source of a GPS receiver. (uint64_t)
                estimator_type            : Class id of the estimator this estimate originated from. (uint8_t)
                lat                       : Latitude, expressed as degrees * 1E7 (int32_t)
                lon                       : Longitude, expressed as degrees * 1E7 (int32_t)
                alt                       : Altitude in meters, expressed as * 1000 (millimeters), above MSL (int32_t)
                relative_alt              : Altitude above ground in meters, expressed as * 1000 (millimeters) (int32_t)
                vx                        : Ground X Speed (Latitude), expressed as m/s (float)
                vy                        : Ground Y Speed (Longitude), expressed as m/s (float)
                vz                        : Ground Z Speed (Altitude), expressed as m/s (float)
                covariance                : Covariance matrix (first six entries are the first ROW, next six entries are the second row, etc.) (float)

*/
mavlink.messages.global_position_int_cov = function(time_boot_ms, time_utc, estimator_type, lat, lon, alt, relative_alt, vx, vy, vz, covariance) {

    this.format = '<QIiiiifff36fB';
    this.id = mavlink.MAVLINK_MSG_ID_GLOBAL_POSITION_INT_COV;
    this.order_map = [1, 0, 10, 2, 3, 4, 5, 6, 7, 8, 9];
    this.crc_extra = 51;
    this.name = 'GLOBAL_POSITION_INT_COV';

    this.fieldnames = ['time_boot_ms', 'time_utc', 'estimator_type', 'lat', 'lon', 'alt', 'relative_alt', 'vx', 'vy', 'vz', 'covariance'];


    this.set(arguments);

}
        
mavlink.messages.global_position_int_cov.prototype = new mavlink.message;

mavlink.messages.global_position_int_cov.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_utc, this.time_boot_ms, this.lat, this.lon, this.alt, this.relative_alt, this.vx, this.vy, this.vz, this.covariance, this.estimator_type]));
}

/* 
The filtered local position (e.g. fused computer vision and
accelerometers). Coordinate frame is right-handed, Z-axis down
(aeronautical frame, NED / north-east-down convention)

                time_boot_ms              : Timestamp (milliseconds since system boot) (uint32_t)
                time_utc                  : Timestamp (microseconds since UNIX epoch) in UTC. 0 for unknown. Commonly filled by the precision time source of a GPS receiver. (uint64_t)
                estimator_type            : Class id of the estimator this estimate originated from. (uint8_t)
                x                         : X Position (float)
                y                         : Y Position (float)
                z                         : Z Position (float)
                vx                        : X Speed (float)
                vy                        : Y Speed (float)
                vz                        : Z Speed (float)
                covariance                : Covariance matrix (first six entries are the first ROW, next six entries are the second row, etc.) (float)

*/
mavlink.messages.local_position_ned_cov = function(time_boot_ms, time_utc, estimator_type, x, y, z, vx, vy, vz, covariance) {

    this.format = '<QIffffff36fB';
    this.id = mavlink.MAVLINK_MSG_ID_LOCAL_POSITION_NED_COV;
    this.order_map = [1, 0, 9, 2, 3, 4, 5, 6, 7, 8];
    this.crc_extra = 82;
    this.name = 'LOCAL_POSITION_NED_COV';

    this.fieldnames = ['time_boot_ms', 'time_utc', 'estimator_type', 'x', 'y', 'z', 'vx', 'vy', 'vz', 'covariance'];


    this.set(arguments);

}
        
mavlink.messages.local_position_ned_cov.prototype = new mavlink.message;

mavlink.messages.local_position_ned_cov.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_utc, this.time_boot_ms, this.x, this.y, this.z, this.vx, this.vy, this.vz, this.covariance, this.estimator_type]));
}

/* 
The PPM values of the RC channels received. The standard PPM
modulation is as follows: 1000 microseconds: 0%, 2000 microseconds:
100%. Individual receivers/transmitters might violate this
specification.

                time_boot_ms              : Timestamp (milliseconds since system boot) (uint32_t)
                chancount                 : Total number of RC channels being received. This can be larger than 18, indicating that more channels are available but not given in this message. This value should be 0 when no RC channels are available. (uint8_t)
                chan1_raw                 : RC channel 1 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                chan2_raw                 : RC channel 2 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                chan3_raw                 : RC channel 3 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                chan4_raw                 : RC channel 4 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                chan5_raw                 : RC channel 5 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                chan6_raw                 : RC channel 6 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                chan7_raw                 : RC channel 7 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                chan8_raw                 : RC channel 8 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                chan9_raw                 : RC channel 9 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                chan10_raw                : RC channel 10 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                chan11_raw                : RC channel 11 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                chan12_raw                : RC channel 12 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                chan13_raw                : RC channel 13 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                chan14_raw                : RC channel 14 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                chan15_raw                : RC channel 15 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                chan16_raw                : RC channel 16 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                chan17_raw                : RC channel 17 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                chan18_raw                : RC channel 18 value, in microseconds. A value of UINT16_MAX implies the channel is unused. (uint16_t)
                rssi                      : Receive signal strength indicator, 0: 0%, 100: 100%, 255: invalid/unknown. (uint8_t)

*/
mavlink.messages.rc_channels = function(time_boot_ms, chancount, chan1_raw, chan2_raw, chan3_raw, chan4_raw, chan5_raw, chan6_raw, chan7_raw, chan8_raw, chan9_raw, chan10_raw, chan11_raw, chan12_raw, chan13_raw, chan14_raw, chan15_raw, chan16_raw, chan17_raw, chan18_raw, rssi) {

    this.format = '<IHHHHHHHHHHHHHHHHHHBB';
    this.id = mavlink.MAVLINK_MSG_ID_RC_CHANNELS;
    this.order_map = [0, 19, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 20];
    this.crc_extra = 118;
    this.name = 'RC_CHANNELS';

    this.fieldnames = ['time_boot_ms', 'chancount', 'chan1_raw', 'chan2_raw', 'chan3_raw', 'chan4_raw', 'chan5_raw', 'chan6_raw', 'chan7_raw', 'chan8_raw', 'chan9_raw', 'chan10_raw', 'chan11_raw', 'chan12_raw', 'chan13_raw', 'chan14_raw', 'chan15_raw', 'chan16_raw', 'chan17_raw', 'chan18_raw', 'rssi'];


    this.set(arguments);

}
        
mavlink.messages.rc_channels.prototype = new mavlink.message;

mavlink.messages.rc_channels.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.chan1_raw, this.chan2_raw, this.chan3_raw, this.chan4_raw, this.chan5_raw, this.chan6_raw, this.chan7_raw, this.chan8_raw, this.chan9_raw, this.chan10_raw, this.chan11_raw, this.chan12_raw, this.chan13_raw, this.chan14_raw, this.chan15_raw, this.chan16_raw, this.chan17_raw, this.chan18_raw, this.chancount, this.rssi]));
}

/* 


                target_system             : The target requested to send the message stream. (uint8_t)
                target_component          : The target requested to send the message stream. (uint8_t)
                req_stream_id             : The ID of the requested data stream (uint8_t)
                req_message_rate          : The requested interval between two messages of this type (uint16_t)
                start_stop                : 1 to start sending, 0 to stop sending. (uint8_t)

*/
mavlink.messages.request_data_stream = function(target_system, target_component, req_stream_id, req_message_rate, start_stop) {

    this.format = '<HBBBB';
    this.id = mavlink.MAVLINK_MSG_ID_REQUEST_DATA_STREAM;
    this.order_map = [1, 2, 3, 0, 4];
    this.crc_extra = 148;
    this.name = 'REQUEST_DATA_STREAM';

    this.fieldnames = ['target_system', 'target_component', 'req_stream_id', 'req_message_rate', 'start_stop'];


    this.set(arguments);

}
        
mavlink.messages.request_data_stream.prototype = new mavlink.message;

mavlink.messages.request_data_stream.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.req_message_rate, this.target_system, this.target_component, this.req_stream_id, this.start_stop]));
}

/* 


                stream_id                 : The ID of the requested data stream (uint8_t)
                message_rate              : The requested interval between two messages of this type (uint16_t)
                on_off                    : 1 stream is enabled, 0 stream is stopped. (uint8_t)

*/
mavlink.messages.data_stream = function(stream_id, message_rate, on_off) {

    this.format = '<HBB';
    this.id = mavlink.MAVLINK_MSG_ID_DATA_STREAM;
    this.order_map = [1, 0, 2];
    this.crc_extra = 21;
    this.name = 'DATA_STREAM';

    this.fieldnames = ['stream_id', 'message_rate', 'on_off'];


    this.set(arguments);

}
        
mavlink.messages.data_stream.prototype = new mavlink.message;

mavlink.messages.data_stream.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.message_rate, this.stream_id, this.on_off]));
}

/* 
This message provides an API for manually controlling the vehicle
using standard joystick axes nomenclature, along with a joystick-like
input device. Unused axes can be disabled an buttons are also transmit
as boolean values of their

                target                    : The system to be controlled. (uint8_t)
                x                         : X-axis, normalized to the range [-1000,1000]. A value of INT16_MAX indicates that this axis is invalid. Generally corresponds to forward(1000)-backward(-1000) movement on a joystick and the pitch of a vehicle. (int16_t)
                y                         : Y-axis, normalized to the range [-1000,1000]. A value of INT16_MAX indicates that this axis is invalid. Generally corresponds to left(-1000)-right(1000) movement on a joystick and the roll of a vehicle. (int16_t)
                z                         : Z-axis, normalized to the range [-1000,1000]. A value of INT16_MAX indicates that this axis is invalid. Generally corresponds to a separate slider movement with maximum being 1000 and minimum being -1000 on a joystick and the thrust of a vehicle. (int16_t)
                r                         : R-axis, normalized to the range [-1000,1000]. A value of INT16_MAX indicates that this axis is invalid. Generally corresponds to a twisting of the joystick, with counter-clockwise being 1000 and clockwise being -1000, and the yaw of a vehicle. (int16_t)
                buttons                   : A bitfield corresponding to the joystick buttons' current state, 1 for pressed, 0 for released. The lowest bit corresponds to Button 1. (uint16_t)

*/
mavlink.messages.manual_control = function(target, x, y, z, r, buttons) {

    this.format = '<hhhhHB';
    this.id = mavlink.MAVLINK_MSG_ID_MANUAL_CONTROL;
    this.order_map = [5, 0, 1, 2, 3, 4];
    this.crc_extra = 243;
    this.name = 'MANUAL_CONTROL';

    this.fieldnames = ['target', 'x', 'y', 'z', 'r', 'buttons'];


    this.set(arguments);

}
        
mavlink.messages.manual_control.prototype = new mavlink.message;

mavlink.messages.manual_control.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.x, this.y, this.z, this.r, this.buttons, this.target]));
}

/* 
The RAW values of the RC channels sent to the MAV to override info
received from the RC radio. A value of UINT16_MAX means no change to
that channel. A value of 0 means control of that channel should be
released back to the RC radio. The standard PPM modulation is as
follows: 1000 microseconds: 0%, 2000 microseconds: 100%. Individual
receivers/transmitters might violate this specification.

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                chan1_raw                 : RC channel 1 value, in microseconds. A value of UINT16_MAX means to ignore this field. (uint16_t)
                chan2_raw                 : RC channel 2 value, in microseconds. A value of UINT16_MAX means to ignore this field. (uint16_t)
                chan3_raw                 : RC channel 3 value, in microseconds. A value of UINT16_MAX means to ignore this field. (uint16_t)
                chan4_raw                 : RC channel 4 value, in microseconds. A value of UINT16_MAX means to ignore this field. (uint16_t)
                chan5_raw                 : RC channel 5 value, in microseconds. A value of UINT16_MAX means to ignore this field. (uint16_t)
                chan6_raw                 : RC channel 6 value, in microseconds. A value of UINT16_MAX means to ignore this field. (uint16_t)
                chan7_raw                 : RC channel 7 value, in microseconds. A value of UINT16_MAX means to ignore this field. (uint16_t)
                chan8_raw                 : RC channel 8 value, in microseconds. A value of UINT16_MAX means to ignore this field. (uint16_t)

*/
mavlink.messages.rc_channels_override = function(target_system, target_component, chan1_raw, chan2_raw, chan3_raw, chan4_raw, chan5_raw, chan6_raw, chan7_raw, chan8_raw) {

    this.format = '<HHHHHHHHBB';
    this.id = mavlink.MAVLINK_MSG_ID_RC_CHANNELS_OVERRIDE;
    this.order_map = [8, 9, 0, 1, 2, 3, 4, 5, 6, 7];
    this.crc_extra = 124;
    this.name = 'RC_CHANNELS_OVERRIDE';

    this.fieldnames = ['target_system', 'target_component', 'chan1_raw', 'chan2_raw', 'chan3_raw', 'chan4_raw', 'chan5_raw', 'chan6_raw', 'chan7_raw', 'chan8_raw'];


    this.set(arguments);

}
        
mavlink.messages.rc_channels_override.prototype = new mavlink.message;

mavlink.messages.rc_channels_override.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.chan1_raw, this.chan2_raw, this.chan3_raw, this.chan4_raw, this.chan5_raw, this.chan6_raw, this.chan7_raw, this.chan8_raw, this.target_system, this.target_component]));
}

/* 
Message encoding a mission item. This message is emitted to announce
the presence of a mission item and to set a mission item on the
system. The mission item can be either in x, y, z meters (type: LOCAL)
or x:lat, y:lon, z:altitude. Local frame is Z-down, right handed
(NED), global frame is Z-up, right handed (ENU). See
alsohttp://qgroundcontrol.org/mavlink/waypoint_protocol.

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                seq                       : Waypoint ID (sequence number). Starts at zero. Increases monotonically for each waypoint, no gaps in the sequence (0,1,2,3,4). (uint16_t)
                frame                     : The coordinate system of the MISSION. see MAV_FRAME in mavlink_types.h (uint8_t)
                command                   : The scheduled action for the MISSION. see MAV_CMD in common.xml MAVLink specs (uint16_t)
                current                   : false:0, true:1 (uint8_t)
                autocontinue              : autocontinue to next wp (uint8_t)
                param1                    : PARAM1, see MAV_CMD enum (float)
                param2                    : PARAM2, see MAV_CMD enum (float)
                param3                    : PARAM3, see MAV_CMD enum (float)
                param4                    : PARAM4, see MAV_CMD enum (float)
                x                         : PARAM5 / local: x position in meters * 1e4, global: latitude in degrees * 10^7 (int32_t)
                y                         : PARAM6 / y position: local: x position in meters * 1e4, global: longitude in degrees *10^7 (int32_t)
                z                         : PARAM7 / z position: global: altitude in meters (relative or absolute, depending on frame. (float)

*/
mavlink.messages.mission_item_int = function(target_system, target_component, seq, frame, command, current, autocontinue, param1, param2, param3, param4, x, y, z) {

    this.format = '<ffffiifHHBBBBB';
    this.id = mavlink.MAVLINK_MSG_ID_MISSION_ITEM_INT;
    this.order_map = [9, 10, 7, 11, 8, 12, 13, 0, 1, 2, 3, 4, 5, 6];
    this.crc_extra = 38;
    this.name = 'MISSION_ITEM_INT';

    this.fieldnames = ['target_system', 'target_component', 'seq', 'frame', 'command', 'current', 'autocontinue', 'param1', 'param2', 'param3', 'param4', 'x', 'y', 'z'];


    this.set(arguments);

}
        
mavlink.messages.mission_item_int.prototype = new mavlink.message;

mavlink.messages.mission_item_int.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.param1, this.param2, this.param3, this.param4, this.x, this.y, this.z, this.seq, this.command, this.target_system, this.target_component, this.frame, this.current, this.autocontinue]));
}

/* 
Metrics typically displayed on a HUD for fixed wing aircraft

                airspeed                  : Current airspeed in m/s (float)
                groundspeed               : Current ground speed in m/s (float)
                heading                   : Current heading in degrees, in compass units (0..360, 0=north) (int16_t)
                throttle                  : Current throttle setting in integer percent, 0 to 100 (uint16_t)
                alt                       : Current altitude (MSL), in meters (float)
                climb                     : Current climb rate in meters/second (float)

*/
mavlink.messages.vfr_hud = function(airspeed, groundspeed, heading, throttle, alt, climb) {

    this.format = '<ffffhH';
    this.id = mavlink.MAVLINK_MSG_ID_VFR_HUD;
    this.order_map = [0, 1, 4, 5, 2, 3];
    this.crc_extra = 20;
    this.name = 'VFR_HUD';

    this.fieldnames = ['airspeed', 'groundspeed', 'heading', 'throttle', 'alt', 'climb'];


    this.set(arguments);

}
        
mavlink.messages.vfr_hud.prototype = new mavlink.message;

mavlink.messages.vfr_hud.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.airspeed, this.groundspeed, this.alt, this.climb, this.heading, this.throttle]));
}

/* 
Message encoding a command with parameters as scaled integers. Scaling
depends on the actual command value.

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                frame                     : The coordinate system of the COMMAND. see MAV_FRAME in mavlink_types.h (uint8_t)
                command                   : The scheduled action for the mission item. see MAV_CMD in common.xml MAVLink specs (uint16_t)
                current                   : false:0, true:1 (uint8_t)
                autocontinue              : autocontinue to next wp (uint8_t)
                param1                    : PARAM1, see MAV_CMD enum (float)
                param2                    : PARAM2, see MAV_CMD enum (float)
                param3                    : PARAM3, see MAV_CMD enum (float)
                param4                    : PARAM4, see MAV_CMD enum (float)
                x                         : PARAM5 / local: x position in meters * 1e4, global: latitude in degrees * 10^7 (int32_t)
                y                         : PARAM6 / local: y position in meters * 1e4, global: longitude in degrees * 10^7 (int32_t)
                z                         : PARAM7 / z position: global: altitude in meters (relative or absolute, depending on frame. (float)

*/
mavlink.messages.command_int = function(target_system, target_component, frame, command, current, autocontinue, param1, param2, param3, param4, x, y, z) {

    this.format = '<ffffiifHBBBBB';
    this.id = mavlink.MAVLINK_MSG_ID_COMMAND_INT;
    this.order_map = [8, 9, 10, 7, 11, 12, 0, 1, 2, 3, 4, 5, 6];
    this.crc_extra = 158;
    this.name = 'COMMAND_INT';

    this.fieldnames = ['target_system', 'target_component', 'frame', 'command', 'current', 'autocontinue', 'param1', 'param2', 'param3', 'param4', 'x', 'y', 'z'];


    this.set(arguments);

}
        
mavlink.messages.command_int.prototype = new mavlink.message;

mavlink.messages.command_int.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.param1, this.param2, this.param3, this.param4, this.x, this.y, this.z, this.command, this.target_system, this.target_component, this.frame, this.current, this.autocontinue]));
}

/* 
Send a command with up to seven parameters to the MAV

                target_system             : System which should execute the command (uint8_t)
                target_component          : Component which should execute the command, 0 for all components (uint8_t)
                command                   : Command ID, as defined by MAV_CMD enum. (uint16_t)
                confirmation              : 0: First transmission of this command. 1-255: Confirmation transmissions (e.g. for kill command) (uint8_t)
                param1                    : Parameter 1, as defined by MAV_CMD enum. (float)
                param2                    : Parameter 2, as defined by MAV_CMD enum. (float)
                param3                    : Parameter 3, as defined by MAV_CMD enum. (float)
                param4                    : Parameter 4, as defined by MAV_CMD enum. (float)
                param5                    : Parameter 5, as defined by MAV_CMD enum. (float)
                param6                    : Parameter 6, as defined by MAV_CMD enum. (float)
                param7                    : Parameter 7, as defined by MAV_CMD enum. (float)

*/
mavlink.messages.command_long = function(target_system, target_component, command, confirmation, param1, param2, param3, param4, param5, param6, param7) {

    this.format = '<fffffffHBBB';
    this.id = mavlink.MAVLINK_MSG_ID_COMMAND_LONG;
    this.order_map = [8, 9, 7, 10, 0, 1, 2, 3, 4, 5, 6];
    this.crc_extra = 152;
    this.name = 'COMMAND_LONG';

    this.fieldnames = ['target_system', 'target_component', 'command', 'confirmation', 'param1', 'param2', 'param3', 'param4', 'param5', 'param6', 'param7'];


    this.set(arguments);

}
        
mavlink.messages.command_long.prototype = new mavlink.message;

mavlink.messages.command_long.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.param1, this.param2, this.param3, this.param4, this.param5, this.param6, this.param7, this.command, this.target_system, this.target_component, this.confirmation]));
}

/* 
Report status of a command. Includes feedback wether the command was
executed.

                command                   : Command ID, as defined by MAV_CMD enum. (uint16_t)
                result                    : See MAV_RESULT enum (uint8_t)

*/
mavlink.messages.command_ack = function(command, result) {

    this.format = '<HB';
    this.id = mavlink.MAVLINK_MSG_ID_COMMAND_ACK;
    this.order_map = [0, 1];
    this.crc_extra = 143;
    this.name = 'COMMAND_ACK';

    this.fieldnames = ['command', 'result'];


    this.set(arguments);

}
        
mavlink.messages.command_ack.prototype = new mavlink.message;

mavlink.messages.command_ack.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.command, this.result]));
}

/* 
Setpoint in roll, pitch, yaw and thrust from the operator

                time_boot_ms              : Timestamp in milliseconds since system boot (uint32_t)
                roll                      : Desired roll rate in radians per second (float)
                pitch                     : Desired pitch rate in radians per second (float)
                yaw                       : Desired yaw rate in radians per second (float)
                thrust                    : Collective thrust, normalized to 0 .. 1 (float)
                mode_switch               : Flight mode switch position, 0.. 255 (uint8_t)
                manual_override_switch        : Override mode switch position, 0.. 255 (uint8_t)

*/
mavlink.messages.manual_setpoint = function(time_boot_ms, roll, pitch, yaw, thrust, mode_switch, manual_override_switch) {

    this.format = '<IffffBB';
    this.id = mavlink.MAVLINK_MSG_ID_MANUAL_SETPOINT;
    this.order_map = [0, 1, 2, 3, 4, 5, 6];
    this.crc_extra = 106;
    this.name = 'MANUAL_SETPOINT';

    this.fieldnames = ['time_boot_ms', 'roll', 'pitch', 'yaw', 'thrust', 'mode_switch', 'manual_override_switch'];


    this.set(arguments);

}
        
mavlink.messages.manual_setpoint.prototype = new mavlink.message;

mavlink.messages.manual_setpoint.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.roll, this.pitch, this.yaw, this.thrust, this.mode_switch, this.manual_override_switch]));
}

/* 
Set the vehicle attitude and body angular rates.

                time_boot_ms              : Timestamp in milliseconds since system boot (uint32_t)
                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                type_mask                 : Mappings: If any of these bits are set, the corresponding input should be ignored: bit 1: body roll rate, bit 2: body pitch rate, bit 3: body yaw rate. bit 4-bit 6: reserved, bit 7: throttle, bit 8: attitude (uint8_t)
                q                         : Attitude quaternion (w, x, y, z order, zero-rotation is 1, 0, 0, 0) (float)
                body_roll_rate            : Body roll rate in radians per second (float)
                body_pitch_rate           : Body roll rate in radians per second (float)
                body_yaw_rate             : Body roll rate in radians per second (float)
                thrust                    : Collective thrust, normalized to 0 .. 1 (-1 .. 1 for vehicles capable of reverse trust) (float)

*/
mavlink.messages.set_attitude_target = function(time_boot_ms, target_system, target_component, type_mask, q, body_roll_rate, body_pitch_rate, body_yaw_rate, thrust) {

    this.format = '<I4fffffBBB';
    this.id = mavlink.MAVLINK_MSG_ID_SET_ATTITUDE_TARGET;
    this.order_map = [0, 6, 7, 8, 1, 2, 3, 4, 5];
    this.crc_extra = 49;
    this.name = 'SET_ATTITUDE_TARGET';

    this.fieldnames = ['time_boot_ms', 'target_system', 'target_component', 'type_mask', 'q', 'body_roll_rate', 'body_pitch_rate', 'body_yaw_rate', 'thrust'];


    this.set(arguments);

}
        
mavlink.messages.set_attitude_target.prototype = new mavlink.message;

mavlink.messages.set_attitude_target.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.q, this.body_roll_rate, this.body_pitch_rate, this.body_yaw_rate, this.thrust, this.target_system, this.target_component, this.type_mask]));
}

/* 
Set the vehicle attitude and body angular rates.

                time_boot_ms              : Timestamp in milliseconds since system boot (uint32_t)
                type_mask                 : Mappings: If any of these bits are set, the corresponding input should be ignored: bit 1: body roll rate, bit 2: body pitch rate, bit 3: body yaw rate. bit 4-bit 7: reserved, bit 8: attitude (uint8_t)
                q                         : Attitude quaternion (w, x, y, z order, zero-rotation is 1, 0, 0, 0) (float)
                body_roll_rate            : Body roll rate in radians per second (float)
                body_pitch_rate           : Body roll rate in radians per second (float)
                body_yaw_rate             : Body roll rate in radians per second (float)
                thrust                    : Collective thrust, normalized to 0 .. 1 (-1 .. 1 for vehicles capable of reverse trust) (float)

*/
mavlink.messages.attitude_target = function(time_boot_ms, type_mask, q, body_roll_rate, body_pitch_rate, body_yaw_rate, thrust) {

    this.format = '<I4fffffB';
    this.id = mavlink.MAVLINK_MSG_ID_ATTITUDE_TARGET;
    this.order_map = [0, 6, 1, 2, 3, 4, 5];
    this.crc_extra = 22;
    this.name = 'ATTITUDE_TARGET';

    this.fieldnames = ['time_boot_ms', 'type_mask', 'q', 'body_roll_rate', 'body_pitch_rate', 'body_yaw_rate', 'thrust'];


    this.set(arguments);

}
        
mavlink.messages.attitude_target.prototype = new mavlink.message;

mavlink.messages.attitude_target.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.q, this.body_roll_rate, this.body_pitch_rate, this.body_yaw_rate, this.thrust, this.type_mask]));
}

/* 
Set vehicle position, velocity and acceleration setpoint in local
frame.

                time_boot_ms              : Timestamp in milliseconds since system boot (uint32_t)
                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                coordinate_frame          : Valid options are: MAV_FRAME_LOCAL_NED = 1, MAV_FRAME_LOCAL_OFFSET_NED = 7, MAV_FRAME_BODY_NED = 8, MAV_FRAME_BODY_OFFSET_NED = 9 (uint8_t)
                type_mask                 : Bitmask to indicate which dimensions should be ignored by the vehicle: a value of 0b0000000000000000 or 0b0000001000000000 indicates that none of the setpoint dimensions should be ignored. If bit 10 is set the floats afx afy afz should be interpreted as force instead of acceleration. Mapping: bit 1: x, bit 2: y, bit 3: z, bit 4: vx, bit 5: vy, bit 6: vz, bit 7: ax, bit 8: ay, bit 9: az, bit 10: is force setpoint, bit 11: yaw, bit 12: yaw rate (uint16_t)
                x                         : X Position in NED frame in meters (float)
                y                         : Y Position in NED frame in meters (float)
                z                         : Z Position in NED frame in meters (note, altitude is negative in NED) (float)
                vx                        : X velocity in NED frame in meter / s (float)
                vy                        : Y velocity in NED frame in meter / s (float)
                vz                        : Z velocity in NED frame in meter / s (float)
                afx                       : X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N (float)
                afy                       : Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N (float)
                afz                       : Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N (float)
                yaw                       : yaw setpoint in rad (float)
                yaw_rate                  : yaw rate setpoint in rad/s (float)

*/
mavlink.messages.set_position_target_local_ned = function(time_boot_ms, target_system, target_component, coordinate_frame, type_mask, x, y, z, vx, vy, vz, afx, afy, afz, yaw, yaw_rate) {

    this.format = '<IfffffffffffHBBB';
    this.id = mavlink.MAVLINK_MSG_ID_SET_POSITION_TARGET_LOCAL_NED;
    this.order_map = [0, 13, 14, 15, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];
    this.crc_extra = 143;
    this.name = 'SET_POSITION_TARGET_LOCAL_NED';

    this.fieldnames = ['time_boot_ms', 'target_system', 'target_component', 'coordinate_frame', 'type_mask', 'x', 'y', 'z', 'vx', 'vy', 'vz', 'afx', 'afy', 'afz', 'yaw', 'yaw_rate'];


    this.set(arguments);

}
        
mavlink.messages.set_position_target_local_ned.prototype = new mavlink.message;

mavlink.messages.set_position_target_local_ned.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.x, this.y, this.z, this.vx, this.vy, this.vz, this.afx, this.afy, this.afz, this.yaw, this.yaw_rate, this.type_mask, this.target_system, this.target_component, this.coordinate_frame]));
}

/* 
Set vehicle position, velocity and acceleration setpoint in local
frame.

                time_boot_ms              : Timestamp in milliseconds since system boot (uint32_t)
                coordinate_frame          : Valid options are: MAV_FRAME_LOCAL_NED = 1, MAV_FRAME_LOCAL_OFFSET_NED = 7, MAV_FRAME_BODY_NED = 8, MAV_FRAME_BODY_OFFSET_NED = 9 (uint8_t)
                type_mask                 : Bitmask to indicate which dimensions should be ignored by the vehicle: a value of 0b0000000000000000 or 0b0000001000000000 indicates that none of the setpoint dimensions should be ignored. If bit 10 is set the floats afx afy afz should be interpreted as force instead of acceleration. Mapping: bit 1: x, bit 2: y, bit 3: z, bit 4: vx, bit 5: vy, bit 6: vz, bit 7: ax, bit 8: ay, bit 9: az, bit 10: is force setpoint, bit 11: yaw, bit 12: yaw rate (uint16_t)
                x                         : X Position in NED frame in meters (float)
                y                         : Y Position in NED frame in meters (float)
                z                         : Z Position in NED frame in meters (note, altitude is negative in NED) (float)
                vx                        : X velocity in NED frame in meter / s (float)
                vy                        : Y velocity in NED frame in meter / s (float)
                vz                        : Z velocity in NED frame in meter / s (float)
                afx                       : X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N (float)
                afy                       : Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N (float)
                afz                       : Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N (float)
                yaw                       : yaw setpoint in rad (float)
                yaw_rate                  : yaw rate setpoint in rad/s (float)

*/
mavlink.messages.position_target_local_ned = function(time_boot_ms, coordinate_frame, type_mask, x, y, z, vx, vy, vz, afx, afy, afz, yaw, yaw_rate) {

    this.format = '<IfffffffffffHB';
    this.id = mavlink.MAVLINK_MSG_ID_POSITION_TARGET_LOCAL_NED;
    this.order_map = [0, 13, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];
    this.crc_extra = 140;
    this.name = 'POSITION_TARGET_LOCAL_NED';

    this.fieldnames = ['time_boot_ms', 'coordinate_frame', 'type_mask', 'x', 'y', 'z', 'vx', 'vy', 'vz', 'afx', 'afy', 'afz', 'yaw', 'yaw_rate'];


    this.set(arguments);

}
        
mavlink.messages.position_target_local_ned.prototype = new mavlink.message;

mavlink.messages.position_target_local_ned.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.x, this.y, this.z, this.vx, this.vy, this.vz, this.afx, this.afy, this.afz, this.yaw, this.yaw_rate, this.type_mask, this.coordinate_frame]));
}

/* 
Set vehicle position, velocity and acceleration setpoint in the WGS84
coordinate system.

                time_boot_ms              : Timestamp in milliseconds since system boot. The rationale for the timestamp in the setpoint is to allow the system to compensate for the transport delay of the setpoint. This allows the system to compensate processing latency. (uint32_t)
                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                coordinate_frame          : Valid options are: MAV_FRAME_GLOBAL_INT = 5, MAV_FRAME_GLOBAL_RELATIVE_ALT_INT = 6, MAV_FRAME_GLOBAL_TERRAIN_ALT_INT = 11 (uint8_t)
                type_mask                 : Bitmask to indicate which dimensions should be ignored by the vehicle: a value of 0b0000000000000000 or 0b0000001000000000 indicates that none of the setpoint dimensions should be ignored. If bit 10 is set the floats afx afy afz should be interpreted as force instead of acceleration. Mapping: bit 1: x, bit 2: y, bit 3: z, bit 4: vx, bit 5: vy, bit 6: vz, bit 7: ax, bit 8: ay, bit 9: az, bit 10: is force setpoint, bit 11: yaw, bit 12: yaw rate (uint16_t)
                lat_int                   : X Position in WGS84 frame in 1e7 * meters (int32_t)
                lon_int                   : Y Position in WGS84 frame in 1e7 * meters (int32_t)
                alt                       : Altitude in meters in AMSL altitude, not WGS84 if absolute or relative, above terrain if GLOBAL_TERRAIN_ALT_INT (float)
                vx                        : X velocity in NED frame in meter / s (float)
                vy                        : Y velocity in NED frame in meter / s (float)
                vz                        : Z velocity in NED frame in meter / s (float)
                afx                       : X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N (float)
                afy                       : Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N (float)
                afz                       : Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N (float)
                yaw                       : yaw setpoint in rad (float)
                yaw_rate                  : yaw rate setpoint in rad/s (float)

*/
mavlink.messages.set_position_target_global_int = function(time_boot_ms, target_system, target_component, coordinate_frame, type_mask, lat_int, lon_int, alt, vx, vy, vz, afx, afy, afz, yaw, yaw_rate) {

    this.format = '<IiifffffffffHBBB';
    this.id = mavlink.MAVLINK_MSG_ID_SET_POSITION_TARGET_GLOBAL_INT;
    this.order_map = [0, 13, 14, 15, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];
    this.crc_extra = 5;
    this.name = 'SET_POSITION_TARGET_GLOBAL_INT';

    this.fieldnames = ['time_boot_ms', 'target_system', 'target_component', 'coordinate_frame', 'type_mask', 'lat_int', 'lon_int', 'alt', 'vx', 'vy', 'vz', 'afx', 'afy', 'afz', 'yaw', 'yaw_rate'];


    this.set(arguments);

}
        
mavlink.messages.set_position_target_global_int.prototype = new mavlink.message;

mavlink.messages.set_position_target_global_int.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.lat_int, this.lon_int, this.alt, this.vx, this.vy, this.vz, this.afx, this.afy, this.afz, this.yaw, this.yaw_rate, this.type_mask, this.target_system, this.target_component, this.coordinate_frame]));
}

/* 
Set vehicle position, velocity and acceleration setpoint in the WGS84
coordinate system.

                time_boot_ms              : Timestamp in milliseconds since system boot. The rationale for the timestamp in the setpoint is to allow the system to compensate for the transport delay of the setpoint. This allows the system to compensate processing latency. (uint32_t)
                coordinate_frame          : Valid options are: MAV_FRAME_GLOBAL_INT = 5, MAV_FRAME_GLOBAL_RELATIVE_ALT_INT = 6, MAV_FRAME_GLOBAL_TERRAIN_ALT_INT = 11 (uint8_t)
                type_mask                 : Bitmask to indicate which dimensions should be ignored by the vehicle: a value of 0b0000000000000000 or 0b0000001000000000 indicates that none of the setpoint dimensions should be ignored. If bit 10 is set the floats afx afy afz should be interpreted as force instead of acceleration. Mapping: bit 1: x, bit 2: y, bit 3: z, bit 4: vx, bit 5: vy, bit 6: vz, bit 7: ax, bit 8: ay, bit 9: az, bit 10: is force setpoint, bit 11: yaw, bit 12: yaw rate (uint16_t)
                lat_int                   : X Position in WGS84 frame in 1e7 * meters (int32_t)
                lon_int                   : Y Position in WGS84 frame in 1e7 * meters (int32_t)
                alt                       : Altitude in meters in AMSL altitude, not WGS84 if absolute or relative, above terrain if GLOBAL_TERRAIN_ALT_INT (float)
                vx                        : X velocity in NED frame in meter / s (float)
                vy                        : Y velocity in NED frame in meter / s (float)
                vz                        : Z velocity in NED frame in meter / s (float)
                afx                       : X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N (float)
                afy                       : Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N (float)
                afz                       : Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N (float)
                yaw                       : yaw setpoint in rad (float)
                yaw_rate                  : yaw rate setpoint in rad/s (float)

*/
mavlink.messages.position_target_global_int = function(time_boot_ms, coordinate_frame, type_mask, lat_int, lon_int, alt, vx, vy, vz, afx, afy, afz, yaw, yaw_rate) {

    this.format = '<IiifffffffffHB';
    this.id = mavlink.MAVLINK_MSG_ID_POSITION_TARGET_GLOBAL_INT;
    this.order_map = [0, 13, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];
    this.crc_extra = 150;
    this.name = 'POSITION_TARGET_GLOBAL_INT';

    this.fieldnames = ['time_boot_ms', 'coordinate_frame', 'type_mask', 'lat_int', 'lon_int', 'alt', 'vx', 'vy', 'vz', 'afx', 'afy', 'afz', 'yaw', 'yaw_rate'];


    this.set(arguments);

}
        
mavlink.messages.position_target_global_int.prototype = new mavlink.message;

mavlink.messages.position_target_global_int.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.lat_int, this.lon_int, this.alt, this.vx, this.vy, this.vz, this.afx, this.afy, this.afz, this.yaw, this.yaw_rate, this.type_mask, this.coordinate_frame]));
}

/* 
The offset in X, Y, Z and yaw between the LOCAL_POSITION_NED messages
of MAV X and the global coordinate frame in NED coordinates.
Coordinate frame is right-handed, Z-axis down (aeronautical frame, NED
/ north-east-down convention)

                time_boot_ms              : Timestamp (milliseconds since system boot) (uint32_t)
                x                         : X Position (float)
                y                         : Y Position (float)
                z                         : Z Position (float)
                roll                      : Roll (float)
                pitch                     : Pitch (float)
                yaw                       : Yaw (float)

*/
mavlink.messages.local_position_ned_system_global_offset = function(time_boot_ms, x, y, z, roll, pitch, yaw) {

    this.format = '<Iffffff';
    this.id = mavlink.MAVLINK_MSG_ID_LOCAL_POSITION_NED_SYSTEM_GLOBAL_OFFSET;
    this.order_map = [0, 1, 2, 3, 4, 5, 6];
    this.crc_extra = 231;
    this.name = 'LOCAL_POSITION_NED_SYSTEM_GLOBAL_OFFSET';

    this.fieldnames = ['time_boot_ms', 'x', 'y', 'z', 'roll', 'pitch', 'yaw'];


    this.set(arguments);

}
        
mavlink.messages.local_position_ned_system_global_offset.prototype = new mavlink.message;

mavlink.messages.local_position_ned_system_global_offset.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.x, this.y, this.z, this.roll, this.pitch, this.yaw]));
}

/* 
DEPRECATED PACKET! Suffers from missing airspeed fields and
singularities due to Euler angles. Please use HIL_STATE_QUATERNION
instead. Sent from simulation to autopilot. This packet is useful for
high throughput applications such as hardware in the loop simulations.

                time_usec                 : Timestamp (microseconds since UNIX epoch or microseconds since system boot) (uint64_t)
                roll                      : Roll angle (rad) (float)
                pitch                     : Pitch angle (rad) (float)
                yaw                       : Yaw angle (rad) (float)
                rollspeed                 : Body frame roll / phi angular speed (rad/s) (float)
                pitchspeed                : Body frame pitch / theta angular speed (rad/s) (float)
                yawspeed                  : Body frame yaw / psi angular speed (rad/s) (float)
                lat                       : Latitude, expressed as * 1E7 (int32_t)
                lon                       : Longitude, expressed as * 1E7 (int32_t)
                alt                       : Altitude in meters, expressed as * 1000 (millimeters) (int32_t)
                vx                        : Ground X Speed (Latitude), expressed as m/s * 100 (int16_t)
                vy                        : Ground Y Speed (Longitude), expressed as m/s * 100 (int16_t)
                vz                        : Ground Z Speed (Altitude), expressed as m/s * 100 (int16_t)
                xacc                      : X acceleration (mg) (int16_t)
                yacc                      : Y acceleration (mg) (int16_t)
                zacc                      : Z acceleration (mg) (int16_t)

*/
mavlink.messages.hil_state = function(time_usec, roll, pitch, yaw, rollspeed, pitchspeed, yawspeed, lat, lon, alt, vx, vy, vz, xacc, yacc, zacc) {

    this.format = '<Qffffffiiihhhhhh';
    this.id = mavlink.MAVLINK_MSG_ID_HIL_STATE;
    this.order_map = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];
    this.crc_extra = 183;
    this.name = 'HIL_STATE';

    this.fieldnames = ['time_usec', 'roll', 'pitch', 'yaw', 'rollspeed', 'pitchspeed', 'yawspeed', 'lat', 'lon', 'alt', 'vx', 'vy', 'vz', 'xacc', 'yacc', 'zacc'];


    this.set(arguments);

}
        
mavlink.messages.hil_state.prototype = new mavlink.message;

mavlink.messages.hil_state.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_usec, this.roll, this.pitch, this.yaw, this.rollspeed, this.pitchspeed, this.yawspeed, this.lat, this.lon, this.alt, this.vx, this.vy, this.vz, this.xacc, this.yacc, this.zacc]));
}

/* 
Sent from autopilot to simulation. Hardware in the loop control
outputs

                time_usec                 : Timestamp (microseconds since UNIX epoch or microseconds since system boot) (uint64_t)
                roll_ailerons             : Control output -1 .. 1 (float)
                pitch_elevator            : Control output -1 .. 1 (float)
                yaw_rudder                : Control output -1 .. 1 (float)
                throttle                  : Throttle 0 .. 1 (float)
                aux1                      : Aux 1, -1 .. 1 (float)
                aux2                      : Aux 2, -1 .. 1 (float)
                aux3                      : Aux 3, -1 .. 1 (float)
                aux4                      : Aux 4, -1 .. 1 (float)
                mode                      : System mode (MAV_MODE) (uint8_t)
                nav_mode                  : Navigation mode (MAV_NAV_MODE) (uint8_t)

*/
mavlink.messages.hil_controls = function(time_usec, roll_ailerons, pitch_elevator, yaw_rudder, throttle, aux1, aux2, aux3, aux4, mode, nav_mode) {

    this.format = '<QffffffffBB';
    this.id = mavlink.MAVLINK_MSG_ID_HIL_CONTROLS;
    this.order_map = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
    this.crc_extra = 63;
    this.name = 'HIL_CONTROLS';

    this.fieldnames = ['time_usec', 'roll_ailerons', 'pitch_elevator', 'yaw_rudder', 'throttle', 'aux1', 'aux2', 'aux3', 'aux4', 'mode', 'nav_mode'];


    this.set(arguments);

}
        
mavlink.messages.hil_controls.prototype = new mavlink.message;

mavlink.messages.hil_controls.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_usec, this.roll_ailerons, this.pitch_elevator, this.yaw_rudder, this.throttle, this.aux1, this.aux2, this.aux3, this.aux4, this.mode, this.nav_mode]));
}

/* 
Sent from simulation to autopilot. The RAW values of the RC channels
received. The standard PPM modulation is as follows: 1000
microseconds: 0%, 2000 microseconds: 100%. Individual
receivers/transmitters might violate this specification.

                time_usec                 : Timestamp (microseconds since UNIX epoch or microseconds since system boot) (uint64_t)
                chan1_raw                 : RC channel 1 value, in microseconds (uint16_t)
                chan2_raw                 : RC channel 2 value, in microseconds (uint16_t)
                chan3_raw                 : RC channel 3 value, in microseconds (uint16_t)
                chan4_raw                 : RC channel 4 value, in microseconds (uint16_t)
                chan5_raw                 : RC channel 5 value, in microseconds (uint16_t)
                chan6_raw                 : RC channel 6 value, in microseconds (uint16_t)
                chan7_raw                 : RC channel 7 value, in microseconds (uint16_t)
                chan8_raw                 : RC channel 8 value, in microseconds (uint16_t)
                chan9_raw                 : RC channel 9 value, in microseconds (uint16_t)
                chan10_raw                : RC channel 10 value, in microseconds (uint16_t)
                chan11_raw                : RC channel 11 value, in microseconds (uint16_t)
                chan12_raw                : RC channel 12 value, in microseconds (uint16_t)
                rssi                      : Receive signal strength indicator, 0: 0%, 255: 100% (uint8_t)

*/
mavlink.messages.hil_rc_inputs_raw = function(time_usec, chan1_raw, chan2_raw, chan3_raw, chan4_raw, chan5_raw, chan6_raw, chan7_raw, chan8_raw, chan9_raw, chan10_raw, chan11_raw, chan12_raw, rssi) {

    this.format = '<QHHHHHHHHHHHHB';
    this.id = mavlink.MAVLINK_MSG_ID_HIL_RC_INPUTS_RAW;
    this.order_map = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13];
    this.crc_extra = 54;
    this.name = 'HIL_RC_INPUTS_RAW';

    this.fieldnames = ['time_usec', 'chan1_raw', 'chan2_raw', 'chan3_raw', 'chan4_raw', 'chan5_raw', 'chan6_raw', 'chan7_raw', 'chan8_raw', 'chan9_raw', 'chan10_raw', 'chan11_raw', 'chan12_raw', 'rssi'];


    this.set(arguments);

}
        
mavlink.messages.hil_rc_inputs_raw.prototype = new mavlink.message;

mavlink.messages.hil_rc_inputs_raw.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_usec, this.chan1_raw, this.chan2_raw, this.chan3_raw, this.chan4_raw, this.chan5_raw, this.chan6_raw, this.chan7_raw, this.chan8_raw, this.chan9_raw, this.chan10_raw, this.chan11_raw, this.chan12_raw, this.rssi]));
}

/* 
Optical flow from a flow sensor (e.g. optical mouse sensor)

                time_usec                 : Timestamp (UNIX) (uint64_t)
                sensor_id                 : Sensor ID (uint8_t)
                flow_x                    : Flow in pixels * 10 in x-sensor direction (dezi-pixels) (int16_t)
                flow_y                    : Flow in pixels * 10 in y-sensor direction (dezi-pixels) (int16_t)
                flow_comp_m_x             : Flow in meters in x-sensor direction, angular-speed compensated (float)
                flow_comp_m_y             : Flow in meters in y-sensor direction, angular-speed compensated (float)
                quality                   : Optical flow quality / confidence. 0: bad, 255: maximum quality (uint8_t)
                ground_distance           : Ground distance in meters. Positive value: distance known. Negative value: Unknown distance (float)

*/
mavlink.messages.optical_flow = function(time_usec, sensor_id, flow_x, flow_y, flow_comp_m_x, flow_comp_m_y, quality, ground_distance) {

    this.format = '<QfffhhBB';
    this.id = mavlink.MAVLINK_MSG_ID_OPTICAL_FLOW;
    this.order_map = [0, 6, 4, 5, 1, 2, 7, 3];
    this.crc_extra = 175;
    this.name = 'OPTICAL_FLOW';

    this.fieldnames = ['time_usec', 'sensor_id', 'flow_x', 'flow_y', 'flow_comp_m_x', 'flow_comp_m_y', 'quality', 'ground_distance'];


    this.set(arguments);

}
        
mavlink.messages.optical_flow.prototype = new mavlink.message;

mavlink.messages.optical_flow.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_usec, this.flow_comp_m_x, this.flow_comp_m_y, this.ground_distance, this.flow_x, this.flow_y, this.sensor_id, this.quality]));
}

/* 


                usec                      : Timestamp (microseconds, synced to UNIX time or since system boot) (uint64_t)
                x                         : Global X position (float)
                y                         : Global Y position (float)
                z                         : Global Z position (float)
                roll                      : Roll angle in rad (float)
                pitch                     : Pitch angle in rad (float)
                yaw                       : Yaw angle in rad (float)

*/
mavlink.messages.global_vision_position_estimate = function(usec, x, y, z, roll, pitch, yaw) {

    this.format = '<Qffffff';
    this.id = mavlink.MAVLINK_MSG_ID_GLOBAL_VISION_POSITION_ESTIMATE;
    this.order_map = [0, 1, 2, 3, 4, 5, 6];
    this.crc_extra = 102;
    this.name = 'GLOBAL_VISION_POSITION_ESTIMATE';

    this.fieldnames = ['usec', 'x', 'y', 'z', 'roll', 'pitch', 'yaw'];


    this.set(arguments);

}
        
mavlink.messages.global_vision_position_estimate.prototype = new mavlink.message;

mavlink.messages.global_vision_position_estimate.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.usec, this.x, this.y, this.z, this.roll, this.pitch, this.yaw]));
}

/* 


                usec                      : Timestamp (microseconds, synced to UNIX time or since system boot) (uint64_t)
                x                         : Global X position (float)
                y                         : Global Y position (float)
                z                         : Global Z position (float)
                roll                      : Roll angle in rad (float)
                pitch                     : Pitch angle in rad (float)
                yaw                       : Yaw angle in rad (float)

*/
mavlink.messages.vision_position_estimate = function(usec, x, y, z, roll, pitch, yaw) {

    this.format = '<Qffffff';
    this.id = mavlink.MAVLINK_MSG_ID_VISION_POSITION_ESTIMATE;
    this.order_map = [0, 1, 2, 3, 4, 5, 6];
    this.crc_extra = 158;
    this.name = 'VISION_POSITION_ESTIMATE';

    this.fieldnames = ['usec', 'x', 'y', 'z', 'roll', 'pitch', 'yaw'];


    this.set(arguments);

}
        
mavlink.messages.vision_position_estimate.prototype = new mavlink.message;

mavlink.messages.vision_position_estimate.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.usec, this.x, this.y, this.z, this.roll, this.pitch, this.yaw]));
}

/* 


                usec                      : Timestamp (microseconds, synced to UNIX time or since system boot) (uint64_t)
                x                         : Global X speed (float)
                y                         : Global Y speed (float)
                z                         : Global Z speed (float)

*/
mavlink.messages.vision_speed_estimate = function(usec, x, y, z) {

    this.format = '<Qfff';
    this.id = mavlink.MAVLINK_MSG_ID_VISION_SPEED_ESTIMATE;
    this.order_map = [0, 1, 2, 3];
    this.crc_extra = 208;
    this.name = 'VISION_SPEED_ESTIMATE';

    this.fieldnames = ['usec', 'x', 'y', 'z'];


    this.set(arguments);

}
        
mavlink.messages.vision_speed_estimate.prototype = new mavlink.message;

mavlink.messages.vision_speed_estimate.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.usec, this.x, this.y, this.z]));
}

/* 


                usec                      : Timestamp (microseconds, synced to UNIX time or since system boot) (uint64_t)
                x                         : Global X position (float)
                y                         : Global Y position (float)
                z                         : Global Z position (float)
                roll                      : Roll angle in rad (float)
                pitch                     : Pitch angle in rad (float)
                yaw                       : Yaw angle in rad (float)

*/
mavlink.messages.vicon_position_estimate = function(usec, x, y, z, roll, pitch, yaw) {

    this.format = '<Qffffff';
    this.id = mavlink.MAVLINK_MSG_ID_VICON_POSITION_ESTIMATE;
    this.order_map = [0, 1, 2, 3, 4, 5, 6];
    this.crc_extra = 56;
    this.name = 'VICON_POSITION_ESTIMATE';

    this.fieldnames = ['usec', 'x', 'y', 'z', 'roll', 'pitch', 'yaw'];


    this.set(arguments);

}
        
mavlink.messages.vicon_position_estimate.prototype = new mavlink.message;

mavlink.messages.vicon_position_estimate.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.usec, this.x, this.y, this.z, this.roll, this.pitch, this.yaw]));
}

/* 
The IMU readings in SI units in NED body frame

                time_usec                 : Timestamp (microseconds, synced to UNIX time or since system boot) (uint64_t)
                xacc                      : X acceleration (m/s^2) (float)
                yacc                      : Y acceleration (m/s^2) (float)
                zacc                      : Z acceleration (m/s^2) (float)
                xgyro                     : Angular speed around X axis (rad / sec) (float)
                ygyro                     : Angular speed around Y axis (rad / sec) (float)
                zgyro                     : Angular speed around Z axis (rad / sec) (float)
                xmag                      : X Magnetic field (Gauss) (float)
                ymag                      : Y Magnetic field (Gauss) (float)
                zmag                      : Z Magnetic field (Gauss) (float)
                abs_pressure              : Absolute pressure in millibar (float)
                diff_pressure             : Differential pressure in millibar (float)
                pressure_alt              : Altitude calculated from pressure (float)
                temperature               : Temperature in degrees celsius (float)
                fields_updated            : Bitmask for fields that have updated since last message, bit 0 = xacc, bit 12: temperature (uint16_t)

*/
mavlink.messages.highres_imu = function(time_usec, xacc, yacc, zacc, xgyro, ygyro, zgyro, xmag, ymag, zmag, abs_pressure, diff_pressure, pressure_alt, temperature, fields_updated) {

    this.format = '<QfffffffffffffH';
    this.id = mavlink.MAVLINK_MSG_ID_HIGHRES_IMU;
    this.order_map = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14];
    this.crc_extra = 93;
    this.name = 'HIGHRES_IMU';

    this.fieldnames = ['time_usec', 'xacc', 'yacc', 'zacc', 'xgyro', 'ygyro', 'zgyro', 'xmag', 'ymag', 'zmag', 'abs_pressure', 'diff_pressure', 'pressure_alt', 'temperature', 'fields_updated'];


    this.set(arguments);

}
        
mavlink.messages.highres_imu.prototype = new mavlink.message;

mavlink.messages.highres_imu.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_usec, this.xacc, this.yacc, this.zacc, this.xgyro, this.ygyro, this.zgyro, this.xmag, this.ymag, this.zmag, this.abs_pressure, this.diff_pressure, this.pressure_alt, this.temperature, this.fields_updated]));
}

/* 
Optical flow from an angular rate flow sensor (e.g. PX4FLOW or mouse
sensor)

                time_usec                 : Timestamp (microseconds, synced to UNIX time or since system boot) (uint64_t)
                sensor_id                 : Sensor ID (uint8_t)
                integration_time_us        : Integration time in microseconds. Divide integrated_x and integrated_y by the integration time to obtain average flow. The integration time also indicates the. (uint32_t)
                integrated_x              : Flow in radians around X axis (Sensor RH rotation about the X axis induces a positive flow. Sensor linear motion along the positive Y axis induces a negative flow.) (float)
                integrated_y              : Flow in radians around Y axis (Sensor RH rotation about the Y axis induces a positive flow. Sensor linear motion along the positive X axis induces a positive flow.) (float)
                integrated_xgyro          : RH rotation around X axis (rad) (float)
                integrated_ygyro          : RH rotation around Y axis (rad) (float)
                integrated_zgyro          : RH rotation around Z axis (rad) (float)
                temperature               : Temperature * 100 in centi-degrees Celsius (int16_t)
                quality                   : Optical flow quality / confidence. 0: no valid flow, 255: maximum quality (uint8_t)
                time_delta_distance_us        : Time in microseconds since the distance was sampled. (uint32_t)
                distance                  : Distance to the center of the flow field in meters. Positive value (including zero): distance known. Negative value: Unknown distance. (float)

*/
mavlink.messages.optical_flow_rad = function(time_usec, sensor_id, integration_time_us, integrated_x, integrated_y, integrated_xgyro, integrated_ygyro, integrated_zgyro, temperature, quality, time_delta_distance_us, distance) {

    this.format = '<QIfffffIfhBB';
    this.id = mavlink.MAVLINK_MSG_ID_OPTICAL_FLOW_RAD;
    this.order_map = [0, 10, 1, 2, 3, 4, 5, 6, 9, 11, 7, 8];
    this.crc_extra = 138;
    this.name = 'OPTICAL_FLOW_RAD';

    this.fieldnames = ['time_usec', 'sensor_id', 'integration_time_us', 'integrated_x', 'integrated_y', 'integrated_xgyro', 'integrated_ygyro', 'integrated_zgyro', 'temperature', 'quality', 'time_delta_distance_us', 'distance'];


    this.set(arguments);

}
        
mavlink.messages.optical_flow_rad.prototype = new mavlink.message;

mavlink.messages.optical_flow_rad.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_usec, this.integration_time_us, this.integrated_x, this.integrated_y, this.integrated_xgyro, this.integrated_ygyro, this.integrated_zgyro, this.time_delta_distance_us, this.distance, this.temperature, this.sensor_id, this.quality]));
}

/* 
The IMU readings in SI units in NED body frame

                time_usec                 : Timestamp (microseconds, synced to UNIX time or since system boot) (uint64_t)
                xacc                      : X acceleration (m/s^2) (float)
                yacc                      : Y acceleration (m/s^2) (float)
                zacc                      : Z acceleration (m/s^2) (float)
                xgyro                     : Angular speed around X axis in body frame (rad / sec) (float)
                ygyro                     : Angular speed around Y axis in body frame (rad / sec) (float)
                zgyro                     : Angular speed around Z axis in body frame (rad / sec) (float)
                xmag                      : X Magnetic field (Gauss) (float)
                ymag                      : Y Magnetic field (Gauss) (float)
                zmag                      : Z Magnetic field (Gauss) (float)
                abs_pressure              : Absolute pressure in millibar (float)
                diff_pressure             : Differential pressure (airspeed) in millibar (float)
                pressure_alt              : Altitude calculated from pressure (float)
                temperature               : Temperature in degrees celsius (float)
                fields_updated            : Bitmask for fields that have updated since last message, bit 0 = xacc, bit 12: temperature (uint32_t)

*/
mavlink.messages.hil_sensor = function(time_usec, xacc, yacc, zacc, xgyro, ygyro, zgyro, xmag, ymag, zmag, abs_pressure, diff_pressure, pressure_alt, temperature, fields_updated) {

    this.format = '<QfffffffffffffI';
    this.id = mavlink.MAVLINK_MSG_ID_HIL_SENSOR;
    this.order_map = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14];
    this.crc_extra = 108;
    this.name = 'HIL_SENSOR';

    this.fieldnames = ['time_usec', 'xacc', 'yacc', 'zacc', 'xgyro', 'ygyro', 'zgyro', 'xmag', 'ymag', 'zmag', 'abs_pressure', 'diff_pressure', 'pressure_alt', 'temperature', 'fields_updated'];


    this.set(arguments);

}
        
mavlink.messages.hil_sensor.prototype = new mavlink.message;

mavlink.messages.hil_sensor.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_usec, this.xacc, this.yacc, this.zacc, this.xgyro, this.ygyro, this.zgyro, this.xmag, this.ymag, this.zmag, this.abs_pressure, this.diff_pressure, this.pressure_alt, this.temperature, this.fields_updated]));
}

/* 
Status of simulation environment, if used

                q1                        : True attitude quaternion component 1, w (1 in null-rotation) (float)
                q2                        : True attitude quaternion component 2, x (0 in null-rotation) (float)
                q3                        : True attitude quaternion component 3, y (0 in null-rotation) (float)
                q4                        : True attitude quaternion component 4, z (0 in null-rotation) (float)
                roll                      : Attitude roll expressed as Euler angles, not recommended except for human-readable outputs (float)
                pitch                     : Attitude pitch expressed as Euler angles, not recommended except for human-readable outputs (float)
                yaw                       : Attitude yaw expressed as Euler angles, not recommended except for human-readable outputs (float)
                xacc                      : X acceleration m/s/s (float)
                yacc                      : Y acceleration m/s/s (float)
                zacc                      : Z acceleration m/s/s (float)
                xgyro                     : Angular speed around X axis rad/s (float)
                ygyro                     : Angular speed around Y axis rad/s (float)
                zgyro                     : Angular speed around Z axis rad/s (float)
                lat                       : Latitude in degrees (float)
                lon                       : Longitude in degrees (float)
                alt                       : Altitude in meters (float)
                std_dev_horz              : Horizontal position standard deviation (float)
                std_dev_vert              : Vertical position standard deviation (float)
                vn                        : True velocity in m/s in NORTH direction in earth-fixed NED frame (float)
                ve                        : True velocity in m/s in EAST direction in earth-fixed NED frame (float)
                vd                        : True velocity in m/s in DOWN direction in earth-fixed NED frame (float)

*/
mavlink.messages.sim_state = function(q1, q2, q3, q4, roll, pitch, yaw, xacc, yacc, zacc, xgyro, ygyro, zgyro, lat, lon, alt, std_dev_horz, std_dev_vert, vn, ve, vd) {

    this.format = '<fffffffffffffffffffff';
    this.id = mavlink.MAVLINK_MSG_ID_SIM_STATE;
    this.order_map = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20];
    this.crc_extra = 32;
    this.name = 'SIM_STATE';

    this.fieldnames = ['q1', 'q2', 'q3', 'q4', 'roll', 'pitch', 'yaw', 'xacc', 'yacc', 'zacc', 'xgyro', 'ygyro', 'zgyro', 'lat', 'lon', 'alt', 'std_dev_horz', 'std_dev_vert', 'vn', 've', 'vd'];


    this.set(arguments);

}
        
mavlink.messages.sim_state.prototype = new mavlink.message;

mavlink.messages.sim_state.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.q1, this.q2, this.q3, this.q4, this.roll, this.pitch, this.yaw, this.xacc, this.yacc, this.zacc, this.xgyro, this.ygyro, this.zgyro, this.lat, this.lon, this.alt, this.std_dev_horz, this.std_dev_vert, this.vn, this.ve, this.vd]));
}

/* 
Status generated by radio and injected into MAVLink stream.

                rssi                      : Local signal strength (uint8_t)
                remrssi                   : Remote signal strength (uint8_t)
                txbuf                     : Remaining free buffer space in percent. (uint8_t)
                noise                     : Background noise level (uint8_t)
                remnoise                  : Remote background noise level (uint8_t)
                rxerrors                  : Receive errors (uint16_t)
                fixed                     : Count of error corrected packets (uint16_t)

*/
mavlink.messages.radio_status = function(rssi, remrssi, txbuf, noise, remnoise, rxerrors, fixed) {

    this.format = '<HHBBBBB';
    this.id = mavlink.MAVLINK_MSG_ID_RADIO_STATUS;
    this.order_map = [2, 3, 4, 5, 6, 0, 1];
    this.crc_extra = 185;
    this.name = 'RADIO_STATUS';

    this.fieldnames = ['rssi', 'remrssi', 'txbuf', 'noise', 'remnoise', 'rxerrors', 'fixed'];


    this.set(arguments);

}
        
mavlink.messages.radio_status.prototype = new mavlink.message;

mavlink.messages.radio_status.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.rxerrors, this.fixed, this.rssi, this.remrssi, this.txbuf, this.noise, this.remnoise]));
}

/* 
File transfer message

                target_network            : Network ID (0 for broadcast) (uint8_t)
                target_system             : System ID (0 for broadcast) (uint8_t)
                target_component          : Component ID (0 for broadcast) (uint8_t)
                payload                   : Variable length payload. The length is defined by the remaining message length when subtracting the header and other fields.  The entire content of this block is opaque unless you understand any the encoding message_type.  The particular encoding used can be extension specific and might not always be documented as part of the mavlink specification. (uint8_t)

*/
mavlink.messages.file_transfer_protocol = function(target_network, target_system, target_component, payload) {

    this.format = '<BBB251s';
    this.id = mavlink.MAVLINK_MSG_ID_FILE_TRANSFER_PROTOCOL;
    this.order_map = [0, 1, 2, 3];
    this.crc_extra = 84;
    this.name = 'FILE_TRANSFER_PROTOCOL';

    this.fieldnames = ['target_network', 'target_system', 'target_component', 'payload'];


    this.set(arguments);

}
        
mavlink.messages.file_transfer_protocol.prototype = new mavlink.message;

mavlink.messages.file_transfer_protocol.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.target_network, this.target_system, this.target_component, this.payload]));
}

/* 
Time synchronization message.

                tc1                       : Time sync timestamp 1 (int64_t)
                ts1                       : Time sync timestamp 2 (int64_t)

*/
mavlink.messages.timesync = function(tc1, ts1) {

    this.format = '<qq';
    this.id = mavlink.MAVLINK_MSG_ID_TIMESYNC;
    this.order_map = [0, 1];
    this.crc_extra = 34;
    this.name = 'TIMESYNC';

    this.fieldnames = ['tc1', 'ts1'];


    this.set(arguments);

}
        
mavlink.messages.timesync.prototype = new mavlink.message;

mavlink.messages.timesync.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.tc1, this.ts1]));
}

/* 
The global position, as returned by the Global Positioning System
(GPS). This is                  NOT the global position estimate of
the sytem, but rather a RAW sensor value. See message GLOBAL_POSITION
for the global position estimate. Coordinate frame is right-handed,
Z-axis up (GPS frame).

                time_usec                 : Timestamp (microseconds since UNIX epoch or microseconds since system boot) (uint64_t)
                fix_type                  : 0-1: no fix, 2: 2D fix, 3: 3D fix. Some applications will not use the value of this field unless it is at least two, so always correctly fill in the fix. (uint8_t)
                lat                       : Latitude (WGS84), in degrees * 1E7 (int32_t)
                lon                       : Longitude (WGS84), in degrees * 1E7 (int32_t)
                alt                       : Altitude (AMSL, not WGS84), in meters * 1000 (positive for up) (int32_t)
                eph                       : GPS HDOP horizontal dilution of position in cm (m*100). If unknown, set to: 65535 (uint16_t)
                epv                       : GPS VDOP vertical dilution of position in cm (m*100). If unknown, set to: 65535 (uint16_t)
                vel                       : GPS ground speed (m/s * 100). If unknown, set to: 65535 (uint16_t)
                vn                        : GPS velocity in cm/s in NORTH direction in earth-fixed NED frame (int16_t)
                ve                        : GPS velocity in cm/s in EAST direction in earth-fixed NED frame (int16_t)
                vd                        : GPS velocity in cm/s in DOWN direction in earth-fixed NED frame (int16_t)
                cog                       : Course over ground (NOT heading, but direction of movement) in degrees * 100, 0.0..359.99 degrees. If unknown, set to: 65535 (uint16_t)
                satellites_visible        : Number of satellites visible. If unknown, set to 255 (uint8_t)

*/
mavlink.messages.hil_gps = function(time_usec, fix_type, lat, lon, alt, eph, epv, vel, vn, ve, vd, cog, satellites_visible) {

    this.format = '<QiiiHHHhhhHBB';
    this.id = mavlink.MAVLINK_MSG_ID_HIL_GPS;
    this.order_map = [0, 11, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12];
    this.crc_extra = 124;
    this.name = 'HIL_GPS';

    this.fieldnames = ['time_usec', 'fix_type', 'lat', 'lon', 'alt', 'eph', 'epv', 'vel', 'vn', 've', 'vd', 'cog', 'satellites_visible'];


    this.set(arguments);

}
        
mavlink.messages.hil_gps.prototype = new mavlink.message;

mavlink.messages.hil_gps.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_usec, this.lat, this.lon, this.alt, this.eph, this.epv, this.vel, this.vn, this.ve, this.vd, this.cog, this.fix_type, this.satellites_visible]));
}

/* 
Simulated optical flow from a flow sensor (e.g. PX4FLOW or optical
mouse sensor)

                time_usec                 : Timestamp (microseconds, synced to UNIX time or since system boot) (uint64_t)
                sensor_id                 : Sensor ID (uint8_t)
                integration_time_us        : Integration time in microseconds. Divide integrated_x and integrated_y by the integration time to obtain average flow. The integration time also indicates the. (uint32_t)
                integrated_x              : Flow in radians around X axis (Sensor RH rotation about the X axis induces a positive flow. Sensor linear motion along the positive Y axis induces a negative flow.) (float)
                integrated_y              : Flow in radians around Y axis (Sensor RH rotation about the Y axis induces a positive flow. Sensor linear motion along the positive X axis induces a positive flow.) (float)
                integrated_xgyro          : RH rotation around X axis (rad) (float)
                integrated_ygyro          : RH rotation around Y axis (rad) (float)
                integrated_zgyro          : RH rotation around Z axis (rad) (float)
                temperature               : Temperature * 100 in centi-degrees Celsius (int16_t)
                quality                   : Optical flow quality / confidence. 0: no valid flow, 255: maximum quality (uint8_t)
                time_delta_distance_us        : Time in microseconds since the distance was sampled. (uint32_t)
                distance                  : Distance to the center of the flow field in meters. Positive value (including zero): distance known. Negative value: Unknown distance. (float)

*/
mavlink.messages.hil_optical_flow = function(time_usec, sensor_id, integration_time_us, integrated_x, integrated_y, integrated_xgyro, integrated_ygyro, integrated_zgyro, temperature, quality, time_delta_distance_us, distance) {

    this.format = '<QIfffffIfhBB';
    this.id = mavlink.MAVLINK_MSG_ID_HIL_OPTICAL_FLOW;
    this.order_map = [0, 10, 1, 2, 3, 4, 5, 6, 9, 11, 7, 8];
    this.crc_extra = 237;
    this.name = 'HIL_OPTICAL_FLOW';

    this.fieldnames = ['time_usec', 'sensor_id', 'integration_time_us', 'integrated_x', 'integrated_y', 'integrated_xgyro', 'integrated_ygyro', 'integrated_zgyro', 'temperature', 'quality', 'time_delta_distance_us', 'distance'];


    this.set(arguments);

}
        
mavlink.messages.hil_optical_flow.prototype = new mavlink.message;

mavlink.messages.hil_optical_flow.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_usec, this.integration_time_us, this.integrated_x, this.integrated_y, this.integrated_xgyro, this.integrated_ygyro, this.integrated_zgyro, this.time_delta_distance_us, this.distance, this.temperature, this.sensor_id, this.quality]));
}

/* 
Sent from simulation to autopilot, avoids in contrast to HIL_STATE
singularities. This packet is useful for high throughput applications
such as hardware in the loop simulations.

                time_usec                 : Timestamp (microseconds since UNIX epoch or microseconds since system boot) (uint64_t)
                attitude_quaternion        : Vehicle attitude expressed as normalized quaternion in w, x, y, z order (with 1 0 0 0 being the null-rotation) (float)
                rollspeed                 : Body frame roll / phi angular speed (rad/s) (float)
                pitchspeed                : Body frame pitch / theta angular speed (rad/s) (float)
                yawspeed                  : Body frame yaw / psi angular speed (rad/s) (float)
                lat                       : Latitude, expressed as * 1E7 (int32_t)
                lon                       : Longitude, expressed as * 1E7 (int32_t)
                alt                       : Altitude in meters, expressed as * 1000 (millimeters) (int32_t)
                vx                        : Ground X Speed (Latitude), expressed as m/s * 100 (int16_t)
                vy                        : Ground Y Speed (Longitude), expressed as m/s * 100 (int16_t)
                vz                        : Ground Z Speed (Altitude), expressed as m/s * 100 (int16_t)
                ind_airspeed              : Indicated airspeed, expressed as m/s * 100 (uint16_t)
                true_airspeed             : True airspeed, expressed as m/s * 100 (uint16_t)
                xacc                      : X acceleration (mg) (int16_t)
                yacc                      : Y acceleration (mg) (int16_t)
                zacc                      : Z acceleration (mg) (int16_t)

*/
mavlink.messages.hil_state_quaternion = function(time_usec, attitude_quaternion, rollspeed, pitchspeed, yawspeed, lat, lon, alt, vx, vy, vz, ind_airspeed, true_airspeed, xacc, yacc, zacc) {

    this.format = '<Q4ffffiiihhhHHhhh';
    this.id = mavlink.MAVLINK_MSG_ID_HIL_STATE_QUATERNION;
    this.order_map = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];
    this.crc_extra = 4;
    this.name = 'HIL_STATE_QUATERNION';

    this.fieldnames = ['time_usec', 'attitude_quaternion', 'rollspeed', 'pitchspeed', 'yawspeed', 'lat', 'lon', 'alt', 'vx', 'vy', 'vz', 'ind_airspeed', 'true_airspeed', 'xacc', 'yacc', 'zacc'];


    this.set(arguments);

}
        
mavlink.messages.hil_state_quaternion.prototype = new mavlink.message;

mavlink.messages.hil_state_quaternion.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_usec, this.attitude_quaternion, this.rollspeed, this.pitchspeed, this.yawspeed, this.lat, this.lon, this.alt, this.vx, this.vy, this.vz, this.ind_airspeed, this.true_airspeed, this.xacc, this.yacc, this.zacc]));
}

/* 
The RAW IMU readings for secondary 9DOF sensor setup. This message
should contain the scaled values to the described units

                time_boot_ms              : Timestamp (milliseconds since system boot) (uint32_t)
                xacc                      : X acceleration (mg) (int16_t)
                yacc                      : Y acceleration (mg) (int16_t)
                zacc                      : Z acceleration (mg) (int16_t)
                xgyro                     : Angular speed around X axis (millirad /sec) (int16_t)
                ygyro                     : Angular speed around Y axis (millirad /sec) (int16_t)
                zgyro                     : Angular speed around Z axis (millirad /sec) (int16_t)
                xmag                      : X Magnetic field (milli tesla) (int16_t)
                ymag                      : Y Magnetic field (milli tesla) (int16_t)
                zmag                      : Z Magnetic field (milli tesla) (int16_t)

*/
mavlink.messages.scaled_imu2 = function(time_boot_ms, xacc, yacc, zacc, xgyro, ygyro, zgyro, xmag, ymag, zmag) {

    this.format = '<Ihhhhhhhhh';
    this.id = mavlink.MAVLINK_MSG_ID_SCALED_IMU2;
    this.order_map = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
    this.crc_extra = 76;
    this.name = 'SCALED_IMU2';

    this.fieldnames = ['time_boot_ms', 'xacc', 'yacc', 'zacc', 'xgyro', 'ygyro', 'zgyro', 'xmag', 'ymag', 'zmag'];


    this.set(arguments);

}
        
mavlink.messages.scaled_imu2.prototype = new mavlink.message;

mavlink.messages.scaled_imu2.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.xacc, this.yacc, this.zacc, this.xgyro, this.ygyro, this.zgyro, this.xmag, this.ymag, this.zmag]));
}

/* 
Request a list of available logs. On some systems calling this may
stop on-board logging until LOG_REQUEST_END is called.

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                start                     : First log id (0 for first available) (uint16_t)
                end                       : Last log id (0xffff for last available) (uint16_t)

*/
mavlink.messages.log_request_list = function(target_system, target_component, start, end) {

    this.format = '<HHBB';
    this.id = mavlink.MAVLINK_MSG_ID_LOG_REQUEST_LIST;
    this.order_map = [2, 3, 0, 1];
    this.crc_extra = 128;
    this.name = 'LOG_REQUEST_LIST';

    this.fieldnames = ['target_system', 'target_component', 'start', 'end'];


    this.set(arguments);

}
        
mavlink.messages.log_request_list.prototype = new mavlink.message;

mavlink.messages.log_request_list.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.start, this.end, this.target_system, this.target_component]));
}

/* 
Reply to LOG_REQUEST_LIST

                id                        : Log id (uint16_t)
                num_logs                  : Total number of logs (uint16_t)
                last_log_num              : High log number (uint16_t)
                time_utc                  : UTC timestamp of log in seconds since 1970, or 0 if not available (uint32_t)
                size                      : Size of the log (may be approximate) in bytes (uint32_t)

*/
mavlink.messages.log_entry = function(id, num_logs, last_log_num, time_utc, size) {

    this.format = '<IIHHH';
    this.id = mavlink.MAVLINK_MSG_ID_LOG_ENTRY;
    this.order_map = [2, 3, 4, 0, 1];
    this.crc_extra = 56;
    this.name = 'LOG_ENTRY';

    this.fieldnames = ['id', 'num_logs', 'last_log_num', 'time_utc', 'size'];


    this.set(arguments);

}
        
mavlink.messages.log_entry.prototype = new mavlink.message;

mavlink.messages.log_entry.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_utc, this.size, this.id, this.num_logs, this.last_log_num]));
}

/* 
Request a chunk of a log

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                id                        : Log id (from LOG_ENTRY reply) (uint16_t)
                ofs                       : Offset into the log (uint32_t)
                count                     : Number of bytes (uint32_t)

*/
mavlink.messages.log_request_data = function(target_system, target_component, id, ofs, count) {

    this.format = '<IIHBB';
    this.id = mavlink.MAVLINK_MSG_ID_LOG_REQUEST_DATA;
    this.order_map = [3, 4, 2, 0, 1];
    this.crc_extra = 116;
    this.name = 'LOG_REQUEST_DATA';

    this.fieldnames = ['target_system', 'target_component', 'id', 'ofs', 'count'];


    this.set(arguments);

}
        
mavlink.messages.log_request_data.prototype = new mavlink.message;

mavlink.messages.log_request_data.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.ofs, this.count, this.id, this.target_system, this.target_component]));
}

/* 
Reply to LOG_REQUEST_DATA

                id                        : Log id (from LOG_ENTRY reply) (uint16_t)
                ofs                       : Offset into the log (uint32_t)
                count                     : Number of bytes (zero for end of log) (uint8_t)
                data                      : log data (uint8_t)

*/
mavlink.messages.log_data = function(id, ofs, count, data) {

    this.format = '<IHB90s';
    this.id = mavlink.MAVLINK_MSG_ID_LOG_DATA;
    this.order_map = [1, 0, 2, 3];
    this.crc_extra = 134;
    this.name = 'LOG_DATA';

    this.fieldnames = ['id', 'ofs', 'count', 'data'];


    this.set(arguments);

}
        
mavlink.messages.log_data.prototype = new mavlink.message;

mavlink.messages.log_data.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.ofs, this.id, this.count, this.data]));
}

/* 
Erase all logs

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)

*/
mavlink.messages.log_erase = function(target_system, target_component) {

    this.format = '<BB';
    this.id = mavlink.MAVLINK_MSG_ID_LOG_ERASE;
    this.order_map = [0, 1];
    this.crc_extra = 237;
    this.name = 'LOG_ERASE';

    this.fieldnames = ['target_system', 'target_component'];


    this.set(arguments);

}
        
mavlink.messages.log_erase.prototype = new mavlink.message;

mavlink.messages.log_erase.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.target_system, this.target_component]));
}

/* 
Stop log transfer and resume normal logging

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)

*/
mavlink.messages.log_request_end = function(target_system, target_component) {

    this.format = '<BB';
    this.id = mavlink.MAVLINK_MSG_ID_LOG_REQUEST_END;
    this.order_map = [0, 1];
    this.crc_extra = 203;
    this.name = 'LOG_REQUEST_END';

    this.fieldnames = ['target_system', 'target_component'];


    this.set(arguments);

}
        
mavlink.messages.log_request_end.prototype = new mavlink.message;

mavlink.messages.log_request_end.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.target_system, this.target_component]));
}

/* 
data for injecting into the onboard GPS (used for DGPS)

                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                len                       : data length (uint8_t)
                data                      : raw data (110 is enough for 12 satellites of RTCMv2) (uint8_t)

*/
mavlink.messages.gps_inject_data = function(target_system, target_component, len, data) {

    this.format = '<BBB110s';
    this.id = mavlink.MAVLINK_MSG_ID_GPS_INJECT_DATA;
    this.order_map = [0, 1, 2, 3];
    this.crc_extra = 250;
    this.name = 'GPS_INJECT_DATA';

    this.fieldnames = ['target_system', 'target_component', 'len', 'data'];


    this.set(arguments);

}
        
mavlink.messages.gps_inject_data.prototype = new mavlink.message;

mavlink.messages.gps_inject_data.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.target_system, this.target_component, this.len, this.data]));
}

/* 
Second GPS data. Coordinate frame is right-handed, Z-axis up (GPS
frame).

                time_usec                 : Timestamp (microseconds since UNIX epoch or microseconds since system boot) (uint64_t)
                fix_type                  : 0-1: no fix, 2: 2D fix, 3: 3D fix, 4: DGPS fix, 5: RTK Fix. Some applications will not use the value of this field unless it is at least two, so always correctly fill in the fix. (uint8_t)
                lat                       : Latitude (WGS84), in degrees * 1E7 (int32_t)
                lon                       : Longitude (WGS84), in degrees * 1E7 (int32_t)
                alt                       : Altitude (AMSL, not WGS84), in meters * 1000 (positive for up) (int32_t)
                eph                       : GPS HDOP horizontal dilution of position in cm (m*100). If unknown, set to: UINT16_MAX (uint16_t)
                epv                       : GPS VDOP vertical dilution of position in cm (m*100). If unknown, set to: UINT16_MAX (uint16_t)
                vel                       : GPS ground speed (m/s * 100). If unknown, set to: UINT16_MAX (uint16_t)
                cog                       : Course over ground (NOT heading, but direction of movement) in degrees * 100, 0.0..359.99 degrees. If unknown, set to: UINT16_MAX (uint16_t)
                satellites_visible        : Number of satellites visible. If unknown, set to 255 (uint8_t)
                dgps_numch                : Number of DGPS satellites (uint8_t)
                dgps_age                  : Age of DGPS info (uint32_t)

*/
mavlink.messages.gps2_raw = function(time_usec, fix_type, lat, lon, alt, eph, epv, vel, cog, satellites_visible, dgps_numch, dgps_age) {

    this.format = '<QiiiIHHHHBBB';
    this.id = mavlink.MAVLINK_MSG_ID_GPS2_RAW;
    this.order_map = [0, 9, 1, 2, 3, 5, 6, 7, 8, 10, 11, 4];
    this.crc_extra = 87;
    this.name = 'GPS2_RAW';

    this.fieldnames = ['time_usec', 'fix_type', 'lat', 'lon', 'alt', 'eph', 'epv', 'vel', 'cog', 'satellites_visible', 'dgps_numch', 'dgps_age'];


    this.set(arguments);

}
        
mavlink.messages.gps2_raw.prototype = new mavlink.message;

mavlink.messages.gps2_raw.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_usec, this.lat, this.lon, this.alt, this.dgps_age, this.eph, this.epv, this.vel, this.cog, this.fix_type, this.satellites_visible, this.dgps_numch]));
}

/* 
Power supply status

                Vcc                       : 5V rail voltage in millivolts (uint16_t)
                Vservo                    : servo rail voltage in millivolts (uint16_t)
                flags                     : power supply status flags (see MAV_POWER_STATUS enum) (uint16_t)

*/
mavlink.messages.power_status = function(Vcc, Vservo, flags) {

    this.format = '<HHH';
    this.id = mavlink.MAVLINK_MSG_ID_POWER_STATUS;
    this.order_map = [0, 1, 2];
    this.crc_extra = 203;
    this.name = 'POWER_STATUS';

    this.fieldnames = ['Vcc', 'Vservo', 'flags'];


    this.set(arguments);

}
        
mavlink.messages.power_status.prototype = new mavlink.message;

mavlink.messages.power_status.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.Vcc, this.Vservo, this.flags]));
}

/* 
Control a serial port. This can be used for raw access to an onboard
serial peripheral such as a GPS or telemetry radio. It is designed to
make it possible to update the devices firmware via MAVLink messages
or change the devices settings. A message with zero bytes can be used
to change just the baudrate.

                device                    : See SERIAL_CONTROL_DEV enum (uint8_t)
                flags                     : See SERIAL_CONTROL_FLAG enum (uint8_t)
                timeout                   : Timeout for reply data in milliseconds (uint16_t)
                baudrate                  : Baudrate of transfer. Zero means no change. (uint32_t)
                count                     : how many bytes in this transfer (uint8_t)
                data                      : serial data (uint8_t)

*/
mavlink.messages.serial_control = function(device, flags, timeout, baudrate, count, data) {

    this.format = '<IHBBB70s';
    this.id = mavlink.MAVLINK_MSG_ID_SERIAL_CONTROL;
    this.order_map = [2, 3, 1, 0, 4, 5];
    this.crc_extra = 220;
    this.name = 'SERIAL_CONTROL';

    this.fieldnames = ['device', 'flags', 'timeout', 'baudrate', 'count', 'data'];


    this.set(arguments);

}
        
mavlink.messages.serial_control.prototype = new mavlink.message;

mavlink.messages.serial_control.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.baudrate, this.timeout, this.device, this.flags, this.count, this.data]));
}

/* 
RTK GPS data. Gives information on the relative baseline calculation
the GPS is reporting

                time_last_baseline_ms        : Time since boot of last baseline message received in ms. (uint32_t)
                rtk_receiver_id           : Identification of connected RTK receiver. (uint8_t)
                wn                        : GPS Week Number of last baseline (uint16_t)
                tow                       : GPS Time of Week of last baseline (uint32_t)
                rtk_health                : GPS-specific health report for RTK data. (uint8_t)
                rtk_rate                  : Rate of baseline messages being received by GPS, in HZ (uint8_t)
                nsats                     : Current number of sats used for RTK calculation. (uint8_t)
                baseline_coords_type        : Coordinate system of baseline. 0 == ECEF, 1 == NED (uint8_t)
                baseline_a_mm             : Current baseline in ECEF x or NED north component in mm. (int32_t)
                baseline_b_mm             : Current baseline in ECEF y or NED east component in mm. (int32_t)
                baseline_c_mm             : Current baseline in ECEF z or NED down component in mm. (int32_t)
                accuracy                  : Current estimate of baseline accuracy. (uint32_t)
                iar_num_hypotheses        : Current number of integer ambiguity hypotheses. (int32_t)

*/
mavlink.messages.gps_rtk = function(time_last_baseline_ms, rtk_receiver_id, wn, tow, rtk_health, rtk_rate, nsats, baseline_coords_type, baseline_a_mm, baseline_b_mm, baseline_c_mm, accuracy, iar_num_hypotheses) {

    this.format = '<IIiiiIiHBBBBB';
    this.id = mavlink.MAVLINK_MSG_ID_GPS_RTK;
    this.order_map = [0, 8, 7, 1, 9, 10, 11, 12, 2, 3, 4, 5, 6];
    this.crc_extra = 25;
    this.name = 'GPS_RTK';

    this.fieldnames = ['time_last_baseline_ms', 'rtk_receiver_id', 'wn', 'tow', 'rtk_health', 'rtk_rate', 'nsats', 'baseline_coords_type', 'baseline_a_mm', 'baseline_b_mm', 'baseline_c_mm', 'accuracy', 'iar_num_hypotheses'];


    this.set(arguments);

}
        
mavlink.messages.gps_rtk.prototype = new mavlink.message;

mavlink.messages.gps_rtk.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_last_baseline_ms, this.tow, this.baseline_a_mm, this.baseline_b_mm, this.baseline_c_mm, this.accuracy, this.iar_num_hypotheses, this.wn, this.rtk_receiver_id, this.rtk_health, this.rtk_rate, this.nsats, this.baseline_coords_type]));
}

/* 
RTK GPS data. Gives information on the relative baseline calculation
the GPS is reporting

                time_last_baseline_ms        : Time since boot of last baseline message received in ms. (uint32_t)
                rtk_receiver_id           : Identification of connected RTK receiver. (uint8_t)
                wn                        : GPS Week Number of last baseline (uint16_t)
                tow                       : GPS Time of Week of last baseline (uint32_t)
                rtk_health                : GPS-specific health report for RTK data. (uint8_t)
                rtk_rate                  : Rate of baseline messages being received by GPS, in HZ (uint8_t)
                nsats                     : Current number of sats used for RTK calculation. (uint8_t)
                baseline_coords_type        : Coordinate system of baseline. 0 == ECEF, 1 == NED (uint8_t)
                baseline_a_mm             : Current baseline in ECEF x or NED north component in mm. (int32_t)
                baseline_b_mm             : Current baseline in ECEF y or NED east component in mm. (int32_t)
                baseline_c_mm             : Current baseline in ECEF z or NED down component in mm. (int32_t)
                accuracy                  : Current estimate of baseline accuracy. (uint32_t)
                iar_num_hypotheses        : Current number of integer ambiguity hypotheses. (int32_t)

*/
mavlink.messages.gps2_rtk = function(time_last_baseline_ms, rtk_receiver_id, wn, tow, rtk_health, rtk_rate, nsats, baseline_coords_type, baseline_a_mm, baseline_b_mm, baseline_c_mm, accuracy, iar_num_hypotheses) {

    this.format = '<IIiiiIiHBBBBB';
    this.id = mavlink.MAVLINK_MSG_ID_GPS2_RTK;
    this.order_map = [0, 8, 7, 1, 9, 10, 11, 12, 2, 3, 4, 5, 6];
    this.crc_extra = 226;
    this.name = 'GPS2_RTK';

    this.fieldnames = ['time_last_baseline_ms', 'rtk_receiver_id', 'wn', 'tow', 'rtk_health', 'rtk_rate', 'nsats', 'baseline_coords_type', 'baseline_a_mm', 'baseline_b_mm', 'baseline_c_mm', 'accuracy', 'iar_num_hypotheses'];


    this.set(arguments);

}
        
mavlink.messages.gps2_rtk.prototype = new mavlink.message;

mavlink.messages.gps2_rtk.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_last_baseline_ms, this.tow, this.baseline_a_mm, this.baseline_b_mm, this.baseline_c_mm, this.accuracy, this.iar_num_hypotheses, this.wn, this.rtk_receiver_id, this.rtk_health, this.rtk_rate, this.nsats, this.baseline_coords_type]));
}

/* 
The RAW IMU readings for 3rd 9DOF sensor setup. This message should
contain the scaled values to the described units

                time_boot_ms              : Timestamp (milliseconds since system boot) (uint32_t)
                xacc                      : X acceleration (mg) (int16_t)
                yacc                      : Y acceleration (mg) (int16_t)
                zacc                      : Z acceleration (mg) (int16_t)
                xgyro                     : Angular speed around X axis (millirad /sec) (int16_t)
                ygyro                     : Angular speed around Y axis (millirad /sec) (int16_t)
                zgyro                     : Angular speed around Z axis (millirad /sec) (int16_t)
                xmag                      : X Magnetic field (milli tesla) (int16_t)
                ymag                      : Y Magnetic field (milli tesla) (int16_t)
                zmag                      : Z Magnetic field (milli tesla) (int16_t)

*/
mavlink.messages.scaled_imu3 = function(time_boot_ms, xacc, yacc, zacc, xgyro, ygyro, zgyro, xmag, ymag, zmag) {

    this.format = '<Ihhhhhhhhh';
    this.id = mavlink.MAVLINK_MSG_ID_SCALED_IMU3;
    this.order_map = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
    this.crc_extra = 46;
    this.name = 'SCALED_IMU3';

    this.fieldnames = ['time_boot_ms', 'xacc', 'yacc', 'zacc', 'xgyro', 'ygyro', 'zgyro', 'xmag', 'ymag', 'zmag'];


    this.set(arguments);

}
        
mavlink.messages.scaled_imu3.prototype = new mavlink.message;

mavlink.messages.scaled_imu3.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.xacc, this.yacc, this.zacc, this.xgyro, this.ygyro, this.zgyro, this.xmag, this.ymag, this.zmag]));
}

/* 


                type                      : type of requested/acknowledged data (as defined in ENUM DATA_TYPES in mavlink/include/mavlink_types.h) (uint8_t)
                size                      : total data size in bytes (set on ACK only) (uint32_t)
                width                     : Width of a matrix or image (uint16_t)
                height                    : Height of a matrix or image (uint16_t)
                packets                   : number of packets beeing sent (set on ACK only) (uint16_t)
                payload                   : payload size per packet (normally 253 byte, see DATA field size in message ENCAPSULATED_DATA) (set on ACK only) (uint8_t)
                jpg_quality               : JPEG quality out of [1,100] (uint8_t)

*/
mavlink.messages.data_transmission_handshake = function(type, size, width, height, packets, payload, jpg_quality) {

    this.format = '<IHHHBBB';
    this.id = mavlink.MAVLINK_MSG_ID_DATA_TRANSMISSION_HANDSHAKE;
    this.order_map = [4, 0, 1, 2, 3, 5, 6];
    this.crc_extra = 29;
    this.name = 'DATA_TRANSMISSION_HANDSHAKE';

    this.fieldnames = ['type', 'size', 'width', 'height', 'packets', 'payload', 'jpg_quality'];


    this.set(arguments);

}
        
mavlink.messages.data_transmission_handshake.prototype = new mavlink.message;

mavlink.messages.data_transmission_handshake.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.size, this.width, this.height, this.packets, this.type, this.payload, this.jpg_quality]));
}

/* 


                seqnr                     : sequence number (starting with 0 on every transmission) (uint16_t)
                data                      : image data bytes (uint8_t)

*/
mavlink.messages.encapsulated_data = function(seqnr, data) {

    this.format = '<H253s';
    this.id = mavlink.MAVLINK_MSG_ID_ENCAPSULATED_DATA;
    this.order_map = [0, 1];
    this.crc_extra = 223;
    this.name = 'ENCAPSULATED_DATA';

    this.fieldnames = ['seqnr', 'data'];


    this.set(arguments);

}
        
mavlink.messages.encapsulated_data.prototype = new mavlink.message;

mavlink.messages.encapsulated_data.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.seqnr, this.data]));
}

/* 


                time_boot_ms              : Time since system boot (uint32_t)
                min_distance              : Minimum distance the sensor can measure in centimeters (uint16_t)
                max_distance              : Maximum distance the sensor can measure in centimeters (uint16_t)
                current_distance          : Current distance reading (uint16_t)
                type                      : Type from MAV_DISTANCE_SENSOR enum. (uint8_t)
                id                        : Onboard ID of the sensor (uint8_t)
                orientation               : Direction the sensor faces from FIXME enum. (uint8_t)
                covariance                : Measurement covariance in centimeters, 0 for unknown / invalid readings (uint8_t)

*/
mavlink.messages.distance_sensor = function(time_boot_ms, min_distance, max_distance, current_distance, type, id, orientation, covariance) {

    this.format = '<IHHHBBBB';
    this.id = mavlink.MAVLINK_MSG_ID_DISTANCE_SENSOR;
    this.order_map = [0, 1, 2, 3, 4, 5, 6, 7];
    this.crc_extra = 85;
    this.name = 'DISTANCE_SENSOR';

    this.fieldnames = ['time_boot_ms', 'min_distance', 'max_distance', 'current_distance', 'type', 'id', 'orientation', 'covariance'];


    this.set(arguments);

}
        
mavlink.messages.distance_sensor.prototype = new mavlink.message;

mavlink.messages.distance_sensor.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.min_distance, this.max_distance, this.current_distance, this.type, this.id, this.orientation, this.covariance]));
}

/* 
Request for terrain data and terrain status

                lat                       : Latitude of SW corner of first grid (degrees *10^7) (int32_t)
                lon                       : Longitude of SW corner of first grid (in degrees *10^7) (int32_t)
                grid_spacing              : Grid spacing in meters (uint16_t)
                mask                      : Bitmask of requested 4x4 grids (row major 8x7 array of grids, 56 bits) (uint64_t)

*/
mavlink.messages.terrain_request = function(lat, lon, grid_spacing, mask) {

    this.format = '<QiiH';
    this.id = mavlink.MAVLINK_MSG_ID_TERRAIN_REQUEST;
    this.order_map = [1, 2, 3, 0];
    this.crc_extra = 6;
    this.name = 'TERRAIN_REQUEST';

    this.fieldnames = ['lat', 'lon', 'grid_spacing', 'mask'];


    this.set(arguments);

}
        
mavlink.messages.terrain_request.prototype = new mavlink.message;

mavlink.messages.terrain_request.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.mask, this.lat, this.lon, this.grid_spacing]));
}

/* 
Terrain data sent from GCS. The lat/lon and grid_spacing must be the
same as a lat/lon from a TERRAIN_REQUEST

                lat                       : Latitude of SW corner of first grid (degrees *10^7) (int32_t)
                lon                       : Longitude of SW corner of first grid (in degrees *10^7) (int32_t)
                grid_spacing              : Grid spacing in meters (uint16_t)
                gridbit                   : bit within the terrain request mask (uint8_t)
                data                      : Terrain data in meters AMSL (int16_t)

*/
mavlink.messages.terrain_data = function(lat, lon, grid_spacing, gridbit, data) {

    this.format = '<iiH16hB';
    this.id = mavlink.MAVLINK_MSG_ID_TERRAIN_DATA;
    this.order_map = [0, 1, 2, 4, 3];
    this.crc_extra = 229;
    this.name = 'TERRAIN_DATA';

    this.fieldnames = ['lat', 'lon', 'grid_spacing', 'gridbit', 'data'];


    this.set(arguments);

}
        
mavlink.messages.terrain_data.prototype = new mavlink.message;

mavlink.messages.terrain_data.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.lat, this.lon, this.grid_spacing, this.data, this.gridbit]));
}

/* 
Request that the vehicle report terrain height at the given location.
Used by GCS to check if vehicle has all terrain data needed for a
mission.

                lat                       : Latitude (degrees *10^7) (int32_t)
                lon                       : Longitude (degrees *10^7) (int32_t)

*/
mavlink.messages.terrain_check = function(lat, lon) {

    this.format = '<ii';
    this.id = mavlink.MAVLINK_MSG_ID_TERRAIN_CHECK;
    this.order_map = [0, 1];
    this.crc_extra = 203;
    this.name = 'TERRAIN_CHECK';

    this.fieldnames = ['lat', 'lon'];


    this.set(arguments);

}
        
mavlink.messages.terrain_check.prototype = new mavlink.message;

mavlink.messages.terrain_check.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.lat, this.lon]));
}

/* 
Response from a TERRAIN_CHECK request

                lat                       : Latitude (degrees *10^7) (int32_t)
                lon                       : Longitude (degrees *10^7) (int32_t)
                spacing                   : grid spacing (zero if terrain at this location unavailable) (uint16_t)
                terrain_height            : Terrain height in meters AMSL (float)
                current_height            : Current vehicle height above lat/lon terrain height (meters) (float)
                pending                   : Number of 4x4 terrain blocks waiting to be received or read from disk (uint16_t)
                loaded                    : Number of 4x4 terrain blocks in memory (uint16_t)

*/
mavlink.messages.terrain_report = function(lat, lon, spacing, terrain_height, current_height, pending, loaded) {

    this.format = '<iiffHHH';
    this.id = mavlink.MAVLINK_MSG_ID_TERRAIN_REPORT;
    this.order_map = [0, 1, 4, 2, 3, 5, 6];
    this.crc_extra = 1;
    this.name = 'TERRAIN_REPORT';

    this.fieldnames = ['lat', 'lon', 'spacing', 'terrain_height', 'current_height', 'pending', 'loaded'];


    this.set(arguments);

}
        
mavlink.messages.terrain_report.prototype = new mavlink.message;

mavlink.messages.terrain_report.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.lat, this.lon, this.terrain_height, this.current_height, this.spacing, this.pending, this.loaded]));
}

/* 
Barometer readings for 2nd barometer

                time_boot_ms              : Timestamp (milliseconds since system boot) (uint32_t)
                press_abs                 : Absolute pressure (hectopascal) (float)
                press_diff                : Differential pressure 1 (hectopascal) (float)
                temperature               : Temperature measurement (0.01 degrees celsius) (int16_t)

*/
mavlink.messages.scaled_pressure2 = function(time_boot_ms, press_abs, press_diff, temperature) {

    this.format = '<Iffh';
    this.id = mavlink.MAVLINK_MSG_ID_SCALED_PRESSURE2;
    this.order_map = [0, 1, 2, 3];
    this.crc_extra = 195;
    this.name = 'SCALED_PRESSURE2';

    this.fieldnames = ['time_boot_ms', 'press_abs', 'press_diff', 'temperature'];


    this.set(arguments);

}
        
mavlink.messages.scaled_pressure2.prototype = new mavlink.message;

mavlink.messages.scaled_pressure2.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.press_abs, this.press_diff, this.temperature]));
}

/* 
Motion capture attitude and position

                time_usec                 : Timestamp (micros since boot or Unix epoch) (uint64_t)
                q                         : Attitude quaternion (w, x, y, z order, zero-rotation is 1, 0, 0, 0) (float)
                x                         : X position in meters (NED) (float)
                y                         : Y position in meters (NED) (float)
                z                         : Z position in meters (NED) (float)

*/
mavlink.messages.att_pos_mocap = function(time_usec, q, x, y, z) {

    this.format = '<Q4ffff';
    this.id = mavlink.MAVLINK_MSG_ID_ATT_POS_MOCAP;
    this.order_map = [0, 1, 2, 3, 4];
    this.crc_extra = 109;
    this.name = 'ATT_POS_MOCAP';

    this.fieldnames = ['time_usec', 'q', 'x', 'y', 'z'];


    this.set(arguments);

}
        
mavlink.messages.att_pos_mocap.prototype = new mavlink.message;

mavlink.messages.att_pos_mocap.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_usec, this.q, this.x, this.y, this.z]));
}

/* 
Set the vehicle attitude and body angular rates.

                time_usec                 : Timestamp (micros since boot or Unix epoch) (uint64_t)
                group_mlx                 : Actuator group. The "_mlx" indicates this is a multi-instance message and a MAVLink parser should use this field to difference between instances. (uint8_t)
                target_system             : System ID (uint8_t)
                target_component          : Component ID (uint8_t)
                controls                  : Actuator controls. Normed to -1..+1 where 0 is neutral position. Throttle for single rotation direction motors is 0..1, negative range for reverse direction. Standard mapping for attitude controls (group 0): (index 0-7): roll, pitch, yaw, throttle, flaps, spoilers, airbrakes, landing gear. Load a pass-through mixer to repurpose them as generic outputs. (float)

*/
mavlink.messages.set_actuator_control_target = function(time_usec, group_mlx, target_system, target_component, controls) {

    this.format = '<Q8fBBB';
    this.id = mavlink.MAVLINK_MSG_ID_SET_ACTUATOR_CONTROL_TARGET;
    this.order_map = [0, 2, 3, 4, 1];
    this.crc_extra = 168;
    this.name = 'SET_ACTUATOR_CONTROL_TARGET';

    this.fieldnames = ['time_usec', 'group_mlx', 'target_system', 'target_component', 'controls'];


    this.set(arguments);

}
        
mavlink.messages.set_actuator_control_target.prototype = new mavlink.message;

mavlink.messages.set_actuator_control_target.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_usec, this.controls, this.group_mlx, this.target_system, this.target_component]));
}

/* 
Set the vehicle attitude and body angular rates.

                time_usec                 : Timestamp (micros since boot or Unix epoch) (uint64_t)
                group_mlx                 : Actuator group. The "_mlx" indicates this is a multi-instance message and a MAVLink parser should use this field to difference between instances. (uint8_t)
                controls                  : Actuator controls. Normed to -1..+1 where 0 is neutral position. Throttle for single rotation direction motors is 0..1, negative range for reverse direction. Standard mapping for attitude controls (group 0): (index 0-7): roll, pitch, yaw, throttle, flaps, spoilers, airbrakes, landing gear. Load a pass-through mixer to repurpose them as generic outputs. (float)

*/
mavlink.messages.actuator_control_target = function(time_usec, group_mlx, controls) {

    this.format = '<Q8fB';
    this.id = mavlink.MAVLINK_MSG_ID_ACTUATOR_CONTROL_TARGET;
    this.order_map = [0, 2, 1];
    this.crc_extra = 181;
    this.name = 'ACTUATOR_CONTROL_TARGET';

    this.fieldnames = ['time_usec', 'group_mlx', 'controls'];


    this.set(arguments);

}
        
mavlink.messages.actuator_control_target.prototype = new mavlink.message;

mavlink.messages.actuator_control_target.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_usec, this.controls, this.group_mlx]));
}

/* 
Battery information

                id                        : Battery ID (uint8_t)
                battery_function          : Function of the battery (uint8_t)
                type                      : Type (chemistry) of the battery (uint8_t)
                temperature               : Temperature of the battery in centi-degrees celsius. INT16_MAX for unknown temperature. (int16_t)
                voltages                  : Battery voltage of cells, in millivolts (1 = 1 millivolt) (uint16_t)
                current_battery           : Battery current, in 10*milliamperes (1 = 10 milliampere), -1: autopilot does not measure the current (int16_t)
                current_consumed          : Consumed charge, in milliampere hours (1 = 1 mAh), -1: autopilot does not provide mAh consumption estimate (int32_t)
                energy_consumed           : Consumed energy, in 100*Joules (intergrated U*I*dt)  (1 = 100 Joule), -1: autopilot does not provide energy consumption estimate (int32_t)
                battery_remaining         : Remaining battery energy: (0%: 0, 100%: 100), -1: autopilot does not estimate the remaining battery (int8_t)

*/
mavlink.messages.battery_status = function(id, battery_function, type, temperature, voltages, current_battery, current_consumed, energy_consumed, battery_remaining) {

    this.format = '<iih10HhBBBb';
    this.id = mavlink.MAVLINK_MSG_ID_BATTERY_STATUS;
    this.order_map = [5, 6, 7, 2, 3, 4, 0, 1, 8];
    this.crc_extra = 154;
    this.name = 'BATTERY_STATUS';

    this.fieldnames = ['id', 'battery_function', 'type', 'temperature', 'voltages', 'current_battery', 'current_consumed', 'energy_consumed', 'battery_remaining'];


    this.set(arguments);

}
        
mavlink.messages.battery_status.prototype = new mavlink.message;

mavlink.messages.battery_status.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.current_consumed, this.energy_consumed, this.temperature, this.voltages, this.current_battery, this.id, this.battery_function, this.type, this.battery_remaining]));
}

/* 
Version and capability of autopilot software

                capabilities              : bitmask of capabilities (see MAV_PROTOCOL_CAPABILITY enum) (uint64_t)
                flight_sw_version         : Firmware version number (uint32_t)
                middleware_sw_version        : Middleware version number (uint32_t)
                os_sw_version             : Operating system version number (uint32_t)
                board_version             : HW / board version (last 8 bytes should be silicon ID, if any) (uint32_t)
                flight_custom_version        : Custom version field, commonly the first 8 bytes of the git hash. This is not an unique identifier, but should allow to identify the commit using the main version number even for very large code bases. (uint8_t)
                middleware_custom_version        : Custom version field, commonly the first 8 bytes of the git hash. This is not an unique identifier, but should allow to identify the commit using the main version number even for very large code bases. (uint8_t)
                os_custom_version         : Custom version field, commonly the first 8 bytes of the git hash. This is not an unique identifier, but should allow to identify the commit using the main version number even for very large code bases. (uint8_t)
                vendor_id                 : ID of the board vendor (uint16_t)
                product_id                : ID of the product (uint16_t)
                uid                       : UID if provided by hardware (uint64_t)

*/
mavlink.messages.autopilot_version = function(capabilities, flight_sw_version, middleware_sw_version, os_sw_version, board_version, flight_custom_version, middleware_custom_version, os_custom_version, vendor_id, product_id, uid) {

    this.format = '<QQIIIIHH8s8s8s';
    this.id = mavlink.MAVLINK_MSG_ID_AUTOPILOT_VERSION;
    this.order_map = [0, 2, 3, 4, 5, 8, 9, 10, 6, 7, 1];
    this.crc_extra = 178;
    this.name = 'AUTOPILOT_VERSION';

    this.fieldnames = ['capabilities', 'flight_sw_version', 'middleware_sw_version', 'os_sw_version', 'board_version', 'flight_custom_version', 'middleware_custom_version', 'os_custom_version', 'vendor_id', 'product_id', 'uid'];


    this.set(arguments);

}
        
mavlink.messages.autopilot_version.prototype = new mavlink.message;

mavlink.messages.autopilot_version.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.capabilities, this.uid, this.flight_sw_version, this.middleware_sw_version, this.os_sw_version, this.board_version, this.vendor_id, this.product_id, this.flight_custom_version, this.middleware_custom_version, this.os_custom_version]));
}

/* 
Message implementing parts of the V2 payload specs in V1 frames for
transitional support.

                target_network            : Network ID (0 for broadcast) (uint8_t)
                target_system             : System ID (0 for broadcast) (uint8_t)
                target_component          : Component ID (0 for broadcast) (uint8_t)
                message_type              : A code that identifies the software component that understands this message (analogous to usb device classes or mime type strings).  If this code is less than 32768, it is considered a 'registered' protocol extension and the corresponding entry should be added to https://github.com/mavlink/mavlink/extension-message-ids.xml.  Software creators can register blocks of message IDs as needed (useful for GCS specific metadata, etc...). Message_types greater than 32767 are considered local experiments and should not be checked in to any widely distributed codebase. (uint16_t)
                payload                   : Variable length payload. The length is defined by the remaining message length when subtracting the header and other fields.  The entire content of this block is opaque unless you understand any the encoding message_type.  The particular encoding used can be extension specific and might not always be documented as part of the mavlink specification. (uint8_t)

*/
mavlink.messages.v2_extension = function(target_network, target_system, target_component, message_type, payload) {

    this.format = '<HBBB249s';
    this.id = mavlink.MAVLINK_MSG_ID_V2_EXTENSION;
    this.order_map = [1, 2, 3, 0, 4];
    this.crc_extra = 8;
    this.name = 'V2_EXTENSION';

    this.fieldnames = ['target_network', 'target_system', 'target_component', 'message_type', 'payload'];


    this.set(arguments);

}
        
mavlink.messages.v2_extension.prototype = new mavlink.message;

mavlink.messages.v2_extension.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.message_type, this.target_network, this.target_system, this.target_component, this.payload]));
}

/* 
Send raw controller memory. The use of this message is discouraged for
normal packets, but a quite efficient way for testing new messages and
getting experimental debug output.

                address                   : Starting address of the debug variables (uint16_t)
                ver                       : Version code of the type variable. 0=unknown, type ignored and assumed int16_t. 1=as below (uint8_t)
                type                      : Type code of the memory variables. for ver = 1: 0=16 x int16_t, 1=16 x uint16_t, 2=16 x Q15, 3=16 x 1Q14 (uint8_t)
                value                     : Memory contents at specified address (int8_t)

*/
mavlink.messages.memory_vect = function(address, ver, type, value) {

    this.format = '<HBB32s';
    this.id = mavlink.MAVLINK_MSG_ID_MEMORY_VECT;
    this.order_map = [0, 1, 2, 3];
    this.crc_extra = 204;
    this.name = 'MEMORY_VECT';

    this.fieldnames = ['address', 'ver', 'type', 'value'];


    this.set(arguments);

}
        
mavlink.messages.memory_vect.prototype = new mavlink.message;

mavlink.messages.memory_vect.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.address, this.ver, this.type, this.value]));
}

/* 


                name                      : Name (char)
                time_usec                 : Timestamp (uint64_t)
                x                         : x (float)
                y                         : y (float)
                z                         : z (float)

*/
mavlink.messages.debug_vect = function(name, time_usec, x, y, z) {

    this.format = '<Qfff10s';
    this.id = mavlink.MAVLINK_MSG_ID_DEBUG_VECT;
    this.order_map = [4, 0, 1, 2, 3];
    this.crc_extra = 49;
    this.name = 'DEBUG_VECT';

    this.fieldnames = ['name', 'time_usec', 'x', 'y', 'z'];


    this.set(arguments);

}
        
mavlink.messages.debug_vect.prototype = new mavlink.message;

mavlink.messages.debug_vect.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_usec, this.x, this.y, this.z, this.name]));
}

/* 
Send a key-value pair as float. The use of this message is discouraged
for normal packets, but a quite efficient way for testing new messages
and getting experimental debug output.

                time_boot_ms              : Timestamp (milliseconds since system boot) (uint32_t)
                name                      : Name of the debug variable (char)
                value                     : Floating point value (float)

*/
mavlink.messages.named_value_float = function(time_boot_ms, name, value) {

    this.format = '<If10s';
    this.id = mavlink.MAVLINK_MSG_ID_NAMED_VALUE_FLOAT;
    this.order_map = [0, 2, 1];
    this.crc_extra = 170;
    this.name = 'NAMED_VALUE_FLOAT';

    this.fieldnames = ['time_boot_ms', 'name', 'value'];


    this.set(arguments);

}
        
mavlink.messages.named_value_float.prototype = new mavlink.message;

mavlink.messages.named_value_float.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.value, this.name]));
}

/* 
Send a key-value pair as integer. The use of this message is
discouraged for normal packets, but a quite efficient way for testing
new messages and getting experimental debug output.

                time_boot_ms              : Timestamp (milliseconds since system boot) (uint32_t)
                name                      : Name of the debug variable (char)
                value                     : Signed integer value (int32_t)

*/
mavlink.messages.named_value_int = function(time_boot_ms, name, value) {

    this.format = '<Ii10s';
    this.id = mavlink.MAVLINK_MSG_ID_NAMED_VALUE_INT;
    this.order_map = [0, 2, 1];
    this.crc_extra = 44;
    this.name = 'NAMED_VALUE_INT';

    this.fieldnames = ['time_boot_ms', 'name', 'value'];


    this.set(arguments);

}
        
mavlink.messages.named_value_int.prototype = new mavlink.message;

mavlink.messages.named_value_int.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.value, this.name]));
}

/* 
Status text message. These messages are printed in yellow in the COMM
console of QGroundControl. WARNING: They consume quite some bandwidth,
so use only for important status and error messages. If implemented
wisely, these messages are buffered on the MCU and sent only at a
limited rate (e.g. 10 Hz).

                severity                  : Severity of status. Relies on the definitions within RFC-5424. See enum MAV_SEVERITY. (uint8_t)
                text                      : Status text message, without null termination character (char)

*/
mavlink.messages.statustext = function(severity, text) {

    this.format = '<B50s';
    this.id = mavlink.MAVLINK_MSG_ID_STATUSTEXT;
    this.order_map = [0, 1];
    this.crc_extra = 83;
    this.name = 'STATUSTEXT';

    this.fieldnames = ['severity', 'text'];


    this.set(arguments);

}
        
mavlink.messages.statustext.prototype = new mavlink.message;

mavlink.messages.statustext.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.severity, this.text]));
}

/* 
Send a debug value. The index is used to discriminate between values.
These values show up in the plot of QGroundControl as DEBUG N.

                time_boot_ms              : Timestamp (milliseconds since system boot) (uint32_t)
                ind                       : index of debug variable (uint8_t)
                value                     : DEBUG value (float)

*/
mavlink.messages.debug = function(time_boot_ms, ind, value) {

    this.format = '<IfB';
    this.id = mavlink.MAVLINK_MSG_ID_DEBUG;
    this.order_map = [0, 2, 1];
    this.crc_extra = 46;
    this.name = 'DEBUG';

    this.fieldnames = ['time_boot_ms', 'ind', 'value'];


    this.set(arguments);

}
        
mavlink.messages.debug.prototype = new mavlink.message;

mavlink.messages.debug.prototype.pack = function(mav) {
    return mavlink.message.prototype.pack.call(this, mav, this.crc_extra, jspack.Pack(this.format, [ this.time_boot_ms, this.value, this.ind]));
}



mavlink.map = {
        150: { format: '<fiiffffffhhh', type: mavlink.messages.sensor_offsets, order_map: [9, 10, 11, 0, 1, 2, 3, 4, 5, 6, 7, 8], crc_extra: 134 },
        151: { format: '<hhhBB', type: mavlink.messages.set_mag_offsets, order_map: [3, 4, 0, 1, 2], crc_extra: 219 },
        152: { format: '<HH', type: mavlink.messages.meminfo, order_map: [0, 1], crc_extra: 208 },
        153: { format: '<HHHHHH', type: mavlink.messages.ap_adc, order_map: [0, 1, 2, 3, 4, 5], crc_extra: 188 },
        154: { format: '<fHBBBBBBBBB', type: mavlink.messages.digicam_configure, order_map: [2, 3, 4, 1, 5, 6, 7, 8, 9, 10, 0], crc_extra: 84 },
        155: { format: '<fBBBBbBBBB', type: mavlink.messages.digicam_control, order_map: [1, 2, 3, 4, 5, 6, 7, 8, 9, 0], crc_extra: 22 },
        156: { format: '<BBBBBB', type: mavlink.messages.mount_configure, order_map: [0, 1, 2, 3, 4, 5], crc_extra: 19 },
        157: { format: '<iiiBBB', type: mavlink.messages.mount_control, order_map: [3, 4, 0, 1, 2, 5], crc_extra: 21 },
        158: { format: '<iiiBB', type: mavlink.messages.mount_status, order_map: [3, 4, 0, 1, 2], crc_extra: 134 },
        160: { format: '<ffBBBB', type: mavlink.messages.fence_point, order_map: [2, 3, 4, 5, 0, 1], crc_extra: 78 },
        161: { format: '<BBB', type: mavlink.messages.fence_fetch_point, order_map: [0, 1, 2], crc_extra: 68 },
        162: { format: '<IHBB', type: mavlink.messages.fence_status, order_map: [2, 1, 3, 0], crc_extra: 189 },
        163: { format: '<fffffff', type: mavlink.messages.ahrs, order_map: [0, 1, 2, 3, 4, 5, 6], crc_extra: 127 },
        164: { format: '<fffffffffii', type: mavlink.messages.simstate, order_map: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10], crc_extra: 154 },
        165: { format: '<HB', type: mavlink.messages.hwstatus, order_map: [0, 1], crc_extra: 21 },
        166: { format: '<HHBBBBB', type: mavlink.messages.radio, order_map: [2, 3, 4, 5, 6, 0, 1], crc_extra: 21 },
        167: { format: '<IIIIHBBBB', type: mavlink.messages.limits_status, order_map: [5, 0, 1, 2, 3, 4, 6, 7, 8], crc_extra: 144 },
        168: { format: '<fff', type: mavlink.messages.wind, order_map: [0, 1, 2], crc_extra: 1 },
        169: { format: '<BB16s', type: mavlink.messages.data16, order_map: [0, 1, 2], crc_extra: 234 },
        170: { format: '<BB32s', type: mavlink.messages.data32, order_map: [0, 1, 2], crc_extra: 73 },
        171: { format: '<BB64s', type: mavlink.messages.data64, order_map: [0, 1, 2], crc_extra: 181 },
        172: { format: '<BB96s', type: mavlink.messages.data96, order_map: [0, 1, 2], crc_extra: 22 },
        173: { format: '<ff', type: mavlink.messages.rangefinder, order_map: [0, 1], crc_extra: 83 },
        174: { format: '<ffffffffffff', type: mavlink.messages.airspeed_autocal, order_map: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11], crc_extra: 167 },
        175: { format: '<iihhHBBBBB', type: mavlink.messages.rally_point, order_map: [5, 6, 7, 8, 0, 1, 2, 3, 4, 9], crc_extra: 138 },
        176: { format: '<BBB', type: mavlink.messages.rally_fetch_point, order_map: [0, 1, 2], crc_extra: 234 },
        177: { format: '<ffffHH', type: mavlink.messages.compassmot_status, order_map: [4, 0, 5, 1, 2, 3], crc_extra: 240 },
        178: { format: '<ffffii', type: mavlink.messages.ahrs2, order_map: [0, 1, 2, 3, 4, 5], crc_extra: 47 },
        179: { format: '<QffffHBBB', type: mavlink.messages.camera_status, order_map: [0, 6, 7, 5, 8, 1, 2, 3, 4], crc_extra: 189 },
        180: { format: '<QiiffffffHBBB', type: mavlink.messages.camera_feedback, order_map: [0, 10, 11, 9, 1, 2, 3, 4, 5, 6, 7, 8, 12], crc_extra: 52 },
        181: { format: '<Hh', type: mavlink.messages.battery2, order_map: [0, 1], crc_extra: 174 },
        182: { format: '<ffffiiffff', type: mavlink.messages.ahrs3, order_map: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9], crc_extra: 229 },
        183: { format: '<BB', type: mavlink.messages.autopilot_version_request, order_map: [0, 1], crc_extra: 85 },
        186: { format: '<BBBBB24s', type: mavlink.messages.led_control, order_map: [0, 1, 2, 3, 4, 5], crc_extra: 72 },
        191: { format: '<fffBBBBB10s', type: mavlink.messages.mag_cal_progress, order_map: [3, 4, 5, 6, 7, 8, 0, 1, 2], crc_extra: 92 },
        192: { format: '<ffffffffffBBBB', type: mavlink.messages.mag_cal_report, order_map: [10, 11, 12, 13, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9], crc_extra: 36 },
        193: { format: '<fffffH', type: mavlink.messages.ekf_status_report, order_map: [5, 0, 1, 2, 3, 4], crc_extra: 71 },
        200: { format: '<ffffffffffBB', type: mavlink.messages.gimbal_report, order_map: [10, 11, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9], crc_extra: 134 },
        201: { format: '<fffBB', type: mavlink.messages.gimbal_control, order_map: [3, 4, 0, 1, 2], crc_extra: 205 },
        202: { format: '<BB', type: mavlink.messages.gimbal_reset, order_map: [0, 1], crc_extra: 94 },
        203: { format: '<BBB', type: mavlink.messages.gimbal_axis_calibration_progress, order_map: [0, 1, 2], crc_extra: 128 },
        204: { format: '<BB', type: mavlink.messages.gimbal_set_home_offsets, order_map: [0, 1], crc_extra: 54 },
        205: { format: '<B', type: mavlink.messages.gimbal_home_offset_calibration_result, order_map: [0], crc_extra: 63 },
        206: { format: '<IIIIIIHBBBBBBB', type: mavlink.messages.gimbal_set_factory_parameters, order_map: [7, 8, 0, 1, 2, 6, 9, 10, 11, 12, 13, 3, 4, 5], crc_extra: 112 },
        207: { format: '<B', type: mavlink.messages.gimbal_factory_parameters_loaded, order_map: [0], crc_extra: 201 },
        208: { format: '<IBB', type: mavlink.messages.gimbal_erase_firmware_and_config, order_map: [1, 2, 0], crc_extra: 221 },
        209: { format: '<BB', type: mavlink.messages.gimbal_perform_factory_tests, order_map: [0, 1], crc_extra: 226 },
        210: { format: '<BBBB', type: mavlink.messages.gimbal_report_factory_tests_progress, order_map: [0, 1, 2, 3], crc_extra: 238 },
        215: { format: '<BB', type: mavlink.messages.gopro_power_on, order_map: [0, 1], crc_extra: 241 },
        216: { format: '<BB', type: mavlink.messages.gopro_power_off, order_map: [0, 1], crc_extra: 155 },
        217: { format: '<BBBBB', type: mavlink.messages.gopro_command, order_map: [0, 1, 2, 3, 4], crc_extra: 43 },
        218: { format: '<HBBBB', type: mavlink.messages.gopro_response, order_map: [1, 2, 3, 4, 0], crc_extra: 149 },
        0: { format: '<IBBBBB', type: mavlink.messages.heartbeat, order_map: [1, 2, 3, 0, 4, 5], crc_extra: 50 },
        1: { format: '<IIIHHhHHHHHHb', type: mavlink.messages.sys_status, order_map: [0, 1, 2, 3, 4, 5, 12, 6, 7, 8, 9, 10, 11], crc_extra: 124 },
        2: { format: '<QI', type: mavlink.messages.system_time, order_map: [0, 1], crc_extra: 137 },
        4: { format: '<QIBB', type: mavlink.messages.ping, order_map: [0, 1, 2, 3], crc_extra: 237 },
        5: { format: '<BBB25s', type: mavlink.messages.change_operator_control, order_map: [0, 1, 2, 3], crc_extra: 217 },
        6: { format: '<BBB', type: mavlink.messages.change_operator_control_ack, order_map: [0, 1, 2], crc_extra: 104 },
        7: { format: '<32s', type: mavlink.messages.auth_key, order_map: [0], crc_extra: 119 },
        11: { format: '<IBB', type: mavlink.messages.set_mode, order_map: [1, 2, 0], crc_extra: 89 },
        20: { format: '<hBB16s', type: mavlink.messages.param_request_read, order_map: [1, 2, 3, 0], crc_extra: 214 },
        21: { format: '<BB', type: mavlink.messages.param_request_list, order_map: [0, 1], crc_extra: 159 },
        22: { format: '<fHH16sB', type: mavlink.messages.param_value, order_map: [3, 0, 4, 1, 2], crc_extra: 220 },
        23: { format: '<fBB16sB', type: mavlink.messages.param_set, order_map: [1, 2, 3, 0, 4], crc_extra: 168 },
        24: { format: '<QiiiHHHHBB', type: mavlink.messages.gps_raw_int, order_map: [0, 8, 1, 2, 3, 4, 5, 6, 7, 9], crc_extra: 24 },
        25: { format: '<B20s20s20s20s20s', type: mavlink.messages.gps_status, order_map: [0, 1, 2, 3, 4, 5], crc_extra: 23 },
        26: { format: '<Ihhhhhhhhh', type: mavlink.messages.scaled_imu, order_map: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9], crc_extra: 170 },
        27: { format: '<Qhhhhhhhhh', type: mavlink.messages.raw_imu, order_map: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9], crc_extra: 144 },
        28: { format: '<Qhhhh', type: mavlink.messages.raw_pressure, order_map: [0, 1, 2, 3, 4], crc_extra: 67 },
        29: { format: '<Iffh', type: mavlink.messages.scaled_pressure, order_map: [0, 1, 2, 3], crc_extra: 115 },
        30: { format: '<Iffffff', type: mavlink.messages.attitude, order_map: [0, 1, 2, 3, 4, 5, 6], crc_extra: 39 },
        31: { format: '<Ifffffff', type: mavlink.messages.attitude_quaternion, order_map: [0, 1, 2, 3, 4, 5, 6, 7], crc_extra: 246 },
        32: { format: '<Iffffff', type: mavlink.messages.local_position_ned, order_map: [0, 1, 2, 3, 4, 5, 6], crc_extra: 185 },
        33: { format: '<IiiiihhhH', type: mavlink.messages.global_position_int, order_map: [0, 1, 2, 3, 4, 5, 6, 7, 8], crc_extra: 104 },
        34: { format: '<IhhhhhhhhBB', type: mavlink.messages.rc_channels_scaled, order_map: [0, 9, 1, 2, 3, 4, 5, 6, 7, 8, 10], crc_extra: 237 },
        35: { format: '<IHHHHHHHHBB', type: mavlink.messages.rc_channels_raw, order_map: [0, 9, 1, 2, 3, 4, 5, 6, 7, 8, 10], crc_extra: 244 },
        36: { format: '<IHHHHHHHHB', type: mavlink.messages.servo_output_raw, order_map: [0, 9, 1, 2, 3, 4, 5, 6, 7, 8], crc_extra: 222 },
        37: { format: '<hhBB', type: mavlink.messages.mission_request_partial_list, order_map: [2, 3, 0, 1], crc_extra: 212 },
        38: { format: '<hhBB', type: mavlink.messages.mission_write_partial_list, order_map: [2, 3, 0, 1], crc_extra: 9 },
        39: { format: '<fffffffHHBBBBB', type: mavlink.messages.mission_item, order_map: [9, 10, 7, 11, 8, 12, 13, 0, 1, 2, 3, 4, 5, 6], crc_extra: 254 },
        40: { format: '<HBB', type: mavlink.messages.mission_request, order_map: [1, 2, 0], crc_extra: 230 },
        41: { format: '<HBB', type: mavlink.messages.mission_set_current, order_map: [1, 2, 0], crc_extra: 28 },
        42: { format: '<H', type: mavlink.messages.mission_current, order_map: [0], crc_extra: 28 },
        43: { format: '<BB', type: mavlink.messages.mission_request_list, order_map: [0, 1], crc_extra: 132 },
        44: { format: '<HBB', type: mavlink.messages.mission_count, order_map: [1, 2, 0], crc_extra: 221 },
        45: { format: '<BB', type: mavlink.messages.mission_clear_all, order_map: [0, 1], crc_extra: 232 },
        46: { format: '<H', type: mavlink.messages.mission_item_reached, order_map: [0], crc_extra: 11 },
        47: { format: '<BBB', type: mavlink.messages.mission_ack, order_map: [0, 1, 2], crc_extra: 153 },
        48: { format: '<iiiB', type: mavlink.messages.set_gps_global_origin, order_map: [3, 0, 1, 2], crc_extra: 41 },
        49: { format: '<iii', type: mavlink.messages.gps_global_origin, order_map: [0, 1, 2], crc_extra: 39 },
        50: { format: '<ffffhBB16sB', type: mavlink.messages.param_map_rc, order_map: [5, 6, 7, 4, 8, 0, 1, 2, 3], crc_extra: 78 },
        54: { format: '<ffffffBBB', type: mavlink.messages.safety_set_allowed_area, order_map: [6, 7, 8, 0, 1, 2, 3, 4, 5], crc_extra: 15 },
        55: { format: '<ffffffB', type: mavlink.messages.safety_allowed_area, order_map: [6, 0, 1, 2, 3, 4, 5], crc_extra: 3 },
        61: { format: '<I4ffff9f', type: mavlink.messages.attitude_quaternion_cov, order_map: [0, 1, 2, 3, 4, 5], crc_extra: 153 },
        62: { format: '<fffffhhH', type: mavlink.messages.nav_controller_output, order_map: [0, 1, 5, 6, 7, 2, 3, 4], crc_extra: 183 },
        63: { format: '<QIiiiifff36fB', type: mavlink.messages.global_position_int_cov, order_map: [1, 0, 10, 2, 3, 4, 5, 6, 7, 8, 9], crc_extra: 51 },
        64: { format: '<QIffffff36fB', type: mavlink.messages.local_position_ned_cov, order_map: [1, 0, 9, 2, 3, 4, 5, 6, 7, 8], crc_extra: 82 },
        65: { format: '<IHHHHHHHHHHHHHHHHHHBB', type: mavlink.messages.rc_channels, order_map: [0, 19, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 20], crc_extra: 118 },
        66: { format: '<HBBBB', type: mavlink.messages.request_data_stream, order_map: [1, 2, 3, 0, 4], crc_extra: 148 },
        67: { format: '<HBB', type: mavlink.messages.data_stream, order_map: [1, 0, 2], crc_extra: 21 },
        69: { format: '<hhhhHB', type: mavlink.messages.manual_control, order_map: [5, 0, 1, 2, 3, 4], crc_extra: 243 },
        70: { format: '<HHHHHHHHBB', type: mavlink.messages.rc_channels_override, order_map: [8, 9, 0, 1, 2, 3, 4, 5, 6, 7], crc_extra: 124 },
        73: { format: '<ffffiifHHBBBBB', type: mavlink.messages.mission_item_int, order_map: [9, 10, 7, 11, 8, 12, 13, 0, 1, 2, 3, 4, 5, 6], crc_extra: 38 },
        74: { format: '<ffffhH', type: mavlink.messages.vfr_hud, order_map: [0, 1, 4, 5, 2, 3], crc_extra: 20 },
        75: { format: '<ffffiifHBBBBB', type: mavlink.messages.command_int, order_map: [8, 9, 10, 7, 11, 12, 0, 1, 2, 3, 4, 5, 6], crc_extra: 158 },
        76: { format: '<fffffffHBBB', type: mavlink.messages.command_long, order_map: [8, 9, 7, 10, 0, 1, 2, 3, 4, 5, 6], crc_extra: 152 },
        77: { format: '<HB', type: mavlink.messages.command_ack, order_map: [0, 1], crc_extra: 143 },
        81: { format: '<IffffBB', type: mavlink.messages.manual_setpoint, order_map: [0, 1, 2, 3, 4, 5, 6], crc_extra: 106 },
        82: { format: '<I4fffffBBB', type: mavlink.messages.set_attitude_target, order_map: [0, 6, 7, 8, 1, 2, 3, 4, 5], crc_extra: 49 },
        83: { format: '<I4fffffB', type: mavlink.messages.attitude_target, order_map: [0, 6, 1, 2, 3, 4, 5], crc_extra: 22 },
        84: { format: '<IfffffffffffHBBB', type: mavlink.messages.set_position_target_local_ned, order_map: [0, 13, 14, 15, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11], crc_extra: 143 },
        85: { format: '<IfffffffffffHB', type: mavlink.messages.position_target_local_ned, order_map: [0, 13, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11], crc_extra: 140 },
        86: { format: '<IiifffffffffHBBB', type: mavlink.messages.set_position_target_global_int, order_map: [0, 13, 14, 15, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11], crc_extra: 5 },
        87: { format: '<IiifffffffffHB', type: mavlink.messages.position_target_global_int, order_map: [0, 13, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11], crc_extra: 150 },
        89: { format: '<Iffffff', type: mavlink.messages.local_position_ned_system_global_offset, order_map: [0, 1, 2, 3, 4, 5, 6], crc_extra: 231 },
        90: { format: '<Qffffffiiihhhhhh', type: mavlink.messages.hil_state, order_map: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15], crc_extra: 183 },
        91: { format: '<QffffffffBB', type: mavlink.messages.hil_controls, order_map: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10], crc_extra: 63 },
        92: { format: '<QHHHHHHHHHHHHB', type: mavlink.messages.hil_rc_inputs_raw, order_map: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13], crc_extra: 54 },
        100: { format: '<QfffhhBB', type: mavlink.messages.optical_flow, order_map: [0, 6, 4, 5, 1, 2, 7, 3], crc_extra: 175 },
        101: { format: '<Qffffff', type: mavlink.messages.global_vision_position_estimate, order_map: [0, 1, 2, 3, 4, 5, 6], crc_extra: 102 },
        102: { format: '<Qffffff', type: mavlink.messages.vision_position_estimate, order_map: [0, 1, 2, 3, 4, 5, 6], crc_extra: 158 },
        103: { format: '<Qfff', type: mavlink.messages.vision_speed_estimate, order_map: [0, 1, 2, 3], crc_extra: 208 },
        104: { format: '<Qffffff', type: mavlink.messages.vicon_position_estimate, order_map: [0, 1, 2, 3, 4, 5, 6], crc_extra: 56 },
        105: { format: '<QfffffffffffffH', type: mavlink.messages.highres_imu, order_map: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14], crc_extra: 93 },
        106: { format: '<QIfffffIfhBB', type: mavlink.messages.optical_flow_rad, order_map: [0, 10, 1, 2, 3, 4, 5, 6, 9, 11, 7, 8], crc_extra: 138 },
        107: { format: '<QfffffffffffffI', type: mavlink.messages.hil_sensor, order_map: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14], crc_extra: 108 },
        108: { format: '<fffffffffffffffffffff', type: mavlink.messages.sim_state, order_map: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20], crc_extra: 32 },
        109: { format: '<HHBBBBB', type: mavlink.messages.radio_status, order_map: [2, 3, 4, 5, 6, 0, 1], crc_extra: 185 },
        110: { format: '<BBB251s', type: mavlink.messages.file_transfer_protocol, order_map: [0, 1, 2, 3], crc_extra: 84 },
        111: { format: '<qq', type: mavlink.messages.timesync, order_map: [0, 1], crc_extra: 34 },
        113: { format: '<QiiiHHHhhhHBB', type: mavlink.messages.hil_gps, order_map: [0, 11, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12], crc_extra: 124 },
        114: { format: '<QIfffffIfhBB', type: mavlink.messages.hil_optical_flow, order_map: [0, 10, 1, 2, 3, 4, 5, 6, 9, 11, 7, 8], crc_extra: 237 },
        115: { format: '<Q4ffffiiihhhHHhhh', type: mavlink.messages.hil_state_quaternion, order_map: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15], crc_extra: 4 },
        116: { format: '<Ihhhhhhhhh', type: mavlink.messages.scaled_imu2, order_map: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9], crc_extra: 76 },
        117: { format: '<HHBB', type: mavlink.messages.log_request_list, order_map: [2, 3, 0, 1], crc_extra: 128 },
        118: { format: '<IIHHH', type: mavlink.messages.log_entry, order_map: [2, 3, 4, 0, 1], crc_extra: 56 },
        119: { format: '<IIHBB', type: mavlink.messages.log_request_data, order_map: [3, 4, 2, 0, 1], crc_extra: 116 },
        120: { format: '<IHB90s', type: mavlink.messages.log_data, order_map: [1, 0, 2, 3], crc_extra: 134 },
        121: { format: '<BB', type: mavlink.messages.log_erase, order_map: [0, 1], crc_extra: 237 },
        122: { format: '<BB', type: mavlink.messages.log_request_end, order_map: [0, 1], crc_extra: 203 },
        123: { format: '<BBB110s', type: mavlink.messages.gps_inject_data, order_map: [0, 1, 2, 3], crc_extra: 250 },
        124: { format: '<QiiiIHHHHBBB', type: mavlink.messages.gps2_raw, order_map: [0, 9, 1, 2, 3, 5, 6, 7, 8, 10, 11, 4], crc_extra: 87 },
        125: { format: '<HHH', type: mavlink.messages.power_status, order_map: [0, 1, 2], crc_extra: 203 },
        126: { format: '<IHBBB70s', type: mavlink.messages.serial_control, order_map: [2, 3, 1, 0, 4, 5], crc_extra: 220 },
        127: { format: '<IIiiiIiHBBBBB', type: mavlink.messages.gps_rtk, order_map: [0, 8, 7, 1, 9, 10, 11, 12, 2, 3, 4, 5, 6], crc_extra: 25 },
        128: { format: '<IIiiiIiHBBBBB', type: mavlink.messages.gps2_rtk, order_map: [0, 8, 7, 1, 9, 10, 11, 12, 2, 3, 4, 5, 6], crc_extra: 226 },
        129: { format: '<Ihhhhhhhhh', type: mavlink.messages.scaled_imu3, order_map: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9], crc_extra: 46 },
        130: { format: '<IHHHBBB', type: mavlink.messages.data_transmission_handshake, order_map: [4, 0, 1, 2, 3, 5, 6], crc_extra: 29 },
        131: { format: '<H253s', type: mavlink.messages.encapsulated_data, order_map: [0, 1], crc_extra: 223 },
        132: { format: '<IHHHBBBB', type: mavlink.messages.distance_sensor, order_map: [0, 1, 2, 3, 4, 5, 6, 7], crc_extra: 85 },
        133: { format: '<QiiH', type: mavlink.messages.terrain_request, order_map: [1, 2, 3, 0], crc_extra: 6 },
        134: { format: '<iiH16hB', type: mavlink.messages.terrain_data, order_map: [0, 1, 2, 4, 3], crc_extra: 229 },
        135: { format: '<ii', type: mavlink.messages.terrain_check, order_map: [0, 1], crc_extra: 203 },
        136: { format: '<iiffHHH', type: mavlink.messages.terrain_report, order_map: [0, 1, 4, 2, 3, 5, 6], crc_extra: 1 },
        137: { format: '<Iffh', type: mavlink.messages.scaled_pressure2, order_map: [0, 1, 2, 3], crc_extra: 195 },
        138: { format: '<Q4ffff', type: mavlink.messages.att_pos_mocap, order_map: [0, 1, 2, 3, 4], crc_extra: 109 },
        139: { format: '<Q8fBBB', type: mavlink.messages.set_actuator_control_target, order_map: [0, 2, 3, 4, 1], crc_extra: 168 },
        140: { format: '<Q8fB', type: mavlink.messages.actuator_control_target, order_map: [0, 2, 1], crc_extra: 181 },
        147: { format: '<iih10HhBBBb', type: mavlink.messages.battery_status, order_map: [5, 6, 7, 2, 3, 4, 0, 1, 8], crc_extra: 154 },
        148: { format: '<QQIIIIHH8s8s8s', type: mavlink.messages.autopilot_version, order_map: [0, 2, 3, 4, 5, 8, 9, 10, 6, 7, 1], crc_extra: 178 },
        248: { format: '<HBBB249s', type: mavlink.messages.v2_extension, order_map: [1, 2, 3, 0, 4], crc_extra: 8 },
        249: { format: '<HBB32s', type: mavlink.messages.memory_vect, order_map: [0, 1, 2, 3], crc_extra: 204 },
        250: { format: '<Qfff10s', type: mavlink.messages.debug_vect, order_map: [4, 0, 1, 2, 3], crc_extra: 49 },
        251: { format: '<If10s', type: mavlink.messages.named_value_float, order_map: [0, 2, 1], crc_extra: 170 },
        252: { format: '<Ii10s', type: mavlink.messages.named_value_int, order_map: [0, 2, 1], crc_extra: 44 },
        253: { format: '<B50s', type: mavlink.messages.statustext, order_map: [0, 1], crc_extra: 83 },
        254: { format: '<IfB', type: mavlink.messages.debug, order_map: [0, 2, 1], crc_extra: 46 },
}


// Special mavlink message to capture malformed data packets for debugging
mavlink.messages.bad_data = function(data, reason) {
    this.id = mavlink.MAVLINK_MSG_ID_BAD_DATA;
    this.data = data;
    this.reason = reason;
    this.msgbuf = data;
}

/* MAVLink protocol handling class */
MAVLink = function(logger, srcSystem, srcComponent) {

    this.logger = logger;

    this.seq = 0;
    this.buf = new Buffer(0);
    this.bufInError = new Buffer(0);
   
    this.srcSystem = (typeof srcSystem === 'undefined') ? 0 : srcSystem;
    this.srcComponent =  (typeof srcComponent === 'undefined') ? 0 : srcComponent;

    // The first packet we expect is a valid header, 6 bytes.
    this.expected_length = 6;

    this.have_prefix_error = false;

    this.protocol_marker = 254;
    this.little_endian = true;

    this.crc_extra = true;
    this.sort_fields = true;
    this.total_packets_sent = 0;
    this.total_bytes_sent = 0;
    this.total_packets_received = 0;
    this.total_bytes_received = 0;
    this.total_receive_errors = 0;
    this.startup_time = Date.now();
    
}

// Implements EventEmitter
util.inherits(MAVLink, events.EventEmitter);

// If the logger exists, this function will add a message to it.
// Assumes the logger is a winston object.
MAVLink.prototype.log = function(message) {
    if(this.logger) {
        this.logger.info(message);
    }
}

MAVLink.prototype.log = function(level, message) {
    if(this.logger) {
        this.logger.log(level, message);
    }
}

MAVLink.prototype.send = function(mavmsg) {
    buf = mavmsg.pack(this);
    this.file.write(buf);
    this.seq = (this.seq + 1) % 256;
    this.total_packets_sent +=1;
    this.total_bytes_sent += buf.length;
}

// return number of bytes needed for next parsing stage
MAVLink.prototype.bytes_needed = function() {
    ret = this.expected_length - this.buf.length;
    return ( ret <= 0 ) ? 1 : ret;
}

// add data to the local buffer
MAVLink.prototype.pushBuffer = function(data) {
    if(data) {
        this.buf = Buffer.concat([this.buf, data]);
        this.total_bytes_received += data.length;
    }
}

// Decode prefix.  Elides the prefix.
MAVLink.prototype.parsePrefix = function() {

    // Test for a message prefix.
    if( this.buf.length >= 1 && this.buf[0] != 254 ) {

        // Strip the offending initial byte and throw an error.
        var badPrefix = this.buf[0];
        this.bufInError = this.buf.slice(0,1);
        this.buf = this.buf.slice(1);
        this.expected_length = 6;

        // TODO: enable subsequent prefix error suppression if robust_parsing is implemented
        //if(!this.have_prefix_error) {
        //    this.have_prefix_error = true;
            throw new Error("Bad prefix ("+badPrefix+")");
        //}

    }
    //else if( this.buf.length >= 1 && this.buf[0] == 254 ) {
    //    this.have_prefix_error = false;
    //}

}

// Determine the length.  Leaves buffer untouched.
MAVLink.prototype.parseLength = function() {
    
    if( this.buf.length >= 2 ) {
        var unpacked = jspack.Unpack('BB', this.buf.slice(0, 2));
        this.expected_length = unpacked[1] + 8; // length of message + header + CRC
    }

}

// input some data bytes, possibly returning a new message
MAVLink.prototype.parseChar = function(c) {

    var m = null;

    try {

        this.pushBuffer(c);
        this.parsePrefix();
        this.parseLength();
        m = this.parsePayload();

    } catch(e) {

        this.log('error', e.message);
        this.total_receive_errors += 1;
        m = new mavlink.messages.bad_data(this.bufInError, e.message);
        this.bufInError = new Buffer(0);
        
    }

    if(null != m) {
        this.emit(m.name, m);
        this.emit('message', m);
    }

    return m;

}

MAVLink.prototype.parsePayload = function() {

    var m = null;

    // If we have enough bytes to try and read it, read it.
    if( this.expected_length >= 8 && this.buf.length >= this.expected_length ) {

        // Slice off the expected packet length, reset expectation to be to find a header.
        var mbuf = this.buf.slice(0, this.expected_length);
        // TODO: slicing off the buffer should depend on the error produced by the decode() function
        // - if a message we find a well formed message, cut-off the expected_length
        // - if the message is not well formed (correct prefix by accident), cut-off 1 char only
        this.buf = this.buf.slice(this.expected_length);
        this.expected_length = 6;

        // w.info("Attempting to parse packet, message candidate buffer is ["+mbuf.toByteArray()+"]");

        try {
            m = this.decode(mbuf);
            this.total_packets_received += 1;
        }
        catch(e) {
            // Set buffer in question and re-throw to generic error handling
            this.bufInError = mbuf;
            throw e;
        }
    }

    return m;

}

// input some data bytes, possibly returning an array of new messages
MAVLink.prototype.parseBuffer = function(s) {
    
    // Get a message, if one is available in the stream.
    var m = this.parseChar(s);

    // No messages available, bail.
    if ( null === m ) {
        return null;
    }
    
    // While more valid messages can be read from the existing buffer, add
    // them to the array of new messages and return them.
    var ret = [m];
    while(true) {
        m = this.parseChar();
        if ( null === m ) {
            // No more messages left.
            return ret;
        }
        ret.push(m);
    }
    return ret;

}

/* decode a buffer as a MAVLink message */
MAVLink.prototype.decode = function(msgbuf) {

    var magic, mlen, seq, srcSystem, srcComponent, unpacked, msgId;

    // decode the header
    try {
        unpacked = jspack.Unpack('cBBBBB', msgbuf.slice(0, 6));
        magic = unpacked[0];
        mlen = unpacked[1];
        seq = unpacked[2];
        srcSystem = unpacked[3];
        srcComponent = unpacked[4];
        msgId = unpacked[5];
    }
    catch(e) {
        throw new Error('Unable to unpack MAVLink header: ' + e.message);
    }

    if (magic.charCodeAt(0) != 254) {
        throw new Error("Invalid MAVLink prefix ("+magic.charCodeAt(0)+")");
    }

    if( mlen != msgbuf.length - 8 ) {
        throw new Error("Invalid MAVLink message length.  Got " + (msgbuf.length - 8) + " expected " + mlen + ", msgId=" + msgId);
    }

    if( false === _.has(mavlink.map, msgId) ) {
        throw new Error("Unknown MAVLink message ID (" + msgId + ")");
    }

    // decode the payload
    // refs: (fmt, type, order_map, crc_extra) = mavlink.map[msgId]
    var decoder = mavlink.map[msgId];

    // decode the checksum
    try {
        var receivedChecksum = jspack.Unpack('<H', msgbuf.slice(msgbuf.length - 2));
    } catch (e) {
        throw new Error("Unable to unpack MAVLink CRC: " + e.message);
    }

    var messageChecksum = mavlink.x25Crc(msgbuf.slice(1, msgbuf.length - 2));

    // Assuming using crc_extra = True.  See the message.prototype.pack() function.
    messageChecksum = mavlink.x25Crc([decoder.crc_extra], messageChecksum);
    
    if ( receivedChecksum != messageChecksum ) {
        throw new Error('invalid MAVLink CRC in msgID ' +msgId+ ', got 0x' + receivedChecksum + ' checksum, calculated payload checkum as 0x'+messageChecksum );
    }

    // Decode the payload and reorder the fields to match the order map.
    try {
        var t = jspack.Unpack(decoder.format, msgbuf.slice(6, msgbuf.length));
    }
    catch (e) {
        throw new Error('Unable to unpack MAVLink payload type='+decoder.type+' format='+decoder.format+' payloadLength='+ msgbuf.slice(6, -2).length +': '+ e.message);
    }

    // Reorder the fields to match the order map
    var args = [];
    _.each(t, function(e, i, l) {
        args[i] = t[decoder.order_map[i]]
    });

    // construct the message object
    try {
        var m = new decoder.type(args);
        m.set.call(m, args);
    }
    catch (e) {
        throw new Error('Unable to instantiate MAVLink message of type '+decoder.type+' : ' + e.message);
    }
    m.msgbuf = msgbuf;
    m.payload = msgbuf.slice(6);
    m.crc = receivedChecksum;
    m.header = new mavlink.header(msgId, mlen, seq, srcSystem, srcComponent);
    this.log(m);
    return m;
}


// Expose this code as a module
module.exports = mavlink;


},{"buffer":2,"events":3,"jspack":10,"underscore":11,"util":8}],10:[function(require,module,exports){
/*!
 *  Copyright © 2008 Fair Oaks Labs, Inc.
 *  All rights reserved.
 */

// Utility object:  Encode/Decode C-style binary primitives to/from octet arrays
function JSPack()
{
	// Module-level (private) variables
	var el,  bBE = false, m = this;


	// Raw byte arrays
	m._DeArray = function (a, p, l)
	{
		return [a.slice(p,p+l)];
	};
	m._EnArray = function (a, p, l, v)
	{
		for (var i = 0; i < l; a[p+i] = v[i]?v[i]:0, i++);
	};

	// ASCII characters
	m._DeChar = function (a, p)
	{
		return String.fromCharCode(a[p]);
	};
	m._EnChar = function (a, p, v)
	{
		a[p] = v.charCodeAt(0);
	};

	// Little-endian (un)signed N-byte integers
	m._DeInt = function (a, p)
	{
		var lsb = bBE?(el.len-1):0, nsb = bBE?-1:1, stop = lsb+nsb*el.len, rv, i, f;
		for (rv = 0, i = lsb, f = 1; i != stop; rv+=(a[p+i]*f), i+=nsb, f*=256);
		if (el.bSigned && (rv & Math.pow(2, el.len*8-1))) { rv -= Math.pow(2, el.len*8); }
		return rv;
	};
	m._EnInt = function (a, p, v)
	{
		var lsb = bBE?(el.len-1):0, nsb = bBE?-1:1, stop = lsb+nsb*el.len, i;
		v = (v<el.min)?el.min:(v>el.max)?el.max:v;
		for (i = lsb; i != stop; a[p+i]=v&0xff, i+=nsb, v>>=8);
	};

	// ASCII character strings
	m._DeString = function (a, p, l)
	{
		for (var rv = new Array(l), i = 0; i < l; rv[i] = String.fromCharCode(a[p+i]), i++);
		return rv.join('');
	};
	m._EnString = function (a, p, l, v)
	{
		for (var t, i = 0; i < l; a[p+i] = (t=v.charCodeAt(i))?t:0, i++);
	};

	// Little-endian N-bit IEEE 754 floating point
	m._De754 = function (a, p)
	{
		var s, e, m, i, d, nBits, mLen, eLen, eBias, eMax;
		mLen = el.mLen, eLen = el.len*8-el.mLen-1, eMax = (1<<eLen)-1, eBias = eMax>>1;

		i = bBE?0:(el.len-1); d = bBE?1:-1; s = a[p+i]; i+=d; nBits = -7;
		for (e = s&((1<<(-nBits))-1), s>>=(-nBits), nBits += eLen; nBits > 0; e=e*256+a[p+i], i+=d, nBits-=8);
		for (m = e&((1<<(-nBits))-1), e>>=(-nBits), nBits += mLen; nBits > 0; m=m*256+a[p+i], i+=d, nBits-=8);

		switch (e)
		{
			case 0:
				// Zero, or denormalized number
				e = 1-eBias;
				break;
			case eMax:
				// NaN, or +/-Infinity
				return m?NaN:((s?-1:1)*Infinity);
			default:
				// Normalized number
				m = m + Math.pow(2, mLen);
				e = e - eBias;
				break;
		}
		return (s?-1:1) * m * Math.pow(2, e-mLen);
	};
	m._En754 = function (a, p, v)
	{
		var s, e, m, i, d, c, mLen, eLen, eBias, eMax;
		mLen = el.mLen, eLen = el.len*8-el.mLen-1, eMax = (1<<eLen)-1, eBias = eMax>>1;

		s = v<0?1:0;
		v = Math.abs(v);
		if (isNaN(v) || (v == Infinity))
		{
			m = isNaN(v)?1:0;
			e = eMax;
		}
		else
		{
			e = Math.floor(Math.log(v)/Math.LN2);			// Calculate log2 of the value
			if (v*(c = Math.pow(2, -e)) < 1) { e--; c*=2; }		// Math.log() isn't 100% reliable

			// Round by adding 1/2 the significand's LSD
			if (e+eBias >= 1) { v += el.rt/c; }			// Normalized:  mLen significand digits
			else { v += el.rt*Math.pow(2, 1-eBias); } 		// Denormalized:  <= mLen significand digits
			if (v*c >= 2) { e++; c/=2; }				// Rounding can increment the exponent

			if (e+eBias >= eMax)
			{
				// Overflow
				m = 0;
				e = eMax;
			}
			else if (e+eBias >= 1)
			{
				// Normalized - term order matters, as Math.pow(2, 52-e) and v*Math.pow(2, 52) can overflow
				m = (v*c-1)*Math.pow(2, mLen);
				e = e + eBias;
			}
			else
			{
				// Denormalized - also catches the '0' case, somewhat by chance
				m = v*Math.pow(2, eBias-1)*Math.pow(2, mLen);
				e = 0;
			}
		}

		for (i = bBE?(el.len-1):0, d=bBE?-1:1; mLen >= 8; a[p+i]=m&0xff, i+=d, m/=256, mLen-=8);
		for (e=(e<<mLen)|m, eLen+=mLen; eLen > 0; a[p+i]=e&0xff, i+=d, e/=256, eLen-=8);
		a[p+i-d] |= s*128;
	};

	// Convert int64 to array with 3 elements: [lowBits, highBits, unsignedFlag]
	// '>>>' trick to convert signed 32bit int to unsigned int (because << always results in a signed 32bit int)
	m._DeInt64 = function (a, p) {
		var start = bBE ? 0 : 7, nsb = bBE ? 1 : -1, stop = start + nsb * 8, rv = [0,0, !el.bSigned], i, f, rvi;
		for (i = start, rvi = 1, f = 0;
			i != stop;
			rv[rvi] = (((rv[rvi]<<8)>>>0) + a[p + i]), i += nsb, f++, rvi = (f < 4 ? 1 : 0));
		return rv;
	};
	m._EnInt64 = function (a, p, v) {
		var start = bBE ? 0 : 7, nsb = bBE ? 1 : -1, stop = start + nsb * 8, i, f, rvi, s;
		for (i = start, rvi = 1, f = 0, s = 24;
			i != stop;
			a[p + i] = v[rvi]>>s & 0xff, i += nsb, f++, rvi = (f < 4 ? 1 : 0), s = 24 - (8 * (f % 4)));
	};
	

	// Class data
	m._sPattern	= '(\\d+)?([AxcbBhHsfdiIlLqQ])';
	m._lenLut	= {'A':1, 'x':1, 'c':1, 'b':1, 'B':1, 'h':2, 'H':2, 's':1, 'f':4, 'd':8, 'i':4, 'I':4, 'l':4, 'L':4, 'q':8, 'Q':8};
	m._elLut	= {	'A': {en:m._EnArray, de:m._DeArray},
				's': {en:m._EnString, de:m._DeString},
				'c': {en:m._EnChar, de:m._DeChar},
				'b': {en:m._EnInt, de:m._DeInt, len:1, bSigned:true, min:-Math.pow(2, 7), max:Math.pow(2, 7)-1},
				'B': {en:m._EnInt, de:m._DeInt, len:1, bSigned:false, min:0, max:Math.pow(2, 8)-1},
				'h': {en:m._EnInt, de:m._DeInt, len:2, bSigned:true, min:-Math.pow(2, 15), max:Math.pow(2, 15)-1},
				'H': {en:m._EnInt, de:m._DeInt, len:2, bSigned:false, min:0, max:Math.pow(2, 16)-1},
				'i': {en:m._EnInt, de:m._DeInt, len:4, bSigned:true, min:-Math.pow(2, 31), max:Math.pow(2, 31)-1},
				'I': {en:m._EnInt, de:m._DeInt, len:4, bSigned:false, min:0, max:Math.pow(2, 32)-1},
				'l': {en:m._EnInt, de:m._DeInt, len:4, bSigned:true, min:-Math.pow(2, 31), max:Math.pow(2, 31)-1},
				'L': {en:m._EnInt, de:m._DeInt, len:4, bSigned:false, min:0, max:Math.pow(2, 32)-1},
				'f': {en:m._En754, de:m._De754, len:4, mLen:23, rt:Math.pow(2, -24)-Math.pow(2, -77)},
				'd': {en:m._En754, de:m._De754, len:8, mLen:52, rt:0},
				'q': {en:m._EnInt64, de:m._DeInt64, bSigned:true},
				'Q': {en:m._EnInt64, de:m._DeInt64, bSigned:false}};

	// Unpack a series of n elements of size s from array a at offset p with fxn
	m._UnpackSeries = function (n, s, a, p)
	{
		for (var fxn = el.de, rv = [], i = 0; i < n; rv.push(fxn(a, p+i*s)), i++);
		return rv;
	};

	// Pack a series of n elements of size s from array v at offset i to array a at offset p with fxn
	m._PackSeries = function (n, s, a, p, v, i)
	{
		for (var fxn = el.en, o = 0; o < n; fxn(a, p+o*s, v[i+o]), o++);
	};

	// Unpack the octet array a, beginning at offset p, according to the fmt string
	m.Unpack = function (fmt, a, p)
	{
		// Set the private bBE flag based on the format string - assume big-endianness
		bBE = (fmt.charAt(0) != '<');

		p = p?p:0;
		var re = new RegExp(this._sPattern, 'g'), m, n, s, rv = [];
		while (m = re.exec(fmt))
		{
			n = ((m[1]==undefined)||(m[1]==''))?1:parseInt(m[1]);
			s = this._lenLut[m[2]];
			if ((p + n*s) > a.length)
			{
				return undefined;
			}
			switch (m[2])
			{
				case 'A': case 's':
					rv.push(this._elLut[m[2]].de(a, p, n));
					break;
				case 'c': case 'b': case 'B': case 'h': case 'H':
				case 'i': case 'I': case 'l': case 'L': case 'f': case 'd': case 'q': case 'Q':
					el = this._elLut[m[2]];
					rv.push(this._UnpackSeries(n, s, a, p));
					break;
			}
			p += n*s;
		}
		return Array.prototype.concat.apply([], rv);
	};

	// Pack the supplied values into the octet array a, beginning at offset p, according to the fmt string
	m.PackTo = function (fmt, a, p, values)
	{
		// Set the private bBE flag based on the format string - assume big-endianness
		bBE = (fmt.charAt(0) != '<');

		var re = new RegExp(this._sPattern, 'g'), m, n, s, i = 0, j;
		while (m = re.exec(fmt))
		{
			n = ((m[1]==undefined)||(m[1]==''))?1:parseInt(m[1]);
			s = this._lenLut[m[2]];
			if ((p + n*s) > a.length)
			{
				return false;
			}
			switch (m[2])
			{
				case 'A': case 's':
					if ((i + 1) > values.length) { return false; }
					this._elLut[m[2]].en(a, p, n, values[i]);
					i += 1;
					break;
				case 'c': case 'b': case 'B': case 'h': case 'H':
				case 'i': case 'I': case 'l': case 'L': case 'f': case 'd': case 'q': case 'Q':
					el = this._elLut[m[2]];
					if ((i + n) > values.length) { return false; }
					this._PackSeries(n, s, a, p, values, i);
					i += n;
					break;
				case 'x':
					for (j = 0; j < n; j++) { a[p+j] = 0; }
					break;
			}
			p += n*s;
		}
		return a;
	};

	// Pack the supplied values into a new octet array, according to the fmt string
	m.Pack = function (fmt, values)
	{
		return this.PackTo(fmt, new Array(this.CalcLength(fmt)), 0, values);
	};

	// Determine the number of bytes represented by the format string
	m.CalcLength = function (fmt)
	{
		var re = new RegExp(this._sPattern, 'g'), m, sum = 0;
		while (m = re.exec(fmt))
		{
			sum += (((m[1]==undefined)||(m[1]==''))?1:parseInt(m[1])) * this._lenLut[m[2]];
		}
		return sum;
	};
};

exports.jspack = new JSPack();

},{}],11:[function(require,module,exports){
//     Underscore.js 1.8.3
//     http://underscorejs.org
//     (c) 2009-2015 Jeremy Ashkenas, DocumentCloud and Investigative Reporters & Editors
//     Underscore may be freely distributed under the MIT license.

(function() {

  // Baseline setup
  // --------------

  // Establish the root object, `window` in the browser, or `exports` on the server.
  var root = this;

  // Save the previous value of the `_` variable.
  var previousUnderscore = root._;

  // Save bytes in the minified (but not gzipped) version:
  var ArrayProto = Array.prototype, ObjProto = Object.prototype, FuncProto = Function.prototype;

  // Create quick reference variables for speed access to core prototypes.
  var
    push             = ArrayProto.push,
    slice            = ArrayProto.slice,
    toString         = ObjProto.toString,
    hasOwnProperty   = ObjProto.hasOwnProperty;

  // All **ECMAScript 5** native function implementations that we hope to use
  // are declared here.
  var
    nativeIsArray      = Array.isArray,
    nativeKeys         = Object.keys,
    nativeBind         = FuncProto.bind,
    nativeCreate       = Object.create;

  // Naked function reference for surrogate-prototype-swapping.
  var Ctor = function(){};

  // Create a safe reference to the Underscore object for use below.
  var _ = function(obj) {
    if (obj instanceof _) return obj;
    if (!(this instanceof _)) return new _(obj);
    this._wrapped = obj;
  };

  // Export the Underscore object for **Node.js**, with
  // backwards-compatibility for the old `require()` API. If we're in
  // the browser, add `_` as a global object.
  if (typeof exports !== 'undefined') {
    if (typeof module !== 'undefined' && module.exports) {
      exports = module.exports = _;
    }
    exports._ = _;
  } else {
    root._ = _;
  }

  // Current version.
  _.VERSION = '1.8.3';

  // Internal function that returns an efficient (for current engines) version
  // of the passed-in callback, to be repeatedly applied in other Underscore
  // functions.
  var optimizeCb = function(func, context, argCount) {
    if (context === void 0) return func;
    switch (argCount == null ? 3 : argCount) {
      case 1: return function(value) {
        return func.call(context, value);
      };
      case 2: return function(value, other) {
        return func.call(context, value, other);
      };
      case 3: return function(value, index, collection) {
        return func.call(context, value, index, collection);
      };
      case 4: return function(accumulator, value, index, collection) {
        return func.call(context, accumulator, value, index, collection);
      };
    }
    return function() {
      return func.apply(context, arguments);
    };
  };

  // A mostly-internal function to generate callbacks that can be applied
  // to each element in a collection, returning the desired result — either
  // identity, an arbitrary callback, a property matcher, or a property accessor.
  var cb = function(value, context, argCount) {
    if (value == null) return _.identity;
    if (_.isFunction(value)) return optimizeCb(value, context, argCount);
    if (_.isObject(value)) return _.matcher(value);
    return _.property(value);
  };
  _.iteratee = function(value, context) {
    return cb(value, context, Infinity);
  };

  // An internal function for creating assigner functions.
  var createAssigner = function(keysFunc, undefinedOnly) {
    return function(obj) {
      var length = arguments.length;
      if (length < 2 || obj == null) return obj;
      for (var index = 1; index < length; index++) {
        var source = arguments[index],
            keys = keysFunc(source),
            l = keys.length;
        for (var i = 0; i < l; i++) {
          var key = keys[i];
          if (!undefinedOnly || obj[key] === void 0) obj[key] = source[key];
        }
      }
      return obj;
    };
  };

  // An internal function for creating a new object that inherits from another.
  var baseCreate = function(prototype) {
    if (!_.isObject(prototype)) return {};
    if (nativeCreate) return nativeCreate(prototype);
    Ctor.prototype = prototype;
    var result = new Ctor;
    Ctor.prototype = null;
    return result;
  };

  var property = function(key) {
    return function(obj) {
      return obj == null ? void 0 : obj[key];
    };
  };

  // Helper for collection methods to determine whether a collection
  // should be iterated as an array or as an object
  // Related: http://people.mozilla.org/~jorendorff/es6-draft.html#sec-tolength
  // Avoids a very nasty iOS 8 JIT bug on ARM-64. #2094
  var MAX_ARRAY_INDEX = Math.pow(2, 53) - 1;
  var getLength = property('length');
  var isArrayLike = function(collection) {
    var length = getLength(collection);
    return typeof length == 'number' && length >= 0 && length <= MAX_ARRAY_INDEX;
  };

  // Collection Functions
  // --------------------

  // The cornerstone, an `each` implementation, aka `forEach`.
  // Handles raw objects in addition to array-likes. Treats all
  // sparse array-likes as if they were dense.
  _.each = _.forEach = function(obj, iteratee, context) {
    iteratee = optimizeCb(iteratee, context);
    var i, length;
    if (isArrayLike(obj)) {
      for (i = 0, length = obj.length; i < length; i++) {
        iteratee(obj[i], i, obj);
      }
    } else {
      var keys = _.keys(obj);
      for (i = 0, length = keys.length; i < length; i++) {
        iteratee(obj[keys[i]], keys[i], obj);
      }
    }
    return obj;
  };

  // Return the results of applying the iteratee to each element.
  _.map = _.collect = function(obj, iteratee, context) {
    iteratee = cb(iteratee, context);
    var keys = !isArrayLike(obj) && _.keys(obj),
        length = (keys || obj).length,
        results = Array(length);
    for (var index = 0; index < length; index++) {
      var currentKey = keys ? keys[index] : index;
      results[index] = iteratee(obj[currentKey], currentKey, obj);
    }
    return results;
  };

  // Create a reducing function iterating left or right.
  function createReduce(dir) {
    // Optimized iterator function as using arguments.length
    // in the main function will deoptimize the, see #1991.
    function iterator(obj, iteratee, memo, keys, index, length) {
      for (; index >= 0 && index < length; index += dir) {
        var currentKey = keys ? keys[index] : index;
        memo = iteratee(memo, obj[currentKey], currentKey, obj);
      }
      return memo;
    }

    return function(obj, iteratee, memo, context) {
      iteratee = optimizeCb(iteratee, context, 4);
      var keys = !isArrayLike(obj) && _.keys(obj),
          length = (keys || obj).length,
          index = dir > 0 ? 0 : length - 1;
      // Determine the initial value if none is provided.
      if (arguments.length < 3) {
        memo = obj[keys ? keys[index] : index];
        index += dir;
      }
      return iterator(obj, iteratee, memo, keys, index, length);
    };
  }

  // **Reduce** builds up a single result from a list of values, aka `inject`,
  // or `foldl`.
  _.reduce = _.foldl = _.inject = createReduce(1);

  // The right-associative version of reduce, also known as `foldr`.
  _.reduceRight = _.foldr = createReduce(-1);

  // Return the first value which passes a truth test. Aliased as `detect`.
  _.find = _.detect = function(obj, predicate, context) {
    var key;
    if (isArrayLike(obj)) {
      key = _.findIndex(obj, predicate, context);
    } else {
      key = _.findKey(obj, predicate, context);
    }
    if (key !== void 0 && key !== -1) return obj[key];
  };

  // Return all the elements that pass a truth test.
  // Aliased as `select`.
  _.filter = _.select = function(obj, predicate, context) {
    var results = [];
    predicate = cb(predicate, context);
    _.each(obj, function(value, index, list) {
      if (predicate(value, index, list)) results.push(value);
    });
    return results;
  };

  // Return all the elements for which a truth test fails.
  _.reject = function(obj, predicate, context) {
    return _.filter(obj, _.negate(cb(predicate)), context);
  };

  // Determine whether all of the elements match a truth test.
  // Aliased as `all`.
  _.every = _.all = function(obj, predicate, context) {
    predicate = cb(predicate, context);
    var keys = !isArrayLike(obj) && _.keys(obj),
        length = (keys || obj).length;
    for (var index = 0; index < length; index++) {
      var currentKey = keys ? keys[index] : index;
      if (!predicate(obj[currentKey], currentKey, obj)) return false;
    }
    return true;
  };

  // Determine if at least one element in the object matches a truth test.
  // Aliased as `any`.
  _.some = _.any = function(obj, predicate, context) {
    predicate = cb(predicate, context);
    var keys = !isArrayLike(obj) && _.keys(obj),
        length = (keys || obj).length;
    for (var index = 0; index < length; index++) {
      var currentKey = keys ? keys[index] : index;
      if (predicate(obj[currentKey], currentKey, obj)) return true;
    }
    return false;
  };

  // Determine if the array or object contains a given item (using `===`).
  // Aliased as `includes` and `include`.
  _.contains = _.includes = _.include = function(obj, item, fromIndex, guard) {
    if (!isArrayLike(obj)) obj = _.values(obj);
    if (typeof fromIndex != 'number' || guard) fromIndex = 0;
    return _.indexOf(obj, item, fromIndex) >= 0;
  };

  // Invoke a method (with arguments) on every item in a collection.
  _.invoke = function(obj, method) {
    var args = slice.call(arguments, 2);
    var isFunc = _.isFunction(method);
    return _.map(obj, function(value) {
      var func = isFunc ? method : value[method];
      return func == null ? func : func.apply(value, args);
    });
  };

  // Convenience version of a common use case of `map`: fetching a property.
  _.pluck = function(obj, key) {
    return _.map(obj, _.property(key));
  };

  // Convenience version of a common use case of `filter`: selecting only objects
  // containing specific `key:value` pairs.
  _.where = function(obj, attrs) {
    return _.filter(obj, _.matcher(attrs));
  };

  // Convenience version of a common use case of `find`: getting the first object
  // containing specific `key:value` pairs.
  _.findWhere = function(obj, attrs) {
    return _.find(obj, _.matcher(attrs));
  };

  // Return the maximum element (or element-based computation).
  _.max = function(obj, iteratee, context) {
    var result = -Infinity, lastComputed = -Infinity,
        value, computed;
    if (iteratee == null && obj != null) {
      obj = isArrayLike(obj) ? obj : _.values(obj);
      for (var i = 0, length = obj.length; i < length; i++) {
        value = obj[i];
        if (value > result) {
          result = value;
        }
      }
    } else {
      iteratee = cb(iteratee, context);
      _.each(obj, function(value, index, list) {
        computed = iteratee(value, index, list);
        if (computed > lastComputed || computed === -Infinity && result === -Infinity) {
          result = value;
          lastComputed = computed;
        }
      });
    }
    return result;
  };

  // Return the minimum element (or element-based computation).
  _.min = function(obj, iteratee, context) {
    var result = Infinity, lastComputed = Infinity,
        value, computed;
    if (iteratee == null && obj != null) {
      obj = isArrayLike(obj) ? obj : _.values(obj);
      for (var i = 0, length = obj.length; i < length; i++) {
        value = obj[i];
        if (value < result) {
          result = value;
        }
      }
    } else {
      iteratee = cb(iteratee, context);
      _.each(obj, function(value, index, list) {
        computed = iteratee(value, index, list);
        if (computed < lastComputed || computed === Infinity && result === Infinity) {
          result = value;
          lastComputed = computed;
        }
      });
    }
    return result;
  };

  // Shuffle a collection, using the modern version of the
  // [Fisher-Yates shuffle](http://en.wikipedia.org/wiki/Fisher–Yates_shuffle).
  _.shuffle = function(obj) {
    var set = isArrayLike(obj) ? obj : _.values(obj);
    var length = set.length;
    var shuffled = Array(length);
    for (var index = 0, rand; index < length; index++) {
      rand = _.random(0, index);
      if (rand !== index) shuffled[index] = shuffled[rand];
      shuffled[rand] = set[index];
    }
    return shuffled;
  };

  // Sample **n** random values from a collection.
  // If **n** is not specified, returns a single random element.
  // The internal `guard` argument allows it to work with `map`.
  _.sample = function(obj, n, guard) {
    if (n == null || guard) {
      if (!isArrayLike(obj)) obj = _.values(obj);
      return obj[_.random(obj.length - 1)];
    }
    return _.shuffle(obj).slice(0, Math.max(0, n));
  };

  // Sort the object's values by a criterion produced by an iteratee.
  _.sortBy = function(obj, iteratee, context) {
    iteratee = cb(iteratee, context);
    return _.pluck(_.map(obj, function(value, index, list) {
      return {
        value: value,
        index: index,
        criteria: iteratee(value, index, list)
      };
    }).sort(function(left, right) {
      var a = left.criteria;
      var b = right.criteria;
      if (a !== b) {
        if (a > b || a === void 0) return 1;
        if (a < b || b === void 0) return -1;
      }
      return left.index - right.index;
    }), 'value');
  };

  // An internal function used for aggregate "group by" operations.
  var group = function(behavior) {
    return function(obj, iteratee, context) {
      var result = {};
      iteratee = cb(iteratee, context);
      _.each(obj, function(value, index) {
        var key = iteratee(value, index, obj);
        behavior(result, value, key);
      });
      return result;
    };
  };

  // Groups the object's values by a criterion. Pass either a string attribute
  // to group by, or a function that returns the criterion.
  _.groupBy = group(function(result, value, key) {
    if (_.has(result, key)) result[key].push(value); else result[key] = [value];
  });

  // Indexes the object's values by a criterion, similar to `groupBy`, but for
  // when you know that your index values will be unique.
  _.indexBy = group(function(result, value, key) {
    result[key] = value;
  });

  // Counts instances of an object that group by a certain criterion. Pass
  // either a string attribute to count by, or a function that returns the
  // criterion.
  _.countBy = group(function(result, value, key) {
    if (_.has(result, key)) result[key]++; else result[key] = 1;
  });

  // Safely create a real, live array from anything iterable.
  _.toArray = function(obj) {
    if (!obj) return [];
    if (_.isArray(obj)) return slice.call(obj);
    if (isArrayLike(obj)) return _.map(obj, _.identity);
    return _.values(obj);
  };

  // Return the number of elements in an object.
  _.size = function(obj) {
    if (obj == null) return 0;
    return isArrayLike(obj) ? obj.length : _.keys(obj).length;
  };

  // Split a collection into two arrays: one whose elements all satisfy the given
  // predicate, and one whose elements all do not satisfy the predicate.
  _.partition = function(obj, predicate, context) {
    predicate = cb(predicate, context);
    var pass = [], fail = [];
    _.each(obj, function(value, key, obj) {
      (predicate(value, key, obj) ? pass : fail).push(value);
    });
    return [pass, fail];
  };

  // Array Functions
  // ---------------

  // Get the first element of an array. Passing **n** will return the first N
  // values in the array. Aliased as `head` and `take`. The **guard** check
  // allows it to work with `_.map`.
  _.first = _.head = _.take = function(array, n, guard) {
    if (array == null) return void 0;
    if (n == null || guard) return array[0];
    return _.initial(array, array.length - n);
  };

  // Returns everything but the last entry of the array. Especially useful on
  // the arguments object. Passing **n** will return all the values in
  // the array, excluding the last N.
  _.initial = function(array, n, guard) {
    return slice.call(array, 0, Math.max(0, array.length - (n == null || guard ? 1 : n)));
  };

  // Get the last element of an array. Passing **n** will return the last N
  // values in the array.
  _.last = function(array, n, guard) {
    if (array == null) return void 0;
    if (n == null || guard) return array[array.length - 1];
    return _.rest(array, Math.max(0, array.length - n));
  };

  // Returns everything but the first entry of the array. Aliased as `tail` and `drop`.
  // Especially useful on the arguments object. Passing an **n** will return
  // the rest N values in the array.
  _.rest = _.tail = _.drop = function(array, n, guard) {
    return slice.call(array, n == null || guard ? 1 : n);
  };

  // Trim out all falsy values from an array.
  _.compact = function(array) {
    return _.filter(array, _.identity);
  };

  // Internal implementation of a recursive `flatten` function.
  var flatten = function(input, shallow, strict, startIndex) {
    var output = [], idx = 0;
    for (var i = startIndex || 0, length = getLength(input); i < length; i++) {
      var value = input[i];
      if (isArrayLike(value) && (_.isArray(value) || _.isArguments(value))) {
        //flatten current level of array or arguments object
        if (!shallow) value = flatten(value, shallow, strict);
        var j = 0, len = value.length;
        output.length += len;
        while (j < len) {
          output[idx++] = value[j++];
        }
      } else if (!strict) {
        output[idx++] = value;
      }
    }
    return output;
  };

  // Flatten out an array, either recursively (by default), or just one level.
  _.flatten = function(array, shallow) {
    return flatten(array, shallow, false);
  };

  // Return a version of the array that does not contain the specified value(s).
  _.without = function(array) {
    return _.difference(array, slice.call(arguments, 1));
  };

  // Produce a duplicate-free version of the array. If the array has already
  // been sorted, you have the option of using a faster algorithm.
  // Aliased as `unique`.
  _.uniq = _.unique = function(array, isSorted, iteratee, context) {
    if (!_.isBoolean(isSorted)) {
      context = iteratee;
      iteratee = isSorted;
      isSorted = false;
    }
    if (iteratee != null) iteratee = cb(iteratee, context);
    var result = [];
    var seen = [];
    for (var i = 0, length = getLength(array); i < length; i++) {
      var value = array[i],
          computed = iteratee ? iteratee(value, i, array) : value;
      if (isSorted) {
        if (!i || seen !== computed) result.push(value);
        seen = computed;
      } else if (iteratee) {
        if (!_.contains(seen, computed)) {
          seen.push(computed);
          result.push(value);
        }
      } else if (!_.contains(result, value)) {
        result.push(value);
      }
    }
    return result;
  };

  // Produce an array that contains the union: each distinct element from all of
  // the passed-in arrays.
  _.union = function() {
    return _.uniq(flatten(arguments, true, true));
  };

  // Produce an array that contains every item shared between all the
  // passed-in arrays.
  _.intersection = function(array) {
    var result = [];
    var argsLength = arguments.length;
    for (var i = 0, length = getLength(array); i < length; i++) {
      var item = array[i];
      if (_.contains(result, item)) continue;
      for (var j = 1; j < argsLength; j++) {
        if (!_.contains(arguments[j], item)) break;
      }
      if (j === argsLength) result.push(item);
    }
    return result;
  };

  // Take the difference between one array and a number of other arrays.
  // Only the elements present in just the first array will remain.
  _.difference = function(array) {
    var rest = flatten(arguments, true, true, 1);
    return _.filter(array, function(value){
      return !_.contains(rest, value);
    });
  };

  // Zip together multiple lists into a single array -- elements that share
  // an index go together.
  _.zip = function() {
    return _.unzip(arguments);
  };

  // Complement of _.zip. Unzip accepts an array of arrays and groups
  // each array's elements on shared indices
  _.unzip = function(array) {
    var length = array && _.max(array, getLength).length || 0;
    var result = Array(length);

    for (var index = 0; index < length; index++) {
      result[index] = _.pluck(array, index);
    }
    return result;
  };

  // Converts lists into objects. Pass either a single array of `[key, value]`
  // pairs, or two parallel arrays of the same length -- one of keys, and one of
  // the corresponding values.
  _.object = function(list, values) {
    var result = {};
    for (var i = 0, length = getLength(list); i < length; i++) {
      if (values) {
        result[list[i]] = values[i];
      } else {
        result[list[i][0]] = list[i][1];
      }
    }
    return result;
  };

  // Generator function to create the findIndex and findLastIndex functions
  function createPredicateIndexFinder(dir) {
    return function(array, predicate, context) {
      predicate = cb(predicate, context);
      var length = getLength(array);
      var index = dir > 0 ? 0 : length - 1;
      for (; index >= 0 && index < length; index += dir) {
        if (predicate(array[index], index, array)) return index;
      }
      return -1;
    };
  }

  // Returns the first index on an array-like that passes a predicate test
  _.findIndex = createPredicateIndexFinder(1);
  _.findLastIndex = createPredicateIndexFinder(-1);

  // Use a comparator function to figure out the smallest index at which
  // an object should be inserted so as to maintain order. Uses binary search.
  _.sortedIndex = function(array, obj, iteratee, context) {
    iteratee = cb(iteratee, context, 1);
    var value = iteratee(obj);
    var low = 0, high = getLength(array);
    while (low < high) {
      var mid = Math.floor((low + high) / 2);
      if (iteratee(array[mid]) < value) low = mid + 1; else high = mid;
    }
    return low;
  };

  // Generator function to create the indexOf and lastIndexOf functions
  function createIndexFinder(dir, predicateFind, sortedIndex) {
    return function(array, item, idx) {
      var i = 0, length = getLength(array);
      if (typeof idx == 'number') {
        if (dir > 0) {
            i = idx >= 0 ? idx : Math.max(idx + length, i);
        } else {
            length = idx >= 0 ? Math.min(idx + 1, length) : idx + length + 1;
        }
      } else if (sortedIndex && idx && length) {
        idx = sortedIndex(array, item);
        return array[idx] === item ? idx : -1;
      }
      if (item !== item) {
        idx = predicateFind(slice.call(array, i, length), _.isNaN);
        return idx >= 0 ? idx + i : -1;
      }
      for (idx = dir > 0 ? i : length - 1; idx >= 0 && idx < length; idx += dir) {
        if (array[idx] === item) return idx;
      }
      return -1;
    };
  }

  // Return the position of the first occurrence of an item in an array,
  // or -1 if the item is not included in the array.
  // If the array is large and already in sort order, pass `true`
  // for **isSorted** to use binary search.
  _.indexOf = createIndexFinder(1, _.findIndex, _.sortedIndex);
  _.lastIndexOf = createIndexFinder(-1, _.findLastIndex);

  // Generate an integer Array containing an arithmetic progression. A port of
  // the native Python `range()` function. See
  // [the Python documentation](http://docs.python.org/library/functions.html#range).
  _.range = function(start, stop, step) {
    if (stop == null) {
      stop = start || 0;
      start = 0;
    }
    step = step || 1;

    var length = Math.max(Math.ceil((stop - start) / step), 0);
    var range = Array(length);

    for (var idx = 0; idx < length; idx++, start += step) {
      range[idx] = start;
    }

    return range;
  };

  // Function (ahem) Functions
  // ------------------

  // Determines whether to execute a function as a constructor
  // or a normal function with the provided arguments
  var executeBound = function(sourceFunc, boundFunc, context, callingContext, args) {
    if (!(callingContext instanceof boundFunc)) return sourceFunc.apply(context, args);
    var self = baseCreate(sourceFunc.prototype);
    var result = sourceFunc.apply(self, args);
    if (_.isObject(result)) return result;
    return self;
  };

  // Create a function bound to a given object (assigning `this`, and arguments,
  // optionally). Delegates to **ECMAScript 5**'s native `Function.bind` if
  // available.
  _.bind = function(func, context) {
    if (nativeBind && func.bind === nativeBind) return nativeBind.apply(func, slice.call(arguments, 1));
    if (!_.isFunction(func)) throw new TypeError('Bind must be called on a function');
    var args = slice.call(arguments, 2);
    var bound = function() {
      return executeBound(func, bound, context, this, args.concat(slice.call(arguments)));
    };
    return bound;
  };

  // Partially apply a function by creating a version that has had some of its
  // arguments pre-filled, without changing its dynamic `this` context. _ acts
  // as a placeholder, allowing any combination of arguments to be pre-filled.
  _.partial = function(func) {
    var boundArgs = slice.call(arguments, 1);
    var bound = function() {
      var position = 0, length = boundArgs.length;
      var args = Array(length);
      for (var i = 0; i < length; i++) {
        args[i] = boundArgs[i] === _ ? arguments[position++] : boundArgs[i];
      }
      while (position < arguments.length) args.push(arguments[position++]);
      return executeBound(func, bound, this, this, args);
    };
    return bound;
  };

  // Bind a number of an object's methods to that object. Remaining arguments
  // are the method names to be bound. Useful for ensuring that all callbacks
  // defined on an object belong to it.
  _.bindAll = function(obj) {
    var i, length = arguments.length, key;
    if (length <= 1) throw new Error('bindAll must be passed function names');
    for (i = 1; i < length; i++) {
      key = arguments[i];
      obj[key] = _.bind(obj[key], obj);
    }
    return obj;
  };

  // Memoize an expensive function by storing its results.
  _.memoize = function(func, hasher) {
    var memoize = function(key) {
      var cache = memoize.cache;
      var address = '' + (hasher ? hasher.apply(this, arguments) : key);
      if (!_.has(cache, address)) cache[address] = func.apply(this, arguments);
      return cache[address];
    };
    memoize.cache = {};
    return memoize;
  };

  // Delays a function for the given number of milliseconds, and then calls
  // it with the arguments supplied.
  _.delay = function(func, wait) {
    var args = slice.call(arguments, 2);
    return setTimeout(function(){
      return func.apply(null, args);
    }, wait);
  };

  // Defers a function, scheduling it to run after the current call stack has
  // cleared.
  _.defer = _.partial(_.delay, _, 1);

  // Returns a function, that, when invoked, will only be triggered at most once
  // during a given window of time. Normally, the throttled function will run
  // as much as it can, without ever going more than once per `wait` duration;
  // but if you'd like to disable the execution on the leading edge, pass
  // `{leading: false}`. To disable execution on the trailing edge, ditto.
  _.throttle = function(func, wait, options) {
    var context, args, result;
    var timeout = null;
    var previous = 0;
    if (!options) options = {};
    var later = function() {
      previous = options.leading === false ? 0 : _.now();
      timeout = null;
      result = func.apply(context, args);
      if (!timeout) context = args = null;
    };
    return function() {
      var now = _.now();
      if (!previous && options.leading === false) previous = now;
      var remaining = wait - (now - previous);
      context = this;
      args = arguments;
      if (remaining <= 0 || remaining > wait) {
        if (timeout) {
          clearTimeout(timeout);
          timeout = null;
        }
        previous = now;
        result = func.apply(context, args);
        if (!timeout) context = args = null;
      } else if (!timeout && options.trailing !== false) {
        timeout = setTimeout(later, remaining);
      }
      return result;
    };
  };

  // Returns a function, that, as long as it continues to be invoked, will not
  // be triggered. The function will be called after it stops being called for
  // N milliseconds. If `immediate` is passed, trigger the function on the
  // leading edge, instead of the trailing.
  _.debounce = function(func, wait, immediate) {
    var timeout, args, context, timestamp, result;

    var later = function() {
      var last = _.now() - timestamp;

      if (last < wait && last >= 0) {
        timeout = setTimeout(later, wait - last);
      } else {
        timeout = null;
        if (!immediate) {
          result = func.apply(context, args);
          if (!timeout) context = args = null;
        }
      }
    };

    return function() {
      context = this;
      args = arguments;
      timestamp = _.now();
      var callNow = immediate && !timeout;
      if (!timeout) timeout = setTimeout(later, wait);
      if (callNow) {
        result = func.apply(context, args);
        context = args = null;
      }

      return result;
    };
  };

  // Returns the first function passed as an argument to the second,
  // allowing you to adjust arguments, run code before and after, and
  // conditionally execute the original function.
  _.wrap = function(func, wrapper) {
    return _.partial(wrapper, func);
  };

  // Returns a negated version of the passed-in predicate.
  _.negate = function(predicate) {
    return function() {
      return !predicate.apply(this, arguments);
    };
  };

  // Returns a function that is the composition of a list of functions, each
  // consuming the return value of the function that follows.
  _.compose = function() {
    var args = arguments;
    var start = args.length - 1;
    return function() {
      var i = start;
      var result = args[start].apply(this, arguments);
      while (i--) result = args[i].call(this, result);
      return result;
    };
  };

  // Returns a function that will only be executed on and after the Nth call.
  _.after = function(times, func) {
    return function() {
      if (--times < 1) {
        return func.apply(this, arguments);
      }
    };
  };

  // Returns a function that will only be executed up to (but not including) the Nth call.
  _.before = function(times, func) {
    var memo;
    return function() {
      if (--times > 0) {
        memo = func.apply(this, arguments);
      }
      if (times <= 1) func = null;
      return memo;
    };
  };

  // Returns a function that will be executed at most one time, no matter how
  // often you call it. Useful for lazy initialization.
  _.once = _.partial(_.before, 2);

  // Object Functions
  // ----------------

  // Keys in IE < 9 that won't be iterated by `for key in ...` and thus missed.
  var hasEnumBug = !{toString: null}.propertyIsEnumerable('toString');
  var nonEnumerableProps = ['valueOf', 'isPrototypeOf', 'toString',
                      'propertyIsEnumerable', 'hasOwnProperty', 'toLocaleString'];

  function collectNonEnumProps(obj, keys) {
    var nonEnumIdx = nonEnumerableProps.length;
    var constructor = obj.constructor;
    var proto = (_.isFunction(constructor) && constructor.prototype) || ObjProto;

    // Constructor is a special case.
    var prop = 'constructor';
    if (_.has(obj, prop) && !_.contains(keys, prop)) keys.push(prop);

    while (nonEnumIdx--) {
      prop = nonEnumerableProps[nonEnumIdx];
      if (prop in obj && obj[prop] !== proto[prop] && !_.contains(keys, prop)) {
        keys.push(prop);
      }
    }
  }

  // Retrieve the names of an object's own properties.
  // Delegates to **ECMAScript 5**'s native `Object.keys`
  _.keys = function(obj) {
    if (!_.isObject(obj)) return [];
    if (nativeKeys) return nativeKeys(obj);
    var keys = [];
    for (var key in obj) if (_.has(obj, key)) keys.push(key);
    // Ahem, IE < 9.
    if (hasEnumBug) collectNonEnumProps(obj, keys);
    return keys;
  };

  // Retrieve all the property names of an object.
  _.allKeys = function(obj) {
    if (!_.isObject(obj)) return [];
    var keys = [];
    for (var key in obj) keys.push(key);
    // Ahem, IE < 9.
    if (hasEnumBug) collectNonEnumProps(obj, keys);
    return keys;
  };

  // Retrieve the values of an object's properties.
  _.values = function(obj) {
    var keys = _.keys(obj);
    var length = keys.length;
    var values = Array(length);
    for (var i = 0; i < length; i++) {
      values[i] = obj[keys[i]];
    }
    return values;
  };

  // Returns the results of applying the iteratee to each element of the object
  // In contrast to _.map it returns an object
  _.mapObject = function(obj, iteratee, context) {
    iteratee = cb(iteratee, context);
    var keys =  _.keys(obj),
          length = keys.length,
          results = {},
          currentKey;
      for (var index = 0; index < length; index++) {
        currentKey = keys[index];
        results[currentKey] = iteratee(obj[currentKey], currentKey, obj);
      }
      return results;
  };

  // Convert an object into a list of `[key, value]` pairs.
  _.pairs = function(obj) {
    var keys = _.keys(obj);
    var length = keys.length;
    var pairs = Array(length);
    for (var i = 0; i < length; i++) {
      pairs[i] = [keys[i], obj[keys[i]]];
    }
    return pairs;
  };

  // Invert the keys and values of an object. The values must be serializable.
  _.invert = function(obj) {
    var result = {};
    var keys = _.keys(obj);
    for (var i = 0, length = keys.length; i < length; i++) {
      result[obj[keys[i]]] = keys[i];
    }
    return result;
  };

  // Return a sorted list of the function names available on the object.
  // Aliased as `methods`
  _.functions = _.methods = function(obj) {
    var names = [];
    for (var key in obj) {
      if (_.isFunction(obj[key])) names.push(key);
    }
    return names.sort();
  };

  // Extend a given object with all the properties in passed-in object(s).
  _.extend = createAssigner(_.allKeys);

  // Assigns a given object with all the own properties in the passed-in object(s)
  // (https://developer.mozilla.org/docs/Web/JavaScript/Reference/Global_Objects/Object/assign)
  _.extendOwn = _.assign = createAssigner(_.keys);

  // Returns the first key on an object that passes a predicate test
  _.findKey = function(obj, predicate, context) {
    predicate = cb(predicate, context);
    var keys = _.keys(obj), key;
    for (var i = 0, length = keys.length; i < length; i++) {
      key = keys[i];
      if (predicate(obj[key], key, obj)) return key;
    }
  };

  // Return a copy of the object only containing the whitelisted properties.
  _.pick = function(object, oiteratee, context) {
    var result = {}, obj = object, iteratee, keys;
    if (obj == null) return result;
    if (_.isFunction(oiteratee)) {
      keys = _.allKeys(obj);
      iteratee = optimizeCb(oiteratee, context);
    } else {
      keys = flatten(arguments, false, false, 1);
      iteratee = function(value, key, obj) { return key in obj; };
      obj = Object(obj);
    }
    for (var i = 0, length = keys.length; i < length; i++) {
      var key = keys[i];
      var value = obj[key];
      if (iteratee(value, key, obj)) result[key] = value;
    }
    return result;
  };

   // Return a copy of the object without the blacklisted properties.
  _.omit = function(obj, iteratee, context) {
    if (_.isFunction(iteratee)) {
      iteratee = _.negate(iteratee);
    } else {
      var keys = _.map(flatten(arguments, false, false, 1), String);
      iteratee = function(value, key) {
        return !_.contains(keys, key);
      };
    }
    return _.pick(obj, iteratee, context);
  };

  // Fill in a given object with default properties.
  _.defaults = createAssigner(_.allKeys, true);

  // Creates an object that inherits from the given prototype object.
  // If additional properties are provided then they will be added to the
  // created object.
  _.create = function(prototype, props) {
    var result = baseCreate(prototype);
    if (props) _.extendOwn(result, props);
    return result;
  };

  // Create a (shallow-cloned) duplicate of an object.
  _.clone = function(obj) {
    if (!_.isObject(obj)) return obj;
    return _.isArray(obj) ? obj.slice() : _.extend({}, obj);
  };

  // Invokes interceptor with the obj, and then returns obj.
  // The primary purpose of this method is to "tap into" a method chain, in
  // order to perform operations on intermediate results within the chain.
  _.tap = function(obj, interceptor) {
    interceptor(obj);
    return obj;
  };

  // Returns whether an object has a given set of `key:value` pairs.
  _.isMatch = function(object, attrs) {
    var keys = _.keys(attrs), length = keys.length;
    if (object == null) return !length;
    var obj = Object(object);
    for (var i = 0; i < length; i++) {
      var key = keys[i];
      if (attrs[key] !== obj[key] || !(key in obj)) return false;
    }
    return true;
  };


  // Internal recursive comparison function for `isEqual`.
  var eq = function(a, b, aStack, bStack) {
    // Identical objects are equal. `0 === -0`, but they aren't identical.
    // See the [Harmony `egal` proposal](http://wiki.ecmascript.org/doku.php?id=harmony:egal).
    if (a === b) return a !== 0 || 1 / a === 1 / b;
    // A strict comparison is necessary because `null == undefined`.
    if (a == null || b == null) return a === b;
    // Unwrap any wrapped objects.
    if (a instanceof _) a = a._wrapped;
    if (b instanceof _) b = b._wrapped;
    // Compare `[[Class]]` names.
    var className = toString.call(a);
    if (className !== toString.call(b)) return false;
    switch (className) {
      // Strings, numbers, regular expressions, dates, and booleans are compared by value.
      case '[object RegExp]':
      // RegExps are coerced to strings for comparison (Note: '' + /a/i === '/a/i')
      case '[object String]':
        // Primitives and their corresponding object wrappers are equivalent; thus, `"5"` is
        // equivalent to `new String("5")`.
        return '' + a === '' + b;
      case '[object Number]':
        // `NaN`s are equivalent, but non-reflexive.
        // Object(NaN) is equivalent to NaN
        if (+a !== +a) return +b !== +b;
        // An `egal` comparison is performed for other numeric values.
        return +a === 0 ? 1 / +a === 1 / b : +a === +b;
      case '[object Date]':
      case '[object Boolean]':
        // Coerce dates and booleans to numeric primitive values. Dates are compared by their
        // millisecond representations. Note that invalid dates with millisecond representations
        // of `NaN` are not equivalent.
        return +a === +b;
    }

    var areArrays = className === '[object Array]';
    if (!areArrays) {
      if (typeof a != 'object' || typeof b != 'object') return false;

      // Objects with different constructors are not equivalent, but `Object`s or `Array`s
      // from different frames are.
      var aCtor = a.constructor, bCtor = b.constructor;
      if (aCtor !== bCtor && !(_.isFunction(aCtor) && aCtor instanceof aCtor &&
                               _.isFunction(bCtor) && bCtor instanceof bCtor)
                          && ('constructor' in a && 'constructor' in b)) {
        return false;
      }
    }
    // Assume equality for cyclic structures. The algorithm for detecting cyclic
    // structures is adapted from ES 5.1 section 15.12.3, abstract operation `JO`.

    // Initializing stack of traversed objects.
    // It's done here since we only need them for objects and arrays comparison.
    aStack = aStack || [];
    bStack = bStack || [];
    var length = aStack.length;
    while (length--) {
      // Linear search. Performance is inversely proportional to the number of
      // unique nested structures.
      if (aStack[length] === a) return bStack[length] === b;
    }

    // Add the first object to the stack of traversed objects.
    aStack.push(a);
    bStack.push(b);

    // Recursively compare objects and arrays.
    if (areArrays) {
      // Compare array lengths to determine if a deep comparison is necessary.
      length = a.length;
      if (length !== b.length) return false;
      // Deep compare the contents, ignoring non-numeric properties.
      while (length--) {
        if (!eq(a[length], b[length], aStack, bStack)) return false;
      }
    } else {
      // Deep compare objects.
      var keys = _.keys(a), key;
      length = keys.length;
      // Ensure that both objects contain the same number of properties before comparing deep equality.
      if (_.keys(b).length !== length) return false;
      while (length--) {
        // Deep compare each member
        key = keys[length];
        if (!(_.has(b, key) && eq(a[key], b[key], aStack, bStack))) return false;
      }
    }
    // Remove the first object from the stack of traversed objects.
    aStack.pop();
    bStack.pop();
    return true;
  };

  // Perform a deep comparison to check if two objects are equal.
  _.isEqual = function(a, b) {
    return eq(a, b);
  };

  // Is a given array, string, or object empty?
  // An "empty" object has no enumerable own-properties.
  _.isEmpty = function(obj) {
    if (obj == null) return true;
    if (isArrayLike(obj) && (_.isArray(obj) || _.isString(obj) || _.isArguments(obj))) return obj.length === 0;
    return _.keys(obj).length === 0;
  };

  // Is a given value a DOM element?
  _.isElement = function(obj) {
    return !!(obj && obj.nodeType === 1);
  };

  // Is a given value an array?
  // Delegates to ECMA5's native Array.isArray
  _.isArray = nativeIsArray || function(obj) {
    return toString.call(obj) === '[object Array]';
  };

  // Is a given variable an object?
  _.isObject = function(obj) {
    var type = typeof obj;
    return type === 'function' || type === 'object' && !!obj;
  };

  // Add some isType methods: isArguments, isFunction, isString, isNumber, isDate, isRegExp, isError.
  _.each(['Arguments', 'Function', 'String', 'Number', 'Date', 'RegExp', 'Error'], function(name) {
    _['is' + name] = function(obj) {
      return toString.call(obj) === '[object ' + name + ']';
    };
  });

  // Define a fallback version of the method in browsers (ahem, IE < 9), where
  // there isn't any inspectable "Arguments" type.
  if (!_.isArguments(arguments)) {
    _.isArguments = function(obj) {
      return _.has(obj, 'callee');
    };
  }

  // Optimize `isFunction` if appropriate. Work around some typeof bugs in old v8,
  // IE 11 (#1621), and in Safari 8 (#1929).
  if (typeof /./ != 'function' && typeof Int8Array != 'object') {
    _.isFunction = function(obj) {
      return typeof obj == 'function' || false;
    };
  }

  // Is a given object a finite number?
  _.isFinite = function(obj) {
    return isFinite(obj) && !isNaN(parseFloat(obj));
  };

  // Is the given value `NaN`? (NaN is the only number which does not equal itself).
  _.isNaN = function(obj) {
    return _.isNumber(obj) && obj !== +obj;
  };

  // Is a given value a boolean?
  _.isBoolean = function(obj) {
    return obj === true || obj === false || toString.call(obj) === '[object Boolean]';
  };

  // Is a given value equal to null?
  _.isNull = function(obj) {
    return obj === null;
  };

  // Is a given variable undefined?
  _.isUndefined = function(obj) {
    return obj === void 0;
  };

  // Shortcut function for checking if an object has a given property directly
  // on itself (in other words, not on a prototype).
  _.has = function(obj, key) {
    return obj != null && hasOwnProperty.call(obj, key);
  };

  // Utility Functions
  // -----------------

  // Run Underscore.js in *noConflict* mode, returning the `_` variable to its
  // previous owner. Returns a reference to the Underscore object.
  _.noConflict = function() {
    root._ = previousUnderscore;
    return this;
  };

  // Keep the identity function around for default iteratees.
  _.identity = function(value) {
    return value;
  };

  // Predicate-generating functions. Often useful outside of Underscore.
  _.constant = function(value) {
    return function() {
      return value;
    };
  };

  _.noop = function(){};

  _.property = property;

  // Generates a function for a given object that returns a given property.
  _.propertyOf = function(obj) {
    return obj == null ? function(){} : function(key) {
      return obj[key];
    };
  };

  // Returns a predicate for checking whether an object has a given set of
  // `key:value` pairs.
  _.matcher = _.matches = function(attrs) {
    attrs = _.extendOwn({}, attrs);
    return function(obj) {
      return _.isMatch(obj, attrs);
    };
  };

  // Run a function **n** times.
  _.times = function(n, iteratee, context) {
    var accum = Array(Math.max(0, n));
    iteratee = optimizeCb(iteratee, context, 1);
    for (var i = 0; i < n; i++) accum[i] = iteratee(i);
    return accum;
  };

  // Return a random integer between min and max (inclusive).
  _.random = function(min, max) {
    if (max == null) {
      max = min;
      min = 0;
    }
    return min + Math.floor(Math.random() * (max - min + 1));
  };

  // A (possibly faster) way to get the current timestamp as an integer.
  _.now = Date.now || function() {
    return new Date().getTime();
  };

   // List of HTML entities for escaping.
  var escapeMap = {
    '&': '&amp;',
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": '&#x27;',
    '`': '&#x60;'
  };
  var unescapeMap = _.invert(escapeMap);

  // Functions for escaping and unescaping strings to/from HTML interpolation.
  var createEscaper = function(map) {
    var escaper = function(match) {
      return map[match];
    };
    // Regexes for identifying a key that needs to be escaped
    var source = '(?:' + _.keys(map).join('|') + ')';
    var testRegexp = RegExp(source);
    var replaceRegexp = RegExp(source, 'g');
    return function(string) {
      string = string == null ? '' : '' + string;
      return testRegexp.test(string) ? string.replace(replaceRegexp, escaper) : string;
    };
  };
  _.escape = createEscaper(escapeMap);
  _.unescape = createEscaper(unescapeMap);

  // If the value of the named `property` is a function then invoke it with the
  // `object` as context; otherwise, return it.
  _.result = function(object, property, fallback) {
    var value = object == null ? void 0 : object[property];
    if (value === void 0) {
      value = fallback;
    }
    return _.isFunction(value) ? value.call(object) : value;
  };

  // Generate a unique integer id (unique within the entire client session).
  // Useful for temporary DOM ids.
  var idCounter = 0;
  _.uniqueId = function(prefix) {
    var id = ++idCounter + '';
    return prefix ? prefix + id : id;
  };

  // By default, Underscore uses ERB-style template delimiters, change the
  // following template settings to use alternative delimiters.
  _.templateSettings = {
    evaluate    : /<%([\s\S]+?)%>/g,
    interpolate : /<%=([\s\S]+?)%>/g,
    escape      : /<%-([\s\S]+?)%>/g
  };

  // When customizing `templateSettings`, if you don't want to define an
  // interpolation, evaluation or escaping regex, we need one that is
  // guaranteed not to match.
  var noMatch = /(.)^/;

  // Certain characters need to be escaped so that they can be put into a
  // string literal.
  var escapes = {
    "'":      "'",
    '\\':     '\\',
    '\r':     'r',
    '\n':     'n',
    '\u2028': 'u2028',
    '\u2029': 'u2029'
  };

  var escaper = /\\|'|\r|\n|\u2028|\u2029/g;

  var escapeChar = function(match) {
    return '\\' + escapes[match];
  };

  // JavaScript micro-templating, similar to John Resig's implementation.
  // Underscore templating handles arbitrary delimiters, preserves whitespace,
  // and correctly escapes quotes within interpolated code.
  // NB: `oldSettings` only exists for backwards compatibility.
  _.template = function(text, settings, oldSettings) {
    if (!settings && oldSettings) settings = oldSettings;
    settings = _.defaults({}, settings, _.templateSettings);

    // Combine delimiters into one regular expression via alternation.
    var matcher = RegExp([
      (settings.escape || noMatch).source,
      (settings.interpolate || noMatch).source,
      (settings.evaluate || noMatch).source
    ].join('|') + '|$', 'g');

    // Compile the template source, escaping string literals appropriately.
    var index = 0;
    var source = "__p+='";
    text.replace(matcher, function(match, escape, interpolate, evaluate, offset) {
      source += text.slice(index, offset).replace(escaper, escapeChar);
      index = offset + match.length;

      if (escape) {
        source += "'+\n((__t=(" + escape + "))==null?'':_.escape(__t))+\n'";
      } else if (interpolate) {
        source += "'+\n((__t=(" + interpolate + "))==null?'':__t)+\n'";
      } else if (evaluate) {
        source += "';\n" + evaluate + "\n__p+='";
      }

      // Adobe VMs need the match returned to produce the correct offest.
      return match;
    });
    source += "';\n";

    // If a variable is not specified, place data values in local scope.
    if (!settings.variable) source = 'with(obj||{}){\n' + source + '}\n';

    source = "var __t,__p='',__j=Array.prototype.join," +
      "print=function(){__p+=__j.call(arguments,'');};\n" +
      source + 'return __p;\n';

    try {
      var render = new Function(settings.variable || 'obj', '_', source);
    } catch (e) {
      e.source = source;
      throw e;
    }

    var template = function(data) {
      return render.call(this, data, _);
    };

    // Provide the compiled source as a convenience for precompilation.
    var argument = settings.variable || 'obj';
    template.source = 'function(' + argument + '){\n' + source + '}';

    return template;
  };

  // Add a "chain" function. Start chaining a wrapped Underscore object.
  _.chain = function(obj) {
    var instance = _(obj);
    instance._chain = true;
    return instance;
  };

  // OOP
  // ---------------
  // If Underscore is called as a function, it returns a wrapped object that
  // can be used OO-style. This wrapper holds altered versions of all the
  // underscore functions. Wrapped objects may be chained.

  // Helper function to continue chaining intermediate results.
  var result = function(instance, obj) {
    return instance._chain ? _(obj).chain() : obj;
  };

  // Add your own custom functions to the Underscore object.
  _.mixin = function(obj) {
    _.each(_.functions(obj), function(name) {
      var func = _[name] = obj[name];
      _.prototype[name] = function() {
        var args = [this._wrapped];
        push.apply(args, arguments);
        return result(this, func.apply(_, args));
      };
    });
  };

  // Add all of the Underscore functions to the wrapper object.
  _.mixin(_);

  // Add all mutator Array functions to the wrapper.
  _.each(['pop', 'push', 'reverse', 'shift', 'sort', 'splice', 'unshift'], function(name) {
    var method = ArrayProto[name];
    _.prototype[name] = function() {
      var obj = this._wrapped;
      method.apply(obj, arguments);
      if ((name === 'shift' || name === 'splice') && obj.length === 0) delete obj[0];
      return result(this, obj);
    };
  });

  // Add all accessor Array functions to the wrapper.
  _.each(['concat', 'join', 'slice'], function(name) {
    var method = ArrayProto[name];
    _.prototype[name] = function() {
      return result(this, method.apply(this._wrapped, arguments));
    };
  });

  // Extracts the result from a wrapped and chained object.
  _.prototype.value = function() {
    return this._wrapped;
  };

  // Provide unwrapping proxy for some methods used in engine operations
  // such as arithmetic and JSON stringification.
  _.prototype.valueOf = _.prototype.toJSON = _.prototype.value;

  _.prototype.toString = function() {
    return '' + this._wrapped;
  };

  // AMD registration happens at the end for compatibility with AMD loaders
  // that may not enforce next-turn semantics on modules. Even though general
  // practice for AMD registration is to be anonymous, underscore registers
  // as a named module because, like jQuery, it is a base library that is
  // popular enough to be bundled in a third party lib, but not be part of
  // an AMD load request. Those cases could generate an error when an
  // anonymous define() is called outside of a loader request.
  if (typeof define === 'function' && define.amd) {
    define('underscore', [], function() {
      return _;
    });
  }
}.call(this));

},{}]},{},[9]);
