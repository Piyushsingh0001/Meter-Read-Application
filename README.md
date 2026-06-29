# Cabcon MAUI Meter Reading Application

## 📌 Overview

CabconMAUI is a cross-platform meter reading application built using .NET MAUI.
It supports multiple communication protocols and meter types including DLMS, IEC, and IrDA.

The application enables configuration, communication, and data retrieval from smart and non-smart meters with structured output generation (XML/CSV/report formats).

---

## 🚀 Key Features

### ⚙️ Settings Management

* Communication configuration (Serial/Port settings)
* Protocol configuration (HDLC, IEC)
* Security configuration (LLS/HLS, encryption keys)
* Default read configurations
* Persistent settings storage

---

### 🔌 Meter Communication Workflows

* DLMS (1-phase & 3-phase smart meters)
* IEC (Non-DLMS meters)
* IrDA communication flows

---

### 📊 Read Operations

* Read All
* Instantaneous Data
* Billing Data
* Load Survey
* Daily Profile
* Tamper/Event Logs
* Nameplate Data

---

### 📁 Output Generation

* XML generation
* CSV export
* Report-style processing

---

## 🏗️ Application Architecture

### Entry & Routing

* `Program.cs`
* `fromMainLogin.cs`

Routing is based on selected Meter Mode:

* 1 Phase → APP_1PSM
* 3 Phase → APP_3PSM
* IEC → APP_E150IEC

---

## ⚙️ Settings Modules

### 1. Association Settings

* Port settings
* HDLC configuration
* Association parameters
* Conformance block

### 2. Serial Settings

* COM Port
* Communication Mode (Mode-E / Direct-HDLC)
* Optical/RJ Port selection

### 3. System Settings

* RTC format toggle (IS15959)

---

## 💾 Settings Storage

* Uses .NET Settings:

  * `SerialPortSettings`
  * `SystemSettings`
* Persisted via `.settings` files

---

## 🔐 Security Configuration

* LLS Password (min 8 chars)
* HLS Password (min 16 chars)
* AES Encryption support
* Authentication keys and system titles

---

## 📡 Protocol Stack

### DLMS

* HDLC framing
* COSEM APDU handling
* Block transfer support
* Ciphered communication

### IEC

* Command-based communication
* Multi-packet handling
* XML-driven command execution

### IrDA

* Serial-based infrared communication
* Binary/ASCII parsing

---

## 🔄 Read Workflow

### DLMS Flow

1. Open serial connection
2. HDLC negotiation
3. Association (Plain/Ciphered)
4. Execute reads (single/block)
5. Parse data
6. Generate XML output
7. Disconnect

---

### IEC Flow

1. Connect in IEC mode
2. Load command repository
3. Execute commands
4. Parse responses
5. Generate XML

---

### IrDA Flow

1. Open IrDA port
2. Send commands
3. Parse responses
4. Store/display data
5. Disconnect

---

## ⚠️ Validation Rules

* Mandatory field validation
* Password length enforcement
* Read range validation
* Timeout and retry handling

---

## ❗ Error Handling

* Timeout handling
* Retry mechanisms
* Connection failure detection
* Safe disconnect on failure

---

## 🧩 Core Interfaces (Recommended Design)

* `ISettingsService`
* `IMeterConnection`
* `IDlmsReader`
* `IIecReader`
* `IIrdaReader`
* `IReadWorkflowEngine`
* `IReadoutExporter`

---


---

## 🛠️ Technologies Used

* .NET MAUI (.NET 9)
* C#
* Serial Communication APIs
* DLMS/COSEM Protocol
* XML Processing

---

## 📦 Setup Instructions

```bash
git clone https://github.com/your-username/CabconMAUI.git
cd CabconMAUI
```

Open in Visual Studio and run on:

* Android Emulator / Device
* Windows Machine

---

## 📌 Future Enhancements

* Cloud sync support
* Real-time monitoring dashboard
* Device auto-detection
* Improved UI/UX

---

## 👨‍💻 Author

Piyush Singh

---

## 📄 License

This project is for internal/private use.
