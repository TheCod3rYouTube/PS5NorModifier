namespace NorModifierLib.Data;

/// <summary>
/// Represents a NOR error.
/// </summary>
/// <param name="errorBytes">The raw bytes of the error as read from the device.</param>
public class NorError(byte[] errorBytes)
{
	/// <summary>
	/// The error code description.
	/// </summary>
	public string Code => GetErrorCodeDescription(RawCode);
	/// <summary>
	/// The raw error code value.
	/// </summary>
	public readonly uint RawCode = BitConverter.ToUInt32(errorBytes.AsSpan()[0..4]);

	/// <summary>
	/// Unknown, labled as RTC in documentation.
	/// </summary>
	public readonly uint Rtc = BitConverter.ToUInt32(errorBytes.AsSpan()[4..8]);

	// There are two 'halves' of the power state.
	// 0x00AA00BB - where AA is the first half and BB is the second half.
	// To split these into uint16, we need to mask the first half with 0x00FF0000 and shift it right by 16 bits.
	// For the second half, we can simply mask everything but the least significant byte.
	// 
	// We do this instead of having two separate uint16 so that the RawPowerState can be used elsewhere

	/// <summary>
	/// The description for the first half of the power state.
	/// </summary>
	public string PowerStateA => GetPowerStateDescriptionA((RawPowerState & 0x00FF0000) >> 16);
	
	/// <summary>
	/// The description for the second half of the power state.
	/// </summary>
	public string PowerStateB => _powerStateDescriptionsB.TryGetValue(RawPowerState & 0x000000FF, out var description) ? description : "Unknown Power State";
	public readonly uint RawPowerState = BitConverter.ToUInt32(errorBytes.AsSpan()[8..12]);

	/// <summary>
	/// The description for the boot cause.
	/// </summary>
	public string BootCause => _bootCauseDescriptions.TryGetValue(RawBootCause, out var description) ? description : "Unknown Boot Cause";
	public readonly uint RawBootCause = BitConverter.ToUInt32(errorBytes.AsSpan()[12..16]);

	/// <summary>
	/// The description for the sequence number.
	/// </summary>
	public string SequenceNumber => _sequenceDescriptions.TryGetValue(RawSequenceNumber, out var description) ? description : "Unknown Sequence Number";
	public readonly ushort RawSequenceNumber = BitConverter.ToUInt16(errorBytes.AsSpan()[16..18]);

	/// <summary>
	/// The HDMI power state at the time the error was logged.
	/// </summary>
	public bool HdmiPower => (RawDevicePowerManagement & 0x10) != 0;
	/// <summary>
	/// The BluRay disk drive power state at the time the error was logged.
	/// </summary>
	public bool BddPower => (RawDevicePowerManagement & 0x08) != 0;
	/// <summary>
	/// The HDMI-CEC power state at the time the error was logged.
	/// </summary>
	public bool HdmiCecPower => (RawDevicePowerManagement & 0x04) != 0;
	/// <summary>
	/// The USB power state at the time the error was logged.
	/// </summary>
	public bool UsbPower => (RawDevicePowerManagement & 0x02) != 0;
	/// <summary>
	/// The WiFi power state at the time the error was logged.
	/// </summary>
	public bool WifiPower => (RawDevicePowerManagement & 0x01) != 0;
	public readonly ushort RawDevicePowerManagement = BitConverter.ToUInt16(errorBytes.AsSpan()[18..20]);

	/// <summary>
	/// The SoC temperature at the time the error was logged.
	/// </summary>
	public string ChipTemperature => $"{RawChipTemperature / 256.0f:F2}°C";
	public readonly ushort RawChipTemperature = BitConverter.ToUInt16(errorBytes.AsSpan()[20..22]);

	/// <summary>
	/// The environment temperature at the time the error was logged.
	/// </summary>
	public string EnvironmentTemperature => $"{RawEnvironmentTemperature / 256.0f:F2}°C";
	public readonly ushort RawEnvironmentTemperature = BitConverter.ToUInt16(errorBytes.AsSpan()[22..24]);

	// Unused - padding
	// private byte[] _padding => errorBytes[24..32];

	public static implicit operator string(NorError error) => error.ToString();

	public override string ToString()
	{
		return Enumerable.Range(0, (errorBytes.Length + 4 - 1) / 4)
			.Select(x => Convert.ToHexString([.. errorBytes.Skip(x * 4).Take(4)]))
			.Aggregate((current, next) => current + " " + next);
	}

