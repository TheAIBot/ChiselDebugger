# ChiselDebugger

This a debugger made for FIRRTL generated from Chisel. It's currently capable of visualizing a FIRRTL circuit and showing circuit state on top of it. The debugger is currently available either as an application or as a webpage. The app is many times faster than the alternative so please use that one for larger circuits. They both use a webpage as their UI, the difference lies in where the program is executed. The application version runs on your computer like any other program. The webpage version uses webassembly to run the entire program in the browser. Webassembly version may not function correctly as it uses [a preview of .net 6](https://devblogs.microsoft.com/dotnet/announcing-net-6-preview-5/).

## How to run

### Application version
1. Download release for your OS
2. Unzip file and run ```ChiselDebuggerWebUI.exe```
3. Open a webbrowser and go to either http://localhost:5000/ or https://localhost:5001/
4. Top left corner of the webpage has a button called "Load circuit" click on it and select a ```.fir```, ```.lo.fir``` and ```.vcd``` file in the file menu. A ```.lo.fir``` file is not always necessary. Without a ```.vcd``` it will not be possible to show the state of the circuit.
5. The circuit and VCD timeline should now be visible.

### Webpage version
1. Open https://theaibot.github.io/ChiselDebuggerWeb
    * **An exception will occur when loading the webpage version for the first time. It will disappear when the webpage has loaded which may take up to 10 seconds.**
    * **This version is single threaded. This means the webpage may become unresponsive while it draws large circuits.**
3. Top left corner of the webpage has a button called "Load circuit" click on it and select a ```.fir```, ```.lo.fir``` and ```.vcd``` file in the file menu. A ```.lo.fir``` file is not always necessary. Without a ```.vcd``` it will not be possible to show the state of the circuit.
4. Wait for it to place and route everything. UI will not be responsive while this is happening.
5. The circuit and VCD timeline should now be visible.

## Projects
Description of what the individual projects do

* **Benchmarker** Console program that can benchmark various parts of the program.
* **ChiselDebug** Library for everything between parsing to UI.
  * FIRRTL AST to FIRRTL graph
  * Type inference
  * Compute order
  * Placement
  * Routing
  * VCD Timeline
* **ChiselDebugTests** Tests for **ChiselDebug**. Most tests are autogenerated by **ChiselTestGen** and **TestGenerator**.
* **ChiselDebuggerRazor** Contains all UI code shared between the application and webpage version. [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) is the UI framework used.
  * All UI pages
  * Template system
  * Layouts
  * C# <-> Javascript communication
* **ChiselDebuggerWebAsmUI** Webpage version using Webassembly. It uses [a preview of .net 6](https://devblogs.microsoft.com/dotnet/announcing-net-6-preview-5/) because it allows AOT compilation of C# into webassembly. Without the preview the webpage version would be ~5x times slower.
* **ChiselDebuggerWebUI** Application version of the UI.
* **ChiselTestGen** This project generates FIRRTL and VCD files used for tests in **ChiselDebugTests**.
* **FIRRTL** C# FIRRTL parser. It's mainly transpiled from [the FIRRTL compilers parser](https://github.com/chipsalliance/firrtl/blob/master/src/main/scala/firrtl/Parser.scala).
* **FIRRTLTests** Tests for the FIRRTL parser.
* **TestGenerator** Takes FIRRTL and VCD files from **ChiselTestGen** and produces C# tests that can then be executed by **ChiselDebugTests**.
* **VCDReader** C# Value change dump(VCD) parser. It parses the 4-state VCD format described in [IEEE std 1800-2017](https://standards.ieee.org/standard/1800-2017.html).
* **VCDReaderTests** Tests for the VCD parser.

