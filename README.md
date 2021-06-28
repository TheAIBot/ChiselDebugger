# ChiselDebugger

This a debugger made for FIRRTL generated from Chisel. It's currently capable of visualizing a FIRRTL circuit and showing circuit state on top of it. The debugger is currently available either as an application or as a webpage. The app is many times faster than the alternative so please use that one for larger circuits. They both use a webpage as their UI, the difference lies in where the program is executed. The application version runs on your computer like any other program. The webpage version uses webassembly to run the entire program in the browser. 

## How to run

### Application version
1. Download release for your OS
2. Unzip file and run ```ChiselDebuggerWebUI.exe```
3. Open a webbrowser and go to either http://localhost:5000/ or https://localhost:5001/
4. Top left corner of the webpage has a button called "Load circuit" click on it and select a ```.fir```, ```.lo.fir``` and ```.vcd``` file in the file menu. A ```.lo.fir``` file is not always necessary. Without a ```.vcd``` it will not be possible to show the state of the circuit.
5. The circuit and VCD timeline should now be visible.

## Webpage version
1. Open https://theaibot.github.io/ChiselDebuggerWeb
2. Top left corner of the webpage has a button called "Load circuit" click on it and select a ```.fir```, ```.lo.fir``` and ```.vcd``` file in the file menu. A ```.lo.fir``` file is not always necessary. Without a ```.vcd``` it will not be possible to show the state of the circuit.
3. The circuit and VCD timeline should now be visible.



