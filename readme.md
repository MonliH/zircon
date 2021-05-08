# Zircon 💎

Zircon is a purely functional language with no mutable state.
It is interpreted via [lazy evaluation](https://en.wikipedia.org/wiki/Lazy_evaluation), à la Haskell.

I made the language with haskell in mind, but it's minimal syntax 
and lack of many built-ins makes it feel more like lisp without parentheses:


```python
let fib_seq = 0 : 1 : (zip_with (+) fib_seq (tail fib_seq))

print $ index fib_seq 5
# 
```

## Build & usage
Install the [dotnet build tools](https://dotnet.microsoft.com/). Then:

```
git clone https://github.com/MonliH/zircon-lang.git
dotnet build -c Release
```

To boot the REPL:
```
$ ./ZirconLang/bin/Release/net5.0/ZirconLang
zλ> 
```

To execute a file:
```
$ ./ZirconLang/bin/Release/net5.0/ZirconLang Examples/fib.zn
832040
```

You can also add `net5.0/` to your PATH for convenience.

## Examples

For more, see the [examples](https://github.com/MonLiH/zircon-lang/tree/master/Examples) folder.
