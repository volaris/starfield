Starfield
=========

Software (simulation and control) and electronics for the starfield display.

## Overview

The Starfield is a large grid of color controllable LEDs. They're spaced on vertical strands so that people can walk through them. People will be able to watch it from afar, walk through, or lounge in the space. There will be ambient animations designed to calm and intrigue. Calming music will play throughout the space. At certain times a DJ can connect to the Starfield or it picks up noise in the environment and displays animations that enhance and respond to that sound. IR cameras will detect where people are in the space and how active they are and modify the animations to respond to them. In the future, hand tracking stations will be used to create an individual or collaborative animation looper. Other methods of interaction such as converting EEG patterns to light are being investigated. The first version is a prototype designed to fit in a living room. A 20'x20'x20' version is being constructed for [Critical NW 2015](http://www.criticalnw.org/) and an even larger one is being planned for Burning Man 2016.

## Technical Details

The core of the Starfield is driven by a [Fadecandy](https://github.com/scanlime/fadecandy) running on a [Raspberry Pi](https://www.raspberrypi.org/).

### Prototype

![Prototype Diagram](https://raw.githubusercontent.com/volaris/starfield/master/docs/images/Prototype.png)

### Critical NW



### Burning Man



## Repository Layout
```
\
|-controllers\ _Starfield drivers and control software_
  |-AlgorithmDemo\ _Test controller for showing and tweaking drivers_
  |-StarfieldDrivers\ _Builtin animation implementations_
|-docs\ _Starfield documentation_
|-electronics\ _Electronic designs - power distribution, voxel design, etc._
|-lib\ _Shared code and utility classes_
  |-.NET\ _.NET utility classes_
    |-StarfieldClient\ _OPC and higher level Starfield Model, used to connect and control the Starfield_
    |-Utils\ _Math, color, sound, and other algorithm utilities_
|-mechanical\ _Mechanical engineering designs and calculations for the structure_
|-simulator\ _Unity simulator_
```