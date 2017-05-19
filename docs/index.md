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

### Running the Demo

TBD

### Testing the Client and Server

TBD

### Anatomy of an Algorithm

TBD

### How to Write a Starfield Plugin

TBD

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