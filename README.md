# CoinJoinAnalysis

Analyzing CoinJoin Transactions

# Explanation

https://bitcoin.stackexchange.com/a/92652/26859

# Example Output 1

```
Provide inputs or transaction ID! Example: 21,12,36,28.1 or 0f9f3b68f369b3b95779284d4d0607cb8f5051055c2e1b1813848370496e95aa
21,12,36,28.1
Provide outputs! Example: 25,8,50,14.1
25,8,50,14.1

Sub mappings:
21,12,36,28.1 -> 25,8,50,14.1
21,12 -> 25,8 | 36,28.1 -> 50,14.1

Input match probabilities:
21 - inputs: 12(1) 36(0.5) 28.1(0.5) | outputs: 25(1) 8(1) 50(0.5) 14.1(0.5)
12 - inputs: 21(1) 36(0.5) 28.1(0.5) | outputs: 25(1) 8(1) 50(0.5) 14.1(0.5)
36 - inputs: 21(0.5) 12(0.5) 28.1(1) | outputs: 25(0.5) 8(0.5) 50(1) 14.1(1)
28.1 - inputs: 21(0.5) 12(0.5) 36(1) | outputs: 25(0.5) 8(0.5) 50(1) 14.1(1)

Output match probabilities:
25 - inputs: 8(1) 50(0.5) 14.1(0.5) | outputs: 21(1) 12(1) 36(0.5) 28.1(0.5)
8 - inputs: 25(1) 50(0.5) 14.1(0.5) | outputs: 21(1) 12(1) 36(0.5) 28.1(0.5)
50 - inputs: 25(0.5) 8(0.5) 14.1(1) | outputs: 21(0.5) 12(0.5) 36(1) 28.1(1)
14.1 - inputs: 25(0.5) 8(0.5) 50(1) | outputs: 21(0.5) 12(0.5) 36(1) 28.1(1)

Press a key to exit...
```

# Example Output 2

```
Provide inputs or transaction ID! Example: 21,12,36,28.1 or 0f9f3b68f369b3b95779284d4d0607cb8f5051055c2e1b1813848370496e95aa
0f9f3b68f369b3b95779284d4d0607cb8f5051055c2e1b1813848370496e95aa

Sub mappings:
0.00050000 -> 0.00049059

Input match probabilities:
0.00050000 - inputs:  | outputs: 0.00049059(1)

Output match probabilities:
0.00049059 - inputs:  | outputs: 0.00050000(1)

Press a key to exit...
```
