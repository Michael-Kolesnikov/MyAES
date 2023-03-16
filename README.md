# MyAES
не является полной версией AES
# Как пользоваться?
```c#
  var key = new byte[256]{...};
  var aes = new AES(key);
  
  var msg = new byte[32]{...};
  byte[] encoding = aes.Encode(msg);
  
  var msg2 = new byte[64]{...};
  byte[] decding = aes.Decode(msg2);
```