	/// <summary>
	/// Retrieves the description of the error code from the dictionary.
	/// </summary>
	/// <param name="errorCode">The raw error code</param>
	/// <returns>Description for the error code if it is known</returns>
	private static string GetErrorCodeDescription(uint errorCode)
	{
		// Check if the error code is in the dictionary
		if (_errorCodeDescriptions.TryGetValue(errorCode, out var description))
		{
			return description;
		}

		// Truncate 2 of the least significant bytes and check the second dictionary
		if (_secondaryErrorCodeDescriptions.TryGetValue(errorCode >> 16, out var secondaryDescription))
		{
			return secondaryDescription;
		}

		return "Unknown Error Code";
	}

	/// <summary>
	/// Lookup for when the entire error code is known
	/// </summary>
	private static readonly Dictionary<ulong, string> _errorCodeDescriptions = new()
	{
		{ 0x80000001, "Thermal Sensor Fail - NaN SOC" },
		{ 0x80000004, "AC/DC Power Fail" },
		{ 0x80000005, "Main SoC CPU Power Fail" },
		{ 0x80000006, "Main SoC GFX Power Fail" },
		{ 0x80000007, "Main SoC Thrm High Temperature Abnormality" },
		{ 0x80000008, "Drive Dead Notify Timeout" },
		{ 0x80000009, "AC In Detect(12v)" },
		{ 0x8000000A, "VRM HOT Fatal" },
		{ 0x8000000B, "Unexpected Thermal Shutdown in state that Fatal OFF is not allowed" },
		{ 0x8000000C, "MSoC Temperature Alert" },
		{ 0x80000024, "MEMIO(2) Init FAIL(SoC) (?)" },
		{ 0x80050000, "VRM CPU (2)" },
		{ 0x80060000, "VRM GPU(6)" },
		{ 0x80810001, "FORCE_Fatal_Off - PSQ Error" },
		{ 0x80810002, "PSQ NVS Access Error" },
		{ 0x80810013, "PSQ ScCmd DRAM Init Error" },
		{ 0x80810014, "PSQ ScCmd Link Up Failure" },
		{ 0x80830000, "Power Group 2 Init Fail (?)" },
		{ 0x80870001, "Titania RAM Protect Error" },
		{ 0x80870002, "Titania RAM Parity Error" },
		{ 0x80870003, "Titania Boot Failed : Couldn't read Chip Revision." },
		{ 0x80870004, "Titania Boot Failed : Couldn't read error information." },
		{ 0x80870005, "Titania Boot Failed : State Error" },
		{ 0x808D0000, "Thermal Shutdown : Main SoC" },
		{ 0x808D0001, "Thermal Shutdown : Local Sensor 1" },
		{ 0x808D0002, "Thermal Shutdown : Local Sensor 2" },
		{ 0x808D0003, "Thermal Shutdown : Local Sensor 3" },
		{ 0x808E0000, "EAP_Fail (SSD_CON)" },
		{ 0x808E0001, "EAP_Fail (SSD_CON)" },
		{ 0x808E0002, "EAP_Fail (SSD_CON)" },
		{ 0x808E0003, "EAP_Fail (SSD_CON)" },
		{ 0x808E0004, "EAP_Fail (SSD_CON)" },
		{ 0x808E0005, "EAP_Fail (SSD_CON) - Sig 1" },
		{ 0x808E0006, "EAP_Fail (SSD_CON)" },
		{ 0x808E0007, "EAP_Fail (SSD_CON)" },
		{ 0x808F0001, "SMCU (SSD_CON > EMC) (?)" },
		{ 0x808F0002, "SMCU (SSD_CON > EMC) (?)" },
		{ 0x808F0003, "SMCU (SSD_CON > EMC) (?)" },
		{ 0x808F00FF, "SMCU (SSD_CON > EMC) (?)" },
		{ 0x80C00114, "WatchDog For SoC" },
		{ 0x80C00115, "WatchDog For EAP" },
		{ 0x80C0012C, "BD Drive Detached" },
		{ 0x80C0012D, "EMC Watch Dog Timer Error" },
		{ 0x80C0012E, "ADC Error (Button)" },
		{ 0x80C0012F, "ADC Error (BD Drive)" },
		{ 0x80C00130, "ADC Error (AC In Det)" },
		{ 0x80C00131, "USB Over Current" },
		{ 0x80C00132, "FAN Storage Access Failed" },
		{ 0x80C00133, "USB-BT FW Header Invalid Header" },
		{ 0x80C00134, "USB-BT BT Command Error" },
		{ 0x80C00135, "USB-BT Memory Malloc Failed" },
		{ 0x80C00136, "USB-BT Device Not Found" },
		{ 0x80C00137, "USB-BT MISC Error" },
		{ 0x80C00138, "Titania Interrupt HW Error" },
		{ 0x80C00139, "BD Drive Eject Assert Delayed" },
		{ 0x80801101, "RAM GDDR6 1" },
		{ 0x80801102, "RAM GDDR6 2" },
		{ 0x80801103, "RAM GDDR6 1 2" },
		{ 0x80801104, "RAM GDDR6 3" },
		{ 0x80801105, "RAM GDDR6 1 3" },
		{ 0x80801106, "RAM GDDR6 2 3" },
		{ 0x80801107, "RAM GDDR6 1 2 3" },
		{ 0x80801108, "RAM GDDR6 4" },
		{ 0x80801109, "RAM GDDR6 1 4" },
		{ 0x8080110A, "RAM GDDR6 2 4" },
		{ 0x8080110B, "RAM GDDR6 1 2 4" },
		{ 0x8080110C, "RAM GDDR6 3 4" },
		{ 0x8080110D, "RAM GDDR6 1 3 4" },
		{ 0x8080110E, "RAM GDDR6 2 3 4" },
		{ 0x8080110F, "RAM GDDR6 1 2 3 4" },
		{ 0x80801110, "RAM GDDR6 5" },
		{ 0x80801111, "RAM GDDR6 1 5" },
		{ 0x80801112, "RAM GDDR6 2 5" },
		{ 0x80801113, "RAM GDDR6 1 2 5" },
		{ 0x80801114, "RAM GDDR6 3 5" },
		{ 0x80801115, "RAM GDDR6 1 3 5" },
		{ 0x80801116, "RAM GDDR6 2 3 5" },
		{ 0x80801117, "RAM GDDR6 1 2 3 5" },
		{ 0x80801118, "RAM GDDR6 4 5" },
		{ 0x80801119, "RAM GDDR6 1 4 5" },
		{ 0x8080111A, "RAM GDDR6 2 4 5" },
		{ 0x8080111B, "RAM GDDR6 1 2 4 5" },
		{ 0x8080111C, "RAM GDDR6 3 4 5" },
		{ 0x8080111D, "RAM GDDR6 1 3 4 5" },
		{ 0x8080111E, "RAM GDDR6 2 3 4 5" },
		{ 0x8080111F, "RAM GDDR6 1 2 3 4 5" },
		// { 0xFFFFFFFF, "No Error" },
	};

