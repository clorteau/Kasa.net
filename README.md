# Kasa.NET
Control your Kasa TP-Link smart switches from your Windows 10 device.

This is a .NET program to control Kasa TP-Link smart switches from your Windows 10 device. 

I also have a Python-QT version that works on Windows/Mac OS X/Linux but it's not as modern: https://github.com/clorteau/kasa.

To install this Windows 10 version just get it from the [Windows Store](https://www.microsoft.com/store/productId/9MTFWZLLLLG9).

Basically what this does is send a command to the switches by packing the command string length as a Big Endian followed by an Auto-Key XOR of the command. I thought it was interesting to implement.

Screenshots:

![Dark mode](http://www.lorteau.fr/kasa.net/screenshot01.png)

![Light mode](http://www.lorteau.fr/kasa.net/screenshot02.png)
