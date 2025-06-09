Help contribute to the project with a donation: https://www.streamelements.com/thecod3r/tip

News update: This project will continue soon. We've just aquired the domain "uart.codes" and I'm in the process of building a team and a roadmap to really take this project places it's never seen!

The PS5 NOR Modifier is a GUI based application that makes it easier for one to modify the NOR file for their PlayStation 5 console. This is especially useful if you need to generate a NOR file for your console to replace a corrupt or faulty NOR.


Download compiled versions under the releases section.

![PS5 NOR Modifier](https://raw.githubusercontent.com/thecod3ryoutube/PS5NorModifier/main/Screenshot1.png)

![PS5 NOR Modifier](https://raw.githubusercontent.com/thecod3ryoutube/PS5NorModifier/main/Screenshot2.png)

![PS5 NOR Modifier](https://raw.githubusercontent.com/thecod3ryoutube/PS5NorModifier/main/Screenshot3.png)

Some symptoms that your NOR is corrupt or faulty:
1. No power at all. The console draws 8ma of current or 0ma of current when connecting to a bench power supply
2. 3 beeps of death. The console beeps 3 times when you plug in the main power chord
3. Cannot update the console

This is also useful if you cannot update the console because the disc drive IC is either faulty, or if you don't have a matching disc drive for your motherboard. You can convert your NOR dump from a disc edition to digital edition to "fool" the PS5 during
an update. This will allow an update to go through and will not hurt the console. If you do somehow get the disc drive working at a later date you would need to convert the console back, along with the matching serial number.

Features:
1. Read PS5 NOR files (PS5 Slim not currently supported)
2. Write PS5 NOR files (PS5 Slim not currently supported)
3. Change serial number on NOR file
4. Change version flags (disc edition or digital edition)
5. Read motherboard serial
6. Read Wi-Fi MAC address
7. Read LAN MAC address

UART Features:
1. Connect to any compatible TTL device
2. Read error codes stored on PS5 system
3. Clear error codes stored on PS5 system
4. Automatically convert the error codes into easy to understand descriptions (if they exist in the database)
5. Fetch error codes in real time from uartcodes.com database
6. Option to download entire uartcodes.com database for offline use
7. Send custom UART commands and receive response in output box

System Requirements:

Operating system: Windows 10, 11, Linux, macOS

CPU: Any X86- or X64-based CPU

Software: Microsoft .NET 8.0 Desktop Runtime

Internet connection when using online