	/// <summary>
	/// More 'generic' error codes, where the two least significant bytes are not relevant.
	/// </summary>
	private static readonly Dictionary<uint, string> _secondaryErrorCodeDescriptions = new()
	{
		{ 0x8005, "VRM CPU (2) (?)" }, 
		{ 0x8006, "VRM GPU(6) (?)" }, 
		{ 0x8080, "Fatal Shutdown by OS request" }, 
		{ 0x8087, "Titania ScCmd Response Error" }, // Actually 808710XX
		{ 0x8088, "Titania Boot EAP Error" }, // Actually 8088X[A-F]{3}
		{ 0x8089, "Titania Boot EFC Error" }, // Actually 8089X[A-F]{3}
		{ 0x808A, "Titania Temperature Error" }, 
		{ 0x808B, "Titania Watch Dog Timer" }, 
		{ 0x808C, "USB Type-C Error" }, 
		{ 0x8090, "Fatal Shutdown - OS CRASH" }, 
		{ 0x8091, "SSD PMIC Error" }, 
		{ 0xC001, "Main SoC Access Error (I2C)" }, 
		{ 0xC002, "Main SoC Access Error (SB-TSI I2C)" }, 
		{ 0xC003, "Main SoC Access Error (SB-RMI)" }, 
		{ 0xC00B, "Serial Flash Access Error" }, 
		{ 0xC00C, "VRM Controller Access Error" }, 
		{ 0xC00D, "PMIC (Subsystem) Access Error" }, 
		{ 0xC010, "Flash Controller Access Error" }, 
		{ 0xC011, "Potentiometer Access Error" }, 
		{ 0xC015, "PCIe Access Error" }, 
		{ 0xC016, "PMIC (SSD) Access Error" }, 
		{ 0xC081, "HDMI Tx Access Error" }, 
		{ 0xC090, "USB Type-C PD Controller Access Error" }, 
		{ 0xC091, "USB Type-C USB/DP Mux Access Error" }, 
		{ 0xC092, "USB Type-C Redriver Access Error" }, 
		{ 0xC0FE, "Dummy" }, 
	};

	/// <summary>
	/// Retrieves the descirption for the first half of the power state from the dictionary.
	/// </summary>
	/// <param name="powerState">The raw power state</param>
	/// <returns>Description for the power state if it is known</returns>
	private static string GetPowerStateDescriptionA(uint powerState)
	{
		// Check if the power state is in the dictionary
		if (_powerStateDescriptionsA.TryGetValue(powerState, out var description))
		{
			return description;
		}

		// Mask the least significant nibble and check the secondary lookup
		if (_powerStateDesciprtionsSecondaryA.TryGetValue(powerState & 0xF0, out var secondaryDescription))
		{
			return secondaryDescription;
		}

		return "Unknown Power State";
	}

