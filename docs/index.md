---
layout: home
title: The Starfield
---
# The Starfield

## Getting Started

The topics in this section will help you install all of the required tools, become familiar with the test environment, and create your first Starfield plugin.

### Clone the Repo

<a href="https://github.com/volaris/starfield">Clone or download the master branch</a>

### Requirements

<ul>
	<li><a href="https://www.visualstudio.com/downloads/">Visual Studio</a> Community or Pro editions</li>
</ul>

### Building Solutions

<ol>
	<li>Open solution starfield\lib\.NET\StarfieldLibs.sln</li>
	<li>Switch project configuration to Release mode</li>
	<li>Build solution</li>
	<li>You should see this: <pre>========== Build: 2 succeeded, 0 failed, 0 up-to-date, 0 skipped ==========</pre></li>
	<li>Open solution starfield\controllers\StarfieldDrivers\StarfieldDrivers.sln</li>
	<li>Switch project configuration to Release mode</li>
	<li>Build solution</li>
	<li>You should see this: <pre>========== Build: 1 succeeded, 0 failed, 0 up-to-date, 0 skipped ==========</pre></li>
</ol>

### Get the Simulator

Download the simulator
* [Linux](https://github.com/volaris/starfield/raw/master/simulator/builds/Linux.zip)
* [Mac](https://github.com/volaris/starfield/raw/master/simulator/builds/Mac.zip)
* [Windows](https://github.com/volaris/starfield/raw/master/simulator/builds/Windows.zip)

After running the executable, press *Play!* and use the configuration panel to generate the Starfield size that you want to interract with.

![Simulator Config](https://github.com/volaris/starfield/blob/master/documentation/images/SimulatorConfig.png?raw=true)

You can move the camera around in the Simulator with WASD keys.

### Running the Demo

While the Simulator is running, start the demo.

1. Open solution starfield\controllers\AlgorithmDemo\AlgorithmDemo.sln
1. Switch project configuration to Release mode
1. Build solution
1. Run solution
1. Choose a Starfield (Critical NW Starfield)
1. Choose an algorithm (Smooth Rainbow Simplex Noise)

![Demo Config](https://github.com/volaris/starfield/blob/master/documentation/images/AlgorithmDemo.png?raw=true)

### Testing the Client and Server

TBD

### Anatomy of an Algorithm

Starfield algorithms are included in the solution starfield\controllers\StarfieldDrivers\StarfieldDrivers.sln

Let's start with a simple example: TestFill.

```C#
    [DriverType(DriverTypes.Experimental)]
    public class TestFill : IStarfieldDriver
    {
    	...
    }
```

TestFill implements the IStarfieldDriver interface. 

```C#
    public interface IStarfieldDriver
    {
        void Render(StarfieldModel Starfield);

        void Start(StarfieldModel Starfield);

        void Stop();
    }
```

You can read the [IStarfieldDriver documentation](api/html/T_Starfield_IStarfieldDriver.htm). Then take a look at the source of [starfield\controllers\StarfieldDrivers\StarfieldDrivers\Test\TestFill.cs](https://github.com/volaris/starfield/blob/master/controllers/StarfieldDrivers/StarfieldDrivers/Test/TestFill.cs)

### How to Write a Starfield Plugin

Contributing directly to the Starfield project.

1. Open solution starfield\controllers\StarfieldDrivers\StarfieldDrivers.sln
1. Add your new class to one of the categories or create your own category (namespace)
	- Animation
	- CFD
	- Flocking
	- Fractal
	- Noise
	- PresenceResponsive
	- Projection
	- Sound Responsive
	- Test
1. Implement [IStarfieldDriver](api/html/T_Starfield_IStarfieldDriver.htm)
1. Build the project

Create your own Starfield algorithm project. This will allow you to compile a binary library (dll) and include it with an existing Starfield implemenation.

1. Start a new solution
1. Make sure you can [build the solution](#building-solutions)
1. Add a reference to starfield/controllers/bin/Starfield.dll
1. Optionally add a reference to starfield/controllers/bin/Utils.dll if you need extended math features

### Running the Simulator Over the Network

TBD

### Documentation

<ul>
	<li><a href="api/html/R_Project_Documentation.htm">Namespaces</a></li>
</ul>

### Contributing

If you would like to contribute to this guide or the [API documentation](api/html/R_Project_Documentation.htm) follow these steps...

#### This guide
1. [Pull or fork the master branch](https://github.com/volaris/starfield/)
2. [Set up Jekyll](https://jekyllrb.com/docs/quickstart/). You'll need Ruby. Don't forget to `bundle`.
3. Run the local server `jekyll serve` from `starfield/docs`
4. Browse your local server `https://localhost:4000/starfield/` :warning: The trailing slash is important
5. Make edits
6. Commit changes
7. Make a pull request

#### API Docs
1. [Pull or fork the master branch](https://github.com/volaris/starfield/)
2. Make sure you can [build the solution](#building-solutions)
3. TBD

Need help with Markdown? [Basic writing and formatting syntax](https://help.github.com/articles/basic-writing-and-formatting-syntax/)