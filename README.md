Simple C Sharp Raytracer
========================

What is this?
-------------

SCSRaytracer (Simple C Sharp Raytracer) does exactly what it says on the tin: it's a ray tracer, and it's written in C#. 
It aims to be a basic ray tracer that both runs on the CPU, and emphasizes code readability over excessive optimization.
The core structure of this ray tracer is based on that of the skeleton ray tracer as provided by Kevin Suffern in his
book Ray Tracing from the Ground Up, 1st edition (source code released under GNU GPLv2). The similarities end at the hit
functions and basic structure, however, as SCSRaytracer has the following features:

* Written in managed C#, as opposed to C++.
* Shading structure restructured for thread safety.
* Multithreaded rendering (up to 8 simultaneous render threads)
* Live render view
* Scene loading through an XML based scene description language.
* Uses .NET 4.6's hardware acceleration for vector math.

How do I build this?
--------------------

SCSRaytracer targets 64-bit Windows, the .NET 4.6 framework, and Visual Studio 2015. Simply git checkout and build from within
Visual Studio. It has the following dependencies (provided)

* SFML.NET
* CSFML
* System.Numerics.Vectors (Note that due to this dependency, Mono is not currently supported)