	/// <summary>
	/// Lookup for the power state
	/// </summary>
	private static readonly Dictionary<uint, string> _powerStateDescriptionsA = new() {
		{ 0x00, "SysReady" }, 
		{ 0x01, "MaOnStby" }, 
		{ 0x20, "NOR" }, 
		{ 0x30, "NOR" }, 
		{ 0x40, "EAP_Rdy" }, 
		{ 0xFF, "HstOsOFF" }, 
	};

	/// <summary>
	/// Certain ranges of power states are not well defined, so we use a secondary lookup to get the description.
	/// The least significant nibble is masked out, and the remaining bits are used.
	/// </summary>
	private static readonly Dictionary<uint, string> _powerStateDesciprtionsSecondaryA = new() {
		//0x10-1F - PSP
		{ 0x10, "PSP" }, 

		//0x50-BF - Kernel
		{ 0x50, "Kernel" },
		{ 0x60, "Kernel" },
		{ 0x70, "Kernel" },
		{ 0x80, "Kernel" },
		{ 0x90, "Kernel" },
		{ 0xA0, "Kernel" },
		{ 0xB0, "Kernel" },

		//0xC0-FE - IntPrcss
		{ 0xC0, "IntPrcss" },
		{ 0xD0, "IntPrcss" },
		{ 0xE0, "IntPrcss" },
		{ 0xF0, "IntPrcss" },
	};

	/// <summary>
	/// Lookup for the second half of the power state
	/// </summary>
	private static readonly Dictionary<uint, string> _powerStateDescriptionsB = new() {
		{ 0x00, "ACIN_L Before Standby" },
		{ 0x01, "STANDBY" },
		{ 0x02, "PG2_ON" },
		{ 0x03, "EFC_ON" },
		{ 0x04, "EAP_ON" },
		{ 0x05, "SOC_ON" },
		{ 0x06, "ERROR_DET" },
		{ 0x07, "FATAL_ERRO" },
		{ 0x08, "NEVER_BOOT" },
		{ 0x09, "FORCE_OFF" },
		{ 0x0A, "FORCE_OFF BT Firmware Download" }
	};

	/// <summary>
	/// Lookup for the boot cause
	/// </summary>
	private static readonly Dictionary<uint, string> _bootCauseDescriptions = new()
	{
		{ 0x40000000, "DEV UART" },
		{ 0x00080000, "BT (Bluetooth)" },
		{ 0x00040000, "HDMI-CEC" },
		{ 0x00020000, "EAP" },
		{ 0x00010000, "SoC" },
		{ 0x00000400, "Eject Button" },
		{ 0x00000200, "Disc Loaded" },
		{ 0x00000100, "Power Button" },
		{ 0x00000001, "Boot-Up at power-on" }
	};

