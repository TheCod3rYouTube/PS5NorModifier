# 🎮️ PS5 Nor Modifier

---

**Help contribute to the project with a donation: [www.streamelements.com/thecod3r/tip](https://www.streamelements.com/thecod3r/tip)**

**News update:**
 This project will continue soon. We've just acquired the domain "uart.codes" and I'm in the process of building a team and a roadmap to really take this project places it's never seen!

---

A Windows GUI application to simplify NOR file modification for PlayStation 5 consoles.

The PS5 NOR Modifier is a Windows GUI-based application making it easier for people to modify the NOR file for their PlayStation 5 console. This is especially useful if you need to generate a NOR file for your console to replace a corrupt or faulty NOR.

---

## 🛠️ Install

- Download compiled versions under the [releases section](https://github.com/TheCod3rYouTube/PS5NorModifier/releases/tag/release)
- If you just want to use the software, download the **standalone ZIP** version.
  - [Download the Standalone ZIP directly](https://github.com/TheCod3rYouTube/PS5NorModifier/blob/main/PS5%20NOR%20Modifier%20Standalone.zip)

---

## 🖼️ Screenshots

![PS5 NOR Modifier](https://raw.githubusercontent.com/thecod3ryoutube/PS5NorModifier/main/Screenshot1.png)

![PS5 NOR Modifier](https://raw.githubusercontent.com/thecod3ryoutube/PS5NorModifier/main/Screenshot2.png)

![PS5 NOR Modifier](https://raw.githubusercontent.com/thecod3ryoutube/PS5NorModifier/main/Screenshot3.png)

---

## ❓ Explanation

### Signs of a corrupt or faulty NOR:

1. No power at all. The console draws 8 mA of current or 0 mA of current when connecting to a bench power supply
2. 3 beeps of death. The console beeps 3 times when you plug in the main power cord
3. Cannot update the console

### Special use cases:

If your disc drive IC is faulty or you don't have a matching disc drive for your motherboard, you can convert your NOR dump from a disc edition to a digital edition.  
This "fools" the PS5 during an update and allows the update to proceed safely.  
If you later repair or replace the disc drive, you'll need to convert the console back and restore the matching serial number.

---

## ✨ Features

### General:

1. Read PS5 NOR files (PS5 Slim not currently supported)
2. Write PS5 NOR files (PS5 Slim not currently supported)
3. Change serial number on NOR file
4. Change version flags (disc edition or digital edition)
5. Read motherboard serial
6. Read WiFi MAC address
7. Read LAN MAC address

### UART:

1. Connect to any compatible TTL device
2. Read error codes stored on PS5 system
3. Clear error codes stored on PS5 system
4. Automatically convert the error codes into easier to understand descriptions (if they exist in the database)
5. Fetch error codes in real time from uartcodes.com database
6. Option to download the entire uartcodes.com database for offline use
7. Send custom UART commands and receive response in output window

---

## ⚙️ System Requirements

- **Operating System:** Windows 7, 8, 10, 11  
- **Hard Drive:** 5 MB  
- **Memory:** 128 MB  
- **CPU:** Any x86 or x64-based processor  
- **Software:** [.NET 6.0 Desktop Runtime (v6.0.29)](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)  
- **Internet:** Required for real-time features

---

## ⚠️ Important Cautions

- Modifying NOR files can potentially affect console functionality
- Ensure you understand the implications before making changes
- Back up your original NOR file before modifications

---

## 🤝 Contributing

**Contributions are welcome!**

If you have improvements, ideas, or fixes feel free to open an issue or a pull request and your changes will be reviewed.

Friendly reminder: Please check the open Issues and Pull Requests before starting work to avoid overlap.

---

## 📄 License

This project is licensed under the [GPL-2.0 License](https://www.gnu.org/licenses/old-licenses/gpl-2.0.en.html).
