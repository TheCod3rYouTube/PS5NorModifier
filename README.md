# 🎮️ PS5 Nor Modifier

The PS5 NOR Modifier is a Windows GUI-based application making it easier for people to modify the NOR file for their PlayStation 5 console. This is especially useful if you need to generate a NOR file for your console to replace a corrupt or faulty NOR.

## 🛠️ Install

- Download compiled versions under the [releases section](https://github.com/TheCod3rYouTube/PS5NorModifier/releases/tag/release)
- If you just want to use the software, download the **standalone ZIP** version.

## 🖼️ Screenshots

![PS5 NOR Modifier](https://raw.githubusercontent.com/thecod3ryoutube/PS5NorModifier/main/Screenshot1.png)

![PS5 NOR Modifier](https://raw.githubusercontent.com/thecod3ryoutube/PS5NorModifier/main/Screenshot2.png)

![PS5 NOR Modifier](https://raw.githubusercontent.com/thecod3ryoutube/PS5NorModifier/main/Screenshot3.png)

## ❓ Explanation

Some symptoms that your NOR is corrupt or faulty:

1. No power at all. The console draws 8 mA of current or 0 mA of current when connecting to a bench power supply
2. 3 beeps of death. The console beeps 3 times when you plug in the main power cord
3. Cannot update the console

This is also useful if you cannot update the console because the disc drive IC is either faulty, or if you don't have a matching disc drive for your motherboard. You can convert your NOR dump from a disc edition to digital edition to "fool" the PS5 during
an update. This will allow an update to go through and will not harm the console. If you do somehow get the disc drive working at a later date you would need to convert the console back, along with the matching serial number.

## ✨ Features

General:

1. Read PS5 NOR files (PS5 Slim not currently supported)
2. Write PS5 NOR files (PS5 Slim not currently supported)
3. Change serial number on NOR file
4. Change version flags (disc edition or digital edition)
5. Read motherboard serial
6. Read WiFi MAC address
7. Read LAN MAC address

UART:

1. Connect to any compatible TTL device
2. Read error codes stored on PS5 system
3. Clear error codes stored on PS5 system
4. Automatically convert the error codes into easy to understand descriptions (if they exist in the database)
5. Fetch error codes in real time from uartcodes.com database
6. Option to download the entire uartcodes.com database for offline use
7. Send custom UART commands and receive response in output window

## ⚙️ System Requirements

- Operating system: Windows 7, 8, 10, 11
- Hard Drive: 5MB
- Memory: 128MB
- CPU: Any X86 or X64 based CPU
- Software: Microsoft .NET 6.0 Desktop Runtime (v6.0.29)
- Internet connection when using online

## 🤝 Contributing

**Contributions are welcome!**
If you have improvements, ideas, or fixes feel free to open an issue or a pull request.

## 📄 License

This project is licensed under the [MIT License](https://opensource.org/license/mit).