	/// <summary>
	/// Lookup for the sequence number
	/// </summary>
	private static readonly Dictionary<uint, string> _sequenceDescriptions = new() {
		{ 0x2002, "EmcBootup" },
		{ 0x2067, "EmcBootup" },
		{ 0x2064, "EmcBootup, FATAL OFF" },
		{ 0x218E, "EmcBootup" },
		{ 0x2003, "Subsystem Peripheral Initialize" },
		{ 0x2005, "Subsystem Peripheral Initialize" },
		{ 0x2004, "Subsystem Peripheral Initialize" },
		{ 0x2008, "aEmcTimerIniti" },
		{ 0x2009, "aEmcTimerIniti" },
		{ 0x200A, "aEmcTimerIniti" },
		{ 0x200B, "aEmcTimerIniti" },
		{ 0x200C, "aPowerGroup2On 1" },
		{ 0x2109, "aPowerGroup2On 1" },
		{ 0x200D, "aPowerGroup2On 1" },
		{ 0x2011, "aPowerGroup2On 1" },
		{ 0x200E, "aPowerGroup2On 1, Subsystem PG2 reset" },
		{ 0x200F, "aPowerGroup2On 1" },
		{ 0x2010, "aPowerGroup2On 1, Subsystem PG2 reset" },
		{ 0x202E, "aPowerGroup2On 1, Subsystem PG2 reset" },
		{ 0x2006, "aPowerGroup2On 1, Subsystem PG2 reset" },
		{ 0x21AF, "aPowerGroup2On 1" },
		{ 0x21B1, "aPowerGroup2On 1" },
		{ 0x2014, "aPowerGroup2Off, Flash Controller OFF EFC, Flash Controller OFF EAP, Flash Controller STOP EFC, Flash Controller STOP EAP, FATAL OFF" },
		{ 0x202F, "aPowerGroup2Off, FATAL OFF" },
		{ 0x2015, "aPowerGroup2Off, FATAL OFF" },
		{ 0x2016, "aPowerGroup2Off, Subsystem PG2 reset, FATAL OFF" },
		{ 0x202B, "aPowerGroup2Off, FATAL OFF" },
		{ 0x2017, "aPowerGroup2Off, FATAL OFF" },
		{ 0x210A, "aPowerGroup2Off, FATAL OFF" },
		{ 0x2018, "aPowerGroup2Off, FATAL OFF" },
		{ 0x2019, "aPowerGroup2Off" },
		{ 0x201A, "aSbPcieInitiali" },
		{ 0x2030, "aSbPcieInitiali, aSbPcieInitiali 1, FATAL OFF" },
		{ 0x2031, "aSbPcieInitiali, aSbPcieInitiali 1, FATAL OFF" },
		{ 0x2066, "aSbPcieInitiali 1" },
		{ 0x208D, "aEfcBootModeSet, EAP Boot Mode Set" },
		{ 0x210B, "aEfcBootModeSet, EAP Boot Mode Set" },
		{ 0x210C, "aEfcBootModeSet, EAP Boot Mode Set" },
		{ 0x210D, "aEfcBootModeSet" },
		{ 0x201D, "Flash Controller ON EFC, Flash Controller ON EAP" },
		{ 0x2027, "Flash Controller ON EFC, Flash Controller ON EAP, Flash Controller Soft reset" },
		{ 0x2110, "Flash Controller ON EFC, Flash Controller ON EAP" },
		{ 0x2033, "Flash Controller ON EFC, Flash Controller ON EAP, Flash Controller Soft reset" },
		{ 0x2089, "Flash Controller ON EFC, Flash Controller ON EAP, Flash Controller Soft reset" },
		{ 0x2035, "Flash Controller ON EFC, Flash Controller ON EAP, Flash Controller Soft reset, FC NAND Close Not urgent, FC NAND Close Urgent" },
		{ 0x201C, "Subsystem PCIe USP Enable" },
		{ 0x2029, "Subsystem PCIe DSP Enable, Subsystem PCIe DSP Enable BT DL" },
		{ 0x2107, "Subsystem PCIe DSP Enable, Dev WLAN BT PCIE RESET NEGATE, Dev WLAN BT PCIE RESET ASSERT NEGATE" },
		{ 0x2159, "Flash Controller Initialization EFC, Flash Controller Initialization EAP" },
		{ 0x2045, "Flash Controller Initialization EFC, Flash Controller Initialization EAP" },
		{ 0x2038, "Flash Controller Initialization EFC" },
		{ 0x2043, "Flash Controller Initialization EFC, Flash Controller Initialization EAP" },
		{ 0x2041, "Flash Controller Initialization EFC, Flash Controller Initialization EAP" },
		{ 0x2047, "Flash Controller Initialization EAP" },
		{ 0x204C, "Flash Controller OFF EFC, Flash Controller STOP EFC" },
		{ 0x2108, "Flash Controller OFF EFC, Flash Controller OFF EAP, Flash Controller STOP EFC, Flash Controller STOP EAP, FATAL OFF, Dev WLAN BT PCIE RESET ASSERT, Dev WLAN BT PCIE RESET ASSERT NEGATE" },
		{ 0x206D, "Flash Controller OFF EFC, Flash Controller OFF EAP, Flash Controller STOP EFC, Flash Controller STOP EAP, FATAL OFF" },
		{ 0x2034, "Flash Controller OFF EFC, Flash Controller OFF EAP, FATAL OFF" },
		{ 0x208A, "Flash Controller OFF EFC, Flash Controller OFF EAP, FATAL OFF" },
		{ 0x210F, "Flash Controller OFF EFC, Flash Controller OFF EAP, FATAL OFF" },
		{ 0x2028, "Flash Controller OFF EFC, Flash Controller OFF EAP, Flash Controller STOP EFC, Flash Controller STOP EAP, FATAL OFF" },
		{ 0x201E, "Flash Controller OFF EFC, Flash Controller OFF EAP, FATAL OFF" },
		{ 0x2046, "Flash Controller OFF EAP, Flash Controller STOP EFC, Flash Controller STOP EAP" },
		{ 0x2048, "Flash Controller STOP EFC, Flash Controller STOP EAP" },
		{ 0x204D, "Flash Controller STOP EAP" },
		{ 0x2049, "Flash Controller SRAM Keep Enable" },
		{ 0x2111, "ACDC 12V ON" },
		{ 0x2113, "ACDC 12V ON" },
		{ 0x2052, "ACDC 12V ON" },
		{ 0x2085, "ACDC 12V ON" },
		{ 0x2054, "ACDC 12V ON" },
		{ 0x2087, "ACDC 12V ON" },
		{ 0x216F, "USB VBUS On, USB VBUS Off, Dev USB VBUS On" },
		{ 0x211B, "USB VBUS On, Dev USB VBUS On" },
		{ 0x211D, "BD Drive Power On, Dev BD Drive Power On" },
		{ 0x203A, "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
		{ 0x203D, "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
		{ 0x2126, "Main SoC Power ON Cold Boot, FATAL OFF" },
		{ 0x2128, "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
		{ 0x212A, "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
		{ 0x2135, "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit, Main SoC Power Off, FATAL OFF, Dev VBURN OFF" },
		{ 0x211F, "Main SoC Power ON Cold Boot, GDDR6 USB Power On" },
		{ 0x2189, "Main SoC Power ON Cold Boot, GDDR6 USB Power On" },
		{ 0x218B, "Main SoC Power ON Cold Boot, GDDR6 USB Power On" },
		{ 0x21B6, "Main SoC Power ON Cold Boot, GDDR6 USB Power On" },
		{ 0x21B8, "Main SoC Power ON Cold Boot, GDDR6 USB Power On" },
		{ 0x21BA, "Main SoC Power ON Cold Boot, GDDR6 USB Power On" },
		{ 0x2023, "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
		{ 0x2125, "Main SoC Power ON Cold Boot, GDDR6 USB Power On" },
		{ 0x2167, "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
		{ 0x21C1, "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
		{ 0x21C3, "Main SoC Power ON Cold Boot" },
		{ 0x2121, "Main SoC Power ON Cold Boot" },
		{ 0x21C5, "Main SoC Power ON Cold Boot" },
		{ 0x2175, "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
		{ 0x2133, "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
		{ 0x2141, "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
		{ 0x205F, "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
		{ 0x218D, "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
		{ 0x21BE, "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit, Main SoC Power Off, FATAL OFF" },
		{ 0x21C0, "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit, Main SoC Power Off, FATAL OFF" },
		{ 0x21C4, "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit, Main SoC Power Off, FATAL OFF" },
		{ 0x2123, "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
		{ 0x2136, "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit, Main SoC Power Off, FATAL OFF" },
		{ 0x2137, "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit, Main SoC Power Off, FATAL OFF" },
		{ 0x216D, "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
		{ 0x2060, "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit, Main SoC Power Off, FATAL OFF" },
		{ 0x2061, "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit, Main SoC Power Off, FATAL OFF" },
		{ 0x2025, "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit, Main SoC Power Off, FATAL OFF" },
		{ 0x2127, "Main SoC Reset Release, Cold reset WA" },
		{ 0x204A, "Main SoC Reset Release" },
		{ 0x2129, "Main SoC Reset Release, Cold reset WA" },
		{ 0x21A3, "Main SoC Reset Release, USB VBUS On 2, Dev USBA1 VBUS On" },
		{ 0x21A5, "Main SoC Reset Release, USB VBUS On 2, Dev USBA2 VBUS On" },
		{ 0x21A7, "Main SoC Reset Release, USB VBUS On 2, Dev USBA3 VBUS On" },
		{ 0x21A9, "Main SoC Reset Release, USB VBUS On 2, Dev USBA1 VBUS On" },
		{ 0x21AB, "Main SoC Reset Release, USB VBUS On 2, Dev USBA2 VBUS On" },
		{ 0x21AD, "Main SoC Reset Release, USB VBUS On 2, Dev USBA3 VBUS On" },
		{ 0x212F, "Main SoC Reset Release, Main SoC Power Off, FATAL OFF" },
		{ 0x2169, "Main SoC Reset Release, Main SoC Power Off, FATAL OFF" },
		{ 0x2161, "Main SoC Reset Release, Main SoC Power Off, FATAL OFF" },
		{ 0x21B3, "Main SoC Reset Release, Main SoC Power Off, FATAL OFF" },
		{ 0x21B5, "Main SoC Reset Release" },
		{ 0x213C, "Main SoC Reset Release, Main SoC Power Off, FATAL OFF, Cold reset WA" },
		{ 0x213D, "Main SoC Reset Release, Main SoC Power Off, FATAL OFF, Cold reset WA" },
		{ 0x213F, "Main SoC Reset Release, Main SoC Power Off, FATAL OFF, Cold reset WA" },
		{ 0x2050, "Main SoC Reset Release, Main SoC Power Off, FATAL OFF, Cold reset WA" },
		{ 0x2083, "Main SoC Reset Release" },
		{ 0x2187, "Main SoC Reset Release" },
		{ 0x2195, "Main SoC Reset Release" },
		{ 0x2197, "Main SoC Reset Release" },
		{ 0x2155, "Main SoC Reset Release" },
		{ 0x205C, "Main SoC Reset Release, Main SoC Power Off, FATAL OFF, Cold reset WA" },
		{ 0x217F, "Main SoC Reset Release, Cold reset WA" },
		{ 0x212B, "MSOC Reset Moni High, Main SoC Power Off, FATAL OFF" },
		{ 0x2157, "MSOC Reset Moni High, Main SoC Power Off, FATAL OFF" },
		{ 0x208F, "Main SoC Power Off, FATAL OFF" },
		{ 0x2040, "Main SoC Power Off, FATAL OFF, FC NAND Close Not urgent" },
		{ 0x2156, "Main SoC Power Off, FATAL OFF" },
		{ 0x2196, "Main SoC Thermal Moni Stop, Main SoC Power Off, FATAL OFF" },
		{ 0x2198, "Main SoC Thermal Moni Stop, Main SoC Power Off, FATAL OFF" },
		{ 0x2188, "Main SoC Thermal Moni Stop, Main SoC Power Off, FATAL OFF" },
		{ 0x2084, "Main SoC Thermal Moni Stop, Main SoC Power Off, FATAL OFF" },
		{ 0x2051, "Main SoC Thermal Moni Stop, Main SoC Power Off, FATAL OFF, Cold reset WA" },
		{ 0x211E, "BD Drive Power Off, FATAL OFF, Dev BD Drive Power Off" },
		{ 0x211C, "USB VBUS Off, FATAL OFF" },
		{ 0x2114, "ACDC 12V Off, FATAL OFF" },
		{ 0x2112, "ACDC 12V Off, FATAL OFF" },
		{ 0x207A, "ACDC 12V Off" },
		{ 0x2086, "ACDC 12V Off, FATAL OFF" },
		{ 0x2053, "ACDC 12V Off, FATAL OFF" },
		{ 0x2088, "ACDC 12V Off, FATAL OFF" },
		{ 0x2055, "ACDC 12V Off, FATAL OFF" },
		{ 0x204B, "FC NAND Close Not urgent, FC NAND Close Urgent, FATAL OFF" },
		{ 0x2042, "FC NAND Close Not urgent, FC NAND Close Urgent" },
		{ 0x2044, "FC NAND Close Not urgent, FC NAND Close Urgent" },
		{ 0x2024, "FATAL OFF" },
		{ 0x2152, "USB OC Moni de assert, FATAL OFF" },
		{ 0x2122, "FATAL OFF" },
		{ 0x21AA, "FATAL OFF, USB OC Moni de assert 2, Dev USBA1 VBUS Off" },
		{ 0x21AC, "FATAL OFF, USB OC Moni de assert 2, Dev USBA2 VBUS Off" },
		{ 0x21AE, "FATAL OFF, USB OC Moni de assert 2, Dev USBA3 VBUS Off" },
		{ 0x21A4, "FATAL OFF, USB VBUS Off 2, Dev USBA1 VBUS Off" },
		{ 0x21A6, "FATAL OFF, USB VBUS Off 2, Dev USBA2 VBUS Off" },
		{ 0x21A8, "FATAL OFF, USB VBUS Off 2, Dev USBA3 VBUS Off" },
		{ 0x218C, "FATAL OFF" },
		{ 0x218A, "FATAL OFF" },
		{ 0x2120, "FATAL OFF" },
		{ 0x2118, "FATAL OFF, Dev HDMI 5V Power Off" },
		{ 0x2073, "FATAL OFF, HDMI CECStop" },
		{ 0x2075, "FATAL OFF, HDMI CECStop, HDMIStop" },
		{ 0x2079, "FATAL OFF, HDMI CECStop" },
		{ 0x2071, "FATAL OFF, HDMI CECStop" },
		{ 0x204F, "FATAL OFF, HDMI CECStop" },
		{ 0x2022, "FATAL OFF, HDMI CECStop" },
		{ 0x2116, "FATAL OFF, HDMI CECStop" },
		{ 0x208C, "FATAL OFF" },
		{ 0x2165, "FATAL OFF" },
		{ 0x2164, "FATAL OFF" },
		{ 0x216C, "FATAL OFF" },
		{ 0x21B2, "FATAL OFF" },
		{ 0x21B0, "FATAL OFF" },
		{ 0x2012, "Stop SFlash DMA, FATAL OFF" },
		{ 0x2091, "Local Temp.3 OFF, FATAL OFF" },
		{ 0x2057, "Local Temp.3 OFF, FATAL OFF" },
		{ 0x217E, "Fan Servo Parameter Reset, FATAL OFF" },
		{ 0x2105, "WLAN Module Reset, FATAL OFF, WM Reset, Dev WLAN BT RESET ASSERT, Dev WLAN BT RESET ASSERT NEGATE" },
		{ 0x2092, "FATAL OFF" },
		{ 0x212D, "EAP Reset Moni de assert" },
		{ 0x212E, "EAP Reset Moni Assert, FATAL OFF" },
		{ 0x205D, "EAP Reset Moni Assert, Main SoC Power Off, FATAL OFF" },
		{ 0x213B, "EAP Reset Moni Assert, Main SoC Power Off, FATAL OFF" },
		{ 0x205E, "FAN CONTROL Parameter Reset" },
		{ 0x2065, "EMC SoC Handshake ST" },
		{ 0x2151, "USB OC Moni Assert" },
		{ 0x2068, "HDMI Standby, HDMIStop" },
		{ 0x2106, "WLAN Module USB Enable, WLAN Module Reset, WM Reset, Dev WLAN BT RESET NEGATE, Dev WLAN BT RESET ASSERT NEGATE" },
		{ 0x217B, "WLAN Module Reset, BT WAKE Disabled, WM Reset, Dev WLAN BT RESET ASSERT, Dev WLAN BT RESET ASSERT NEGATE" },
		{ 0x215A, "1GbE NIC Reset de assert" },
		{ 0x215B, "1GbE NIC Reset assert" },
		{ 0x2115, "HDMI CECStart, CECStart" },
		{ 0x2021, "HDMI CECStart" },
		{ 0x204E, "HDMI CECStart" },
		{ 0x2070, "HDMI CECStart, CECStop" },
		{ 0x2078, "HDMI CECStart, CECStop" },
		{ 0x206E, "HDMI CECStart, CECStart" },
		{ 0x2074, "HDMI CECStart" },
		{ 0x2072, "HDMI CECStart" },
		{ 0x2077, "HDMIStop, CECStop" },
		{ 0x215F, "MDCDC ON" },
		{ 0x2160, "MDCDC Off" },
		{ 0x208E, "Titania2 GPIO Glitch Issue WA" },
		{ 0x216E, "Check AC IN DETECT" },
		{ 0x2170, "Check BD DETECT" },
		{ 0x2173, "GPI SW Open" },
		{ 0x2174, "GPI SW Close" },
		{ 0x2102, "Devkit IO Expander Initialize" },
		{ 0x2177, "Salina PMIC Register Initialize" },
		{ 0x2178, "Disable AC IN DETECT" },
		{ 0x2179, "BT WAKE Enabled" },
		{ 0x2094, "Stop PCIePLL NoSS part" },
		{ 0x217A, "Titania PMIC Register Initialize" },
		{ 0x203B, "Setup FC for BTFW DL" },
		{ 0x2039, "Setup FC for BTFW DL" },
		{ 0x217C, "BTFW Download" },
		{ 0x2095, "Telstar ROM Boot Wait" },
		{ 0x201B, "Stop PCIePLL SS NOSS part, FATAL OFF" },
		{ 0x2082, "Stop PCIePLL SS part" },
		{ 0x2013, "Stop Subsystem PG2 Bus Error Detection(DDR4 BufferOverflow)" },
		{ 0x2056, "Local Temp.3 ON" },
		{ 0x2090, "Local Temp.3 ON" },
		{ 0x2180, "FAN Control Start at Restmode during US" },
		{ 0x2181, "FAN Control Start at Restmode during US" },
		{ 0x2182, "FAN Control Start at Restmode during US" },
		{ 0x2193, "FAN Control Start at Restmode during US" },
		{ 0x2183, "FAN Control Stop at Restmode during USB" },
		{ 0x2184, "FAN Control Stop at Restmode during USB" },
		{ 0x2185, "FAN Control Stop at Restmode during USB" },
		{ 0x2194, "FAN Control Stop at Restmode during USB" },
		{ 0x2186, "Read Titania PMIC Registe" },
		{ 0x219B, "I2C Open" },
		{ 0x219C, "I2C Open" },
		{ 0x219D, "I2C Open" },
		{ 0x219E, "I2C Open" },
		{ 0x2199, "I2C Open" },
		{ 0x219A, "I2C Open" },
		{ 0x21A0, "Drive FAN Control Stop" },
		{ 0x219F, "Drive FAN Control Stop" },
		{ 0x21A1, "Drive FAN Control Start" },
		{ 0x21A2, "Drive FAN Control Start" },
		{ 0x2117, "Dev HDMI 5V Power On" },
		{ 0x2134, "Dev VBURN ON" },
		// { 0xFFFF, "Unknown SeqNo" }
	};
}
